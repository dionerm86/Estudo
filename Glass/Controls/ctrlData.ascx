<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlData.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlData" %>
<span style="white-space: nowrap">
    <asp:TextBox ID="txtData" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
        MaxLength="10"></asp:TextBox>
    <asp:TextBox ID="txtHora" runat="server" Width="44px" Visible="false" MaxLength="5"
        onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
    <asp:ImageButton ID="imgData" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
        ToolTip="Alterar" />
    <asp:CustomValidator ID="ctvData" runat="server" ErrorMessage="*" ClientValidationFunction="validaData"
        ControlToValidate="txtData" Display="Dynamic">*</asp:CustomValidator>
    <asp:CustomValidator ID="ctvHora" runat="server" ErrorMessage="*" ClientValidationFunction="validaHora"
        ControlToValidate="txtHora" Display="Dynamic" Enabled="False">*</asp:CustomValidator>
</span>