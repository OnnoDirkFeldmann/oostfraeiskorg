using System;

namespace WFDOT
{
    public partial class onlinecourse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Page.Title = "Onlinekurs - Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
            Master.Page.MetaKeywords += "Onlinekurs, ostfriesische Sprache, ostfriesisch, ōstfräisk, Kurs, Kurs der ostfriesischen Sprache";
            Master.Page.MetaDescription = "Onlinekurs - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
        }
    }
}