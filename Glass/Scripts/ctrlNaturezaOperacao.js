/**
 * Classe com os dados para o controle de natureza de operação.
 * @param nomeControle O nome do controle que está sendo criado.
 * @param dadosControle Os dados para funcionamento do controle.
 */
var NaturezaOperacaoType = function(nomeControle, dadosControle) {
    /**
     * @public @instance
     * Indica se o CFOP da natureza de operação é de devolução.
     * @returns boolean Verdadeiro, se o CFOP for de devolução; caso contrário, falso.
     */
    this.IsCfopDevolucao = function() {
        return !!dadosControle.CfopDevolucao;
    };

    /**
     * @public @instance
     * Função executada como callback da seleção.
     * Altera os controles de CST e Perc. Redução BC ICMS, CSOSN, 
     * CST IPI e CST Pis/Cofins colocando o valor configurado na 
     * natureza de operação selecionada (se houver).
     * @param controle O nome do controle que disparou o método.
     * @param id O código da natureza de operação selecionada.
     */
    this.Callback = function (controle, id) {
        // Só invoca o método de callback se houver necessidade
        if (dadosControle.Atual == (id == "" ? 0 : id))
            return;
        
        // Salva a natureza de operação atual
        dadosControle.Atual = id;

        // Busca os dados de impostos configurados na natureza de operação
        var dados = ctrlNaturezaOperacao.ObtemDadosComplementares(id).value;

        // Se houver valores, coloca-os nos campos referentes
        if (dados) {
            eval("dados = " + dados);

            if (dadosControle.CampoCstIcms && document.getElementById(dadosControle.CampoCstIcms) != null) {
                document.getElementById(dadosControle.CampoCstIcms).value = dados.CstIcms;

                if (document.getElementById(dadosControle.CampoCstIcms).onchange != null)
                    document.getElementById(dadosControle.CampoCstIcms).onchange();
            }

            if (dadosControle.CampoPercReducaoBcIcms && document.getElementById(dadosControle.CampoPercReducaoBcIcms) != null)
                document.getElementById(dadosControle.CampoPercReducaoBcIcms).value = dados.PercReducaoBcIcms;

            if (dadosControle.CampoCstIpi && document.getElementById(dadosControle.CampoCstIpi) != null) {
                document.getElementById(dadosControle.CampoCstIpi).value = dados.CstIpi;
                document.getElementById(dadosControle.CampoCstIpi).onblur();
            }

            if (dadosControle.CampoCstPisCofins && document.getElementById(dadosControle.CampoCstPisCofins) != null) {
                document.getElementById(dadosControle.CampoCstPisCofins).value = dados.CstPisCofins;
                document.getElementById(dadosControle.CampoCstPisCofins).onblur();
            }

            if (dadosControle.CampoCsosn && document.getElementById(dadosControle.CampoCsosn) != null && dados.Csosn != "") {
                document.getElementById(dadosControle.CampoCsosn).value = dados.Csosn;

                if (document.getElementById(dadosControle.CampoCsosn).onchange != null)
                    document.getElementById(dadosControle.CampoCsosn).onchange();
            }

            dadosControle.CfopDevolucao = dados.CfopDevolucao;
        }

        // Invoca a função de callback, se houver
        if (typeof dadosControle.Callback == "string" && dadosControle.Callback != null && dadosControle.Callback != "")
            eval(dadosControle.Callback + "(\"" + nomeControle + "\", id)");
    };
};