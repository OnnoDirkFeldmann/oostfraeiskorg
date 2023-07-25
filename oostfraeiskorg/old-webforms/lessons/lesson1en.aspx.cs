using System;

namespace WFDOT
{
    public partial class lesson1en : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Page.Title = "lesson 1 - Ōstfräisk Wōrdenbauk - East Frisian Dictionary";
            Master.Page.MetaKeywords += "lesson 1, eastfrisian language, eastfrisian, ōstfräisk, course, course of eastfrisian language";
            Master.Page.MetaDescription = "lesson 1 - Dictionary of the East Frisian language - translate words from East Frisian or into East Frisian. Learn the language of the East Frisians as Standard East Frisian with the Dictionary for East Frisian Platt.";
        }
    }
}