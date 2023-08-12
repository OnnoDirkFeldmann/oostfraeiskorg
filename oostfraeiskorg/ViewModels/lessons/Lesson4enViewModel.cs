using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels.lessons
{
    public class Lesson4enViewModel : oostfraeiskorg.ViewModels.MasterPageViewModel
    {
        public override Task Init()
        {
            MasterPageTitle = "lesson 4 - Ōstfräisk Wōrdenbauk - East Frisian Dictionary";
            MasterPageDescription = "lesson 4 - Dictionary of the East Frisian language - translate words from East Frisian or into East Frisian. Learn the language of the East Frisians as Standard East Frisian with the Dictionary for East Frisian Platt.";
            MasterPageKeywords += ", lesson 4, eastfrisian language, eastfrisian, ōstfräisk, course, course of eastfrisian language";
            return base.Init();
        }
    }
}

