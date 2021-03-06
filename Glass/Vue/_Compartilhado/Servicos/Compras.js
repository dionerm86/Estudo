﻿var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de Compras.
 */
Servicos.Compras = (function (http) {
  const API = '/api/v1/compras/';

  return {
    /**
     * Recupera a lista de compras.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obter: function (filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de compras.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Realiza o cancelamento de uma compra.
     * @param {Object} compra A compra que será cancelada.
     * @param {String} motivo O motivo do cancelamento.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    cancelar: function (compra, motivo) {
      if (!compra) {
        throw new Error('Compra é obrigatória.');
      }

      return http().delete(API + compra, motivo);
    },

    /**
     * Realiza a reabertura de uma compra previamente finalizada.
     * @param {Object} compra A compra que será reaberta.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    reabrir: function (compra) {
      if (!compra) {
        throw new Error('Compra é obrigatória.');
      }

      return http().post(API + compra + '/reabrir');
    },

    /**
     * Realiza a finalização de uma compra.
     * @param {Object} compra A compra que será finalizada.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    finalizar: function (compra) {
      if (!compra) {
        throw new Error('Compra é obrigatória.');
      }

      return http().post(API + compra + '/finalizar');
    },

    /**
     * Recupera a lista de situações para um controle na tela.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoesParaControle: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Objeto com os serviços para a API de compras de mercadorias.
     */
    Mercadorias: {
      /**
       * Recupera a lista de compras de mercadorias.
       * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterListaComprasMercadorias: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'mercadorias', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Realiza a reabertura de uma compra de mercadoria previamente finalizada.
       * @param {!number} idMercadoria A mercadoria que será reaberta.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      reabrir: function (idMercadoria) {
        if (!idMercadoria) {
          throw new Error('Compra da mercadoria é obrigatória.');
        }

        return http().post(API + 'mercadorias/reabrir/' + idMercadoria);
      },

      /**
       * Realiza o cancelamento de uma compra de mercadoria.
       * @param {!number} idMercadoria A mercadoria que será cancelada.
       * @param {String} motivo O motivo do cancelamento.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      cancelar: function (idMercadoria, motivo) {
        if (!idMercadoria) {
          throw new Error('Compra da mercadoria é obrigatória.');
        }

        return http().delete(API + 'mercadorias/' + idMercadoria, motivo);
      },

      /**
       * Gera uma nota fiscal para a compra de mercadoria.
       * @param {!number} idMercadoria A compra de mercadoria que será gerada a nota fiscal.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      gerarNotaFiscal: function (idMercadoria) {
        if (!idMercadoria) {
          throw new Error('Compra de mercadoria é obrigatória.');
        }

        return http().post(API + 'mercadorias/gerarNotaFiscal/' + idMercadoria)
      }
    }
  };
})(function () {
  return Vue.prototype.$http;
});
