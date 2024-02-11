using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels;

public class OnlinecourseenViewModel : MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "online course - Oostfräisk Woordenbauk - East Frisian Dictionary";
        MasterPageDescription = "online course - Dictionary of the East Frisian language - translate words from East Frisian or into East Frisian. Learn the language of the East Frisians as Standard East Frisian with the Dictionary for East Frisian Platt.";
        MasterPageKeywords += ", online course, eastfrisian language, eastfrisian, oostfräisk, course, course of eastfrisian language";
        return base.Init();
    }
}

