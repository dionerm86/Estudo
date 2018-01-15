<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConhecimentoTransRod.ascx.cs"
    Inherits="Glass.UI.Web.Controls.CTe.ConhecimentoTransRod" %>
<%@ Register Src="~/Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/LacreCteRod.ascx" TagName="ctrlLacreRod" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/CTe/OrdemColetaCteRod.ascx" TagName="ctrlOrdem" TagPrefix="uc3" %>
<%@ Register Src="~/Controls/CTe/ValePedagioCteRod.ascx" TagName="ctrlValePedagio" TagPrefix="uc4" %>
<div class="dtvRow">
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
<uc2:ctrlLacreRod ID="ctrlLacreRod" runat="server" />
<div id="div1" runat="server" onload="div1_Load">
    <uc3:ctrlOrdem ID="ctrlOrdem" runat="server" />
    <uc4:ctrlValePedagio ID="ctrlValePedagio" runat="server" />
</div>
