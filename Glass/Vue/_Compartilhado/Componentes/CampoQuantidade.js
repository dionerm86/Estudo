Vue.component('campo-quantidade', {
  mixins: [Mixins.ExecutarTimeout],
  props: {
    /**
     * Quantidade selecionada no controle.
     * @type {number}
     */
    quantidade: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Percentual de desconto por quantidade selecionado no controle.
     * @type {?number}
     */
    percentualDescontoPorQuantidade: {
      required: false,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Valor de desconto por quantidade selecionado no controle.
     * @type {?number}
     */
    valorDescontoPorQuantidade: {
      required: false,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica se a quantidade pode ser decimal ou não.
     * @type {?boolean}
     */
    permiteDecimal: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * ID do produto para busca do desconto por quantidade.
     * @type {?number}
     */
    idProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * ID do grupo do produto para busca do desconto por quantidade.
     * @type {?number}
     */
    idGrupoProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * ID do subgrupo do produto para busca do desconto por quantidade.
     * @type {?number}
     */
    idSubgrupoProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * ID do cliente para busca do desconto por quantidade.
     * @type {?number}
     */
    idCliente: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica se o campo de desconto por quantidade deve ser exibido ou não.
     * Se estiver marcado como 'true', o controle de desconto por quantidade será exibido de acordo com
     * seu controle interno.
     * @type {?boolean}
     */
    exibirDescontoPorQuantidade: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBoolean
    },

    /**
     * Indica se o percentual de desconto pode ser alterado.
     * @type {boolean}
     */
    permitirAlterarDescontoPorQuantidade: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  data: function() {
    return {
      percentualDescontoPorQuantidadeMaximo: 0,
      percentualDescontoTabela: 0
    }
  },

  methods: {
    /**
     * Atualiza os percentuais de desconto para o controle de desconto por quantidade.
     */
    atualizarDadosDescontoPorQuantidade: function () {
      if (!this.idProduto || !this.idCliente) {
        return;
      }

      this.executarTimeout('atualizarDadosDescontoPorQuantidade', function () {
        var vm = this;

        Servicos.DescontoPorQuantidade.obterDados(
          this.idProduto,
          this.idGrupoProduto,
          this.idSubgrupoProduto,
          this.idCliente,
          this.quantidadeAtual
        )
          .then(function (resposta) {
            vm.percentualDescontoPorQuantidadeMaximo = resposta.data.percentualDescontoPorQuantidade;
            vm.percentualDescontoTabela = resposta.data.percentualDescontoTabela;
          })
          .catch(function (erro) {
            if (erro && erro.mensagem) {
              vm.exibirMensagem('Erro', erro.mensagem);
            }
          })
      });
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna a quantidade normalizada, e que
     * atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    quantidadeAtual: {
      get: function() {
        return this.quantidade || 0;
      },
      set: function(valor) {
        if (valor !== this.quantidade) {
          this.atualizarDadosDescontoPorQuantidade();
          this.$emit('update:quantidade', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna o percentual de desconto por quantidade normalizado
     * e que atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    percentualDescontoPorQuantidadeAtual: {
      get: function() {
        return this.percentualDescontoPorQuantidade || 0;
      },
      set: function(valor) {
        if (valor !== this.percentualDescontoPorQuantidade) {
          this.$emit('update:percentualDescontoPorQuantidade', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna o valor de desconto por quantidade normalizado
     * e que atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    valorDescontoPorQuantidadeAtual: {
      get: function() {
        return this.valorDescontoPorQuantidade || 0;
      },
      set: function(valor) {
        if (valor !== this.valorDescontoPorQuantidade) {
          this.$emit('update:valorDescontoPorQuantidade', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna o valor do incremento do campo quantidade.
     * @type {number}
     */
    incremento: function() {
      return this.permiteDecimal ? 0.01 : 1;
    },

    /**
     * Propriedade computada que indica se o controle de desconto por quantidade deve ser exibido.
     * @type {boolean}
     */
    exibirDescontoPorQuantidade_: function () {
      return this.percentualDescontoPorQuantidade > 0
        || this.percentualDescontoPorQuantidadeMaximo > 0
        || this.percentualDescontoTabela > 0;
    }
  },

  watch: {
    /**
     * Observador para a variável 'idProduto'.
     * Atualiza os dados de desconto por quantidade.
     */
    idProduto: function () {
      this.atualizarDadosDescontoPorQuantidade();
    },

    /**
     * Observador para a variável 'idCliente'.
     * Atualiza os dados de desconto por quantidade.
     */
    idCliente: function () {
      this.atualizarDadosDescontoPorQuantidade();
    }
  },

  template: '#CampoQuantidade-template'
});
