<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlCondutorVeiculoMDFe.ascx.cs" Inherits="Glass.UI.Web.Controls.MDFe.ctrlCondutorVeiculoMDFe" %>

<table id="tabela" runat="server" class="table-linha" cellpadding="0" cellspacing="0">
    <tr class="dtvRow">
        <td class="dtvHeader">
            Condutor *
        </td>
        <td class="dtvAlternatingRow">
            <asp:DropDownList ID="drpCondutor" runat="server" Width="250px" AppendDataBoundItems="true"
                DataSourceID="odsCondutor" DataTextField="Nome" DataValueField="IdCondutor">
                <asp:ListItem Value="" Text="Selecione"></asp:ListItem>
            </asp:DropDownList>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfCondutores" runat="server" />

<asp:ObjectDataSource ID="odsCondutor" runat="server"
    TypeName="Glass.Data.DAL.CondutoresDAO" DataObjectTypeName="Glass.Data.Model.Condutores" SelectMethod="GetList">
</asp:ObjectDataSource>
