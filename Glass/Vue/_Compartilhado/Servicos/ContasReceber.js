var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de contas receber.
 */
Servicos.ContasReceber = (function(http) {
  const API = '/api/v1/contasReceber/';

  return {
    /**
     * Recupera a lista de contas recebidas.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterListaRecebidas: function(filtro, pagina, numeroRegistros, ordenacao) {
      filtro = filtro || {};
      filtro.pagina = pagina;
      filtro.numeroRegistros = numeroRegistros;
      filtro.ordenacao = ordenacao;

      return http().get(API + '/recebidas', {
        params: filtro
      });
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de contas recebidas.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesListaRecebidas: function() {
      return http().get(API + 'configuracoes/recebidas');
    },

    /**
     * Recupera os tipos contábeis utilizadas na tela de listagem de contas recebidas.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposContabeis: function() {
      return http().get(API + 'tiposContabeis');
    }
  };
}) (function () {
  return Vue.prototype.$http;
});
