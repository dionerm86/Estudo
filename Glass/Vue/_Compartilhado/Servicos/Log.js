var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de log (alteração e cancelamento).
 */
Servicos.Log = (function(http) {
  const API = '/api/v1/log/';

  return {
    /**
     * Objeto com os métodos para o log de alteração.
     */
    Alteracao: {
      /**
       * Verifica se um item (de uma tabela específica) possui log de alteração.
       * Caso seja informado o campo, verifica se o item possui algum log no campo desejado.
       * @param {!number} idTabela O identificador da tabela do item.
       * @param {!number} idItem O identificador do item.
       * @param {?string} [campo=null] O nome de um campo específico que pode ser verificado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      verificarLogItem: function (idTabela, idItem, campo) {
        return http().get(API + 'alteracao/' + idTabela + '/possuiLog/' + idItem, {
          params: {
            campo
          }
        });
      },

      /**
       * Recupera a lista de tabelas passíveis de log do sistema.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterTabelas: function () {
        return http().get(API + 'alteracao/tabelas')
      }
    },

    /**
     * Objeto com os métodos para o log de cancelamento.
     */
    Cancelamento: {
      /**
       * Verifica se um item (de uma tabela específica) possui log de cancelamento.
       * Caso seja informado o campo, verifica se o item possui algum log no campo desejado.
       * @param {!number} idTabela O identificador da tabela do item.
       * @param {!number} idItem O identificador do item.
       * @param {?string} [campo=null] O nome de um campo específico que pode ser verificado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      verificarLogItem: function (idTabela, idItem, campo) {
        return http().get(API + 'cancelamento/' + idTabela + '/possuiLog/' + idItem, {
          params: {
            campo
            }
        });
      },

      /**
       * Recupera a lista de tabelas passíveis de log de cancelamento do sistema.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterTabelas: function () {
        return http().get(API + 'cancelamento/tabelas')
      }
    }
  };
})(function() {
  return Vue.prototype.$http;
});
