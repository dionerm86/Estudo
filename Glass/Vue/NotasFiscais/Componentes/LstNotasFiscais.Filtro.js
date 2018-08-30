Vue.component('notasfiscais-filtros', {
  mixins: [Mixins.Clonar, Mixins.Merge, Mixins.Comparar],
  props: {
    /**
     * Filtros selecionados para a lista de notas fiscais.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function() {
    return {
      filtroAtual: this.merge(
        {
          numeroNfe: null,
          idPedido: null,
          modelo: null,
          idLoja: null,
          idCliente: null,
          nomeCliente: null,
          tipoFiscal: null,
          idFornecedor: null,
          nomeFornecedor: null,
          codigoRota: null,
          situacao: null,
          periodoEmissaoInicio: null,
          periodoEmissaoFim: null,
          idsCfop: null,
          tiposCfop: null,
          periodoEntradaSaidaInicio: null,
          periodoEntradaSaidaFim: null,
          tipoVenda: null,
          idsFormaPagamento: null,
          tipoDocumento: null,
          finalidade: null,
          tipoEmissao: null,
          informacaoComplementar: null,
          codigoInternoProduto: null,
          descricaoProduto: null,
          lote: null,
          valorNotaFiscalInicio: null,
          valorNotaFiscalFim: null,
          ordenacaoFiltro: null,
          agrupar: null
        },
        this.filtro
      ),
      rotaAtual: null,
      lojaAtual: null,
    };
  },

  methods: {
    /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
    filtrar: function() {
      var novoFiltro = this.clonar(this.filtroAtual, true);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }
    },

    /**
     * Retorna os itens para o controle de cfop.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroCfops: function() {
      return Servicos.Cfops.obterFiltro();
    },

    /**
     * Retorna os itens para o controle de tipos de cfop.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposCfop: function () {
      return Servicos.Cfops.obterTipos();
    },

    /**
     * Retorna os itens para o controle de rotas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroRotas: function (id, codigo) {
      return Servicos.Rotas.obterFiltro(id, codigo);
    },

    /**
     * Retorna os itens para o controle de formas de pagamento de nota fiscal.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroFormasPagamentoNotaFiscal: function () {
      return Servicos.FormasPagamento.obterFiltroNotaFiscal();
    },

    /**
     * Retorna os itens para o controle de situações de nota fiscal.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoes: function () {
      return Servicos.NotasFiscais.obterSituacoes();
    }
  },

  watch: {
    /**
     * Observador para a variável 'rotaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    rotaAtual: {
      handler: function (atual) {
        this.filtroAtual.codigoRota = atual ? atual.nome : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'lojaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    lojaAtual: {
      handler: function (atual) {
        this.filtroAtual.idLoja = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstNotasFiscais-Filtro-template'
});
