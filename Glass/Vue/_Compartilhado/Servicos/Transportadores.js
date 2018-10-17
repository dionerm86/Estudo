var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de transportadores.
 */
Servicos.Transportadores = (function(http) {
  const API = '/api/v1/transportadores/';

  return {
    /**
     * Recupera a lista de transportadores.
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
     * Recupera o objeto com as configurações utilizadas na tela de listagem de transportadores.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function() {
      return http().get(API + 'configuracoes');
    },

    /**
     * Remove um transportador.
     * @param {!number} idTransportador O identificador do transportador que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idTransportador) {
      if (!idTransportador) {
        throw new Error('Transportador é obrigatório.');
      }

      return http().delete(API + idTransportador);
    },

    /**
     * Recupera a lista de transportadores para um controle.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterParaControle: function () {
      return http().get(API + 'filtro');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
