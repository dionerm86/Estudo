<%@ Page Title="Centro de Custo" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true" CodeBehind="SelCentroCusto.aspx.cs" Inherits="Glass.UI.Web.Utils.SelCentroCusto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Menu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Pagina" runat="server">

    <script type="text/javascript">

        window.onbeforeunload = function () {
            window.opener.atualizarPagina();
            return null;
        };

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView runat="server" ID="grvDetalhesCentroCusto" DataSourceID="odsDetalhesCentroCustoAssociado" AutoGenerateRows="false"
                    GridLines="None" DataKeyNames="Identificador" DefaultMode="ReadOnly">
                    <Fields>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <table cellspacing="1" cellpadding="5" style="min-width: 300px;">
                                    <tr class="dtvAlternatingRow">
                                        <td class="dtvHeader">
                                            <asp:Label ID="Label7" runat="server" Text='<%# Eval("Descricao") %>' />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("Identificador") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            <asp:Label ID="Label8" runat="server" Text='<%# Eval("DescricaoValorAssociacao") %>' />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("ValorAssociacao", "{0:c}") %>' />
                                        </td>
                                    </tr>
                                    <tr class="dtvAlternatingRow">
                                        <td class="dtvHeader">
                                            <asp:Label ID="Label9" runat="server" Text='Valor total dos Centros de Custos' />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="Label10" runat="server" Text='<%# Eval("ValorTotal", "{0:c}") %>' />
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCentroCustoAssociado" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsCentroCustoAssociado"
                    DataKeyNames="IdCentroCustoAssociado" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    ShowFooter="True" OnRowCommand="grdCentroCustoAssociado_RowCommand">
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CausesValidation="false">
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" OnClientClick="if (!confirm('Tem certeza que deseja excluir esse centro de custo?')) return false;"
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" CausesValidation="false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onUpdateProd();" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                                <asp:HiddenField ID="hdfIdCompra" runat="server" Value='<%# Bind("IdCompra") %>' />
                                <asp:HiddenField ID="hdfIdImpostoServ" runat="server" Value='<%# Bind("IdImpostoServ") %>' />
                                <asp:HiddenField ID="hdfIdNf" runat="server" Value='<%# Bind("IdNf") %>' />
                                <asp:HiddenField ID="hdfIdCentroCusto" runat="server" Value='<%# Eval("IdCentroCusto") %>' />
                                <asp:HiddenField ID="hdfIdContaPg" runat="server" Value='<%# Bind("IdContaPg") %>' />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Centro de Custo" SortExpression="IdCentroCusto">
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlCentroCusto" runat="server" DataSourceID="odsCentroCusto"
                                    DataTextField="Descricao" DataValueField="IdCentroCusto" SelectedValue='<%# Bind("IdCentroCusto") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblCentroCusto" runat="server" Text='<%# Bind("DescricaoCentroCusto") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="ddlCentroCusto" runat="server" DataSourceID="odsCentroCusto"
                                    DataTextField="Descricao" DataValueField="IdCentroCusto" AppendDataBoundItems="true">
                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValor" runat="server" Width="50px" Text='<%# Bind("Valor") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblValor" runat="server" Text='<%# Bind("Valor") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValor" runat="server" Width="50px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="lnkInsCentroCusto" runat="server" ImageUrl="../Images/ok.gif" OnClick="lnkInsCentroCusto_Click" Style="height: 15px" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <EditRowStyle CssClass="edit"></EditRowStyle>

                    <PagerStyle CssClass="pgr"></PagerStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCentroCustoAssociado" runat="server"
                    SelectMethod="ObtemDadosCentroCusto" TypeName="Glass.Data.DAL.CentroCustoAssociadoDAO" DataObjectTypeName="Glass.Data.Model.CentroCustoAssociado"
                    InsertMethod="Insert" UpdateMethod="Update" DeleteMethod="Delete" OnDeleted="odsCentroCustoAssociado_Deleted" OnUpdated="odsCentroCustoAssociado_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idCompra" QueryStringField="idCompra" Type="Int32" />
                        <asp:QueryStringParameter Name="idImpostoServ" QueryStringField="idImpostoServ" Type="Int32" />
                        <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="Int32" />
                        <asp:QueryStringParameter Name="idContaPg" QueryStringField="idContaPg" Type="Int32" />
                        <asp:QueryStringParameter Name="idCte" QueryStringField="idCte" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsDetalhesCentroCustoAssociado" runat="server"
                    SelectMethod="ObtemDetalhesCentroCustoAssociado" TypeName="Glass.Data.DAL.CentroCustoAssociadoDAO" DataObjectTypeName="Glass.Data.Model.DetalhesCentroCustoAssociado">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idCompra" QueryStringField="idCompra" Type="Int32" />
                        <asp:QueryStringParameter Name="idImpostoServ" QueryStringField="idImpostoServ" Type="Int32" />
                        <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="Int32" />
                        <asp:QueryStringParameter Name="idContaPg" QueryStringField="idContaPg" Type="Int32" />
                        <asp:QueryStringParameter Name="idCte" QueryStringField="idCte" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCentroCusto" runat="server" SelectMethod="ObtemParaSelecao" TypeName="Glass.Data.DAL.CentroCustoDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="buscarEstoque" QueryStringField="compra" Type="Boolean" />
                        <asp:QueryStringParameter Name="idLoja" QueryStringField="idLoja" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
