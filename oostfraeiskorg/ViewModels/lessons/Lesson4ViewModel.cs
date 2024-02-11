using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels.lessons;

public class Lesson4ViewModel : oostfraeiskorg.ViewModels.MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "Lektion 4 - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Lektion 4 - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", Lektion 4, ostfriesische Sprache, ostfriesisch, oostfräisk, Kurs, Kurs der ostfriesischen Sprache";
        return base.Init();
    }
}

