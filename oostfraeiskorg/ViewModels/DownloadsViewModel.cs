using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels;

public class DownloadsViewModel : MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "Downloads - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Downloads - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", downloads, ostfriesische Sprache, ostfriesisch, oostfräisk";
        return base.Init();
    }

}

