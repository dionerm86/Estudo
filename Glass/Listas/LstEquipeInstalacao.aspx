<%@ Page Title="Equipes" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstEquipeInstalacao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEquipeInstalacao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" onclick="lnkInserir_Click">Inserir Equipe</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdEquipes" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" 
                    DataSourceID="odsEquipe" CssClass="gridStyle" PagerStyle-CssClass="pgr" 
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    DataKeyNames="IdEquipe" EmptyDataText="Nenhuma Equipe encontrada." 
                    onrowdatabound="grdEquipes_RowDataBound">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar"
                                    NavigateUrl='<%# "../Cadastros/CadEquipeInstalacao.aspx?idEquipe=" + Eval("IdEquipe") %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                                
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" 
                                    onclientclick="return confirm(&quot;Tem certeza que deseja excluir esta Equipe?&quot;);" 
                                    ToolTip="Excluir" />
                                    
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdEquipe" HeaderText="Cód." 
                            SortExpression="IdEquipe" />
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="DescrTipo" HeaderText="Tipo" 
                            SortExpression="DescrTipo" />
                        <asp:BoundField DataField="DescrVeiculo" HeaderText="Veículo" 
                            SortExpression="DescrVeiculo" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" 
                            SortExpression="DescrSituacao" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEquipe" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.Equipe" DeleteMethod="Delete" 
                    SelectMethod="GetList" TypeName="Glass.Data.DAL.EquipeDAO" 
                    ondeleted="odsEquipe_Deleted" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" 
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                </colo:VirtualObjectDataSource>
                </td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
        </tr>
    </table>
</asp:Content>

