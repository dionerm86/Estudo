var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de pagamentos.
 */
Servicos.Pagamentos = (function (http) {
  const API = '/api/v1/pagamentos/';

  return {
    /**
     * Recupera a lista de pagamentos.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca de pagamentos.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    }
  };
})(function () {
  return Vue.prototype.$http;
});
