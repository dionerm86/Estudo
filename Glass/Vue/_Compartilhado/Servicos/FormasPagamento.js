var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de formas de pagamento.
 */
Servicos.FormasPagamento = (function(http) {
  const API = '/api/v1/formasPagamento/';

  return {
    /**
     * Recupera a lista de formas de pagamento para uso no controle de seleção de formas de pagamento.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterFiltro: function () {
      return http().get(API + 'filtro');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
