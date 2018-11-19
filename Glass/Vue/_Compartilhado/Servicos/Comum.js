var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API "comum".
 */
Servicos.Comum = (function(http) {
  const API = '/api/v1/comum/';

  return {
    /**
     * Recupera a lista de situações.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Recupera a lista de tipos de pessoa.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposPessoa: function () {
      return http().get(API + 'tiposPessoa');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
