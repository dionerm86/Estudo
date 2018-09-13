var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de orçamentos.
 */
Servicos.Orcamentos = (function(http) {
  const API = '/api/v1/orcamentos/';

  return {
    /**
     * Recupera a lista de orçamentos.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca de orçamentos.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      filtro = filtro || {};
      filtro.pagina = pagina;
      filtro.numeroRegistros = numeroRegistros;
      filtro.ordenacao = ordenacao;

      return http().get(API.substr(0, API.length - 1), {
        params: filtro
      });
    },

    /**
     * Gera um pedido a partir do orçamento, se possível.
     * @param {!number} idOrcamento O identificador do orçamento que será finalizado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    gerarPedido: function (idOrcamento) {
      if (!idOrcamento) {
        throw new Error('Orçamento é obrigatório.');
      }

      return http().post(API + idOrcamento + '/gerarPedido');
    },

    /**
     * Gera pedidos a partir do orçamento, se possível.
     * @param {!number} idOrcamento O identificador do orçamento que será finalizado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    gerarPedidosAgrupados: function (idOrcamento) {
      if (!idOrcamento) {
        throw new Error('Orçamento é obrigatório.');
      }
      debugger;
      return http().post(API + idOrcamento + '/gerarPedidosAgrupados');
    },

    /**
     * Envia um email do orçamento.
     * @param {!number} idOrcamento O identificador do orçamento que será enviado por email.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    enviarEmail: function (idOrcamento) {
      if (!idOrcamento) {
        throw new Error('Orçamento é obrigatório.');
      }

      return http().post(API + idOrcamento + '/enviarEmail');
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de orçamentos.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function() {
      return http().get(API + 'configuracoes');
    },

    /**
     * Retorna os itens para o controle de situações de pedido.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function() {
      return http().get(API + 'situacoes');
    },

    /**
     * Recupera a lista de funcionários vendedores, incluindo o vendedor já selecionado no pedido (se estiver editanto).
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterVendedores: function (idOrcamento) {
      return http().get(API + (idOrcamento || 0) + '/vendedores');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
