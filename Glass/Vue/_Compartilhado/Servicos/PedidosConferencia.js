var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de notas fiscais.
 */
Servicos.PedidosConferencia = (function (http) {
  const API = '/api/v1/pedidosConferencia/';

  return {
    /**
     * Recupera a lista de pedidos em conferência.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Exclui um pedido em conferência.
     * @param {!number} id O identificador do item que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (id) {
      if (!id) {
        throw new Error('Pedido em conferência é obrigatória.');
      }

      return http().delete(API + id);
    },

    /**
     * Reabre o pedido em conferência.
     * @param {?number} id O identificador do pedido em conferência.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    reabrir: function (id) {
      return http().post(API + id + '/reabrir');
    },

    /**
     * Altera a situação CNC do pedido em conferência.
     * @param {?number} id O identificador do pedido em conferência.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    alterarSituacaoCnc: function (id) {
      return http().post(API + id + '/alterarSituacaoCnc');
    },

    /**
     * Marca o pedido em conferência importado como conferido.
     * @param {?number} id O identificador do pedido em conferência.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    marcarPedidoImportadoConferido: function (id) {
      return http().post(API + id + '/marcarPedidoImportadoConferido');
    },

    /**
     * Recupera o objeto com as situações do pedido comercial para a tela de pedidos em conferência.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoesPedidoComercial: function () {
      return http().get(API + 'situacoesPedidoComercial');
    },

    /**
     * Recupera o objeto com as situações do pedido em conferência.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Recupera os dados de produção do pedido em conferência.
     * @param {?number} id O identificador do pedido em conferência.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterDadosProducao: function (id) {
      return http().get(API + id + '/dadosProducao');
    },

    /**
     * Recupera o objeto com as situações do pedido em conferência.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    podeImprimirImportados: function (filtro) {
      filtro = filtro || {};

      return http().get(API + 'podeImprimirImportados', {
        params: filtro
      });
    }
  };
})(function () {
  return Vue.prototype.$http;
});
