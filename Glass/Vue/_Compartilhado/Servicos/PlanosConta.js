var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de planos de conta.
 */
Servicos.PlanosConta = (function(http) {
  const API = '/api/v1/planosConta/';

  return {
    /**
     * Recupera a lista de planos de conta para uso no controle de busca.
     * @param {!number} tipo Indica o tipo de planos de conta a serem buscados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function (tipo) {
      if (!tipo) {
        throw new Error('Tipo de plano de conta é obrigatório.');
      }

      return http().get(API + 'filtro', {
        params: {
          tipo
        }
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
