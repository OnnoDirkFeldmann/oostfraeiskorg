using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.IO;

namespace WFDOT
{
    public partial class admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Passwortabfrage
        }

        protected void btn_sitemap_Click(object sender, EventArgs e)
        {
            //Fixe Sitemap erstellen
            string ls_sitemap_fixed;
            ls_sitemap_fixed = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            ls_sitemap_fixed += "<urlset xmlns=\"https://www.sitemaps.org/schemas/sitemap/0.9\">";
            ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/</loc><changefreq>always</changefreq><priority>0.8</priority></url>";
            ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/main.aspx</loc><changefreq>always</changefreq></url>";
            ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/grammar.aspx</loc><changefreq>always</changefreq></url>";
            ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/leerbauk.aspx</loc><changefreq>always</changefreq></url>";
            ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/onlinecourse.aspx</loc><changefreq>always</changefreq></url>";
            ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/keyboard.aspx</loc><changefreq>always</changefreq></url>";
            ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/downloads.aspx</loc><changefreq>always</changefreq></url>";
            ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/about.aspx</loc><changefreq>always</changefreq></url>";
            ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/dsgvo.aspx</loc><changefreq>always</changefreq></url>";
            ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/links.aspx</loc><changefreq>always</changefreq></url>";
            ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/instructions.aspx</loc><changefreq>always</changefreq></url>";
            ls_sitemap_fixed += "<url><loc>https://oostfraeisk.org/donateaword.aspx</loc><changefreq>always</changefreq></url>";
            ls_sitemap_fixed += "</urlset>";
            var sitemap_fixed = Server.MapPath("/") + @"sitemapfixed.xml";
            using (StreamWriter writer = new StreamWriter(sitemap_fixed, false))
            {
                writer.WriteLine(ls_sitemap_fixed);
            }

            //Datenbankeiträge
            var sqlCon = SQLCON.GetConnection(Server.MapPath("/"));
            SqliteCommand sqlcmd = new SqliteCommand(sqlCon);
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
                if(i == 0 || i % entryCount == 1)
                {
                    ls_sitemap_dynamic = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                    ls_sitemap_dynamic += "<urlset xmlns=\"https://www.sitemaps.org/schemas/sitemap/0.9\">";
                }
                ls_sitemap_dynamic += "<url><loc>https://oostfraeisk.org/main.aspx?W=" + ((entrys [i].Replace("'", "&apos;")).Replace("\"", "&quot;").Replace(">", "&gt;")).Replace("<", "&lt;") + "&amp;df=frs>de&amp;fts=J</loc><changefreq>always</changefreq></url>";
                if ((i == entrys.Count - 1 || i % entryCount == 0) && i != 0)
                {
                    ls_sitemap_dynamic += "</urlset>";
                    string sitemapxml_dynamic = Server.MapPath("/") + $"sitemapdynamic{index}.xml";
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
}