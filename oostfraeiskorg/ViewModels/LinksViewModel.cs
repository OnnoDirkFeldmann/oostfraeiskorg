using System.Diagnostics.Metrics;
using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels;

public class LinksViewModel : MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "Links - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Links - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", links, ostfriesische Sprache, ostfriesisch, oostfräisk";
        return base.Init();
    }
}

