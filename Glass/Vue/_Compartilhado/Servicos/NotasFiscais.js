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
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
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
     * Remove uma nota fiscal.
     * @param {!number} id O identificador da nota fiscal que será excluída.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (id) {
      if (!id) {
        throw new Error('Nota fiscal é obrigatória.');
      }

      return http().delete(API + id);
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
     * Habilitar o modo de contingência da nota fiscal.
     * @param {!number} tipoContingencia Define o tipo de contingencia que será alterado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    alterarContingencia: function (tipoContingencia) {
      return http().post(API + '/alterarContingencia', {
          tipoContingencia
      });
    },

    /**
     * Baixa o XML da nota.
     * @param {!number} id O identificador da nota fiscal.
     * @param {?boolean} inutilizacao Define se será buscado o xml de inutilização da nota.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    baixarXml: function (id, inutilizacao) {
      const url = '../Handlers/NotaXml.ashx'
        + '?idNf=' + id
        + (inutilizacao ? '&tipo=inut' : '');

      window.location.assign(url);
    },

    /**
     * Baixa vários arquivos XML em lote.
     * @param {string} filtros Os filtros que serão usados para filtrar as notas fiscais.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    baixarXmlEmLote: function (filtros) {
      window.location.assign('../Handlers/NotaXmlLote.ashx' + filtros);
    },

    /**
     * Baixa o XML da nota fiscal de terceiros.
     * @param {!number} id O identificador da nota fiscal.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    baixarXmlTerceiros: function (id) {
      window.location.assign('../Handlers/NFeEntradaTerceirosXML.ashx?idNfTer=' + id);
    },

    /**
     * Recupera o objeto com as situações de nota fiscal.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Recupera os tipos de participantes fiscais para o controle de seleção.
     * @param {?boolean} [incluirAdministradoraCartao=null] Indica se a administradora de cartão será incluída como tipo de participante válido.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposParticipantesFiscais: function (incluirAdministradoraCartao) {
      return http().get(API + 'tiposParticipantes', {
        params: {
          incluirAdministradoraCartao
        }
      });
    }
  };
}) (function () {
  return Vue.prototype.$http;
});
