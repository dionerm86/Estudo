var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de clientes.
 */
Servicos.Clientes = (function(http) {
  const API = '/api/v1/clientes/';

  return {
    /**
     * Recupera a lista de clientes para uso no controle de busca.
     * @param {?number} [idCliente=null] O identificador do cliente para filtro na busca.
     * @param {?string} [nomeCliente=null] O nome do cliente para filtro na busca.
     * @param {?string} [tipoValidacao=null] O tipo de validação que será feita na busca de clientes.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function (idCliente, nomeCliente, tipoValidacao) {
      return http().get(API + 'filtro', {
        params: {
          id: idCliente,
          nome: nomeCliente,
          tipoValidacao
        }
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
