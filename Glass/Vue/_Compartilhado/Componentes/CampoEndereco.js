Vue.component('campo-endereco', {
  inheritAttrs: false,
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Endereço buscado pelo controle.
     * @type {?Object}
     */
    endereco: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Indica se o campo para o complemento será exibido.
     * @type {?boolean}
     */
    exibirComplemento: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Indica se o campo para o número será exibido.
     * @type {?boolean}
     */
    exibirNumero: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  data: function() {
    return {
      cidadeAtual: this.cidade || {}
    };
  },

  computed: {
    /**
     * Propriedade computada que retorna o endereço normalizado e que
     * atualiza a propriedade em caso de alteração.
     */
    enderecoAtual: {
      get: function () {
        return this.endereco || {};
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.endereco)) {
          this.nomeAtual = valor && valor.cidade ? valor.cidade.nome : null;
          this.$emit('update:endereco', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna a UF normalizada e que
     * atualiza a propriedade em caso de alteração.
     */
    ufAtual: {
      get: function () {
        return this.enderecoAtual.cidade.uf || '';
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.enderecoAtual.cidade.uf)) {
          this.enderecoAtual.cidade.uf = valor;
        }
      }
    }
  },

  watch: {
    /**
     * Observador para a propriedade 'endereco'.
     * Atualiza os dados internos se houver mudança externa.
     */
    endereco: {
      handler: function (valor) {
        if (valor && valor.cidade) {
          this.nomeAtual = valor.cidade.nome;
          this.ufAtual = valor.cidade.uf;
        } else {
          this.nomeAtual = null;
        }

        var vm = this;

        this.$nextTick(function () {
          vm.enderecoAtual = valor;
        });
      },
      deep: true
    },

    cidadeAtual: {
      handler: function (valor) {
        if (!this.equivalentes(valor, this.cidadeAtual)) {
          this.enderecoAtual.cidade = valor;
        }
      },
      deep: true
    }
  },

  template: '#CampoEndereco-template'
});
