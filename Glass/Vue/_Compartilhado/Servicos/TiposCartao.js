var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de tipos de cartão.
 */
Servicos.TiposCartao = (function(http) {
  const API = '/api/v1/tiposCartao/';

  return {
    /**
     * Recupera os tipos de cartão para controle de seleção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterParaControle: function() {
      return http().get(API + 'filtro');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
