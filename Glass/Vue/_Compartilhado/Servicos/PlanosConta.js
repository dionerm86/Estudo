var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de planos de conta.
 */
Servicos.PlanosConta = (function(http) {
  const API = '/api/v1/planosConta/';

  return {
    /**
     * Objeto com os serviços para a API de grupos de conta.
     */
    Grupos: {
      /**
       * Recupera a lista de grupos de conta para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'grupos/filtro');
      }
    },

    /**
      * Recupera a lista de planos de conta.
      * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
      * @param {number} pagina O número da página de resultados a ser exibida.
      * @param {number} numeroRegistros O número de registros que serão exibidos na página.
      * @param {string} ordenacao A ordenação para o resultado.
      * @returns {Promise} Uma promise com o resultado da operação.
      */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    },

    /**
     * Remove um plano de conta.
     * @param {!number} id O identificador do plano de conta que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (id) {
      if (!id) {
        throw new Error('Plano de conta é obrigatório.');
      }

      return http().delete(API + id);
    },

    /**
     * Insere um plano de conta.
     * @param {!Object} planoConta O objeto com os dados do plano de conta a ser inserido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (planoConta) {
      return http().post(API, planoConta);
    },

    /**
     * Altera os dados de um plano de conta.
     * @param {!number} id O identificador do item que será alterado.
     * @param {!Object} planoConta O objeto com os dados do item a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (id, planoConta) {
      if (!id) {
        throw new Error('Plano de conta é obrigatório.');
      }

      if (!planoConta || planoConta === {}) {
        return Promise.resolve();
      }

      return http().patch(API + id, planoConta);
    },

    /**
     * Recupera a lista de planos de conta para uso no controle de busca.
     * @param {!number} tipo Indica o tipo de planos de conta a serem buscados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function (tipo) {
      if (!tipo) {
        throw new Error('Tipo de plano de conta é obrigatório.');
      }

      return http().get(API + 'filtro', {
        params: {
          tipo
        }
      });
    },

    /**
     * Recupera a lista de situações.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
