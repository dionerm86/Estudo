var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de funcionários.
 */
Servicos.Funcionarios = (function(http) {
  const API = '/api/v1/funcionarios/';

  return {
    /**
     * Recupera a lista de funcionários vendedores.
     * @param {?number} [idVendedorAtual=null] O identificador do vendedor atual.
     * @param {?boolean} [orcamento=null] Considerar no resultado os emissores de orçamentos?
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterVendedores: function (idVendedorAtual, orcamento) {
      return http().get(API + 'vendedores', {
        params: {
          idVendedorAtual,
          orcamento
        }
      });
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
     * Recupera a lista de funcionários conferentes.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConferentes: function () {
      return http().get(API + 'conferentes');
    },

    /**
     * Recupera a lista de funcionários liberadores de pedido.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterLiberadores: function () {
      return http().get(API + 'liberadores');
    },

    /**
     * Recupera a lista de funcionários de caixa diário.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterCaixaDiario: function () {
      return http().get(API + 'caixaDiario');
    },

    /**
     * Recupera a lista de funcionários do financeiro.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterFinanceiros: function () {
      return http().get(API + 'financeiros');
    },

    /**
     * Recupera a lista de funcionários ativos associados à clientes.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterAtivosAssociadosAClientes: function () {
      return http().get(API + 'ativosAssociadosAClientes');
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
