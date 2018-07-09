Vue.component('campo-total', {
  mixins: [Mixins.JsonQuerystring, Mixins.ExecutarTimeout],
  props: {
    /**
     * Valor total do produto.
     * @type {number}
     */
    total: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Área do produto, em m².
     * @type {number}
     */
    areaM2: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Área calculada do produto, em m².
     * @type {number}
     */
    areaCalculadaM2: {
      required: false,
      twoWay: false,
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
     * Quantidade do ambiente.
     * @type {number}
     */
    quantidadeAmbiente: {
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
     * Valor unitário do produto.
     * @type {?number}
     */
    valorUnitario: {
      required: false,
      twoWay: false,
      default: 0,
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
     * Calcula o valor total do produto, se houver todas as informações.
     */
    calcularTotal: function () {
      this.executarTimeout('calcularTotal', function () {
        if (this.altura > 0 && this.quantidade > 0 && this.valorUnitario > 0) {
          var dadosProduto = {
            idProduto: this.idProduto,
            idCliente: this.idCliente,
            altura: this.altura,
            largura: this.largura,
            quantidade: this.quantidade,
            quantidadeAmbiente: this.quantidadeAmbiente,
            areaM2: this.areaM2,
            areaCalculadaM2: this.areaCalculadaM2,
            redondo: this.redondo,
            espessura: this.espessura,
            calcularMultiploDe5: this.calcularMultiploDe5,
            numeroBeneficiamentosParaAreaMinima: this.numeroBeneficiamentosParaAreaMinima,
            valorUnitario: this.valorUnitario,
            tipoValidacao: this.tipoValidacao,
            dadosAdicionaisValidacao: this.codificarJson(this.dadosAdicionaisValidacao)
          };

          var vm = this;
          var total;

          Servicos.Produtos.calcularTotal(dadosProduto)
            .then(function (resposta) {
              total = resposta.data.total;
            })
            .catch(function (erro) {
              total = 0;
            })
            .then(function () {
              vm.$emit('update:total', total);
            });
        } else {
          this.$emit('update:total', 0);
        }
      });
    }
  },

  watch: {
    /**
     * Observador da propriedade 'idProduto'.
     * Recalcula a área quando o valor for alterado.
     */
    idProduto: function () {
      this.calcularTotal();
    },

    /**
     * Observador da propriedade 'idCliente'.
     * Recalcula a área quando o valor for alterado.
     */
    idCliente: function () {
      this.calcularTotal();
    },

    /**
     * Observador da propriedade 'altura'.
     * Recalcula a área quando o valor for alterado.
     */
    altura: function () {
      this.calcularTotal();
    },

    /**
     * Observador da propriedade 'largura'.
     * Recalcula a área quando o valor for alterado.
     */
    largura: function () {
      this.calcularTotal();
    },

    /**
     * Observador da propriedade 'quantidade'.
     * Recalcula a área quando o valor for alterado.
     */
    quantidade: function () {
      this.calcularTotal();
    },

    /**
     * Observador da propriedade 'quantidadeAmbiente'.
     * Recalcula a área quando o valor for alterado.
     */
    quantidadeAmbiente: function () {
      this.calcularTotal();
    },

    /**
     * Observador da propriedade 'areaM2'.
     * Recalcula a área quando o valor for alterado.
     */
    areaM2: function () {
      this.calcularTotal();
    },

    /**
     * Observador da propriedade 'areaCalculadaM2'.
     * Recalcula a área quando o valor for alterado.
     */
    areaCalculadaM2: function () {
      this.calcularTotal();
    },

    /**
     * Observador da propriedade 'redondo'.
     * Recalcula a área quando o valor for alterado.
     */
    redondo: function () {
      this.calcularTotal();
    },

    /**
     * Observador da propriedade 'espessura'.
     * Recalcula a área quando o valor for alterado.
     */
    espessura: function () {
      this.calcularTotal();
    },

    /**
     * Observador da propriedade 'calcularMultiploDe5'.
     * Recalcula a área quando o valor for alterado.
     */
    calcularMultiploDe5: function () {
      this.calcularTotal();
    },

    /**
     * Observador da propriedade 'numeroBeneficiamentosParaAreaMinima'.
     * Recalcula a área quando o valor for alterado.
     */
    numeroBeneficiamentosParaAreaMinima: function () {
      this.calcularTotal();
    },

    /**
     * Observador da propriedade 'valorUnitario'.
     * Recalcula a área quando o valor for alterado.
     */
    valorUnitario: function () {
      this.calcularTotal();
    }
  },

  template: '#CampoTotal-template'
});
