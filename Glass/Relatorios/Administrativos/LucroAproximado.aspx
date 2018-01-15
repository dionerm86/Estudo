<%@ Page Title="Lucro Aproximado" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LucroAproximado.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.LucroAproximado" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

function openRpt()
{
    var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
    var dataFim = FindControl("ctrlDataFim_txtData", "input").value;

    openWindow(600, 800, "RelBase.aspx?rel=LucroAproximado&dataIni=" + dataIni + "&dataFim=" + dataFim);
    
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
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" Visible="false"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
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
                <asp:GridView ID="grdLucro" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataSourceID="odsLucroAproximado" GridLines="None" ShowHeader="False" OnRowDataBound="grdLucro_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="Valor" DataFormatString="{0:c}" HeaderText="Valor" SortExpression="Valor" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLucroAproximado" runat="server" SelectMethod="GetList"
                    TypeName="Glass.Data.RelDAL.LucroAproximadoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
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
