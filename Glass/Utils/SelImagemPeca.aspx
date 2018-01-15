<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelImagemPeca.aspx.cs" Inherits="Glass.UI.Web.Utils.SelImagemPeca"
    Title="Imagem para a peça" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <asp:Label ID="lblMensagem" runat="server" Font-Bold="False" Font-Italic="True" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPecas" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsPecas" CssClass="gridStyle" DataKeyNames="IdProdPed" ShowHeader="False">
                    <Columns>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0" width="100%" style="margin-top: 0px">
                                    <tr>
                                        <td>
                                            <table style="margin-top: 0px">
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="lblTituloProduto" runat="server" Text="Produto" Font-Bold="true"></asp:Label>
                                                        <asp:Label ID="lblCodInterno" runat="server" Text='<%# Eval("CodInterno") %>'></asp:Label>
                                                        -
                                                        <asp:Label ID="lblDescricao" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="lblTituloAmbiente" runat="server" Text="Ambiente" Font-Bold="true"></asp:Label>
                                                        <asp:Label ID="lblAmbiente" runat="server" Text='<%# Eval("AmbientePedido") %>'></asp:Label>
                                                        &nbsp;&nbsp;
                                                        <asp:Label ID="lblTituloProc" runat="server" Text="Processo" Font-Bold="true"></asp:Label>
                                                        <asp:Label ID="lblProc" runat="server" Text='<%# Eval("CodProcesso") %>'></asp:Label>
                                                        &nbsp;&nbsp;
                                                        <asp:Label ID="lblTituloApl" runat="server" Text="Aplicação" Font-Bold="true"></asp:Label>
                                                        <asp:Label ID="lblApl" runat="server" Text='<%# Eval("CodAplicacao") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="lblTituloQtde" runat="server" Text="Qtde" Font-Bold="true"></asp:Label>
                                                        <asp:Label ID="lblQtde" runat="server" Text='<%# Eval("Qtde") %>'></asp:Label>
                                                        &nbsp;&nbsp;
                                                        <asp:Label ID="lblTituloAltura" runat="server" Text="Altura" Font-Bold="true"></asp:Label>
                                                        <asp:Label ID="lblAltura" runat="server" Text='<%# Eval("AlturaProducao") %>'></asp:Label>
                                                        &nbsp;&nbsp;
                                                        <asp:Label ID="lblTituloLargura" runat="server" Text="Largura" Font-Bold="true"></asp:Label>
                                                        <asp:Label ID="lblLargura" runat="server" Text='<%# Eval("LarguraProducao") %>'></asp:Label>
                                                        &nbsp;&nbsp;
                                                        <asp:Label ID="lblTituloTotM" runat="server" Text="Total m²" Font-Bold="true"></asp:Label>
                                                        <asp:Label ID="lblTotM" runat="server" Text='<%# Eval("TotM") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="lblTituloPedCli" runat="server" Text="Ped. Cli. da Peça" Font-Bold="true"></asp:Label>
                                                        <asp:Label ID="lblPedCli" runat="server" Text='<%# Eval("PedCli") %>'></asp:Label>
                                                        &nbsp;&nbsp;
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="right" style="vertical-align: text-top" nowrap="nowrap">
                                            <table style="margin-top: 0px">
                                                <tr>
                                                    <td align="right">
                                                        <asp:Table ID="tblImagens" runat="server" OnLoad="tblImagens_Load">
                                                        </asp:Table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right">
                                                        <asp:Label ID="lblTituloEtiqueta" runat="server" Text="Etiqueta(s)" Font-Bold="true"></asp:Label>
                                                        <asp:Label ID="lblEtiqueta" runat="server" Text='<%# Eval("EtiquetasLegenda") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdProdPed" runat="server" Value='<%# Eval("IdProdPed") %>' />
                                <asp:HiddenField ID="hdfImagemUrl" runat="server" Value='<%# Eval("ImagemUrl") %>' />
                                <asp:HiddenField ID="hdfIdItemProjeto" runat="server" Value='<%# Eval("IdItemProjeto") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <br />
                <asp:Button ID="btnAplicar" runat="server" OnClick="btnAplicar_Click" OnClientClick="if (!confirm(&quot;Salvar alterações?&quot;)) return false;"
                    Text="Aplicar" />
                <asp:Button ID="btnFechar" runat="server" Text="Fechar" OnClientClick="closeWindow(); return false" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPecas" runat="server" SelectMethod="GetVidros" TypeName="Glass.Data.DAL.ProdutosPedidoEspelhoDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCountVidros"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
