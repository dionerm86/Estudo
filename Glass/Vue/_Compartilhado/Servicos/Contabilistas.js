var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de contabilistas.
 */
Servicos.Contabilistas = (function(http) {
  const API = '/api/v1/contabilistas/';

  return {
    /**
      * Recupera a lista de contabilistas.
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
     * Remove um contabilista.
     * @param {!number} id O identificador do contabilista que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (id) {
      if (!id) {
        throw new Error('Contabilista é obrigatório.');
      }

      return http().delete(API + id);
    },

    /**
     * Insere um contabilista.
     * @param {!Object} planoConta O objeto com os dados do contabilista a ser inserido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (contabilista) {
      return http().post(API, contabilista);
    },

    /**
     * Altera os dados de um contabilista.
     * @param {!number} id O identificador do item que será alterado.
     * @param {!Object} contabilista O objeto com os dados do item a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (id, contabilista) {
      if (!id) {
        throw new Error('Contabilista é obrigatório.');
      }

      if (!contabilista || contabilista === {}) {
        return Promise.resolve();
      }

      return http().patch(API + id, contabilista);
    }
  };
})(function() {
  return Vue.prototype.$http;
});
