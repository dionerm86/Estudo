<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstSeguradora.aspx.cs" Inherits="Glass.UI.Web.Listas.LstSeguradora" Title="Seguradoras" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Conteudo" runat="Server">
    <div>
        <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Seguradora</asp:LinkButton>
    </div>
    <div>
        <asp:GridView GridLines="None" ID="grdSeguradora" runat="server" 
            DataSourceID="odsSeguradora" SkinID="defaultGridView" EnableViewState="false"
            DataKeyNames="IdSeguradora" EmptyDataText="Não há seguradora cadastrada">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadSeguradora.aspx?idSeguradora=" + Eval("IdSeguradora") %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" alt="Editar" /></asp:HyperLink>
                        <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                            OnClientClick="return confirm(&quot;Tem certeza que deseja excluir esta seguradora?&quot;);"
                            ToolTip="Excluir" />
                    </ItemTemplate>
                    <HeaderStyle Wrap="False" />
                    <ItemStyle Wrap="False" />
                </asp:TemplateField>
                <asp:BoundField DataField="NomeSeguradora" HeaderText="Nome" SortExpression="NomeSeguradora" />
            </Columns>
        </asp:GridView>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSeguradora" runat="server" 
            DataObjectTypeName="Glass.Fiscal.Negocios.Entidades.Seguradora"
            DeleteMethod="ApagarSeguradora" 
            DeleteStrategy="GetAndDelete"
            SelectMethod="PesquisarSeguradoras" 
            SelectByKeysMethod="ObtemSeguradora"
            TypeName="Glass.Fiscal.Negocios.ICTeFluxo"
            EnablePaging="True" MaximumRowsParameterName="pageSize"
            SortParameterName="sortExpression">
        </colo:VirtualObjectDataSource>
    </div>
</asp:Content>

