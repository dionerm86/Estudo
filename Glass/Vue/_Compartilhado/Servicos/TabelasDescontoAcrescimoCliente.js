var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de tabelas de desconto/acréscimo.
 */
Servicos.TabelasDescontoAcrescimoCliente = (function (http) {
  const API = '/api/v1/tabelasDescontoAcrescimoCliente/';

  return {
    /**
     * Recupera a lista de tabelas de desconto/acréscimo para uso no controle de seleção.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterFiltro: function () {
      return http().get(API + 'filtro');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
