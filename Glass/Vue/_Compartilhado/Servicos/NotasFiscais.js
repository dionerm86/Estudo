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
     * Consulta a situação do lote da nota fiscal na receita.
     * @param {?number} id O identificador da nota fiscal.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    consultarSituacaoLote: function (id) {
      return http().post(API + id + '/consultarSituacaoLote');
    },

    /**
     * Consulta a situação da nota fiscal na receita.
     * @param {?number} id O identificador da nota fiscal.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    consultarSituacao: function (id) {
      return http().post(API + id + '/consultarSituacao');
    },

    /**
     * Reabre a nota fiscal de terceiros.
     * @param {?number} id O identificador da nota fiscal.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    reabrir: function (id) {
      return http().post(API + id + '/reabrir');
    },

    /**
     * Gerar nota fiscal complementar a partir da nota fiscal.
     * @param {?number} id O identificador da nota fiscal.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    gerarNotaFiscalComplementar: function (id) {
      return http().post(API + id + '/gerarNotaFiscalComplementar');
    },

    /**
     * Emitir nota fiscal FS-DA.
     * @param {?number} id O identificador da nota fiscal.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    emitirNotaFiscalFsda: function (id) {
      return http().post(API + id + '/emitirNotaFiscalFsda');
    },

    /**
     * Reenviar email da nota fiscal.
     * @param {?number} id O identificador da nota fiscal.
     * @param {?boolean} cancelamento Define se é para reenviar email de cancelamento.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    reenviarEmail: function (id, cancelamento) {
      return http().post(API + id + '/reenviarEmail', {
        params: {
          cancelamento: cancelamento || 0
        }
      });
    },

    /**
     * Separar valores de contas a receber da liberação e da nota fiscal.
     * @param {?number} id O identificador da nota fiscal.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    separarValores: function (id) {
      return http().post(API + id + '/separarValores');
    },

    /**
     * Cancela separação de valores de contas a receber da liberação e da nota fiscal.
     * @param {?number} id O identificador da nota fiscal.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    cancelarSeparacaoValores: function (id) {
      return http().post(API + id + '/cancelarSeparacaoValores');
    },

    /**
     * Emitir NFC-e.
     * @param {?number} id O identificador da nota fiscal.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    emitirNfce: function (id) {
      return http().post(API + id + '/emitirNfce');
    },

    /**
     * Emitir NFC-e em modo offline.
     * @param {?number} id O identificador da nota fiscal.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    emitirNfceOffline: function (id) {
      return http().post(API + id + '/emitirNfceOffline');
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
