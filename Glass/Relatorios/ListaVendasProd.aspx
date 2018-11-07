<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaVendasProd.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaVendasProd" Title="Vendas de Produtos" %>

<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRota() {
            if (FindControl("txtRota", "input").value != "")
                return true;

            openWindow(500, 700, "../Utils/SelRota.aspx");
            return false;
        }

        function setRota(codInterno) {
            FindControl("txtRota", "input").value = codInterno;
        }

        function openRpt(exportarExcel, agrupar) {
            var idLoja = FindControl("drpLoja", "select").value;
            var idsGrupos = FindControl("cbdGrupo", "select").itens();
            var situacaoProd = FindControl("cbdSituacaoProd", "select").itens();
            var idsSubgrupo = FindControl("cbdSubgrupo", "select").itens();
            var codInterno = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var codRota = FindControl("txtRota", "input").value;
            var dtIni = FindControl("ctrlDataIniSit_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFimSit_txtData", "input").value;
            var dtIniPed = FindControl("ctrlDataIniPed_txtData", "input").value;
            var dtFimPed = FindControl("ctrlDataFimPed_txtData", "input").value;
            var dtIniEnt = FindControl("ctrlDataIniEnt_txtData", "input").value;
            var dtFimEnt = FindControl("ctrlDataFimEnt_txtData", "input").value;
            var situacao = FindControl("cbdSituacao", "select").itens();
            var agruparCorEsp = FindControl("chkAgruparCorEsp", "input").checked ? "1" : "0";
            var agruparGrupo = FindControl("chkAgruparGrupo", "input").checked ? "1" : "0";
            var agruparCli = FindControl("chkAgruparCli", "input").checked ? "1" : "0";
            var agruparPedido = FindControl("chkAgruparPedido", "input").checked ? "1" : "0";
            var agruparNcm = FindControl("chkAgruparNcm", "input").checked ? "1" : "0";
            var agruparLiberacao = FindControl("chkAgruparLiberacao", "input") != null && FindControl("chkAgruparLiberacao", "input").checked ? "1" : "0";
            var agruparAmbiente = FindControl("chkAmbiente", "input").checked ? "1" : "0";
            var ordenacao = FindControl("drpOrdenacao", "select").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var idFuncCliente = FindControl("drpVendedorCliente", "select");
            var idFuncPedido = FindControl("drpVendedorPedido", "select");
            idFuncPedido = idFuncPedido != null ? idFuncPedido.value : "0";
            idFuncCliente = idFuncCliente != null ? idFuncCliente.value : "0";
            var tipoVenda = FindControl("cbdTipoVenda", "select").itens();
            var tipoFastDelivery = FindControl("drpFastDelivery", "select").value;
            var incluirMateriaPrima = FindControl("chkMateriaPrima", "input").checked ? "1" : "0";
            var idPedido = FindControl("txtNumPedido", "input").value;
            var desconto = FindControl("drpDesconto", "select").value;
            var semValores = FindControl("chkSemValores", "input").checked;
            var notaFiscal = FindControl("drpNotaFiscal", "select"); notaFiscal = !!notaFiscal ? notaFiscal.value : "0";
            var idLiberacao = FindControl("txtIdLiberacao", "input").value;
            var liberacaoNf = FindControl("cbLiberarcaoNfe", "select").value;
            var idFuncLiberacao = FindControl("drpFunc", "select").value;

            idCliente = idCliente == "" ? 0 : idCliente;

            agrupar = agrupar == true;
            openWindow(600, 800, "RelBase.aspx?Rel=vendasProd&idLoja=" + idLoja + "&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente +
                "&codRota=" + codRota + "&codInterno=" + codInterno + "&descrProd=" + descrProd + "&dtIni=" + dtIni + "&dtFim=" + dtFim +
                "&idsGrupos=" + idsGrupos + "&idsSubgrupo=" + idsSubgrupo + "&dtIniPed=" + dtIniPed + "&dtFimPed=" + dtFimPed +
                "&dtIniEnt=" + dtIniEnt + "&dtFimEnt=" + dtFimEnt + "&situacao=" + situacao + "&situacaoProd=" + situacaoProd +
                "&agruparCli=" + agruparCli + "&agruparGrupo=" + agruparGrupo + "&ordenacao=" + ordenacao + "&idFunc=" + idFunc +
                "&idFuncCliente=" + idFuncCliente + "&idFuncPedido=" + idFuncPedido + "&exportarExcel=" + exportarExcel + "&tipoFastDelivery=" + tipoFastDelivery +
                "&tipoVenda=" + tipoVenda + "&idPedido=" + idPedido + "&agruparPedido=" + agruparPedido +
                "&incluirMateriaPrima=" + incluirMateriaPrima + "&tipoDesconto=" + desconto + "&agrupar=" + agrupar +
                "&agruparNcm=" + agruparNcm + "&agruparCorEsp=" + agruparCorEsp + "&semValores=" + semValores +
                "&agruparLiberacao=" + agruparLiberacao + "&buscarNotaFiscal=" + notaFiscal + "&idLiberacao=" + idLiberacao+
                "&agruparAmbiente=" + agruparAmbiente + "&liberacaoNf=" + liberacaoNf + "&idFuncLiberacao=" + idFuncLiberacao);

            return false;
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

        function limparFiltros() {
            FindControl("drpLoja", "select").selectedIndex = 0;
            FindControl("cbdGrupo", "select").selectedIndex = 0;
            FindControl("cbdSubgrupo", "select").selectedIndex = 0;
            FindControl("txtCodProd", "input").value = "";
            FindControl("txtDescr", "input").value = "";
            FindControl("txtNumCli", "input").value = "";
            FindControl("txtNome", "input").value = "";
            FindControl("txtRota", "input").value = "";
            FindControl("txtNumPedido", "input").value = "";
            FindControl("ctrlDataIniSit_txtData", "input").value = "";
            FindControl("ctrlDataFimSit_txtData", "input").value = "";
            FindControl("ctrlDataIniPed_txtData", "input").value = "";
            FindControl("ctrlDataFimPed_txtData", "input").value = "";
            FindControl("ctrlDataIniEnt_txtData", "input").value = "";
            FindControl("ctrlDataFimEnt_txtData", "input").value = "";
            FindControl("cbdSituacao", "select").itens();
            FindControl("chkAgruparGrupo", "input").checked = false;
            FindControl("chkAgruparCli", "input").checked = false;
            FindControl("drpFuncionario", "select").selectedIndex = 0;

            if (FindControl("drpVendedorCliente", "select") != null) {
                FindControl("drpVendedorCliente", "select").selectedIndex = 0;
            }

            if (FindControl("drpVendedorPedido", "select") != null) {
                FindControl("drpVendedorPedido", "select").selectedIndex = 0;
            }

            FindControl("drpTipoVenda", "select").selectedIndex = 0;
            FindControl("drpFastDelivery", "select").selectedIndex = 0;
            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" Width="150px" OnClientClick="if (isEnter(event)) cOnClick('imgPesq1', null); return false;"></asp:TextBox>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkMateriaPrima" runat="server" Text="Buscar matéria-prima" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgBtn" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="setProduto();" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdGrupo" runat="server" CheckAll="False" DataSourceID="odsGrupo"
                                DataTextField="Descricao" DataValueField="IdGrupoProd" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False"
                                Title="Selecione o grupo">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSubgrupo" runat="server" CheckAll="False" DataSourceID="odsSubgrupo"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False"
                                Title="Selecione o subgrupo" OnDataBound="cbdSubgrupo_DataBound">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton3" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdLiberacao" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton4" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label15" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtRota" runat="server" MaxLength="20" Width="80px"></asp:TextBox>
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" OnClientClick="return openRota();" />
                        </td>
                        <td align="left">
                            <asp:Label ID="Label12" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq1', null);"></asp:TextBox>
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="left">
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <uc3:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="True" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Período (Pedido)" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniPed" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimPed" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq2" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq5" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblSituacao" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSituacao" runat="server" DataSourceID="odsSituacao"
                                DataTextField="Descr" DataValueField="Id" Title="Selecione a situação" CheckAll="False"
                                OnDataBound="cbdSituacao_DataBound">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="left">
                            <asp:Label ID="lblPeriodoSituacao" runat="server" Text="Período (Situação)" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td align="left">
                                        <uc2:ctrlData ID="ctrlDataIniSit" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <uc2:ctrlData ID="ctrlDataFimSit" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="lnkPesq1" runat="server" OnClientClick="setProduto();"
                                            OnClick="lnkPesq_Click"><img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <asp:Label ID="lblFastDelivery" runat="server" Text="Fast Delivery" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFastDelivery" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Sim</asp:ListItem>
                                <asp:ListItem Value="2">Não</asp:ListItem>
                            </asp:DropDownList>
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
                        <td nowrap="nowrap">
                            <asp:Label ID="Label17" runat="server" Text="Liberado por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpFunc" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsFunc" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
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
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Tipo de venda" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdTipoVenda" runat="server" DataSourceID="odsTipoVenda"
                                DataTextField="Descr" DataValueField="Id" Title="Selecione o tipo de venda" CheckAll="false"
                                OnDataBound="cbdTipoVenda_DataBound">
                            </sync:CheckBoxListDropDown>
                        </td>                        
                        <td align="right">
                            <asp:Label ID="Label18" runat="server" Text="Nota Fiscal" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:DropDownList ID="cbLiberarcaoNfe" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0" Selected="True">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Apenas produtos de liberações com nota fiscal</asp:ListItem>
                                <asp:ListItem Value="2">Apenas produtos de liberações sem nota fiscal</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Período (Entrega)" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniEnt" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimEnt" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq4" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        </tr>
                    </table>
                    <table>
                    <tr>                        
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Vendedor (Assoc. Pedido)" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedorPedido" runat="server" DataSourceID="odsVendedor" DataTextField="Nome"
                                DataValueField="IdFunc" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Usuário Cad." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsVendedor" DataTextField="Nome"
                                DataValueField="IdFunc" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label19" runat="server" Text="Vendedor (Assoc. Cliente)" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedorCliente" runat="server" DataSourceID="odsVendedor" DataTextField="Nome"
                                DataValueField="IdFunc" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Total Vendido</asp:ListItem>
                                <asp:ListItem Value="1">Valor Vendido</asp:ListItem>
                                <asp:ListItem Value="2">Cód. Produto</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkLimparFiltros" runat="server" OnClientClick="return limparFiltros();"><img border="0"
                                src="../Images/ExcluirGrid.gif" /> Limpar filtros</asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Agrupar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparPedido" runat="server" AutoPostBack="True" Text="Pedido" />
                        </td>
                         <td>
                            <asp:CheckBox ID="chkAmbiente" runat="server" AutoPostBack="True" Text="Ambiente" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparLiberacao" runat="server" Text="Liberação de Pedido"
                                AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparCli" runat="server" Text="Cliente" AutoPostBack="True" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparGrupo" runat="server" Text="Grupo de produto" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparNcm" runat="server" Text="NCM" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparCorEsp" runat="server" Text="Cor/espessura" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkSemValores" runat="server" Text="Não exibir valores no relatório" />
                        </td>
                    </tr>
                </table>
                <table runat="server" id="filtroNotaFiscal">
                    <tr>
                        <td>
                            <asp:Label ID="Label14" runat="server" Text="Nota Fiscal" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpNotaFiscal" runat="server">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Produtos sem Nota Fiscal gerada</asp:ListItem>
                                <asp:ListItem Value="2">Produtos com Nota Fiscal gerada</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="setProduto();"
                                OnClick="lnkPesq_Click"><img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
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
                <asp:GridView GridLines="None" ID="grdVendasProd" runat="server" AllowPaging="True"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" PageSize="20" AutoGenerateColumns="False" DataSourceID="odsVendasProd"
                    EmptyDataText="Nenhum registro encontrado">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                         <asp:BoundField DataField="Ambiente" HeaderText="Ambiente"  Visible="False" />
                        <asp:BoundField DataField="IdLiberarPedido" HeaderText="Liberação"
                            SortExpression="IdLiberarPedido" Visible="False" />
                        <asp:BoundField DataField="CodInterno" HeaderText="Cod." SortExpression="CodInterno" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:TemplateField HeaderText="Cliente" SortExpression="IdClienteVend">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("IdClienteVend") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("IdClienteVend") + " - " + Eval("NomeClienteVend") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="TotalQtde" HeaderText="Qtde" SortExpression="TotalQtde" />
                        <asp:BoundField DataField="TotalM2Rel" HeaderText="Total M²" SortExpression="TotalM2Rel" />
                        <asp:BoundField DataField="TotalMLRel" HeaderText="Total ML" SortExpression="TotalMLRel" />
                        <asp:BoundField DataField="TotalCusto" DataFormatString="{0:C}" HeaderText="Custo Total"
                            SortExpression="TotalCusto" />
                        <asp:BoundField DataField="ValorVendido" DataFormatString="{0:C}" HeaderText="Valor Vendido"
                            SortExpression="ValorVendido" />
                        <asp:BoundField DataField="TotalVend" DataFormatString="{0:C}" HeaderText="Total Vendido"
                            SortExpression="TotalVend" />
                        <asp:BoundField DataField="Lucro" DataFormatString="{0:C}" HeaderText="Lucro" SortExpression="Lucro" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Eval("ImagemUrl") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVendasProd" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetListaVendasProdCount"
                    SelectMethod="GetListaVendasProd" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ProdutoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtRota" Name="codRota" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdGrupo" Name="idsGrupos" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdSubgrupo" Name="idsSubgrupo" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descrProd" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="chkMateriaPrima" Name="incluirMateriaPrima" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="ctrlDataIniSit" Name="dtIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimSit" Name="dtFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniPed" Name="dtIniPed" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimPed" Name="dtFimPed" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniEnt" Name="dtIniEnt" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimEnt" Name="dtFimEnt" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdSituacaoProd" Name="situacaoProd" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdTipoVenda" Name="tipoVendaPedido" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpVendedorCliente" Name="idFuncCliente" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpVendedorPedido" Name="idFuncPedido" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFastDelivery" Name="tipoFastDelivery" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpDesconto" Name="tipoDesconto" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkAgruparCli" Name="agruparCliente" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="chkAgruparPedido" Name="agruparPedido" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="drpOrdenacao" Name="ordenacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkAgruparLiberacao" Name="agruparLiberacao" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="drpNotaFiscal" Name="buscarNotaFiscal" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtIdLiberacao" Name="idLiberacao" PropertyName="Text"
                            Type="Int32" />
                         <asp:ControlParameter ControlID="chkAmbiente" Name="agruparAmbiente" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="drpFunc" Name="idFuncLiberacao" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="cbLiberarcaoNfe" Name="liberacaoNf" PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="cbdGrupo" Name="idGrupos" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacao" runat="server" SelectMethod="GetSituacaoPedido"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.GrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="false" Name="incluirTodos" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVendedor" runat="server" SelectMethod="GetVendedoresComissao"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoVenda" runat="server" SelectMethod="GetTipoVenda"
                    TypeName="Glass.Data.Helper.DataSources">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="false" Name="incluirVazio" Type="Boolean" />
                        <asp:Parameter DefaultValue="true" Name="paraFiltro" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacaoProd" runat="server"
                    SelectMethod="GetSituacaoProducao" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFunc" runat="server" SelectMethod="GetFuncLiberacao"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="setProduto(); return openRpt(false, false);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true, false); return false;"><img border="0"
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                <br />
                <asp:LinkButton ID="lnkImprimirGrupo" runat="server" OnClientClick="setProduto(); return openRpt(false, true);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir (apenas grupos)</asp:LinkButton>
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
