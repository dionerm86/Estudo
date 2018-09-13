var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de produtos.
 */
Servicos.Produtos = (function(http) {
  const API = '/api/v1/produtos/';

  return {
    /**
     * Recupera a lista de produtos.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos produtos.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      filtro = filtro || {};
      filtro.pagina = pagina;
      filtro.numeroRegistros = numeroRegistros;
      filtro.ordenacao = ordenacao;

      return http().get(API.substr(0, API.length - 1), {
        params: filtro
      });
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de produtos.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Remove um produto.
     * @param {!number} idProduto O identificador do produto que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idProduto) {
      if (!idProduto) {
        throw new Error('Produto é obrigatório.');
      }

      return http().delete(API + idProduto);
    },

    /**
     * Recupera os produtos a partir do código interno.
     * @param {!string} codigoInterno O código interno do produto, para busca.
     * @param {?string} [tipoValidacao=null] O tipo de validação que será feita na busca.
     * @param {?string} [dadosAdicionaisValidacao=null] Os dados adicionais para validação, se houver.
     */
    obterParaControle: function (codigoInterno, tipoValidacao, dadosAdicionaisValidacao) {
      if (!codigoInterno) {
        return Promise.reject();
      }

      return http().get(API + 'filtro', {
        params: {
          codigoInterno,
          tipoValidacao,
          dadosAdicionaisValidacao
        }
      });
    },

    /**
     * Calcula a área, em m², para um produto.
     * @param {!Object} dadosProduto Os dados do produto que serão usados para o cálculo da área.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    calcularAreaM2: function (dadosProduto) {
      if (!dadosProduto) {
        throw new Error('Dados do produto são obrigatórios.');
      }

      return http().post(API + 'calcularAreaM2', dadosProduto);
    },

    /**
     * Calcula o valor total para um produto.
     * @param {!Object} dadosProduto Os dados do produto que serão usados para o cálculo do total.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    calcularTotal: function (dadosProduto) {
      if (!dadosProduto) {
        throw new Error('Dados do produto são obrigatórios.');
      }

      return http().post(API + 'calcularTotal', dadosProduto);
    }
  };
})(function() {
  return Vue.prototype.$http;
});
