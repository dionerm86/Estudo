<%@ Page Title="Recebimentos por Tipo de Pagamento" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="RecebimentosTipo.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.RecebimentosTipo" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

function openRpt()
{
    var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
    var dataFim = FindControl("ctrlDataFim_txtData", "input").value;

    openWindow(600, 800, "RelBase.aspx?rel=RecebimentosTipo&dataIni=" + dataIni + "&dataFim=" + dataFim);
    
    return false;
}

function openRptDetalhado() {
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
                    CssClass="gridStyle" DataSourceID="odsRecebimentoTipo" GridLines="None" OnRowDataBound="grdRecebimentoTipo_RowDataBound"
                    ShowHeader="False">
                    <Columns>
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="Valor" DataFormatString="{0:c}" HeaderText="Valor" SortExpression="Valor" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRecebimentoTipo" runat="server" SelectMethod="GetRecebimentosTipo"
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
                <asp:Label ID="Label5" runat="server" Text="* Apenas informativo, não é somado ao total"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;">
                    <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
            &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkImprimir0" runat="server" OnClientClick="openRptDetalhado(); return false;">
                    <img alt="" border="0" src="../../Images/printer.png" /> Imprimir Detalhado</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
