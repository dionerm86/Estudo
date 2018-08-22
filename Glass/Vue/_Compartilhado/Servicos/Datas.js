var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de datas.
 */
Servicos.Datas = (function(http) {
  const API = '/api/v1/datas/';

  return {
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
