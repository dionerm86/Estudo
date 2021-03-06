var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de caixas (diário e geral).
 */
Servicos.Caixas = (function(http) {
  const API = '/api/v1/caixas/';

  return {
    /**
     * Objeto com os serviços para a API de caixa diário.
     */
    Diario: {
      /**
       * Recupera a lista de movimentações do caixa diário.
       * @param {?Object} filtro Objeto com os filtros a serem usados para a busca das movimentações do caixa diário.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API.substr(0, API.length - 1) + '/diario', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Recupera o objeto com as configurações utilizadas na tela de listagem de movimentações do caixa diário.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterConfiguracoesLista: function () {
        return http().get(API + 'diario/configuracoes');
      },

      /**
       * Recupera o objeto com dados necessários para fechamento do caixa.
       * @param {!idLoja} idLoja O identificador da loja.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterDadosFechamento: function (idLoja) {
        if (!idLoja) {
          throw new Error('Loja é obrigatória.');
        }

        return http().get(API + 'diario/' + idLoja + '/saldoFechamento');
      },

      /**
       * Fecha o caixa.
       * @param {!idLoja} idLoja O identificador da loja que terá o caixa fechado.
       * @param {!Object} dadosFechamento Os dados necessários para fechamento do caixa.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      fechar: function (idLoja, dadosFechamento) {
        if (!idLoja) {
          throw new Error('Loja é obrigatória.');
        }

        if (!dadosFechamento) {
          throw new Error('Dados do fechamento são obrigatórios.');
        }

        return http().post(API + 'diario/' + idLoja + '/fechar', dadosFechamento);
      },

      /**
       * Reabre o caixa.
       * @param {!idLoja} idLoja O identificador da loja que terá o caixa reaberto.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      reabrir: function (idLoja) {
        if (!idLoja) {
          throw new Error('Loja é obrigatória.');
        }

        return http().post(API + 'diario/' + idLoja + '/reabrir');
      }
    },

    /**
     * Objeto com os serviços para a API de caixa geral.
     */
    Geral: {
      /**
       * Recupera a lista de movimentações do caixa geral.
       * @param {?Object} filtro Objeto com os filtros a serem usados para a busca das movimentações do caixa geral.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API.substr(0, API.length - 1) + '/geral', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Recupera o objeto com as configurações utilizadas na tela de listagem de movimentações do caixa geral.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterConfiguracoesLista: function () {
        return http().get(API + 'geral/configuracoes');
      },

      /**
       * Recupera o objeto com os totais por forma de pagamento.
       * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos totais do caixa geral.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterTotaisPorFormaPagamento: function (filtro) {
        filtro = filtro || {};

        return http().get(API + 'geral/totaisPorFormaPagamento', {
          params: filtro
        });
      },

      /**
       * Recupera os tipos de movimentação do caixa geral.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterTiposMovimentacao: function () {
        return http().get(API + 'geral/tiposMovimentacao');
      }
    }
  };
})(function() {
  return Vue.prototype.$http;
});
