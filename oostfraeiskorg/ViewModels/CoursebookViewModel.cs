using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels;

public class CoursebookViewModel : MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "Lehrbuch - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Lehrbuch - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", Lehrbuch, Kurs, lernen, jungfräiske mäinskup, jfm, ostfriesische Sprache, ostfriesisch, oostfräisk";
        return base.Init();
    }
}

