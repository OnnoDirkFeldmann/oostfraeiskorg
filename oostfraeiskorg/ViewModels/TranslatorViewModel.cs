using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using DotVVM.Framework.Binding;
using System.Net.Mail;
using System.Net;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using oostfraeiskorg.Services;

namespace oostfraeiskorg.ViewModels;

/// <summary>
/// Holds an ordered list of API URLs and tracks which one is currently active.
/// Once a URL fails, the next one is tried and the failed one is never retried.
/// Thread-safe so concurrent requests don't corrupt the index.
/// </summary>
public class ApiEndpoints
{
    private readonly string[] _urls;
    private volatile int _currentIndex;

    public ApiEndpoints(params string[] urls)
    {
        if (urls is null || urls.Length == 0)
            throw new ArgumentException("At least one URL must be provided.", nameof(urls));
        _urls = urls;
    }

    public string CurrentUrl => _urls[_currentIndex];

    /// <summary>
    /// Advances to the next URL if the current one matches <paramref name="failedUrl"/>.
    /// Returns true if a fallback URL is available, false if all URLs are exhausted.
    /// </summary>
    public bool TryAdvance(string failedUrl)
    {
        lock (_urls)
        {
            // Only advance if the failure is for the URL we're currently on
            if (_currentIndex < _urls.Length && _urls[_currentIndex] == failedUrl)
            {
                _currentIndex++;
            }
            return _currentIndex < _urls.Length;
        }
    }

    public bool IsExhausted => _currentIndex >= _urls.Length;

    /// <summary>
    /// Resets the endpoint chain to the first URL, allowing retries after exhaustion.
    /// Thread-safe.
    /// </summary>
    public void Reset()
    {
        lock (_urls)
        {
            _currentIndex = 0;
        }
    }
}

public class TranslatorViewModel : MasterPageViewModel
{
    private const int MaxTextLength = 300;
    private const int DelayMilliseconds = 50;
    private const int PollTimeoutSeconds = 60;
    private const int TtsRequestTimeoutSeconds = 300;
    private const int MaxTtsRetryDelaySeconds = 20;
    private const int MaxTtsCacheSize = 50;

    // Shared NLLB endpoint chain — used by both the official page and the test page.
    // Failover state persists for the lifetime of the app process and is shared across both pages.
    public static readonly ApiEndpoints SharedNllbEndpoints = new(
        "https://vanmoders114-east-frisian-nllb-translator.hf.space/gradio_api/call/translate",
        "https://vanmoders114-east-frisian-nllb-translator-backup.hf.space/gradio_api/call/translate"
    );

    private static readonly string ApiSaveFeedbackUrl = "https://vanmoders114-ooversetter-feedback.hf.space/gradio_api/call/save_translation";
    private static readonly string ApiTtsUrl = "https://vanmoders114-east-frisian-tts.hf.space/gradio_api/call/predict";

    private readonly string BearerToken;
    private readonly string SmtpCredentialName;
    private readonly string SmtpCredentialPassword;
    private readonly TranslationCounterService _counterService;

    private static readonly HttpClient SharedHttpClient = new()
    {
        Timeout = Timeout.InfiniteTimeSpan
    };
    private static readonly ConcurrentDictionary<string, string> TtsCache = new();
    private static readonly HuggingFaceSpaceWakeup TtsSpaceWakeup = new(
        SharedHttpClient, "VanModers114/East_Frisian_TTS");

    public List<string> LanguageOptions { get; } = ["Deutsch", "English", "Oostfräisk"];
    public bool DoubleTranslationEnabled { get; } = true;

    public int MaximumTextLength { get; } = MaxTextLength;
    public string InputTitle { get; set; } = "Deutsch";
    public string TranslationTitle { get; set; } = "Oostfräisk";
    public string InputText { get; set; } = "";
    public string InputPlaceholderText { get; set; } = "Hier steht der Text.";
    public string OutputText { get; set; } = "";
    public string TranslationPlaceholderText { get; set; } = "Hier steht die Übersetzung.";
    public string TranslationText { get; set; } = "Übersetzen";
    public bool ShowTranslationFeedback { get; set; } = false;
    public bool IsLoading { get; set; } = false;
    [Bind(Direction.ServerToClient)]
    public string TtsAudioUrl { get; set; } = "";
    [Bind(Direction.ServerToClient)]
    public string TtsErrorMessage { get; set; } = "";
    public bool IsTtsLoading { get; set; } = false;


    public TranslatorViewModel(IConfiguration configuration, TranslationCounterService counterService)
    {
        // Load settings
        BearerToken = configuration["TranslatorConfig:BearerToken"];
        if (string.IsNullOrEmpty(BearerToken))
        {
            throw new Exception("BearerToken is missing in the configuration.");
        }
        SmtpCredentialName = configuration["TranslatorConfig:SmtpCredentialName"];
        if (string.IsNullOrEmpty(SmtpCredentialName))
        {
            throw new Exception("SmtpCredentialName is missing in the configuration.");
        }
        SmtpCredentialPassword = configuration["TranslatorConfig:SmtpCredentialPassword"];
        if (string.IsNullOrEmpty(SmtpCredentialPassword))
        {
            throw new Exception("SmtpCredentialPassword is missing in the configuration.");
        }
        _counterService = counterService;
    }

    public override Task Init()
    {
        MasterPageTitle = "Ooversetter - Übersetzer für das Ostfriesische Platt - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Ooversetter - Übersetzer für das Ostfriesische Platt - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", ooversetter, übersetzer, ostfriesisches platt, translator ostfriesische Sprache, ostfriesisch, oostfräisk, ostfriesisches plattdeutsch";
        NormalizeLanguageSelection();
        return base.Init();
    }

    public override Task PreRender()
    {
        NormalizeLanguageSelection();
        return base.PreRender();
    }

    public void SwitchTranslationMode()
    {
        ClearSpeechResult();
        var temp = InputTitle;
        InputTitle = TranslationTitle;
        TranslationTitle = temp;
        var tempText = InputText;
        InputText = OutputText;
        OutputText = tempText;
        NormalizeLanguageSelection();
    }

    public void ClearInput()
    {
        ClearSpeechResult();
        InputText = "";
        OutputText = "";
        ShowTranslationFeedback = false;
    }

    public void DeactivateFeedback()
    {
        ShowTranslationFeedback = false;
    }

    public async Task ReportTranslationIssue()
    {
        await SentReport("Translation Issue Report", " [FEELER]");
        ShowTranslationFeedback = false;
    }

    public async Task ReportTranslationSuccess()
    {
        await SentReport("Translation Success Report", "");
        ShowTranslationFeedback = false;
    }

    private async Task SentReport(string subject, string addedMarker)
    {
        string markedOutput = OutputText + addedMarker;
        string gerText = InputTitle == "Deutsch"
            ? InputText
            : (TranslationTitle == "Deutsch" ? markedOutput : string.Empty);
        string frsText = InputTitle == "Oostfräisk"
            ? InputText
            : (TranslationTitle == "Oostfräisk" ? markedOutput : string.Empty);

        await SaveFeedback(gerText, frsText, ApiSaveFeedbackUrl, BearerToken);
        try
        {
            using (var client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(SmtpCredentialName, SmtpCredentialPassword)
            })
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("edufraeisk@gmail.com"),
                    Subject = subject,
                    Body = $"{InputTitle}: {InputText}\n\n{TranslationTitle}:\n\n{OutputText}{addedMarker}",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add("oostfraeisk.ooversetter@gmail.com");

                client.Send(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }

    public static async Task SaveFeedback(string gerText, string frsText, string apiUrl, string bearerToken)
    {
        using HttpClient client = new HttpClient();

        // Set up the headers
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        // JSON payload
        var requestBody = new
        {
            data = new string[] { gerText, frsText }
        };

        string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            // Step 1: Send POST request to get the event ID
            HttpResponseMessage response = await client.PostAsync(apiUrl, content);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            var jsonIdResponse = JObject.Parse(responseJson);
            string eventId = jsonIdResponse["event_id"]?.ToString();

            if (string.IsNullOrEmpty(eventId))
            {
                throw new Exception("Failed to retrieve event_id from API response.");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void PrepareTranslation()
    {
        ClearSpeechResult();
        OutputText = "";
        IsLoading = true;
        ShowTranslationFeedback = false;
    }

    public async Task TranslateAsync()
    {
        if (!IsAllowedPair(InputTitle, TranslationTitle))
        {
            OutputText = "Diese Sprachkombination wird derzeit nicht unterstützt.";
            IsLoading = false;
            ShowTranslationFeedback = false;
            return;
        }

        // Perform translation with automatic failover
        OutputText = await TranslateWithFallback(InputText, InputTitle, TranslationTitle, SharedNllbEndpoints, BearerToken, MaxTextLength);
        IsLoading = false;
        ShowTranslationFeedback = true;

        // Increment translation counter for the official translator page
        _counterService.IncrementCounter();
    }

    public void PrepareGenerateSpeech()
    {
        ClearSpeechResult();
        IsTtsLoading = true;
    }

    public void ChangeLanguageSelection()
    {
        ClearSpeechResult();
        NormalizeLanguageSelection();
    }

    private void ClearSpeechResult()
    {
        TtsAudioUrl = "";
        TtsErrorMessage = "";
    }

    public async Task GenerateSpeechAsync()
    {
        if (!IsTtsLoading)
            return;

        try
        {
            string textToSpeak = TranslationTitle == "Oostfräisk" ? OutputText : InputText;
            if (string.IsNullOrWhiteSpace(textToSpeak))
                return;

            if (TtsCache.TryGetValue(textToSpeak, out string cachedAudio))
            {
                TtsAudioUrl = cachedAudio;
                return;
            }

            string audio = await GenerateSpeech(textToSpeak, ApiTtsUrl, BearerToken);
            if (TtsCache.Count >= MaxTtsCacheSize)
                TtsCache.Clear();
            TtsCache[textToSpeak] = audio;
            TtsAudioUrl = audio;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TTS Error: {ex.Message}");
            TtsErrorMessage = "Die KI-Sprachausgabe konnte nicht gestartet werden. Bitte versuchen Sie es in einigen Minuten erneut.";
        }
        finally
        {
            IsTtsLoading = false;
        }
    }

    private static bool IsTransientTtsStatus(HttpStatusCode statusCode) =>
        statusCode == HttpStatusCode.RequestTimeout ||
        statusCode == HttpStatusCode.TooManyRequests ||
        statusCode == HttpStatusCode.InternalServerError ||
        statusCode == HttpStatusCode.BadGateway ||
        statusCode == HttpStatusCode.ServiceUnavailable ||
        statusCode == HttpStatusCode.GatewayTimeout;

    private static async Task<HttpResponseMessage> SendTtsRequestWithRetry(
        Func<HttpRequestMessage> requestFactory,
        string operation,
        CancellationToken cancellationToken)
    {
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                using HttpRequestMessage request = requestFactory();
                HttpResponseMessage response = await SharedHttpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                    return response;

                if (!IsTransientTtsStatus(response.StatusCode))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    string body = await response.Content.ReadAsStringAsync(cancellationToken);
                    response.Dispose();
                    string preview = body.Length > 300 ? body[..300] : body;
                    throw new InvalidOperationException(
                        $"TTS {operation} failed with HTTP {(int)statusCode}: {preview}");
                }

                HttpStatusCode transientStatusCode = response.StatusCode;
                TimeSpan retryDelay = GetTtsRetryDelay(response, attempt);
                response.Dispose();
                Console.WriteLine(
                    $"TTS {operation} received HTTP {(int)transientStatusCode}; " +
                    $"retrying in {retryDelay.TotalSeconds:0} seconds (attempt {attempt}).");
                await Task.Delay(retryDelay, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                TimeSpan retryDelay = GetTtsRetryDelay(null, attempt);
                Console.WriteLine(
                    $"TTS {operation} request failed ({ex.GetType().Name}); " +
                    $"retrying in {retryDelay.TotalSeconds:0} seconds (attempt {attempt}).");
                await Task.Delay(retryDelay, cancellationToken);
            }
        }
    }

    private static TimeSpan GetTtsRetryDelay(HttpResponseMessage response, int attempt)
    {
        TimeSpan? retryAfter = response?.Headers.RetryAfter?.Delta;
        if (!retryAfter.HasValue && response?.Headers.RetryAfter?.Date is DateTimeOffset retryDate)
            retryAfter = retryDate - DateTimeOffset.UtcNow;

        TimeSpan delay = retryAfter.HasValue && retryAfter.Value > TimeSpan.Zero
            ? retryAfter.Value
            : TimeSpan.FromSeconds(Math.Pow(2, Math.Min(attempt, 4)));

        return delay > TimeSpan.FromSeconds(MaxTtsRetryDelaySeconds)
            ? TimeSpan.FromSeconds(MaxTtsRetryDelaySeconds)
            : delay;
    }

    public static async Task<string> GenerateSpeech(
        string text,
        string apiUrl,
        string bearerToken)
    {
        string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(new
        {
            data = new string[] { text }
        });
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(TtsRequestTimeoutSeconds));

        if (string.Equals(apiUrl, ApiTtsUrl, StringComparison.OrdinalIgnoreCase))
            await TtsSpaceWakeup.EnsureRunningAsync(bearerToken, timeout.Token);

        using HttpResponseMessage response = await SendTtsRequestWithRetry(
            () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Headers.Add("Authorization", $"Bearer {bearerToken}");
                request.Headers.Add("Accept", "application/json");
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                return request;
            },
            "submission",
            timeout.Token);
        string responseJson = await response.Content.ReadAsStringAsync(timeout.Token);
        string eventId = JObject.Parse(responseJson)["event_id"]?.ToString();
        if (string.IsNullOrEmpty(eventId))
            throw new InvalidOperationException("Failed to retrieve event_id from TTS API response.");

        string resultUrl = $"{apiUrl}/{eventId}";
        using HttpResponseMessage resultResponse = await SendTtsRequestWithRetry(
            () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, resultUrl);
                request.Headers.Add("Authorization", $"Bearer {bearerToken}");
                request.Headers.Add("Accept", "text/event-stream");
                return request;
            },
            "result stream",
            timeout.Token);
        string jsonData = await resultResponse.Content.ReadAsStringAsync(timeout.Token);
        if (jsonData.Contains("event: error"))
            throw new InvalidOperationException("The TTS Space reported an inference error.");
        if (!jsonData.Contains("event: complete"))
            throw new InvalidOperationException("The TTS Space closed without a complete event.");

        jsonData = ExtractJsonFromEventStream(jsonData)
            ?? throw new InvalidOperationException("The TTS result did not contain JSON data.");
        string fileUrl = JArray.Parse(jsonData)[0]?["url"]?.ToString();
        if (string.IsNullOrEmpty(fileUrl))
            throw new InvalidOperationException("The TTS result did not contain an audio URL.");

        using HttpResponseMessage downloadResponse = await SendTtsRequestWithRetry(
            () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, fileUrl);
                request.Headers.Add("Authorization", $"Bearer {bearerToken}");
                return request;
            },
            "audio download",
            timeout.Token);
        byte[] wavBytes = await downloadResponse.Content.ReadAsByteArrayAsync(timeout.Token);
        if (wavBytes.Length < 12 || Encoding.ASCII.GetString(wavBytes, 0, 4) != "RIFF" ||
            Encoding.ASCII.GetString(wavBytes, 8, 4) != "WAVE")
            throw new InvalidOperationException("The TTS download was not a WAV file.");

        return $"data:audio/wav;base64,{Convert.ToBase64String(wavBytes)}";
    }

    /// <summary>
    /// Attempts translation using the current endpoint. On failure, advances to the next
    /// backup URL and retries. Stops when a translation succeeds or all URLs are exhausted.
    /// Resets the endpoint chain after exhaustion to allow retries on future attempts.
    /// </summary>
    public static async Task<string> TranslateWithFallback(string text, string sourceLanguage, string targetLanguage, ApiEndpoints endpoints, string bearerToken, int maxTextLength)
    {
        while (!endpoints.IsExhausted)
        {
            string currentUrl = endpoints.CurrentUrl;
            try
            {
                return await Translate(text, sourceLanguage, targetLanguage, currentUrl, bearerToken, maxTextLength);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Translation failed for {currentUrl}: {ex.Message}");
                if (!endpoints.TryAdvance(currentUrl))
                {
                    break;
                }
                Console.WriteLine($"Falling back to {endpoints.CurrentUrl}");
            }
        }

        // Reset endpoints for future retry attempts
        endpoints.Reset();
        return "Translation failed.";
    }

    /// <summary>
    /// Performs the actual API call. Throws on failure so the caller can handle failover.
    /// </summary>
    public static async Task<string> Translate(string text, string sourceLanguage, string targetLanguage, string apiUrl, string bearerToken, int maxTextLength)
    {
        if (text.Length > maxTextLength)
        {
            text = text.Substring(0, maxTextLength);
        }

        using HttpClient client = new HttpClient();

        // Set up the headers
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        // JSON payload — NLLB model expects [text, sourceLanguage, targetLanguage]
        var requestBody = new
        {
            data = new string[] { text, sourceLanguage, targetLanguage }
        };

        string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // Step 1: Send POST request to get the event ID
        HttpResponseMessage response = await client.PostAsync(apiUrl, content);
        response.EnsureSuccessStatusCode();

        string responseJson = await response.Content.ReadAsStringAsync();
        var jsonIdResponse = JObject.Parse(responseJson);
        string eventId = jsonIdResponse["event_id"]?.ToString();

        if (string.IsNullOrEmpty(eventId))
        {
            throw new Exception("Failed to retrieve event_id from API response.");
        }

        // Step 2: Fetch the translation result using event ID
        string resultUrl = $"{apiUrl}/{eventId}";
        var deadline = DateTime.UtcNow.AddSeconds(PollTimeoutSeconds);

        while (DateTime.UtcNow < deadline)
        {
            await Task.Delay(DelayMilliseconds); // Wait before checking the result
            HttpResponseMessage resultResponse = await client.GetAsync(resultUrl);
            string jsonData = await resultResponse.Content.ReadAsStringAsync();

            if (jsonData.Contains("event: complete"))
            {
                jsonData = ExtractJsonFromEventStream(jsonData);
                JArray jsonResponse = JArray.Parse(jsonData);

                // Extract the translation from the "data" array
                string translation = jsonResponse[0]?.ToString();
                return translation ?? throw new Exception("Translation response was null.");
            }

            if (jsonData.Contains("event: error"))
            {
                throw new Exception($"API returned an error event for {apiUrl}.");
            }
        }

        throw new Exception($"API polling timed out for {apiUrl}.");
    }

    private static string ExtractJsonFromEventStream(string rawResponse)
    {
        // Split response by lines
        string[] lines = rawResponse.Split('\n');
        string lastLine = "";

        foreach (string line in lines)
        {
            if (lastLine.StartsWith("event: complete") && line.StartsWith("data: "))
            {
                // Extract the JSON part after "data: "
                return line.Substring(6).Trim();
            }
            lastLine = line;
        }
        return null;
    }

    private bool IsAllowedPair(string sourceLanguage, string targetLanguage)
    {
        if (string.Equals(sourceLanguage, targetLanguage, StringComparison.Ordinal))
            return false;

        bool germanEnglishPair =
            (sourceLanguage == "Deutsch" && targetLanguage == "English") ||
            (sourceLanguage == "English" && targetLanguage == "Deutsch");
        return !germanEnglishPair;
    }

    public void NormalizeLanguageSelection()
    {
        if (IsAllowedPair(InputTitle, TranslationTitle))
            return;

        string allowedOutput = LanguageOptions.FirstOrDefault(
            language => IsAllowedPair(InputTitle, language));
        if (!string.IsNullOrEmpty(allowedOutput))
        {
            TranslationTitle = allowedOutput;
            return;
        }

        string allowedInput = LanguageOptions.FirstOrDefault(
            language => IsAllowedPair(language, TranslationTitle));
        if (!string.IsNullOrEmpty(allowedInput))
        {
            InputTitle = allowedInput;
            return;
        }

        InputTitle = "Deutsch";
        TranslationTitle = "Oostfräisk";
    }
}
