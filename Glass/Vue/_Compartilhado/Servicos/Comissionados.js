var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de comissionados.
 */
Servicos.Comissionados = (function(http) {
  const API = '/api/v1/comissionados/';

  return {
    /**
     * Recupera a lista de comissionados.
     * @param {?number} [idComissionado=null] O identificador do comissionado para filtro na busca.
     * @param {?string} [nomeComissionado=null] O nome do comissionado para filtro na busca.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterComissionados: function (idComissionado, nomeComissionado) {
      return http().get(API + 'filtro', {
        params: {
          id: idComissionado,
          nome: nomeComissionado
        }
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
