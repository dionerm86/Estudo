<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstLoja.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstLoja" Title="Lojas" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div class="inserir">
        <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Loja</asp:LinkButton>
    </div>
    <asp:GridView GridLines="None" ID="grdLoja" runat="server" SkinID="defaultGridView"
        DataSourceID="odsLoja" DataKeyNames="IdLoja" EmptyDataText="Não há Lojas cadastradas" onrowcommand="grdLoja_RowCommand">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadLoja.aspx?idLoja=" + Eval("IdLoja") %>'>
                        <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                    <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                        OnClientClick="return confirm(&quot;Tem certeza que deseja excluir esta Loja?&quot;);"
                        ToolTip="Excluir" />
                    <asp:LinkButton ID="lnkInativar" runat="server" 
                        CommandArgument='<%# Eval("IdLoja") %>' CommandName="Inativar" 
                        ToolTip="Ativar/Inativar">
                            <img border="0" src="../Images/Inativar.gif"></img></asp:LinkButton>
                    <a href="#" onclick="openWindow(300, 600, '../Utils/SetCertificado.aspx?IdLoja=<%# Eval("IdLoja") %>'); return false;">
                        <img src="../Images/Cert.gif" border="0" alt="Certificado Digital" title="Certificado Digital"></a>
                </ItemTemplate>
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="RazaoSocial" HeaderText="Razão Social" SortExpression="RazaoSocial" />
            <asp:BoundField DataField="Cnpj" HeaderText="CNPJ" SortExpression="Cnpj" />
            <asp:BoundField DataField="DescrEndereco" HeaderText="Endereço" SortExpression="Endereco" />
            <asp:BoundField DataField="Telefone" HeaderText="Telefone" SortExpression="Telefone" />
            <asp:BoundField DataField="InscEst" HeaderText="Insc. Est." SortExpression="InscEst" />
            <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblSituacao" Text='<%# ExibeSituacao(Container.DataItem) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                        IdRegistro='<%# (uint)(int)Eval("IdLoja") %>' Tabela="Loja" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerStyle />
        <EditRowStyle />
        <AlternatingRowStyle />
    </asp:GridView>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" 
        DataObjectTypeName="Glass.Global.Negocios.Entidades.Loja"
        DeleteMethod="ApagarLoja" 
        DeleteStrategy="GetAndDelete"
        SelectMethod="PesquisarLojas" 
        SelectByKeysMethod="ObtemLoja"
        TypeName="Glass.Global.Negocios.ILojaFluxo"
        EnablePaging="True" MaximumRowsParameterName="pageSize"
        SortParameterName="sortExpression"></colo:VirtualObjectDataSource>
</asp:Content>
