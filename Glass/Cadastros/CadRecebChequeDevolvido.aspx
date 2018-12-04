<%@ Page Title="Quitar Cheque Devolvido/Aberto/Protestado" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadRecebChequeDevolvido.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRecebChequeDevolvido" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrltextboxfloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Cheque.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src="https://s3.amazonaws.com/cappta.api/js/cappta-checkout.js"></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/cappta-tef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">
    
    var reapresentado = <%= IsQuitarReapresentados().ToString().ToLower() %>;
    var financeiroPagto = <%= IsFinanceiroPagto().ToString().ToLower() %>;

    function setChequeReceb(idCheque, numCheque, titular, banco, agencia, conta, valor, dataVenc, obs, selChequeWin, idCliente) {        
        // Verifica se o cheque já foi adicionado
        var cheques = FindControl("hdfIdsCheque", "input").value.split(',');
        for (i = 0; i < cheques.length; i++) {
            if (idCheque == cheques[i]) {
                selChequeWin.alert("Cheque já incluído.");
                return false;
            }
        }

        if (!financeiroPagto)
        {
            /* Chamado 14759.
             * O id do cliente associado ao primeiro cheque selecionado deve ser setado no campo cliente. */
            var txtNumCli = FindControl("txtNumCli", "input");

            if (txtNumCli != null && txtNumCli.value != undefined && txtNumCli.value == "")
            {
                txtNumCli.value = idCliente;
                txtNumCli.onblur();
            }
        }

        // Adiciona item à tabela
        addItem(new Array(numCheque, titular, banco, agencia, conta, valor, dataVenc),
            new Array('Número', 'Titular', 'Banco', 'Agência', 'Conta', 'Valor', 'Vencimento'), 
            'lstCheque', idCheque, 'hdfIdsCheque', valor, 'lblTotal', 'itemRemovido');

        // Carrega o cliente do primeiro cheque inserido
        var txtNumCli = FindControl("txtNumCli", "input");
        if (txtNumCli != null && txtNumCli.value == "" && idCliente > 0)
        {
            txtNumCli.value = idCliente;
            txtNumCli.onblur();
        }
         
        usarCredito("<%= ctrlFormaPagto1.ClientID %>", "");
        return false;
    }

    function itemRemovido(linha) {        
        usarCredito("<%= ctrlFormaPagto1.ClientID %>", "");
    }

    // Validações realizadas ao receber valor por cheque devolvido
    function onReceber() {
        if (!reapresentado && !validate())
            return false;
        
        FindControl("btnReceber", "input").disabled = true;
        
        var idsCheque = FindControl("hdfIdsCheque", "input").value;
        var controle = <%= ctrlFormaPagto1.ClientID %>;
        var idCliente = controle.ClienteID();
        var dataRecebido = controle.DataRecebimento();
        var juros = controle.Juros();
        var desconto = FindControl("txtDesconto", "input").value;
        
        // Se o cheque não tiver sido selecionado
        if (idsCheque == "" || idsCheque == null || idsCheque == "0") {
            alert("Busque um cheque " + (!reapresentado ? "devolvido" : "reapresentado") + " primeiro");
            FindControl("btnReceber", "input").disabled = false;
            return false;
        }

        if (reapresentado)
        {
            if (idCliente == 0 && !financeiroPagto)
            {
                alert("Selecione o cliente.");
                FindControl("btnReceber", "input").disabled = false;
                return false;
            }
        
            var contasBanco = document.getElementById("contaBancoReap").rows[0].cells[1].getElementsByTagName("select")[0].value;
            if (contasBanco == "" && financeiroPagto)
            {
                alert("Selecione a conta bancária.");
                FindControl("btnReceber", "input").disabled = false;
                return false;
            }
        }
        else
        {
            var formasPagto = controle.FormasPagamento();
            var tiposCartao = controle.TiposCartao();
            
            // Guarda os cheques proprios ou de terceiros, de acordo com a forma de pagamento, cadastrados/selecionados, separados por |
            var chequesPagto = controle.Cheques();
            var valores = controle.Valores();
            var parcial = controle.RecebimentoParcial();
            var numAut = controle.NumeroConstrucard();
            var parcCredito = controle.ParcelasCartao();
            var isGerarCredito = controle.GerarCredito();
            var creditoUtilizado = controle.CreditoUtilizado();
            var contasBanco = controle.ContasBanco();
            var depositoNaoIdentificado = controle.DepositosNaoIdentificados();
            var numAutCartao = controle.NumeroAutCartao();
        }
        
        var obs = FindControl("txtObs", "textarea").value;
        FindControl("loadGif", "img").style.visibility = "visible";

        var caixaDiario = <%= Request["caixaDiario"] ?? "false" %>;
        
        if (reapresentado)
        {
            if (idCliente == 0 && !financeiroPagto)
            {
                alert("Selecione o cliente.");
                FindControl("btnReceber", "input").disabled = false;
                return false;
            }
        
            var contasBanco = document.getElementById("contaBancoReap").rows[0].cells[1].getElementsByTagName("select")[0].value;
            if (contasBanco == "")
            {
                alert("Selecione a conta bancária.");
                FindControl("btnReceber", "input").disabled = false;
                return false;
            }

            var retorno = CadRecebChequeDevolvido.QuitarReapresentados(idsCheque, idCliente, contasBanco, dataRecebido, juros, desconto, financeiroPagto, obs).value;
        }
        else
        {
            var formasPagto = controle.FormasPagamento();
            var tiposCartao = controle.TiposCartao();
            
            // Guarda os cheques proprios ou de terceiros, de acordo com a forma de pagamento, cadastrados/selecionados, separados por |
            var chequesPagto = controle.Cheques();
            var valores = controle.Valores();
            var parcial = controle.RecebimentoParcial();
            var numAut = controle.NumeroConstrucard();
            var parcCredito = controle.ParcelasCartao();
            var isGerarCredito = controle.GerarCredito();
            var creditoUtilizado = controle.CreditoUtilizado();
            var contasBanco = controle.ContasBanco();
            var depositoNaoIdentificado = controle.DepositosNaoIdentificados();
            var numAutCartao = controle.NumeroAutCartao();
            var idFormaPgtoCartao = <%= (int)Glass.Data.Model.Pagto.FormaPagto.Cartao %>;
            var utilizarTefCappta = <%= Glass.Configuracoes.FinanceiroConfig.UtilizarTefCappta.ToString().ToLower() %>;
            var tipoCartaoCredito = <%= (int)Glass.Data.Model.TipoCartaoEnum.Credito %>;
            var tipoRecebimento = <%= (int)Glass.Data.Helper.UtilsFinanceiro.TipoReceb.ChequeDevolvido %>;
            var receberCappta = utilizarTefCappta && formasPagto.split(';').indexOf(idFormaPgtoCartao.toString()) > -1;

            var CNI = controle.CartoesNaoIdentificados();
            var retorno = CadRecebChequeDevolvido.Receber(idsCheque, dataRecebido, formasPagto, valores, tiposCartao, contasBanco, depositoNaoIdentificado, CNI, juros, numAut, parcial, parcCredito,
                chequesPagto, isGerarCredito, creditoUtilizado, idCliente, desconto, financeiroPagto, obs, caixaDiario, numAutCartao, receberCappta.toString().toLowerCase()).value;
        }

        if (retorno == null) {
            alert("Falha ao receber cheque. AJAX Error.");
            return false;
        }

        retorno = retorno.split('\t');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            FindControl("loadGif", "img").style.visibility = "hidden";
            FindControl("btnReceber", "input").disabled = false;
            return false;
        }

        //Se utilizar o TEF CAPPTA e tiver selecionado pagamento com cartão à vista
        if (receberCappta) {

            //Busca os dados para autenticar na cappta
            var dadosAutenticacaoCappta = MetodosAjax.ObterDadosAutenticacaoCappta();

            if(dadosAutenticacaoCappta.error) {
                desbloquearPagina(true);
                alert(dadosAutenticacaoCappta.error.description);
                return false;
            }

            //Instancia do canal de recebimento
            CapptaTef.init(dadosAutenticacaoCappta.value, (sucesso, msgErro, codigosAdministrativos, msgRetorno) => callbackCappta(sucesso, msgErro, codigosAdministrativos, retorno));

            //Inicia o recebimento
            CapptaTef.efetuarRecebimento(retorno[1], tipoRecebimento, idFormaPgtoCartao, tipoCartaoCredito, formasPagto, tiposCartao, valores, parcCredito);

            return false;
        }
            

        alert("Valor recebido.");
        // Abre a impressão do acerto assim que o mesmo for recebido.
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=AcertoCheque&idAcertoCheque=" + retorno[1]);
        redirectUrl(window.location.href);
        
    }

    //Método chamado ao realizar o pagamento atraves do TEF CAPPTA
    function callbackCappta(sucesso, msgErro, codigosAdministrativos, retorno) {

        desbloquearPagina(true);

        if(!sucesso) {
            FindControl("loadGif", "img").style.visibility = "hidden";
            FindControl("btnReceber", "input").disabled = false;
            alert(msgErro);
            return false;
        }

        alert("Valor recebido.");
        // Abre a impressão do acerto assim que o mesmo for recebido.
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=AcertoCheque&idAcertoCheque=" + retorno[1]);
        openWindow(600, 800, "../Relatorios/Relbase.aspx?rel=ComprovanteTef&codControle=" + codigosAdministrativos.join(';'));
        redirectUrl(window.location.href);
    }

    // Abre popup para cadastrar cheques
    function queryStringCheques() {
        return "?origem=6";
    }
    
    function reapresentar()
    {
        if (!reapresentado)
            return;
        
        var nomeControle = "<%= ctrlFormaPagto1.ClientID %>";
        document.getElementById(nomeControle + "_tblFormaPagto").style.display = "none";
        document.getElementById("contaBancoReap").rows[0].cells[0].innerHTML = document.getElementById(nomeControle + "_tblFormaPagto_Pagto1_Conta_Titulo").innerHTML;
        document.getElementById("contaBancoReap").rows[0].cells[1].innerHTML = document.getElementById(nomeControle + "_tblFormaPagto_Pagto1_Conta_Controles").innerHTML;
        document.getElementById("contaBancoReap").style.display = financeiroPagto == true ? "" : "none";
    }

    function getUrlCheques(tipoPagto, urlPadrao)
    {
        return !financeiroPagto ? "CadCheque.aspx" : tipoPagto == "2" ? "CadChequePagto.aspx" : "CadChequePagtoTerc.aspx";
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:Button ID="btnBuscarCheques" runat="server" Text="Buscar cheques" OnClientClick="return openWindow(600, 850, '../Utils/SelCheque.aspx?tipo=5&situacao=10&caixaDiario=false'); return false;" />
                <br />
                <br />
                <table id="lstCheque" align="left" cellpadding="4" cellspacing="0" width="100%">
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
                            Desconto R$
                        </td>
                        <td>
                            <asp:TextBox ID="txtDesconto" runat="server" Width="60px"
                                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <div id="divReceber" runat="server">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblTotal" runat="server" Font-Size="Large" Text="R$ 0,00" Style="display: none"></asp:Label>
                                <uc2:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" ExibirCliente="true"
                                    OnLoad="ctrlFormaPagto1_Load" ExibirValorAPagar="true" ExibirComissaoComissionado="false"
                                    FuncaoQueryStringCheques="queryStringCheques"
                                    FuncaoUrlCheques="getUrlCheques" />
                                <table id="contaBancoReap" style="display: none">
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="4" align="center">
                <table id="observacao" style="margin: 8px; <%= IsQuitarReapresentados() ? "display: none" : "" %>">
                    <tr>
                        <td>
                            Obs.
                        </td>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server" Rows="3" TextMode="MultiLine" 
                                Width="350px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <asp:Button ID="btnReceber" runat="server" OnClientClick="return onReceber();" Text="Receber" />
                <img id="loadGif" src="../Images/load.gif" border="0px" title="Aguarde..." width="20px"
                    height="20px" style="visibility: hidden;" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="hdfIdsCheque" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForQuitarChequeDev"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco1" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco2" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
