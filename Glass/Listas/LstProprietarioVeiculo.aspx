<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstProprietarioVeiculo.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstProprietarioVeiculo" Title="Proprietários Veículos" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Conteudo" runat="Server">
    <div>
        <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Proprietário</asp:LinkButton>
    </div>
    <div>
        <asp:GridView GridLines="None" ID="grdProprietarioVeiculo" runat="server" AllowPaging="True"
            AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProprietarioVeiculo"
            CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
            EditRowStyle-CssClass="edit" DataKeyNames="IdPropVeic" EmptyDataText="Não há proprietário cadastrado">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadProprietarioVeiculo.aspx?idPropVeiculo=" + Eval("IdPropVeic") %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                        <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                            OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este proprietário?&quot;);"
                            ToolTip="Excluir" />
                    </ItemTemplate>
                    <HeaderStyle Wrap="False" />
                    <ItemStyle Wrap="False" />
                </asp:TemplateField>
                <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                <asp:BoundField DataField="RNTRC" HeaderText="RNTRC" SortExpression="RNTRC" />
                <asp:BoundField DataField="IE" HeaderText="Insc. Estadual" SortExpression="IE" />
                <asp:BoundField DataField="UF" HeaderText="UF" SortExpression="UF" />
                <asp:BoundField DataField="TipoProp" HeaderText="TipoProp" SortExpression="TipoProp" />
            </Columns>
            <PagerStyle />
            <EditRowStyle />
            <AlternatingRowStyle />
        </asp:GridView>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProprietarioVeiculo" runat="server" DataObjectTypeName="Glass.Data.Model.CTe.ProprietarioVeiculo"
            DeleteMethod="Delete" SelectMethod="GetList" TypeName="Glass.Data.DAL.CTe.ProprietarioVeiculoDAO"
            EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
            SortParameterName="sortExpression" StartRowIndexParameterName="startRow" OnDeleted="odsProprietarioVeiculo_Deleted">
        </colo:VirtualObjectDataSource>
    </div>
</asp:Content>
