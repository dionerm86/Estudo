<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelEtiquetaAplicacao.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelEtiquetaAplicacao" Title="Selecione a Aplicação" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setApl(idAplicacao, codInterno, descr)
        {
            if (GetQueryString("buscaComPopup") === "true") {
                var idControle = GetQueryString("id-controle");
                if (idControle) {
                    window.opener.Busca.Popup.atualizar(idControle, null, codInterno);
                    closeWindow();
                    return;
                }
            }

            if (GetQueryString("idProdPed") != "" && GetQueryString("idProdPed") != 'undefined' && GetQueryString("idProdPed") != null)
                window.opener.setAplComposicao(idAplicacao, codInterno, GetQueryString("idProdPed"));
            else
                window.opener.setApl(idAplicacao, codInterno, GetQueryString("idControle"));

            closeWindow();
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdAplicacao" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsAplicacao" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    DataKeyNames="IdAplicacao">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="setApl('<%# Eval("IdAplicacao") %>', '<%# Eval("CodInterno") %>', '<%# Eval("Descricao") %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código" SortExpression="CodInterno">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAplicacao" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetForSelCount" SelectMethod="GetForSel" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.EtiquetaAplicacaoDAO"
                    >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
