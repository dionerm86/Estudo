var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de rotas.
 */
Servicos.Rotas = (function(http) {
  const API = '/api/v1/rotas/';

  return {
    /**
     * Recupera a lista de rotas para uso no controle de seleção.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterFiltro: function () {
      return http().get(API + 'filtro');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
