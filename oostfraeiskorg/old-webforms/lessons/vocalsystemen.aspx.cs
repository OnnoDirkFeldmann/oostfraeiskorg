using System;

namespace WFDOT
{
    public partial class vocalsystemen : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Page.Title = "vocalsystem - Ōstfräisk Wōrdenbauk - East Frisian Dictionary";
            Master.Page.MetaKeywords += "vocalsystem, eastfrisian language, eastfrisian, ōstfräisk, course, course of eastfrisian language";
            Master.Page.MetaDescription = "vocalsystem - Dictionary of the East Frisian language - translate words from East Frisian or into East Frisian. Learn the language of the East Frisians as Standard East Frisian with the Dictionary for East Frisian Platt.";
        }
    }
}