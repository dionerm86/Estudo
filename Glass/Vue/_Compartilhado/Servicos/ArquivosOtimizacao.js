var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de arquivos de otimização.
 */
Servicos.ArquivosOtimizacao = (function (http) {
  const API = '/api/v1/arquivosOtimizacao/';

  return {
    /**
     * Recupera a lista de arquivos de otimização.
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
     * Recupera o objeto com as configurações utilizadas na tela de listagem de arquivos de otimização.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Recupera o objeto com as direções associadas a um arquivo de otimização.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterDirecoes: function () {
      return http().get(API + 'direcoes');
    },
  }
})(function () {
  return Vue.prototype.$http;
});
