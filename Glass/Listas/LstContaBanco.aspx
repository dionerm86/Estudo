<%@ Page Title="Contas Bancárias" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstContaBanco.aspx.cs" Inherits="Glass.UI.Web.Listas.LstContaBanco" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Conta Bancária</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdContaBancaria" runat="server" SkinID="defaultGridView"
                    DataSourceID="odsContaBancaria" DataKeyNames="IdContaBanco" EmptyDataText="Não há Contas cadastradas.">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadContaBanco.aspx?IdContaBanco=" + Eval("IdContaBanco") %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" alt="Editar" /></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir esta Conta Bancária?&quot;);"
                                    Visible='<%# (int)Eval("QtdeMovimentacoes") == 0 %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Loja" HeaderText="Loja" SortExpression="Loja" />
                        <asp:BoundField DataField="Nome" HeaderText="Banco" SortExpression="Nome" />
                        <asp:BoundField DataField="CodBanco" HeaderText="Cód. Banco" 
                            SortExpression="CodBanco" />
                        <asp:BoundField DataField="Agencia" HeaderText="Agência" SortExpression="Agencia" />
                        <asp:BoundField DataField="Conta" HeaderText="Conta" SortExpression="Conta" />
                        <asp:BoundField DataField="Titular" HeaderText="Titular" SortExpression="Titular" />
                        <asp:BoundField DataField="CodConvenio" HeaderText="Cód. Convênio" 
                            SortExpression="CodConvenio" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="ContaBanco" IdRegistro='<%# (uint)(int)Eval("IdContaBanco") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBancaria" runat="server" 
                    DataObjectTypeName="Glass.Financeiro.Negocios.Entidades.ContaBanco"
                    DeleteMethod="ApagarContaBanco" 
                    DeleteStrategy="GetAndDelete"
                    SelectMethod="PesquisarContasBanco" 
                    SelectByKeysMethod="ObtemContaBanco"
                    TypeName="Glass.Financeiro.Negocios.IContaBancariaFluxo"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SortParameterName="sortExpression">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
