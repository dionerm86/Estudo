<%@ Page Title="Relatórios dinâmicos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstRelatorioDinamico.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Dinamicos.LstRelatorioDinamico" %>

<%@ Register Src="../../Controls/ctrlLogPopup.ascx" TagName="ctrllogpopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <table>
    <tr>
        <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir relatório dinâmico</asp:LinkButton>
            </td>
    </tr>
    <tr>
        <td align="center">
            &nbsp;</td>
    </tr>
    <tr>
        <td align="center">
            <asp:GridView GridLines="None" ID="grdRelatorioDinamico" runat="server" SkinID="defaultGridView"
                DataSourceID="odsRelatorioDinamico" DataKeyNames="IdRelatorioDinamico" EmptyDataText="Não há relatórios cadastrados.">
                <PagerSettings PageButtonCount="15" />
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "CadRelatorioDinamico.aspx?IdRelatorioDinamico=" + Eval("IdRelatorioDinamico") %>'>
                                <img border="0" src="../../Images/EditarGrid.gif" alt="Editar" /></asp:HyperLink>
                            <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                ToolTip="Excluir" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este relatório dinâmico?&quot;);" />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="IdRelatorioDinamico" HeaderText="Cód." SortExpression="IdRelatorioDinamico" />
                    <asp:BoundField DataField="NomeRelatorio" HeaderText="Nome relatório" SortExpression="NomeRelatorio" />
                    <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                        <ItemTemplate>
                            <%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRelatorioDinamico" runat="server"
                DataObjectTypeName="Glass.Global.Negocios.Entidades.RelatorioDinamico"
                DeleteMethod="ApagarRelatorioDinamico"
                DeleteStrategy="GetAndDelete"
                SelectMethod="PesquisarRelatoriosDinamico"
                SelectByKeysMethod="ObterRelatorioDinamico"
                TypeName="Glass.Global.Negocios.IRelatorioDinamicoFluxo"
                EnablePaging="True" MaximumRowsParameterName="pageSize"
                SortParameterName="sortExpression">
            </colo:VirtualObjectDataSource>
        </td>
    </tr>
</table>
</asp:Content>
