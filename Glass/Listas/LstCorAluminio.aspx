<%@ Page Title="Cores de Alumínio" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstCorAluminio.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCorAluminio" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    
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
            
                <asp:GridView ID="grdCorAluminio" runat="server" SkinID="gridViewEditable"
                    DataKeyNames="IdCorAluminio" DataSourceID="odsCorAluminio" EnableViewState="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif" alt="Editar" /></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" 
                                    Height="16px" ImageUrl="~/Images/ok.gif" OnClientClick="return onSave(false);" 
                                    ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Cor" SortExpression="IdCorAluminio">
                            <ItemTemplate>
                                <asp:Label ID="lblIdCorAluminio" runat="server" 
                                    Text='<%# Bind("IdCorAluminio") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblIdCorAluminio" runat="server"></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricaoIns" runat="server" 
                                    Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sigla" SortExpression="Sigla">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Sigla") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSigla" runat="server" MaxLength="5" 
                                    Text='<%# Bind("Sigla") %>' Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtSiglaIns" runat="server" MaxLength="5" 
                                    Text='<%# Bind("Sigla") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click" 
                                    OnClientClick="return onSave(true);"><img border="0" src="../Images/insert.gif" alt="Inserir" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            
            </td>
        </tr>
        <tr>
            <td align="center">
            
                <colo:VirtualObjectDataSource  culture="pt-BR" ID="odsCorAluminio" runat="server" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.CorAluminio" 
                    DeleteMethod="ApagarCorAluminio" 
                    DeleteStrategy="GetAndDelete"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" 
                    SelectMethod="PesquisarCoresAluminio" 
                    SelectByKeysMethod="ObtemCorAluminio"
                    SortParameterName="sortExpression"
                    TypeName="Glass.Global.Negocios.ICoresFluxo" 
                    UpdateMethod="SalvarCorAluminio"
                    UpdateStrategy="GetAndUpdate" >
                </colo:VirtualObjectDataSource>
            
            </td>
        </tr>
    </table>
</asp:Content>

