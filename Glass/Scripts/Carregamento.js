
function efetuaLeitura() {

    //Busca o carregamento e a etiqueta que esta sendo feita a leitura
    var idCarregamento = FindControl("txtCodCarregamento", "input").value;
    var etiqueta = FindControl("txtCodEtiqueta", "input").value;
    var txtNumCli = FindControl("txtNumCli", "input").value;
    var txtNome = FindControl("txtNome", "input").value;

    var codOc = "";
    if (FindControl("txtIdOc", "input") != null)
        codOc = FindControl("txtIdOc", "input").value;

    var codPedido = "";
    if (FindControl("txtIdPedido", "input") != null)
        codPedido = FindControl("txtIdPedido", "input").value;

    var altura = "";
    if (FindControl("txtAltura", "input") != null)
        altura = FindControl("txtAltura", "input").value;

    var largura = "";
    if (FindControl("txtLargura", "input") != null)
        largura = FindControl("txtLargura", "input").value;

    var etqFiltro = "";
    if (FindControl("txtEtqFiltro", "input") != null)
        etqFiltro = FindControl("txtEtqFiltro", "input").value;

    var idClienteExterno = "";
    if (FindControl("txtNumCliExterno", "input") != null)
        idClienteExterno = FindControl("txtNumCliExterno", "input").value;

    var nomeClienteExterno = "";
    if (FindControl("txtNomeClienteExterno", "input") != null)
        nomeClienteExterno = FindControl("txtNomeClienteExterno", "input").value;

    var idPedidoExterno = "";
    if (FindControl("txtIdPedidoExterno", "input") != null)
        idPedidoExterno = FindControl("txtIdPedidoExterno", "input").value;


    //Verifica se informou a etiqueta
    if (etiqueta == "") {
        //alertaPadrao("Erro ao efetuar leitura", "Infome o número da etiqueta.", 'erro', 280, 600);
        return false;
    }

    etiqueta = corrigeLeituraEtiqueta(etiqueta);

    //Verifica se a etiqueta e de revenda(box)
    var retEtqRevenda = CadLeituraCarregamento.IsEtiquetaRevenda(etiqueta);

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

        var idPedRev = CadLeituraCarregamento.ObterIdPedidoRevenda(etiqueta);

        if (idPedRev > 0)
            idPedidoExp = idPedRev;

        if (idPedidoExp == null || idPedidoExp == "")
            idPedidoExp = prompt("Entre com pedido para vinculo", "Cód Pedido");
    }

    var idFunc = FindControl("hdfFunc", "input").value;

    //Efetua a leitura da etiqueta
    var retLeitura = CadLeituraCarregamento.EfetuaLeitura(idFunc, idCarregamento, etiqueta, idPedidoExp,
        txtNumCli, txtNome, codOc, codPedido, altura, largura, etqFiltro, idClienteExterno, nomeClienteExterno, idPedidoExterno);

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
    else
        AtualizaPagina();

    return false;
}

function carregaCarregamento() {

    var idCarregamento = FindControl("txtCodCarregamento", "input").value;

    if (idCarregamento == "") {
        FindControl("txtCodCarregamento", "input").focus();
        return false;
    }

    var retorno = CadLeituraCarregamento.CarregamentoExiste(idCarregamento).value.split(";");

    if (retorno[0] == "Erro") {
        alertaPadrao("Erro ao carregar", retorno[1], 'erro', 280, 600);
        FindControl("txtCodCarregamento", "input").value = "";
        FindControl("txtCodCarregamento", "input").focus();

        document.getElementById("sndError").play();
        return false;
    }

    document.getElementById("sndOk").play();
    setTimeout(function () { AtualizaPagina(); }, 400);

    return false;
}

function AtualizaPagina() {
    cOnClick("imgPesq", "input");
}

function getCli(idCli) {
    if (idCli.value == "")
        return;

    var retorno = CadLeituraCarregamento.GetCli(idCli.value).value.split(';');

    if (retorno[0] == "Erro") {
        alert(retorno[1]);
        idCli.value = "";
        FindControl("txtNome", "input").value = "";
        return false;
    }

    FindControl("txtNome", "input").value = retorno[1];
}

function exibirEstornos(idItemCarregamento) {
    openWindow(170, 430, '../../Utils/ShowLogEstornoItemCarregamento.aspx?popup=true&idItemCarregamento=' + idItemCarregamento);
}

function showConsultaProducao(repeating) {
    var iFrame = document.getElementById("frameModuloSistema");

    if (!repeating) {
        document.getElementById("divModuloSistema").style.display = "inline";
        document.getElementById("boxCarregamento").style.display = "none";

        iFrame.style.display = "inline";

        iFrame.contentWindow.location = "../Producao/LstProducao.aspx?producao=1&popup=true";
        iFrame.style.width = "100%";
        iFrame.style.height = "100%";
    }

    // Reajusta tamanho do iframe
    if (iFrame.contentDocument.body != null) {
        //var boxModuloSistema = document.getElementById("boxModuloSistema");
        //iFrame.style.height = boxModuloSistema.contentDocument.body.scrollHeight + "px";
        //iFrame.style.width = boxModuloSistema.contentDocument.documentElement.scrollWidth + "px";
    }

    //if (iFrame.contentDocument.body == null || iFrame.contentDocument.body.scrollHeight <= 150)
    //setTimeout(function() { showConsultaProducao(true); }, 500);

    return false;
}

function showCarregamento() {
    window.location.href = window.location.href;
}

function exibirPedidosSemCarregamento() {

    var idCarregamento = FindControl("txtCodCarregamento", "input").value;

    if (idCarregamento === 0){
        alert('Informe um carregamento.');
        return false;
    }

    openWindow(600, 800, "../../Relatorios/RelBase.aspx?rel=PedidoProntoSemCarregamento&idCarregamento=" + idCarregamento);

    return false;

}

