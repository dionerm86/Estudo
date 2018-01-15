<%@ Page Title="Vincular Planos de Conta" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true" CodeBehind="CadVincularPlanoConta.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadVincularPlanoConta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Menu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Pagina" runat="server">

    <script type="text/javascript">

        function openPlanoConta() {

            var planoConta = FindControl("txtPlanoConta", "input");
            var hdfPlanoConta = FindControl("hdfPlanoConta", "input");

            if (planoConta == null || hdfPlanoConta == null)
                return false;

            planoConta.value = "";
            hdfPlanoConta.value = "";

            openWindow(500, 700, '../Utils/SelPlanoConta.aspx');
            return false;
        }

        function setPlanoConta(idConta, descricao) {

            var planoConta = FindControl("txtPlanoConta", "input");
            var hdfPlanoConta = FindControl("hdfPlanoConta", "input");

            if (planoConta == null || hdfPlanoConta == null)
                return false;

            planoConta.value = descricao.split('-')[descricao.split('-').length - 1].trim();
            hdfPlanoConta.value = idConta;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Plano de Contas: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPlanoConta" runat="server" ReadOnly="true">
                            </asp:TextBox>
                            <asp:HiddenField ID="hdfPlanoConta" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="return openPlanoConta();" ToolTip="Pesquisar Plano de Contas" /></td>
                        <td>
                            <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClick="imgAdd_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView SkinID="gridViewEditable" ID="grdPlanoConta" runat="server"
                    DataKeyNames="IdConta" DataSourceID="odsPlanoConta" EnableViewState="false" AutoGenerateColumns="false" OnRowCommand="grdPlanoConta_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server"
                                    CommandName="Desvincular" CommandArgument='<%# Eval("IdConta") %>' ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja desvincular este Plano de Conta?&quot;);"
                                    ToolTip="Desvincular" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num. Conta" SortExpression="IdContaGrupo">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdContaGrupo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grupo" SortExpression="Grupo">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Grupo.Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPlanoConta" runat="server"
        DataObjectTypeName="Glass.Financeiro.Negocios.Entidades.PlanoContas"
        EnablePaging="True"
        MaximumRowsParameterName="pageSize"
        SelectMethod="ObterPlanosContasVincularPlanoContas"
        SortParameterName="sortExpression"
        TypeName="Glass.Financeiro.Negocios.IPlanoContasFluxo">
        <SelectParameters>
            <asp:QueryStringParameter QueryStringField="idContaContabil" Name="idContaContabil" Type="Int32" />
        </SelectParameters>

    </colo:VirtualObjectDataSource>
</asp:Content>
