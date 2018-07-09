Vue.component('campo-altura', {
  inheritAttrs: false,
  props: {
    /**
     * Altura que é exibida pelo controle.
     * Pode ser a altura real ou para cálculo, dependendo do
     * valor da propriedade exibirAlturaReal.
     * @see exibirAlturaReal
     */
    alturaParaExibir: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Altura que não é exibida pelo controle, mas que também é calculada.
     * Pode ser a altura real ou para cálculo, dependendo do
     * valor da propriedade exibirAlturaReal.
     * @see exibirAlturaReal
     */
    alturaOculta: {
      required: false,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica se a altura pode ser decimal ou não.
     * @type {?boolean}
     */
    permiteDecimal: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Indica se a altura pode ser editada ou não.
     * @type {?boolean}
     */
    permiteEditar: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Indica se o controle deve exibir a altura real ou para cálculo.
     * @type {?boolean}
     */
    exibirAlturaReal: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBoolean
    },

    /**
     * Fator de arredondamento para o cálculo de altura (alumínio).
     * @type {?number}
     */
    fatorArredondamento: {
      required: false,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarNumeroOuVazio
    }
  },

  data: function() {
    return {
      alturaParaExibirAtual: this.alturaParaExibir || 0
    };
  },

  methods: {
    /**
     * Atualiza os campos de altura do controle.
     */
    calcularAlturas: function() {
      var valor = this.alturaParaExibirAtual;
      var alturaParaExibir = null;
      var alturaOculta = null;

      if (this.exibindoAlturaReal) {
        if (valor !== this.alturaParaExibir) {
          alturaParaExibir = valor;
          alturaOculta = this.calcularAlturaParaCalculo(valor);
        }
      } else if (valor !== this.alturaOculta) {
        alturaParaExibir = this.calcularAlturaParaCalculo(valor);
        alturaOculta = valor;
        this.alturaParaExibirAtual = alturaParaExibir;
      }

      if (alturaParaExibir && alturaOculta) {
        this.$emit('update:alturaParaExibir', alturaParaExibir);
        this.$emit('update:alturaOculta', alturaOculta);
      }
    },

    /**
     * Calcula a altura (para cálculo) a partir da altura.
     * @param {number} alturaReal O valor da altura real do objeto.
     * @returns {number} O valor da altura calculada.
     */
    calcularAlturaParaCalculo: function(alturaReal) {
      if (this.fatorArredondamento) {
        var alturaInteira = Math.floor(alturaReal);
        var decimosAltura = alturaReal - alturaInteira;

        if (this.fatorArredondamento < 6) {
          if (decimosAltura > 0 && decimosAltura < this.fatorArredondamento) {
            return alturaInteira + this.fatorArredondamento;
          } else if (decimosAltura > this.fatorArredondamento) {
            return alturaInteira + (this.fatorArredondamento * 2);
          }
        }
        else if (altura < 6) {
          return 6;
        }
      }

      return alturaReal;
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o valor do incremento do campo altura.
     * @type {number}
     */
    incremento: function() {
      return this.permiteDecimal ? 0.01 : 1;
    }
  },

  watch: {
    /**
     * Observador para a propriedade 'alturaParaExibir'.
     * Atualiza o valor da altura do controle.
     */
    alturaParaExibir: function() {
      this.alturaParaExibirAtual = this.alturaParaExibir || 0;
    }
  },

  template: '#CampoAltura-template'
});
