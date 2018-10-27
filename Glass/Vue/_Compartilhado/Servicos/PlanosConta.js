var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de planos de conta.
 */
Servicos.PlanosConta = (function(http) {
  const API = '/api/v1/planosConta/';

  return {
    /**
     * Objeto com os serviços para a API de grupos de conta.
     */
    Grupos: {
      /**
       * Objeto com os serviços para a API de categorias de conta.
       */
      Categorias: {
        /**
         * Recupera a lista de categorias de conta.
         * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
         * @param {number} pagina O número da página de resultados a ser exibida.
         * @param {number} numeroRegistros O número de registros que serão exibidos na página.
         * @param {string} ordenacao A ordenação para o resultado.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
          return http().get(API + 'grupos/categorias', {
            params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
          });
        },

        /**
         * Remove uma categoria de conta.
         * @param {!number} id O identificador da categoria de conta que será excluída.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        excluir: function (id) {
          if (!id) {
            throw new Error('Categoria de conta é obrigatória.');
          }

          return http().delete(API + 'grupos/categorias/' + id);
        },

        /**
         * Insere uma categoria de conta.
         * @param {!Object} categoriaConta O objeto com os dados da categoria de conta a ser inserida.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        inserir: function (categoriaConta) {
          return http().post(API + 'grupos/categorias/', categoriaConta);
        },

        /**
         * Altera os dados de uma categoria de conta.
         * @param {!number} id O identificador do item que será alterado.
         * @param {!Object} categoriaConta O objeto com os dados do item a serem alterados.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        atualizar: function (id, categoriaConta) {
          if (!id) {
            throw new Error('Categoria de conta é obrigatório.');
          }

          if (!categoriaConta || categoriaConta === {}) {
            return Promise.resolve();
          }

          return http().patch(API + 'grupos/categorias/' + id, categoriaConta);
        },

        /**
         * Altera a posição de uma categoria de conta.
         * @param {!number} id O identificador do item que será alterado.
         * @param {!boolean} acima Define se o setor será movimentado para cima.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        alterarPosicao: function (id, acima) {
          if (!id) {
            throw new Error('Categoria de conta é obrigatória.');
          }

          var posicao = {
            acima
            };

          return http().patch(API + 'grupos/categorias/' + id + '/posicao', posicao);
        },

        /**
         * Recupera a lista de tipos de categoria de conta para uso no controle de seleção.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        obterTipos: function () {
          return http().get(API + 'grupos/categorias/tipos');
        },

        /**
         * Recupera a lista de categorias de conta para uso no controle de seleção.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        obterParaControle: function () {
          return http().get(API + 'grupos/categorias/filtro');
        }
      },

      /**
      * Recupera a lista de grupos de conta.
      * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
      * @param {number} pagina O número da página de resultados a ser exibida.
      * @param {number} numeroRegistros O número de registros que serão exibidos na página.
      * @param {string} ordenacao A ordenação para o resultado.
      * @returns {Promise} Uma promise com o resultado da operação.
      */
      obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'grupos', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Remove um grupo de conta.
       * @param {!number} id O identificador do grupo de conta que será excluído.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (id) {
        if (!id) {
          throw new Error('Grupo de conta é obrigatório.');
        }

        return http().delete(API + 'grupos/' + id);
      },

      /**
       * Insere um grupo de conta.
       * @param {!Object} grupoConta O objeto com os dados do grupo de conta a ser inserido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (grupoConta) {
        return http().post(API + 'grupos/', grupoConta);
      },

      /**
       * Altera os dados de um grupo de conta.
       * @param {!number} id O identificador do item que será alterado.
       * @param {!Object} grupoConta O objeto com os dados do item a serem alterados.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (id, grupoConta) {
        if (!id) {
          throw new Error('Grupo de conta é obrigatório.');
        }

        if (!grupoConta || grupoConta === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'grupos/' + id, grupoConta);
      },

      /**
       * Altera a posição de um grupo de conta.
       * @param {!number} id O identificador do item que será alterado.
       * @param {!boolean} acima Define se o setor será movimentado para cima.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      alterarPosicao: function (id, acima) {
        if (!id) {
          throw new Error('Grupo de conta é obrigatório.');
        }

        var posicao = {
          acima
        };

        return http().patch(API + 'grupos/' + id + '/posicao', posicao);
      },

      /**
       * Recupera a lista de grupos de conta para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'grupos/filtro');
      }
    },

    /**
      * Recupera a lista de planos de conta.
      * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
      * @param {number} pagina O número da página de resultados a ser exibida.
      * @param {number} numeroRegistros O número de registros que serão exibidos na página.
      * @param {string} ordenacao A ordenação para o resultado.
      * @returns {Promise} Uma promise com o resultado da operação.
      */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    },

    /**
     * Remove um plano de conta.
     * @param {!number} id O identificador do plano de conta que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (id) {
      if (!id) {
        throw new Error('Plano de conta é obrigatório.');
      }

      return http().delete(API + id);
    },

    /**
     * Insere um plano de conta.
     * @param {!Object} planoConta O objeto com os dados do plano de conta a ser inserido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (planoConta) {
      return http().post(API, planoConta);
    },

    /**
     * Altera os dados de um plano de conta.
     * @param {!number} id O identificador do item que será alterado.
     * @param {!Object} planoConta O objeto com os dados do item a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (id, planoConta) {
      if (!id) {
        throw new Error('Plano de conta é obrigatório.');
      }

      if (!planoConta || planoConta === {}) {
        return Promise.resolve();
      }

      return http().patch(API + id, planoConta);
    },

    /**
     * Recupera a lista de planos de conta para uso no controle de busca.
     * @param {!number} tipo Indica o tipo de planos de conta a serem buscados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function (tipo) {
      if (!tipo) {
        throw new Error('Tipo de plano de conta é obrigatório.');
      }

      return http().get(API + 'filtro', {
        params: {
          tipo
        }
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
