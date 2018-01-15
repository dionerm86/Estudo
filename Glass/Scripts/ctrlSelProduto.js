/**
 * Tipo para o controle de seleção de produto.
 * @param nomeControle O nome do controle que está sendo criado.
 * @param dadosControle Os dados para execução das funções do controle.
 */
var SelProdutoType = function(nomeControle, dadosControle)
{
    // Variáveis internas
    var _nomeControle = nomeControle, _dadosControle = dadosControle, _campos = null;

    /**
     * (Função privada) Obtém os campos utilizados pelo controle.
     */
    var obtemCampos = function()
    {
        if (!_campos)
        {
            _campos = {
                "Codigo": document.getElementById(_nomeControle + "_ctrlSelProdBuscar_hdfValor"),
                "Descricao": document.getElementById(_nomeControle + "_lblDescricaoProd"),
                "TipoCalculo": document.getElementById(_nomeControle + "_hdfTipoCalculo"),
                "ContainerItemGenerico": document.getElementById(_nomeControle + "_containerItemGenerico"),
                "DescricaoItemGenerico": document.getElementById(_nomeControle + "_txtDescricaoItemGenerico")
            };
        }

        return _campos;
    };

    /**
     * Função executada para invocação do callback do controle.
     * @param nomeControle O nome do controle de seleção interno.
     * @param id O código do produto selecionado pelo controle.
     */
    this.CallbackSelecao = function(nomeControle, id)
    {
        // Invoca a função de callback, se houver
        if (typeof _dadosControle.Callback == "string" && _dadosControle.Callback != null && _dadosControle.Callback != "")
            eval(_dadosControle.Callback + "(\"" + _nomeControle + "\", \"" + id + "\")");
    };

    /**
     * Função executada como callback do controle de seleção.
     * @param nomeControle O nome do controle de seleção interno.
     * @param id O código do produto que foi selecionado.
     */
    this.ObtemDadosProduto = function(nomeControle, id)
    {
        var compra = !!_dadosControle.Compra ? _dadosControle.Compra : "";
        var nf = !!_dadosControle.Nf ? _dadosControle.Nf : "";

        // Busca a descrição do produto
        var resposta = id == "" ? ["", "", ""] :
            ctrlSelProduto.GetDadosProduto(id, compra, nf).value.split("|");

        // Altera os campos com os dados do produto
        if (resposta[0] != "Erro")
        {
            obtemCampos().Descricao.innerHTML = resposta[1];
            obtemCampos().TipoCalculo.value = resposta[2];
            obtemCampos().ContainerItemGenerico.style.display = resposta[3] == "true" ? "" : "none";
            if (resposta[3] != "true")
                obtemCampos().DescricaoItemGenerico.value = "";
        }

        // Limpa o controle e exibe a mensagem de erro
        else
        {
            obtemCampos().Descricao.innerHTML = "";
            obtemCampos().TipoCalculo.value = "";
            obtemCampos().ContainerItemGenerico.style.display = "none";
            obtemCampos().DescricaoItemGenerico.value = "";
            alert(resposta[1]);
        }

        // Invoca a função de callback do controle
        this.CallbackSelecao(nomeControle, id);
    };

    /**
     * Função utilizada para recuperar os dados do produto selecionado
     * atualmente pelo controle.
     */
    this.DadosProduto = function()
    {
        return {
            "Codigo": obtemCampos().Codigo.value,
            "Descricao": obtemCampos().Descricao.value,
            "TipoCalculo": obtemCampos().TipoCalculo.value,
            "ItemGenerico": obtemCampos().ContainerItemGenerico.style.display != "none",
            "DescricaoItemGenerico": obtemCampos().DescricaoItemGenerico.value
        }
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
            if (!!novoDadosControle[k] && typeof(novoDadosControle[k]) == "string" && !!textoBuscar && !!textoAlterar)
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
        return new SelProdutoType(novoNomeControle, novoDadosControle);
    };
}