using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels;

public class InstructionsViewModel : MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "Benutzerhinweise - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Benutzerhinweise - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", benutzerhinweise, ostfriesische Sprache, ostfriesisch, oostfräisk";
        return base.Init();
    }
}

