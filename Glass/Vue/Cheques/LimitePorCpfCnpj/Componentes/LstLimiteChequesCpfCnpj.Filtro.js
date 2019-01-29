Vue.component('limite-cheques-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de limite de cheques.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      filtroAtual: this.merge(
        {
          cpfCnpj: null
        },
        this.filtro
      ),
      tipoPessoaAtual: {}
    };
  },

  methods: {
    /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
    filtrar: function () {
      var novoFiltro = this.clonar(this.filtroAtual);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }
    }
  },

  watch: {
    tipoPessoaAtual: {
      handler: function () {
        this.filtroAtual.cpfCnpj = null;
      },
      deep: true
    }
  },

  template: '#LstLimiteChequesCpfCnpj-Filtro-template'
});
