var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API pública.
 */
Servicos.Publicos = (function(http) {
  const API = '/api/v1/publico/';

  return {
    /**
     * Verifica a disponibilidade da API.
     * @param {?number} [timeout=null] O tempo de timeout para a chamada. Pode ser null para que não haja timeout.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    disponibilidade: function (timeout) {
      return http().get(API + 'disponibilidade', {
        timeout
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
