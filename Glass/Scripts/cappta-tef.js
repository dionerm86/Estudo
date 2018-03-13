//Alerta se a janela for fechado antes da hora
window.addEventListener('beforeunload', function (event) {

    if (this.processamentoConcluido) {
        return;
    }

    var confirmationMessage = "O pagamento esta sendo processado, deseja realmente sair?";

    if (event) {
        event.preventDefault();
        event.returnValue = confirmationMessage;
    }

    return confirmationMessage;
});

//Ao fechar a janela
window.addEventListener('unload', function (event) {

    if (this.processamentoConcluido) {
        return;
    }

    //Se for multiplos pagamentos cancela toda a operação
    if (this.multiplePayments) {
        this.multiplePaymentsError = true;
        checkout.undoPayments();
        return;
    }

    this.callbackErro('A janela foi fechada.');

});

//Mostra as informações sobre o processo de recebimento
function logMessage(msg) {
    $(".status-message").text(msg);
}

//Quando autenticar com sucesso
function onAuthenticationSuccess(isPayment, response) {
    logMessage("Autenticado!");
    this.merchantCheckoutGuid = response.merchantCheckoutGuid;
    console.log(this.merchantCheckoutGuid);

    if (isPayment) {
        processPayments();
    } else {
        paymentReversal();
    }
};

//Quando ocorrer algum erro de altenticação
function onAuthenticationError(error) {
    this.processamentoConcluido = true;
    logMessage("Falha ao autenticar: " + error.reason);
    callbackErro(error.reason);
    close();
};


// ------------------------ Pagamento ------------------------

//Método de inicialização de pagamento
function initPayment(idFormaPgtoCartao, tipoCartaoCredito, formasPagto, tiposCartao, valores, parcelasCartao, callbackSucesso, callbackErro) {

    this.processamentoConcluido = false;

    this.idFormaPgtoCartao = idFormaPgtoCartao;
    this.tipoCartaoCredito = tipoCartaoCredito;
    this.formasPagto = formasPagto.split(';');
    this.tiposCartao = tiposCartao.split(';');
    this.valores = valores.split(';');
    this.parcelasCartao = parcelasCartao.split(';');
    this.callbackSucesso = callbackSucesso;
    this.callbackErro = callbackErro;

    logMessage("Autenticando...");

    var capptaAuth = MetodosAjax.ObterDadosAutenticacaoCappta();

    if (capptaAuth.error != null) {
        logMessage("Falha ao buscar dados para autenticação. " + capptaAuth.error.description);
        callbackErro(capptaAuth.error.description);
        close();
        return;
    }

    var capptaAuthData = capptaAuth.value.split(';');

    //Dados usados para autenticação
    var authenticationRequest = {
        authenticationKey: capptaAuthData[0],
        merchantCnpj: capptaAuthData[1],
        checkoutNumber: capptaAuthData[2]
    };


    //inicializa o checkout cappta
    this.checkout = CapptaCheckout.authenticate(authenticationRequest, function (response) { onAuthenticationSuccess(true, response); }, onAuthenticationError, onPendingPayments);

    setTimeout(function () {

        if (!this.merchantCheckoutGuid) {
            this.processamentoConcluido = true;
            var msg = "Falha ao autenticar, ocorreram problemas na comunicação com pinpad";
            logMessage(msg);
            callbackErro(msg);
            close();
        }

    }, 10000);
}

//Quanto autenticar e tiver pagamentos anteriores pendentes
function onPendingPayments(response) {
    console.log(response);
    this.multiplePaymentsError = true;
    checkout.undoPayments();
};

//Quando um pagamento ocorrer com sucesso
function onPaymentSuccess(response) {

    logMessage("Pagto. efetuado");

    //Salva a resposta
    this.administrativeCode.push(response.administrativeCode);
    this.customerReceipt.push(response.receipt.customerReceipt);
    this.merchantReceipt.push(response.receipt.merchantReceipt);

    //Se for multiplos pagamentos e ainda houver algum para ser feito chama o método de pagamento
    if (this.multiplePayments && this.pagtoIndex < this.formasPagto.length) {
        logMessage("Processando proximo pagto.");
        makePayment();
        return;
    } else if (this.multiplePayments) {
        return;
    }

    this.processamentoConcluido = true;

    this.callbackSucesso(this.merchantCheckoutGuid, this.administrativeCode, this.customerReceipt, this.merchantReceipt);
    close();
}

//Quando ocorrer algum problema com um pagamento
function onPaymentError(response) {

    this.processamentoConcluido = true;

    //Se for multiplos pagamentos cancela toda a operação
    if (this.multiplePayments) {
        this.multiplePaymentsError = true;
        this.erros.push(response.reason);
        checkout.undoPayments();
        return;
    }

    this.callbackErro(response.reason);
    close();
}

//Ao finalizar um recebimento com multiplos cartões
function onMultiplePaymentsCompleted() {

    this.processamentoConcluido = true;
    if (!this.multiplePaymentsError) {
        this.callbackSucesso(this.merchantCheckoutGuid, this.administrativeCode, this.customerReceipt, this.merchantReceipt);
    } else {
        this.callbackErro(this.erros.join('\r\n'));
    }

    close();
}

//Realiza o pagamento no TEF
function processPayments() {

    logMessage("Processando pagamentos...");

    if (this.multiplePaymentsError) {
        logMessage("Existe uma transação aberta, cancele antes de continuar...");
        return;
    }

    this.pagtoIndex = 0;
    this.numPagtosCartao = 0;
    this.administrativeCode = [];
    this.customerReceipt = [];
    this.merchantReceipt = [];
    this.erros = [];

    //Percorre os pagamentos verificando quantos sao cartão
    for (var i = 0; i < this.formasPagto.length; i++) {
        if (this.formasPagto[i] == this.idFormaPgtoCartao) {
            this.numPagtosCartao++;
        }
    }

    //Verifica se é mais de um cartão para iniciar o pagamento com multiplos catões
    if (this.numPagtosCartao == 0) {
        logMessage("Nenhuma forma de pagto. é do tipo cartão.");
        this.callbackErro('Nenhuma forma de pagto. é do tipo cartão.');
        close();
        return;
    } else if (this.numPagtosCartao > 1) {
        this.multiplePayments = true;
        this.multiplePaymentsError = false;
        logMessage("Processando multiplos pagamentos...");
        checkout.startMultiplePayments(this.numPagtosCartao, onMultiplePaymentsCompleted);
    }

    //Realiza os pagamentos
    makePayment();
}

//Efetua o pagto
function makePayment() {

    if (this.pagtoIndex >= this.formasPagto.length) {
        return;
    } else if (this.formasPagto[this.pagtoIndex] != this.idFormaPgtoCartao) { //Verifica se a forma de pagto atual é cartão, se não for vai pra proxima
        this.pagtoIndex++;
        makePayment();
        return;
    }

    //Busca o tipo do cartão (Crédito ou Débito)
    var tipoCartaoRet = MetodosAjax.ObterTipoCartao(this.tiposCartao[this.pagtoIndex]);

    //Verifica se houve erro
    if (tipoCartaoRet.error != null) {
        logMessage("Falha ao buscar o tipo do cartão: " + tipoCartaoRet.error.description);
        callbackErro(tipoCartaoRet.error.description);
        close();
        return;
    }

    //Valor a ser pago
    var valor = Number((this.valores[this.pagtoIndex]).replace(',', '.'));

    //Recebimento com crédito
    if (tipoCartaoRet.value == this.tipoCartaoCredito) {
        creditPayment(valor, Number(this.parcelasCartao[this.pagtoIndex]));
    } else { //Recebimento com débito
        debitPayment(valor);
    }

    //Seta o proximo pagto.
    this.pagtoIndex++;
}

//Pagamento com crédito
function creditPayment(valor, numParcelas) {
    logMessage("Recebendo crédito R$" + valor);
    var request = {
        amount: valor,
        installments: numParcelas,
        installmentType: 2
    };

    checkout.creditPayment(request, onPaymentSuccess, onPaymentError);
}

//Pagamento com débito
function debitPayment(valor) {
    logMessage("Recebendo débito R$" + valor);
    checkout.debitPayment({ amount: valor }, onPaymentSuccess, onPaymentError);
}



// ------------------------ Cancelamento ------------------------

//Método de inicialização de cancelamento
function initReversal(administrativePassword, administrativeCode, callbackSucesso, callbackErro) {

    this.processamentoConcluido = false;

    this.administrativePassword = administrativePassword;
    this.administrativeCode = administrativeCode;
    this.callbackSucesso = callbackSucesso;
    this.callbackErro = callbackErro;

    logMessage("Autenticando...");

    var capptaAuth = MetodosAjax.ObterDadosAutenticacaoCappta();

    if (capptaAuth.error != null) {
        logMessage("Falha ao buscar dados para autenticação. " + capptaAuth.error.description);
        callbackErro(capptaAuth.error.description);
        close();
        return;
    }

    var capptaAuthData = capptaAuth.value.split(';');

    //Dados usados para autenticação
    var authenticationRequest = {
        authenticationKey: capptaAuthData[0],
        merchantCnpj: capptaAuthData[1],
        checkoutNumber: capptaAuthData[2]
    };

    //inicializa o checkout cappta
    this.checkout = CapptaCheckout.authenticate(authenticationRequest, function (response) { onAuthenticationSuccess(false, response); }, onAuthenticationError, onPendingPayments);
}

//Quando um cancelamento ocorrer com sucesso
function onPaymentReversalSuccess(response) {

    this.processamentoConcluido = true;

    this.callbackSucesso(response.administrativeCode, response.receipt.customerReceipt, response.receipt.merchantReceipt);
    close();
}

//Quando ocorrer algum problema com um pagamento
function onPaymentReversalError(response) {

    this.processamentoConcluido = true;

    this.callbackErro(response.reason);
    close();
}

//Realiza o cancelamento de um pagamento
function paymentReversal() {

    var request = {
        administrativePassword: this.administrativePassword,
        administrativeCode: this.administrativeCode
    };

    checkout.paymentReversal(request, onPaymentReversalSuccess, onPaymentReversalError);
}

