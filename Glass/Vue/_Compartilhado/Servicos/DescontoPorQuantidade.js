var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de desconto por quantidade.
 */
Servicos.DescontoPorQuantidade = (function(http) {
  const API = '/api/v1/desconto-quantidade/';

  return {
    /**
     * Recupera os dados de desconto por quantidade para um produto/cliente/quantidade.
     * @param {!number} idProduto O identificador do produto.
     * @param {?number} idGrupoProduto O identificador do grupo do produto.
     * @param {?number} idSubgrupoProduto O identificador do subgrupo do produto.
     * @param {!number} idCliente O identificador do cliente.
     * @param {number} quantidade A quantidade de produtos informada.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterDados: function (idProduto, idGrupoProduto, idSubgrupoProduto, idCliente, quantidade) {
      if (!idProduto) {
        throw new Error('Produto é obrigatório.');
      }

      if (!idCliente) {
        throw new Error('Cliente é obrigatório.');
      }

      return http().get(API + 'dados', {
        params: {
          idProduto,
          idGrupoProduto,
          idSubgrupoProduto,
          idCliente,
          quantidade
        }
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
