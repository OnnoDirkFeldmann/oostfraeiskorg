using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels.lessons;

public class VocalsystemViewModel : oostfraeiskorg.ViewModels.MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "Vokalsystem - Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Vokalsystem - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", Vokalsystem, ostfriesische Sprache, ostfriesisch, ōstfräisk, Kurs, Kurs der ostfriesischen Sprache";
        return base.Init();
    }
}

