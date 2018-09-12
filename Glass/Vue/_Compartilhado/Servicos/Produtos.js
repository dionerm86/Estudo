var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de produtos.
 */
Servicos.Produtos = (function(http) {
  const API = '/api/v1/produtos/';

  return {
    /**
     * Objeto com os serviços para a API de grupos de produtos.
     */
    Grupos: {
      /**
       * Recupera a lista de grupos de produto para uso no controle de busca.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'grupos/filtro');
      }
    },

    /**
     * Objeto com os serviços para a API de subgrupos de produtos.
     */
    Subgrupos: {
      /**
       * Recupera a lista de subgrupos de produto para uso no controle de busca.
       * @param {?number} [idGrupoProduto=null] O identificador do grupo de produto que irá filtrar os subgrupos.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function (idGrupoProduto) {
        return http().get(API + 'subgrupos/filtro', {
          params: {
            idGrupoProduto: idGrupoProduto
          }
        });
      }
    },

    /**
     * Objeto com os serviços para a API de cores de vidro.
     */
    CoresVidro: {
      /**
       * Recupera a lista de cores de vidro para uso no controle de busca.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'coresVidro/filtro');
      }
    },

    /**
     * Objeto com os serviços para a API de cores de ferragem.
     */
    CoresFerragem: {
      /**
       * Recupera a lista de cores de ferragem para uso no controle de busca.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'coresFerragem/filtro');
      }
    },

    /**
     * Objeto com os serviços para a API de cores de alumínio.
     */
    CoresAluminio: {
      /**
       * Recupera a lista de cores de alumínio para uso no controle de busca.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'coresAluminio/filtro');
      }
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
