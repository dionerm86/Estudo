var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de devoluções de pagamento.
 */
Servicos.DevolucoesPagamento = (function (http) {
    const API = '/api/v1/devolucoesPagamento/';

    return {
        /**
         * Recupera a lista de devoluções de pagamento.
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
         * Recupera a lista de situações para uso no controle de devoluções de pagamento.
         * @returns {Promise} Uma promise com o resultado da busca.
         */
        obterSituacoes: function () {
            return http().get(API + 'situacoes');
        }
    };
})(function () {
    return Vue.prototype.$http;
});
