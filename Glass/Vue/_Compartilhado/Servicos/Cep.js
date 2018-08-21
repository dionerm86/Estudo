var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de CEP.
 */
Servicos.Cep = (function(http) {
  const API = '/api/v1/cep/';

  return {
    /**
     * Retorna os dados de endereço de um CEP.
     * @param {!string} cep O CEP para a consulta do endereço.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterEndereco: function (cep) {
      if (!cep) {
        throw new Error('CEP é obrigatório.');
      }

      return http().get(API + cep + '/endereco');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
