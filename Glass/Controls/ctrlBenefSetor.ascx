<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlBenefSetor.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlBenefSetor" %>
<asp:Table ID="tblBenef" runat="server" CellPadding="1" CellSpacing="0" 
    onload="tblBenef_Load">
</asp:Table>
<asp:ValidationSummary ID="vsuSumario" runat="server" ShowMessageBox="True" ShowSummary="False" />
<script type="text/javascript">
    exibirTodosOsFilhos("<%= this.ClientID %>");
</script>