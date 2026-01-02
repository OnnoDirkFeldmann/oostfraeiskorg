using DotVVM.Framework.Controls;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;

namespace oostfraeiskorg.ViewModels;

public class MainViewModel : MasterPageViewModel
{

    public MainViewModel()
    {
        Entries = new GridViewDataSet<DictionaryEntry>();
        CurrentLanguage = oostfraeiskorg.Languages.German;
    }

    [FromQuery("W")]
    public string W { get; set; }

    [FromQuery("df")]
    public string df { get; set; }

    [FromQuery("fts")]
    public string fts { get; set; }

    public GridViewDataSet<DictionaryEntry> Entries { get; set; }

    public string EastFrisianHeader { get; set; } = "Ostfriesisch";

    public string TranslationHeader { get; set; } = "Deutsch";

    public string PopUpBody { get; set; } = "";

    public long WordId { get; set; }

    public string MorePhrasesText { get; set; } = "Weitere Phrasen";

    public string CloseButtonText { get; set; } = "Schließen";

    public oostfraeiskorg.Languages CurrentLanguage { get; set; }

    public override Task Init()
    {
        if (W != null)
        {
            Entries = new GridViewDataSet<DictionaryEntry>();

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
            if (fts != "J" && fts != "N" && fts != "X" && fts != "B" && fts != "E")
            {
                fts = "N";
            }

            if (W.Contains("%"))
            {
                Context.RedirectToRoute("main");
            }
            else
            {
                Entries.LoadFromQueryable(Searcher.SearchAndFill(W, df, fts));
            }

            switch (df)
            {
                case "de>frs":
                case "frs>de":
                    MasterPageTitle = $"Suche nach {W}({df}) - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
                    MasterPageDescription = $"Übersetzung für {W}({df}) auf Ostfriesisch - Wörterbuch der ostfriesischen Sprache";
                    break;
                case "en>frs":
                case "frs>en":
                    MasterPageTitle = $"Searched for {W}({df})) - Oostfräisk Woordenbauk - East Frisian Dictionary";
                    MasterPageDescription = $"Translation for {W}({df})) into East Frisian - Dictionary of the East Frisian Language";
                    break;
            }
        }
        return base.Init();
    }

    public void UpdateHeader()
    {
        if (df.Equals("de>frs") || df.Equals("frs>de"))
        {
            EastFrisianHeader = "Ostfriesisch";
            TranslationHeader = "Deutsch";
            MorePhrasesText = "Weitere Phrasen";
            CloseButtonText = "Schließen";
            CurrentLanguage = oostfraeiskorg.Languages.German;
        }
        else
        {
            EastFrisianHeader = "East Frisian";
            TranslationHeader = "English";
            MorePhrasesText = "More phrases";
            CloseButtonText = "Close";
            CurrentLanguage = oostfraeiskorg.Languages.English;
        }
    }

    public void GetPopUpBody()
    {
        PopUpBody = Searcher.GetPopUpBody(WordId, CurrentLanguage);
    }
}
