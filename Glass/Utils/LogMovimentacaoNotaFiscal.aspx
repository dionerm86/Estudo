<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogMovimentacaoNotaFiscal.aspx.cs"
    Inherits="Glass.UI.Web.Utils.LogMovimentacaoNotaFiscal" Title="Log de Movimentações da Nota Fiscal" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu"></asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
     
    <table>
        <tr>
            <td align="center">
                Restrições:
                <table id="tbNf" runat="server">
                    <tr>
                        <td align="center">
                            <asp:GridView ID="grdLogNota" DataSourceID="odsLogMovimentacaoNotaFiscal"
                                GridLines="None"  runat="server" AllowPaging="True"
                                AllowSorting="True" AutoGenerateColumns="False" 
                                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" EmptyDataText="Nenhuma restrição encontrada.">
                                <Columns>
                                    <asp:BoundField DataField="NumeroNfe" HeaderText="NFE" />
                                    <asp:BoundField DataField="DescricaoProd" HeaderText="Descrição Prod." />
                                    <asp:BoundField DataField="MensagemLog" HeaderText="Mensagem" />
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLogMovimentacaoNotaFiscal" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.LogMovimentacaoNotaFiscal"
                    TypeName="Glass.Data.DAL.LogMovimentacaoNotaFiscalDAO"
                    SelectMethod="ObtemLogsNotaFiscal">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idNf" QueryStringField="idNf"  />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
            <td align="center">
                Movimentações:
                <table id="Table1" runat="server">
                    <tr>
                        <td align="center">
                            <asp:GridView ID="GridView1" DataSourceID="odsMovEstoquesLog"
                                GridLines="None"  runat="server" AllowPaging="True"
                                AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdProdNf"  
                                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" EmptyDataText="Nenhuma movimentação encontrada.">
                                <Columns>
                                    <asp:BoundField DataField="CodProd" HeaderText="Cod." ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="DescricaoProd" HeaderText="Descrição Prod." ItemStyle-HorizontalAlign="Center" />
                                    
                                    <asp:BoundField DataField="MovEstoqueFiscal" HeaderText="Cód. Mov. Fiscal" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="TipoMovFiscal" HeaderText="Tipo Mov. Fiscal" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="QtdeMovFiscal" HeaderText="Qtde. Mov. Fiscal" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="DataMovFiscal" HeaderText="Data Mov. Fiscal" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:d}" />
                                    
                                    <asp:BoundField DataField="MovEstoqueReal" HeaderText="Cód. Mov. Real" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="TipoMovReal" HeaderText="Tipo Mov. Real" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="QtdeMovReal" HeaderText="Qtde. Mov. Real" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="DataMovReal" HeaderText="Data Mov. Real" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:d}" />
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMovEstoquesLog" runat="server" 
                    DataObjectTypeName="Glass.Data.RelModel.MovEstoquesLog"
                    TypeName="Glass.Data.RelDAL.MovEstoquesLogDAO"
                    SelectMethod="ObtemMovEstoquesLogNota">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idNf" QueryStringField="idNf"  />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
