<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlParcelasSelecionar.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlParcelasSelecionar" %>
<%@ Register Src="ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc1" %>
<asp:DropDownList ID="drpParcelas" runat="server" DataSourceID="odsParcelas" DataTextField="Descricao"
    DataValueField="IdParcela" OnDataBound="drpParcelas_DataBound">
</asp:DropDownList>
<span id="numeroParcelas" runat="server" style="display: none">&nbsp; Número de parcelas:
    <asp:DropDownList ID="drpNumParcCustom" runat="server">
    </asp:DropDownList>
</span>
<asp:CustomValidator ID="ctvValidaParc" runat="server" ErrorMessage="Selecione uma parcela"
    ClientValidationFunction="validaParcelaSel" ControlToValidate="drpParcelas" Display="Dynamic"
    ValidateEmptyText="True"></asp:CustomValidator>
<asp:HiddenField ID="hdfIdParcela" runat="server" />
<asp:HiddenField ID="hdfNumParcelas" runat="server" />
<asp:HiddenField ID="hdfDiasParcelas" runat="server" />
<asp:HiddenField ID="hdfTipoConsulta" runat="server" Value="2" />
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsParcelas" runat="server" SelectMethod="GetForControleSelecionar"
    TypeName="Glass.Data.DAL.ParcelasDAO">
    <SelectParameters>
        <asp:ControlParameter ControlID="hdfTipoConsulta" Name="tipo" PropertyName="Value"
            Type="Object" />
    </SelectParameters>
</colo:VirtualObjectDataSource>
