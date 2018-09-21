var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de boletos.
 */
Servicos.Boletos = (function(http) {
  const API = '/api/v1/boletos/';

  return {
    /**
     * Valida se um boleto pode ser impresso.
     * @param {?number} idNotaFiscal O identificador da nota fiscal.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    validarBoleto: function (idNotaFiscal) {
      return http().get(API + 'validarImpressao', {
        params: {
          idNotaFiscal
        }
      });
    },

    /**
     * Recupera o identificador da nota fiscal através da conta a receber.
     * @param {?number} idContaReceber O identificador da conta a receber.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterIdNotaFiscalPeloIdContaReceber: function (idContaReceber) {
      return http().get(API + 'obterIdNotaFiscalPeloIdContaReceber', {
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
     * @param {?number} idConhecimentoTransporte O identificador do conhecimento de transporte.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterMensagemBoletoImpresso: function (idNotaFiscal, idContaReceber, idLiberacao, idConhecimentoTransporte) {
      return http().get(API + 'mensagemImpresso', {
        params: {
          idNotaFiscal,
          idContaReceber,
          idLiberacao,
          idCte: idConhecimentoTransporte
        }
      });
    }
  };
}) (function () {
  return Vue.prototype.$http;
});
