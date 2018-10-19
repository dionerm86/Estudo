var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de fornecedores.
 */
Servicos.Fornecedores = (function (http) {
  const API = '/api/v1/fornecedores/';

  return {
    /**
     * Recupera a lista de fornecedores.
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
     * Recupera o objeto com as configurações utilizadas na tela de listagem de fornecedores.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function() {
      return http().get(API + 'configuracoes');
    },

    /**
     * Remove um fornecedor.
     * @param {!number} idFornecedor O identificador do fornecedor que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idFornecedor) {
      if (!idFornecedor) {
        throw new Error('Fornecedor é obrigatório.');
      }

      return http().delete(API + idFornecedor);
    },

    /**
     * Altera a situação do fornecedor.
     * @param {!number} idFornecedor O identificador do fornecedor que será alterada a situação.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    alterarSituacao: function (idFornecedor) {
      if (!idFornecedor) {
        throw new Error('Fornecedor é obrigatório.');
      }

      return http().post(API + idFornecedor + '/alterarSituacao');
    },

    /**
     * Recupera a lista de situações de fornecedor para um controle.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
