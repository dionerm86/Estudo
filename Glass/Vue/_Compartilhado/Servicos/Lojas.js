var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de lojas.
 */
Servicos.Lojas = (function(http) {
  const API = '/api/v1/lojas/';

  return {
    /**
     * Recupera a lista de lojas para uso no controle de busca.
     * @param {?boolean} [ativas=null] Indica se apenas lojas ativas devem ser retornadas (pode ser null para que o filtro seja ignorado).
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function (ativas) {
      return http().get(API + 'filtro', {
        params: {
          ativas
        }
      });
    },

    /**
     * Recupera a data de vencimento do certificado da loja.
     * @param {!number} id O identificador da loja.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterDataVencimentoCertificado: function (id) {
      if (!id) {
        throw new Error('Loja é obrigatório.');
      }

      return http().get(API + id + '/obterDataVencimentoCertificado');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
