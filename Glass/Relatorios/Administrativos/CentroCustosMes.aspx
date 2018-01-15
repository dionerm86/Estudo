<%@ Page Title="Centro de Custos por Mês" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CentroCustosMes.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.CentroCustosMes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {

            var ano = FindControl("txtAno", "input").value;

            if (ano == null || ano == "") {
                alert("Informe o ano para geração do relatório.");
                return false;
            }

            var idLoja = FindControl("drpLoja", "select").value;

            openWindow(600, 800, "RelBase.aspx?rel=CentroCustosMes&ano=" + ano + "&idLoja=" + idLoja + "&exportarExcel=" + exportarExcel);

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
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Ano: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAno" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
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
