var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de beneficiamentos.
 */
Servicos.Beneficiamentos = (function(http) {
  const API = '/api/v1/beneficiamentos/';

  return {
    /**
     * Recupera a lista de beneficiamentos para uso no controle.
     * @param {!string} tipoBeneficiamentos Um tipo para a busca de beneficiamentos (pode ser 'Todos', 'Venda' ou 'MaoDeObraEspecial').
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterParaControle: function (tipoBeneficiamentos) {
      if (!tipoBeneficiamentos) {
        throw new Error('O tipo para busca de beneficiamentos é obrigatório.');
      }

      return http().get(API + 'filtro', {
        params: {
          tipoBeneficiamentos
        }
      });
    },

    /**
     * Recupera as configurações necessárias para o controle de beneficiamentos.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoes: function () {
      return http().get(API + 'configuracoes')
    },

    /**
     * Calcula o valor total dos beneficiamentos desejados.
     * @param {!Object[]} itensSelecionados Os beneficiamentos selecionados no controle.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    calcularTotal: function (itensSelecionados) {
      if (!itensSelecionados) {
        return Promise.reject();
      }

      if (!itensSelecionados.dadosCalculo) {
        throw new Error('É necessário informar os dados de produto para cálculo.');
      }

      if (!Array.isArray(itensSelecionados.beneficiamentos)) {
        throw new Error('Beneficiamentos precisam estar no formato de array.');
      }

      if (!itensSelecionados.beneficiamentos.length) {
        return Promise.reject();
      }

      return http().post(API + 'total', itensSelecionados);
    }
  };
})(function() {
  return Vue.prototype.$http;
});
