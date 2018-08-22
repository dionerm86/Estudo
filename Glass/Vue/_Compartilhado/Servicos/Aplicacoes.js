var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de aplicações (etiqueta).
 */
Servicos.Aplicacoes = (function(http) {
  const API = '/api/v1/aplicacoes/';

  return {
    /**
     * Recupera a lista de aplicações (etiqueta) para uso no controle de busca.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function () {
      return http().get(API + 'filtro');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
