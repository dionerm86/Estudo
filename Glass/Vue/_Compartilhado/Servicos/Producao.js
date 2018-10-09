var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de produção.
 */
Servicos.Producao = (function(http) {
  const API = '/api/v1/producao/';

  return {
    /**
     * Objeto com os serviços para a API de setores de produção.
     */
    Setores: {
      /**
       * Recupera a lista de setores de produção para uso no controle de busca.
       * @param {?boolean} [incluirSetorImpressao=null] Indica se o setor de impressão de etiquetas deve ser retornado.
       * @param {?boolean} [incluirEtiquetaNaoImpressa=null] Indica se deve ser retornado um setor de 'etiqueta não impressa'.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function (incluirSetorImpressao, incluirEtiquetaNaoImpressa) {
        return http().get(API + 'setores/filtro', {
          params: {
            incluirSetorImpressao: incluirSetorImpressao || false,
            incluirEtiquetaNaoImpressa: incluirEtiquetaNaoImpressa || false
          }
        });
      }
    },

    /**
     * Recupera as configurações para a tela de consulta de produção.
     */
    obterConfiguracoesConsulta: function () {
      return http().get(API + 'configuracoes/consulta');
    },

    /**
     * Recupera a lista de situações de produção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Recupera a lista de tipos de situações de produção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposSituacoes: function () {
      return http().get(API + 'tiposSituacoes');
    },

    /**
     * Recupera a lista de tipos de pedidos para o filtro.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposPedido: function () {
      return http().get(API + 'tiposPedidos');
    },

    /**
     * Recupera a lista de tipos de peças que poderão ser exibidas.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposPecasExibir: function () {
      return http().get(API + 'tiposPecasExibir');
    },

    /**
     * Recupera a lista de tipos de produtos de composição.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposProdutosComposicao: function () {
      return http().get(API + 'tiposProdutosComposicao');
    },

    /**
     * Recupera a lista de tipos de 'fast delivery'.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposFastDelivery: function () {
      return http().get(API + 'tiposFastDelivery');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
