var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de clientes.
 */
Servicos.Clientes = (function(http) {
  const API = '/api/v1/clientes/';

  return {
    /**
     * Recupera a lista de clientes para uso no controle de busca.
     * @param {?number} [idCliente=null] O identificador do cliente para filtro na busca.
     * @param {?string} [nomeCliente=null] O nome do cliente para filtro na busca.
     * @param {?string} [tipoValidacao=null] O tipo de validação que será feita na busca de clientes.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function (idCliente, nomeCliente, tipoValidacao) {
      return http().get(API + 'filtro', {
        params: {
          id: idCliente,
          nome: nomeCliente,
          tipoValidacao
        }
      });
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de clientes.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes/lista');
    },

    /**
     * Retorna os itens para o controle de situações de cliente.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Retorna os itens para o controle de tipos de cliente.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTipos: function () {
      return http().get(API + 'tipos');
    },

    /**
     * Retorna os itens para o controle de tipos fiscal de cliente.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposFiscal: function () {
      return http().get(API + 'tiposFiscal');
    },

    /**
     * Remove um cliente.
     * @param {!number} idCliente O identificador do cliente que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idCliente) {
      if (!idCliente) {
        throw new Error('Cliente é obrigatório.');
      }

      return http().delete(API + idCliente);
    },

    /**
     * Altera a situação do cliente.
     * @param {!number} idCliente O identificador do cliente que será alterada a situação.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    alterarSituacao: function (idCliente) {
      if (!idCliente) {
        throw new Error('Cliente é obrigatório.');
      }

      return http().post(API + idCliente + '/alterarSituacao');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
