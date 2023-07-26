using System;
using System.Collections.Generic;
using System.Linq;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WFDOT;

namespace oostfraeiskorg.ViewModels
{
    public class MasterPageViewModel : DotvvmViewModelBase
    {
        public List<string> Languages { get => new List<string>() { "DE>FRS", "FRS>DE", "EN>FRS", "FRS>EN" }; }

        private string _selectedLanguage = "DE>FRS";

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => _selectedLanguage = value;
        }

        private string _lbl_keyboard = "collapse";

        public string lbl_keyboard {
            get => _lbl_keyboard;
            set => _lbl_keyboard = value;
        }

        private string _txt_search = "";
        public string txt_search
        {
            get => _txt_search;
            set => _txt_search = value;
        }

        private static IQueryable<Entry> GetData()
        {
            return new[]
            {
                new Entry("fründskup", "Freundschaft"),
                new Entry("mäinskup", "Gemeinschaft"),
                new Entry("hôp", "Hoffnung"),
                new Entry("lauğ", "Dorf")
            }.AsQueryable();
        }

        public GridViewDataSet<Entry> Entries { get; set; }

        public MasterPageViewModel() {
            Entries = new GridViewDataSet<Entry>();
        }

        public class Entry
        {
            public string Frisian { get; set; }
            public string Translation { get; set; }

            public Entry()
            {
                // NOTE: This default constructor is required. 
                // Remember that the viewmodel is JSON-serialized
                // which requires all objects to have a public 
                // parameterless constructor
            }

            public Entry(string frisian, string translation)
            {
                Frisian = frisian;
                Translation = translation;
            }
        }

        public void Search()
        {
            Entries.LoadFromQueryable(WFDOT.Searcher.SearchAndFill(txt_search, SelectedLanguage, "J"));
        }

        public void SearchLike()
        {
            Entries.LoadFromQueryable(WFDOT.Searcher.SearchAndFill(txt_search, SelectedLanguage, "N"));
        }

        public void SearchExact()
        {
            Entries.LoadFromQueryable(WFDOT.Searcher.SearchAndFill(txt_search, SelectedLanguage, "X"));
        }

        public void btn_keyboard_Click()
        {
            lbl_keyboard = lbl_keyboard == "collapse" ? "expand" : "collapse";
        }

        public void btn_symbols_Click(string text)
        {
            txt_search += text;
        }

    }
}
