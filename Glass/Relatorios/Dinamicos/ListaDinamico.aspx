<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaDinamico.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Dinamicos.ListaDinamico" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <table>
        <tr>
            <td align="center">
                <asp:Table ID="tbFiltros" runat="server">
                </asp:Table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInsercao" runat="server"></asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdDinamico" runat="server" AllowPaging="True"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" 
                    EditRowStyle-CssClass="edit" PageSize="10" EmptyDataText="Nenhum registro encontrado" OnPageIndexChanging="grdDinamico_PageIndexChanging">
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClick="lnkImprimir_Click"> 
                    <img border="0" src="../../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClick="lnkExportarExcel_Click">
                    <img border="0" src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
