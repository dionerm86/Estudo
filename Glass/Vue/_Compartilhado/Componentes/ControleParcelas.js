Vue.component('controle-parcelas', {
  inheritAttrs: false,
  mixins: [Mixins.Data, Mixins.Comparar, Mixins.Clonar],
  props: {
    /**
     * Objeto de parcelas
     * @type {!Object}
     */
    parcelas: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Total a ser distribuído entre as parcelas
     * @type {!number}
     */
    total: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Data mínima possível para a primeira parcela
     * @type {!Date}
     */
    dataMinima: {
      required: true,
      twoWay: false,
      default: function () {
        return new Date()
      },
      validator: Mixins.Validacao.validarDataOuVazio
    },

    /**
     * Define se se será possível alterar as datas das parcelas
     * @type {?bool}
     */
    podeEditar: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Indica o número de parcelas a ser exibida por linha no controle.
     * @type {?number}
     */
    numeroParcelasPorLinha: {
      required: false,
      twoWay: false,
      default: 6,
      validator: Mixins.Validacao.validarNumeroOuVazio
    }
  },

  data: function () {
    return {
      parcelasAtuais: (this.parcelas || {}).detalhes || [],
      controlesValor: null,
      invalido: false
    }
  },

  methods: {
    /**
     * Preenche o valor, data e data mínima das parcelas.
     */
    calcularParcelas: function () {
      if (this.verificarDeveCalcular()) {
        this.parcelasAtuais = [];

        for (var i = 0; i < this.parcelas.numeroParcelas; i++) {
          this.parcelasAtuais.push({
            valor: this.calcularValorParcela(this.parcelas.numeroParcelas == i + 1),
            data: this.calcularDataParcela(i)
          });
        }
      }

      for (var i = 0; i < this.parcelas.numeroParcelas; i++) {
        this.parcelasAtuais[i].dataMinima = i === 0
          ? this.dataMinima
          : this.adicionarDias(this.parcelasAtuais[i - 1].data, 1);
      }

      this.validarTotal();
    },

    /**
     * Verifica se as parcelas devem ser recalculadas
     * @returns {boolean} Um valor que indica se as parcelas devem ser calculadas
     */
    verificarDeveCalcular: function () {
      return this.parcelasAtuais === null
        || this.parcelasAtuais.length !== this.parcelas.numeroParcelas
        || this.total.toFixed(2) !== this.totalParcelas.toFixed(2);
    },

    /**
     * Calcula o valor de cada parcela, jogando a diferença na última parcela
     * @param {boolean} ultima Identifica se é a última parcela sendo calculada
     */
    calcularValorParcela: function (ultima) {
      var valorParcelaNormalizado;

      const arredondar = function (valor) {
        return parseFloat((valor + 0.001).toFixed(2));
      }

      if (!ultima) {
        var valorParcelaBruto = this.total / this.parcelas.numeroParcelas;
        valorParcelaNormalizado = arredondar(valorParcelaBruto);
      } else {
        var totalParcelasAnteriores = this.parcelasAtuais
          .slice(0, this.parcelasAtuais.length - 2)
          .reduce(function (acumulador, parcela) {
            return acumulador + parcela.valor;
          }, 0);

        valorParcelaNormalizado = this.total - totalParcelasAnteriores;
      }

      return arredondar(valorParcelaNormalizado);
    },

    /**
     * Calcula a data da parcela
     * @param {!number} posicao Posição da parcela no controle
     * @returns {Date} Diferença em dias que esta parcela deve ter da anterior ou da data base
     */
    calcularDataParcela: function (posicao) {
      var dias = 30 * posicao;
      if (this.parcelas.dias && this.parcelas.dias.length >= posicao) {
        dias = parseInt(this.parcelas.dias[posicao]);
      }

      return this.adicionarDias(new Date(this.dataMinima), dias);
    },

    /**
     * Retorna o estilo que será aplicado às parcelas.
     * @param {!number} indice O número da parcela que está sendo desenhada.
     * @returns {Object} Um objeto com os estilos para a parcela.
     */
    obterEstiloParcela: function (indice) {
      var estilo = {
        padding: '1px 3px'
      };

      if (indice === 0) {
        estilo.paddingLeft = '0';
      } else if (indice === this.parcelas.numeroParcelas - 1) {
        estilo.paddingRight = '0';
      }

      return estilo;
    },

    /**
     * Realiza a validação do valor informado nas parcelas com o valor total.
     */
    validarTotal: function () {
      this.invalido = this.total !== this.totalParcelas;
      
      if (this.controlesValor === null) {
        this.controlesValor = this.$el.children[0].querySelector('input[type=number]');
      }

      var mensagem = this.invalido
        ? 'Valor inválido.'
        : '';

      for (var controleValor of this.controlesValor) {
        controleValor.setCustomValidity(mensagem);
      }
    }
  },

  mounted: function () {
    this.calcularParcelas();
  },

  computed: {
    /**
     * Propriedade computada com o estilo para o controle de parcelas. 
     */
    estiloControle: function () {
      return {
        display: 'inline-grid',
        gridTemplateColumns: 'repeat(' + this.numeroParcelasPorLinha + ', max-content)',
        gridGap: '4px'
      };
    },

    /**
     * Propriedade computada com o valor mínimo aceitável por parcela.
     */
    valorMinimoParcela: function () {
      return this.total > 0 ? 0.01 : 0;
    },

    /**
     * Propriedade computada que retorna o valor total das parcelas atuais do controle.
     */
    totalParcelas: function () {
      var total = this.parcelasAtuais.reduce(function (acumulador, parcela) {
        return acumulador + parcela.valor;
      }, 0);

      return parseFloat((total + 0.001).toFixed(2));
    }
  },

  watch: {
    /**
     * Observador para a variável 'total'.
     * Recalcula as parcelas se o total for alterado.
     */
    total: function () {
      this.calcularParcelas();
    },

    /**
     * Observador para a variável 'parcelas'.
     * Recalcula as parcelas internas do controle, se necessário.
     */
    parcelas: {
      handler: function (atual) {
        this.controlesValor = null;
        this.parcelasAtuais = (atual || {}).detalhes || [];
        this.calcularParcelas();
      },
      deep: true
    },

    /**
     * Observador para a variável 'parcelasAtuais'.
     * Atualiza a propriedade com as parcelas se houver alteração nos dados.
     */
    parcelasAtuais: {
      handler: function (atual) {
        var parcelas = this.parcelas || {};

        if (!this.equivalentes(atual, parcelas.detalhes)) {
          var novo = this.clonar(parcelas);
          novo.detalhes = atual || [];

          this.$emit('update:parcelas', novo);
        }
      },
      deep: true
    }
  },

  template: '#ControleParcelas-template'
});
