var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de impressões de etiquetas.
 */
Servicos.ImpressoesEtiquetas = (function (http) {
  const API = '/api/v1/impressoesEtiquetas/';

  return {
    /**
     * Recupera a lista de impressões de etiquetas.
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
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de impressões de etiquetas.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Recupera a lista de tipos de impressão de etiqueta.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTipos: function () {
      return http().get(API + 'tipos');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
