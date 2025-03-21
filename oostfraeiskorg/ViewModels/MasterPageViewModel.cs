﻿using System.Collections.Generic;
using DotVVM.Framework.ViewModel;

namespace oostfraeiskorg.ViewModels;

public class MasterPageViewModel : DotvvmViewModelBase
{
    public MasterPageViewModel()
    {
        ShowKeyboard = false;
    }

    public List<string> Languages { get => new List<string>() { "DE>FRS", "FRS>DE", "EN>FRS", "FRS>EN" }; }

    private string _selectedLanguage = "DE>FRS";
    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set => _selectedLanguage = value;
    }

    public string InputPlaceHolder { get; set; } = "Gib ein deutsches Wort ein";
    public bool ShowKeyboard { get; set; } = false;
    public string SearchText { get; set; } = "";
    public string MasterPageTitle { get; set; } = "Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
    public string MasterPageDescription { get; set; } = "Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
    public string MasterPageKeywords { get; set; } = "platt, ostfriesisch, Wörterbuch, Ostfriesland, plattdeutsch, ostfriesisich-deutsch Wörterbuch, deutsch-ostfriesisch-Wörterbuch, elektronisches Wörterbuch, Onlinewörterbuch, Wörterbuch online, ostfriesich online, oostfräisk, Sprache von Ostfriesland, Ostfriesland Platt, Ostfriesisches Platt, ostfriesische Sprache, Regionalsprache, Wörterbuch der ostfriesischen Sprache, Weigelt, Schreibweise, Rechtschreibung, ostfriesische Schreibweise, friesisch schreiben, ostfriesisch lernen";
    public string AlertText { get; set; } = "";
    public bool AlertVisible { get; set; }
    public string SearchButtonText { get; set; } = "Suche";
    public string ExtendedSearchButtonText { get; set; } = "Erweiterte Suche";
    public string SearchLikeButtonText { get; set; } = "Unscharfe Suche";
    public string SearchBeginsButtonText { get; set; } = "Beginnt mit";
    public string SearchEndsButtonText { get; set; } = "Endet mit";
    public string SearchExactButtonText { get; set; } = "Exakte Suche";

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

    public void LanguageSelectionChanged()
    {
        switch (SelectedLanguage)
        {
            case "DE>FRS":
                InputPlaceHolder = "Gib ein deutsches Wort ein";
                SearchButtonText  = "Suche";
                ExtendedSearchButtonText = "Erweiterte Suche";
                SearchLikeButtonText = "Unscharfe Suche";
                SearchBeginsButtonText = "Beginnt mit";
                SearchEndsButtonText = "Endet mit";
                SearchExactButtonText = "Exakte Suche";
                break;
            case "EN>FRS":
                InputPlaceHolder = "enter a english word";
                SearchButtonText = "search";
                ExtendedSearchButtonText = "extended search";
                SearchLikeButtonText = "fuzzy search";
                SearchBeginsButtonText = "begins with";
                SearchEndsButtonText = "ends with";
                SearchExactButtonText = "exact search";
                break;
            case "FRS>DE":
            case "FRS>EN":
                InputPlaceHolder = "gif 'n oostfräisk woord in";
                SearchButtonText = "söyken";
                ExtendedSearchButtonText = "ferwiidert söyken";
                SearchLikeButtonText = "unskaarp söyken";
                SearchBeginsButtonText = "fangt an mit";
                SearchEndsButtonText = "ent mit";
                SearchExactButtonText = "eksakt söyken";
                break;
        }
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
