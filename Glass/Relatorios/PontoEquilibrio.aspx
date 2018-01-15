<%@ Page Title="Ponto de Equilíbrio" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="PontoEquilibrio.aspx.cs" Inherits="Glass.UI.Web.Relatorios.PontoEquilibrio" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var dtIni = FindControl("txtDataIni", "input").value;
            var dtFim = FindControl("txtDataFim", "input").value;

            openWindow(600, 800, "RelBase.aspx?rel=PontoEquilibrio&dataIni=" + dtIni + "&dataFim=" + dtFim + "&exportarExcel=" + exportarExcel);

            return false;
        }

        function openDados() {

            openWindow(600, 800, "ListaNaoConsideradosPE.aspx?popup=true");

            return false;
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="txtDataIni" runat="server" ReadOnly="ReadWrite" ValidationGroup="c" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="txtDataFim" runat="server" ReadOnly="ReadWrite" ValidationGroup="c" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" ValidationGroup="c" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <div id="pontoEquilibrio" runat="server">
                </div>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openDados(); return false;">Dados não considerados pelo Ponto de Equilíbrio</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false;">
                <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"
                    Visible="False"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
