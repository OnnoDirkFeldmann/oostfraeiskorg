using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels.lessons
{
    public class Lesson3ViewModel : oostfraeiskorg.ViewModels.MasterPageViewModel
    {
        public override Task Init()
        {
            MasterPageTitle = "Lektion 3 - Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
            MasterPageDescription = "Lektion 3 - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
            MasterPageKeywords += ", Lektion 3, ostfriesische Sprache, ostfriesisch, ōstfräisk, Kurs, Kurs der ostfriesischen Sprache";
            return base.Init();
        }
    }
}

