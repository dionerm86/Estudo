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

      return http().get(API + 'recebidas', {
        params: filtro
      });
    },

    /**
     * Altera os dados de uma conta recebida.
     * @param {!number} idContaReceber O identificador da conta recebida que será alterada.
     * @param {!Object} contaRecebida O objeto com os dados da conta recebida a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (idContaReceber, contaRecebida) {
      if (!idContaReceber) {
        throw new Error('Conta recebida é obrigatória.');
      }

      if (!contaRecebida || contaRecebida === {}) {
        return Promise.resolve();
      }

      return http().patch(API + idContaReceber, contaRecebida);
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
    },

    /**
     * Recupera os tipos de busca de NF-e utilizadas na tela de listagem de contas recebidas.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterFiltroBuscaNfe: function () {
      return http().get(API + 'tiposBuscaNfe');
    }
  };
}) (function () {
  return Vue.prototype.$http;
});
