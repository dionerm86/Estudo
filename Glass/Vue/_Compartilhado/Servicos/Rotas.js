var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de rotas.
 */
Servicos.Rotas = (function(http) {
  const API = '/api/v1/rotas/';

  return {
    /*
     * Recupera a lista de rotas.
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
     * Recupera o objeto com as configurações utilizadas na tela de listagem de rotas.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Remove uma rota.
     * @param {!number} id O identificador da rota que será excluída.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (id) {
      if (!id) {
        throw new Error('Rota é obrigatória.');
      }

      return http().delete(API + id);
    },

    /**
     * Recupera a lista de rotas para uso no controle de seleção.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterFiltro: function (id, codigo) {
      return http().get(API + 'filtro', {
        params: {
          id: id,
          codigo: codigo
        }
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
