var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de estoques.
 */
Servicos.Estoques = (function(http) {
  const API = '/api/v1/estoques/';
  const API_Troca = API + 'trocasDevolucoes/';
  const API_Troca_Origens = API_Troca + 'origens/';

  return {
    TrocasDevolucoes: {
      Origens: {
        /**
         * Recupera a lista de origens para uso no controle de filtros.
         * @returns {Promise} Uma promise com o resultado da busca.
         */
        obterParaFiltro: function () {
          return http().get(API_Troca_Origens + 'filtro');
        }
      },

      /**
       * Recupera a lista de trocas/devoluções.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterListaTrocaDevolucao: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API_Troca.substr(0, API_Troca.length - 1), {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Recuprea as configurações para a tela de listagem de trocas/devoluções.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterConfiguracoesLista: function () {
        return http().get(API_Troca + 'configuracoes');
      }
    },

    /**
     * Recupera a lista de estoques de produto.
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
     * Recupera o objeto com as configurações utilizadas na tela de listagem de estoques de produto.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function() {
      return http().get(API + 'configuracoes');
    },

    /**
     * Altera os dados do estoque real de um produto.
     * @param {!number} idProduto O identificador do produto que terá o estoque alterado.
     * @param {!number} idLoja O identificador da loja do produto que terá o estoque alterado.
     * @param {!Object} estoqueProduto O objeto com os dados de um estoque de produto a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizarEstoqueReal: function (idProduto, idLoja, estoqueProduto) {
      if (!idProduto) {
        throw new Error('Produto é obrigatório.');
      }

      if (!idLoja) {
        throw new Error('Loja é obrigatória.');
      }

      if (!estoqueProduto || estoqueProduto === {}) {
        return Promise.resolve();
      }

      return http().patch(API + 'produto/' + idProduto + '/loja/' + idLoja + '/real', estoqueProduto);
    },

    /**
     * Altera os dados do estoque fiscal de um produto.
     * @param {!number} idProduto O identificador do produto que terá o estoque alterado.
     * @param {!number} idLoja O identificador da loja do produto que terá o estoque alterado.
     * @param {!Object} estoqueProduto O objeto com os dados do estoque de produto a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizarEstoqueFiscal: function (idProduto, idLoja, estoqueProduto) {
      if (!idProduto) {
        throw new Error('Produto é obrigatório.');
      }

      if (!idLoja) {
        throw new Error('Loja é obrigatória.');
      }

      if (!estoqueProduto || estoqueProduto === {}) {
        return Promise.resolve();
      }

      return http().patch(API + 'produto/' + idProduto + '/loja/' + idLoja + '/fiscal', estoqueProduto);
    },

    /**
     * Altera os dados do estoque real de vários produtos.
     * @param {!number} idProduto O identificador do produto que terá o estoque alterado.
     * @param {!number} idLoja O identificador da loja do produto que terá o estoque alterado.
     * @param {!Object} quantidadeEstoqueReal O objeto com os dados de estoque real do produto.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizarEstoqueRealCampoUnico: function (idProduto, idLoja, quantidadeEstoqueReal) {
      if (!idProduto) {
        throw new Error('Produto é obrigatório.');
      }

      if (!idLoja) {
        throw new Error('Loja é obrigatória.');
      }

      return http().patch(API + 'produto/' + idProduto + '/loja/' + idLoja + '/atualizacaoRapidaEstoqueReal', {
        quantidadeEstoque: quantidadeEstoqueReal
      });
    },

    /**
     * Altera os dados do estoque fiscal de vários produtos.
     * @param {!number} idProduto O identificador do produto que terá o estoque alterado.
     * @param {!number} idLoja O identificador da loja do produto que terá o estoque alterado.
     * @param {!Object} quantidadeEstoqueReal O objeto com os dados de estoque fiscal do produto.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizarEstoqueFiscalCampoUnico: function (idProduto, idLoja, quantidadeEstoqueFiscal) {
      if (!idProduto) {
        throw new Error('Produto é obrigatório.');
      }

      if (!idLoja) {
        throw new Error('Loja é obrigatória.');
      }

      return http().patch(API + 'produto/' + idProduto + '/loja/' + idLoja + '/atualizacaoRapidaEstoqueFiscal', {
        quantidadeEstoqueFiscal: quantidadeEstoqueFiscal
      });
    }
  };
}) (function () {
  return Vue.prototype.$http;
});
