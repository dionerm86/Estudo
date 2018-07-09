Vue.component('campo-endereco', {
  inheritAttrs: false,
  mixins: [Mixins.Comparar],
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

  computed: {
    /**
     * Propriedade computada que retorna o endereço normalizado e que
     * atualiza a propriedade em caso de alteração.
     */
    enderecoAtual: {
      get: function () {
        return this.endereco;
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.endereco)) {
          this.$emit('update:endereco', valor);
        }
      }
    }
  },

  template: '#CampoEndereco-template'
});
