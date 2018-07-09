var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de funcionários.
 */
Servicos.Funcionarios = (function(http) {
  const API = '/api/v1/funcionarios/';

  return {
    /**
     * Recupera a lista de funcionários vendedores.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterVendedores: function () {
      return http().get(API + 'vendedores');
    },

    /**
     * Recupera a lista de funcionários compradores.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterCompradores: function () {
      return http().get(API + 'compradores');
    },

    /**
     * Recupera a lista de funcionários medidores.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterMedidores: function () {
      return http().get(API + 'medidores');
    },

    /**
     * Recupera a lista de funcionários medidores.
     * @param {!number} idVendedor O identificador do vendedor que será feita a busca.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterDataTrabalho: function (idVendedor) {
      if (!idVendedor) {
        throw new Error('Vendedor é obrigatório.');
      }

      return http().get(API + idVendedor + '/dataTrabalho');
    },

    /**
     * Recupera a lista de funcionários que finalizaram pedidos.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterFinalizacaoPedidos: function () {
      return http().get(API + 'finalizacao');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
