using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels;

public class CoursebookViewModel : MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "Lehrbuch - Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Lehrbuch - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", Lehrbuch, Kurs, lernen, jungfräiske mäinskup, jfm, ostfriesische Sprache, ostfriesisch, ōstfräisk";
        return base.Init();
    }
}

