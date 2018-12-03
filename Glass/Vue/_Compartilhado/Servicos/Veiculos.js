var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de veículos.
 */
Servicos.Veiculos = (function(http) {
  const API = '/api/v1/veiculos/';

  return {
    /**
     * Recupera a lista de veículos.
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
     * Recupera o objeto com as configurações utilizadas na tela de listagem de veículos.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function() {
      return http().get(API + 'configuracoes');
    },

    /**
     * Remove um veículo.
     * @param {!number} placa O identificador do veículo que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (placa) {
      if (!placa) {
        throw new Error('Veículo é obrigatório.');
      }

      return http().delete(API + placa);
    },

    /**
     * Recupera a lista de veículos para controle.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterFiltro: function () {
      return http().get(API + 'filtro');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
