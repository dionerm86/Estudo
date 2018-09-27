var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de produção.
 */
Servicos.Producao = (function(http) {
  const API = '/api/v1/producao/';

  return {
    /**
     * Recupera a lista de situações de produção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Recupera a lista de setores de produção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSetores: function () {
      return http().get("api/v1/producao/setores");
    }
  };
})(function() {
  return Vue.prototype.$http;
});
