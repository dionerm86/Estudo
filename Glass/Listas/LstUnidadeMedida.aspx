<%@ Page Title="Unidades de Medidas" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstUnidadeMedida.aspx.cs" Inherits="Glass.UI.Web.Listas.LstUnidadeMedida" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function onSave(insert) {
            var descricao = FindControl(insert ? "txtDescricaoIns" : "txtDescricaoEdit", "input").value;
            var codigo = FindControl(insert ? "txtCodigoIns" : "txtCodigoEdit", "input").value;

            if (descricao == "") {
                alert("Informe a descrição.");
                return false;
            }

            if (codigo == "") {
                alert("Informe o código.");
                return false;
            }
        }

    </script>

    <section>
        <section id="pesquisa">
            <div class="boxLinha">
                <asp:Label ID="Label1" runat="server" Text="Código" ForeColor="#0066FF"></asp:Label>
                <asp:TextBox ID="txtCodigo" runat="server" Width="60px"></asp:TextBox>
                <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    OnClick="imgPesq_Click" />
                <asp:Label ID="Label4" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                <asp:TextBox ID="txtDescricao" runat="server" Width="170px"></asp:TextBox>
                <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    OnClick="imgPesq_Click" />
            </div>
        </section>
        <br />
        <br />
        <section id="produtos">
            <asp:GridView ID="grdUnidadeMedida" runat="server" SkinID="gridViewEditable" 
                DataSourceID="odsUnidadeMedida" DataKeyNames="IdUnidadeMedida">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                            <img border="0" src="../Images/Edit.gif" alt="Editar" /></asp:LinkButton>
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
                    <asp:TemplateField HeaderText="Código" SortExpression="Codigo">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCodigoEdit" runat="server" MaxLength="10" Text='<%# Bind("Codigo") %>'
                                Width="50px"></asp:TextBox>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtCodigoIns" runat="server" MaxLength="10" Text='<%# Bind("Codigo") %>'
                                Width="50px"></asp:TextBox>
                        </FooterTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("Codigo") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDescricaoEdit" runat="server" MaxLength="30" Text='<%# Bind("Descricao") %>'
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
                    <asp:TemplateField>
                        <FooterTemplate>
                            <asp:LinkButton ID="lnkInserir" runat="server" OnClientClick="return onSave(true);"
                                OnClick="lnkInserir_Click"><img border="0" src="../Images/insert.gif" alt="Inserir" /></asp:LinkButton>
                        </FooterTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle />
                <EditRowStyle />
                <AlternatingRowStyle />
            </asp:GridView>
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsUnidadeMedida" runat="server" 
                DeleteMethod="ApagarUnidadeMedida"
                DeleteStrategy="GetAndDelete"
                EnablePaging="True" MaximumRowsParameterName="pageSize" 
                SelectMethod="PesquisarUnidadesMedida" SortParameterName="sortExpression"
                SelectByKeysMethod="ObtemUnidadeMedida"
                TypeName="Glass.Global.Negocios.IUnidadesFluxo"
                DataObjectTypeName="Glass.Global.Negocios.Entidades.UnidadeMedida"
                UpdateMethod="SalvarUnidadeMedida"
                UpdateStrategy="GetAndUpdate">
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtCodigo" Name="codigo" PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="txtDescricao" Name="descricao" PropertyName="Text"
                        Type="String" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
        </section>
    </section>
</asp:Content>
