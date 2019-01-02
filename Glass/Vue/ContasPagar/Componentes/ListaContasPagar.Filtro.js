Vue.component('contaspagar-filtro', {
  mixins: [Mixins.Data, Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de contas a pagar.
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
          numeroNotaFiscal: null,
          idLoja: null,
          idCustoFixo: null,
          idImpostoServico: null,
          idPagamentoRestante: null,
          idFornecedor: null,
          nomeFornecedor: null,
          periodoVencimentoInicio: null,
          periodoVencimentoFim: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          idsFormaPagamento: [],
          valorInicial: null,
          valorFinal: null,
          buscarCheques: null,
          tipo: null,
          buscarPrevisaoCustoFixo: false,
          apenasContasDeComissao: false,
          planoConta: null,
          apenasContasDeCustoFixo: false,
          apenasContasComValorAPagar: false,
          periodoPagamentoInicio: null,
          periodoPagamentoFim: null,
          periodoNotaFiscalInicio: null,
          periodoNotaFiscalFim: null,
          numeroCte: null,
          idTransportadora: null,
          nomeTransportadora: null,
          idFuncionarioComissao: false,
          idComissao: false,
          agruparImpressaoPor: null
        },
        this.filtro
      ),
      lojaAtual: null,
      tipoAtual: null,
      formaPagamentoAtual: null
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
    obterFormasPagamentoCompras: function () {
      return Servicos.FormasPagamento.obterFormasPagamentoCompra();
    },

    /**
     * Retorna os itens para o controle de transportadoras.
     * @param {Object} tipo O tipo de plano de conta (credito/debito).
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterTransportadoras: function () {
      return Servicos.Transportadores.obterParaControle();
    }
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

    /**
     * Observador para a variável 'formaPagamentoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    formaPagamentoAtual: {
      handler: function (atual) {
        this.filtroAtual.formaPagamento = atual ? atual.id : null;
      },
      deep: true
    },
  },

  template: '#ListaContasPagar-Filtro-template'
});
