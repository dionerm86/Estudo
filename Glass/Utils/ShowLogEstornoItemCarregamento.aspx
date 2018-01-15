<%@ Page Title="Estornos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ShowLogEstornoItemCarregamento.aspx.cs" Inherits="Glass.UI.Web.Utils.ShowLogEstornoItemCarregamento" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdLog" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdEstorno" DataSourceID="odsLog" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Nenhum estorno registrado para este item.">
                    <Columns>
                        <asp:BoundField DataField="CodInternoDescrPeca" HeaderText="Item" />
                        <asp:BoundField DataField="NomeFuncionario" HeaderText="Funcionário" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="Motivo" HeaderText="Motivo" SortExpression="Motivo" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLog" runat="server" 
                      SelectMethod="GetListEstornoItem"
                      TypeName="WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idItemCarregamento" QueryStringField="idItemCarregamento" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
