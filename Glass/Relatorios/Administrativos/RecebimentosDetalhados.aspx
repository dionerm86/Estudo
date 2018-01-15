<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecebimentosDetalhados.aspx.cs" MasterPageFile="~/Painel.master"
    Title="Recebimentos Detalhados" Inherits="Glass.UI.Web.Relatorios.Administrativos.RecebimentosDetalhados" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

function openRpt()
{
    var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
    var dataFim = FindControl("ctrlDataFim_txtData", "input").value;

    openWindow(600, 800, "RelBase.aspx?rel=RecebimentosDetalhados&dataIni=" + dataIni + "&dataFim=" + dataFim);
    
    return false;
}

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Período"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
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
                <asp:GridView ID="grdRecebimentoTipo" runat="server" AutoGenerateColumns="False"
                    CssClass="gridStyle" DataSourceID="odsRecebimentoTipo" GridLines="None" 
                    OnRowDataBound="grdRecebimentoTipo_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                        <asp:BoundField DataField="NomeCLiente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="DescricaoPlanoConta" HeaderText="Plano Conta" SortExpression="DescricaoPlanoConta" />
                        <asp:BoundField DataField="DataMovimentacao" DataFormatString="{0:g}" HeaderText="Data Mov." SortExpression="DataMovimentacao" />                        
                        <asp:BoundField DataField="Valor" DataFormatString="{0:c}" HeaderText="Valor" SortExpression="Valor" />                                                
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRecebimentoTipo" runat="server" SelectMethod="GetRecebimentosDetalhados"
                    TypeName="Glass.Data.RelDAL.RecebimentoDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:Parameter Name="idLoja" Type="UInt32" />
                        <asp:Parameter Name="usuCad" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;">
                    <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
