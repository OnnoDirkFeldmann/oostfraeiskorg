using System.Collections.Generic;
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

        public void Search()
        {

        }

        public void SearchLike()
        {

        }

        public void SearchExact()
        {

        }

    }
}
