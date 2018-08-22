var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de obras.
 */
Servicos.Obras = (function(http) {
  const API = '/api/v1/obras/';

  return {
    /**
     * Recupera a lista de obras para um controle.
     * @param {number} idObra O identificador da obra para filtro.
     * @param {string} descricao A descrição da obra para filtro.
     * @param {number} idCliente O identificador do cliente para filtro.
     * @param {number[]} idsPedidosIgnorar Os pedidos que serão ignorados para a busca da obra.
     * @param {number} situacao A situação das obras para filtro.
     * @param {string|number} tipoObras O tipo de obras a serem buscadas.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterParaControle: function (idObra, descricao, idCliente, idsPedidosIgnorar, situacao, tipoObras) {
      return http().get(API + 'filtro', {
        params: {
          id: idObra,
          descricao,
          idCliente,
          idsPedidosIgnorar,
          situacao,
          tipoObras
        }
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
