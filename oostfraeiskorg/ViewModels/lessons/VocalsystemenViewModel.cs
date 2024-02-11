using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels.lessons;

public class VocalsystemenViewModel : oostfraeiskorg.ViewModels.MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "vocalsystem - Oostfräisk Woordenbauk - East Frisian Dictionary";
        MasterPageDescription = "vocalsystem - Dictionary of the East Frisian language - translate words from East Frisian or into East Frisian. Learn the language of the East Frisians as Standard East Frisian with the Dictionary for East Frisian Platt.";
        MasterPageKeywords += ", vocalsystem, eastfrisian language, eastfrisian, oostfräisk, course, course of eastfrisian language";
        return base.Init();
    }
}

