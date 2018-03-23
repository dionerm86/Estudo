<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlUFPercursoMDFe.ascx.cs"
    Inherits="Glass.UI.Web.Controls.MDFe.ctrlUFPercursoMDFe" %>

<table id="tabela" runat="server" class="table-linha" cellpadding="0" cellspacing="0">
    <tr>
        <td class="dtvHeader">
            UF Percurso
        </td>
        <td class="dtvAlternatingRow">
            <asp:DropDownList ID="drpUFPercurso" runat="server" Width="250px" AppendDataBoundItems="true"
                DataSourceID="odsUFPercurso" DataTextField="NomeUf" DataValueField="NomeUf" OnDataBound="drpUFPercurso_DataBound">
                <asp:ListItem Value="" Text="Selecione"></asp:ListItem>
            </asp:DropDownList>
            <asp:Image ID="imgUFPercurso" runat="server" ImageUrl="~/Images/Help.gif"
                ToolTip="Não é necessário repetir as UF de Início e Fim"></asp:Image>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfUFsPercurso" runat="server" />

<asp:ObjectDataSource ID="odsUFPercurso" runat="server"
    TypeName="Glass.Data.DAL.CidadeDAO" DataObjectTypeName="Glass.Data.Model.Cidade"
    SelectMethod="ObterUF">
</asp:ObjectDataSource>
