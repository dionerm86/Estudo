<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlParcelas.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlParcelas" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Table ID="tblParcelas" runat="server" OnLoad="tblParcelas_Load">
</asp:Table>
<asp:CustomValidator ID="ctvValorTotal" runat="server" ErrorMessage="Valor total das parcelas difere do valor a pagar."
    ClientValidationFunction="validaValorTotal" Display="None" ValidateEmptyText="True"></asp:CustomValidator>
<asp:ValidationSummary ID="vsuValidacao" runat="server" ShowMessageBox="True" ShowSummary="False" />
<asp:TextBox ID="txtValorParcelas" runat="server" Style="display: none" EnableViewState="false"></asp:TextBox>
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="ObterFormasPagtoParaControleParcelas"
    TypeName="Glass.Data.DAL.FormaPagtoDAO">
</colo:VirtualObjectDataSource>
