var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de integração.
 */
Servicos.Integracao = (function(http) {
  const API = '/api/v1/integracao/';

  return {
    /**
     * Objeto com os serviços para a API de integradores.
     */
    Integradores: {
      /**
       * Recupera a lista de integradores.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterLista: function () {
        return http().get(API + 'integradores');
      },

      /**
       * Obtém o logger do integrador.
       * @param {string} integrador Nome do integrador de onde o logger será recuperado.
       * @returns {Promise} Um promise com os dados do Logger.
       **/
      obterLogger: function (integrador) {
        if (!integrador) {
          throw new Error('O integrador deve ser informado.');
        }

        return http().get(API + 'integradores/' + integrador + '/logger');
      },

      /**
       * Obtém os itens do histórico do integrador.
       * @param {string} integrador Nome do integrador.
       * @param {string} itemEsquema Nome do item do esquema de histórico.
       * @param {string} tipo Tipo dos itens que serão filtrados.
       * @param {Array} identificadores Identificadores que serão usados para filtrar os itens.
       * @returns {Promise} Um promise com os dados dos itens do histórico.
       **/
      obterItensHistorico: function (integrador, itemEsquema, tipo, identificadores) {
        if (!integrador) {
          throw new Error('O integrador deve ser informado.');
        }

        if (!itemEsquema) {
          throw new Error('O item do esquema de histório deve ser informado.');
        }

        if (!identificadores) {
          identificadores = [];
        }

        return http().post(API + 'integradores/' + integrador + '/historico/' + itemEsquema + '/itens', {
          tipo: tipo,
          identificadores: identificadores,
        });
      },

      /**
       * Executa a operação de integração.
       * @param {string} integrador Nome do integrador onde a operação será executada.
       * @param {string} operacao Nome da operação que será executada.
       * @param {Object} parametros Coleção dos parametros que serão usados na execução.
       * @returns {Promise} Um promise com o resultado da operação.
       **/
      executarOperacao: function (integrador, operacao, parametros) {

        if (!integrador) {
          throw new Error('O integrador deve ser informado.');
        }

        if (!operacao) {
          throw new Error('O nome da operação deve ser informado.');
        }

        return http().post(API + 'integradores/' + integrador + '/executarOperacao', {
          operacao: operacao,
          parametros: parametros
        });
      },

      /**
       * Executa o job do integrador.
       * @param {string} integrador Nome do integrado onde o job será executado.
       * @param {string} job Nome do job que será executado.
       * @returns {Promise} Um promise da execução.
       **/
      executarJob: function (integrador, job) {

        if (!integrador) {
          throw new Error('O integrador deve ser informado.');
        }

        if (!job) {
          throw new Error('O nome do job deve ser informado.');
        }

        return http().put(API + 'integradores/' + integrador + '/executarJob/' + job);
      }
    },
  };
})(function() {
  return Vue.prototype.$http;
});
