<%@ Page Title="Rotas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstRota.aspx.cs" Inherits="Glass.UI.Web.Listas.LstRota" %>

<%@ Register src="../Controls/ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc1" %>
<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaRota&exportarExcel=" + exportarExcel);
        }

        function openRptDet(idRota) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=DadosRota&idRota=" + idRota);
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Rota</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdRota" runat="server" SkinID="defaultGridView"
                        DataKeyNames="IdRota" DataSourceID="odsRota" >
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" NavigateUrl='<%# "../Cadastros/CadRota.aspx?idRota=" + Eval("IdRota") %>' ToolTip="Editar"><img border="0" src="../Images/EditarGrid.gif" alt="Editar" /></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja excluir esta Rota?&quot;);"
                                    ToolTip="Excluir" />
                                <asp:LinkButton ID="lnkDetalhes" runat="server" OnClientClick='<%# "return openRptDet(" + Eval("IdRota") + ");" %>'> <img alt="" border="0" 
                                     src="../Images/Relatorio.gif" /></asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodInterno" HeaderText="Cód." SortExpression="CodInterno" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <%# Colosoft.Translator.Translate(Eval("Situacao"), "fem").Format() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Distância" SortExpression="Distancia">
                            <ItemTemplate>
                                <%# (int)Eval("Distancia") + "km" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Obs" HeaderText="Obs" SortExpression="Obs" />
                        <asp:TemplateField HeaderText="Dias da Rota" SortExpression="DiasSemana">
                            <ItemTemplate>
                                <%# Colosoft.Translator.Translate(Eval("DiasSemana")).Format() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumeroMinimoDiasEntrega" HeaderText="Núm. Mín. Dias Entrega"
                            SortExpression="NumeroMinimoDiasEntrega" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc2:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                    IdRegistro='<%# (uint)(int)Eval("IdRota") %>' Tabela="Rota" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRota" runat="server" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.Rota"
                    DeleteMethod="ApagarRota" 
                    DeleteStrategy="GetAndDelete"
                    SelectMethod="PesquisarRotas" 
                    SelectByKeysMethod="ObtemRota"
                    TypeName="Glass.Global.Negocios.IRotaFluxo"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SortParameterName="sortExpression"></colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" 
                    src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"> 
                    <img border="0" src="../Images/Excel.gif" alt="Exportar para o Excel" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
