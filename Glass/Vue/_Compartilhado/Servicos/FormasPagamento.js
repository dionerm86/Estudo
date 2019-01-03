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
    },

    /**
     * Recupera a lista de formas de pagamento para uso no controle de seleção de formas de pagamento de notas fiscais.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterFiltroNotaFiscal: function () {
      return http().get(API + 'filtroNotaFiscal');
    },

    /**
     * Recupera a lista de formas de pagamento para uso no controle de seleção de formas de pagamento de contas recebidas.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterFiltroContasRecebidas: function () {
      return http().get(API + 'filtroContasRecebidas');
    },

    /**
     * Recupera a lista de formas de pagamento para uso no controle de seleção de formas de pagamento de compra.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterFormasPagamentoCompra: function () {
      return http().get(API + 'filtroCompras');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
