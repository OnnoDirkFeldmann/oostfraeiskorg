using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels;

public class AiAudioViewModel : MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "KI-Sprachausgabe prüfen - Oostfräisk Woordenbauk";
        MasterPageDescription = "KI-Kennzeichnung und AudioSeal-Wasserzeichen in Ooversetter-Audiodateien prüfen.";
        return base.Init();
    }
}
