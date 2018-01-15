<%@ Page Title="Origem Troca / Devolução" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstOrigemTrocaDevolucao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstOrigemTrocaDevolucao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <section>
        <div>
            <asp:GridView ID="grdOrigem" runat="server" SkinID="gridViewEditable"
                DataKeyNames="IdOrigemTrocaDesconto" DataSourceID="odsOrigem" Width="450px">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/Images/Edit.gif"
                                ToolTip="Editar" Visible='<%# ((int)Eval("QtdeContasReceber")) == 0 %>' />
                            <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                ToolTip="Excluir" Visible='<%# ((int)Eval("QtdeContasReceber")) == 0 %>' />
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
                                Width="200px"></asp:TextBox></EditItemTemplate><FooterTemplate>
                            <asp:TextBox ID="txtDescricaoIns" runat="server" MaxLength="40" Text='<%# Bind("Descricao") %>'
                                Width="200px"></asp:TextBox></FooterTemplate><ItemTemplate>
                            <asp:Label ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                            </asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblSituacao" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <FooterTemplate>
                            <asp:LinkButton ID="lnkInserir" runat="server" OnClientClick="return onSave(true);" OnClick="lnkInserir_Click">
                                <img border="0" src="../Images/insert.gif" alt="Inserir" />
                            </asp:LinkButton>
                        </FooterTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsOrigem" runat="server" 
                DeleteMethod="ApagarOrigemTrocaDesconto" EnablePaging="True"
                DeleteStrategy="GetAndDelete"
                MaximumRowsParameterName="pageSize"
                SelectMethod="PesquisarOrigensTrocaDesconto" SortParameterName="sortExpression"
                SelectByKeysMethod="ObtemOrigemTrocaDesconto"
                TypeName="Glass.Global.Negocios.IOrigemTrocaDescontoFluxo" 
                DataObjectTypeName="Glass.Global.Negocios.Entidades.OrigemTrocaDesconto"
                UpdateMethod="SalvarOrigemTrocaDesconto"
                UpdateStrategy="GetAndUpdate">
            </colo:VirtualObjectDataSource>
        </div>
    </section>
</asp:Content>
