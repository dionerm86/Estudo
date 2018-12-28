Vue.component('pagamento-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de pagamentos.
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
          idCompra: null,
          idCustoFixo: null,
          idImpostoServico: null,
          idFornecedor: null,
          nomeFornecedor: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          valorInicial: null,
          valorFinal: null,
          situacao: 0,
          numeroNotaFiscal: null,
          observacao: null
        },
        this.filtro
      )
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

  template: '#LstPagamentos-Filtro-template'
});
