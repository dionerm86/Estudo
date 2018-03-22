<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlCidadeCarga.ascx.cs"
    Inherits="Glass.UI.Web.Controls.MDFe.ctrlCidadeCarga" %>

<table id="tabela" runat="server" class="table-linha" cellpadding="0" cellspacing="0">
    <tr>
        <td class="dtvHeader">
            Cidade Carga *
        </td>
        <td class="dtvAlternatingRow">
            <asp:DropDownList ID="drpCidadeCarga" runat="server" Width="250px"
                DataSourceID="odsCidade" DataTextField="NomeCidade" DataValueField="IdCidade" OnDataBound="drpCidadeCarga_DataBound">
            </asp:DropDownList>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfCidadesCarga" runat="server" />

<asp:ObjectDataSource ID="odsCidade" runat="server"
    TypeName="Glass.Data.DAL.CidadeDAO" DataObjectTypeName="Glass.Data.Model.Cidade"
    SelectMethod="GetList"></asp:ObjectDataSource>





