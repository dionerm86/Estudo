var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de Sugestão de Clientes.
 */
Servicos.SugestaoCliente = (function (http) {
  const API = '/api/v1/clientes/sugestoes/';

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
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    },

    /**
     * Recupera os tipos de sugestão para a tela de lista de sugestões.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposSugestaoCliente: function () {
      return http().get(API + 'tipos/filtro');
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

      return http().delete(API + idSugestao);
    },

    /**
     * Retorna os itens para o filtro de situação.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    situacoes: function () {
      return Promise.resolve({
        "data": [
          {
            "id": "1",
            "nome": "Ativas"
          },
          {
            "id": "2",
            "nome": "Canceladas"
          }
        ]
      });
    }

  };
})(function () {
  return Vue.prototype.$http;
});
