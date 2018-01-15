<%@ Page Title="Usuários atualmente logados no sistema" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="UsuariosLogados.aspx.cs" Inherits="Glass.UI.Web.Utils.UsuariosLogados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdUsuariosLogados" runat="server"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AutoGenerateColumns="False" 
                    DataSourceID="odsUsuariosLogados">
                    <Columns>
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="DescrTipoFunc" HeaderText="Tipo Funcionário" ReadOnly="True"
                            SortExpression="DescrTipoFunc" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" ReadOnly="True" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="UltimaAtividade" HeaderText="Última Atividade" SortExpression="UltimaAtividade" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsUsuariosLogados" runat="server" SelectMethod="GetLogados"
                    TypeName="Glass.Data.DAL.FuncionarioDAO"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
