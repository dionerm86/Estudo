<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaPedidos.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaPedidos" Title="Pedidos" %>

<%@ Register Src="../Controls/ctrlBenefSetor.ascx" TagName="ctrlBenefSetor" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function exibirBenef(botao) {
            for (iTip = 0; iTip < 2; iTip++) {
                TagToTip('benef', FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamentos', CLOSEBTN, true,
                    CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                    FIX, [botao, 0, 0]);
            }
        }

        function openRptBase(exportarExcel, nomeRel) {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idOrcamento = FindControl("txtIdOrcamento", "input").value;
            var codCliente = FindControl("txtNumPedCli", "input").value;
            var idsRota = FindControl("cblRota", "select").itens();
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var tipoFiscal = FindControl("drpTipoFiscal", "select").value;
            var loja = FindControl("drpLoja", "select").value;
            var situacao = FindControl("cbdSituacao", "select").itens();
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dtIniSit = FindControl("ctrlDataSitIni_txtData", "input").value;
            var dtFimSit = FindControl("ctrlDataSitFim_txtData", "input").value;
            var dtIniEnt = FindControl("ctrlDataIniEnt_txtData", "input").value;
            var dtFimEnt = FindControl("ctrlDataFimEnt_txtData", "input").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var idVendAssoc = FindControl("drpVendAssoc", "select").value;
            var ordenacao = FindControl("drpOrdenacao", "select").value;
            var situacaoProd = FindControl("cbdSituacaoProd", "select").itens();
            var tipoEntrega = FindControl("drpTipoEntrega", "select").value;
            var tipoVenda = FindControl("cblTipoVenda", "select").itens();
            var idsGrupos = FindControl("cbdGrupo", "select").itens();
            var idsSubgrupoProd = FindControl("drpSubgrupo", "select").itens();
            var idsBenef = FindControl("hdfBenef", "input").value;
            var exibirProdutos = FindControl("chkExibirProdutos", "input").checked;
            var pedidosSemAnexos = FindControl("chkPedidosSemAnexo", "input").checked;
            var altura = FindControl("txtAltura", "input").value;
            var largura = FindControl("txtLargura", "input").value;
            var tipoCliente = FindControl("drpTipoCliente", "select").itens();
            var trazerPedCliVinculado = FindControl("chkCliVinculado", "input").checked;
            var agrupar = FindControl("cbdAgrupar", "select") != null ? FindControl("cbdAgrupar", "select").itens() : "";
            var desconto = FindControl("drpDesconto", "select").value;
            var cidade = FindControl("hdfCidade", "input").value;
            var comSemNf = FindControl("cblNotaFiscal", "select").itens();
            var origemPedido = FindControl("drpOrigemPedido", "select").value;
            var exibirPronto = FindControl("chkFiltroPronto", "input");
            exibirPronto = exibirPronto != null && exibirPronto.checked;
            var dataIniPronto = "", dataFimPronto = "", diasDifProntoLib = "";
            var obs = FindControl("txtObs", "input").value;
            var idCarregamento = FindControl("txtCarregamento", "input").value;
            var bairro = FindControl("txtBairro", "input").value;
            var dtIniMed = FindControl("ctrlDataMedIni_txtData", "input").value;
            var dtFimMed = FindControl("ctrlDataMedFim_txtData", "input").value;

            if (exibirPronto && FindControl("ctrlDataProntoIni_txtData", "input") != null) {
                dataIniPronto = FindControl("ctrlDataProntoIni_txtData", "input").value;
                dataFimPronto = FindControl("ctrlDataProntoFim_txtData", "input").value;
                diasDifProntoLib = FindControl("txtDiasProntoLib", "input").value;
            }

            var dataIniInst = "", dataFimInst = "";
            if (FindControl("ctrlDataIniInst_txtData", "input") != null) {
                dataIniInst = FindControl("ctrlDataIniInst_txtData", "input").value;
                dataFimInst = FindControl("ctrlDataFimInst_txtData", "input").value;
            }

            var tipo = FindControl("cblTipoPedido", "select").itens();
            var fastDelivery = FindControl("drpFastDelivery", "select");
            fastDelivery = fastDelivery != null ? fastDelivery.value : "0";

            var codProd = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescrProd", "input").value;
            var esconderTotal = FindControl("chkEsconderTotal", "input").checked;
            var mostrarDescontoTotal = FindControl("chkMostrarDescontoTotal", "input").checked;
            var idMedidor = FindControl("cbMedidorPedido", "select").value;
            var idOC = FindControl("txtIdOC", "input").value;
            var usuCad = FindControl("drpUsucad", "select").value;

            var queryString = "&idPedido=" + idPedido + "&idOrcamento=" + idOrcamento + "&codCliente=" + codCliente + "&idsRota=" + idsRota + "&IdCli=" + idCli +
                "&nomeCli=" + nomeCli + "&tipoFiscal=" + tipoFiscal + "&loja=" + loja + "&situacao=" + situacao + "&dtIniSit=" + dtIniSit +
                "&dtFimSit=" + dtFimSit + "&dtIni=" + dtIni + "&dtFim=" + dtFim + "&dtIniEnt=" + dtIniEnt + "&dtFimEnt=" + dtFimEnt +
                "&idFunc=" + idFunc + "&idVendAssoc=" + idVendAssoc + "&tipo=" + tipo + "&tipoEntrega=" + tipoEntrega +
                "&fastDelivery=" + fastDelivery + "&ordenacao=" + ordenacao + "&situacaoProd=" + situacaoProd + "&tipoVenda=" + tipoVenda +
                "&idsGrupos=" + idsGrupos + "&idsSubgrupoProd=" + idsSubgrupoProd + "&idsBenef=" + idsBenef + "&exibirProdutos=" + exibirProdutos +
                "&pedidosSemAnexos=" + pedidosSemAnexos + "&exibirPronto=" + exibirPronto + "&dataIniPronto=" + dataIniPronto +
                "&dataFimPronto=" + dataFimPronto + "&diasDifProntoLib=" + diasDifProntoLib + "&dataIniInst=" + dataIniInst +
                "&dataFimInst=" + dataFimInst + "&altura=" + altura + "&largura=" + largura + "&codProd=" + codProd + "&descrProd=" + descrProd +
                "&tipoCliente=" + tipoCliente + "&trazerPedCliVinculado=" + trazerPedCliVinculado + "&esconderTotal=" + esconderTotal +
                "&mostrarDescontoTotal=" + mostrarDescontoTotal + "&desconto=" + desconto + "&agrupar=" + agrupar +
                "&cidade=" + cidade + "&comSemNf=" + comSemNf + "&idMedidor=" + idMedidor +
                "&idOC=" + idOC + "&usuCad=" + usuCad + "&origemPedido=" + origemPedido + "&exportarExcel=" + exportarExcel + "&observacao=" + obs + "&idCarregamento=" + idCarregamento + 
                "&bairro=" + bairro + "&dataInicioMedicao" + dtIniMed + "&dataFimMedicao" + dtFimMed;;

            openWindow(600, 800, 'RelBase.aspx?rel=' + nomeRel + queryString);
            return false;
        }

        function openRpt(exportarExcel) {
            return openRptBase(exportarExcel, 'ListaPedidos<%= Request["prod"] == "1" ? "Prod" : "" %>');
        }

        function openRptSimples(exportarExcel) {
            return openRptBase(exportarExcel, 'ListaPedidosSimples');
        }

        function openRptRota(exportarExcel) {
            return openRptBase(exportarExcel, 'ListaPedidosRota');
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function getProduto(codProd) {
            if (codProd.value == "")
                return;

            var retorno = MetodosAjax.GetProd(codProd.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                codProd.value = "";
                FindControl("txtDescrProd", "input").value = "";
                return false;
            }

            FindControl("txtDescrProd", "input").value = retorno[2];
        }

        function openRptUnico(idPedido) {
            openWindow(600, 800, "RelPedido.aspx?idPedido=" + idPedido);
            return false;
        }

        function openRota() {
            if (FindControl("txtRota", "input").value != "")
                return true;

            openWindow(500, 700, "../Utils/SelRota.aspx");
            return false;
        }

        function setRota(codInterno) {
            FindControl("txtRota", "input").value = codInterno;
        }

        function setCidade(idCidade, nomeCidade) {
            FindControl('hdfCidade', 'input').value = idCidade;
            FindControl('txtCidade', 'input').value = nomeCidade;
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblIdOrcamento" runat="server" Text="Orçamento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdOrcamento" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton17" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Pedido Cli." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedCli" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:Label ID="Label25" runat="server" Text="Ordem de Carga" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdOC" runat="server" MaxLength="20" Width="80px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSituacao" runat="server" Title="Selecione a situação"
                                OpenOnStart="False" AltRowColor="" DataSourceID="odsSituacao" DataTextField="Descr"
                                DataValueField="Id" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label23" runat="server" Text="Cidade" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" Width="200px" ReadOnly="True"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx'); return false;" />
                            <asp:HiddenField ID="hdfCidade" runat="server" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left" nowrap="nowrap">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label15" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <sync:CheckBoxListDropDown ID="cblRota" runat="server" Width="110px" CheckAll="False"
                                            Title="Selecione a rota" DataSourceID="odsRota" DataTextField="Descricao" DataValueField="IdRota"
                                            ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                            OpenOnStart="False">
                                        </sync:CheckBoxListDropDown>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                                            TypeName="Glass.Data.DAL.RotaDAO">
                                        </colo:VirtualObjectDataSource>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton16" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label8" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                            onblur="getCli(this);"></asp:TextBox>
                                        <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label26" runat="server" Text="Tipo Fiscal" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpTipoFiscal" runat="server">
                                            <asp:ListItem Value="0">Todos</asp:ListItem>
                                            <asp:ListItem Value="1">Consumidor Final</asp:ListItem>
                                            <asp:ListItem Value="2">Revenda</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton13" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            oolTip="Pesquisar" OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label22" runat="server" Text="Tipo do Cliente" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <sync:CheckBoxListDropDown ID="drpTipoCliente" runat="server" DataSourceID="odsTipoCliente"
                                            DataTextField="Name" DataValueField="Id" AppendDataBoundItems="True"
                                            Title="Selecione o tipo de cliente">
                                        </sync:CheckBoxListDropDown>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoCliente" runat="server"
                                            SelectMethod="ObtemDescritoresTipoCliente" TypeName="Glass.Global.Negocios.IClienteFluxo">
                                        </colo:VirtualObjectDataSource>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                            OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkCliVinculado" runat="server" Text="Trazer pedidos de clientes vinculados" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label31" runat="server" Text="Período (Medição)" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataMedIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataMedFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton21" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>                        
                        <td>
                            <asp:Label ID="Label27" runat="server" Text="Bairro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtBairro" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                MaxLength="50"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton20" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" ForeColor="#0066FF" Text="Vendedor (Assoc. Pedido)"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblVendAssoc" runat="server" ForeColor="#0066FF" Text="Vendedor (Assoc. Cliente)"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendAssoc" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label28" runat="server" ForeColor="#0066FF" Text="Origem Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrigemPedido" runat="server" AutoPostBack="true">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Ecommerce</asp:ListItem>
                                <asp:ListItem Value="2">Normal</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblTipoPedido" runat="server" ForeColor="#0066FF" Text="Tipo de pedido"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblTipoPedido" runat="server" CheckAll="False" Title="Selecione o tipo"
                                DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OnDataBound="cblTipoPedido_DataBound"
                                OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:Label ID="lblUsucad" runat="server" ForeColor="#0066FF" Text="Usuário Cad."></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpUsucad" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton15" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblTipoPedido0" runat="server" ForeColor="#0066FF" Text="Tipo Entrega"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoEntrega" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsTipoEntrega" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Tipo de venda"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblTipoVenda" runat="server" DataSourceID="odsTipoVenda"
                                DataTextField="Descr" DataValueField="Id" CheckAll="False" Title="Selecione o tipo de venda"
                                OnDataBound="cblTipoVenda_DataBound">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:Label ID="lblFastDelivery" runat="server" ForeColor="#0066FF" Text="Fast Delivery"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFastDelivery" runat="server">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Sim</asp:ListItem>
                                <asp:ListItem Value="2">Não</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblMedidor" runat="server" Text="Medidor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="cbMedidorPedido" AppendDataBoundItems="true" DataSourceID="odsMedidorPedido"
                                DataTextField="NOME" DataValueField="IDFUNC" runat="server">
                                <asp:ListItem Text="Selecione..." Value="0" />
                            </asp:DropDownList>
                        </td>
                        <td style='<%= ExibirInstalacao() %>'>
                            <asp:Label ID="Label18" runat="server" ForeColor="#0066FF" Text="Período Instalação"></asp:Label>
                        </td>
                        <td style='<%= ExibirInstalacao() %>'>
                            <uc2:ctrlData ID="ctrlDataIniInst" runat="server" />
                        </td>
                        <td style='<%= ExibirInstalacao() %>'>
                            <uc2:ctrlData ID="ctrlDataFimInst" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label14" runat="server" Text="Período (Situação)" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataSitIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataSitFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq6" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Período (Pedido)" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniEnt" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimEnt" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left" nowrap="nowrap">
                            <asp:Label ID="Label24" runat="server" Text="Nota Fiscal" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <sync:CheckBoxListDropDown ID="cblNotaFiscal" runat="server" CheckAll="True">
                                <asp:ListItem Value="1">Com NF gerada</asp:ListItem>
                                <asp:ListItem Value="2">Sem NF gerada</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton12" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblDesconto" runat="server" Text="Desconto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpDesconto" runat="server">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Com Desconto</asp:ListItem>
                                <asp:ListItem Value="2">Sem Desconto</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton14" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td runat="server" id="filtroPronto">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label16" runat="server" ForeColor="#0066FF" Text="Período (Pronto)"></asp:Label>
                                    </td>
                                    <td>
                                        <uc2:ctrlData ID="ctrlDataProntoIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                                    </td>
                                    <td>
                                        <uc2:ctrlData ID="ctrlDataProntoFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="Diferença dias entre Pedido Pronto e Liberado"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDiasProntoLib" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblSituacaoProd" runat="server" ForeColor="#0066FF" Text="Situação Prod."></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSituacaoProd" runat="server" Title="Selecione a situação"
                                OpenOnStart="False" AltRowColor="" DataSourceID="odsSituacaoProd" DataTextField="Descr"
                                DataValueField="Id" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Grupo"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdGrupo" runat="server" CheckAll="False" DataSourceID="odsGrupo"
                                DataTextField="Descricao" DataValueField="IdGrupoProd" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False"
                                Title="Selecione o grupo">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Subgrupo"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupo"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd" OnDataBound="drpSubgrupo0_DataBound">
                            </sync:CheckBoxListDropDown>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label21" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="getProduto(this);"></asp:TextBox>
                            <asp:TextBox ID="txtDescrProd" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getProduto(FindControl('txtCodProd', 'input'));"
                                OnClick="imgPesq_Click" Height="16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label19" runat="server" Text="Altura Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAltura" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label20" runat="server" Text="Largura Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLargura" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" ForeColor="#0066FF" Text="Beneficiamentos"></asp:Label>
                        </td>
                        <td>
                            <img id="botaoExibirBenef" src="../Images/gear_add.gif" border="0" style="cursor: pointer"
                                onclick="exibirBenef(this)" />
                            <div id="benef" style="display: none">
                                <uc1:ctrlBenefSetor ID="ctrlBenefSetor1" runat="server" FuncaoExibir="exibirBenef(document.getElementById('botaoExibirBenef'))" />
                            </div>
                            <asp:HiddenField ID="hdfBenef" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblAgrupar" runat="server" ForeColor="#0066FF" Text="Agrupar por"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdAgrupar" runat="server">
                                <asp:ListItem Value="5">Cidade</asp:ListItem>
                                <asp:ListItem Value="1">Cliente</asp:ListItem>
                                <asp:ListItem Value="2">Vendedor (Assoc. Pedido)</asp:ListItem>
                                <asp:ListItem Value="3">Vendedor (Assoc. Cliente)</asp:ListItem>
                                <asp:ListItem Value="4">Data Pedido</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Ordenar por"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenacao" runat="server">
                                <asp:ListItem Value="0">Data do pedido (descresc.)</asp:ListItem>
                                <asp:ListItem Value="1">Data de entrega (cresc.)</asp:ListItem>
                                <asp:ListItem Value="2">Data de entrega (descresc.)</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label29" runat="server" Text="Observação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton19" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label30" runat="server" Text="Carregamento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCarregamento" runat="server" Width="50px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton18" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkExibirProdutos" runat="server" Text="Exibir Produtos no Relatório (ao agrupar)" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkEsconderTotal" runat="server" Text="Esconder Total no Relatório" 
                                OnCheckedChanged="chkEsconderTotal_CheckedChanged"/>
                        </td>
                        <td>&nbsp;
                            <asp:CheckBox ID="chkMostrarDescontoTotal" runat="server" Text="Mostrar Desconto Total"
                                OnCheckedChanged="chkMostrarDescontoTotal_CheckedChanged" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkPedidosSemAnexo" runat="server" Text="Pedidos sem anexos" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkFiltroPronto" runat="server" Text="Exibir data em que o Pedido ficou Pronto" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkLimparFiltros_Click"> <img border="0" src="../Images/ExcluirGrid.gif" /> Limpar filtros</asp:LinkButton>
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
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsPedido" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdPedido" EmptyDataText="Nenhum pedido encontrado."
                    AllowPaging="True" OnRowCommand="grdPedido_RowCommand">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Eval("ExibirRelatorio") %>'>
                                    <a href="#" onclick="openRptUnico('<%# Eval("IdPedido") %>');">
                                        <img border="0" src="../Images/Relatorio.gif" /></a> </asp:PlaceHolder>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Num" SortExpression="IdPedido" />
                        <asp:BoundField DataField="IdOrcamento" HeaderText="Orca" SortExpression="IdOrcamento" />
                        <asp:BoundField DataField="NfeAssociada" HeaderText="Num. NFe" SortExpression="NfeAssociada" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:TemplateField HeaderText="Cidade" SortExpression="RptCidade">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("RptCidade") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("RptCidade") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodCliente" HeaderText="Pedido Cli." SortExpression="CodCliente" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="TotalComDescontoConcatenado" HeaderText="Total" SortExpression="TotalComDescontoConcatenado" />
                        <asp:TemplateField HeaderText="Desconto Total" SortExpression="TotalComDescontoConcatenado" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblTotal" runat="server" Text='<%# Bind("TextoDescontoTotalPerc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrTipoVenda" HeaderText="Pagto" SortExpression="DescrTipoVenda">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataPedido" HeaderText="Data" SortExpression="DataPedido" />
                        <asp:BoundField DataField="DataEntregaExibicao" HeaderText="Entrega" SortExpression="DataEntrega" />
                        <asp:BoundField DataField="DataConfLib" HeaderText="Conf. / Lib." SortExpression="DataConfLib" />
                        <asp:BoundField DataField="DataPronto" HeaderText="Pronto" SortExpression="DataPronto" />
                        <asp:BoundField DataField="DescrSituacaoPedido" HeaderText="Situação" SortExpression="DescrSituacaoPedido" />
                        <asp:TemplateField HeaderText="Situação Prod." SortExpression="DescrSituacaoProducao">
                            <ItemTemplate>
                                <asp:Label ID="lblSitProd" runat="server" OnLoad="lblSitProd_Load" Text='<%# Eval("DescrSituacaoProducao") %>'></asp:Label>
                                <asp:LinkButton ID="lnkSitProd" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                    CommandName="Producao" OnLoad="lblSitProd_Load" Text='<%# Eval("DescrSituacaoProducao") %>'></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DescrSituacaoProducao") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescricaoTipoPedido" HeaderText="Tipo" SortExpression="DescricaoTipoPedido" />
                        <asp:BoundField DataField="FastDeliveryString" HeaderText="Fast Delivery?" SortExpression="FastDeliveryString" />
                        <asp:BoundField DataField="TotM" HeaderText="Total m²" SortExpression="TotM" />
                        <asp:BoundField DataField="Peso" HeaderText="Peso total" SortExpression="Peso" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
         <tr>
             <td align="center">
                <asp:DetailsView ID="dtvTotaisPedidos" runat="server" DataSourceID="odsTotaisPedidos" AutoGenerateRows="False" GridLines="None">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblTotal" runat="server" Font-Bold="true" Font-Size="Medium" Text='<%# string.Format("Total: {0:C}", Eval("Total") != null ? Eval("Total") : "0") %>'></asp:Label>
                                        </td>
                                        <td>&nbsp;&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="lblTotM" runat="server" Font-Bold="true" Font-Size="Medium" Text='<%# string.Format("M2: {0:N}m²", Eval("TotM") != null ? Eval("TotM") : "0") %>'></asp:Label>
                                        </td>
                                        <td>&nbsp;&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPeso" runat="server" Font-Bold="true" Font-Size="Medium" Text='<%# string.Format("Peso: {0:N}kg", Eval("Peso") != null ? Eval("Peso") : "0") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTotaisPedidos" runat="server" SelectMethod="ObterTotaisListaPedidos" TypeName="Glass.Data.DAL.PedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtIdOrcamento" Name="idOrcamento" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumPedCli" Name="codCliente" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="cblRota" Name="idsRota" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpTipoFiscal" Name="tipoFiscal" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="loja" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="cbdSituacao" Name="situacao" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataSitIni" Name="dataInicioSituacao" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataSitFim" Name="dataFimSituacao" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataInicioPedido" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFimPedido" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniEnt" Name="dataInicioEntrega" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimEnt" Name="dataFimEntrega" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpVendAssoc" Name="idVendAssoc" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="cblTipoPedido" Name="tiposPedido" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="drpTipoEntrega" Name="tipoEntrega" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpFastDelivery" Name="fastDelivery" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpOrdenacao" Name="ordenacao" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="cbdSituacaoProd" Name="situacaoProducao" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="cblTipoVenda" Name="tiposVenda" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="drpSubgrupo" Name="idsSubgrupoProd" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="hdfBenef" Name="idsBenef" PropertyName="Value" Type="String" />
                        <asp:Parameter DefaultValue="false" Name="exibirProdutos" Type="Boolean" />
                        <asp:ControlParameter ControlID="chkPedidosSemAnexo" Name="pedidosSemAnexos" PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="ctrlDataProntoIni" Name="dataInicioPronto" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataProntoFim" Name="dataFimPronto" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="txtDiasProntoLib" Name="numeroDiasDiferencaProntoLib" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIniInst" Name="dataInicioInstalacao" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimInst" Name="dataFimInstalacao" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="txtAltura" Name="altura" PropertyName="Text" Type="Single" />
                        <asp:ControlParameter ControlID="txtLargura" Name="largura" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codigoProduto" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtDescrProd" Name="descricaoProduto" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="cbdGrupo" Name="idsGrupo" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="drpTipoCliente" Name="tipoCliente" Type="String" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="drpDesconto" Name="desconto" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="hdfCidade" Name="cidade" PropertyName="Value" Type="Int32" />
                        <asp:ControlParameter ControlID="cblNotaFiscal" Name="comSemNF" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="cbMedidorPedido" Name="idMedidor" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="chkCliVinculado" Name="trazerPedCliVinculado" PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="txtIdOC" Name="idOC" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="drpUsucad" Name="usuarioCadastro" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpOrigemPedido" Name="origemPedido" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="txtObs" Name="observacao" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtCarregamento" Name="idCarregamento" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataMedIni" Name="dataInicioMedicao" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataMedFim" Name="dataFimMedicao" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:LinkButton ID="lnkImprimirSimples" runat="server" OnClientClick="return openRptSimples();"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir (Peso e Tot. m²)</asp:LinkButton>
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:LinkButton ID="lnkExportarExcelSimples" runat="server" OnClientClick="openRptSimples(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel (Peso e Tot. m²)</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:LinkButton ID="lnkImprimirRota" runat="server" OnClientClick="return openRptRota();"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir (Peso e Tot. m² por Rota)</asp:LinkButton>
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:LinkButton ID="lnkExportarExcelRota" runat="server" OnClientClick="openRptRota(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel (Peso e Tot. m² por Rota)</asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedido" runat="server" MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarListaVendasPedidos" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.PedidoDAO"
                    EnablePaging="True" SelectCountMethod="PesquisarListaVendasPedidosCount" SortParameterName="sortExpression">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtIdOrcamento" Name="idOrcamento" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumPedCli" Name="codCliente" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="cblRota" Name="idsRota" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpTipoFiscal" Name="tipoFiscal" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="loja" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="cbdSituacao" Name="situacao" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataSitIni" Name="dataInicioSituacao" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataSitFim" Name="dataFimSituacao" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataInicioPedido" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFimPedido" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniEnt" Name="dataInicioEntrega" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimEnt" Name="dataFimEntrega" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpVendAssoc" Name="idVendAssoc" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="cblTipoPedido" Name="tiposPedido" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="drpTipoEntrega" Name="tipoEntrega" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpFastDelivery" Name="fastDelivery" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpOrdenacao" Name="ordenacao" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="cbdSituacaoProd" Name="situacaoProducao" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="cblTipoVenda" Name="tiposVenda" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="drpSubgrupo" Name="idsSubgrupoProd" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="hdfBenef" Name="idsBenef" PropertyName="Value" Type="String" />
                        <asp:Parameter DefaultValue="false" Name="exibirProdutos" Type="Boolean" />
                        <asp:ControlParameter ControlID="chkPedidosSemAnexo" Name="pedidosSemAnexos" PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="ctrlDataProntoIni" Name="dataInicioPronto" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataProntoFim" Name="dataFimPronto" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="txtDiasProntoLib" Name="numeroDiasDiferencaProntoLib" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIniInst" Name="dataInicioInstalacao" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimInst" Name="dataFimInstalacao" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="txtAltura" Name="altura" PropertyName="Text" Type="Single" />
                        <asp:ControlParameter ControlID="txtLargura" Name="largura" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codigoProduto" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtDescrProd" Name="descricaoProduto" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="cbdGrupo" Name="idsGrupo" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="drpTipoCliente" Name="tipoCliente" Type="String" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="drpDesconto" Name="desconto" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="hdfCidade" Name="cidade" PropertyName="Value" Type="Int32" />
                        <asp:ControlParameter ControlID="cblNotaFiscal" Name="comSemNF" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="cbMedidorPedido" Name="idMedidor" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="chkCliVinculado" Name="trazerPedCliVinculado" PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="txtIdOC" Name="idOC" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="drpUsucad" Name="usuarioCadastro" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpOrigemPedido" Name="origemPedido" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="txtObs" Name="observacao" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtCarregamento" Name="idCarregamento" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataMedIni" Name="dataInicioMedicao" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataMedFim" Name="dataFimMedicao" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacaoProd" runat="server"
                    SelectMethod="GetSituacaoProducao" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacao" runat="server" SelectMethod="GetSituacaoPedido"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoEntrega" runat="server"
                    SelectMethod="GetTipoEntrega" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoVenda" runat="server" SelectMethod="GetTipoVenda"
                    TypeName="Glass.Data.Helper.DataSources">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="false" Name="incluirVazio" Type="Boolean" />
                        <asp:Parameter DefaultValue="true" Name="paraFiltro" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGrupoProd" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.GrupoProdDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPedido" runat="server" SelectMethod="GetTipoPedidoFilter"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="cbdGrupo" Name="idGrupos" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.GrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="false" Name="incluirTodos" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsMedidorPedido" runat="server"
                    SelectMethod="GetMedidores" TypeName="Glass.Data.DAL.FuncionarioDAO" />
            </td>
        </tr>
    </table>
</asp:Content>
