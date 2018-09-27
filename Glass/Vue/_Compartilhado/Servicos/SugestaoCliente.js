var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de Sugestão de Clientes.
 */
Servicos.SugestaoCliente = (function (http) {
  const API = '/api/v1/sugestaoCliente/';

  return {

    /**
     * Recupera a lista de pedidos.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca de pedidos.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      filtro = filtro || {};
      filtro.pagina = pagina;
      filtro.numeroRegistros = numeroRegistros;
      filtro.ordenacao = ordenacao;

      return http().get(API.substr(0, API.length - 1), {
        params: filtro
      });
    },

    /**
     * Recupera os tipos de sugestão para a tela de lista de sugestões.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposSugestaoCliente: function () {
      return http().get(API + 'tipos');
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de sugestões de clientes.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Cancela a sugestão de clientes, se possível.
     * @param {!number} idSugestao O identificador da sugestão que será cancelada.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    cancelar: function (idSugestao) {
      if (!idSugestao) {
        throw new Error('Sugestão é obrigatória.');
      }

      return http().post(API + idSugestao + '/cancelar');
    },

  };
})(function () {
  return Vue.prototype.$http;
});
