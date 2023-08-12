﻿using System.Collections.Generic;
using DotVVM.Framework.ViewModel;

namespace oostfraeiskorg.ViewModels
{
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

        private bool _showKeyboard = false;

        public bool ShowKeyboard
        {
            get => _showKeyboard;
            set => _showKeyboard = value;
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set => _searchText = value;
        }

        private string _masterPageTitle = "Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
        public string MasterPageTitle
        {
            get => _masterPageTitle;
            set => _masterPageTitle = value;
        }

        private string _masterPageDescription = "Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
        public string MasterPageDescription
        {
            get => _masterPageDescription;
            set => _masterPageDescription = value;
        }

        private string _masterPageKeywords = "platt, ostfriesisch, Wörterbuch, Ostfriesland, plattdeutsch, ostfriesisich-deutsch Wörterbuch, deutsch-ostfriesisch-Wörterbuch, elektronisches Wörterbuch, Onlinewörterbuch, Wörterbuch online, ostfriesich online, ōstfräisk, Jungfräiske Mäinskup, JFM, JFM Ostfriesland, Sprache von Ostfriesland, Ostfriesland Platt, Ostfriesisches Platt, ostfriesische Sprache, Regionalsprache, Wörterbuch der ostfriesischen Sprache, Weigelt, Schreibweise, Rechtschreibung, ostfriesische Schreibweise, friesisch schreiben, ostfriesisch lernen";
        public string MasterPageKeywords
        {
            get => _masterPageKeywords;
            set => _masterPageKeywords = value;
        }

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
}
