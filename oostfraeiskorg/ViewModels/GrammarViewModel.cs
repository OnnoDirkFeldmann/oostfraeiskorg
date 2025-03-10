﻿using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels;

public class GrammarViewModel : MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "Grammatik - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Grammatik - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", Grammatik, ostfriesische Sprache, ostfriesisch, oostfräisk, Sprachlehre, Grammatik der ostfriesischen Sprache";
        return base.Init();
    }
}

