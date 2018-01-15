<%@ Page Title="Movimentações Internas de Estoque Fiscal" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstMovInternaEstoqueFiscal.aspx.cs" Inherits="Glass.UI.Web.Listas.LstMovInternaEstoqueFiscal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <table>
        <tr>
            <td align="center"></td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Nova Movimentação</asp:LinkButton></td>
        </tr>
        <tr>
            <td align="center">

                <asp:GridView ID="grdMovimentacao" runat="server" SkinID="defaultGridView"
                    DataSourceID="odsMovimentacao" DataKeyNames="IdMovInternaEstoqueFiscal" EmptyDataText="Não há movimentações cadastradas."
                    AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField HeaderText="Cód" DataField="IdMovInternaEstoqueFiscal" />
                        <asp:BoundField HeaderText="Prod. Origem" DataField="ProdutoOrigem.CodInterno" />
                        <asp:BoundField HeaderText="Qtde. Origem" DataField="QtdeOrigem" />
                        <asp:BoundField HeaderText="Prod. Destino" DataField="ProdutoDestino.CodInterno" />
                        <asp:BoundField HeaderText="Qtde. Destino" DataField="QtdeDestino" />
                        <asp:BoundField HeaderText="Qtde. Destino" DataField="Loja.FindName" />
                        <asp:BoundField HeaderText="Funcionário" DataField="Funcionario.FindName" />
                        <asp:BoundField HeaderText="Data" DataField="DataCadastro" />
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsMovimentacao" runat="server"
                    DataObjectTypeName="Glass.Estoque.Negocios.Entidades.MovInternaEstoqueFiscal"
                    SelectMethod="PesquisarMovimentacoes"
                    SelectByKeysMethod="ObtemMovimentacaoInterna"
                    TypeName="Glass.Estoque.Negocios.IMovInternaEstoqueFiscalFluxo"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SortParameterName="sortExpression">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
