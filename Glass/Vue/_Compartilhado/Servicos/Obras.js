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
          id,
          descricao,
          idCliente: this.pedido.idCliente,
          idsPedidosIgnorar: this.pedido.id ? [this.pedido.id] : null,
          situacao: this.configuracoes.situacaoObraConfirmada,
          gerarCredito: false
        }
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
