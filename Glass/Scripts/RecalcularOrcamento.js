var recalculando = false;

function recalcularOrcamento(idOrcamento, perguntar, nomeImagemLoad, tipoEntregaNovo, idClienteNovo) {
    if (recalculando || (perguntar && !confirm("Deseja atualizar o valor dos produtos do orçamento?"))) {
        return false;
    }

    recalculando = true;

    if (nomeImagemLoad != null && document.getElementById(nomeImagemLoad) != null) {
        document.getElementById(nomeImagemLoad).style.display = "";
    }

    tipoEntregaNovo = typeof tipoEntregaNovo == "undefined" || tipoEntregaNovo == null ? "" : tipoEntregaNovo;
    idClienteNovo = typeof idClienteNovo == "undefined" || idClienteNovo == null ? "" : idClienteNovo;

    var resposta = RecalcularOrcamento.Recalcular(idOrcamento, tipoEntregaNovo, idClienteNovo).value.split(';');

    if (resposta[0] == "Erro") {
        alert(resposta[1]);
        return false;
    }

    var tipoDesconto = resposta[1];
    var desconto = resposta[2];
    var tipoAcrescimo = resposta[3];
    var acrescimo = resposta[4];
    var idComissionado = resposta[5];
    var percComissao = resposta[6];
    var dadosAmbientes = resposta[7];

    try {
        eval("dadosProdutos = " + RecalcularOrcamento.GetDadosProdutosRecalcular(idOrcamento, idClienteNovo).value);
    }
    catch (err) {
        RecalcularOrcamento.FinalizarRecalcular(idOrcamento, tipoDesconto, desconto, tipoAcrescimo, acrescimo, idComissionado, percComissao, dadosAmbientes);
        alert("Falha ao recuperar dados dos produtos ao recalcular. Erro: " + err);
        return false;
    }

    RecalcularOrcamento.FinalizarRecalcular(idOrcamento, tipoDesconto, desconto, tipoAcrescimo, acrescimo, idComissionado, percComissao, dadosAmbientes);
    return true;
}