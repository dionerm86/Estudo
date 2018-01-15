<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelMedidor.aspx.cs" Inherits="Glass.UI.Web.Utils.SelMedidor"
    Title="Selecione o Medidor" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <br />
                <asp:GridView GridLines="None" ID="grdFunc" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsFunc" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdFunc"
                    EmptyDataText="Nenhum medidor encontrado.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="window.opener.setMedidor('<%# Eval("IdFunc") %>', '<%# Eval("Nome") %>'); closeWindow();">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="DescrEndereco" HeaderText="Endereço" SortExpression="DescrEndereco" />
                        <asp:BoundField DataField="Cpf" HeaderText="CPF" SortExpression="Cpf" />
                        <asp:BoundField DataField="Rg" HeaderText="RG" SortExpression="Rg" />
                        <asp:BoundField DataField="TelRes" HeaderText="Tel Res" SortExpression="TelRes">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TelCel" HeaderText="Cel" SortExpression="TelCel">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFunc" runat="server" DataObjectTypeName="Glass.Data.Model.Funcionario"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetMedidoresCount" SelectMethod="GetMedidores" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
