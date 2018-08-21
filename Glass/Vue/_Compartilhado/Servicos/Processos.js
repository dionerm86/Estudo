var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de processos (etiqueta).
 */
Servicos.Processos = (function(http) {
  const API = '/api/v1/processos/';

  return {
    /**
     * Recupera a lista de processos (etiqueta) para uso no controle de busca.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function () {
      return http().get(API + 'filtro');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
