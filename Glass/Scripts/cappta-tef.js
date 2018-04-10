var CapptaTef = new function () {

    //Url do handler de processamento do retorno do recebimento
    this.BASE_URL_RETORNO = "../Handlers/RetornoRecebimentoCappta.ashx";

    this.idReferencia = 0;
    this.tipoRecebimento = 0;
    this.idFormaPgtoCartao = 0;
    this.tipoCartaoCredito = 0;
    this.formasPagto = [];
    this.tiposCartao = [];
    this.valores = [];
    this.parcelasCartao = [];
    this.estorno = false;
    this.pagtoIndex = 0;
    this.numPagtosCartao = 0;
    this.multiplePayments = false;
    this.multiplePaymentsError = false;
    this.responses = [];
    this.erros = [];
    this.administrativePassword = 0;
    this.administrativeCode = 0;
    this.processandoCappta = false;
    this.recebeuCappta = false;
    this.chamouRetorno = false;

    //Inicia o canal de recebimento
    this.init = function (authRequest, callback) {
        //Dados de autenticação
        this.authRequest = JSON.parse(authRequest);

        //Metodo que ira ser chamado ao final do processamento
        this.callback = callback;
    }

    //Caso tente fechar a janela verificar se esta em recebimento e avisa o usuario para nao fechar
    window.addEventListener('beforeunload', (event) => {

        if (!CapptaTef.processandoCappta) {
            return;
        }

        var confirmationMessage = "O pagamento esta sendo processado, deseja realmente sair?";

        if (event) {
            event.preventDefault();
            event.returnValue = confirmationMessage;
        }

        return confirmationMessage;
    });

    //Caso feche a janela antes de receber na cappta
    //Faz o cancelamento do recebimento
    window.addEventListener('unload', (event) => {

        //Se for estorno ou nao estiver processando não faz nada
        if (!CapptaTef.processandoCappta || CapptaTef.estorno) {
            return;
        }

        //Se recebeu na cappta porem não chamou o handler para finalizar no webglass 
        //Chama o handler novamente para fazer a finalização no webglass
        if (CapptaTef.recebeuCappta && !CapptaTef.chamouRetorno) {
            CapptaTef.onComplete(true);
            return;
        }

        //Se fechou a janela antes de receber na cappta faz o cancelamento no webglass
        if (!CapptaTef.recebeuCappta) {
            CapptaTef.onComplete(false, "A janela foi fechada.");
            return;
        }

    });

    //Callback para autenticação bem sucedida
    this.onAuthenticationSuccess = function (response) {

        //Se a autenticação não for bem sucedida dispara o erro.
        if (!response.authenticated) {
            this.onComplete(false, "Falha ao autenticar, ocorreram problemas na comunicação com pinpad");
            return;
        }

        //Seta a chave de autenticação
        this.merchantCheckoutGuid = response.merchantCheckoutGuid;

        !this.estorno ? this.processPayments() : this.paymentReversal();
    }

    //Callback para notificação de transações pendentes
    this.handlePendingPayments = function (response) {
        this.multiplePaymentsError = true;
        this.checkout.undoPayments();
    };

    //Ao finalizar o processamento
    this.onComplete = function (sucesso, msg) {

        if (sucesso) {
            this.recebeuCappta = true;
        }

        bloquearPagina();

        var parametros = {
            sucesso: sucesso,
            mensagemErro: msg,
            estorno: this.estorno,
            idReferencia: this.idReferencia,
            tipoRecebimento: this.tipoRecebimento,
            checkoutGuid: this.merchantCheckoutGuid,
            responses: this.responses
        };

        var response = {
            respostaRecebimento: JSON.stringify(parametros)
        };

        $.post(this.BASE_URL_RETORNO, response, (data, success) => {
            this.processandoCappta = false;
            this.callback(data.sucesso, data.mensagemErro, data.codigosAdministrativos, data.mensagemRetorno);
        });

        this.chamouRetorno = true;
    }


    // ------------------------ Recebimento ------------------------


    //Método para efetuar o recebimento
    this.efetuarRecebimento = function (idReferencia, tipoRecebimento, idFormaPgtoCartao, tipoCartaoCredito, formasPagto, tiposCartao, valores, parcelasCartao) {

        this.processandoCappta = true;
        this.recebeuCappta = false;
        this.chamouRetorno = false;

        this.idReferencia = idReferencia;
        this.tipoRecebimento = tipoRecebimento;
        this.idFormaPgtoCartao = idFormaPgtoCartao;
        this.tipoCartaoCredito = tipoCartaoCredito;
        this.formasPagto = formasPagto.split(';');
        this.tiposCartao = tiposCartao.split(';');
        this.valores = valores.split(';');
        this.parcelasCartao = parcelasCartao.split(';');

        this.estorno = false;

        //inicializa o checkout cappta
        this.checkout = CapptaCheckout.authenticate(this.authRequest,
            (response) => this.onAuthenticationSuccess(response),
            (error) => this.onComplete(false, "Falha ao autenticar pinpad CAPPTA: " + error.reason),
            (response) => this.handlePendingPayments(response));

        desbloquearPagina(true);

    }

    //Realiza o recebimentos no TEF
    this.processPayments = function () {

        //Percorre os recebimentos verificando quantos sao cartão
        for (var i = 0; i < this.formasPagto.length; i++) {
            if (this.formasPagto[i] == this.idFormaPgtoCartao) {
                this.numPagtosCartao++;
            }
        }

        //Verifica se algum dos recebimentos é do tipo cartão
        if (this.numPagtosCartao == 0) {
            this.onComplete(false, 'Nenhuma forma de pagto. é do tipo cartão.');
            return;
        }

        //Verifica se é mais de um cartão para iniciar o pagamento com multiplos catões
        if (this.numPagtosCartao > 1) {
            this.multiplePayments = true;
            this.multiplePaymentsError = false;
            this.checkout.startMultiplePayments(this.numPagtosCartao, () => this.onMultiplePaymentsCompleted());
        }

        //Realiza os pagamentos
        this.makePayment();
    }

    //Efetua o pagto
    this.makePayment = function () {

        if (this.pagtoIndex >= this.formasPagto.length) {
            return;
        }

        //Verifica se a forma de pagto atual é cartão, se não for vai pra proxima
        if (this.formasPagto[this.pagtoIndex] != this.idFormaPgtoCartao) {
            this.pagtoIndex++;
            this.makePayment();
            return;
        }

        //Busca o tipo do cartão (Crédito ou Débito)
        var tipoCartaoRet = MetodosAjax.ObterTipoCartao(this.tiposCartao[this.pagtoIndex]);

        //Verifica se houve erro
        if (tipoCartaoRet.error) {
            this.onComplete(false, "Falha ao buscar o tipo do cartão: " + tipoCartaoRet.error.description);
            return;
        }

        //Valor a ser pago
        var valor = Number((this.valores[this.pagtoIndex]).replace(',', '.'));

        //Recebimento com crédito
        if (tipoCartaoRet.value == this.tipoCartaoCredito) {
            this.creditPayment(valor, Number(this.parcelasCartao[this.pagtoIndex]));
        } else { //Recebimento com débito
            this.debitPayment(valor);
        }

        //Seta o proximo pagto.
        this.pagtoIndex++;
    }

    //Pagamento com crédito
    this.creditPayment = function (valor, numParcelas) {

        var request = {
            amount: valor,
            installments: numParcelas,
            installmentType: 2
        };

        this.checkout.creditPayment(request, (response) => this.onPaymentSuccess(response), (error) => this.onPaymentError(error));
    }

    //Pagamento com débito
    this.debitPayment = function (valor) {

        var request = {
            amount: valor
        };

        this.checkout.debitPayment(request, (response) => this.onPaymentSuccess(response), (error) => this.onPaymentError(error));
    }

    //Quando um recebimento ocorrer com sucesso
    this.onPaymentSuccess = function (response) {

        //Informa a posição do recebimento
        response.pagtoIndex = this.pagtoIndex - 1;

        //Salva a resposta
        this.responses.push(response);

        //Se for multiplos pagamentos e ainda houver algum para ser feito chama o método de pagamento
        if (this.multiplePayments && this.pagtoIndex < this.formasPagto.length) {
            this.makePayment();
            return;
        }

        if (this.multiplePayments) {
            return;
        }

        this.onComplete(true);
    }

    //Quando ocorrer algum problema com um recebimentos
    this.onPaymentError = function (response) {

        //Se for multiplos pagamentos cancela toda a operação
        if (this.multiplePayments) {
            this.multiplePaymentsError = true;
            this.erros.push(response.reason);
            this.checkout.undoPayments();
            return;
        }

        this.onComplete(false, response.reason);
    }

    //Ao finalizar um recebimento com multiplos cartões
    this.onMultiplePaymentsCompleted = function () {

        if (this.multiplePaymentsError) {
            this.onComplete(false, this.erros.join('\r\n'));
            return;
        }

        this.onComplete(true);
    }


    // ------------------------ Cancelamento ------------------------


    //Método para efetuar o estorno
    this.efetuarEstorno = function (idReferencia, tipoRecebimento, administrativePassword, administrativeCode) {

        this.processandoCappta = true;
        this.recebeuCappta = false;
        this.chamouRetorno = false;

        this.idReferencia = idReferencia;
        this.tipoRecebimento = tipoRecebimento;
        this.administrativePassword = administrativePassword;
        this.administrativeCode = administrativeCode;
        this.estorno = true;

        //inicializa o checkout cappta
        this.checkout = CapptaCheckout.authenticate(this.authRequest,
            (response) => this.onAuthenticationSuccess(response),
            (error) => this.onComplete(false, "Falha ao autenticar pinpad CAPPTA: " + error.reason),
            (response) => this.handlePendingPayments(response));

        desbloquearPagina(true);
    }

    //Realiza o cancelamento de um pagamento
    this.paymentReversal = function () {

        var request = {
            administrativePassword: this.administrativePassword,
            administrativeCode: this.administrativeCode
        };

        this.checkout.paymentReversal(request, (response) => {
            //Salva a resposta
            this.responses.push(response);
            this.onComplete(true);
        }, (response) => this.onComplete(false, "Falha ao realizar estorno. " + response.reason));
    }

}