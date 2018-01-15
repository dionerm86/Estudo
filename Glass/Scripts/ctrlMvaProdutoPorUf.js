/**
 * Classe com os dados para o controle de MVA de produto por UF.
 * @param nomeControle O nome do controle que está sendo criado.
 */
var MvaProdutoPorUfType = function(nomeControle)
{
    // Constante com o número máximo de exceções
    var NUMERO_MAX_EXCECOES = Number.MAX_VALUE;

    // Variáveis de controle
    var _nomeControle = nomeControle, numeroExcecoes = 1, _itens = {},

    /**
     * (Função privada)
     * Recupera os itens (controles, labels e botões) de uma linha de exceção.
     * @param numeroItem O número do item de exceção que será recuperado.
     * @return Um objeto com os controles da linha desejada.
     */
    recuperaControlesItem = function(numeroItem)
    {
        // Verifica se os itens ainda não estão salvos na variável de controle
        if (!_itens[numeroItem])
        {
            // Busca o DIV com os controles do item
            var item = document.getElementById(_nomeControle + "_excecoes_item" + numeroItem);

            /**
             * FindControl é uma função do Utils.js
             */

            // Recupera alguns campos exibidos em partes duplicadas na lista
            var campoUfOrigem = FindControl("drpUfOrigem", "select", item);
            var campoUfDestino = FindControl("drpUfDestino", "select", item);

            // Busca os controles do item
            var item = {
                "NumeroItem": numeroItem,
                "CampoUfOrigem": campoUfOrigem,
                "CampoUfDestino": campoUfDestino,
                "Validador": FindControl("ctvExcecao", "span", item),

                "Labels": [
                    FindControl("lblUfOrigem", "span", item),
                    FindControl("lblUfDestino", "span", item),
                    FindControl("lblMvaOriginal", "span", item),
                    FindControl("lblMvaSimples", "span", item)
                ],

                "Campos": [
                    campoUfOrigem,
                    campoUfDestino,
                    FindControl("txtMvaOriginal", "input", item),
                    FindControl("txtMvaSimples", "input", item)
                ],

                "Botoes": [
                    FindControl("imgAdicionar", "input", item),
                    FindControl("imgExcluir", "input", item)
                ]
            };

            // Salva na variável de controle
            _itens[numeroItem] = item;
        }

        // Retorna o item salvo na variável de controle
        return _itens[numeroItem];
    },

    /**
     * (Função privada)
     * Altera os identificadores dos controles de um item (esse item é copiado do primeiro, id=0).
     * @param controles Os controles que serão alterados.
     * @param numeroLinha O número da linha que contém os controles.
     */
    corrigeIdsControlesItem = function(controles, numeroLinha)
    {
        // Corrige o id dos labels
        for (var i = 0; i < controles.Labels.length; i++)
            controles.Labels[i].id = controles.Labels[i].id.replace("_item0_", "_item" + numeroLinha + "_");

        // Corrige o id dos controles
        for (var i = 0; i < controles.Campos.length; i++)
            controles.Campos[i].id = controles.Campos[i].id.replace("_item0_", "_item" + numeroLinha + "_");

        // Corrige o id dos botões
        for (var i = 0; i < controles.Botoes.length; i++)
            controles.Botoes[i].id = controles.Botoes[i].id.replace("_item0_", "_item" + numeroLinha + "_");

        // Corrige o id do validador
        if (!!controles.Validador)
            controles.Validador.id = controles.Validador.id.replace("_item0_", "_item" + numeroLinha + "_");
    },

    /**
     * (Função privada)
     * Limpa os valores dos controles indicados.
     * @param controles Os controles que serão alterados.
     */
    limpaControlesItem = function(controles)
    {
        // Limpa os valores dos campos
        for (var i = 0; i < controles.Campos.length; i++)
            controles.Campos[i].value = "";
    },

    /**
     * (Função privada)
     * Esconde os botões de todas as linhas, com exceção dos botões da última linha, 
     * que são exibidos de acordo com o seu número.
     */
    exibeApenasUltimosBotoes = function()
    {
        // Percorre todas as linhas de exceção
        for (var i = 0; i < numeroExcecoes; i++)
        {
            // Recupera os controles da linha
            var controles = recuperaControlesItem(i);

            // Verifica se é a última linha
            if ((i + 1) != numeroExcecoes)
            {
                // Esconde todos os botões da linha
                for (var j = 0; j < controles.Botoes.length; j++)
                    controles.Botoes[j].style.visibility = "hidden";
            }
            else
            {
                // Exibe o botão para adicionar se a linha não for a última
                // (conforme definido pela constante com o número máximo de exceções)
                controles.Botoes[0].style.visibility = i < (NUMERO_MAX_EXCECOES - 1) ? "" : "hidden";

                // Exibe o botão para remover se a linha não for a primeira
                // (mínimo de uma exceção)
                controles.Botoes[1].style.visibility = i > 0 ? "" : "hidden";
            }
        }
    },

    /**
     * (Função privada)
     * Permite selecionar todas as opções de UF novamente.
     */
    desbloqueiaUfsSelecionadas = function()
    {
        // Percorre todas as exceções
        for (var i = 0; i < numeroExcecoes; i++)
        {
            // Recupera os controles da linha
            var controles = recuperaControlesItem(i);

            // Habilita todas as opções do campo de UF
            for (var j = 0; j < controles.CampoUfDestino.options.length; j++)
                controles.CampoUfDestino.options[j].disabled = false;
        }
    },

    /**
     * (Função privada)
     * Bloqueia uma UF em todas as linhas de exceção, exceto a
     * linha que possui a UF selecionada.
     * @param numeroLinha O número da linha que será usada para buscar a UF.
     * @param ufOrigem A UF de origem da linha que está sendo alterada.
     */
    bloqueiaUfSelecionada = function(numeroLinha, ufOrigem)
    {
        // Salva a posição da UF
        var posUf = -1, uf = recuperaControlesItem(numeroLinha).CampoUfDestino.options;
        for (var j = 0; j < uf.length; j++)
            if (uf[j].selected)
        {
            posUf = j;
            break;
        }

        // Percorre todas as exceções
        for (var j = 0; j < numeroExcecoes; j++)
        {
            // Não verifica a linha atual e não verifica linhas que possuem
            // UF de origem selecionada diferente da atual
            if (j == numeroLinha || recuperaControlesItem(j).CampoUfOrigem.value != ufOrigem)
                continue;

            // Desabilita a UF
            recuperaControlesItem(j).CampoUfDestino.options[posUf].disabled = true;
        }
    },

    /**
     * (Função privada)
     * Adiciona/remove o validador à lista de validadores da página, e atualiza a lista de validadores.
     * @param validador O validador que será adicionado/removido.
     * @param numeroLinha O número da linha que está sendo atualizada.
     * @param adicionar O validador será adicionado?
     */
    atualizaValidador = function(validador, numeroLinha, adicionar)
    {
        // Busca o validador na lista
        var posValidador = -1;
        for (var i = 0; i < Page_Validators.length; i++)
            if (Page_Validators[i].id == validador.id)
        {
            posValidador = i;
            break;
        }

        var alterou = false;

        // Adiciona o validador, se necessário
        if (adicionar && posValidador == -1)
        {
            // Coloca os mesmos parâmetros do validador original ao validador
            // que está sendo adicionado (cópia do primeiro)
            if (numeroLinha != 0)
            {
                // Recupera o validador da primeira linha
                var validadorOriginal = recuperaControlesItem(0).Validador;

                // Busca todos os atributos do validador original, selecionando
                // aqueles que não estiverem presentes no novo validador
                for (var atributo in validadorOriginal)
                    if (validador[atributo] == null || validador[atributo] === undefined)
                {
                    try
                    {
                        // Tenta colocar o valor do atributo do validador original
                        // no novo validador
                        validador[atributo] = validadorOriginal[atributo];

                        // Verifica se o atributo possui valor e se ele é numérico
                        if (validador[atributo] && typeof (validador[atributo]) == "string")
                        {
                            // Substitui no valor do atributo o número do item, se existir
                            validador[atributo] = validador[atributo].replace("_item0_", "_item" + numeroLinha + "_");

                            // Altera o número da linha na mensagem de erro
                            if (atributo == "errormessage")
                                validador[atributo] = validador[atributo].replace("Exceção 1", "Exceção " + (numeroLinha + 1));
                        }
                    }
                    catch (err) { }
                }
            }

            // Adiciona o validador à lista da página
            Page_Validators.push(validador);
            alterou = true;
        }

        // Remove o validador, se necessário
        else if (!adicionar && posValidador > -1)
        {
            // Coloca os validadores em uma pilha temporária
            var pilha = [];
            while (Page_Validators.length > posValidador)
                pilha.push(Page_Validators.pop());

            // Remove o primeiro item da pilha, que é o
            // validador que desejamos remover da lista
            pilha.pop();

            // Retorna os validadores para a lista da página
            while (pilha.length > 0)
                Page_Validators.push(pilha.pop());

            alterou = true;
        }

        // Atualiza a lista de validadores (função do Utils.js)
        if (alterou)
            atualizaValidadores();
    },

    /**
     * (Função privada)
     * Coloca o validador nos controles de uma linha.
     * @param numeroLinha O número da linha que será alterada.
     */
    colocaValidadorControles = function(numeroLinha)
    {
        var controles = recuperaControlesItem(0);

        // Coloca o evento de validação nos controles da linha
        if (!!controles.Validador && !!window["ValidatorHookupControl"])
        {
            for (var i = 0; i < controles.Campos.length; i++)
                ValidatorHookupControl(controles.Campos[i], controles.Validador);
        }
    };

    /**
     * Bloqueia todas as UFs selecionadas em todas as linhas,
     * permitindo apenas uma seleção da mesma UF em todo o controle.
     */
    this.BloqueiaUfsSelecionadas = function()
    {
        // Desbloqueia o uso de todas as UFs
        desbloqueiaUfsSelecionadas();

        // Bloqueia novamente todas as UFs selecionadas
        for (var i = 0; i < numeroExcecoes; i++)
        {
            // Verifica se há uma UF selecionada, bloqueando a
            // sua seleção para as outras linhas de exceção
            if (recuperaControlesItem(i).CampoUfDestino.value != "")
                bloqueiaUfSelecionada(i, recuperaControlesItem(i).CampoUfOrigem.value);
        }
    };

    /**
     * Adiciona uma linha de exceção, com os novos controles.
     */
    this.AdicionarItemExcecao = function()
    {
        // Recupera os DIVs
        var excecoes = document.getElementById(_nomeControle + "_excecoes");
        var itemBase = document.getElementById(_nomeControle + "_excecoes_item0");

        // Cria o novo item, copiando o primeiro
        var novoItem = document.createElement("div");
        novoItem.id = _nomeControle + "_excecoes_item" + numeroExcecoes++;
        novoItem.style.marginLeft = itemBase.style.marginLeft;
        novoItem.innerHTML = itemBase.innerHTML;

        for (var i = 0; i < itemBase.style.length; i++)
        {
            var s = itemBase.style[i];
            novoItem.style[s] = itemBase.style[s];
        }

        excecoes.appendChild(novoItem);

        // Recupera os controles da linha e faz as alterações necessárias
        var controles = recuperaControlesItem(numeroExcecoes - 1);
        corrigeIdsControlesItem(controles, numeroExcecoes - 1);
        limpaControlesItem(controles);
        exibeApenasUltimosBotoes();
        colocaValidadorControles(numeroExcecoes - 1);

        // Bloqueia os itens selecionados na nova linha
        this.BloqueiaUfsSelecionadas();

        // Registra o validador da página
        if (!!controles.Validador)
            atualizaValidador(controles.Validador, numeroExcecoes - 1, true);
    };

    /**
     * Remove uma linha de exceção e todos os seus controles.
     */
    this.RemoverItemExcecao = function()
    {
        // Recupera os DIVs
        var excecoes = document.getElementById(_nomeControle + "_excecoes");
        var itemRemover = document.getElementById(_nomeControle + "_excecoes_item" + --numeroExcecoes);

        // Remove o último item
        excecoes.removeChild(itemRemover);
        exibeApenasUltimosBotoes();

        // Habilita novamente o item selecionado na linha removida
        this.BloqueiaUfsSelecionadas();

        // Remove o validador da página
        if (!!recuperaControlesItem(numeroExcecoes).Validador)
            atualizaValidador(recuperaControlesItem(numeroExcecoes).Validador, numeroExcecoes, false);

        _itens[numeroExcecoes] = null;
    };

    /**
     * Função de validação dos valores gerais.
     */
    this.ValidaItemGeral = function(val, args)
    {
        var mvaOriginal = document.getElementById(_nomeControle + "_txtMvaOriginal").value;
        var mvaSimples = document.getElementById(_nomeControle + "_txtMvaSimples").value;

        args.IsValid = mvaOriginal != "" && mvaSimples != "";
    };

    /**
     * Função de validação de uma linha.
     */
    this.ValidaItemExcecao = function(val, args)
    {
        // Recupera o número da linha 
        var numeroItem = -1;
        for (var i = 0; i < numeroExcecoes; i++)
            if (val.id.indexOf("_item" + i + "_") > -1)
        {
            numeroItem = i;
            break;
        }

        // Recupera os controles do item
        var controles = recuperaControlesItem(numeroItem);

        // Verifica se há algum campo preenchido ou vazio
        var vazio = false;
        var preenchido = false;

        for (var i = 0; i < controles.Campos.length; i++)
            if (controles.Campos[i].value != "")
        {
            preenchido = true;
            if (vazio) break;
        }
        else
        {
            vazio = true;
            if (preenchido) break;
        }

        // Indica a validade do campo se não houver nenhum campo preenchido
        // ou se não houver nenhum campo vazio
        args.IsValid = !preenchido || !vazio;
    };

    /**
     * Prepara o controle para o submit, montando a string que
     * representa as exceções.
     */
    this.PreparaSubmit = function()
    {
        // Variável com as exceções
        var itens = [];

        // Busca em todas as exceções
        for (var i = 0; i < numeroExcecoes; i++)
        {
            // Recupera os controles da linha
            var controles = recuperaControlesItem(i);

            // Verifica se o campo de UF possui valor, o que
            // significa que todos os campos também possuem,
            // porque o validador garante isso
            if (controles.CampoUfOrigem.value != "")
            {
                // Variável com os valores dos campos
                var item = [];

                // Coloca os valores dos campos na variável
                for (var j = 0; j < controles.Campos.length; j++)
                    item.push(controles.Campos[j].value);

                // Coloca a variável como string separada por "|" na variável principal
                itens.push(item.join("|"));
            }
        }

        // Coloca os dados da variável principal (itens) no HiddenField hdfExcecoes
        // com os dados separados por "/" entre itens e por "|" entre dados do item
        document.getElementById(_nomeControle + "_hdfExcecoes").value = itens.join("/");
    };

    /**
     * Preenche os campos de um item, em sequência.
     * @param numeroItem O número do item de exceção que será preenchido.
     * @param [args] Os valores dos campos, em sequência.
     */
    this.PreencheItemExcecao = function(numeroItem)
    {
        // Recupera o número de argumentos passados, excluindo-se o primeiro
        var numeroArgumentos = arguments.length - 1;

        // Recupera os controles de um item de exceção
        var controles = recuperaControlesItem(numeroItem);

        // Preenche os campos do item de exceção
        for (var i = 0; i < Math.min(numeroArgumentos, controles.Campos.length); i++)
            controles.Campos[i].value = arguments[i + 1];
    };

    /**
     * Retorna uma string utilizável para funções em Ajax.
     * Contém os dados gerais e as exceções em uma única string.
     */
    this.DadosParaAjax = function()
    {
        // Preenche o campo com as exceções cadastradas
        this.PreparaSubmit();

        // Recupera o valor dos campos gerais
        var mvaOriginal = document.getElementById(_nomeControle + "_txtMvaOriginal").value;
        var mvaSimples = document.getElementById(_nomeControle + "_txtMvaSimples").value;

        // Retorna uma string com os dados originais e as exceções concatenadas
        return mvaOriginal + "/" + mvaSimples + "/" + document.getElementById(_nomeControle + "_hdfExcecoes").value;
    };

    /**
     * Habilita/desabilita o controle.
     * @param habilitar O controle será habilitado?
     */
    this.Habilitar = function(habilitar)
    {
        // Habilita/desabilita os campos gerais
        document.getElementById(_nomeControle + "_txtMvaOriginal").disabled = !habilitar;
        document.getElementById(_nomeControle + "_txtMvaSimples").disabled = !habilitar;

        // Se for habilitar, bloqueia novamente as UF já selecionadas
        if (habilitar)
            this.BloqueiaUfsSelecionadas();

        // Busca em todas as exceções
        for (var i = 0; i < numeroExcecoes; i++)
        {
            // Recupera os controles da linha
            var controles = recuperaControlesItem(i);

            // Habilita/desabilita os controles
            for (var j = 0; j < controles.Campos.length; j++)
                controles.Campos[j].disabled = !habilitar;
        }

        // Habilita/desabilita os botões da última linha
        for (var i = 0; i < controles.Botoes.length; i++)
            controles.Botoes[i].disabled = !habilitar;
    };
    
    /**
     * Limpa o controle.
     */
    this.Limpar = function()
    {
        // Habilita/desabilita os campos gerais
        document.getElementById(_nomeControle + "_txtMvaOriginal").value = "";
        document.getElementById(_nomeControle + "_txtMvaSimples").value = "";

        // Mantém apenas 1 exceção
        while (numeroExcecoes > 1)
            this.RemoverItemExcecao();
        
        // Busca em todas as exceções
        for (var i = 0; i < numeroExcecoes; i++)
            limpaControlesItem(recuperaControlesItem(i));
    };

    // Coloca o validador nos controles gerais e da primeira linha
    var geral = document.getElementById(_nomeControle + "_geral");
    ValidatorHookupControl(FindControl("txtMvaSimples", "input", geral), FindControl("ctvGeral", "span", geral));
    colocaValidadorControles(0);
};