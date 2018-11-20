Vue.component('campo-telefone', {
  props: {
    /**
     * Telefone buscado pelo controle.
     * @type {?string}
     */
    telefone: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarStringOuVazio
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o telefone e que
     * atualiza a propriedade em caso de alteração.
     */
    telefoneAtual: {
      get: function () {
        return this.telefone;
      },
      set: function (valor) {
        if (valor !== this.telefone) {
          this.$emit('update:telefone', valor);
        }
      }
    }
  },

  template: '#CampoTelefone-template'
});
