<%@ Page Language="C#" MasterPageFile="respmaster.master" AutoEventWireup="true" CodeBehind="keyboard.aspx.cs" Inherits="WFDOT.keyboard" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <html xmlns="https://www.w3.org/1999/xhtml">
    <body>
        <div class="container">
            <h2>Ostfriesische Tastaturen</h2>
            <p>
                Um das Ostfriesische zu schreiben, muss man die Vokale mit Makron und Zirkumflex, sowie das æ und das ğ tippen können. 
                Da diese Zeichen auf der deutschen Tastaturbelegung nicht vorhanden sind gibt es installierbare ostfriesische Tastaturen.
                Das Vorgehen dafür ist zumeist sehr einfach, jedoch geräteabhängig.<br />
                <br />
            </p>
            <h3>Windows:</h3>
            <p>
                Für Windows haben wir ein eigenes Tastaturlayout entwickelt. 
                Das Schreiben des Ostfriesischen ist dadurch sehr leicht. 
                Allgemeine Tastenkombination für Zeichen die im Deutschen 
                nicht vorkommen ist SHIFT + ^.<br />
                <br />
                SHIFT + ^ dann A = Ā
                <br />
                SHIFT + ^ dann a = ā
                <br />
                SHIFT + ^ dann E = Ē
                <br />
                SHIFT + ^ dann e = ē
                <br />
                SHIFT + ^ dann I = Ī
                <br />
                SHIFT + ^ dann i = ī
                <br />
                SHIFT + ^ dann O = Ō
                <br />
                SHIFT + ^ dann o = ō
                <br />
                SHIFT + ^ dann U = Ū
                <br />
                SHIFT + ^ dann u = ū
                <br />
                SHIFT + ^ dann Ä = Æ
                <br />
                SHIFT + ^ dann ä = æ
                <br />
                SHIFT + ^ dann G = Ğ
                <br />
                SHIFT + ^ dann g = ğ
                <br />
                <br />
                Die restlichen Zeichen lassen sich wie mit der deutschen 
                Tastatur schreiben:<br />
                <br />
                ^ dann A = Â
                <br />
                ^ dann a = â
                <br />
                ^ dann E = Ê
                <br />
                ^ dann e = ê
                <br />
                ^ dann I = Î
                <br />
                ^ dann i = î
                <br />
                ^ dann O = Ô
                <br />
                ^ dann o = ô
                <br />
                ^ dann U = Û
                <br />
                ^ dann u = û
                <br />
                ´ dann O = Ó
                <br />
                ´ dann o = ó
                <br />
                <br />
                Die Tastatur kann hier heruntergeladen werden:<br />
                <a href='https://download.oostfraeisk.org/keyboard/ofrs.7z'>Download Tastatur Windows</a><br />
                Zur Installation muss die Datei entpackt und die setup.exe gestartet werden. 
                Nach einem Rechnerneustart kann in den Sprachoptionen der Sprache Deutsch die Tastatur aktiviert werden.
                <br />
                <br />
            </p>
            <h3>Mac OS:</h3>
            <p>
                Unter Mac OS existiert noch kein eigenes Tastaturlayout.<br />
                <br />
            </p>
            <h3>Linux:</h3>
            <p>
                Unter Linux kann der sogenannte Compose-Key verwendet werden.<br />
                In den meisten Linux-Distributionen ist der Compose-Key von Haus aus instaliert.<br />
                Unter Ubuntu muss hierfür Tweaks heruntergeladen werden:<br />
                sudo apt install gnome-tweak-tool<br />
                In der Anwendung kann der der Compose-Key dann aktiviert werden.<br />
                <br />
                Folgende Kombinationen ergeben die jeweiligen ostfriesischen Zeichen.<br />
                <br />
                Compose + - dann A = Ā<br />
                Compose + - dann a = ā<br />
                Compose + - dann E = Ē<br />
                Compose + - dann e = ē<br />
                Compose + - dann I = Ī<br />
                Compose + - dann i = ī<br />
                Compose + - dann O = Ō<br />
                Compose + - dann o = ō<br />
                Compose + - dann U = Ū<br />
                Compose + - dann u = ū<br />
                Compose + A dann E = Æ<br />
                Compose + a dann e = æ<br />
                Compose + b dann G = Ğ<br />
                Compose + b dann g = ğ<br />
                <br />
                Die restlichen Zeichen lassen sich wie mit der deutschen Tastatur schreiben:<br />
                <br />
                ^ dann A = Â<br />
                ^ dann a = â<br />
                ^ dann E = Ê<br />
                ^ dann e = ê<br />
                ^ dann I = Î<br />
                ^ dann i = î<br />
                ^ dann O = Ô<br />
                ^ dann o = ô<br />
                ^ dann U = Û<br />
                ^ dann u = û<br />
                ´ dann O = Ó<br />
                ´ dann o = ó<br />
                <br />
            </p>
            <h3>Android:</h3>
            <p>
                Für Android wurde im Rahmen eines Projektes eine interfriesische Tastatur mit dem Namen 
                "Interfrisian Keyboard" entwickelt, die neben diversen friesischen Sprachen auch das Ostfriesische (Ōstfräisk) enthält. 
                Die App ist über den Google PlayStore herunterladbar:<br />
                <a href="https://play.google.com/store/apps/details?id=com.frisian.keyboard" target="blank" rel="external">Interfrisian Keyboard im PlayStore</a><br />
                <br />
                <h3>Apple IOS:</h3>
                Unter IOS existiert noch kein eigenes Tastaturlayout. 
                Durch die Installation der App Swift Key mit den Sprachen 
                Norwegisch, Deutsch und Maori stehen jedoch alle Zeichen zur 
                Verfügung und das Ostfriesische kann getippt werden. 
                Die jeweiligen diakritischen Zeichen sind durch langes Drücken auf 
                ein Zeichen zu erreichen.<br />
                <br />
                <br />
                <br />
                <br />
            </p>
        </div>
    </body>
    </html>
</asp:Content>
