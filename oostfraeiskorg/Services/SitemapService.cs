using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace oostfraeiskorg.Services;

/// <summary>
/// Result of a sitemap generation run.
/// </summary>
/// <param name="UrlCount">Number of URLs that were written into the sitemaps.</param>
/// <param name="FileCount">Number of sitemap files that were written.</param>
/// <param name="DurationMilliseconds">Duration of the whole run in milliseconds.</param>
public sealed record SitemapResult(int UrlCount, int FileCount, long DurationMilliseconds);

/// <summary>
/// Builds the XML sitemaps of the site.
/// </summary>
/// <remarks>
/// The dynamic sitemap holds one URL per dictionary entry, so it is far too large to
/// be assembled as a string in memory. The rows are streamed straight from SQLite
/// into the XML writer and every file is swapped in atomically, so a crawler can
/// never download a half written sitemap.
/// </remarks>
public sealed class SitemapService
{
    /// <summary>Public address of the site.</summary>
    private const string SiteUrl = "https://oostfraeisk.org";

    /// <summary>Namespace of the sitemap protocol.</summary>
    private const string SitemapNamespace = "https://www.sitemaps.org/schemas/sitemap/0.9";

    /// <summary>
    /// Maximum number of URLs per sitemap file.
    /// </summary>
    /// <remarks>
    /// The protocol allows 50.000 URLs per file, but we stick to 30.000 so that the
    /// file names (sitemapdynamic1.xml ... sitemapdynamic4.xml) stay the same as
    /// before. Those names are already entered in the Google Search Console and
    /// should not suddenly return a 404.
    /// </remarks>
    private const int MaxUrlsPerSitemap = 30_000;

    /// <summary>
    /// Maximum size of a sitemap file in bytes (protocol limit: 50 MB). Some headroom
    /// is kept so that the closing tags always fit into the file as well.
    /// </summary>
    private const long MaxBytesPerSitemap = 45L * 1024 * 1024;

    /// <summary>Buffer size of the file streams that receive the sitemaps.</summary>
    private const int FileBufferSize = 64 * 1024;

    /// <summary>Name of the sitemap that holds the static pages of the site.</summary>
    private const string FixedSitemapName = "sitemapfixed.xml";

    /// <summary>Name of the sitemap index that lists all sitemaps.</summary>
    private const string SitemapIndexName = "sitemap.xml";

    /// <summary>Prefix of the sitemap files that hold the dictionary entries.</summary>
    private const string DynamicSitemapPrefix = "sitemapdynamic";

    /// <summary>
    /// The XML declaration of the sitemap files. It is written by hand, because the
    /// XML writer would write the encoding in lower case.
    /// </summary>
    private const string XmlDeclaration = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

    /// <summary>The static pages of the site.</summary>
    private static readonly string[] FixedPages =
    [
        "/", "/main", "/translator", "/grammar", "/coursebook", "/onlinecourse",
        "/keyboard", "/downloads", "/about", "/dsgvo", "/links", "/instructions", "/donateaword"
    ];

    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<SitemapService> _logger;
    private readonly object _lock = new();

    public SitemapService(IWebHostEnvironment environment, ILogger<SitemapService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Rebuilds all sitemaps, the sitemap index and robots.txt.
    /// </summary>
    public SitemapResult Generate()
    {
        var stopwatch = Stopwatch.StartNew();

        lock (_lock)
        {
            string webRoot = ResolveWebRoot();
            Directory.CreateDirectory(webRoot);
            RemoveLeftoverTempFiles(webRoot);

            // 1. the static pages of the site
            using (var writer = new SitemapFileWriter(Path.Combine(webRoot, FixedSitemapName), _logger))
            {
                foreach (string page in FixedPages)
                {
                    // the home page is the most important one and gets an explicit priority
                    writer.WriteUrl(SiteUrl + page, page == "/" ? "0.8" : null);
                }
                writer.Complete();
            }

            // 2. one URL per dictionary entry, split over several sitemap files
            (List<string> dynamicFiles, int urlCount) = WriteDynamicSitemaps(webRoot);

            // 3. an index over all sitemaps, so robots.txt never has to be edited by hand
            var sitemapFiles = new List<string> { FixedSitemapName };
            sitemapFiles.AddRange(dynamicFiles);
            WriteSitemapIndex(webRoot, sitemapFiles);

            // 4. robots.txt points at the index and at the single files
            WriteRobotsTxt(webRoot, sitemapFiles);

            // 5. remove the leftovers of an older run that had more files
            RemoveStaleSitemaps(webRoot, dynamicFiles.Count);

            stopwatch.Stop();

            int fileCount = sitemapFiles.Count;
            _logger.LogInformation(
                "Sitemaps neu erstellt: {UrlCount} URLs in {FileCount} Dateien ({Duration} ms)",
                urlCount, fileCount, stopwatch.ElapsedMilliseconds);

            return new SitemapResult(urlCount, fileCount, stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Writes one sitemap file per <see cref="MaxUrlsPerSitemap"/> dictionary entries.
    /// </summary>
    private (List<string> Files, int UrlCount) WriteDynamicSitemaps(string webRoot)
    {
        var files = new List<string>();
        int urlCount = 0;
        int index = 1;

        using var connection = DataBaseConnection.GetConnection(Server.MapPath(""));
        using var command = connection.CreateCommand();

        // DISTINCT: the table contains some entries more than once, they would end up
        // as the same URL several times in the sitemap. Every distinct entry is
        // listed exactly once, the set of URLs stays the same.
        command.CommandText = "SELECT DISTINCT Ostfriesisch FROM WB ORDER BY Ostfriesisch ASC";
        using var reader = command.ExecuteReader();

        SitemapFileWriter writer = null;
        try
        {
            while (reader.Read())
            {
                string entry = reader.GetValue(0).ToString();

                if (writer == null)
                {
                    writer = new SitemapFileWriter(
                        Path.Combine(webRoot, $"{DynamicSitemapPrefix}{index}.xml"), _logger);
                }

                // the entry is escaped by hand, so that the file stays byte for byte
                // the same as the one of the old implementation
                writer.WriteRawUrl($"{SiteUrl}/main?W={EscapeEntry(entry)}&amp;df=frs>de&amp;fts=X");
                urlCount++;

                if (writer.UrlCount >= MaxUrlsPerSitemap || writer.BytesWritten >= MaxBytesPerSitemap)
                {
                    writer.Complete();
                    files.Add($"{DynamicSitemapPrefix}{index}.xml");
                    index++;
                    writer.Dispose();
                    writer = null;
                }
            }

            if (writer != null)
            {
                writer.Complete();
                files.Add($"{DynamicSitemapPrefix}{index}.xml");
            }
        }
        finally
        {
            writer?.Dispose();
        }

        return (files, urlCount);
    }

    /// <summary>
    /// Writes the sitemap index that lists all sitemaps of the site.
    /// </summary>
    private void WriteSitemapIndex(string webRoot, IEnumerable<string> sitemapFiles)
    {
        string finalPath = Path.Combine(webRoot, SitemapIndexName);
        string tempPath = finalPath + ".tmp";

        using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, FileBufferSize))
        {
            using (var writer = XmlWriter.Create(stream, CreateWriterSettings()))
            {
                writer.WriteRaw(XmlDeclaration);
                writer.WriteStartElement("sitemapindex", SitemapNamespace);
                string lastModified = DateTime.UtcNow.ToString("yyyy-MM-dd");
                foreach (string file in sitemapFiles)
                {
                    writer.WriteStartElement("sitemap");
                    writer.WriteStartElement("loc");
                    writer.WriteString($"{SiteUrl}/{file}");
                    writer.WriteEndElement();
                    writer.WriteElementString("lastmod", lastModified);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }
            // the writer does not own the stream, so it has to be flushed and closed here
            stream.Flush();
        }

        // keep the file a proper text file for git
        AppendNewLine(tempPath);

        MoveWithRetry(tempPath, finalPath);
    }

    /// <summary>
    /// Points robots.txt at the sitemap index and at the single sitemap files.
    /// Everything above the sitemap entries is kept, so custom rules of the site
    /// survive the regeneration.
    /// </summary>
    private void WriteRobotsTxt(string webRoot, IEnumerable<string> sitemapFiles)
    {
        string path = Path.Combine(webRoot, "robots.txt");
        var lines = new List<string>();

        if (File.Exists(path))
        {
            foreach (string line in File.ReadAllLines(path))
            {
                if (line.TrimStart().StartsWith("Sitemap:", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                lines.Add(line);
            }
            while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[^1]))
            {
                lines.RemoveAt(lines.Count - 1);
            }
        }

        if (lines.Count == 0)
        {
            lines.Add("User-agent: *");
            lines.Add("Allow: /");
            lines.Add("Crawl-delay: 1");
        }

        lines.Add("");

        // the index is the single entry point for new crawlers
        lines.Add($"Sitemap: {SiteUrl}/{SitemapIndexName}");

        // the single files stay listed as well, so that every sitemap that was
        // known before still resolves - nothing changes for the search engines
        foreach (string file in sitemapFiles)
        {
            lines.Add($"Sitemap: {SiteUrl}/{file}");
        }

        WriteAllTextAtomic(path, string.Join("\n", lines) + "\n");
    }

    /// <summary>
    /// Deletes the sitemap files of an older run that are not needed anymore.
    /// </summary>
    private static void RemoveStaleSitemaps(string webRoot, int generatedCount)
    {
        foreach (string path in Directory.EnumerateFiles(webRoot, $"{DynamicSitemapPrefix}*.xml"))
        {
            string name = Path.GetFileName(path);
            int numberLength = name.Length - DynamicSitemapPrefix.Length - ".xml".Length;
            if (numberLength <= 0)
            {
                continue;
            }
            string number = name.Substring(DynamicSitemapPrefix.Length, numberLength);
            if (int.TryParse(number, out int index) && index > generatedCount)
            {
                DeleteWithRetry(path);
            }
        }
    }

    /// <summary>
    /// Deletes the temporary files of a run that was interrupted.
    /// </summary>
    private static void RemoveLeftoverTempFiles(string webRoot)
    {
        foreach (string path in Directory.EnumerateFiles(webRoot, "*.xml.tmp"))
        {
            DeleteWithRetry(path);
        }
    }

    private string ResolveWebRoot()
    {
        string webRoot = _environment.WebRootPath;
        if (!string.IsNullOrEmpty(webRoot))
        {
            return webRoot;
        }
        return Path.Combine(_environment.ContentRootPath ?? Directory.GetCurrentDirectory(), "wwwroot");
    }

    private static void WriteAllTextAtomic(string path, string content)
    {
        string tempPath = path + ".tmp";
        File.WriteAllText(tempPath, content, new UTF8Encoding(false));
        MoveWithRetry(tempPath, path);
    }

    /// <summary>
    /// Moves a file and replaces an existing one. A file that has just been written
    /// can be locked for a moment by a virus scanner or by a crawler that is
    /// downloading it, in that case we simply try again.
    /// </summary>
    private static void MoveWithRetry(string source, string destination)
    {
        const int attempts = 5;

        for (int attempt = 1; ; attempt++)
        {
            try
            {
                File.Move(source, destination, overwrite: true);
                return;
            }
            catch (Exception ex) when (attempt < attempts && (ex is IOException || ex is UnauthorizedAccessException))
            {
                Thread.Sleep(200 * attempt);
            }
        }
    }

    /// <summary>
    /// Deletes a file. A file that is still being downloaded by a crawler can be
    /// locked for a moment, in that case we simply try again.
    /// </summary>
    private static void DeleteWithRetry(string path)
    {
        const int attempts = 5;

        for (int attempt = 1; ; attempt++)
        {
            try
            {
                File.Delete(path);
                return;
            }
            catch (Exception ex) when (attempt < attempts && (ex is IOException || ex is UnauthorizedAccessException))
            {
                Thread.Sleep(200 * attempt);
            }
        }
    }

    private static XmlWriterSettings CreateWriterSettings() => new()
    {
        Encoding = new UTF8Encoding(false),
        Indent = false,
        // the declaration is written by hand, see XmlDeclaration
        OmitXmlDeclaration = true,
        // the lifetime of the stream is managed by the caller, the writer must not
        // close it - otherwise the file would stay locked and could not be replaced
        CloseOutput = false
    };

    /// <summary>
    /// Appends a line break to the end of a file.
    /// </summary>
    private static void AppendNewLine(string path)
    {
        using var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None);
        stream.WriteByte((byte)'\n');
    }

    /// <summary>
    /// Escapes a dictionary entry for the use inside a URL.
    /// </summary>
    /// <remarks>
    /// The old implementation escaped the quotes and the angle brackets, but not the
    /// ampersand. That was a bug: an ampersand in a dictionary entry would have
    /// produced an invalid XML document. The ampersand is escaped first, so that the
    /// escapes that are created afterwards are not escaped a second time.
    /// <para>
    /// Control characters (line break, tab, ...) are not allowed inside a URL. Two
    /// dictionary entries contain such a line break, so they are percent encoded
    /// here. The encoding has to be exact: the site compares the search term with
    /// the database entry character by character, so only the correct encoding
    /// (%0A for a line break) leads to the found entry. An encoded space (%20)
    /// would produce a page without any result.
    /// </para>
    /// </remarks>
    private static string EscapeEntry(string entry)
    {
        string text = EncodeControlCharacters(entry);

        return text
            .Replace("&", "&amp;")
            .Replace("'", "&apos;")
            .Replace("\"", "&quot;")
            .Replace(">", "&gt;")
            .Replace("<", "&lt;");
    }

    /// <summary>
    /// Percent encodes the control characters of a dictionary entry, so that they
    /// do not appear as raw control characters in the URL.
    /// </summary>
    private static string EncodeControlCharacters(string entry)
    {
        StringBuilder builder = null;

        for (int i = 0; i < entry.Length; i++)
        {
            char character = entry[i];
            if (character < 0x20 || character == 0x7F)
            {
                if (builder == null)
                {
                    builder = new StringBuilder(entry.Length + 8);
                    builder.Append(entry, 0, i);
                }
                builder.Append('%').Append(((int)character).ToString("X2"));
            }
            else
            {
                builder?.Append(character);
            }
        }

        // the common case - an entry without control characters - does not allocate
        return builder == null ? entry : builder.ToString();
    }

    /// <summary>
    /// Writes a single sitemap file. The file is created as a temporary file and only
    /// moved to its final name when it is complete, so a crawler never sees a broken
    /// sitemap.
    /// </summary>
    private sealed class SitemapFileWriter : IDisposable
    {
        private readonly string _finalPath;
        private readonly string _tempPath;
        private readonly CountingStream _stream;
        private readonly XmlWriter _writer;
        private readonly ILogger<SitemapService> _logger;
        private bool _closed;

        public int UrlCount { get; private set; }

        public long BytesWritten => _stream.BytesWritten;

        public SitemapFileWriter(string finalPath, ILogger<SitemapService> logger)
        {
            _finalPath = finalPath;
            _tempPath = finalPath + ".tmp";
            _logger = logger;

            _stream = new CountingStream(
                new FileStream(_tempPath, FileMode.Create, FileAccess.Write, FileShare.None, FileBufferSize));
            _writer = XmlWriter.Create(_stream, CreateWriterSettings());
            _writer.WriteRaw(XmlDeclaration);
            _writer.WriteStartElement("urlset", SitemapNamespace);
        }

        public void WriteUrl(string location, string priority = null)
        {
            _writer.WriteStartElement("url");
            _writer.WriteStartElement("loc");
            _writer.WriteString(location);
            _writer.WriteEndElement();
            _writer.WriteElementString("changefreq", "always");
            if (priority != null)
            {
                _writer.WriteElementString("priority", priority);
            }
            _writer.WriteEndElement();
            UrlCount++;
        }

        /// <summary>
        /// Writes a URL whose location is already escaped. The text is written as it
        /// is, without any further escaping by the XML writer.
        /// </summary>
        public void WriteRawUrl(string escapedLocation, string priority = null)
        {
            _writer.WriteStartElement("url");
            _writer.WriteStartElement("loc");
            _writer.WriteRaw(escapedLocation);
            _writer.WriteEndElement();
            _writer.WriteElementString("changefreq", "always");
            if (priority != null)
            {
                _writer.WriteElementString("priority", priority);
            }
            _writer.WriteEndElement();
            UrlCount++;
        }

        /// <summary>
        /// Closes the document and puts the finished file in place.
        /// </summary>
        public void Complete()
        {
            if (_closed)
            {
                return;
            }

            _writer.WriteEndElement();
            _writer.WriteEndDocument();

            // the file handle has to be released before the file can be moved,
            // otherwise the move fails with "Access to the path is denied"
            CloseWriter();
            long writtenBytes = _stream.BytesWritten;

            // keep the file a proper text file for git
            AppendNewLine(_tempPath);

            long fileBytes = new FileInfo(_tempPath).Length;
            if (fileBytes != writtenBytes + 1)
            {
                _logger.LogWarning(
                    "{File}: {WrittenBytes} Bytes geschrieben, die Datei ist aber {FileBytes} Bytes groß.",
                    _finalPath, writtenBytes, fileBytes);
            }

            MoveWithRetry(_tempPath, _finalPath);
        }

        public void Dispose()
        {
            if (_closed)
            {
                return;
            }
            CloseWriter();
            if (File.Exists(_tempPath))
            {
                DeleteWithRetry(_tempPath);
            }
        }

        /// <summary>
        /// Flushes and closes the XML writer and the underlying file. Afterwards the
        /// file is no longer locked and can be moved or deleted.
        /// </summary>
        private void CloseWriter()
        {
            if (_closed)
            {
                return;
            }
            _closed = true;
            _writer.Flush();
            _writer.Dispose();
            _stream.Flush();
            _stream.Dispose();
        }
    }

    /// <summary>
    /// Stream that counts the written bytes, so that the size limit of a sitemap file
    /// can be enforced while the file is being written.
    /// </summary>
    private sealed class CountingStream : Stream
    {
        private readonly Stream _inner;

        public CountingStream(Stream inner)
        {
            _inner = inner;
        }

        public long BytesWritten { get; private set; }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => _inner.CanWrite;
        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override void Flush() => _inner.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
        public override void SetLength(long value) => _inner.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Write(buffer, offset, count);
            BytesWritten += count;
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            _inner.Write(buffer);
            BytesWritten += buffer.Length;
        }

        public override void WriteByte(byte value)
        {
            _inner.WriteByte(value);
            BytesWritten++;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _inner.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
