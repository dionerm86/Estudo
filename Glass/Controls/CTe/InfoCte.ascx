<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InfoCte.ascx.cs" Inherits="Glass.UI.Web.Controls.CTe.InfoCte" %>
<%@ Register Src="~/Controls/CTe/InfoCargaCte.ascx" TagName="ctrlCargaCte" TagPrefix="uc1" %>
<uc1:ctrlCargaCte ID="ctrlCargaCte" runat="server" />
<div class="dtvRow">
    <div class="dtvHeader">
        Produto Predominante
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox ID="txtProdutoPredominante"  runat="server" Width="300px"/>
        <asp:RequiredFieldValidator ID="rfvProdutoPredominante" runat="server" ErrorMessage="Campo: Produto Predominante não pode ser vazio."
            ControlToValidate="txtProdutoPredominante" ValidationGroup="c">*</asp:RequiredFieldValidator>
    </div>
    <div class="dtvHeader">
        <asp:Label ID="Label3" runat="server" Text="Valor Carga" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox ID="txtValorCarga" runat="server" onclick="mascaraValor(this, 2); return false;"></asp:TextBox>
    </div>
</div>
<div class="dtvRow">
    <div class="dtvHeader">
        <asp:Label ID="Label4" runat="server" Text="Outras Características" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox ID="txtOutrasCaract" runat="server" Width="250px" MaxLength="30"></asp:TextBox>
    </div>
</div>
