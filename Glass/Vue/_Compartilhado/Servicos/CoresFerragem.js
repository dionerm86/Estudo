var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de cores de ferragem.
 */
Servicos.CoresFerragem = (function(http) {
  const API = '/api/v1/produtos/coresFerragem/';

  return {
    /**
     * Recupera a lista de itens para uso no controle de busca.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function () {
      return http().get(API + 'filtro');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
