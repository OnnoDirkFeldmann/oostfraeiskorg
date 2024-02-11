using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels.lessons;

public class Lesson2enViewModel : oostfraeiskorg.ViewModels.MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "lesson 2 - Oostfräisk Woordenbauk - East Frisian Dictionary";
        MasterPageDescription = "lesson 2 - Dictionary of the East Frisian language - translate words from East Frisian or into East Frisian. Learn the language of the East Frisians as Standard East Frisian with the Dictionary for East Frisian Platt.";
        MasterPageKeywords += ", lesson 2, eastfrisian language, eastfrisian, oostfräisk, course, course of eastfrisian language";
        return base.Init();
    }
}

