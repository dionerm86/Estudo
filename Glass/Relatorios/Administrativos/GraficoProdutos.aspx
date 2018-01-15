<%@ Page Title="Gráfico de Produtos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="GraficoProdutos.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.GraficoProdutos" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src="../../Scripts/FusionCharts.js"></script>

    <script type="text/javascript">

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = GraficoProdutos.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function Graph() {
            if (!validate())
                return;

            var myChart = new FusionCharts("../../Charts/Pie2D.swf", "myChartId", "800", "400");

            var idLoja = FindControl("drpLoja", "select").value;
            var idVend = FindControl("drpVendedor", "select").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var idGrupo = FindControl("drpGrupoProd", "select").value;
            var idSubgrupo = FindControl("drpSubgrupoProd", "select").value;
            var qtde = FindControl("txtQuantidade", "input").value;
            var tipo = FindControl("drpTipo", "select").value;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var codInternoMP = FindControl("txtCodProd", "input").value;
            var descrMP = FindControl("txtDescr", "input").value;
            var apenasMP = FindControl("chkApenasMateriaPrima", "input").checked;

            myChart.setDataURL("../../Handlers/Chart.ashx?query=2;" + idLoja + ";" + idVend + ";" + idCliente + ";" + nomeCliente + ";" +
                idGrupo + ";" + idSubgrupo + ";" + qtde + ";" + tipo + ";" + dtIni + ";" + dtFim + ";" + codInternoMP + ";" + descrMP + ";" + apenasMP);
            myChart.render("divChart");
        }

        // Carrega dados do produto com base no código do produto passado
        function setProduto() {
            var codInterno = FindControl("txtCodProd", "input").value;

            if (codInterno == "")
                return false;

            try {
                var retorno = MetodosAjax.GetProd(codInterno).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    FindControl("txtCodProd", "input").value = "";
                    return false;
                }

                FindControl("txtDescr", "input").value = retorno[2];
            }
            catch (err) {
                alert(err.value);
            }
        }

        function openRptTeste() {
            var idLoja = FindControl("drpLoja", "select").value;
            var idVend = FindControl("drpVendedor", "select").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var idGrupo = FindControl("drpGrupoProd", "select").value;
            var idSubgrupo = FindControl("drpSubgrupoProd", "select").value;
            var qtde = FindControl("txtQuantidade", "input").value;
            var tipo = FindControl("drpTipo", "select").value;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var tempFile = FindControl("hdfTempFile", "input").value;

            openWindow(600, 800, "RelBase.aspx?rel=GraficoProdutos&idLoja=" + idLoja + "&dtIni=" + dtIni + "&dtFim=" + dtFim +
                "&idVend=" + idVend + "&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente + "&idGrupo=" + idGrupo +
                "&idSubgrupo=" + idSubgrupo + "&qtde=" + qtde + "&tipo=" + tipo + "&tempFile=" + tempFile);

            return false;
        }


        var data;

        function openRpt() {

            var idLoja = FindControl("drpLoja", "select").value;
            var idVend = FindControl("drpVendedor", "select").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var idGrupo = FindControl("drpGrupoProd", "select").value;
            var idSubgrupo = FindControl("drpSubgrupoProd", "select").value;
            var qtde = FindControl("txtQuantidade", "input").value;
            var tipo = FindControl("drpTipo", "select").value;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var tempFile = FindControl("hdfTempFile", "input").value;

            var rel = openWindow(600, 800, "RelBase.aspx?postData=getPostData()");
            data = new Object();
            data["rel"] = "GraficoProdutos";
            data["idLoja"] = idLoja;
            data["idVend"] = idVend;
            data["idCliente"] = idCliente;
            data["nomeCliente"] = nomeCliente;
            data["idGrupo"] = idGrupo;
            data["idSubgrupo"] = idSubgrupo;
            data["qtde"] = qtde;
            data["tipo"] = tipo;
            data["dtIni"] = dtIni;
            data["dtFim"] = dtFim;
            data["tempFile"] = tempFile;            
        }

        function getPostData() {
            return data;
        }
        function mudaMP(apenasMP) {
            var tabela = apenasMP;
            while (tabela.nodeName.toLowerCase() != "table")
                tabela = tabela.parentNode;

            for (i = 2; i < tabela.rows[0].cells.length; i++)
                tabela.rows[0].cells[i].style.display = apenasMP.checked ? "none" : "";

            if (apenasMP) {
                FindControl("txtCodProd", "input").value = "";
                FindControl("txtDescr", "input").value = "";
            }
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
                                DataValueField="IdFunc" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupoProd" runat="server" DataSourceID="odsGrupoProd" DataTextField="Descricao"
                                DataValueField="IdGrupoProd" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupoProd" runat="server" DataSourceID="odsSubgrupoProd"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Quantidade de produtos" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtQuantidade" runat="server" onkeypress="return soNumeros(event, true, true)"
                                Width="50px" Text="10"></asp:TextBox>
                            <asp:RangeValidator ID="rgvQtde" runat="server" ErrorMessage="Valor de 5 a 50." ControlToValidate="txtQuantidade"
                                Display="Dynamic" MaximumValue="50" MinimumValue="5" Type="Integer"></asp:RangeValidator>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Buscar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="1">Quantidade vendida</asp:ListItem>
                                <asp:ListItem Value="2">Valor vendido</asp:ListItem>
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
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkApenasMateriaPrima" runat="server" Text="Apenas matéria-prima"
                                onclick="mudaMP(this)" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Matéria-Prima" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <div id="divChart" align="center" style="z-index: -1;">
                </div>
            </td>
        </tr>
        <tr>
            <td align="Center">
                <asp:CheckBox ID="chkLegenda" runat="server" AutoPostBack="true" Text="Visualizar Legenda"
                    ForeColor="#0066FF" />
            </td>
        </tr>
        <td align="center">
            <div id="msChart" align="center" style="z-index: -1;">
                <asp:Chart ID="Chart1" runat="server">
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
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVendedor" runat="server" SelectMethod="GetVendedoresComVendas"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" DefaultValue="" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoProd" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.GrupoProdDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupoProd" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpGrupoProd" Name="idGrupo" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfTempFile" runat="server" />

    <script>
        mudaMP(FindControl("chkApenasMateriaPrima", "input"));
        //Graph();
    </script>

</asp:Content>
