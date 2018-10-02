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
     * Recupera a lista de situações de produção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
