Vue.component('campo-area-m2', {
  mixins: [Mixins.JsonQuerystring, Mixins.ExecutarTimeout],
  props: {
    /**
     * Área do produto, em m².
     * @type {number}
     */
    areaM2: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Área calculada do produto, em m².
     * @type {number}
     */
    areaCalculadaM2: {
      required: false,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Área calculada do produto, desconsiderando a chapa de vidro, em m².
     * @type {number}
     */
    areaCalculadaSemChapaM2: {
      required: false,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * ID do produto.
     * @type {?number}
     */
    idProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * ID do cliente.
     * @type {?number}
     */
    idCliente: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Altura do produto.
     * @type {?number}
     */
    altura: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Largura do produto.
     * @type {?number}
     */
    largura: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Quantidade do produto.
     * @type {number}
     */
    quantidade: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica para o cálculo se o produto é redondo.
     * @type {?boolean}
     */
    redondo: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Espessura do produto a ser calculado.
     * @type {?number}
     */
    espessura: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica para o cálculo se o cálculo considera múltiplo de 5.
     * @type {?boolean}
     */
    calcularMultiploDe5: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Número de beneficiamentos para que seja considerada a área mínima no cálculo.
     * @type {?number}
     */
    numeroBeneficiamentosParaAreaMinima: {
      required: false,
      twoWay: false,
      default: 0,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Tamanho máximo permitido para o produto.
     * @type {?number}
     */
    tamanhoMaximoProduto: {
      required: false,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Tipo de validação para o cálculo.
     * @type {?string}
     */
    tipoValidacao: {
      required: false,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarValoresOuVazio('Pedido')
    },

    /**
     * Dados adicionais para informar ao método de validação.
     * @type {?object}
     */
    dadosAdicionaisValidacao: {
      required: false,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarObjetoOuVazio
    }
  },

  methods: {
    /**
     * Calcula a área em m² do produto, se houver todas as informações.
     */
    calcularArea: function () {
      this.executarTimeout('calcularArea', function () {
        if (this.altura > 0 && this.largura > 0 && this.quantidade > 0) {
          var dadosProduto = {
            idProduto: this.idProduto,
            idCliente: this.idCliente,
            altura: this.altura,
            largura: this.largura,
            quantidade: this.quantidade,
            redondo: this.redondo,
            espessura: this.espessura,
            calcularMultiploDe5: this.calcularMultiploDe5,
            numeroBeneficiamentosParaAreaMinima: this.numeroBeneficiamentosParaAreaMinima,
            tipoValidacao: this.tipoValidacao,
            dadosAdicionaisValidacao: this.codificarJson(this.dadosAdicionaisValidacao)
          };

          var vm = this;
          var areaM2, areaCalculadaM2, areaCalculadaSemChapaM2;

          Servicos.Produtos.calcularAreaM2(dadosProduto)
            .then(function (resposta) {
              areaM2 = resposta.data.areaM2;
              areaCalculadaM2 = resposta.data.areaM2Calculo;
              areaCalculadaSemChapaM2 = resposta.data.areaM2CalculoSemChapaDeVidro;
            })
            .catch(function (erro) {
              areaM2 = 0;
              areaCalculadaM2 = 0;
              areaCalculadaSemChapaM2 = 0;
            })
            .then(function () {
              vm.$emit('update:areaM2', areaM2);
              vm.$emit('update:areaCalculadaM2', areaCalculadaM2);
              vm.$emit('update:areaCalculadaSemChapaM2', areaCalculadaSemChapaM2);
            });
        } else {
          this.$emit('update:areaM2', 0);
          this.$emit('update:areaCalculadaM2', 0);
          this.$emit('update:areaCalculadaSemChapaM2', 0);
        }
      });
    }
  },

  computed: {
    /**
     * Propriedade computada que indica se a mensagem de validação
     * deve ser exibida para o tamanho máximo do produto de obra.
     */
    exibirValidacao: function () {
      return this.tamanhoMaximoProduto > 0
        && this.areaM2 > this.tamanhoMaximoProduto;
    }
  },

  watch: {
    /**
     * Observador da propriedade 'idProduto'.
     * Recalcula a área quando o valor for alterado.
     */
    idProduto: function () {
      this.calcularArea();
    },

    /**
     * Observador da propriedade 'idCliente'.
     * Recalcula a área quando o valor for alterado.
     */
    idCliente: function () {
      this.calcularArea();
    },

    /**
     * Observador da propriedade 'altura'.
     * Recalcula a área quando o valor for alterado.
     */
    altura: function () {
      this.calcularArea();
    },

    /**
     * Observador da propriedade 'largura'.
     * Recalcula a área quando o valor for alterado.
     */
    largura: function () {
      this.calcularArea();
    },

    /**
     * Observador da propriedade 'quantidade'.
     * Recalcula a área quando o valor for alterado.
     */
    quantidade: function () {
      this.calcularArea();
    },

    /**
     * Observador da propriedade 'redondo'.
     * Recalcula a área quando o valor for alterado.
     */
    redondo: function () {
      this.calcularArea();
    },

    /**
     * Observador da propriedade 'espessura'.
     * Recalcula a área quando o valor for alterado.
     */
    espessura: function () {
      this.calcularArea();
    },

    /**
     * Observador da propriedade 'calcularMultiploDe5'.
     * Recalcula a área quando o valor for alterado.
     */
    calcularMultiploDe5: function () {
      this.calcularArea();
    },

    /**
     * Observador da propriedade 'numeroBeneficiamentosParaAreaMinima'.
     * Recalcula a área quando o valor for alterado.
     */
    numeroBeneficiamentosParaAreaMinima: function () {
      this.calcularArea();
    }
  },

  template: '#CampoAreaM2-template'
});
