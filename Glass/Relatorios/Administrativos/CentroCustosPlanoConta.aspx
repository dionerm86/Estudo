<%@ Page Title="Centro de Custos por Planos de Conta" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CentroCustosPlanoConta.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.CentroCustosPlanoConta" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {

            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;

            var idLoja = FindControl("drpLoja", "select").value;

            openWindow(600, 800, "RelBase.aspx?rel=CentroCustosPlanoContas&dataIni=" + dataIni + "&dataFim=" + dataFim + "&idLoja=" + idLoja + "&exportarExcel=" + exportarExcel);

            return false;
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Loja: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja"
                                DataTextField="NomeFantasia" DataValueField="IdLoja">
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
                                TypeName="Glass.Data.DAL.LojaDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Data Inicial: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadOnly" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Data Final: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadOnly" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"
                    CausesValidation="False"> <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>

</asp:Content>
