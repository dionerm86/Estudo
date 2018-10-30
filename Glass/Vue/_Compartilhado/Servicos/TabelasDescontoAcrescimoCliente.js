var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de tabelas de desconto/acréscimo.
 */
Servicos.TabelasDescontoAcrescimoCliente = (function (http) {
  const API = '/api/v1/tabelasDescontoAcrescimoCliente/';

  return {
    /**
     * Recupera a lista de tabelas de desconto/acréscimo de cliente para uso no controle de busca.
     * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obter: function (filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    },

    /**
     * Remove uma tabela de desconto/acréscimo de cliente.
     * @param {!number} id O identificador da tabela de desconto/acréscimo de cliente que será excluída.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (id) {
      if (!id) {
        throw new Error('Tabela de desconto/acréscimo de cliente é obrigatória.');
      }

      return http().delete(API + id);
    },

    /**
     * Insere uma tabela de desconto/acréscimo de cliente.
     * @param {!Object} tabelaDescontoAcrescimoCliente O objeto com os dados da tabela de desconto/acréscimo de cliente a ser inserida.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (tabelaDescontoAcrescimoCliente) {
      return http().post(API, tabelaDescontoAcrescimoCliente);
    },

    /**
     * Altera os dados de uma tabela de desconto/acréscimo de cliente.
     * @param {!number} id O identificador da tabela de desconto/acréscimo de cliente que será alterada.
     * @param {!Object} tipoCliente O objeto com os dados da tabela de desconto/acréscimo de cliente a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (id, tabelaDescontoAcrescimoCliente) {
      if (!id) {
        throw new Error('Tabela de desconto/acréscimo de cliente é obrigatória.');
      }

      if (!tabelaDescontoAcrescimoCliente || tabelaDescontoAcrescimoCliente === {}) {
        return Promise.resolve();
      }

      return http().patch(API + id, tabelaDescontoAcrescimoCliente);
    },

    /**
     * Recupera a lista de tabelas de desconto/acréscimo para uso no controle de seleção.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterFiltro: function () {
      return http().get(API + 'filtro');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
