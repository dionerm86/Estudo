var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de arquivos de remessa.
 */
Servicos.ArquivosRemessa = (function (http) {
  const API = '/api/v1/arquivosRemessa/';

  return {
    /**
     * Recupera a lista de arquivos de remessa.
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
     * Recupera os tipos de arquivos de remessa para uso no controle da tela.
     * @param {!number} id O identificador do arquivo de remessa.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    excluir: function (id) {
      return http().delete(API + id);
    },

    /**
     * Recupera os tipos de arquivos de remessa para uso no controle da tela.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTipos: function () {
      return http().get(API + 'tipos');
    },
  }
})(function () {
  return Vue.prototype.$http;
});
