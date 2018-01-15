<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelPopup.aspx.cs" Inherits="Glass.UI.Web.Utils.SelPopup"
    MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        var selecionado = false;

        function validar(controle, id, descr)
        {
            if (selecionado)
                return;

            selecionado = true;

            controle = eval("window.opener." + controle);
            controle.AlteraValor(id, descr);
            closeWindow();
        }
    </script>

    <table>
        <tr runat="server" id="filtros">
            <td align="center">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="center">
                            <asp:Table ID="tblFiltros" runat="server">
                            </asp:Table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdBuscar" runat="server" DataSourceID="odsBuscar"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="A pesquisa não retornou resultados."
                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="return validar('<%= Request["controle"] %>', '<%# Eval(Decode(Request["colunaId"])) %>', '<%# Eval(Decode(Request["colunaDescr"])) %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsBuscar" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
