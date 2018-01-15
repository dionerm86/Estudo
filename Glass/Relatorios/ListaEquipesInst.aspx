<%@ Page Title="Equipes de Instalação" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ListaEquipesInst.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaEquipesInst" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt() {
            var tipo = FindControl("drpTipo", "select").value;

            openWindow(600, 800, "RelBase.aspx?rel=EquipesInst&tipo=" + tipo);

            return false;
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Tipo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Comum</asp:ListItem>
                                <asp:ListItem Value="2">Temperado</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdEquipes" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsEquipe"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdEquipe" 
                    EmptyDataText="Nenhuma Equipe encontrada.">
                    <Columns>
                        <asp:BoundField DataField="IdEquipe" HeaderText="Cód." SortExpression="IdEquipe" />
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="DescrTipo" HeaderText="Tipo" SortExpression="DescrTipo" />
                        <asp:BoundField DataField="DescrVeiculo" HeaderText="Veículo" SortExpression="DescrVeiculo" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt();">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEquipe" runat="server" DataObjectTypeName="Glass.Data.Model.Equipe"
                    DeleteMethod="Delete" SelectMethod="GetListRpt" TypeName="Glass.Data.DAL.EquipeDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCountRpt"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpTipo" Name="tipo" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
