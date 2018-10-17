Vue.component('campo-cep', {
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
  },

  data: function () {
    return {
      cep: (this.endereco ? this.endereco.cep : null) || ''
    };
  },

  methods: {
    /**
     * Busca o endereço vinculado ao CEP desejado.
     */
    buscarCep: function () {
      if (this.enderecoAtual.cep === this.cep) {
        return;
      }

      var vm = this;

      Servicos.Cep.obterEndereco(this.cep)
        .then(function (resposta) {
          vm.enderecoAtual = resposta.data;
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
            vm.enderecoAtual = {
              cep: vm.cep
            };
          }
        });
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o endereço normalizado e que
     * atualiza a propriedade em caso de alteração.
     */
    enderecoAtual: {
      get: function () {
        return this.endereco || {
          cep: this.cep
        };
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.endereco)) {
          this.$emit('update:endereco', valor);
        }
      }
    }
  },

  watch: {
    /**
     * Observador para a variável 'enderecoAtual.cep'.
     * Atualiza o texto do controle em caso de alteração.
     */
    'enderecoAtual.cep': function (atual) {
      this.cep = atual;
    }
  },

  template: '#CampoCep-template'
});
