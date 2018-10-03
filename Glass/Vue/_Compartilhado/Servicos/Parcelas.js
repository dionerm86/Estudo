var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de parcelas.
 */
Servicos.Parcelas = (function(http) {
  const API = '/api/v1/parcelas/';

  return {
    /**
     * Recupera a lista de parcelas para uso no controle de busca.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function () {
      return http().get(API + 'filtro');
    },

    /**
     * Recupera a lista de parcelas para uso no controle de seleção de parcelas.
     * @param {?Object} [filtro=null] O filtro usado pelo controle de seleção de parcelas.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParcelasCliente: function (filtro) {
      return http().get(API + 'cliente', {
        params: filtro
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
