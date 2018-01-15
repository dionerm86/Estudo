<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LstFuncDepartamento.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstFuncDepartamento" MasterPageFile="~/Layout.master" Title="Funcionários por Departamento" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setApl(idAplicacao, codInterno, descr)
        {
            window.opener.setApl(idAplicacao, codInterno);
            closeWindow();
        }

    </script>

    <table>
        <tr>
            <td class="subtitle1">
                <colo:ItemDetailsView runat="server" DataSourceID="odsDepartamento">
                    <ItemTemplate>
                        <%# Eval("Nome") %>
                    </ItemTemplate>
                </colo:ItemDetailsView>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Adicionar funcionário ao departamento:
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFunc" runat="server" DataSourceID="odsFunc" EnableViewState="false" 
                                DataTextField="Name" DataValueField="Id">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClick="imgAdd_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdFuncDepartamento" runat="server" SkinID="defaultGridView"
                    DataSourceID="odsFuncDepartamento"
                    DataKeyNames="Id" EnableViewState="false"
                    EmptyDataText="Não há funcionários para esse departamento.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete2" CommandArgument='<%# Eval("Id") %>' ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir esse funcionário desse setor?&quot;)) return false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Funcionário" SortExpression="Name">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncDepartamento" runat="server" 
                    MaximumRowsParameterName="pageSize" EnableViewState="false"
                    SelectMethod="ObtemFuncionariosPorDepartamento" 
                    TypeName="Glass.Global.Negocios.IFuncionarioFluxo"
                    DeleteMethod="ApagarFuncionarioDepartamento">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idDepartamento" QueryStringField="idDepartamento"
                            Type="Int32" />
                    </SelectParameters>
                    <DeleteParameters>
                         <asp:QueryStringParameter Name="idDepartamento" QueryStringField="idDepartamento"
                            Type="Int32" />
                    </DeleteParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFunc" runat="server" 
                    SelectMethod="ObtemFuncionariosForaDepartamento" EnableViewState="false"
                    TypeName="Glass.Global.Negocios.IFuncionarioFluxo">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idDepartamento" QueryStringField="idDepartamento"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>

                <colo:VirtualObjectDataSource runat="server" ID="odsDepartamento"
                    SelectMethod="ObtemDepartamento" EnableViewState="false"
                    TypeName="Glass.Global.Negocios.IFuncionarioFluxo">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idDepartamento" QueryStringField="idDepartamento"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
