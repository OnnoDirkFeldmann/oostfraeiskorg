using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels;

public class InstructionsViewModel : MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "Benutzerhinweise - Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Benutzerhinweise - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", benutzerhinweise, jungfräiske mäinskup, jfm, ostfriesische Sprache, ostfriesisch, ōstfräisk";
        return base.Init();
    }
}

