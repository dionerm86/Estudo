var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de exportacao.
 */
Servicos.Exportacao = (function (http) {
  const API = '/api/v1/exportacao/';

  return {
    /**
     * Recupera a lista de exportação de pedidos.
     * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API, {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    },

    /**
     * Recupera a lista de situações para uso no controle de seleção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterFiltroSituacoes: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Consulta da situação para a exportação de pedidos.
     * @param {number} idExportacao O identificador da exportação do pedido.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    consultarSituacao: function (idExportacao) {      
      return http().post(API + idExportacao + '/consultarSituacao');
    }
  };
})(function () {
  return Vue.prototype.$http;
});
