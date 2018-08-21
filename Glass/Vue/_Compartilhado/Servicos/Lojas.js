var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de lojas.
 */
Servicos.Lojas = (function(http) {
  const API = '/api/v1/lojas/';

  return {
    /**
     * Recupera a lista de aplicações (etiqueta) para uso no controle de busca.
     * @param {?boolean} [ativas=null] Indica se apenas lojas ativas devem ser retornadas (pode ser null para que o filtro seja ignorado).
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function (ativas) {
      return http().get(API + 'filtro', {
        params: {
          ativas
        }
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
