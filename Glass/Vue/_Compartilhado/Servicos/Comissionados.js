var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de comissionados.
 */
Servicos.Comissionados = (function(http) {
  const API = '/api/v1/comissionados/';

  return {
    /**
     * Recupera a lista de comissionados.
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
     * Recupera o objeto com as configurações utilizadas na tela de listagem de comissionados.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Remove um comissionado.
     * @param {!number} idComissionado O identificador do transportador que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idComissionado) {
      if (!idComissionado) {
        throw new Error('Comissionado é obrigatório.');
      }

      return http().delete(API + idComissionado);
    },

    /**
     * Recupera a lista de comissionados.
     * @param {?number} [idComissionado=null] O identificador do comissionado para filtro na busca.
     * @param {?string} [nomeComissionado=null] O nome do comissionado para filtro na busca.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterComissionados: function (idComissionado, nomeComissionado) {
      return http().get(API + 'filtro', {
        params: {
          id: idComissionado,
          nome: nomeComissionado
        }
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
