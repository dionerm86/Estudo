var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de contas bancárias.
 */
Servicos.ContasBancarias = (function(http) {
  const API = '/api/v1/contasBancarias/';

  return {
    /**
     * Recupera a lista de contas bancárias para uso no controle de busca.
     * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obter: function (filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    },

    /**
     * Remove uma conta bancária.
     * @param {!number} idContaBancaria O identificador da conta bancária que será excluída.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idContaBancaria) {
      if (!idContaBancaria) {
        throw new Error('Conta bancária é obrigatória.');
      }

      return http().delete(API + idContaBancaria);
    },

    /**
     * Insere uma conta bancária.
     * @param {!Object} contaBancaria O objeto com os dados da conta bancária a ser inserida.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (contaBancaria) {
      return http().post(API.substr(0, API.length - 1), contaBancaria);
    },

    /**
     * Altera os dados de uma conta bancária.
     * @param {!number} idProcesso O identificador do item que será alterado.
     * @param {!Object} processo O objeto com os dados do item a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (idContaBancaria, contaBancaria) {
      if (!idContaBancaria) {
        throw new Error('Conta bancária é obrigatória.');
      }

      if (!contaBancaria || contaBancaria === {}) {
        return Promise.resolve();
      }

      return http().patch(API + idContaBancaria, contaBancaria);
    },

    /**
     * Recupera os bancos para o controle.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterBancos: function () {
      return http().get(API + 'bancos');
    },

    /**
     * Recupera situações para o controle.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Recupera as contas bancárias para o controle.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterParaControle: function () {
      return http().get(API + 'filtro');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
