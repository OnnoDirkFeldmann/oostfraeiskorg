using System;

namespace WFDOT
{
    public partial class onlinecourseen : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Page.Title = "online course - Ōstfräisk Wōrdenbauk - East Frisian Dictionary";
            Master.Page.MetaKeywords += "online course, eastfrisian language, eastfrisian, ōstfräisk, course, course of eastfrisian language";
            Master.Page.MetaDescription = "online course - Dictionary of the East Frisian language - translate words from East Frisian or into East Frisian. Learn the language of the East Frisians as Standard East Frisian with the Dictionary for East Frisian Platt.";
        }
    }
}