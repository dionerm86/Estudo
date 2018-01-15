<%@ Page Title="Administradoras de Cartão" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstAdminCartao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAdminCartao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:HyperLink ID="lnkInserir" runat="server" 
                    NavigateUrl="~/Cadastros/CadAdminCartao.aspx">Inserir Administradora de Cartão</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="grdAdminCartao" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                    DataKeyNames="IdAdminCartao" DataSourceID="odsAdminCartao" GridLines="None" 
                    onrowcommand="grdAdminCartao_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" 
                                    ImageUrl="~/Images/EditarGrid.gif" 
                                    onclientclick='<%# "redirectUrl(\"../Cadastros/CadAdminCartao.aspx?idAdminCartao=" + Eval("IdAdminCartao") + "\"); return false" %>' />
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" 
                                    onclientclick="if (!confirm(&quot;Deseja excluir essa administradora de cartão?&quot;)) return false" />
                                <asp:ImageButton ID="imgPagto" runat="server" 
                                    ImageUrl="~/Images/money_hist.gif" 
                                    onclientclick='<%# "redirectUrl(\"LstPagtoAdminCartao.aspx?idAdminCartao=" + Eval("IdAdminCartao") + "\"); return false" %>' 
                                    ToolTip="Pagamentos" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdAdminCartao" HeaderText="Cód." 
                            SortExpression="IdAdminCartao" />
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="Cnpj" HeaderText="CNPJ" SortExpression="Cnpj" />
                        <asp:BoundField DataField="InscrEst" HeaderText="Inscr. Estadual" 
                            SortExpression="InscrEst" />
                        <asp:BoundField DataField="DescrEndereco" HeaderText="Endereço" 
                            SortExpression="DescrEndereco" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAdminCartao" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.AdministradoraCartao" 
                    DeleteMethod="Delete" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" 
                    SelectMethod="GetList" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.AdministradoraCartaoDAO"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

