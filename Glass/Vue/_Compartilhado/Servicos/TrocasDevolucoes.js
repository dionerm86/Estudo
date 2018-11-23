var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de parcelas.
 */
Servicos.TrocasDevolucoes = (function (http) {
  const API = '/api/v1/estoques/trocasDevolucoes/';

  return {
    /**
     * Recupera a lista de parcelas.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterListaTrocaDevolucao: function (filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    },

    /**
     * Recupera a lista de parcelas para uso no controle de busca.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function () {
      return http().get(API + 'filtro');
    },

    /**
     * Remove uma parcela.
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
     * Insere uma parcela.
     * @param {!number} parcela Os dados da parcela que será inserida.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (parcela) {
      return http().post(API.substr(0, API.length - 1), parcela);
    },

    /**
     * Atualiza uma parcela.
     * @param {!number} idParcela Id da parcela que será atualizada.
     * @param {!number} parcela Os dados da parcela que será atualizada.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (idParcela, parcela) {
      return http().patch(API + idParcela, parcela);
    },

    /**
     * Recupera os Tipos pagamentos de parcelas para uso no controle de seleção Tipo Pagto.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    tiposPagamento: function () {
      return http().get(API + 'tiposPagamento');
    },

  };

})(function () {
  return Vue.prototype.$http;
});
