<%@ Page Title="Classificação - Roteiro da Produção" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstClassificacaoRoteiroProducao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstClassificacaoRoteiroProducao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <table>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Classificação</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdClassificacaoRoteiroProducao" runat="server" SkinID="gridViewEditable"
                    DataSourceID="odsClassificacaoRoteiroProducao" DataKeyNames="IdClassificacaoRoteiroProducao" EmptyDataText="Nenhuma Classificação encontrada.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" NavigateUrl='<%# "../Cadastros/CadClassificacaoRoteiroProducao.aspx?idClassificacao=" + Eval("IdClassificacaoRoteiroProducao") %>' ToolTip="Editar">
                                    <img alt="" border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm('Tem certeza que deseja excluir essa Classificação?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdClassificacaoRoteiroProducao" HeaderText="Cód." SortExpression="IdClassificacaoRoteiroProducao" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="CapacidadeDiaria" HeaderText="Capacidade Diária" SortExpression="CapacidadeDiaria" ><ItemStyle HorizontalAlign="Center" /></asp:BoundField>
                        <asp:BoundField DataField="MetaDiaria" HeaderText="Meta Diária" SortExpression="MetaDiaria" />
                    </Columns>
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsClassificacaoRoteiroProducao" runat="server"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarClassificacao"
                    SelectByKeysMethod="ObtemClassificacao"
                    SortParameterName="sortExpression"
                    TypeName="Glass.PCP.Negocios.IClassificacaoRoteiroProducaoFluxo"
                    DataObjectTypeName="Glass.PCP.Negocios.Entidades.ClassificacaoRoteiroProducao"
                    DeleteMethod="ApagarClassificacao"
                    DeleteStrategy="GetAndDelete">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
