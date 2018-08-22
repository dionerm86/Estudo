var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de transportadores.
 */
Servicos.Transportadores = (function(http) {
  const API = '/api/v1/transportadores/';

  return {
    /**
     * Recupera a lista de transportadores para um controle.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterParaControle: function () {
      return http().get(API + 'filtro');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
