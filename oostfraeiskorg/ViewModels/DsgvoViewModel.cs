using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels
{
    public class DsgvoViewModel : MasterPageViewModel
    {
        public override Task Init()
        {
            MasterPageTitle = "Datenschutzerklärung - Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
            MasterPageDescription = "Datenschutz - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
            MasterPageKeywords = "Datenschutzerklärung, dsgvo, jungfräiske mäinskup, jfm, ostfriesische Sprache, ostfriesisch, ōstfräisk";
            return base.Init();
        }
    }
}

