<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="WFDOT.main" MasterPageFile="respmaster.master" MaintainScrollPositionOnPostback="true" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <html xmlns="https://www.w3.org/1999/xhtml" lang="de">
    <body>
        <!-- Such-Resultat -->
        <div class="container-fluid justify-content-center">

            <div class="container justify-content-center">
                <asp:Table ID="tbResult" runat="server" class="table table-striped table-responsive-md"></asp:Table>
            </div>
        </div>
    </body>
    </html>
</asp:Content>
