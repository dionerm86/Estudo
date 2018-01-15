<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetFornecedorVinculos.aspx.cs"
    Title="Vincular fornecedores" Inherits="Glass.UI.Web.Utils.SetFornecedorVinculos" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <style type="text/css">
        .style1 {
            width: 50%;
        }
    </style>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td class="subtitle1">Fornecedores Disponíveis
                        </td>
                    </tr>
                    <tr>
                        <td align="center">&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center" nowrap="nowrap">
                            <table>
                                <tr>
                                    <td align="right" nowrap="nowrap">
                                        <asp:Label ID="Label3" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox ID="txtNum" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                            onblur="getFornec(this);"></asp:TextBox>
                                        <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                        <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClientClick="getFornec(FindControl('txtNum', 'input'));" OnClick="imgPesq_Click"
                                            Style="width: 16px" />
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label5" runat="server" Text="CPF/CNPJ" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox ID="txtCnpj" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                        <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView ID="grdFornecedor" runat="server" DataKeyNames="IdFornec" DataSourceID="odsFornecedor"
                                EmptyDataText="Não há Fornecedores Cadastrados" SkinID="defaultGridView" OnRowCommand="grdCli_RowCommand"
                                AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkCriarVinc" runat="server" CommandName="CriarVinculo" CommandArgument='<%# Eval("IdFornec") %>'>
                                                <img src="../Images/insert.gif" border="0" title="Vincular" /></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdFornec" HeaderText="Cód." SortExpression="IdFornec" />
                                    <asp:BoundField DataField="NomeFantasia" HeaderText="Nome" SortExpression="NomeFantasia" />

                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>

                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFornecedor" runat="server"
                                DataObjectTypeName="Glass.Global.Negocios.Entidades.Fornecedor"
                                SelectMethod="PesquisarFornecedores"
                                SelectByKeysMethod="ObtemFornecedor"
                                TypeName="Glass.Global.Negocios.IFornecedorFluxo"
                                EnablePaging="True" MaximumRowsParameterName="pageSize"
                                SortParameterName="sortExpression">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtNum" Name="idFornec" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtNome" Name="nomeFornec" PropertyName="Text" />
                                    <asp:Parameter Name="situacao"  />
                                    <asp:ControlParameter ControlID="txtCnpj" Name="cnpj" PropertyName="Text" />
                                    <asp:Parameter Name="comCredito" />
                                    <asp:Parameter Name="idPlanoConta" />
                                    <asp:Parameter Name="idTipoPagto" />
                                    <asp:Parameter Name="endereco"  />
                                    <asp:Parameter Name="vendedor" />
                                    <asp:Parameter Name="tipoPessoa" DefaultValue="" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
            <td valign="top">
                <table>
                    <tr>
                        <td class="subtitle1">Fornecedores Vinculados
                        </td>
                    </tr>
                    <tr>
                        <td align="center">&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdVinculados" runat="server" AutoGenerateColumns="False"
                                DataSourceID="odsVinculados" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" DataKeyNames="IdFornec" EmptyDataText="Nenhum fornecedor vinculado."
                                OnRowCommand="grdVinculados_RowCommand">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkRemVinc" runat="server" CommandName="RemoverVinculo" CommandArgument='<%# Eval("IdFornec") %>'>
                                            <img src="../Images/removergrid.gif" border="0" title="Vincular" /></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdNome" HeaderText="Nome" SortExpression="Nome" />
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVinculados" runat="server" MaximumRowsParameterName=""
                                SelectMethod="GetVinculados" StartRowIndexParameterName="" TypeName="Glass.Data.DAL.FornecedorDAO">
                                <SelectParameters>
                                    <asp:QueryStringParameter Name="idFornec" QueryStringField="idFornec" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
