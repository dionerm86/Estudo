<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="PecasPendentes.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.Producao.PecasPendentes" Title="Peças Pendentes" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <style>
        .tbPesquisa
        {
            vertical-align: middle;
        }
        .tbPesquisa td
        {
            /*background-color: #FAFAFA;
            border: solid 1px #F0F0F0;*/
            display: table-cell;
            vertical-align: middle;
            white-space: nowrap;
            width: auto;
            margin: 0;
            padding: 0;
        }
        .tituloCampos
        {
            text-align: left;
        }
    </style>

    <script type="text/javascript">
    
    function openRpt(exportarExcel)
    {
        var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
        var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
        var usarProximoSetor = FindControl("chkUsarProximoSetor", "input").checked;
        var tipoPerido = FindControl("ddlTipoPeriodo", "select").value;
        
        openWindow(600, 800, "RelBase.aspx?rel=PecasPendentes&dataIni=" + dataIni + "&dataFim=" + dataFim + "&tipoPeriodo=" + tipoPerido + "&usarProximoSetor=" + usarProximoSetor + "&exportarExcel=" + exportarExcel);
    }
    
    </script>

    <section>
        <section id="Pesquisa">
            <div>
                <table class="tbPesquisa">
                    <tr>
                        <td class="tituloCampos">
                            <asp:DropDownList ID="ddlTipoPeriodo" runat="server" Width="170px">
                                <asp:ListItem Selected="True" Text="Período (Entrega)" Value="PeriodoEntrega" />
                                <asp:ListItem Text="Período (Fábrica)" Value="PeriodoFabrica" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:CheckBox runat="server" ID="chkUsarProximoSetor" Text="Exibir próximo setor a ser efetuado na peça" />
                        </td>
                    </tr>
                </table>
            </div>
            <br />
            <br />
            <div>
                <table>
                    <tr>
                        <td>
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false;">
                            <img alt="" src="../../Images/Printer.png" /> Imprimir</asp:LinkButton>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;">
                            <img alt="" src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </div>
        </section>
    </section>
</asp:Content>
