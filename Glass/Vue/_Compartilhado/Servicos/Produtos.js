var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de produtos.
 */
Servicos.Produtos = (function(http) {
  const API = '/api/v1/produtos/';

  return {
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
