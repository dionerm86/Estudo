﻿Vue.component('campo-endereco', {
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

  data: function () {
    var cidadeEndereco = (this.endereco || {}).cidade || {};

    return {
      ufAtual: cidadeEndereco.uf,
      cidadeAtual: cidadeEndereco,
      atualizandoUf: false
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
          this.$emit('update:endereco', valor);
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
        this.enderecoAtual = valor;
        var vm = this;

        this.$nextTick(function () {
          vm.cidadeAtual = valor ? valor.cidade : null;
        });
      },
      deep: true
    },

    /**
     * Observador para a variável 'cidadeAtual'.
     * Atualiza o endereço com os dados da cidade, se houver mudança.
     */
    cidadeAtual: {
      handler: function (valor, anterior) {
        if (this.atualizandoUf && !valor && anterior) {
          this.cidadeAtual = anterior;
          this.atualizandoUf = false;
          return;
        }

        if (valor && this.ufAtual !== valor.uf) {
          this.ufAtual = valor.uf;
        }

        if (!this.equivalentes(valor, this.enderecoAtual.cidade)) {
          var novoEndereco = this.clonar(this.enderecoAtual);
          novoEndereco.cidade = valor;

          this.enderecoAtual = novoEndereco;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'ufAtual'.
     * Atualiza o endereço com a UF, se houver mudança.
     */
    ufAtual: function (valor) {
      this.atualizandoUf = true;

      if (valor !== (this.enderecoAtual.cidade || {}).uf) {
        var novoEndereco = this.clonar(this.enderecoAtual);

        if (novoEndereco.cidade) {
          novoEndereco.cidade.uf = valor;

          this.enderecoAtual = novoEndereco;
        }
      }
    }
  },

  template: '#CampoEndereco-template'
});
