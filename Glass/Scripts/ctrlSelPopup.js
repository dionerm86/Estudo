/**
 * Tipo para o controle de seleção por popup.
 * @param nomeControle O nome do controle que está sendo carregado.
 * @param dadosControle Os dados para execução das funções do controle.
 */
var SelPopupType = function(nomeControle, dadosControle)
{
    // Variáveis internas
    var _nomeControle = nomeControle, _dadosControle = dadosControle;

    /**
     * (Função privada) Recupera os valores que serão utilizados como parâmetros.
     */
    var obtemParametrosReais = function()
    {
        // Recupera os parâmetros definidos no controle
        var parametros = _dadosControle.Parametros;

        // Verifica se os parâmetros são lidos dos controles da tela
        if (_dadosControle.UsarParametrosReais)
        {
            var tempParam = parametros.split("|");
            parametros = "";

            // Para cada parâmetro busca o valor do controle associado
            for (i = 0; i < tempParam.length; i++)
            {
                var dadosParam = tempParam[i].split(":");

                if (i > 0) parametros += "|";
                parametros += dadosParam[0] + ":" + eval(dadosParam[1]);
            }
        }

        // Retorna a string com os parâmetros
        return parametros;
    };

    /**
     * Função executada como callback da janela de popup.
     * Altera o valor dos campos do controle.
     * @param id O valor do campo "hdfValor".
     * @param descr O valor do campo "txtDescr".
     */
    this.AlteraValor = function(id, descr)
    {
        // Atribui os dados aos controles
        var valor = document.getElementById(_nomeControle + "_hdfValor");
        var texto = document.getElementById(_nomeControle + "_txtDescr");

        valor.value = id;
        texto.value = descr;

        // Invoca a função de callback, se houver
        if (typeof _dadosControle.Callback == "string" && _dadosControle.Callback != null && _dadosControle.Callback != "")
            eval(_dadosControle.Callback + "(\"" + _nomeControle + "\", id)");
    };

    /**
     * Função executada ao selecionar o botão de pesquisa de item (lupa).
     * Exibe a tela de popup para seleção do item do controle.
     */
    this.AbrirPopup = function()
    {
        // Recupera os parâmetros
        var parametros = obtemParametrosReais();

        // Exibe a janela de popup para busca do item
        openWindow(_dadosControle.Altura, _dadosControle.Largura, _dadosControle.Url +
            (_dadosControle != null && _dadosControle.Url != null && _dadosControle.Url != "" && _dadosControle.Url.indexOf("?") == -1 ? "?" : "&") +
            "controle=" + _nomeControle + "&callbackSel=" + _dadosControle.Callback + (!_dadosControle.PaginaPadrao ? "&callback=setForPopup" :
            "&tituloTela=" + _dadosControle.TituloTela + "&colunaId=" + _dadosControle.NomeCampoId +
            "&colunaDescr=" + _dadosControle.NomeCampoDescr + "&exibirId=" + _dadosControle.ExibirCampoId +
            "&colunasExibir=" + _dadosControle.ColunasExibir + "&nomeMetodo=" + _dadosControle.NomeMetodo +
            "&parametros=" + parametros + "&titulosColunas=" + _dadosControle.TitulosColunas +
            "&parametrosReais=" + _dadosControle.UsarParametrosReais));
    };

    /**
     * Função executada ao alterar o valor do campo.
     * Busca o valor de acordo com a descrição digitada.
     */
    this.AlterarCampo = function()
    {
        // Recupera o texto digitado
        var texto = document.getElementById(_nomeControle + "_txtDescr").value;

        // Se o texto estiver vazio só limpa o campo de valor
        if (texto == "")
        {
            this.AlteraValor("", "");
            return;
        }

        // Recupera os parâmetros
        var parametros = obtemParametrosReais();

        // Busca via Ajax o valor pela descrição
        var buscar = ctrlSelPopup.BuscarByDescricao(_dadosControle.NomeCampoId, _dadosControle.NomeCampoDescr,
            _dadosControle.TipoObjetoDados, _dadosControle.NomeMetodo, parametros, texto, _dadosControle.UsarParametrosReais).value.split("|");

        // Se a busca for bem-sucedida altera o valor
        if (buscar[0] == "Ok")
            this.AlteraValor(buscar[1], buscar[2]);

        // Limpa o controle e exibe o erro, se necessário
        else
        {
            this.AlteraValor("", "");
            if (_dadosControle.ExibirErroItemNaoEncontrado)
                alert(buscar[1]);
        }
    };

    /**
     * Função de validação do controle.
     */
    this.Validar = function(val, args)
    {
        args.IsValid = args.Value != "";
    };

    /**
     * Retorna um objeto que é um clone do objeto atual.
     * @param novoNomeControle O nome do novo controle.
     */
    this.Clonar = function(novoNomeControle, textoBuscar, textoAlterar)
    {
        // Cria o objeto dos parâmetros
        var novoDadosControle = {};

        // Preenche o novo objeto com os mesmos parâmetros do objeto atual,
        // alterando o nome do controle para o novo nome
        for (var k in _dadosControle)
        {
            // Atribui o valor
            novoDadosControle[k] = _dadosControle[k];

            // Verifica se a propriedade do objeto é string
            if (!!novoDadosControle[k] && typeof (novoDadosControle[k]) == "string" && !!textoBuscar && !!textoAlterar)
            {
                var pos = 0;

                // Altera o nome do controle na propriedade, enquanto existir
                while ((pos = novoDadosControle[k].indexOf(textoBuscar, pos)) > -1)
                {
                    var inicio = novoDadosControle[k].substr(0, pos);
                    var fim = novoDadosControle[k].substr(pos + textoBuscar.length);

                    novoDadosControle[k] = inicio + textoAlterar + fim;
                    pos += textoAlterar.length;
                }
            }
        }

        // Retorna o novo objeto
        return new SelPopupType(novoNomeControle, novoDadosControle);
    };
}