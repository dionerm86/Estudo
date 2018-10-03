var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de datas.
 */
Servicos.Datas = (function(http) {
  const API = '/api/v1/datas/';

  return {
    /**
     * Objeto com os serviços para a API de feriados.
     */
    Feriados: {
      /**
       * Recupera a lista de feriados para uso no controle de busca.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        filtro = filtro || {};
        filtro.pagina = pagina;
        filtro.numeroRegistros = numeroRegistros;
        filtro.ordenacao = ordenacao;

        return http().get(API + 'feriados', {
          params: filtro
        });
      },

      /**
       * Remove um feriado.
       * @param {!number} idFeriado O identificador do feriado que será excluído.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (idFeriado) {
        if (!idFeriado) {
          throw new Error('Feriado é obrigatório.');
        }

        return http().delete(API + 'feriados/' + idFeriado);
      },

      /**
       * Insere uma cor de vidro.
       * @param {!Object} feriado O objeto com os dados do feriado a ser inserido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (feriado) {
        return http().post(API + 'feriados', feriado);
      },

      /**
       * Altera os dados de um feriado.
       * @param {!number} idFeriado O identificador do feriado que será alterado.
       * @param {!Object} feriado O objeto com os dados do feriado a serem alterados.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (idFeriado, feriado) {
        if (!idFeriado) {
          throw new Error('Feriado é obrigatório.');
        }

        if (!feriado || feriado === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'feriados/' + idFeriado, feriado);
      }
    },

    /**
     * Valida se uma data pode ser selecionada.
     * @param {!Date} data A data a ser validada.
     * @param {?boolean} [permitirFimDeSemana=true] Indica se dias no fim-de-semana podem ser selecionados.
     * @param {?boolean} [permitirFeriado=true] Indica se dias de feriado podem ser selecionados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    validar: function (data, permitirFimDeSemana, permitirFeriado) {
      return http().post(API + 'validar', {
        data,
        permitirFimDeSemana: permitirFimDeSemana || true,
        permitirFeriado: permitirFeriado || true
      });
    }
  };
})(function() {
  return Vue.prototype.$http;
});
