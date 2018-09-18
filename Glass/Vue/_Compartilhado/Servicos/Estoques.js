var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de estoques.
 */
Servicos.Estoques = (function(http) {
  const API = '/api/v1/estoques/';

  return {
    /**
     * Recupera a lista de estoques de produto.
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