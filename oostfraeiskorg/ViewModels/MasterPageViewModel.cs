using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Hosting;
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

        private bool _lbl_keyboard = false;

        public bool lbl_keyboard {
            get => _lbl_keyboard;
            set => _lbl_keyboard = value;
        }

        private string _txt_search = "";
        public string txt_search
        {
            get => _txt_search;
            set => _txt_search = value;
        }

        public GridViewDataSet<Entry> Entries { get; set; }

        [FromQuery("W")]
        public string W { get; set; }

        [FromQuery("df")]
        public string df { get; set; }

        [FromQuery("fts")]
        public string fts { get; set; }

        public MasterPageViewModel() {
            Entries = new GridViewDataSet<Entry>();
            lbl_keyboard = false;
        }
        public override Task Init()
        {
            if (W != null && df != null && fts != null)
            {
                Entries = new GridViewDataSet<Entry>();

                //Alte URL-Versionen händeln
                if (df == "ofrs" || df == "frs")
                {
                    df = "frs>de";
                }
                if (df == "de")
                {
                    df = "de>frs";
                }

                //Default Sprache
                if (df != "de>frs" && df != "frs>de" && df != "en>frs" && df != "frs>en")
                {
                    df = "de>frs";
                }

                //Default Search
                if (fts != "J" && fts != "N" && fts != "X")
                {
                    fts = "N";
                }

                if (W.Contains("%"))
                {
                    Context.RedirectToRoute("main");
                }
                else
                {
                    Entries.LoadFromQueryable(WFDOT.Searcher.SearchAndFill(W, df, fts));
                }
            }
            return base.Init();
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
            Context.RedirectToRoute("main", query: new { W = txt_search, df = SelectedLanguage.ToLower(), fts = "J" });
        }

        public void SearchLike()
        {
            Context.RedirectToRoute("main", query: new { W = txt_search, df = SelectedLanguage.ToLower(), fts = "N" });
        }

        public void SearchExact()
        {
            Context.RedirectToRoute("main", query: new { W = txt_search, df = SelectedLanguage.ToLower(), fts = "X" });
        }

        public void btn_keyboard_Click()
        {
            lbl_keyboard = lbl_keyboard ? false : true;
        }

        public void btn_symbols_Click(string text)
        {
            txt_search += text;
        }

    }
}
