<%@ Page Title="Gráfico de Orçamentos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="GraficoOrcamentos.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.GraficoOrcamentos" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/FusionCharts.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>

    <script type="text/javascript">

        function Graph() {
            var myChart = new FusionCharts("../../Charts/MSLine.swf", "myChartId", "800", "400");

            var idLoja = FindControl("drpLoja", "select").value;
            var idVend = FindControl("drpVendedor", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var agrupar = FindControl("drpAgrupar", "select").value;

            myChart.setDataURL("../../Handlers/Chart.ashx?query=1;" + idLoja + ";" + idVend + ";" + situacao + ";" + dtIni + ";" + dtFim + ";" + agrupar);
            myChart.render("divChart");
        }

        function openRpt() {
            var altura = 600;
            var largura = 800;
            var scrY = (screen.height - altura) / 2;
            var scrX = (screen.width - largura) / 2;
            var momentoAtual = new Date();

            var idLoja = FindControl("drpLoja", "select").value;
            var idVend = FindControl("drpVendedor", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var agrupar = FindControl("drpAgrupar", "select").value;
            var tempFile = FindControl("hdfTempFile", "input").value;

            openWindow(600, 800, "RelBase.aspx?rel=GraficoOrcamentos&idLoja=" + idLoja + "&dtIni=" + dtIni + "&dtFim=" + dtFim + "&idVend=" + idVend + "&agrupar=" + agrupar + "&tempFile=" + tempFile);

            return false;
        }

        var data;

        function openRpt() {

            var idLoja = FindControl("drpLoja", "select").value;
            var idVend = FindControl("drpVendedor", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var agrupar = FindControl("drpAgrupar", "select").value;
            var tempFile = FindControl("hdfTempFile", "input").value;
            
            var rel = openWindow(600, 800, "RelBase.aspx?postData=getPostData()");
            data = new Object();
            data["rel"] = "GraficoOrcamentos";
            data["dtIni"] = dtIni;
            data["dtFim"] = dtFim;
            data["idLoja"] = idLoja;
            data["idVend"] = idVend;
            data["agrupar"] = agrupar;
            data["tempFile"] = tempFile;
        }

        function getPostData() {
            return data;
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True" AutoPostBack="True">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedor" runat="server" DataSourceID="odsVendedor" DataTextField="Nome"
                                DataValueField="IdFunc" AppendDataBoundItems="true" AutoPostBack="True">
                                <asp:ListItem Value="0">TODOS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" DataSourceID="odsSituacaoOrca"
                                DataTextField="Descr" DataValueField="Id" AutoPostBack="True" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
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
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Agrupar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAgrupar" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="1">Loja</asp:ListItem>
                                <asp:ListItem Value="2">Vendedor</asp:ListItem>
                                <asp:ListItem Value="3">Situação</asp:ListItem>
                                <asp:ListItem Value="0">Nenhum</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%--        <tr>
            <td align="center">
                <div id="divChart" align="center" style="z-index: -1;">
                </div>
            </td>
        </tr>--%>
        <tr>
            <td align="center">
                <div id="msChart" align="center" style="z-index: -1;">
                    <asp:Chart ID="Chart1" runat="server">
                    </asp:Chart>
                </div>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false"
                    CausesValidation="false">
                <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
                <%--<a href="#" onclick="openRpt(); return false"><img alt="" border="0" src="../../Images/printer.png" /> Imprimir</a>    --%>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVendedor" runat="server" SelectMethod="GetVendedoresOrca"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:Parameter Name="idOrcamento" Type="UInt32" DefaultValue="0" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSituacaoOrca" runat="server" SelectMethod="GetSituacaoOrcamento"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfTempFile" runat="server" />

    <script>
        //Graph();
    </script>

</asp:Content>
