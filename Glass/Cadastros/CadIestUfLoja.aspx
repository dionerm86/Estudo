<%@ Page Title="Cadastro de IEST UF por Loja" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true"
    CodeBehind="CadIestUfLoja.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadIestUfLoja" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="menu" ContentPlaceHolderID="Menu" runat="server">
</asp:Content>
<asp:Content ID="pagina" ContentPlaceHolderID="Pagina" runat="server">

    <asp:GridView ID="grdIestUfLoja" runat="server" GridLines="None" SkinID="gridViewEditable"
        DataSourceID="odsIestUfLoja" DataKeyNames="IdIestUfLoja"  EnableViewState="false">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="imbEditar" runat="server" ImageUrl="~/Images/edit.gif" CommandName="Edit" />
                    <asp:ImageButton ID="imbCancelar" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" CommandName="Delete" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:HiddenField ID="hdfIdIestUfLoja" runat="server" Value='<%# Eval("IdIestUfLoja") %>' />
                    <asp:HiddenField ID="hdfIdLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                    <asp:ImageButton ID="imbAtualizar" runat="server" ImageUrl="~/Images/ok.gif" CommandName="Update" />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="UF">
                <ItemTemplate>
                    <asp:Label ID="lblNomeUf" runat="server" Text='<%# Eval("NomeUf") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:Label ID="lblNomeUf" runat="server" Text='<%# Eval("NomeUf") %>'></asp:Label>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="drpNomeUf" runat="server" DataSourceID="odsUf"
                        DataTextField="name" DataValueField="id">
                    </asp:DropDownList>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Inscrição Estadual ST">
                <ItemTemplate>
                    <asp:Label ID="lblInscEstSt" runat="server" Text='<%# Eval("InscEstSt") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtInscEstSt" runat="server" Text='<%# Bind("InscEstSt") %>'></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtInscEstSt" runat="server" ></asp:TextBox>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <uc1:ctrlLogPopup ID="ctrlLogPopup" runat="server" Tabela="IestUfLoja" IdRegistro='<%# (int)Eval("IdIestUfLoja") %>' />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:ImageButton ID="imbInserir" runat="server" ImageUrl="~/Images/Insert.gif" OnClick="imbInserir_Click" />
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <colo:VirtualObjectDataSource ID="odsIestUfLoja" Culture="pt-BR" runat="server"
        EnablePaging="true" MaximumRowsParameterName="pageSize" SortParameterName="sortExpression" 
        DataObjectTypeName="Glass.Fiscal.Negocios.Entidades.IestUfLoja"
        TypeName="Glass.Fiscal.Negocios.IIestUfLojaFluxo"
        SelectMethod="PesquisarIest" SelectByKeysMethod="ObtemIestUfLoja"
        UpdateMethod="SalvarIestUfLoja" UpdateStrategy="GetAndUpdate"
        DeleteMethod="ApagarIestUfLoja" DeleteStrategy="GetAndDelete">
        <SelectParameters>
            <asp:QueryStringParameter QueryStringField="idLoja" Name="idLoja" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsUf" runat="server" SelectMethod="ObtemUfs"
        TypeName="Glass.Global.Negocios.ILocalizacaoFluxo">
    </colo:VirtualObjectDataSource>

</asp:Content>
