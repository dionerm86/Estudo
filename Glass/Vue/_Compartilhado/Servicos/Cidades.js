var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de cidades.
 */
Servicos.Cidades = (function(http) {
  const API = '/api/v1/cidades/';

  return {
    /**
     * Retorna a lista de UFs do sistema.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    listarUfs: function () {
      return http().get(API + 'ufs');
    },

    /**
     * Recupera a lista de cidades a partir da UF, podendo filtrar por ID ou nome.
     * @param {!string} uf A sigla da UF para busca da lista de cidades.
     * @param {?number} [idCidade=null] O identificador da cidade para filtro na busca.
     * @param {?string} [nomeCidade=null] O nome da cidade para filtro na busca.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterListaPorUf: function (uf, idCidade, nomeCidade) {
      if (!uf) {
        throw new Error('UF é obrigatória.');
      }

      return http().get(API + 'porUf/' + uf, {
        params: {
          id: idCidade,
          nome: nomeCidade
        }
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
