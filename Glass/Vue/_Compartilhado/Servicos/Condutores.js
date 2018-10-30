var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de condutores.
 */
Servicos.Condutores = (function (http) {
  const API = '/api/v1/condutores/';

  return {
    /**
     * Recupera a lista de condutores para uso no controle de busca.
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
     * Remove um condutor.
     * @param {!number} idCondutor O identificador do condutor de etiqueta que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idCondutor) {
      if (!idCondutor) {
        throw new Error('Condutor é obrigatório.');
      }

      return http().delete(API + idCondutor);
    },

    /**
     * Insere um condutor de etiqueta.
     * @param {!Object} condutor O objeto com os dados do condutor a ser inserido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (condutor) {
      return http().post(API.substr(0, API.length - 1), condutor);
    },

    /**
     * Altera os dados de um condutor.
     * @param {!number} idCondutor O identificador do condutor que será alterado.
     * @param {!Object} condutor O objeto com os dados do condutor a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (idCondutor, condutor) {
      if (!idCondutor) {
        throw new Error('Condutor é obrigatório.');
      }

      if (!condutor || condutor === {}) {
        return Promise.resolve();
      }

      return http().patch(API + idCondutor, condutor);
    },

  };
})(function() {
  return Vue.prototype.$http;
});
