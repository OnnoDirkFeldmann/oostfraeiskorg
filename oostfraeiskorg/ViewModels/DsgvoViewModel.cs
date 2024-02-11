using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels;

public class DsgvoViewModel : MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "Datenschutzerklärung - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Datenschutz - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords = ", Datenschutzerklärung, dsgvo, jungfräiske mäinskup, jfm, ostfriesische Sprache, ostfriesisch, oostfräisk";
        return base.Init();
    }
}

