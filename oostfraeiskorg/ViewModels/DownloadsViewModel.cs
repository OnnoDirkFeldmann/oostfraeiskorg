using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels
{
    public class DownloadsViewModel : MasterPageViewModel
    {
        public override Task Init()
        {
            MasterPageTitle = "Downloads - Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
            MasterPageDescription = "Downloads - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
            MasterPageKeywords += ", downloads, jungfräiske mäinskup, jfm, ostfriesische Sprache, ostfriesisch, ōstfräisk";
            return base.Init();
        }

    }
}

