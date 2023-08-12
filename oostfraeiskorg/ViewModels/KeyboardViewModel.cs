using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels
{
    public class KeyboardViewModel : MasterPageViewModel
    {
        public override Task Init()
        {
            MasterPageTitle = "Tastatur - Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
            MasterPageDescription = "Tastatur - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
            MasterPageKeywords += "Tastaturlayout, Tastatur, jungfräiske mäinskup, jfm, ostfriesische Sprache, ostfriesisch, ōstfräisk";
            return base.Init();
        }
    }
}

