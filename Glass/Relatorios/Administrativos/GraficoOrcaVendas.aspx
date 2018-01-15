<%@ Page Title="Gráfico de Orçamentos e Vendas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="GraficoOrcaVendas.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.GraficoOrcaVendas" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    function openRptTeste() {
        var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
        var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
        var idLoja = FindControl("drpLoja", "select").value;
        var idVendedor = FindControl("drpVendedor", "select").value;
        var situacao = FindControl("drpSituacao", "select").itens();
        var tipoFunc = FindControl("drpTipoFunc", "select").value;
        var tempFile = FindControl("hdfTempFile", "input").value;
        openWindow(600, 800, "RelBase.aspx?rel=GraficoOrcaVendas&idLoja=" + idLoja 
                                + "&idVendedor=" + idVendedor + "&situacao=" 
                                + situacao + "&tipoFunc=" + tipoFunc
                                + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&tempFile=" + tempFile);
    }

    var data;

    function openRpt() {

        var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
        var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
        var idLoja = FindControl("drpLoja", "select").value;
        var idVendedor = FindControl("drpVendedor", "select").value;
        var situacao = FindControl("drpSituacao", "select").itens();
        var tipoFunc = FindControl("drpTipoFunc", "select").value;
        var tempFile = FindControl("hdfTempFile", "input").value;
        var rel = openWindow(600, 800, "RelBase.aspx?postData=getPostData()");
        data = new Object();
        data["rel"] = "GraficoOrcaVendas";
        data["dataIni"] = dataIni;
        data["dataFim"] = dataFim;
        data["idLoja"] = idLoja;
        data["idVendedor"] = idVendedor;
        data["tempFile"] = tempFile;
        data["situacao"] = situacao;
        data["tipoFunc"] = tipoFunc;
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
                            <asp:Label ID="Label1" runat="server" Text="Tipo Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoFunc" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Emissor</asp:ListItem>
                                <asp:ListItem Value="1">Vendedor</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedor" runat="server" DataSourceID="odsVendedor" AppendDataBoundItems="true"
                                DataTextField="Name" DataValueField="Id" AutoPostBack="True">
                                <asp:ListItem Value="0">TODOS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown runat="server" ID="drpSituacao" DataSourceID="odsSituacaoOrca"
                                DataTextField="Descr" DataValueField="Id" AutoPostBack="True">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Data Incial" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Data Final" ForeColor="#0066FF" Style="text-align: right"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                            &nbsp;
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
                <div id="Div1">
                    <asp:Chart ID="Chart1" runat="server" Width="800px">
                        <Legends>
                            <asp:Legend Name="Legenda">
                            </asp:Legend>
                        </Legends>
                        <Series>
                            <asp:Series Name="Orçamentos" Legend="Legenda" LegendText="#SERIESNAME" ChartArea="ChartArea1"
                                LabelToolTip="#VALX #VAL">
                            </asp:Series>
                            <asp:Series ChartArea="ChartArea1" Legend="Legenda" Name="Vendas" LegendText="#SERIESNAME">
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
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;">
                <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource  culture="pt-BR"  ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVendedor" runat="server"
                    SelectMethod="ObtemFuncionariosAtivosAssociadosAClientes" TypeName="Glass.Global.Negocios.IFuncionarioFluxo">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource  culture="pt-BR"  ID="odsSituacaoOrca" runat="server" SelectMethod="GetSituacaoOrcamento"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfTempFile" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
