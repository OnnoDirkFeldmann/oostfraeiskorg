using System;
using System.Collections.Generic;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;

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

        public void Search()
        {

        }

        public void SearchLike()
        {

        }

        public void SearchExact()
        {

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
