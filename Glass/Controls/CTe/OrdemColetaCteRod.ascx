<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrdemColetaCteRod.ascx.cs"
    Inherits="Glass.UI.Web.Controls.CTe.OrdemColetaCteRod" %>
<%@ Register Src="~/Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<table runat="server" id="tabelaOrdemColeta" class="table-linha" cellpadding="1" cellspacing="1">
    <tr style="margin-bottom: 15px">
        <td class="dtvHeader">
            Transportador
        </td>
        <td class="dtvAlternatingRow">
            <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True"
                DataSourceID="odsTransportador" DataTextField="Nome" DataValueField="IdTransportador">
                <asp:ListItem Value="0" Text="Selecione"></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td class="dtvHeader">
            Número Ordem Coleta
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox runat="server" ID="txtNumeroOrdColeta" MaxLength="6" Width="50px"></asp:TextBox>
        </td>
        <td class="dtvHeader">
            Série
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox runat="server" ID="txtSerieTrans" MaxLength="3" Width="25px"></asp:TextBox>
        </td>
        <td class="dtvHeader">
            Data Emissão
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtData0" runat="server" Width="70px" MaxLength="10" />
            <asp:ImageButton ID="imgData" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                ToolTip="Alterar" />
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfIdTransportador" runat="server" />
<asp:HiddenField ID="hdfNumOrdemColeta" runat="server" />
<asp:HiddenField ID="hdfSerie" runat="server" />
<asp:HiddenField ID="hdfDataEmissao" runat="server" />
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsTransportador" runat="server" SelectMethod="GetOrdered"
    TypeName="Glass.Data.DAL.TransportadorDAO" />
