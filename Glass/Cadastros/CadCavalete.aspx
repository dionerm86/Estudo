<%@ Page Title="Cavaletes" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadCavalete.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCavalete" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRptEtiqueta(idCavalete) {
            if (idCavalete == null || idCavalete == "") {
                alert("Informe o cavalete.");
                return false;
            }

            openWindow(500, 700, "../Relatorios/RelBase.aspx?rel=EtqCavalete&idCavalete=" + idCavalete, null, true, true);
            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCavalete" runat="server" SkinID="gridViewEditable"
                    DataSourceID="odsCavalete" DataKeyNames="IdCavalete" AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm('Tem certeza que deseja excluir este Cavalete?');" />
                                <asp:ImageButton ID="imbImpEtiqueta" runat="server" ImageUrl="~/Images/printer.png"
                                                            ToolTip="Imprimir Etiqueta" Visible="true"
                                                            OnClientClick='<%# "return openRptEtiqueta("+ Eval("IdCavalete") + ");" %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CodInterno" SortExpression="CodInterno">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodInterno" runat="server" Columns="17" MaxLength="16"
                                    Text='<%# Bind("CodInterno") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvCodInetno" runat="server" ValidationGroup="c"
                                    ControlToValidate="txtCodInterno" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodInterno" runat="server" Columns="17" MaxLength="16"
                                    Text='<%# Bind("CodInterno") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvCodInetno" runat="server" ValidationGroup="c"
                                    ControlToValidate="txtCodInterno" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblCodInterno" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Localização" SortExpression="Localizacao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLocalizacao" runat="server" MaxLength="64" Text='<%# Bind("Localizacao") %>'
                                    Width="120px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLocalizacao" runat="server" MaxLength="64" Text='<%# Bind("Localizacao") %>'
                                    Width="120px"></asp:TextBox>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                                    ShowMessageBox="True" ShowSummary="False" ValidationGroup="c" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLocalizacao" runat="server" Text='<%# Bind("Localizacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click" ValidationGroup="c">
                                     <img border="0" src="../Images/insert.gif" alt="Inserir" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrllogpopup id="ctrlLogPopup1" runat="server" idregistro='<%# (uint)(int)Eval("IdCavalete") %>'
                                    tabela="Cavalete" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCavalete" runat="server"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarCavaletes"
                    SelectByKeysMethod="ObterCavalete"
                    SortParameterName="sortExpression"
                    DeleteMethod="ApagarCavalete"
                    DeleteStrategy="GetAndDelete"
                    TypeName="Glass.PCP.Negocios.ICavaleteFluxo"
                    DataObjectTypeName="Glass.PCP.Negocios.Entidades.Cavalete"
                    InsertMethod="SalvarCavalete"
                    UpdateMethod="SalvarCavalete"
                    UpdateStrategy="GetAndUpdate">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
