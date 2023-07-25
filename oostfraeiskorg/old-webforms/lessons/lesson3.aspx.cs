using System;

namespace WFDOT
{
    public partial class lesson3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Page.Title = "Lektion 3 - Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
            Master.Page.MetaKeywords += "Lektion 3, ostfriesische Sprache, ostfriesisch, ōstfräisk, Kurs, Kurs der ostfriesischen Sprache";
            Master.Page.MetaDescription = "Lektion 3 - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
        }
    }
}