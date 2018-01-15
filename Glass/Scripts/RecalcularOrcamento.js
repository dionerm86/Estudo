var recalculando = false;
    
function recalcularOrcamento(idOrcamento, perguntar, nomeImagemLoad, nomeControleBenef, nomeCampoAltura, nomeCampoEspessura, nomeCampoLargura,
    nomeCampoIdProd, nomeCampoQtde, nomeCampoTotM, nomeCampoValorUnit, tipoEntregaNovo, idClienteNovo)
{
    if (recalculando || (perguntar && !confirm("Deseja atualizar o valor dos produtos do orçamento?")))
        return false;
    
    recalculando = true;
    if (nomeImagemLoad != null && document.getElementById(nomeImagemLoad) != null)
        document.getElementById(nomeImagemLoad).style.display = "";

    tipoEntregaNovo = typeof tipoEntregaNovo == "undefined" || tipoEntregaNovo == null ? "" : tipoEntregaNovo;
    idClienteNovo = typeof idClienteNovo == "undefined" || idClienteNovo == null ? "" : idClienteNovo;
    
    var resposta = RecalcularOrcamento.Recalcular(idOrcamento, tipoEntregaNovo, idClienteNovo).value.split(';');
    if (resposta[0] == "Erro")
    {
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
    
    var dadosProdutos = new Array();

    try
    {
        eval("dadosProdutos = " + RecalcularOrcamento.GetDadosProdutosRecalcular(idOrcamento, idClienteNovo).value);
    }
    catch (err)
    {
        RecalcularOrcamento.FinalizarRecalcular(idOrcamento, tipoDesconto, desconto, tipoAcrescimo, acrescimo, idComissionado, percComissao, dadosAmbientes);
        alert("Falha ao recuperar dados dos produtos ao recalcular. Erro: " + err);
        return false;
    }

    var controleBenef = eval(nomeControleBenef);
    
    // Remove o percentual de comissão do cálculo dos beneficiamentos (aplicado ao finalizar)
    try { document.getElementById(controleBenef.PercComissao).value = ""; }
    catch (err) { }
    
    var campoAltura = document.getElementById(nomeCampoAltura);
    var campoEspessura = document.getElementById(nomeCampoEspessura);
    var campoLargura = document.getElementById(nomeCampoLargura);
    var campoIdProd = document.getElementById(nomeCampoIdProd);
    var campoQtde = document.getElementById(nomeCampoQtde);
    var campoTotM = document.getElementById(nomeCampoTotM);
    var campoValorUnit = document.getElementById(nomeCampoValorUnit);

    for (rec = 0; rec < dadosProdutos.length; rec++)
    {
        if (campoAltura != null) campoAltura.value = dadosProdutos[rec].Altura;
        if (campoEspessura != null) campoEspessura.value = dadosProdutos[rec].Espessura;
        if (campoLargura != null) campoLargura.value = dadosProdutos[rec].Largura;
        if (campoIdProd != null) campoIdProd.value = dadosProdutos[rec].CodInterno;
        if (campoQtde != null) campoQtde.value = dadosProdutos[rec].Quantidade;
        if (campoTotM != null) campoTotM.value = dadosProdutos[rec].TotalM2;
        if (campoValorUnit != null) campoValorUnit.value = dadosProdutos[rec].ValorUnitario;

        controleBenef.Limpar();
        controleBenef.CarregarBeneficiamentos(dadosProdutos[rec].IdProd, dadosProdutos[rec].Tipo);
        
        var servicos = controleBenef.Servicos();
        resposta = RecalcularOrcamento.AtualizaBenef(dadosProdutos[rec].IdProd, dadosProdutos[rec].Tipo, servicos.Info).value.split(';');

        if (resposta[0] == "Erro")
        {
            RecalcularOrcamento.FinalizarRecalcular(idOrcamento, tipoDesconto, desconto, tipoAcrescimo, acrescimo, idComissionado, percComissao, dadosAmbientes);
            alert(resposta[1]);
            return false;
        }
    }

    RecalcularOrcamento.FinalizarRecalcular(idOrcamento, tipoDesconto, desconto, tipoAcrescimo, acrescimo, idComissionado, percComissao, dadosAmbientes);
    return true;
}