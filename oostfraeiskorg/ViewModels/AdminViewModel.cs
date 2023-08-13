using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using DotVVM.Framework.ViewModel;

namespace oostfraeiskorg.ViewModels;

public class AdminViewModel : MasterPageViewModel
{

    [Bind(Direction.Both)]
    public string Message { get; set; }

    public void RecreateSitemap()
    {
        RecreateSitemapTask();
        Message = "Sitemaps erstellt";
    }

    public void RecreateSitemapTask()
    {
        //Fixe Sitemap erstellen
        string ls_sitemap_fixed;
        ls_sitemap_fixed = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
        ls_sitemap_fixed += "<urlset xmlns=\"https://www.sitemaps.org/schemas/sitemap/0.9\">";
        ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/</loc><changefreq>always</changefreq><priority>0.8</priority></url>";
        ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/main</loc><changefreq>always</changefreq></url>";
        ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/grammar</loc><changefreq>always</changefreq></url>";
        ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/coursebook</loc><changefreq>always</changefreq></url>";
        ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/onlinecourse</loc><changefreq>always</changefreq></url>";
        ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/keyboard</loc><changefreq>always</changefreq></url>";
        ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/downloads</loc><changefreq>always</changefreq></url>";
        ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/about</loc><changefreq>always</changefreq></url>";
        ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/dsgvo</loc><changefreq>always</changefreq></url>";
        ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/links</loc><changefreq>always</changefreq></url>";
        ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/instructions</loc><changefreq>always</changefreq></url>";
        ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/donateaword</loc><changefreq>always</changefreq></url>";
        ls_sitemap_fixed += "</urlset>";
        var sitemap_fixed = "wwwroot\\sitemapfixed.xml";
        using (StreamWriter writer = new StreamWriter(sitemap_fixed, false))
        {
            writer.WriteLine(ls_sitemap_fixed);
        }

        //Datenbankeiträge
        var sqlCon = DataBaseConnection.GetConnection(oostfraeiskorg.Server.MapPath(""));
        var sqlcmd = new SqliteCommand();
        sqlcmd.Connection = sqlCon;
        sqlcmd.CommandText = "SELECT Ostfriesisch FROM WB ORDER BY Ostfriesisch ASC";
        SqliteDataReader reader = sqlcmd.ExecuteReader();
        List<string> entrys = new List<string>();
        while (reader.Read())
        {
            entrys.Add(reader.GetValue(0).ToString());
        }
        reader.Close();
        sqlCon.Close();

        string ls_sitemap_dynamic = "";
        var index = 1;
        var entryCount = 30000;
        for (int i = 0; i < entrys.Count; i++)
        {
            if (i == 0 || i % entryCount == 1)
            {
                ls_sitemap_dynamic = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                ls_sitemap_dynamic += "<urlset xmlns=\"https://www.sitemaps.org/schemas/sitemap/0.9\">";
            }
            ls_sitemap_dynamic += "<url><loc>https://oostfraeisk.org/main?W=" + entrys[i].Replace("'", "&apos;").Replace("\"", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;") + "&amp;df=frs>de&amp;fts=J</loc><changefreq>always</changefreq></url>";
            if ((i == entrys.Count - 1 || i % entryCount == 0) && i != 0)
            {
                ls_sitemap_dynamic += "</urlset>";
                string sitemapxml_dynamic = $"wwwroot\\sitemapdynamic{index}.xml";
                using (StreamWriter writer = new StreamWriter(sitemapxml_dynamic, false))
                {
                    writer.WriteLine(ls_sitemap_dynamic);
                }
                ls_sitemap_dynamic = "";
                index++;
            }
        }
    }
}

