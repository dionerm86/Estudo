var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de projetos.
 */
Servicos.Projetos = (function(http) {
  const API = '/api/v1/projetos/';

  return {
    /**
     * Objeto com os serviços para a API de modelos de projeto.
     */
    Modelos: {
      /**
       * Recupera a lista de modelos de projeto.
       * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'modelos', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Recupera a lista de situações de modelo de projeto para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterSituacoes: function () {
        return http().get(API + 'modelos/situacoes');
      }
    },

    /**
     * Objeto com os serviços para a API de grupos de projeto.
     */
    Grupos: {
      /**
       * Recupera a lista de grupos de projeto para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'grupos/filtro');
      }
    },

    /**
     * Recupera a lista de projetos.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    },

    /**
     * Remove um projeto.
     * @param {!number} idProjeto O identificador do projeto que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idProjeto) {
      if (!idProjeto) {
        throw new Error('Projeto é obrigatório.');
      }

      return http().delete(API + idProjeto);
    }
  };
})(function() {
  return Vue.prototype.$http;
});
