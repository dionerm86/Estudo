var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de liberações.
 */
Servicos.Liberacoes = (function(http) {
  const API = '/api/v1/liberacoes/';

  return {
    /**
     * Recupera a lista de liberações.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca de liberações.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      filtro = filtro || {};
      filtro.pagina = pagina;
      filtro.numeroRegistros = numeroRegistros;
      filtro.ordenacao = ordenacao;

      return http().get(API.substr(0, API.length - 1), {
        params: filtro
      });
    },

    /**
     * Reenvia um email da liberação.
     * @param {!number} idLiberacao O identificador da liberação que será enviado por email.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    reenviarEmail: function (idLiberacao) {
      if (!idLiberacao) {
        throw new Error('Liberação é obrigatória.');
      }

      return http().post(API + idLiberacao + '/reenviarEmail');
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de orçamentos.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function() {
      return http().get(API + 'configuracoes');
    }
  };
}) (function () {
  return Vue.prototype.$http;
});
