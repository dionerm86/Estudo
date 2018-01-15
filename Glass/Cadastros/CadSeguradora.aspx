<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadSeguradora.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadSeguradora" Title="Cadastro Seguradora" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Conteudo" Runat="Server">
<link href="<%= ResolveUrl("~") %>Style/CTe/CadCTe" rel="stylesheet" type="text/css" />
<div class="pagina">
        <div class="dtvCadSeguradora">
            <asp:DetailsView ID="dtvCadSeguradora" DataKeyNames="IdSeguradora" DataSourceID="odsSeguradora"
                SkinID="defaultDetailsView" runat="server">
                <Fields>
                    <asp:TemplateField ShowHeader="false">
                        <InsertItemTemplate>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label54" runat="server" Text="Nome"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtNomeSeguradora" runat="server" MaxLength="50" Width="300px"
                                        Text='<%# Bind("NomeSeguradora") %>'></asp:TextBox>
                                </div>
                            </div>
                            
                        </InsertItemTemplate>
                        <EditItemTemplate>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label54" runat="server" Text="Nome"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtNomeSeguradora" runat="server" MaxLength="50" Width="300px"
                                        Text='<%# Bind("NomeSeguradora") %>'></asp:TextBox>
                                </div>
                            </div>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="false">
                        <EditItemTemplate>
                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                ValidationGroup="c" />
                            <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                CausesValidation="false" />
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" 
                                ValidationGroup="c" />
                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                Text="Cancelar" />
                        </InsertItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
        </div>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSeguradora" runat="server" 
            DataObjectTypeName="Glass.Fiscal.Negocios.Entidades.Seguradora"
            SelectMethod="ObtemSeguradora" 
            TypeName="Glass.Fiscal.Negocios.ICTeFluxo"
            InsertMethod="SalvarSeguradora" UpdateMethod="SalvarSeguradora"
            UpdateStrategy="GetAndUpdate" >
            <SelectParameters>
                <asp:QueryStringParameter Name="idSeguradora" QueryStringField="IdSeguradora" Type="Int32"
                    DefaultValue="" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
    </div>
</asp:Content>

