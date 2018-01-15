/**
 * Tipo para o controle de associação de retalhos à peça.
 * @param {string} nomeControle - O nome do controle que está sendo representado.
 * @param {Object} dadosControle - Um objeto com os dados necessários para o funcionamento do controle.
 */
var RetalhoAssociadoType = (function($)
{
    /*******************************************************************************\
    * Esse controle está preparado para que seja feita apenas uma requisição AJAX *
    * para todos os controles de retalhos da tela. Entretanto, para maior simpli- *
    * cidade da operação (e também para melhor performance geral) as requisições  *
    * são feitas individualmente (uma por controle).                              *
    \*******************************************************************************/

    /** @private @inner */
    var CONTEXTO_RETALHOS = "retalhosAssociados_";

    /** @private @inner */
    var controlesRetalhos = {}, dadosAtuais = {}, retalhosSelecionados = {};

    /** @private @inner */
    var atualizacao = {}, parado = {};

    /**
     * @private @inner
     * Cancela as requisições AJAX de um controle ou de todos os controles.
     * @param {string} controle - O nome do controle que será interrompido. Pode ser vazio para interromper todos os controles.
     */
    var pararAtualizacao = function(controle)
    {
        // Verifica se o controle foi informado
        if (!!controle)
        {
            // Se o controle já estiver parado retorna
            if (!!parado[controle])
                return;

            // Interrompe a "thread" com a função AJAX
            clearTimeout(atualizacao[controle]);

            // Para todas as requisições AJAX feitas pelo controle,
            // verificando pelo contexto da requisição
            for (var req = 0; req < requests.length; req++)
            {
                if (requests[req].context == CONTEXTO_RETALHOS + controle && requests[req].obj.readyState != 4)
                    requests[req].obj.abort();
            }

            // Para as atualizações do controle
            parado[controle] = true;
            atualizacao[controle] = null;
        }
        else
        {
            // Invoca o método de parar atualização para todos os controles
            // registrados na variável controlesRetalhos
            for (var c in controlesRetalhos)
                break;

            // Se não houver controle retorna
            if (!c || !atualizacao[c])
                return;

            // Interrompe a "thread" com a função AJAX
            clearTimeout(atualizacao[c]);

            // Indica que todos os controles estão parados
            for (var c in controlesRetalhos)
            {
                atualizacao[c] = null;
                parado[c] = true;
            }

            // Para todas as requisições AJAX feitas pelo controle,
            // verificando pelo contexto da requisição
            for (var req = 0; req < requests.length; req++)
            {
                if (requests[req].context == CONTEXTO_RETALHOS && requests[req].obj.readyState != 4)
                    requests[req].obj.abort();
            }
        }
    };

    /**
     * @private @inner
     * Inicia as requisições AJAX de um controle ou de todos os controles.
     * @param {Boolean} forcar - Deverá forçar a atualização do controle?
     * @param {string} controle - O nome do controle que será iniciado. Pode ser vazio para iniciar todos os controles.
     * @param {Boolean} invocarImediatamente - A função de atualização deverá ser invicada imediatamente?
     */
    var iniciarAtualizacao = function(forcar, controle, invocarImediatamente)
    {
        // Verifica se o controle foi informado
        if (!!controle)
        {
            // Só habilita se o controle estiver parado ou se será forçado
            if (!!parado[controle] || forcar == true)
            {
                // Inicia a "thread" e indica a execução
                atualizacao[controle] = setTimeout(function() { verificaAtualizacao(controle) }, invocarImediatamente == true ? 1 : 20000);
                parado[controle] = false;
            }
        }
        else
        {
            pararAtualizacao();
            var timeout = setTimeout(function() { verificaAtualizacao() }, invocarImediatamente == true ? 1 : 20000);
            
            // Invoca o método de iniciar atualização para todos os controles
            // registrados na variável controlesRetalhos
            for (var c in controlesRetalhos)
            {
                atualizacao[c] = timeout;
                parado[c] = false;
            }
        }
    };

    /**
     * @private @inner
     * Função que faz a verificação de novos retalhos para o controle.
     * @param {string} controle - O nome do controle que será verificado. Pode ser vazio para verificar todos os controles.
     */
    var verificaAtualizacao = function(controle)
    {
        // Variáveis de suporte
        var dados = [];
        var atualizar = {};

        // Percorre todos os controles registrados
        for (var c in controlesRetalhos)
        {
            // Verifica se o controle informado é o controle
            // atual do for (se um controle específico for informado)
            // ou se o controle está parado
            if ((!!controle && c != controle) || parado[c])
                continue;

            // Inicia a variável
            atualizar[c] = false;

            // Verifica se há algum produto para busca
            var idProdPed = controlesRetalhos[c].IdProdPed;
            if (idProdPed == "" || idProdPed == "0")
                continue;

            // Indica a atualização
            atualizar[c] = true;

            // Coloca o controle na variável para o AJAX
            dados.push({
                NomeControle: c,
                IdProdPed: idProdPed,
                IdsRetalhosDisponiveis: controlesRetalhos[c].RetalhosDisponiveis,
                IdsRetalhosAssociados: controlesRetalhos[c].RetalhosSelecionados,
                CallbackSelecao: controlesRetalhos[c].CallbackSelecao,
                Resposta: null,
                DadosResposta: null,
                NumeroRetalhosFolga: 0
            });
        }

        // Verifica se há algum controle para atualização
        if (dados.length > 0)
        {
            // Invoca o método de atualização de forma assíncrona
            var atualizacao = ctrlRetalhoAssociado.Atualizar(JSON.stringify(dados), 2, function(resposta)
            {
                // Verifica se há resposta
                if (!resposta.value)
                    return;

                // Converte a resposta para array
                eval("resposta = " + resposta.value);

                // Percorre todas as posições da resposta
                for (var i = 0; i < resposta.length; i++)
                {
                    // Variáveis de suporte
                    var r = resposta[i];
                    var c = r.NomeControle;

                    // Verifica se há alteração
                    if (r.Resposta == "Ok")
                    {
                        document.getElementById(c + "_lblDescricao").style.display = "";
                        document.getElementById(c + "_imgSelecionar").style.display = "";
                        document.getElementById("table_" + c).innerHTML = r.DadosResposta;

                        controlesRetalhos[c].NumeroRetalhosFolga = parseInt(r.NumeroRetalhosFolga);
                        controlesRetalhos[c].RetalhosDisponiveis = r.IdsRetalhosDisponiveis;
                        controlesRetalhos[c].Alterado = true;
                    }

                    // Verifica se não há retalhos
                    else if (r.Resposta == "Sem retalhos")
                    {
                        document.getElementById(c + "_lblDescricao").style.display = "none";
                        document.getElementById(c + "_imgSelecionar").style.display = "none";
                        document.getElementById("table_" + c).innerHTML = "";

                        controlesRetalhos[c].NumeroRetalhosFolga = 0;
                        controlesRetalhos[c].RetalhosSelecionados = null;
                        controlesRetalhos[c].RetalhosDisponiveis = null;
                        controlesRetalhos[c].Alterado = true;
                    }

                    // Verifica se há erro 
                    else if (r.Resposta == "Erro")
                        alert("Erro: " + r.DadosResposta);

                    // Exibe o controle se houver algum retalho a selecionar
                    var inputs = document.getElementById("table_" + c).getElementsByTagName("input");
                    document.getElementById("div_" + c).style.display = inputs.length > 0 ? "inline-block" : "none";

                    // Salva os dados atuais do controle
                    dadosAtuais[c] = {
                        IdsRetalhosAssociados: controlesRetalhos[c].RetalhosSelecionados,
                        IdsRetalhosDisponiveis: controlesRetalhos[c].RetalhosDisponiveis
                    };

                    // Invoca o callback, se houver
                    if (controlesRetalhos[c].Callback != "" && window[controlesRetalhos[c].Callback])
                        window[controlesRetalhos[c].Callback](c);

                    // Marca os retalhos selecionados
                    marcarRetalhos(c);
                }
            }, CONTEXTO_RETALHOS + (controle ? controle.toString() : ""));
        }

        // Invoca o método novamente se não houver dados para buscar
        else if ((!controle || atualizar[controle]) && !parado[controle])
            iniciarAtualizacao(true, controle, false);
    };

    /**
     * @private @inner
     * Função para marcar os retalhos selecionados depois da atualização.
     * @param {string} controle - O nome do controle que terá os retalhos marcados.
     */
    var marcarRetalhos = function(controle)
    {
        // Recupera os campos dos retalhos
        var inputs = document.getElementById("table_" + controle).getElementsByTagName("input");

        // Verifica se o controle foi alterado
        if (controlesRetalhos[controle].Alterado)
        {
            // Indica que o controle já foi atualizado
            controlesRetalhos[controle].Alterado = false;

            // Marca os retalhos que já estavam selecionados
            for (var j = 0; j < inputs.length; j++)
            {
                if (inputs[j].type.toLowerCase() == "checkbox")
                    inputs[j].checked = ("," + controlesRetalhos[controle].RetalhosSelecionados + ",").indexOf("," + inputs[j].value + ",") > -1;
            }
        }

        // Atualiza o campo com os retalhos selecionados
        selecionarRetalho(controle, inputs && inputs.length > 0 ? inputs[0] : null);
    };

    /**
     * @private @inner
     * Função para atualizar os retalhos selecionados.
     * @param {string} controle - O nome do controle que será atualizado.
     * @param {Object} input - O campo (que representa o retalho) que foi alterado.
     */
    var selecionarRetalho = function(controle, input)
    {
        // Verifica se ambos os parâmetros foram informados
        if (!controle || !input)
            return;

        // Recupera a tabela e todos os campos de retalhos
        var tabela = document.getElementById("table_" + controle);
        var inputs = tabela.getElementsByTagName("input");

        // Variáveis de suporte
        var sel = [];
        var textoSel = "";

        // Percorre todos os campos de retalhos
        for (var j = 0; j < inputs.length; j++)
        {
            // Se o retalho estiver selecionado, inclui na lista
            // e monta o HTML que será exibido informando a seleção
            if (inputs[j].type.toLowerCase() == "checkbox" && inputs[j].checked)
            {
                var descricao = inputs[j].nextElementSibling.innerHTML;

                sel.push({
                    ID: inputs[j].value,
                    Descricao: descricao
                });

                textoSel += "<br /><div style='padding: 2px 0 2px 8px; display: inline-block'>• " + descricao + "</div>";
            }
        }

        // Altera a descrição do controle de retalhos, informando
        // o número de retalhos selecionados, o número de retalhos disponíveis
        // e quais são os retalhos selecionados
        var descricao = document.getElementById(controle + "_lblDescricao");
        descricao.innerHTML = "<b>" + inputs.length + " retalho" + (inputs.length > 1 ? "s" : "") +
            " disponíve" + (inputs.length > 1 ? "is" : "l") + "<br />" + sel.length + " selecionado" + (sel.length > 1 ? "s" : "") + "</b>" + textoSel;

        // Seleciona apenas os IDs dos retalhos e coloca-os no objeto do controle
        var temp = [];
        for (var j = 0; j < sel.length; j++)
            temp.push(sel[j].ID);

        controlesRetalhos[controle].RetalhosSelecionados = temp.join(",");

        // Atualiza o objeto dos retalhos selecionados do controle
        retalhosSelecionados[controle] = [];

        for (var i = 0; i < sel.length; i++)
        {
            retalhosSelecionados[controle].push({
                ID: sel[i].ID,
                Descricao: sel[i].Descricao.replace("<b>", "").replace("</b>", "")
            });
        }

        // Inicia novamente a função 
        pararAtualizacao();
        iniciarAtualizacao(true, null, false);

        // Invoca o callback de seleção do controle
        if (!!controlesRetalhos[controle].CallbackSelecao)
            window[controlesRetalhos[controle].CallbackSelecao](controle);
    };

    /**
     * @private @inner
     * Função que realiza a validação dos retalhos selecionados, garantindo
     * que o mesmo retalho não seja selecionado para dois controles diferentes.
     * @returns {Boolean} A validação foi bem sucedida?
     */
    var validarRetalhos = function()
    {
        // Variáveis de suporte
        var retalhos = [];

        // Percorre os retalhos selecionados
        for (var c in retalhosSelecionados)
        {
            // Recupera o campo com a quantidade a imprimir para o controle
            var qtdImprimir = controlesRetalhos[c].CampoQtdeImprimir;
            qtdImprimir = document.getElementById(qtdImprimir);

            // Só verifica se o campo de quantidade for encontrado
            if (!!qtdImprimir)
            {
                //Verifica se é etq de reposição
                var reposicao = qtdImprimir.id.split('_')[1][0].toLowerCase() == 'r';
                           
                // Obtém a quantidade a imprimir
                qtdImprimir = parseInt(qtdImprimir.value, 10);

                // Peça reposta
                if (qtdImprimir == 0) qtdImprimir = 1;
                
                qtdImprimir = !isNaN(qtdImprimir) ? qtdImprimir : 0;

                // Verifica se o número de retalhos selecionados é
                // maior que a quantidade de peças a imprimir
                if ((reposicao && retalhosSelecionados[c].length > 1) || (!reposicao && retalhosSelecionados[c].length > qtdImprimir))
                {
                    alert("Não é possível imprimir.\nHá retalhos associados além da quantidade de produtos " +
                        "de pedido a serem impressos.\nVerifique a seleção dos retalhos.");

                    return false;
                }
            }

            // Coloca os retalhos selecionados na variável de suporte
            for (var i = 0; i < retalhosSelecionados[c].length; i++)
                retalhos.push(retalhosSelecionados[c][i]);
        }

        // Verifica se a quantidade de retalhos selecionados para um controle
        // é maior que a quantidade de peças a imprimir
        //for (var i = 0; i < retalhos.length; i++)
        //{
        //    // Verifica se o mesmo retalho foi selecionado outra vez
        //    for (var j = i + 1; j < retalhos.length; j++)
        //    {
        //        // Compara o identificador do retalho selecionado
        //        if (retalhos[i].ID == retalhos[j].ID)
        //        {
        //            alert("Não é possível imprimir.\nO mesmo retalho (" + retalhos[i].Descricao + ") " +
        //                "foi selecionado pelo menos duas vezes.\nVerifique a seleção dos retalhos.");

        //            return false;
        //        }
        //    }
        //}

        // Indica que não foi encontrado erro
        return true;
    };

    /**
     * Construtor do objeto.
     * @constructor
     * @param nomeControle - O nome do controle que está sendo criado.
     * @param dadosControle - A variável com os dados para o controle.
     * @returns {RetalhoAssociadoType} O objeto criado.
     */
    return function(nomeControle, dadosControle)
    {
        // Remover comentário para que o controle tenha apenas uma requisição AJAX
        pararAtualizacao();

        // Inicia a variável com os dados do controle de retalhos
        controlesRetalhos[nomeControle] = dadosControle;
        controlesRetalhos[nomeControle].RetalhosDisponiveis = null;
        controlesRetalhos[nomeControle].RetalhosSelecionados = null;
        controlesRetalhos[nomeControle].RetalhosAlterados = null;
        controlesRetalhos[nomeControle].NumeroRetalhosFolga = 0;
        controlesRetalhos[nomeControle].Alterado = false;

        // Inicia a variável indicando que o controle está parado
        parado[nomeControle] = true;

        /**
         * @public @instance
         * Função utilizada para indicar a seleção de um retalho.
         * @param {Object} campo - O campo que foi marcado.
         */
        this.SelecionarRetalho = function(campo)
        {
            selecionarRetalho(nomeControle, campo);
        };

        /**
         * @public @instance
         * Função utilizada para recuperar/alterar os retalhos selecionados.
         * @param {string} [valor] - O valor que será selecionado pelo controle.
         * Pode ser omitido para recuperar os valores selecionados.
         * @returns {string} Os retalhos selecionados, se não for indicado novo valor para o controle.
         */
        this.RetalhosAssociados = function(valor)
        {
            // Verifica se o valor foi informado, indicando que os retalhos
            // selecionados serão alterados
            if (!valor && valor != "")
            {
                // Retorna os retalhos selecionados
                return controlesRetalhos[nomeControle].RetalhosSelecionados;
            }

            // Altera os retalhos selecionados
            else
            {
                controlesRetalhos[nomeControle].RetalhosSelecionados = valor;
                controlesRetalhos[nomeControle].Alterado = true;
                marcarRetalhos(nomeControle);
            }
        };

        /**
         * @public @instance
         * Verifica se o controle possui retalhos para selecionar.
         * @returns {Boolean} O controle possui retalhos para selecionar?
         */
        this.PossuiRetalhos = function()
        {
            return !!controlesRetalhos[nomeControle].RetalhosDisponiveis;
        };

        /**
         * @public @instance
         * Verifica se o controle possui retalhos cujo tamanho estão dentro da "folga"
         * do produto definida nas configurações.
         * @returns {Boolean} Há retalhos com "folga" para selecionar?
         */
        this.PossuiRetalhosFolga = function()
        {
            return controlesRetalhos[nomeControle].NumeroRetalhosFolga > 0;
        };

        /**
         * @public @instance
         * Exibe os retalhos para seleção.
         * @param {Object} botao - O botão que exibe os retalhos.
         */
        this.ExibirRetalhos = function(botao)
        {
            var tabela = 'table_' + nomeControle;

            for (var i = 0; i < 2; i++)
            {
                var tableHeight = getTableHeight(tabela) > 400 ? 400 : getTableHeight(tabela);

                TagToTip(tabela, FADEIN, 300, COPYCONTENT, false, TITLE, 'Retalhos disponíveis', CLOSEBTN, true,
                    CLOSEBTNTEXT, 'Fechar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                    HEIGHT, tableHeight);
            }
        };

        /**
         * @public @instance
         * Para a execução dos métodos de verificação de retalhos do controle.
         */
        this.Parar = function()
        {
            pararAtualizacao(nomeControle);
        };

        /**
         * @public @instance
         * Inicia a execução dos métodos de verificação de retalhos do controle.
         */
        this.Iniciar = function()
        {
            iniciarAtualizacao(false, null, true);
        };

        /**
         * @public @instance
         * Realiza a validação dos retalhos selecionados.
         * @returns {Boolean} Os retalhos selecionados são válidos?
         */
        this.Validar = function()
        {
            return validarRetalhos();
        };

        /**
         * @public @instance
         * Indica se o controle está parado.
         * @returns {Boolean} O controle está parado?
         */
        this.Parado = function()
        {
            return parado[nomeControle];
        };

        // Remover comentário para que o controle tenha apenas uma requisição AJAX
        iniciarAtualizacao();

        // Inicia a verificação de retalhos do controle
        this.Iniciar();
    };

    // Para todos os controles ao fechar a janela
    if (!!$)
    {
        $(window).on("unload", function()
        {
            pararAtualizacao();
        });
    }
})(jQuery);