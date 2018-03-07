<%@ Page Title="Quitar Cheque Devolvido/Aberto/Protestado" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadRecebChequeDevolvido.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRecebChequeDevolvido" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrltextboxfloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Cheque.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">
    
    var reapresentado = <%= IsQuitarReapresentados().ToString().ToLower() %>;
        var financeiroPagto = <%= IsFinanceiroPagto().ToString().ToLower() %>;
        var recebendoCappta = false;

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
            'lstCheque', idCheque, 'hdfIdsCheque', valor, 'lblTotal');

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
        debugger;
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
            var retorno = CadRecebChequeDevolvido.QuitarReapresentados(idsCheque, idCliente, contasBanco, dataRecebido, juros, desconto, financeiroPagto, obs).value;
        }
        else
        {
            var CNI = controle.CartoesNaoIdentificados();
            var retorno = CadRecebChequeDevolvido.Receber(idsCheque, dataRecebido, formasPagto, valores, tiposCartao, contasBanco, depositoNaoIdentificado, CNI, juros, 
                numAut, parcial, parcCredito, chequesPagto, isGerarCredito, creditoUtilizado, idCliente, desconto, financeiroPagto, obs, caixaDiario, numAutCartao).value;
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
        else {

            if(!reapresentado){

                var idFormaPgtoCartao = <%= (int)Glass.Data.Model.Pagto.FormaPagto.Cartao %>;
                var utilizarTefCappta = <%= Glass.Configuracoes.FinanceiroConfig.UtilizarTefCappta.ToString().ToLower() %>;
                var tipoCartaoCredito = <%= (int)Glass.Data.Model.TipoCartaoEnum.Credito %>;

                //Se utilizar o TEF CAPPTA e tiver selecionado pagamento com cartão à vista
                if (utilizarTefCappta && formasPagto.split(';').indexOf(idFormaPgtoCartao.toString()) > -1) {

                    recebendoCappta = true;

                    //Abre a tela de gerenciamento de pagamento do TEF
                    var recebimentoCapptaTef = openWindowRet(768, 1024, '../Utils/RecebimentoCapptaTef.aspx');

                    //Quando a tela de gerenciamento for carregada, chama o método de inicialização.
                    //Passa os parametros para receber, e os callbacks de sucesso e falha. 
                    recebimentoCapptaTef.onload = function (event) {
                        recebimentoCapptaTef.initPayment(idFormaPgtoCartao, tipoCartaoCredito, formasPagto, tiposCartao, valores, parcCredito, 
                            function (checkoutGuid, administrativeCodes, customerReceipt, merchantReceipt) { callbackCapptaSucesso(checkoutGuid, administrativeCodes, customerReceipt, merchantReceipt, retorno, formasPagto) },
                            function (msg) { callbackCapptaErro(msg, retorno) });
                    }

                    return false;
                }
            }

            alert("Valor recebido.");
            // Abre a impressão do acerto assim que o mesmo for recebido.
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=AcertoCheque&idAcertoCheque=" + retorno[1]);
            redirectUrl(window.location.href);
        }
    }

        //Método chamado ao realizar o pagamento atraves do TEF CAPPTA
        function callbackCapptaSucesso(checkoutGuid, administrativeCodes, customerReceipt, merchantReceipt, retorno, formasPagto) {

            //Atualiza os pagamentos
            var retAtualizaPagamentos = CadRecebChequeDevolvido.AtualizaPagamentos(retorno[1], checkoutGuid, administrativeCodes.join(';'), customerReceipt.join(';'), merchantReceipt.join(';'), formasPagto);

            if(retAtualizaPagamentos.error != null) {
                alert(retAtualizaPagamentos.error.description);
                desbloquearPagina(true);
                return false;
            }

            desbloquearPagina(true);
            recebendoCappta = false;
            alert("Valor recebido.");
            // Abre a impressão do acerto assim que o mesmo for recebido.
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=AcertoCheque&idAcertoCheque=" + retorno[1]);
            openWindow(600, 800, "../Relatorios/Relbase.aspx?rel=ComprovanteTef&codControle=" + administrativeCodes.join(';'));
            redirectUrl(window.location.href);
        }

        //Método chamado caso ocorrer algum erro no recebimento atraves do TEF CAPPTA
        function callbackCapptaErro(msg, retorno) {

            var retCancelar = CadRecebChequeDevolvido.CancelarAcertoChequeErroTef(retorno[1], msg);

            if(retCancelar.error != null) {
                alert(retCancelar.error.description);
            }

            FindControl("loadGif", "img").style.visibility = "hidden";
            FindControl("btnReceber", "input").disabled = false;

            desbloquearPagina(true);
            recebendoCappta = false;
            alert(msg);
        }

        //Alerta se a janela for fechado antes da hora
        window.addEventListener('beforeunload', function (event) {

            if (!recebendoCappta) {
                return;
            }

            var confirmationMessage = "O pagamento esta sendo processado, deseja realmente sair?";

            if (event) {
                event.preventDefault();
                event.returnValue = confirmationMessage;
            }

            return confirmationMessage;
        });

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
        document.getElementById("contaBancoReap").style.display = "";
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
