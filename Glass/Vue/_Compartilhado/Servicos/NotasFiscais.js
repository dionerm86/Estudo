var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de notas fiscais.
 */
Servicos.NotasFiscais = (function(http) {
  const API = '/api/v1/notasFiscais/';

  return {
    /**
     * Recupera a lista de notas fiscais.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
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
     * Recupera o objeto com as configurações utilizadas na tela de listagem.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function() {
      return http().get(API + 'configuracoes');
    },

    /**
     * Valida se um boleto pode ser impresso.
     * @param {?number} id O identificador da nota fiscal.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    validarBoleto: function (id) {
      return http().get(API + (id || 0) + '/validarBoleto');
    },

    /**
     * Recupera o id da nota fisal através da conta a receber.
     * @param {?number} idNotaFiscal O identificador da nota fiscal.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterIdNotaFiscalPeloIdContaReceber: function (idContaReceber) {
      return http().get(API + '/obterIdNotaFiscalPeloIdContaReceber', {
        params: {
          idContaReceber: idContaReceber || 0
        }
      });
    },

    /**
     * Verifica se o boleto já foi impresso.
     * @param {?number} idNotaFiscal O identificador da nota fiscal.
     * @param {?number} idContaReceber O identificador da conta a receber.
     * @param {?number} idLiberacao O identificador da liberação.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterMensagemBoletoImpresso: function (id, idContaReceber, idLiberacao) {
      return http().get(API + id + '/obterMensagemBoletoImpresso', {
        params: {
          idContaReceber: idContaReceber || 0,
          idLiberacao: idLiberacao || 0
        }
      });
    }
  };
}) (function () {
  return Vue.prototype.$http;
});
