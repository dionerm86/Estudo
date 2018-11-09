var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de parcelas.
 */
Servicos.Parcelas = (function(http) {
  const API = '/api/v1/parcelas/';

  return {
      /**
       * Recupera a lista de parcelas.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterListaParcelas: function (filtro, pagina, numeroRegistros, ordenacao) {
          return http().get(API.substr(0, API.length - 1), {
              params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
          });
      },

      /**
       * Recupera o objeto com as configurações utilizadas na tela de listagem de parcelas.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterConfiguracoesListaParcelas: function () {
          return http().get(API + 'configuracoes');
      },


      /**
       * Recupera a lista de parcelas para uso no controle de busca.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
          return http().get(API + 'filtro');
      },

      /**
     * Recupera os detalhes de uma parcela.
     * @param {!number} idParcela O identificador da parcela que será retornado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
      obterParcela: function (idParcela) {
          debugger;
          if (!idParcela) {
              throw new Error('A parcela é obrigatório.');
          }

          return http().get(API + idParcela);
      },

      /**
       * Remove uma rota.
       * @param {!number} id O identificador da parcela que será excluída.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (id) {
          if (!id) {
              throw new Error('Parcela é obrigatória.');
          }

          return http().delete(API + id);
      },

      /**
       * Recupera a lista de parcelas para uso no controle de seleção de parcelas.
       * @param {?Object} [filtro=null] O filtro usado pelo controle de seleção de parcelas.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParcelasCliente: function (filtro) {
          return http().get(API + 'cliente', {
              params: filtro
          });
      },


      /**
       * Insere uma parcela.
       * @param {!number} parcela Os dados da parcela que será inserido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (parcela) {
          debugger;
          return http().post(API.substr(0, API.length - 1), parcela);
      },

      /**
      * Insere uma parcela.
      * @param {!number} parcela Os dados da parcela que será inserido.
      * @returns {Promise} Uma promise com o resultado da operação.
      */
      atualizar: function (idParcela, parcela) {
          debugger;
          return http().patch(API + idParcela, parcela);
      },

      /**
       * Recupera a lista de parcelas para uso no controle de seleção de parcelas.
       * @param {?Object} [filtro=null] O filtro usado pelo controle de seleção de parcelas.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      formaPagto: function () {
          return Promise.resolve({
              "data": [
                {
                    "id": "1",
                    "nome": "À vista"
                },
                {
                    "id": "2",
                    "nome": "À prazo"
                }
              ]
          });
      },

  };

})(function() {
  return Vue.prototype.$http;
});
