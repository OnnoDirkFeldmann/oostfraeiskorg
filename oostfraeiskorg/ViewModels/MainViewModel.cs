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
    }
}
