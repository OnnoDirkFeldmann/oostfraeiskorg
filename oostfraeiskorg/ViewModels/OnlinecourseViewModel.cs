using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Hosting;
using System.Diagnostics.Metrics;

namespace oostfraeiskorg.ViewModels
{
    public class OnlinecourseViewModel : MasterPageViewModel
    {
        public override Task Init()
        {
            MasterPageTitle = "Onlinekurs - Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
            MasterPageDescription = "Onlinekurs - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
            MasterPageKeywords += ", Onlinekurs, ostfriesische Sprache, ostfriesisch, ōstfräisk, Kurs, Kurs der ostfriesischen Sprache";
            return base.Init();
        }
    }
}

