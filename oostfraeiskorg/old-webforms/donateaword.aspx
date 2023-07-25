<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="donateaword.aspx.cs" Inherits="WFDOT.donateaword" MasterPageFile="respmaster.master" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <html xmlns="https://www.w3.org/1999/xhtml">
    <body>
        <div class="container">
            <h2>Fehlendes Wort spenden</h2>
            <h4>
                Hier können Sie uns ostfriesische Wörter oder Phrasen mitteilen, die noch nicht in unserem Online-Wörterbuch zu finden sind. 
                Wir werden Ihre Mitteilung prüfen und gegebenenfalls in unser Wörterbuch aufnehmen.
            </h4>
            <span>Wort/Phrase</span>
            <asp:TextBox ID="txt_word" runat="server" class="form-control"></asp:TextBox>
            <span>Deutsche Übersetzung</span>
            <asp:TextBox ID="txt_german" runat="server" class="form-control"></asp:TextBox>
            <span>Gegend in der das Wort gebraucht wird</span>
            <asp:TextBox ID="txt_place" runat="server" class="form-control"></asp:TextBox>
            <span>Name, Vorname (optional)</span>
            <asp:TextBox ID="txt_name" runat="server" class="form-control"></asp:TextBox>
            <span>Geburtsjahr (optional)</span>
            <asp:TextBox ID="txt_birth" runat="server" class="form-control"></asp:TextBox>
            <span>Wohnort (optional)</span>
            <asp:TextBox ID="txt_home" runat="server" class="form-control"></asp:TextBox>
            <span>Emailadresse zur Kontaktaufnahme (optional)</span>
            <asp:TextBox ID="txt_email" runat="server" class="form-control"></asp:TextBox>
            <span>Sonstiges (optional)</span>
            <asp:TextBox ID="txt_other" runat="server" class="form-control"></asp:TextBox>
            <br />
            <div class="float-right">
                <asp:Button ID="btn_submit" runat="server" class="btn btn-secondary bg-primary" Text="Wort spenden" OnClick="btn_submit_Click" />
            </div>
            <br />
            <br />
        </div>
    </body>
    </html>
</asp:Content>
