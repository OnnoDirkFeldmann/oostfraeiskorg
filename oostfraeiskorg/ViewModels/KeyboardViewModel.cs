﻿using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels;

public class KeyboardViewModel : MasterPageViewModel
{
    public override Task Init()
    {
        MasterPageTitle = "Tastatur - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Tastatur - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", Tastaturlayout, Tastatur, ostfriesische Sprache, ostfriesisch, oostfräisk";
        return base.Init();
    }
}

