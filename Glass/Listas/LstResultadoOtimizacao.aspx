<%@ Page Title="Resultado da Otimização" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstResultadoOtimizacao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstResultadoOtimizacao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <asp:DetailsView runat="server" ID="dtvResultadoOtmizacao" AutoGenerateRows="false"
        GridLines="None" DefaultMode="ReadOnly" DataKeyNames="IdOtimizacao" DataSourceID="odsResultadoOtimizacao"
        EmptyDataText="Nenhuma otimização foi encontrada">
        <Fields>
            <asp:TemplateField>
                <ItemTemplate>
                    <table>
                        <tr>
                            <td class="dtvHeader">
                                Funcionário: 
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblNomeFunc" Text='<%# Eval("NomeFunc") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="dtvAlternatingRow">
                            <td class="dtvHeader">
                                Data de Otimização: 
                            </td>
                            <td>
                                <asp:Label ID="lblDtOtimiz" runat="server" Text='<%# Eval("DataCadastro") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td class="dtvHeader">
                                Total KG Bruto: 
                            </td>
                            <td>
                                <asp:Label runat="server" ID="Label1" Text='<%# Eval("TotalKgBruto") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="dtvAlternatingRow">
                            <td class="dtvHeader">
                                Total KG Líquido: 
                            </td>
                            <td>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("TotalKgLiquido") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td class="dtvHeader">
                                Total KG Perda: 
                            </td>
                            <td>
                                <asp:Label runat="server" ID="Label3" Text='<%# Eval("TotalKgPerda") %>'></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <hr />

                    <asp:ListView runat="server" ID="lstLayouts">
                        <LayoutTemplate>

                            <asp:GridView runat="server" ID="grdPecas">
                                <Columns>
                                    <asp:TemplateField>

                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                        </LayoutTemplate>
                    </asp:ListView>

                </ItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>

    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsResultadoOtimizacao" runat="server"
        DataObjectTypeName="Glass.PCP.Negocios.Entidades.OtimizacaoPesquisaListaResultadoOtimizacao"
        SelectMethod="ObterResultadoOtimizacao"
        TypeName="Glass.PCP.Negocios.IOtimizacaoFluxo">
        <SelectParameters>
            <asp:QueryStringParameter Name="idOtimizacao" QueryStringField="idOtimizacao" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>

</asp:Content>
