Vue.component('controle-tipo-perda', {
  inheritAttrs: false,
  mixins: [Mixins.Objetos],

  props: {
    tipoPerda: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    subtipoPerda: {
      required: false,
      twoWay: true,
      default: null,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    exibirSubtipos: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  methods: {
    obterTiposPerda: function () {
      return Servicos.Producao.TiposPerda.obterParaFiltro();
    },

    obterSubtiposPerda: function (filtro) {
      const idTipoPerda = filtro.idTipoPerda;
      return Servicos.Producao.TiposPerda.SubtiposPerda.obterParaFiltro(idTipoPerda);
    }
  },

  computed: {
    filtroSubtipoPerda: function () {
      return {
        idTipoPerda: this.tipoPerda ? this.tipoPerda.id : null
      };
    },

    tipoPerdaAtual: {
      get: function () {
        return this.tipoPerda;
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.tipoPerda)) {
          this.$emit('update:tipoPerda', valor);
        }
      }
    },

    subtipoPerdaAtual: {
      get: function () {
        return this.subtipoPerda;
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.subtipoPerda)) {
          this.$emit('update:subtipoPerda', valor);
        }
      }
    }
  },

  template: '#ControleTipoPerda-template'
});
