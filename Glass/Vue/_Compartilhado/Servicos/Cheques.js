var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de cheques.
 */
Servicos.Cheques = (function(http) {
  const API = '/api/v1/cheques/';

  return {
    /**
     * Recupera a lista de cheques.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
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
     * Recupera o objeto com as configurações utilizadas na tela de listagem de cheques.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function() {
      return http().get(API + 'configuracoes');
    },

    /**
     * Altera os dados do cheque.
     * @param {!number} id O identificador do cheque que será alterado.
     * @param {!Object} cheque O objeto com os dados de um cheque a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizarCheque: function (id, cheque) {
      if (!id) {
        throw new Error('Cheque é obrigatório.');
      }

      if (!cheque || cheque === {}) {
        return Promise.resolve();
      }

      return http().patch(API + id + '/alterarDados', cheque);
    },

    /**
     * Cancela a devolução do cheque.
     * @param {!number} id O identificador do cheque.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    cancelarDevolucao: function (id) {
      if (!id) {
        throw new Error('Cheque é obrigatório.');
      }

      return http().delete(API + id + '/devolucao');
    },

    /**
     * Cancela o protesto do cheque.
     * @param {!number} id O identificador do cheque.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    cancelarProtesto: function (id) {
      if (!id) {
        throw new Error('Cheque é obrigatório.');
      }

      return http().delete(API + id + '/protesto');
    },

    /**
     * Cancela a reapresentação do cheque.
     * @param {!number} id O identificador do cheque.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    cancelarReapresentacao: function (id) {
      if (!id) {
        throw new Error('Cheque é obrigatório.');
      }

      return http().delete(API + id + '/reapresentacao');
    },

    /**
     * Retorna os itens para o controle de situações de cheque.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    }
  };
}) (function () {
  return Vue.prototype.$http;
});
