<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlBenef.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlBenef" %>
<style type="text/css">
    .valorBenef
    {
    	border: solid 1px Gray;
        background-color: #DAF1F8;
        width: 50px;
    }
</style>
<asp:Table ID="tblBenef" runat="server" ChdfSalvarBenefellPadding="1" CellSpacing="0" 
    onload="tblBenef_Load">
</asp:Table>
<asp:HiddenField ID="hdfCustoTotal" runat="server" />
<asp:HiddenField ID="hdfValorUnitTotal" runat="server" />
<asp:HiddenField ID="hdfValorTotal" runat="server" />
<asp:HiddenField ID="hdf_benef_IdProdPed" runat="server" />
<asp:CustomValidator ID="ctvEspessura" runat="server" ErrorMessage="A espessura do vidro não é valida." ValidateEmptyText="True" Display="None" ClientValidationFunction="validaEspessuraVidro"></asp:CustomValidator>
<asp:ValidationSummary ID="vsuSumario" runat="server" ShowMessageBox="True" ShowSummary="False" />
