var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de notas fiscais.
 */
Servicos.Cfops = (function(http) {
  const API = '/api/v1/cfops/';

  return {
    /**
     * Objeto com os serviços para a API de naturezas de operação.
     */
    NaturezasOperacao: {
      /**
       * Objeto com os serviços para a API de regras de natureza de operação.
       */
      Regras: {
        /**
         * Recupera a lista de regras de natureza de operação.
         * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
         * @param {number} pagina O número da página de resultados a ser exibida.
         * @param {number} numeroRegistros O número de registros que serão exibidos na página.
         * @param {string} ordenacao A ordenação para o resultado.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
          return http().get(API + 'naturezasOperacao/regras', {
            params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
          });
        },

        /**
         * Recupera as configurações para a tela de regras de natureza de operação.
         * @returns {Promise} Uma promise com o resultado da busca.
         */
        obterConfiguracoesLista: function () {
          return http().get(API + 'naturezasOperacao/regras/configuracoes');
        },

        /**
         * Remove uma regra de natureza de operação.
         * @param {!number} id O identificador da regras de natureza de operação que será excluída.
         * @param {string} motivo O motivo da exclusão.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        excluir: function (id, motivo) {
          if (!id) {
            throw new Error('Regra de natureza de operação é obrigatória.');
          }

          var cancelamento = {
            motivo
          };

          return http().post(API + 'naturezasOperacao/regras/excluir/' + id, cancelamento);
        },

        /**
         * Insere uma regra de natureza de operação.
         * @param {!Object} regra O objeto com os dados da regra de natureza de operação a ser inserida.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        inserir: function (regra) {
          return http().post(API + 'naturezasOperacao/regras', regra);
        },

        /**
         * Altera os dados de uma regra de natureza de operação.
         * @param {!number} id O identificador do item que será alterado.
         * @param {!Object} regra O objeto com os dados do item a serem alterados.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        atualizar: function (id, regra) {
          if (!id) {
            throw new Error('Regra de natureza de operação é obrigatória.');
          }

          if (!regra || regra === {}) {
            return Promise.resolve();
          }

          return http().patch(API + 'naturezasOperacao/regras/' + id, regra);
        }
      },

      /**
       * Recupera a lista de naturezas de operação.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'naturezasOperacao', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Remove uma natureza de operação.
       * @param {!number} id O identificador do item que será excluído.
       * @param {string} motivo O motivo da exclusão.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (id) {
        if (!id) {
          throw new Error('Natureza de operação é obrigatória.');
        }

        return http().delete(API + 'naturezasOperacao/' + id);
      },

      /**
       * Insere uma natureza de operação.
       * @param {!Object} naturezaOperacao O objeto com os dados da natureza de operação a ser inserida.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (naturezaOperacao) {
        return http().post(API + 'naturezasOperacao', naturezaOperacao);
      },

      /**
       * Altera os dados de uma regra de natureza de operação.
       * @param {!number} id O identificador do item que será alterado.
       * @param {!Object} naturezaOperacao O objeto com os dados do item a serem alterados.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (id, naturezaOperacao) {
        if (!id) {
          throw new Error('Regra de natureza de operação é obrigatória.');
        }

        if (!naturezaOperacao || naturezaOperacao === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'naturezasOperacao/' + id, naturezaOperacao);
      },

      /**
       * Retorna os itens para o controle de CSOSN's.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterCsosns: function () {
        return http().get(API + 'naturezasOperacao/csosns');
      },

      /**
       * Retorna os itens para o controle de CST's de ICMS.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterCstsIcms: function () {
        return http().get(API + 'naturezasOperacao/cstsIcms');
      },

      /**
       * Retorna os itens para o controle de CST's de IPI.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterCstsIpi: function () {
        return http().get(API + 'naturezasOperacao/cstsIpi');
      },

      /**
       * Retorna os itens para o controle de CST's de PIS/COFINS.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterCstsPisCofins: function () {
        return http().get(API + 'naturezasOperacao/cstsPisCofins');
      },

      /**
       * Recupera a lista de naturezas de operação para uso no controle de busca.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'naturezasOperacao/filtro');
      }
    },

    /**
     * Recupera a lista de cfops.
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
     * Recupera o objeto com as configurações utilizadas na tela de listagem de CFOP.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes');
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
    obterTiposCfop: function () {
      return http().get(API + 'tipos');
    },

    /**
     * Retorna os itens para o controle de tipos de mercadoria.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposMercadoria: function () {
      return http().get(API + 'tiposMercadoria');
    },

    /**
     * Insere um CFOP.
     * @param {!Object} cfop O objeto com os dados do CFOP a ser inserido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (cfop) {
      return http().post(API, cfop);
    },

    /**
     * Altera os dados de um CFOP.
     * @param {!number} idCfop O identificador do item que será alterado.
     * @param {!Object} cfop O objeto com os dados do item a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (idCfop, cfop) {
      if (!idCfop) {
        throw new Error('CFOP é obrigatório.');
      }

      if (!cfop || cfop === {}) {
        return Promise.resolve();
      }

      return http().patch(API + idCfop, cfop);
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

      return http().delete(API + idCfop);
    }
  };
}) (function () {
  return Vue.prototype.$http;
});
