using System.Collections.Generic;
using DotVVM.Framework.ViewModel;

namespace oostfraeiskorg.ViewModels;

public class MasterPageViewModel : DotvvmViewModelBase
{
    public MasterPageViewModel()
    {
        ShowKeyboard = false;
    }

    public List<string> Languages { get => new List<string>() { "DE>FRS", "FRS>DE", "EN>FRS", "FRS>EN" }; }

    public string SelectedLanguage { get; set; } = "DE>FRS";

    public bool ShowKeyboard { get; set; } = false;

    public string SearchText { get; set; } = "";
    public string MasterPageTitle { get; set; } = "Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";

    public string MasterPageDescription { get; set; } = "Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";

    public string MasterPageKeywords { get; set; } = "platt, ostfriesisch, Wörterbuch, Ostfriesland, plattdeutsch, ostfriesisich-deutsch Wörterbuch, deutsch-ostfriesisch-Wörterbuch, elektronisches Wörterbuch, Onlinewörterbuch, Wörterbuch online, ostfriesich online, ōstfräisk, Jungfräiske Mäinskup, JFM, JFM Ostfriesland, Sprache von Ostfriesland, Ostfriesland Platt, Ostfriesisches Platt, ostfriesische Sprache, Regionalsprache, Wörterbuch der ostfriesischen Sprache, Weigelt, Schreibweise, Rechtschreibung, ostfriesische Schreibweise, friesisch schreiben, ostfriesisch lernen";

    public string AlertText { get; set; } = "";

    public bool AlertVisible { get; set; }

    #region methods

    public void Search()
    {
        Context.RedirectToRoute("main", query: new { W = SearchText, df = SelectedLanguage.ToLower(), fts = "J" });
    }

    public void SearchLike()
    {
        Context.RedirectToRoute("main", query: new { W = SearchText, df = SelectedLanguage.ToLower(), fts = "N" });
    }

    public void SearchExact()
    {
        Context.RedirectToRoute("main", query: new { W = SearchText, df = SelectedLanguage.ToLower(), fts = "X" });
    }

    public void SearchBegins()
    {
        Context.RedirectToRoute("main", query: new { W = SearchText, df = SelectedLanguage.ToLower(), fts = "B" });
    }

    public void SearchEnds()
    {
        Context.RedirectToRoute("main", query: new { W = SearchText, df = SelectedLanguage.ToLower(), fts = "E" });
    }

    #endregion

    #region events

    public void btn_keyboard_Click()
    {
        ShowKeyboard = ShowKeyboard ? false : true;
    }

    public void btn_symbols_Click(string text)
    {
        SearchText += text;
    }

    #endregion

}
