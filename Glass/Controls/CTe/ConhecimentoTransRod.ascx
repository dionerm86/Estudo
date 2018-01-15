<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConhecimentoTransRod.ascx.cs"
    Inherits="Glass.UI.Web.Controls.CTe.ConhecimentoTransRod" %>
<%@ Register Src="~/Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/LacreCteRod.ascx" TagName="ctrlLacreRod" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/MotoristaCteRod.ascx" TagName="ctrlMotorista" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/OrdemColetaCteRod.ascx" TagName="ctrlOrdem" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/ValePedagioCteRod.ascx" TagName="ctrlValePedagio"
    TagPrefix="uc1" %>
<div class="dtvRow">
    <div class="dtvHeader">
        Data Prev Entrega
        <%= ObtemTextoCampoObrigatorio(ctrlDataPrevEntrega.Validadores[0], cpvDataPrevEntrega) %>
    </div>
    <div class="dtvAlternatingRow">
        <uc1:ctrlData ID="ctrlDataPrevEntrega" runat="server" ValidateEmptyText="true" ValidationGroup="c"
            ReadOnly="ReadWrite" ErrorMessage="Campo Data Prev Entrega deve ser preenchido."
            ExibirHoras="False" />
        <asp:CompareValidator ID="cpvDataPrevEntrega" runat="server" ErrorMessage="A Data Prev Entrega deve ser maior ou igual a data atual"
            ControlToValidate="ctrlDataPrevEntrega$txtData" Display="Dynamic" Operator="GreaterThanEqual" Type="Date"
            ValidationGroup="c">*</asp:CompareValidator>
    </div>
    <div class="dtvHeader">
        <asp:Label ID="lblLotacao" runat="server" Text="Lotação" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:CheckBox runat="server" ID="chkLotacao" Text="Lotação" />
    </div>
</div>
<div class="dtvRow">
    <div class="dtvHeader">
        <asp:Label ID="lblCiot" runat="server" Text="CIOT" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox ID="txtCIOT" runat="server" MaxLength="12"></asp:TextBox>
    </div>
</div>
<uc1:ctrlLacreRod ID="ctrlLacreRod" runat="server" />
<uc1:ctrlMotorista ID="ctrlMotorista" runat="server" />
<div id="div1" runat="server" onload="div1_Load">
    <uc1:ctrlOrdem ID="ctrlOrdem" runat="server" />
    <uc1:ctrlValePedagio ID="ctrlValePedagio" runat="server" />
</div>
