var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de notas fiscais.
 */
Servicos.Cfops = (function(http) {
  const API = '/api/v1/cfops/';

  return {
    /**
     * Recupera a lista de cfops.
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
     * Recupera o objeto com as configurações utilizadas na tela de listagem de CFOP.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'cfops/configuracoes');
    },

    /**
     * Retorna os itens para filtro de cfops.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterFiltro: function () {
      return http().get(API + 'filtro');
    },

    /**
     * Retorna os itens para o controle de tipos de cfop.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTipos: function () {
      return http().get(API + 'tipos');
    },

    /**
     * Insere um CFOP.
     * @param {!Object} cfop O objeto com os dados do CFOP a ser inserido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (cfop) {
      return http().post(API + 'cfops', cfop);
    },

    /**
     * Altera os dados de um CFOP.
     * @param {!number} idCfop O identificador do item que será alterado.
     * @param {!Object} grupoProduto O objeto com os dados do item a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (idCfop, cfop) {
      if (!idCfop) {
        throw new Error('CFOP é obrigatório.');
      }

      if (!cfop || cfop === {}) {
        return Promise.resolve();
      }

      return http().patch(API + 'cfops/' + idCfop, cfop);
    },

    /**
     * Remove um CFOP.
     * @param {!number} idCfop O identificador do CFOP que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idCfop) {
      if (!idCfop) {
        throw new Error('CFOP é obrigatório.');
      }

      return http().delete(API + 'cfops/' + idCfop);
    }
  };
}) (function () {
  return Vue.prototype.$http;
});
