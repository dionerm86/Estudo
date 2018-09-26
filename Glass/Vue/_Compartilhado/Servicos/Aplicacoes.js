var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de aplicações (etiqueta).
 */
Servicos.Aplicacoes = (function(http) {
  const API = '/api/v1/aplicacoes/';

  return {
    /**
     * Recupera a lista de aplicações (etiqueta) para uso no controle de busca.
     * @param {any} filtro
     * @param {any} pagina
     * @param {any} numeroRegistros
     * @param {any} ordenacao
     */
    obter: function (filtro, pagina, numeroRegistros, ordenacao) {
      filtro = filtro || {};
      filtro.pagina = pagina;
      filtro.numeroRegistros = numeroRegistros;
      filtro.ordenacao = ordenacao;

      return http().get(API.substr(0, API.length - 1), {
        params: filtro
      });
    },

    /**
     * Recupera a lista de aplicações (etiqueta) para uso no controle de busca.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function () {
      return http().get(API + 'filtro');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
