Vue.component('integradores-parametrooperacaointegracao', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Dados da operação de integração associada.
     * @type {Object}
     */
    parametro: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Indica se o parâmetro está em modo somente leitura.
     * @type {Boolean}
     **/
    somenteLeitura: {
      required: false,
      twoWay: false,
      default: true
    }
  },

  template: '#LstIntegradores-ParametroOperacaoIntegracao-template'
});
