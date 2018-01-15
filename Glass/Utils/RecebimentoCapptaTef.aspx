<%@ Page Title="Recebimento TEF" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true" CodeBehind="RecebimentoCapptaTef.aspx.cs" Inherits="Glass.UI.Web.Utils.RecebimentoCapptaTef" %>

<asp:Content ID="Content1" ContentPlaceHolderID="javaScript" runat="server">

    <script type="text/javascript" src="https://s3.amazonaws.com/cappta.api/js/cappta-checkout.js"></script>
    <script type="text/javascript" src="../scripts/cappta-tef.js"></script>
    <link href="../Style/cappta-tef.css" rel="stylesheet" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Pagina" runat="server">
    <table>
        <tr>
            <td>
                <div class="box-log">
                    <h1>Status</h1>
                    <hr />
                    <h3 class="status-message">Iniciando canal de recebimento...</h3>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
