<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlSelCidade.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlSelCidade" %>
<%@ Register Src="ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<table class="pos" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <asp:DropDownList ID="drpUf" runat="server" DataSourceID="odsUf" DataTextField="Value"
                DataValueField="Key" onchange="limpar(this)">
            </asp:DropDownList>
        </td>
        <td>
            <uc1:ctrlSelPopup ID="ctrlSelCidade1" runat="server" DataSourceID="odsCidade" DataTextField="NomeCidade" 
                DataValueField="IdCidade" Descricao='<%# Eval("NomeCidade") %>' ExibirIdPopup="False"
                TituloTela="Selecione a Cidade" Valor='<%# Bind("IdCidade") %>' UsarValorRealControle="True"
                ColunasExibirPopup="IdCidade|NomeCidade|NomeUf|CodUfMunicipio" TitulosColunas="Id|Nome|UF|Cód. IBGE"
                TextWidth="100px" />
        </td>
    </tr>
</table>
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsUf" runat="server" SelectMethod="GetUf"
    TypeName="Glass.Data.DAL.CidadeDAO">
</colo:VirtualObjectDataSource>
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCidade" runat="server" SelectMethod="GetByUf"
    TypeName="Glass.Data.DAL.CidadeDAO">
    <SelectParameters>
        <asp:ControlParameter ControlID="drpUf" Name="uf" PropertyName="SelectedValue" Type="String" />
    </SelectParameters>
</colo:VirtualObjectDataSource>
