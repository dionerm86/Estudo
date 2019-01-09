Vue.component('contaspagas-filtro', {
  mixins: [Mixins.Data, Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de contas pagas.
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
          idComissao: null,
          idLoja: null,
          idCustoFixo: null,
          idImpostoServico: null,
          idFornecedor: null,
          nomeFornecedor: null,
          numeroNotaFiscal: null,
          numeroCte: null,
          idsFormaPagamento: [],
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          periodoPagamentoInicio: null,
          periodoPagamentoFim: null,
          periodoVencimentoInicio: null,
          periodoVencimentoFim: null,
          valorVencimemtoInicial: null,
          valorVencimemtoFinal: null,
          tipo: null,
          apenasContasDeComissao: false,
          buscarRenegociadas: false,
          buscarContasComJurosMulta: false,
          planoConta: null,
          apenasContasDeCustoFixo: false,
          buscarContasPagar: false,
          observacao: null,
          agruparImpressaoContasPagasPor: null
        },
        this.filtro
      ),
      lojaAtual: {},
      tipoAtual: {}
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
    },

    /**
     * Retorna os itens para o controle de tipos contábeis.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterTiposContabeis: function () {
      return Servicos.ContasPagar.obterTiposContabeis();
    },

    /**
     * Retorna os itens para o controle de formas de pagamento.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterFormasPagamentoPagamentos: function () {
      return Servicos.FormasPagamento.obterFormasPagamentoPagamentos();
    },
  },

  watch: {
    /**
     * Observador para a variável 'lojaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    lojaAtual: {
      handler: function (atual) {
        this.filtroAtual.idLoja = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoAtual: {
      handler: function (atual) {
        this.filtroAtual.tipo = atual ? atual.id : null;
      },
      deep: true
    },
  },

  template: '#ListaContasPagas-Filtro-template'
});
