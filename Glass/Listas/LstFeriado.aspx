<%@ Page Title="Lista de Feriados" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstFeriado.aspx.cs" Inherits="Glass.UI.Web.Listas.LstFeriado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function onSave(insert) {
            var descricao = FindControl(insert ? "txtDescricaoIns" : "txtDescricao", "input").value;
            var dia = FindControl(insert ? "txtDiaIns" : "txtDia", "input").value;
            var mes = FindControl(insert ? "txtMesIns" : "txtMes", "input").value;

            if (descricao == "") {
                alert("Informe a descrição do feriado.");
                return false;
            }

            if (dia == "") {
                alert("Informe o dia.");
                return false;
            }
            else {
                if (parseInt(dia) < 1 || parseInt(dia) > 31)
                {
                    alert("Dia inválido.");
                    return false;
                }
            }

            if (mes == "") {
                alert("Informe o Mês.");
                return false;
            }
            else {
                if (parseInt(mes) < 1 || parseInt(mes) > 12)
                {
                    alert("Mês inválido.");
                    return false;
                }
            }
        }

    </script>

    <section>
        <div>
            <asp:GridView ID="grdFeriado" runat="server" SkinID="gridViewEditable"
                          DataKeyNames="IdFeriado" DataSourceID="odsFeriado" 
                          Width="378px">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
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
                    <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDescricao" runat="server" MaxLength="40" Text='<%# Bind("Descricao") %>'
                                Width="200px"></asp:TextBox>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtDescricaoIns" runat="server" MaxLength="40" Text='<%# Bind("Descricao") %>'
                                Width="200px"></asp:TextBox>
                        </FooterTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Dia" SortExpression="Dia">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDia" runat="server" MaxLength="30" Text='<%# Bind("Dia") %>'
                                Width="40px"></asp:TextBox>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtDiaIns" runat="server" MaxLength="30" Text='<%# Bind("Dia") %>'
                                Width="40px"></asp:TextBox>
                        </FooterTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("Dia") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Mês" SortExpression="Mes">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtMes" runat="server" MaxLength="30" Text='<%# Bind("Mes") %>'
                                Width="40px"></asp:TextBox>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtMesIns" runat="server" MaxLength="30" Text='<%# Bind("Mes") %>'
                                Width="40px"></asp:TextBox>
                        </FooterTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("Mes") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <FooterTemplate>
                            <asp:LinkButton ID="lnkInserir" runat="server" OnClientClick="return onSave(true);"
                                OnClick="lnkInserir_Click"><img border="0" src="../Images/insert.gif" /></asp:LinkButton>
                        </FooterTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFeriado" runat="server" EnablePaging="True"
                DeleteMethod="ApagarFeriado"  MaximumRowsParameterName="pageSize"
                SelectMethod="PesquisarFeriados" 
                SelectByKeysMethod="ObtemFeriado"
                UpdateStrategy="GetAndUpdate" DeleteStrategy="GetAndDelete"
                SortParameterName="sortExpression"
                TypeName="Glass.Global.Negocios.IDataFluxo" DataObjectTypeName="Glass.Global.Negocios.Entidades.Feriado"
                UpdateMethod="SalvarFeriado" InsertMethod="SalvarFeriado">
            </colo:VirtualObjectDataSource>
        </div>
    </section>
</asp:Content>
