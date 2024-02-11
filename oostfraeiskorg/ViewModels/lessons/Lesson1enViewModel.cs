using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels.lessons;

public class Lesson1enViewModel : oostfraeiskorg.ViewModels.MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "lesson 1 - Oostfräisk Woordenbauk - East Frisian Dictionary";
        MasterPageDescription = "lesson 1 - Dictionary of the East Frisian language - translate words from East Frisian or into East Frisian. Learn the language of the East Frisians as Standard East Frisian with the Dictionary for East Frisian Platt.";
        MasterPageKeywords += ", lesson 1, eastfrisian language, eastfrisian, oostfräisk, course, course of eastfrisian language";
        return base.Init();
    }
}

