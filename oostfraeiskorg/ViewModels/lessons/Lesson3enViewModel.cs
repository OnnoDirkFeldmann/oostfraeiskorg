using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels.lessons;

public class Lesson3enViewModel : oostfraeiskorg.ViewModels.MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "lesson 3 - Oostfräisk Woordenbauk - East Frisian Dictionary";
        MasterPageDescription = "lesson 3 - Dictionary of the East Frisian language - translate words from East Frisian or into East Frisian. Learn the language of the East Frisians as Standard East Frisian with the Dictionary for East Frisian Platt.";
        MasterPageKeywords += ", lesson 3, eastfrisian language, eastfrisian, oostfräisk, course, course of eastfrisian language";
        return base.Init();
    }
}

