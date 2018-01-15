<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeguroCte.ascx.cs" Inherits="Glass.UI.Web.Controls.CTe.SeguroCte" %>
<table runat="server" id="tabelaSeguroCte" class="table-linha" cellpadding="1" cellspacing="1">
    <tr style="margin-bottom: 15px">
        <td class="dtvHeader">
            Responsável Seguro
            <%= ObtemTextoCampoObrigatorio(cvDrpRespSeguro) %>
        </td>
        <td class="dtvAlternatingRow">
            <asp:DropDownList ID="drpRespSeguro" runat="server" Height="20px" Width="180px">
                <asp:ListItem Value="6" Text="Selecione"></asp:ListItem>
                <asp:ListItem Value="0" Text="Remetente"></asp:ListItem>
                <asp:ListItem Value="1" Text="Expedidor"></asp:ListItem>
                <asp:ListItem Value="2" Text="Recebedor"></asp:ListItem>
                <asp:ListItem Value="3" Text="Destinatário"></asp:ListItem>
                <asp:ListItem Value="4" Text="Emitente do CT-e"></asp:ListItem>
                <asp:ListItem Value="5" Text="Tomador de Serviço"></asp:ListItem>
            </asp:DropDownList>
            <asp:CompareValidator ID="cvDrpRespSeguro" ControlToValidate="drpRespSeguro" runat="server"
                ErrorMessage="Selecione Responsável Seguro" ValueToCompare="6" Operator="NotEqual"
                ValidationGroup="c">*</asp:CompareValidator>
        </td>
        <td class="dtvHeader">
            Nome Seguradora
        </td>
        <td class="dtvAlternatingRow">
            <asp:DropDownList ID="drpSeguradora" runat="server" DataSourceID="odsSeguradora"
                DataTextField="NomeSeguradora" DataValueField="IdSeguradora" Height="20px" Width="150px"
                AppendDataBoundItems="True" Enabled="true" Visible="true">
                <asp:ListItem Value="selecione" Text="Selecione"></asp:ListItem>
            </asp:DropDownList>
            <%--<asp:CompareValidator ID="cvSeguradora" ControlToValidate="drpSeguradora" runat="server"
                ErrorMessage="Selecione Seguradora" ValueToCompare="selecione" Operator="NotEqual"
                ValidationGroup="c">*</asp:CompareValidator>--%>
        </td>
        <td class="dtvHeader">
            Número Apólice
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtNumApolice" runat="server" MaxLength="20" Width="150px"></asp:TextBox>
        </td>
        <td class="dtvHeader">
            Número Averbação
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtNumAverbacao" runat="server" MaxLength="20" Width="125px"></asp:TextBox>
        </td>
        <td class="dtvHeader">
            Valor Carga Averbação
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtValorCargaAverbacao" runat="server" MaxLength="20" onclick="mascaraValor(this, 2); return false;"
                Width="140px"></asp:TextBox>
            <%--<asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                Style="display: none" />--%>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfIdRespSeguro" runat="server" />
<asp:HiddenField ID="hdfIdSeguradora" runat="server" />
<asp:HiddenField ID="hdfNumApolice" runat="server" />
<asp:HiddenField ID="hdfNumAverbacao" runat="server" />
<asp:HiddenField ID="hdfValorCargaAverbacao" runat="server" />
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSeguradora" runat="server" DataObjectTypeName="Glass.Data.Model.CTe.Seguradora"
    SelectMethod="GetList" TypeName="Glass.Data.DAL.CTe.SeguradoraDAO" EnablePaging="True"
    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SortParameterName="sortExpression"
    StartRowIndexParameterName="startRow">
</colo:VirtualObjectDataSource>
