var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de processos (etiqueta).
 */
Servicos.Processos = (function(http) {
  const API = '/api/v1/processos/';

  return {
    /**
     * Recupera a lista de processos (etiqueta) para uso no controle de busca.
     * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obter: function (filtro, pagina, numeroRegistros, ordenacao) {
      filtro = filtro || {};
      filtro.pagina = pagina;
      filtro.numeroRegistros = numeroRegistros;
      filtro.ordenacao = ordenacao;

      return http().get(API.substr(0, API.length - 1), {
        params: filtro
      });
    },

    /**
     * Remove um processo de etiqueta.
     * @param {!number} idProcesso O identificador do processo de etiqueta que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function(idProcesso) {
      if (!idProcesso) {
        throw new Error('Processo é obrigatório.');
      }

      return http().delete(API + idProcesso);
    },

    /**
     * Insere um processo de etiqueta.
     * @param {!Object} processo O objeto com os dados do processo a ser inserido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (processo) {
      return http().post(API.substr(0, API.length - 1), processo);
    },

    /**
     * Altera os dados de um processo de etiqueta.
     * @param {!number} idProcesso O identificador do processo de etiqueta que será alterado.
     * @param {!Object} processo O objeto com os dados do processo a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (idProcesso, processo) {
      if (!idProcesso) {
        throw new Error('Processo é obrigatório.');
      }

      if (!processo || processo === {}) {
        return Promise.resolve();
      }

      return http().patch(API + idProcesso, processo);
    },

    /**
     * Recupera as configurações para a tela de lista de processos.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoes: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Recupera os tipos de processo para o controle.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterTiposProcesso: function () {
      return http().get(API + 'tipos');
    },

    /**
     * Recupera as situações processo para o controle.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Recupera a lista de processos (etiqueta) para uso no controle de busca.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function () {
      return http().get(API + 'filtro');
    },

    /**
     * Realiza a validação do processo para os subgrupos informados.
     * @param {!number} idProcesso O identificador do processo de etiqueta que será validado.
     * @param {number[]} idsSubgruposParaValidar Os identificadores dos subgrupos que serão validados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    validarSubgrupos: function (idProcesso, idsSubgruposParaValidar) {
      if (!idProcesso) {
        throw new Error('Processo é obrigatório.');
      }

      if (!idsSubgruposParaValidar || !idsSubgruposParaValidar.length) {
        return Promise.resolve();
      }

      return http().get(API + idProcesso + '/validarSubgrupos', {
        params: {
          idsSubgrupos: idsSubgruposParaValidar
        }
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
