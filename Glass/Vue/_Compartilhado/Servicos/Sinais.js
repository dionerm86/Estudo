var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de sinais.
 */
Servicos.Sinais = (function (http) {
  const API = '/api/v1/sinais/';

  return {
    /**
     * Recupera a lista de sinais.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    }
  };
}) (function () {
  return Vue.prototype.$http;
});
