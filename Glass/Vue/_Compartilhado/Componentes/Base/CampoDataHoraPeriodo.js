Vue.component('campo-data-hora-periodo', {
  props: {
    /**
     * Data/hora inicial do período.
     * @type {?Date}
     */
    dataHoraInicial: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarDataOuVazio
    },

    /**
     * Data/hora final do período.
     * @type {?Date}
     */
    dataHoraFinal: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarDataOuVazio
    },

    /**
     * Indica se os campos com as horas do período serão exibidos.
     * @type {?boolean}
     */
    exibirHoras: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  computed: {
    /**
     * Propridade computada para uso interno do controle.
     * Retorna a data/hora inicial e dispara o evento para atualização da propriedade
     * em caso de alteração do valor.
     * @type {?Date}
     */
    dataHoraAtualInicial: {
      get: function() {
        return this.dataHoraInicial;
      },
      set: function(valor) {
        if (valor !== this.dataHoraInicial) {
          this.$emit('update:dataHoraInicial', valor);
        }
      }
    },

    /**
     * Propridade computada para uso interno do controle.
     * Retorna a data/hora final e dispara o evento para atualização da propriedade
     * em caso de alteração do valor.
     * @type {?Date}
     */
    dataHoraAtualFinal: {
      get: function() {
        return this.dataHoraFinal;
      },
      set: function(valor) {
        if (valor !== this.dataHoraFinal) {
          this.$emit('update:dataHoraFinal', valor);
        }
      }
    }
  },

  template: '#CampoDataHoraPeriodo-template'
});
