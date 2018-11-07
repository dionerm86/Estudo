var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de produção.
 */
Servicos.Seguradoras = (function(http) {
  const API = '/api/v1/seguradoras/';

  return {
    /**
     * Recupera a lista de seguradoras.
     * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obter: function (filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API, {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    },

    /**
     * Remove uma seguradora.
     * @param {!number} id O identificador da seguradora que será excluída.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (id) {
      if (!id) {
        throw new Error('Seguradora é obrigatória.');
      }

      return http().delete(API + id);
    },

    /**
     * Insere uma seguradora.
     * @param {!Object} seguradora O objeto com os dados da seguradora a ser inserida.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (seguradora) {
      return http().post(API, seguradora);
    },

    /**
     * Altera os dados de um seguradora.
     * @param {!number} id O identificador do item que será alterado.
     * @param {!Object} seguradora O objeto com os dados do item a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (id, seguradora) {
      if (!id) {
        throw new Error('Seguradora é obrigatória.');
      }

      if (!seguradora || seguradora === {}) {
        return Promise.resolve();
      }

      return http().patch(API + id, seguradora);
    }
  };
})(function() {
  return Vue.prototype.$http;
});
