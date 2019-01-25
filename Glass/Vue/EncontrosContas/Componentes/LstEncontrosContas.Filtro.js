Vue.component('encontros-contas-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de encontros de contas.
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
          id: null,
          idCliente: null,
          nomeCliente: null,
          idFornecedor: null,
          nomeFornecedor: null,
          observacao: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
        },
        this.filtro
      ),
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

  template: '#LstEncontrosContas-Filtro-template'
});