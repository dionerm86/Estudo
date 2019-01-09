var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de contas a pagar.
 */
Servicos.ContasPagar = (function (http) {
  const API = '/api/v1/contasPagar/';
  const API_Pagas = API + 'pagas/';

  return {
    /**
     * Objeto com os serviços para a API de contas pagas.
     */
    Pagas: {
      /**
       * Recupera a lista de contas pagas.
       * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API_Pagas.substr(0, API_Pagas.length - 1), {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

     /**
      * Recupera a lista de tipos contábeis para o controle de busca.
      * @returns {Promise} Uma promise com o resultado da operação.
      */
      obterConfiguracoes: function () {
        return http().get(API_Pagas + 'configuracoes');
      },

      /**
       * Atualiza uma conta paga.
       * @param {number} id O identificador da conta paga que será atualizada.
       * @param {Object} contaAPagar O objeto com os dados do item a serem alterados.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (id, contaPaga) {
        return http().patch(API_Pagas + id, contaPaga);
      }
    },

    /**
     * Recupera a lista de contas a pagar.
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
     * Recupera a lista de tipos contábeis para o controle de busca.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterTiposContabeis: function () {
      return http().get(API + 'tiposContabeis');
    },

    /**
     * Atualiza uma conta a pagar.
     * @param {number} id O identificador da conta a pagar que será atualizada.
     * @param {Object} contaAPagar O objeto com os dados do item a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (id, contaAPagar) {
      return http().patch(API + id, contaAPagar);
    }
  };
})(function () {
  return Vue.prototype.$http;
});
