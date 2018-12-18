var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de carregamentos.
 */
Servicos.Carregamentos = (function(http) {
  const API = '/api/v1/carregamentos/';

  return {
    /**
     *Objeto com os serviços para a API de itens.
     */
    Itens: {
      /**
       *Objeto com os serviços para a API de pendencias de carregamentos.
       */
      Pendencias: {
        /**
       * Recupera a lista de carregamentos pendentes.
       * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
        obter: function (filtro, pagina, numeroRegistros, ordenacao) {
          return http().get(API + 'itens/pendencias', {
            params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
          });
        },

        /**
         * Recupera o objeto com as configurações utilizadas na tela de listagem de carregamentos pendentes.
         * @returns {Promise} Uma promise com o resultado da busca.
         */
        obterConfiguracoesLista: function () {
          return http().get(API + 'itens/pendencias/configuracoes');
        },
      },
    },

     /**
     * Objeto com os serviços para a API de ordens de carga.
     */
    OrdensCarga: {
     /**
      * Recupera a lista de ordens de carga.
      * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
      * @param {number} pagina O número da página de resultados a ser exibida.
      * @param {number} numeroRegistros O número de registros que serão exibidos na página.
      * @param {string} ordenacao A ordenação para o resultado.
      * @returns {Promise} Uma promise com o resultado da busca.
      */
      obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'ordensCarga', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Recupera o objeto com as configurações utilizadas na tela de listagem de ordens de carga.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterConfiguracoesLista: function () {
        return http().get(API + 'ordensCarga/configuracoes');
      },

     /**
       * Recupera o objeto com os pedidos associados a uma ordem de carga.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterListaPedidosPorOrdemCarga: function (id) {
        if (!id) {
          throw new Error('Ordem de carga é obrigatória.');
        }

        return http().get(API + 'ordensCarga/' + id + '/pedidos');
      },

      /**
       * Verifica se é possível associar um pedido a uma determinada ordem de carga.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      verificarPermissaoParaAssociarPedidosNaOrdemDeCarga: function (id) {
        if (!id) {
          throw new Error('Ordem de carga é obrigatória.');
        }

        return http().post(API + 'ordensCarga/' + id + '/verificarPermissaoAssociarPedidos');
      },

      /**
       * Exclui uma ordem de carga.
       * @returns {Promise} Uma promise com o resultado da exclusão.
       */
      excluir: function (id) {
        if (!id) {
          throw new Error('Ordem de carga é obrigatória.');
        }

        return http().delete(API + 'ordensCarga/' + id)
      },

      /**
       * Desassocia um pedido de uma ordem de carga.
       * @returns {Promise} Uma promise com o resultado da exclusão.
       */
      desassociarPedidoOrdemCarga: function (id, idPedido) {
        if (!id) {
          throw new Error('Ordem de carga é obrigatória.');
        }

        if (!idPedido) {
          throw new Error('Pedido é obrigatório.');
        }

        return this.http().delete(API + 'ordensCarga/' + id + '/desassociarPedido/' + idPedido);
      },

      /**
       * Obtem uma lista com os tipos de ordem de carga.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterTiposOrdensCarga: function () {
        return this.http().get(API + 'ordensCarga/tipos');
      },

      /**
       * Obtem uma lista com as situações de ordem de carga.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterTiposOrdensCarga: function () {
        return this.http().get(API + 'ordensCarga/situacoes');
      },

      /**
       * Recupera a lista de ordens de carga.
       * @param {number} idCarregamento O identificador do carregamento.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaListaCarregamento: function (idCarregamento) {
        if (!idCarregamento) {
          throw new Error('Carregamento é obrigatório.');
        }

        return http().get(API + idCarregamento + '/ordensCarga');
      },

      /**
       * Desassocia uma ordem de carga de um carregamento.
       * @param {!number} idCarregamento O identificador do carregamento.
       * @param {!number} idOrdemCarga O identificador da ordem de carga que será desassociada.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      desassociarDoCarregamento: function (idCarregamento, idOrdemCarga) {
        if (!idCarregamento) {
          throw new Error('Carregamento é obrigatório.');
        }

        if (!idOrdemCarga) {
          throw new Error('Ordem de carga é obrigatória.');
        }

        return http().delete(API + idCarregamento + '/ordensCarga/' + idOrdemCarga);
      }
    },

    /**
     * Recupera a lista de carregamentos.
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
     * Recupera o objeto com as configurações utilizadas na tela de listagem de carregamentos.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Retorna dados do faturamento do carregamento.
     * @param {!number} id O identificador do carregamento.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterDadosFaturamento: function (id) {
      if (!id) {
        throw new Error('Carregamento é obrigatório.');
      }

      return http().get(API + id + '/dadosFaturamento');
    },

    /**
     * Fatura um carregamento.
     * @param {!Object} id O identificador do carregamento a ser faturado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    faturar: function (id) {
      if (!id) {
        throw new Error('Carregamento é obrigatório.');
      }

      return http().post(API + id + '/faturar');
    },

    /**
     * Altera os dados de um carregamento.
     * @param {!number} id O identificador do item que será alterado.
     * @param {!Object} item O objeto com os dados do item a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (id, item) {
      if (!id) {
        throw new Error('Carregamento é obrigatório.');
      }

      if (!item || item === {}) {
        return Promise.resolve();
      }

      return http().patch(API + id, item);
    },

    /**
     * Remove um carregamento.
     * @param {!number} id O identificador do carregamento que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (id) {
      if (!id) {
        throw new Error('Carregamento é obrigatório.');
      }

      return http().delete(API + id);
    },

    /**
     * Recupera a lista de situações de carregamento.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    }
  };
}) (function () {
  return Vue.prototype.$http;
});
