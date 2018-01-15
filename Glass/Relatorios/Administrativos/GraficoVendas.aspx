<%@ Page Title="Gráfico de Vendas (Curva ABC)" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="GraficoVendas.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.GraficoVendas" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
       
        function getCli(idCli)
        {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');
            
            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }
            
            FindControl("txtNome", "input").value = retorno[1];
        }
        
        function openRptTeste() {
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idVendedor = FindControl("drpVendedor", "select").value; 
            var idCliente = FindControl("txtNumCli", "input").value; 
            var nomeCliente = FindControl("txtNome", "input").value; 
            var tipoFunc = FindControl("drpTipoFunc", "select").value;
            var tipoPedido = FindControl("cblTipoPedido", "select").itens();
            var idRota = FindControl("drpRota", "select").value;
            var agrupar = FindControl("drpAgrupar", "select").value;
            var series = FindControl("hdfSeries", "input").value;
            var periodos = FindControl("hdfPeriodos", "input").value;
            var tempFile = FindControl("hdfTempFile", "input").value;
            
            openWindow(600, 800, "RelBase.aspx?rel=GraficoVendas&idLoja=" + idLoja +
                "&idVendedor=" + idVendedor + "&idCliente=" + idCliente + 
                "&nomeCliente=" + nomeCliente + "&agrupar=" + agrupar + "&tipoPedido=" + tipoPedido +
                "&idRota=" + idRota + "&tipoFunc=" + tipoFunc + "&dataIni=" + dataIni +
                "&dataFim=" + dataFim + "&series=" + series + "&periodos=" + periodos + "&tempFile=" + tempFile);
        }

        function pegaGrafico() {

            return FindControl("hdfTempFile", "input").value;
        }

        var data;

        function openRpt() {

            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idVendedor = FindControl("drpVendedor", "select").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var tipoFunc = FindControl("drpTipoFunc", "select").value;
            var tipoPedido = FindControl("cblTipoPedido", "select").itens();
            var idRota = FindControl("drpRota", "select").value;
            var agrupar = FindControl("drpAgrupar", "select").value;
            var series = FindControl("hdfSeries", "input").value;
            var periodos = FindControl("hdfPeriodos", "input").value;
            var tempFile = FindControl("hdfTempFile", "input").value;
            var rel = openWindow(600, 800, "RelBase.aspx?postData=getPostData()");
            data = new Object();
            data["rel"] = "GraficoVendas";
            data["dataIni"] = dataIni;
            data["dataFim"] = dataFim;
            data["idLoja"] = idLoja;
            data["idVendedor"] = idVendedor;
            data["idCliente"] = idCliente;
            data["nomeCliente"] = nomeCliente;
            data["tipoFunc"] = tipoFunc;
            data["tipoPedido"] = tipoPedido;
            data["idRota"] = idRota;
            data["agrupar"] = agrupar;
            data["series"] = series;
            data["periodos"] = periodos;
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
                            <asp:Label ID="Label1" runat="server" Text="Tipo Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoFunc" runat="server" AutoPostBack="True" 
                                onselectedindexchanged="drpTipoFunc_SelectedIndexChanged">
                                <asp:ListItem Value="0">Vendedor (Assoc. Pedido)</asp:ListItem>
                                <asp:ListItem Value="1">Vendedor (Assoc. Cliente)</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedor" runat="server" DataSourceID="odsVendedor" DataTextField="Nome"
                                DataValueField="IdFunc" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblTipoPedido" runat="server" ForeColor="#0066FF" Text="Tipo"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblTipoPedido" runat="server" CheckAll="False" Title="Selecione o tipo"
                                DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OnDataBound="cblTipoPedido_DataBound"
                                OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" ForeColor="#0066FF" Text="Rota"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpRota" runat="server" DataSourceID="odsRota" DataTextField="Descricao"
                                DataValueField="IdRota" AppendDataBoundItems="True" AutoPostBack="true">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
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
                                <asp:ListItem Value="2">Emissor/Vendedor</asp:ListItem>
                                <asp:ListItem Value="3">Cliente</asp:ListItem>
                                <asp:ListItem Value="4">Tipo Pedido</asp:ListItem>
                                <asp:ListItem Value="5">Rota</asp:ListItem>
                                <asp:ListItem Value="0">Nenhum</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <div id="msChart">
                    <asp:Chart ID="Chart1" runat="server">
                    </asp:Chart>
                </div>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdVendas" runat="server" AutoGenerateColumns="False"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EnableViewState="False">
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false">
                    <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfChartData" runat="server" />
                <asp:HiddenField ID="hdfSeries" runat="server" />
                <asp:HiddenField ID="hdfPeriodos" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVendedor" runat="server" SelectMethod="GetVendedoresComVendas"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" DefaultValue="" Name="idLoja" PropertyName="SelectedValue" Type="UInt32" />
                        <asp:Parameter Name="funcCliente" DefaultValue="false" Type="Boolean" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:Parameter Name="quantidadeRegistros" Type="Int32" DefaultValue="30" />
                        <asp:Parameter Name="valorVendidoMinimoNoPeriodo" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="buscarSomenteFuncionarioAtivo" Type="Boolean" DefaultValue="false" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoPedido" runat="server" SelectMethod="GetTipoPedidoFilter"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRota" runat="server" SelectMethod="ObterRotas" TypeName="Glass.Data.DAL.RotaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfTempFile" runat="server" />
</asp:Content>
