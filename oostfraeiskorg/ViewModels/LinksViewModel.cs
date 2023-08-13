using System.Diagnostics.Metrics;
using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels;

public class LinksViewModel : MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "Links - Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Links - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", links, jungfräiske mäinskup, jfm, ostfriesische Sprache, ostfriesisch, ōstfräisk";
        return base.Init();
    }
}

