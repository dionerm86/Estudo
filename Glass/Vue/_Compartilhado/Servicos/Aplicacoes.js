var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de aplicações (etiqueta).
 */
Servicos.Aplicacoes = (function(http) {
  const API = '/api/v1/aplicacoes/';

  return {
    /**
     * Recupera a lista de aplicações (etiqueta) para uso no controle de busca.
     * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obter: function (filtro, pagina, numeroRegistros, ordenacao) {
      filtro = filtro || {};
      filtro.pagina = pagina;
      filtro.numeroRegistros = numeroRegistros;
      filtro.ordenacao = ordenacao;

      return http().get(API.substr(0, API.length - 1), {
        params: filtro
      });
    },

    /**
     * Remove uma aplicação de etiqueta.
     * @param {!number} idAplicacao O identificador da aplicação de etiqueta que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idAplicacao) {
      if (!idAplicacao) {
        throw new Error('Aplicacao é obrigatória.');
      }

      return http().delete(API + idAplicacao);
    },

    /**
     * Insere uma aplicação de etiqueta.
     * @param {!Object} aplicacao O objeto com os dados do aplicação a ser inserido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (aplicacao) {
      return http().post(API.substr(0, API.length - 1), aplicacao);
    },

    /**
     * Altera os dados de uma aplicação de etiqueta.
     * @param {!number} idAplicacao O identificador da aplicacao de etiqueta que será alterado.
     * @param {!Object} aplicacao O objeto com os dados do aplicação a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (idAplicacao, aplicacao) {
      if (!idAplicacao) {
        throw new Error('Aplicação é obrigatória.');
      }

      if (!aplicacao || aplicacao === {}) {
        return Promise.resolve();
      }

      return http().patch(API + idAplicacao, aplicacao);
    },

    /**
     * Recupera as configurações para a tela de lista de aplicações.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoes: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Recupera as situações das aplicações para o controle.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Recupera a lista de aplicações (etiqueta) para uso no controle de busca.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function () {
      return http().get(API + 'filtro');
    }
    }
  };
})(function() {
  return Vue.prototype.$http;
});
