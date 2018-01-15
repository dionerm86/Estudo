<%@ Page Title="Cores de Ferragem" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstCorFerragem.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCorFerragem" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function onSave(insert) {
            var descricao = FindControl(insert ? "txtDescricaoIns" : "txtDescricao", "input").value;
            var sigla = FindControl(insert ? "txtSiglaIns" : "txtSigla", "input").value;

            if (descricao == "") {
                alert("Informe a descrição.");
                return false;
            }

            if (sigla == "") {
                alert("Informe a sigla.");
                return false;
            }
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdCorFerragem" runat="server" SkinID="gridViewEditable"
                    DataSourceID="odsCorFerragem" DataKeyNames="IdCorFerragem" EnableViewState="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif" alt="Editar"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onSave(false);" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Cor" SortExpression="IdCorFerragem">
                            <EditItemTemplate>
                                <asp:Label ID="lblIdCorFerragem" runat="server" Text='<%# Bind("IdCorFerragem") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblIdCorFerragem" runat="server" Text='<%# Bind("IdCorFerragem") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="30" Text='<%# Bind("Descricao") %>'
                                    Width="150px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricaoIns" runat="server" MaxLength="30" Text='<%# Bind("Descricao") %>'
                                    Width="150px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sigla" SortExpression="Sigla">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSigla" runat="server" MaxLength="5" Text='<%# Bind("Sigla") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtSiglaIns" runat="server" MaxLength="5" Text='<%# Bind("Sigla") %>'
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Sigla") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClientClick="return onSave(true);"
                                    OnClick="lnkInserir_Click"><img border="0" src="../Images/insert.gif" alt="Inserir" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource  culture="pt-BR" ID="odsCorFerragem" runat="server" 
                    DeleteMethod="ApagarCorFerragem" 
                    DeleteStrategy="GetAndDelete"
                    EnablePaging="True"
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarCoresFerragem" 
                    SelectByKeysMethod="ObtemCorFerragem"
                    SortParameterName="sortExpression"
                    TypeName="Glass.Global.Negocios.ICoresFluxo" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.CorFerragem"
                    UpdateMethod="SalvarCorFerragem"
                    UpdateStrategy="GetAndUpdate"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>


