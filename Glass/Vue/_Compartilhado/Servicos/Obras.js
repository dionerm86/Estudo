var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de obras.
 */
Servicos.Obras = (function(http) {
  const API = '/api/v1/obras/';

  return {
    /**
     * Recupera a lista de obras para um controle.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterParaControle: function (idObra, descricao, idCliente, idsPedidosIgnorar, situacao) {
      return http().get(API + 'filtro', {
        params: {
          id: idObra,
          descricao,
          idCliente: idCliente,
          idsPedidosIgnorar: idsPedidosIgnorar,
          situacao: situacao,
          tipoObras: 'PagamentoAntecipado'
        }
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
