var recalculando = false;
    
function recalcularOrcamento(idOrcamento, perguntar, nomeImagemLoad, nomeControleBenef, nomeCampoAltura, nomeCampoEspessura, nomeCampoLargura,
    nomeCampoIdProd, nomeCampoQtde, nomeCampoTotM, nomeCampoValorUnit, tipoEntregaNovo, idClienteNovo)
{
    var imagemLoad = nomeImagemLoad != null
        ? document.getElementById(nomeImagemLoad)
        : null;

    return Promise.all([true])
        .then(function() {
            if (recalculando || (perguntar && !confirm("Deseja atualizar o valor dos produtos do orçamento?"))) {
                throw new Error();
            }
            else {
                recalculando = true;

                if (imagemLoad != null)
                    imagemLoad.style.display = "";

                tipoEntregaNovo = tipoEntregaNovo || "";
                idClienteNovo = idClienteNovo || "";
            }
        })
        .then(function () {
            var resposta = RecalcularOrcamento.Recalcular(idOrcamento, tipoEntregaNovo, idClienteNovo).value.split(';');
            if (resposta[0] == "Erro") {
                throw new Error(resposta[1]);
            }
            else {
                return {
                    tipoDesconto: resposta[1],
                    desconto: resposta[2],
                    tipoAcrescimo: resposta[3],
                    acrescimo: resposta[4],
                    idComissionado: resposta[5],
                    percComissao: resposta[6],
                    dadosAmbientes: resposta[7]
                };
            }
        })
        .then(function (resposta) {
            var dadosProdutos = new Array();

            try {
                eval("dadosProdutos = " + RecalcularOrcamento.GetDadosProdutosRecalcular(idOrcamento, idClienteNovo).value);
                resposta.dadosProdutos = dadosProdutos;
                return resposta;
            }
            catch (err) {
                RecalcularOrcamento.FinalizarRecalcular(
                    idOrcamento,
                    resposta.tipoDesconto,
                    resposta.desconto,
                    resposta.tipoAcrescimo,
                    resposta.acrescimo,
                    resposta.idComissionado,
                    resposta.percComissao,
                    resposta.dadosAmbientes
                );

                throw new Error("Falha ao recuperar dados dos produtos ao recalcular. Erro: " + err);
            }
        })
        .then(function (resposta) {
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

            var promises = [];
            var promiseAtualizarBeneficiamento = function (idProd, tipo, servicos) {
                return new Promise(function (resolve, reject) {
                    RecalcularOrcamento.AtualizaBenef(idProd, tipo, servicos, function (resposta_ajax) {
                        var resposta = resposta_ajax.value.split(';');

                        if (resposta[0] == "Erro") {
                            reject(resposta[1]);
                        } else {
                            resolve();
                        }
                    });
                });
            };

            for (var rec = 0; rec < resposta.dadosProdutos.length; rec++) {
                if (campoAltura != null) campoAltura.value = resposta.dadosProdutos[rec].Altura;
                if (campoEspessura != null) campoEspessura.value = resposta.dadosProdutos[rec].Espessura;
                if (campoLargura != null) campoLargura.value = resposta.dadosProdutos[rec].Largura;
                if (campoIdProd != null) campoIdProd.value = resposta.dadosProdutos[rec].CodInterno;
                if (campoQtde != null) campoQtde.value = resposta.dadosProdutos[rec].Quantidade;
                if (campoTotM != null) campoTotM.value = resposta.dadosProdutos[rec].TotalM2;
                if (campoValorUnit != null) campoValorUnit.value = resposta.dadosProdutos[rec].ValorUnitario;

                controleBenef.Limpar();
                controleBenef.CarregarBeneficiamentos(
                    resposta.dadosProdutos[rec].IdProd,
                    resposta.dadosProdutos[rec].Tipo
                );

                var servicos = controleBenef.Servicos().Info;
                promises.push(promiseAtualizarBeneficiamento(
                    resposta.dadosProdutos[rec].IdProd,
                    resposta.dadosProdutos[rec].Tipo,
                    servicos
                ));
            }

            return Promise.all(promises)
                .catch(function (error) {
                    throw new Error(error);
                })
                .then(function () {
                    RecalcularOrcamento.FinalizarRecalcular(
                        idOrcamento,
                        resposta.tipoDesconto,
                        resposta.desconto,
                        resposta.tipoAcrescimo,
                        resposta.acrescimo,
                        resposta.idComissionado,
                        resposta.percComissao,
                        resposta.dadosAmbientes
                    );

                    return true;
                });
        })
        .catch(function (error) {
            if (error && error.message)
                alert(error.message);

            if (imagemLoad != null)
                imagemLoad.style.display = "none";

            recalculando = false;
            return false;
        });
}