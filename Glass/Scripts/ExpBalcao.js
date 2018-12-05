function carregaLiberacao() {

    var idLiberacao = FindControl("txtCodLiberacao", "input").value;

    if (idLiberacao == "") {
        FindControl("txtCodLiberacao", "input").focus();
        return false;
    }

    var retorno = CadLeituraExpBalcao.ValidaLiberacao(idLiberacao);

    if (retorno.error != null) {
        alertaPadrao("Falha ao validar liberação", retorno.error.description, 'erro', 280, 600);
        return false;
    }

    document.getElementById("sndOk").play();
    setTimeout(function () { AtualizaPagina(); }, 400);

    return false;
}

function AtualizaPagina() {
    cOnClick("imgPesq", "input");
}

function showConsultaProducao(repeating) {
    var iFrame = document.getElementById("frameModuloSistema");

    if (!repeating) {
        document.getElementById("divModuloSistema").style.display = "inline";
        document.getElementById("boxCarregamento").style.display = "none";

        iFrame.style.display = "inline";

        iFrame.contentWindow.location = "../Producao/LstProducao.aspx?producao=1&popup=true";
    }

    // Reajusta tamanho do iframe
    if (iFrame.contentDocument.body != null && iFrame.contentDocument.body.scrollHeight > 150) {
        iFrame.style.height = (iFrame.contentDocument.body.scrollHeight + 100) + "px";

    }

    if (iFrame.contentDocument.body == null || iFrame.contentDocument.body.scrollHeight <= 150)
        setTimeout(function () { showConsultaProducao(true); }, 500);

    return false;
}

function showCarregamento() {
    window.location.href = window.location.href;
}

function efetuaLeitura() {

    //Busca o carregamento e a etiqueta que esta sendo feita a leitura
    var idLiberacao = FindControl("txtCodLiberacao", "input").value;
    var etiqueta = FindControl("txtCodEtiqueta", "input").value;

    //Verifica se informou a etiqueta
    if (etiqueta == "") {
        return false;
    }

    etiqueta = corrigeLeituraEtiqueta(etiqueta);

    //Verifica se a etiqueta e de revenda(box)
    var retEtqRevenda = CadLeituraExpBalcao.IsEtiquetaRevenda(etiqueta);

    //Id do pedido de expedição
    var idPedidoExp = "";

    //Verifica se houve algum erro
    if (retEtqRevenda.error != null) {
        alertaPadrao("Erro ao ler peça", retEtqRevenda.error.description, 'erro', 280, 600);
        FindControl("txtCodEtiqueta", "input").value = "";
        FindControl("txtCodEtiqueta", "input").focus();

        document.getElementById("sndError").play();
        return false;
    }

    if (retEtqRevenda.value.toLowerCase() == "true") {

        var idPedidoExp = FindControl("txtIdPedido", "input").value;

        var idPedRev = CadLeituraExpBalcao.ObterIdPedidoRevenda(etiqueta);

        if (idPedRev > 0)
            idPedidoExp = idPedRev;

        if (idPedidoExp == null || idPedidoExp == "")
            idPedidoExp = prompt("Entre com pedido para vinculo", "Cód Pedido");
    }

    var idFunc = FindControl("hdfFunc", "input").value;

    //Efetua a leitura da etiqueta
    var retLeitura = CadLeituraExpBalcao.EfetuaLeitura(idFunc, idLiberacao, etiqueta, idPedidoExp);

    if (retLeitura.error != null) {
        alertaPadrao("Erro ao efetuar leitura", retLeitura.error.description, 'erro', 280, 600);
        FindControl("txtCodEtiqueta", "input").value = "";
        FindControl("txtCodEtiqueta", "input").focus();

        document.getElementById("sndError").play();
        return false;
    }

    FindControl("txtCodEtiqueta", "input").value = "";
    FindControl("txtCodEtiqueta", "input").focus();

    document.getElementById("sndOk").play();

    if (FindControl("chkAtuAutomaticamente", "input").checked)
        setTimeout(function () { AtualizaPagina(); }, 400);

    return false;
}
