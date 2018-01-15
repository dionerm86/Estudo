<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstAssociarPropVeic.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAssociarPropVeic" Title="Associar Proprietário/Veículo" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Conteudo" runat="Server">
    <div>
        <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Associar Proprietário/Veículo</asp:LinkButton>
    </div>
    <div>
        <asp:GridView GridLines="None" ID="grdAssociarProprietarioVeiculo" 
            runat="server" AllowPaging="True"
            AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsAssociarProprietarioVeiculo"
            CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
            EditRowStyle-CssClass="edit" DataKeyNames="IdPropVeic" 
            EmptyDataText="Não há associação cadastrada" 
            onrowdatabound="grdAssociarProprietarioVeiculo_RowDataBound">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadAssociarPropVeic.aspx?idPropVeiculo=" + Eval("IdPropVeic") + "&placa=" + Eval("Placa")%>'>
                                    <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                        <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                            OnClientClick="return confirm(&quot;Tem certeza que deseja desfazer associação?&quot;);"
                            ToolTip="Excluir" />
                    </ItemTemplate>
                    <HeaderStyle Wrap="False" />
                    <ItemStyle Wrap="False" />
                </asp:TemplateField>
                <asp:BoundField  HeaderText="Nome" SortExpression="Nome" />
                <asp:BoundField DataField="Placa" HeaderText="Placa" SortExpression="Placa" />                
            </Columns>
            <PagerStyle />
            <EditRowStyle />
            <AlternatingRowStyle />
        </asp:GridView>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAssociarProprietarioVeiculo" runat="server" DataObjectTypeName="Glass.Data.Model.CTe.ProprietarioVeiculo_Veiculo"
            DeleteMethod="Delete" SelectMethod="GetList" TypeName="Glass.Data.DAL.CTe.ProprietarioVeiculo_VeiculoDAO"
            EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" 
            SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
            OnDeleted="odsAssociarProprietarioVeiculo_Deleted" 
            onselected="odsAssociarProprietarioVeiculo_Selected">
        </colo:VirtualObjectDataSource>
    </div>
</asp:Content>

