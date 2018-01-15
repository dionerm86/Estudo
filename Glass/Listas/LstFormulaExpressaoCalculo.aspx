<%@ Page Title="Fórmulas de Expressão de Cálculo" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstFormulaExpressaoCalculo.aspx.cs" Inherits="Glass.UI.Web.Listas.LstFormulaExpressaoCalculo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <script type="text/javascript">

        var txtCalcSel = null;

        function setExpressao(expressao) {
            txtCalcSel.value = expressao;
        }

        function openExpressao(nomeControle) {
            txtCalcSel = FindControl(nomeControle, "input");

            var idFormulaExpreCalc = FindControl("hdfIdFormulaExpreCalc", "input").value;
            var expressao = trocaSinalMais(txtCalcSel.value);

            var url = "../Utils/SelExpressao.aspx?tipo=posicao";

            url += idFormulaExpreCalc != null ? "&idFormulaExp=" + idFormulaExpreCalc : "";
            url += expressao != null ? "&expr=" + expressao : "";

            openWindow(500, 700, url);

            return false;
        }

        // Troca os sinais de + das expressões de cálculo para que ao editar a mesma o + não suma
        function trocaSinalMais(descricao) {
            while (descricao.indexOf("+") > 0)
                descricao = descricao.replace("+", "@");

            return descricao;
        }

        function validaDescricao() {
            var descricao = FindControl("txtDescricao", "input").value;
            if (descricao == null || descricao == "") {
                alert("Informe a descrição da fórmula.");
                return false;
            }
            return true;
        }

    </script>
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdFormulaExpressaoCalculo" runat="server" SkinID="gridViewEditable" EnableViewState="false"
                    DataSourceID="odsFormulaExpressaoCalculo" DataKeyNames="IdFormulaExpreCalc" AutoGenerateColumns="false" GridLines="None"
                    OnDataBound="grdFormulaExpressaoCalculo_DataBound">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" CommandName="Edit" ImageUrl="../Images/Edit.gif" ToolTip="Editar"/>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="if (!validaDescricao()) return false;" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                                <asp:HiddenField ID="hdfIdFormulaExpreCalc" runat="server" Value='<%# Eval("IdFormulaExpreCalc") %>' />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="lblDescricao" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Expressão" SortExpression="Expressao">
                            <ItemTemplate>
                                <asp:Label ID="lblExpressao" runat="server" Text='<%# Eval("Expressao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCalc1" runat="server" Text='<%# Bind("Expressao") %>' onpaste="return false;" onkeydown="return false;" onkeyup="return false;" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc1');">
                                            <img src="../Images/Pesquisar.gif" border="0" /></a>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imbInserir" runat="server" Height="16px" ToolTip="Atualizar"
                                    ImageUrl="~/Images/Insert.gif" OnClick="imbInserir_Click" OnClientClick="if (!validaDescricao()) return false;" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource ID="odsFormulaExpressaoCalculo" runat="server" Culture="pt-BR" EnableViewState="false" EnablePaging="true"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" MaximumRowsParameterName="pageSize"
                    TypeName="Glass.Data.DAL.FormulaExpressaoCalculoDAO" DataObjectTypeName="Glass.Data.Model.FormulaExpressaoCalculo"
                    SelectMethod="GetList" SelectCountMethod="GetCount" DeleteMethod="Delete" UpdateMethod="Update" InsertMethod="Insert"
                    OnDeleted="odsFormulaExpressaoCalculo_Deleted">

                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
