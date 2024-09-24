using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels.lessons;

public class Lesson1ViewModel : oostfraeiskorg.ViewModels.MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "Lektion 1 - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Lektion 1 - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", Lektion 1, ostfriesische Sprache, ostfriesisch, oostfräisk, Kurs, Kurs der ostfriesischen Sprache";
        return base.Init();
    }
}

