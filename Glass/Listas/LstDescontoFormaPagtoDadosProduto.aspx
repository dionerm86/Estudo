<%@ Page Title="Desconto por Forma Pagto. e Dados Produto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstDescontoFormaPagtoDadosProduto.aspx.cs" Inherits="Glass.UI.Web.Listas.LstDescontoFormaPagtoDadosProduto" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <table>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" PostBackUrl="~/Cadastros/CadDescontoFormaPagtoDadosProduto.aspx">Inserir Desconto</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdDescontoFormaPagtoDadosProduto" runat="server" SkinID="defaultGridView"
                    DataSourceID="odsDescontoFormaPagtoDadosProduto" DataKeyNames="IdDescontoFormaPagamentoDadosProduto"
                    OnRowCommand="grdDescontoFormaPagtoDadosProduto_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                    PostBackUrl='<%# "~/Cadastros/CadDescontoFormaPagtoDadosProduto.aspx?idDescontoFormaPagamentoDadosProduto=" + Eval("IdDescontoFormaPagamentoDadosProduto") %>' />
                                <asp:ImageButton ID="imgAtivarInativar" runat="server" ImageUrl="~/Images/Inativar.gif"
                                    CommandName="AtivarInativar" CommandArgument='<%# Eval("IdDescontoFormaPagamentoDadosProduto") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Venda" SortExpression="DescrTipoVenda">
                            <ItemTemplate>
                                <asp:Label ID="lblTipoVenda" runat="server" Text='<%# Eval("DescrTipoVenda") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Forma Pagamento" SortExpression="DescFormaPagto">
                            <ItemTemplate>
                                <asp:Label ID="lblFormaPagto" runat="server" Text='<%# Eval("DescFormaPagto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Cartão" SortExpression="DescTipoCartao">
                            <ItemTemplate>
                                <asp:Label ID="lblTipoCartao" runat="server" Text='<%# Eval("DescTipoCartao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Parcela" SortExpression="DescricaoParcelas">
                            <ItemTemplate>
                                <asp:Label ID="lblParcela" runat="server" Text='<%# Eval("DescricaoParcelas") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grupo do Produto" SortExpression="DescGrupoProd">
                            <ItemTemplate>
                                <asp:Label ID="lblGrupoProd" runat="server" Text='<%# Eval("DescGrupoProd") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Subgrupo do Produto" SortExpression="DescSubgrupoProd">
                            <ItemTemplate>
                                <asp:Label ID="lblSubgrupoProd" runat="server" Text='<%# Eval("DescSubgrupoProd") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Desconto (%)" SortExpression="Desconto">
                            <ItemTemplate>
                                <asp:Label ID="lblDesconto" runat="server" Text='<%# Eval("Desconto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("Situacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false">
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="DescontoFormaPagamentoDadosProduto" IdRegistro='<%# Eval("IdDescontoFormaPagamentoDadosProduto") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource ID="odsDescontoFormaPagtoDadosProduto" Culture="pt-BR" runat="server"
                    TypeName="Glass.Data.DAL.DescontoFormaPagamentoDadosProdutoDAO"
                    DataObjectTypeName="Glass.Financeiro.Negocios.Entidades.DescontoFormaPagamentoDadosProduto"
                    EnablePaging="true" MaximumRowsParameterName="pageSize" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    SelectMethod="PesquisarDescontoFormaPagamentoDadosProduto" SelectCountMethod="CountDescontoFormaPagamentoDadosProduto">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblAviso" runat="server" Text="Os descontos são buscados de acordo com os campos preenchidos seguindo a sequência 'Tipo Venda', 'Forma Pagamento', 'Tipo Cartão', 'Parcela', 'Grupo' e 'Subgrupo'."></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
