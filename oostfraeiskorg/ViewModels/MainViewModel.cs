using DotVVM.Framework.Controls;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;

namespace oostfraeiskorg.ViewModels
{
    public class MainViewModel : MasterPageViewModel
    {

        public MainViewModel()
		{
            Entries = new GridViewDataSet<Entry>();
        }

        [FromQuery("W")]
        public string W { get; set; }

        [FromQuery("df")]
        public string df { get; set; }

        [FromQuery("fts")]
        public string fts { get; set; }

        public GridViewDataSet<Entry> Entries { get; set; }

        private string _eastFrisianHeader = "Ostfriesisch";

        public string EastFrisianHeader
        {
            get => _eastFrisianHeader;
            set => _eastFrisianHeader = value;
        }

        private string _translationHeader = "Deutsch";

        public string TranslationHeader
        {
            get => _translationHeader;
            set => _translationHeader = value;
        }

        private string _PopUpBody = "";

        public string PopUpBody
        {
            get => _PopUpBody;
            set => _PopUpBody = value;
        }

        private long _wordid;

        public long wordid
        {
            get => _wordid;
            set => _wordid = value;
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

                UpdateHeader();

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
        
        public void UpdateHeader()
        {
            if (df.Equals("de>frs") || SelectedLanguage.Equals("frs>de"))
            {
                EastFrisianHeader = "Ostfriesisch";
                TranslationHeader = "Deutsch";
            }
            else
            {
                EastFrisianHeader = "East Frisian";
                TranslationHeader = "English";
            }
        }

        public void GetPopUpBody()
        {
            PopUpBody = WFDOT.Searcher.GetPopUpBody(wordid);
        }
    }
}
