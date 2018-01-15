<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstPedidos.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstPedidos" Title="Pedidos" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">

        function openRpt(idPedido, isReposicao, tipo) {
            if (!isReposicao)
                openWindow(600, 800, "../Relatorios/RelPedido.aspx?idPedido=" + idPedido + "&tipo=" + tipo);
            else
                openWindow(600, 800, "../Relatorios/RelPedidoRepos.aspx?idPedido=" + idPedido + "&tipo=" + tipo);
            
            return false;
        }

        function openRptLista(idPedido, isReposicao, tipo) {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var codPedCliente = FindControl("txtNumPedCli", "input").value;
            var idCidade = FindControl("hdfCidade", "input").value;
            var endereco = FindControl("txtEndereco", "input").value;
            var bairro = FindControl("txtBairro", "input").value;
            var complemento = FindControl("txtComplemento", "input").value;
            var byVend = '<%= Request["byVend"] %>';
            var byConf = '<%= Request["byConf"] %>';
            var altura = FindControl("txtAltura", "input").value;
            var largura = FindControl("txtLargura", "input").value;
            var diasProntoLib = FindControl("txtDiasProntoLib", "input").value;
            var valorDe = FindControl("txtValorDe", "input") != null ? FindControl("txtValorDe", "input").value : 0;
            var valorAte = FindControl("txtValorAte", "input") != null ? FindControl("txtValorAte", "input").value : 0;
            var dataCadIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dataCadFim = FindControl("ctrlDataCadFim_txtData", "input").value;
            var tipo = FindControl("cblTipoPedido", "select").itens();
            var fastDelivery = FindControl("drpFastDelivery", "select");
            fastDelivery = fastDelivery != null ? fastDelivery.value : "0";
            var tipoVenda = FindControl("drpTipoVenda", "select");
            tipoVenda = tipoVenda != null ? tipoVenda.value : "0";
            var origemPedido = FindControl("drpOrigemPedido", "select").value;
            var obs = FindControl("txtObs", "input").value != null ? FindControl("txtObs", "input").value : "";

            if (idPedido == "") idPedido = 0;
            if (idCliente == "") idCliente = 0;
            if (idCidade == "") idCidade = 0;

            openWindow(600, 700, "../Relatorios/RelBase.aspx?rel=Pedidos&idPedido=" + idPedido + "&idCliente=" + idCliente +
                "&nomeCliente=" + nomeCliente + "&codPedCliente=" + codPedCliente + "&idCidade=" + idCidade + "&endereco=" + endereco +
                "&bairro=" + bairro + "&complemento=" + complemento + "&idLoja=" + idLoja + "&byVend=" + byVend + "&byConf=" + byConf + "&altura=" + altura +
                "&largura=" + largura + "&diasProntoLib=" + diasProntoLib + "&valorDe=" + valorDe + "&valorAte=" + valorAte +
                "&dataCadIni=" + dataCadIni + "&dataCadFim=" + dataCadFim + "&tipo=" + tipo + "&fastDelivery=" + fastDelivery +
                "&tipoVenda=" + tipoVenda + "&origemPedido=" + origemPedido + "&obs=" + obs);
                
            return false;
        }

        function openGraficoTotaisDiarios() {
           
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var codCliente = FindControl("txtNumPedCli", "input").value;
            var idCidade = FindControl("hdfCidade", "input").value;
            var endereco = FindControl("txtEndereco", "input").value;
            var bairro = FindControl("txtBairro", "input").value;
            var complemento = FindControl("txtComplemento", "input").value;
            var byVend = '';
            var byConf = '';
            var altura = FindControl("txtAltura", "input").value;
            var largura = FindControl("txtLargura", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var situacaoProd = FindControl("drpSituacaoProd", "select").value;
            var idOrcamento = FindControl("txtNumOrca", "input").value;
            var maoObra = '';
            var maoObraEspecial = '';
            var producao = '';
            var diasProntoLib = FindControl("txtDiasProntoLib", "input").value;
            var valorDe = FindControl("txtValorDe", "input") != null ? FindControl("txtValorDe", "input").value : 0;
            var valorAte = FindControl("txtValorAte", "input") != null ? FindControl("txtValorAte", "input").value : 0;
            var dataCadIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dataCadFim = FindControl("ctrlDataCadFim_txtData", "input").value;
            var dataFinIni = FindControl("ctrlDataFinIni_txtData", "input").value;
            var dataFinFim = FindControl("ctrlDataFinFim_txtData", "input").value;
            var funcFinalizacao = FindControl("cblFuncFinalizacao", "select").itens();
            var tipo = FindControl("cblTipoPedido", "select").itens();
            var fastDelivery = FindControl("drpFastDelivery", "select");
            fastDelivery = fastDelivery != null ? fastDelivery.value : "0";
            var origemPedido = FindControl("drpOrigemPedido", "select").value;
            var obs = FindControl("txtObs", "input").value != null ? FindControl("txtObs", "input").value : "";

            if (idPedido == "") idPedido = 0;
            if (idCli == "") idCli = 0;
            if (idLoja == "") idLoja = 0;
            if (idOrcamento == "") idOrcamento = 0;
            if (idCidade == "") idCidade = 0;
            if (largura == "") largura = 0;
            if (altura == "") altura = 0;
            if (diasProntoLib == "") diasProntoLib = 0;
            if (valorDe == "") valorDe = 0.0;
            if (valorAte == "") valorAte = 0.0;

            openWindow(600, 800, "../Relatorios/GraficoTotaisDiariosPedido.aspx?idPedido=" + idPedido + "&idLoja=" + idLoja + "&idCli=" + idCli +
                "&nomeCli=" + nomeCli + "&codCliente=" + codCliente + "&idCidade=" + idCidade + "&endereco=" + endereco +
                "&bairro=" + bairro + "&complemento=" + complemento + "&byVend=" + byVend + "&byConf=" + byConf + "&altura=" + altura + "&largura=" + largura +
                "&situacao=" + situacao + "&situacaoProd=" + situacaoProd + "&idOrcamento=" + idOrcamento + "&maoObra=" + maoObra +
                "&maoObraEspecial=" + maoObraEspecial + "&producao=" + producao + "&diasProntoLib=" + diasProntoLib +
                "&valorDe=" + valorDe + "&valorAte=" + valorAte + "&dataCadIni=" + dataCadIni + "&dataCadFim=" + dataCadFim +
                "&dataFinIni=" + dataFinIni + "&dataFinFim=" + dataFinFim + "&funcFinalizacao=" + funcFinalizacao + "&tipo=" + tipo +
                "&fastDelivery=" + fastDelivery + "&origemPedido=" + origemPedido + "&obs=" + obs);
          
            return false;
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = LstPedidos.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function openMotivoCanc(idPedido) {
            openWindow(350, 600, "../Utils/SetMotivoCanc.aspx?idPedido=" + idPedido);
            return false;
        }

        function openRptProj(idPedido, pcp) {
            openWindow(600, 800, "../Cadastros/Projeto/ImprimirProjeto.aspx?idPedido=" + idPedido + (pcp ? "&pcp=1" : ""));
            return false;
        }

        function openRptProm(idPedido)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=NotaPromissoria&idPedido=" + idPedido);
        }
        
        function openRptLiberar(idPedido)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ProdutosLiberar&idPedido=" + idPedido);
        }

        function openListaTotal() {

            var idPedido = FindControl("txtNumPedido", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var codCliente = FindControl("txtNumPedCli", "input").value;
            var idCidade = FindControl("hdfCidade", "input").value;
            var endereco = FindControl("txtEndereco", "input").value;
            var bairro = FindControl("txtBairro", "input").value;
            var complemento = FindControl("txtComplemento", "input").value;
            var byVend = '<%= Request["byVend"] %>';
            var byConf = '<%= Request["byConf"] %>';
            var altura = FindControl("txtAltura", "input").value;
            var largura = FindControl("txtLargura", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var situacaoProd = FindControl("drpSituacaoProd", "select").value;
            var idOrcamento = FindControl("txtNumOrca", "input").value;
            var maoObra = '<%= Request["maoObra"] %>';
            var maoObraEspecial = '<%= Request["maoObraEspecial"] %>';
            var producao = '<%= Request["producao"] %>';
            var diasProntoLib = FindControl("txtDiasProntoLib", "input").value;
            var valorDe = FindControl("txtValorDe", "input") != null ? FindControl("txtValorDe", "input").value : 0;
            var valorAte = FindControl("txtValorAte", "input") != null ? FindControl("txtValorAte", "input").value : 0;
            var dataCadIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dataCadFim = FindControl("ctrlDataCadFim_txtData", "input").value;
            var dataFinIni = FindControl("ctrlDataFinIni_txtData", "input").value;
            var dataFinFim = FindControl("ctrlDataFinFim_txtData", "input").value;
            var funcFinalizacao = FindControl("cblFuncFinalizacao", "select").itens();
            var tipo = FindControl("cblTipoPedido", "select").itens();
            var fastDelivery = FindControl("drpFastDelivery", "select");
            fastDelivery = fastDelivery != null ? fastDelivery.value : "0";
            var tipoVenda = FindControl("drpTipoVenda", "select");
            tipoVenda = tipoVenda != null ? tipoVenda.value : "0";
            var origemPedido = FindControl("drpOrigemPedido", "select").value;
            var obs = FindControl("txtObs", "input").value != null ? FindControl("txtObs", "input").value : "";

            if (idPedido == "") idPedido = 0;
            if (idCli == "") idCli = 0;
            if (idLoja == "") idLoja = 0;
            if (idOrcamento == "") idOrcamento = 0;
            if (idCidade == "") idCidade = 0;
            if (largura == "") largura = 0;
            if (altura == "") altura = 0;
            if (diasProntoLib == "") diasProntoLib = 0;
            if (valorDe == "") valorDe = 0.0;
            if (valorAte == "") valorAte = 0.0;

            openWindow(140, 320, "../Utils/ListaTotalPedido.aspx?idPedido=" + idPedido + "&idLoja=" + idLoja + "&idCli=" + idCli +
                "&nomeCli=" + nomeCli + "&codCliente=" + codCliente + "&idCidade=" + idCidade + "&endereco=" + endereco +
                "&bairro=" + bairro + "&complemento=" + complemento + "&byVend=" + byVend + "&byConf=" + byConf + "&altura=" + altura + "&largura=" + largura +
                "&situacao=" + situacao + "&situacaoProd=" + situacaoProd + "&idOrcamento=" + idOrcamento + "&maoObra=" + maoObra +
                "&maoObraEspecial=" + maoObraEspecial + "&producao=" + producao + "&diasProntoLib=" + diasProntoLib +
                "&valorDe=" + valorDe + "&valorAte=" + valorAte + "&dataCadIni=" + dataCadIni + "&dataCadFim=" + dataCadFim +
                "&dataFinIni=" + dataFinIni + "&dataFinFim=" + dataFinFim + "&funcFinalizacao=" + funcFinalizacao + "&tipo=" + tipo +
                "&fastDelivery=" + fastDelivery + "&tipoVenda=" + tipoVenda + "&origemPedido=" + origemPedido + "&obs=" + obs);
                
            return false;
        }

        function setCidade(idCidade, nomeCidade, nomeUf) {
            FindControl('hdfCidade', 'input').value = idCidade;
            FindControl('txtCidade', 'input').value = nomeCidade + " - " + nomeUf;
        }
        
        function limpaCampoCidade() {
            FindControl('hdfCidade', 'input').value = 0;
            FindControl('txtCidade', 'input').value = "";
        }

        function exibirObsPopup(div, botao)
        {
            for (iTip = 0; iTip < 2; iTip++)
            {
                TagToTip(div, FADEIN, 300, COPYCONTENT, false,
                    FIX, [botao, 9 - getTableWidth(div), -25 - getTableHeight(div)]);
            }
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Num. Orçamento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumOrca" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Pedido Cli." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedCli" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblValorDe" runat="server" Text="Valor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorDe" runat="server" Width="60px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblValorAte" runat="server" Text="à" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorAte" runat="server" Width="61px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqValor" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqLoja" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsSituacao"
                                DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Situação Prod." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacaoProd" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsSituacaoProd" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td style="<%= EstiloFiltroPronto() %>">
                            <asp:Label ID="lblTipoPedido" runat="server" ForeColor="#0066FF" Text="Tipo"></asp:Label>
                        </td>
                        <td style="<%= EstiloFiltroPronto() %>">
                            <sync:CheckBoxListDropDown ID="cblTipoPedido" runat="server" Width="110px" CheckAll="False" Title="Selecione o tipo"
                                DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td style="<%= EstiloFiltroPronto() %>">
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
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
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
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
                                <asp:ListItem Value="1">Normal</asp:ListItem>
                                <asp:ListItem Value="2">Ecommerce</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Cidade" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" Width="200px" onkeydown="return false;"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?retUf=1'); return false;" />
                            <asp:HiddenField ID="hdfCidade" runat="server"></asp:HiddenField>
                            <asp:ImageButton ID="imbLimpaCampoCidade" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick="limpaCampoCidade(); return false;">
                            </asp:ImageButton>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Bairro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtBairro" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                MaxLength="50"></asp:TextBox>
                            <asp:LinkButton ID="lnkPesquisar4" runat="server" OnClick="lnkPesquisar_Click"><img border="0" 
                                src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Endereço" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEndereco" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                MaxLength="80"></asp:TextBox>
                            <asp:LinkButton ID="lnkPesquisar3" runat="server" OnClick="lnkPesquisar_Click"><img border="0" 
                                src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="lblComplemento" runat="server" Text="Complemento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtComplemento" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                MaxLength="50"></asp:TextBox>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkPesquisar_Click"><img border="0" 
                                src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Altura Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAltura" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Largura Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLargura" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td style="<%= EstiloFiltroPronto() %>">
                            <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="Diferença dias entre Pedido Pronto e Liberado"></asp:Label>
                        </td>
                        <td style="<%= EstiloFiltroPronto() %>">
                            <asp:TextBox ID="txtDiasProntoLib" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td style="<%= EstiloFiltroPronto() %>">
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Período Cad.:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqValor0" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>                    
                        <td>
                            <asp:Label ID="Label15" runat="server" Text="Período Finalização:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFinIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFinFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td style="<%= EstiloFiltroPronto() %>">
                            <asp:Label ID="lblUsuFin" runat="server" ForeColor="#0066FF" Text="Usuário Fin."></asp:Label>
                        </td>
                        <td style="<%= EstiloFiltroPronto() %>">
                            <sync:CheckBoxListDropDown ID="cblFuncFinalizacao" runat="server" Width="110px" CheckAll="False" Title="Selecione o usuário"
                                DataSourceID="odsFuncFinalizacao" DataTextField="Nome" DataValueField="IdFunc" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbUsuFin" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td style="<%= EstiloFiltroPronto() %>">
                            <asp:Label ID="Label16" runat="server" ForeColor="#0066FF" Text="Tipo Venda"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoVenda" runat="server" AppendDataBoundItems="True" AutoPostBack="true"
                                DataSourceID="odsTipoVenda" DataTextField="Descr" DataValueField="Id">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton12" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblObs" runat="server" ForeColor="#0066FF" Text="Observação:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton13" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click"> Inserir Pedido</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsPedido" DataKeyNames="IdPedido"
                    EmptyDataText="Nenhum pedido encontrado." OnRowCommand="grdPedido_RowCommand"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" OnRowDataBound="grdPedido_RowDataBound">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadPedido.aspx?idPedido=" + Eval("IdPedido") + "&ByVend=" + Request["ByVend"] %>'
                                    Visible='<%# Eval("EditVisible") %>' OnDataBinding="lnkEditar_DataBinding">
                                    <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("ExibirImpressaoPcp") %>'>
                                    <a href="#" onclick='openRpt(&#039;<%# Eval("IdPedido") %>&#039;, <%# Eval("UsarControleReposicao").ToString().ToLower() %>, 2);'>
                                        <img border="0" src="../Images/page_gear.png" title="Pedido PCP" /></a>
                                </asp:PlaceHolder>
                                <asp:ImageButton ID="imbPedido" runat="server" ImageUrl='<%# (bool)Eval("ExibirImpressaoPcp") ? "~/Images/Relatorio_menos.jpg" : "~/Images/Relatorio.gif" %>'
                                    OnClientClick='<%# "openRpt(" + Eval("IdPedido") + ", " + Eval("UsarControleReposicao").ToString().ToLower() + ", 0); return false" %>'
                                    Visible='<%# Eval("ExibirRelatorio") %>' />
                                <asp:ImageButton ID="imbMemoriaCalculo" runat="server" ImageUrl="~/Images/calculator.gif"
                                    OnClientClick='<%# "openRpt(" + Eval("IdPedido") + ", false, 1); return false" %>'
                                    ToolTip="Memória de cálculo" Visible='<%# Eval("ExibirRelatorioCalculo") %>' />
                                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Nota.gif" OnClientClick='<%# "openRptProm(" + Eval("IdPedido") + "); return false" %>'
                                    ToolTip="Nota promissória" Visible='<%# Eval("ExibirNotaPromissoria") %>' />
                                <asp:PlaceHolder ID="pchImprProj" runat="server" Visible='<%# Eval("ExibirImpressaoProjeto") %>'>
                                    <a href="#" onclick="openRptProj('<%# Eval("IdPedido") %>', <%# (UsarImpressaoProjetoPcp() && (bool)Eval("TemAlteracaoPcp")).ToString().ToLower() %>);">
                                        <img border="0" src="../Images/clipboard.gif" title="Projeto" /></a> </asp:PlaceHolder>
                                <asp:PlaceHolder ID="pchAnexos" runat="server">
                                    <a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdPedido") %>&tipo=pedido&#039;); return false;'>
                                    <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>                                
                                
                                <asp:HyperLink ID="lnkSugestao" runat="server" ToolTip="Sugestões" NavigateUrl='<%# "../Listas/LstSugestaoCliente.aspx?idPedido=" + Eval("IdPedido") %>'>
                                    <img border="0" src="../Images/Nota.gif" Visible='<%# SugestoesVisible() %>' /></asp:HyperLink>

                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Eval("CancelarVisible") %>'>
                                    <a href="#" onclick="openMotivoCanc('<%# Eval("IdPedido") %>');return false;">
                                        <img border="0" src="../Images/ExcluirGrid.gif" /></a></asp:PlaceHolder>
                                <asp:ImageButton ID="imgDesconto" runat="server" ImageUrl="~/Images/money_delete.gif"
                                    OnClientClick='<%# "openWindow(600, 800, \"../Utils/DescontoPedido.aspx?idPedido=" + Eval("IdPedido") + "\"); return false" %>'
                                    ToolTip="Alterações" Visible='<%# Eval("DescontoVisible") %>' />
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/imagem.gif"
                                    OnClientClick='<%# "openWindow(600, 800, \"../Utils/SelImagemPeca.aspx?tipo=pedido&idPedido=" + Eval("IdPedido") + "\"); return false" %>'
                                    ToolTip="Exibir imagem das peças" Visible='<%# Eval("TemEspelho") %>' />
                                <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/book_go.png"
                                    OnClientClick='<%# "openRptLiberar(" + Eval("IdPedido") + "); return false" %>'
                                    ToolTip="Itens que ainda faltam liberar" Visible='<%# Eval("ExibirImpressaoItensLiberar") %>' />
                                <asp:HiddenField ID="hdfMaoDeObra" runat="server" Value='<%# (int)Eval("TipoPedido") == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObra %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedidoExibir" HeaderText="Num" SortExpression="IdPedido" />
                        <asp:BoundField DataField="IdProjeto" HeaderText="Proj." SortExpression="IdProjeto" />
                        <asp:BoundField DataField="IdOrcamento" HeaderText="Orça." SortExpression="IdOrcamento" />
                        <asp:BoundField DataField="CodCliente" HeaderText="Pedido Cli." SortExpression="CodCliente" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Total") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Total", "{0:C}") %>' Visible='<%# !(bool)Eval("ExibirTotalEspelho") %>'></asp:Label>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("TotalEspelho", "{0:C}") %>'
                                    Visible='<%# Eval("ExibirTotalEspelho") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrTipoVenda" HeaderText="Pagto" SortExpression="DescrTipoVenda">
                            <ItemStyle Wrap="True" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataPedido" HeaderText="Data" SortExpression="DataPedido" />
                        <asp:TemplateField HeaderText="Finalização" SortExpression="DataFin">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DataFin", "{0:d}") %>'></asp:Label>
                                <asp:Label ID="Label19" runat="server" 
                                    Text='<%# Eval("NomeUsuFin") != null && Eval("NomeUsuFin") != "" ? Eval("NomeUsuFin", "(Func.: {0})") : null %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("DataFin") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DataEntregaExibicao" HeaderText="Entrega" SortExpression="DataEntrega" />
                        <asp:TemplateField HeaderText="Confirm." SortExpression="DataConf">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("DataConf", "{0:d}") %>'></asp:Label>
                                <asp:Label ID="Label20" runat="server" 
                                    Text='<%# Eval("NomeUsuConf") != null && Eval("NomeUsuConf") != "" ? Eval("NomeUsuConf", "(Func.: {0})") : null %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("DataConf") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DataPronto" DataFormatString="{0:d}" HeaderText="Pronto"
                            SortExpression="DataPronto" />
                        <asp:TemplateField HeaderText="Liberação" SortExpression="DataLiberacao">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" 
                                    Text='<%# Bind("DataLiberacao", "{0:d}") %>'></asp:Label>
                                <asp:Label ID="Label21" runat="server" 
                                    Text='<%# Eval("NomeUsuLib") != null && Eval("NomeUsuLib") != "" ? Eval("NomeUsuLib", "(Func.: {0})") : null %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("DataLiberacao") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrSituacaoPedido") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrSituacaoPedido") %>' Visible='<%# Eval("DescrSituacaoPedido") != "Liberado" %>'></asp:Label>
                                            <asp:PlaceHolder ID="linkSit" runat="server" Visible='<%# Eval("DescrSituacaoPedido") == "Liberado" %>'>
                                                <a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdLiberarPedido") %>&tipo=liberacao&#039;); return false;'>
                                                    <asp:Label ID="Label10" runat="server" Text='<%# Bind("DescrSituacaoPedido") %>'></asp:Label></a></asp:PlaceHolder>
                                        </td>
                                        <td style="padding-left: 4px">
                                            <asp:ImageButton ID="imbReabrir" runat="server" Visible='<%# Eval("ExibirReabrir") %>'
                                                CommandArgument='<%# Eval("IdPedido") %>' CommandName="Reabrir" ImageUrl="~/Images/cadeado.gif"
                                                ToolTip="Reabrir pedido" />
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                            <ItemStyle Wrap="True" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação Produção" SortExpression="situacaoProducao">
                            <ItemTemplate>
                                <asp:Label ID="lblSitProd" runat="server" Text='<%# Eval("DescrSituacaoProducao") %>'
                                    OnLoad="lblSitProd_Load"></asp:Label>
                                <asp:LinkButton ID="lnkSitProd" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                    CommandName="Producao" Text='<%# Eval("DescrSituacaoProducao") %>' OnLoad="lblSitProd_Load"></asp:LinkButton>
                                <asp:ImageButton ID="imbPendentePronto" runat="server" ImageUrl='<%# (int)Eval("SituacaoProducao") == 3 ? "~/Images/curtir.gif" : 
                                    (int)Eval("SituacaoProducao") == 2 ? "~/Images/não curtir.gif" : "" %>' Visible='<%# (int)Eval("SituacaoProducao") == 2 || (int)Eval("SituacaoProducao") == 3 %>'
                                    CommandName="Producao" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DescrSituacaoProducao") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescricaoTipoPedido" HeaderText="Tipo" SortExpression="TipoPedido">
                            <ItemStyle Wrap="True" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Liberado p/ Entrega">
                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblLiberarFinanc" runat="server" Text='<%# ((bool)Eval("LiberadoFinanc") ? "Sim" : "Não") %>'></asp:Label>
                                        </td>
                                        <td style="padding-left: 4px">
                                            <asp:LinkButton ID="lnkLiberarFinanc" runat="server" CommandArgument='<%# Eval("IdPedido") + "#true" %>'
                                                CommandName="Liberar" Visible='<%# (!(bool)Eval("LiberadoFinanc"))%>'>Liberar</asp:LinkButton>
                                            <asp:LinkButton ID="lnkDesfazerFinanc" runat="server" CommandArgument='<%# Eval("IdPedido") + "#false" %>'
                                                CommandName="Liberar" Visible='<%# ((bool)Eval("LiberadoFinanc"))%>'>Desfazer</asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="Pedido" IdRegistro='<%# Eval("IdPedido") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Image ID="imgPGSinal" runat="server" ImageUrl="../Images/cifrao.png"
                                    ToolTip='<%# ((bool)Eval("PagamentoAntecipado") == true && Eval("IdSinal") != null ? "Sinal e Pagamento Antecipado" :
                                    (bool)Eval("PagamentoAntecipado") == true ? "Pagamento Antecipado" : Eval("IdSinal") != null ? "Sinal" : "") + " (" + string.Format("{0:c}", Eval("TotalRecebSinalPagtoAntecip")) + ")" %>'
                                    Visible='<%# (bool)Eval("PagamentoAntecipado") == true || Eval("IdSinal") != null ? true : false %>' />
                                <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Nota.gif" onmouseout="UnTip()"
                                    ToolTip="Observação liberação" onmouseover='<%# Eval("IdPedido", "exibirObsPopup(\"obsLiberacao_{0}\", this)") %>'
                                    Visible='<%# !String.IsNullOrEmpty(Eval("ObsLiberacao") as string) %>' OnClientClick="return false" />
                                <div id="obsLiberacao_<%# Eval("IdPedido") %>" style="display: none">
                                    Observação da Liberação: <br />
                                    <asp:Label ID="Label14" runat="server" Text='<%# Eval("ObsLiberacao") %>'></asp:Label>
                                </div>
                                <asp:ImageButton ID="ImageButton10" runat="server" onmouseover='<%# Eval("IdPedido", "exibirObsPopup(\"obsFinFinanc_{0}\", this)") %>' onmouseout="UnTip()"
                                    ImageUrl="~/Images/money_hist.gif" ToolTip="Finalizações financeiro" 
                                    Visible='<%# Eval("ExibirFinalizacoesFinanceiro") %>' OnClientClick="return false" />
                                <div id="obsFinFinanc_<%# Eval("IdPedido") %>" style="display: none">
                                    <asp:HiddenField runat="server" ID="hdfIdPedidoObsFin" Value='<%# Eval("IdPedido") %>' />
                                    <asp:GridView ID="grdObservacaoFinanceiro" runat="server" AllowPaging="True" 
                                        AllowSorting="True" AutoGenerateColumns="False" CssClass="pos" CellPadding="2" CellSpacing="1"
                                        DataSourceID="odsObservacaoFinanceiro" GridLines="None" 
                                        onrowdatabound="grdObservacaoFinanceiro_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="Motivo" HeaderText="Ação" ReadOnly="True" 
                                                SortExpression="Motivo" />
                                            <asp:BoundField DataField="Observacao" HeaderText="Observação" 
                                                SortExpression="Observacao" />
                                            <asp:BoundField DataField="NomeFuncionarioCadastro" HeaderText="Funcionário" 
                                                ReadOnly="True" SortExpression="NomeFuncionarioCadastro" />
                                            <asp:BoundField DataField="DataCadastro" HeaderText="Data Cadastro" 
                                                ReadOnly="True" SortExpression="DataCadastro" />
                                        </Columns>
                                        <PagerStyle CssClass="pgr" />
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsObservacaoFinanceiro" runat="server" 
                                        EnablePaging="True" MaximumRowsParameterName="pageSize" 
                                        SelectCountMethod="ObtemNumeroObservacoesFinanceiro" 
                                        SelectMethod="ObtemObservacoesFinanceiro" SortParameterName="sortExpression" 
                                        StartRowIndexParameterName="startRow" 
                                        TypeName="WebGlass.Business.Pedido.Fluxo.MotivoFinalizacaoFinanceiro" 
                                        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" SkinID="">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="hdfIdPedidoObsFin" Name="idPedido" 
                                                PropertyName="Value" Type="UInt32" />
                                            <asp:Parameter Name="idFuncCad" Type="UInt32" />
                                            <asp:Parameter Name="dataCadIni" Type="String" />
                                            <asp:Parameter Name="dataCadFim" Type="String" />
                                            <asp:Parameter Name="motivo" Type="String" />
                                        </SelectParameters>
                                    </colo:VirtualObjectDataSource>
                                </div>
                                <img src="../Images/carregamento.png" alt='<%# "Ordem de Carga: " + Eval("IdsOCs") %>' title='<%# "Ordem de Carga: " + Eval("IdsOCs") %>'
                                    style='<%# string.IsNullOrEmpty((string)Eval("IdsOCs")) ? "display:none;" : "" %>' Width="16" Height="16" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedido" runat="server" DataObjectTypeName="Glass.Data.Model.Pedido"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.PedidoDAO" 
                    OnDeleted="odsPedido_Deleted" CacheExpirationPolicy="Absolute" 
                    ConflictDetection="OverwriteChanges" SkinID="">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtNumPedCli" Name="codCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="hdfCidade" Name="idCidade" PropertyName="Value"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtEndereco" Name="endereco" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtComplemento" Name="complemento" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSituacaoProd" Name="situacaoProd" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:QueryStringParameter DefaultValue="" Name="byVend" QueryStringField="ByVend"
                            Type="String" />
                        <asp:QueryStringParameter Name="byConf" QueryStringField="ByConf" Type="String" />
                        <asp:QueryStringParameter Name="maoObra" QueryStringField="maoObra" Type="String" />
                        <asp:QueryStringParameter Name="maoObraEspecial" 
                            QueryStringField="maoObraEspecial" Type="String" />
                        <asp:QueryStringParameter Name="producao" QueryStringField="producao" Type="String" />
                        <asp:ControlParameter ControlID="txtNumOrca" Name="idOrcamento" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtAltura" Name="altura" PropertyName="Text" Type="Single" />
                        <asp:ControlParameter ControlID="txtLargura" Name="largura" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtDiasProntoLib" Name="numeroDiasDiferencaProntoLib"
                            PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtValorDe" Name="valorDe" PropertyName="Text" Type="Single" />
                        <asp:ControlParameter ControlID="txtValorAte" Name="valorAte" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFinIni" Name="dataFinIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFinFim" Name="dataFinFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="cblFuncFinalizacao" Name="funcFinalizacao" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cblTipoPedido" Name="tipo" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpFastDelivery" Name="fastDelivery" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpTipoVenda" Name="tipoVenda" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpOrigemPedido" Name="origemPedido" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtObs" Name="obs" PropertyName="Text" />
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
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPedido" runat="server" SelectMethod="GetTipoPedidoFilter"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncFinalizacao" runat="server" SelectMethod="GetFuncFin"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>       
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoVenda" runat="server" SelectMethod="GetTipoVenda"
                    TypeName="Glass.Data.Helper.DataSources">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="true" Name="incluirVazio" Type="Boolean" />
                        <asp:Parameter DefaultValue="true" Name="paraFiltro" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>         
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Image ID="Image1" runat="server" ImageUrl="../Images/Clipe.gif" />
                <asp:LinkButton ID="lnkAnexos" runat="server" OnClientClick="openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=0&tipo=pedido&#039;); return false;"
                    ToolTip="Anexar arquivos à vários pedidos">Anexar arquivos à vários pedidos</asp:LinkButton>
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRptLista();"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;
                <asp:LinkButton ID="lnkTotais" runat="server" OnClientClick="return openListaTotal();"
                    ToolTip="Exibe os valores de preço, peso e m² totais dos pedidos listados."> <img 
                    alt="" border="0" src="../Images/detalhes.gif" /> Total</asp:LinkButton>
                &nbsp;
                <asp:LinkButton ID="lnkGraficoDiario" runat="server" OnClientClick="return openGraficoTotaisDiarios();"
                    ToolTip="Exibe o gráfico com os totais diários dos pedidos"> 
                    <img alt="" border="0" src="../Images/detalhes.gif" /> Gráfico Totais Diários
                </asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
