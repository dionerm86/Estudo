<%@ Page Title="Gráfico de Recebimentos por Tipo" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="GraficoRecebimentosTipo.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.GraficoRecebimentosTipo" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src="https://www.google.com/jsapi"></script>

    <script type="text/javascript">

    function openRptTeste() {
        var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
        var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
        var idLoja = FindControl("drpLoja", "select").value;
        var idFunc = FindControl("drpFunc", "select").value;
        var tempFile = FindControl("hdfTempFile", "input").value;

        openWindow(600, 800, "RelBase.aspx?rel=RecebimentosTipoGrafico&idLoja=" + idLoja +
                "&idFunc=" + idFunc + "&dataIni=" + dtIni +
                "&dataFim=" + dtFim + "&tempFile=" + tempFile);
    }

    var data;

    function openRpt() {

        var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
        var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
        var idLoja = FindControl("drpLoja", "select").value;
        var idFunc = FindControl("drpFunc", "select").value;
        var tempFile = FindControl("hdfTempFile", "input").value;
        var rel = openWindow(600, 800, "RelBase.aspx?postData=getPostData()");
        data = new Object();
        data["rel"] = "RecebimentosTipoGrafico";
        data["dataIni"] = dataIni;
        data["dataFim"] = dataFim;
        data["idLoja"] = idLoja;
        data["idFunc"] = idFunc;
        data["tempFile"] = tempFile;
    }

    function getPostData() {
        return data;
    }
    </script>

    <table>
        <tr>
            <td align="Center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Loja: "></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True" AutoPostBack="True">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Funcionário: "></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFunc" runat="server" DataSourceID="odsFuncionario" DataTextField="Nome"
                                DataValueField="IdFunc" AppendDataBoundItems="True" AutoPostBack="True">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Período: "></asp:Label>
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
            <td align="Center">
            </td>
        </tr>
        <tr>
            <td align="Center">
                <asp:CheckBox ID="chkLegenda" runat="server" AutoPostBack="true" Text="Visualizar Legenda"
                    ForeColor="#0066FF" OnCheckedChanged="chkLegenda_CheckedChanged" ToolTip="Selecionando essa opção, o gráfico será exibido com uma legenda" />
            </td>
        </tr>
        <td align="Center">
            <br /><br />
            <asp:Label ID="lblTotal" runat="server"></asp:Label>
            <div id="divChart" align="center" style="z-index: -1;">
                <asp:Chart ID="Chart1" runat="server">
                    <Series>
                        <asp:Series Name="Series1">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
            </div>
        </td>
        </tr>
        <tr>
            <td align="Center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false">
                <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetCaixaDiario"
        TypeName="Glass.Data.DAL.FuncionarioDAO">
    </colo:VirtualObjectDataSource>
    <asp:HiddenField ID="hdfTempFile" runat="server" />
</asp:Content>
