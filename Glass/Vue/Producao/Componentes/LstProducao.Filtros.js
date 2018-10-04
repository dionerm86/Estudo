Vue.component('producao-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de produção.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de produção.
     * @type {!Object}
     */
    configuracoes: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      filtroAtual: this.merge(
        {
          idPedido: null,
          idLiberacaoPedido: null,
          idCarregamento: null,
          idPedidoImportado: null,
          codigoPedidoCliente: null,
          idsRotas: null,
          idCliente: null,
          nomeCliente: null,
          idImpressao: null,
          numeroEtiquetaPeca: null,
          situacoesProducao: null,
          idSetor: null,
          periodoSetorInicio: null,
          periodoSetorFim: null,
          idLoja: null,
          tipoSituacaoProducao: null,
          situacaoPedido: null,
          idsSubgrupos: null,
          idsBeneficiamentos: null,
          idVendedorPedido: null,
          tipoEntregaPedido: null,
          periodoEntregaInicio: null,
          periodoEntregaFim: null,
          parguraPeca: null,
          alturaPeca: null,
          tiposPedidos: null,
          tiposPecasExibir: null,
          periodoFabricaInicio: null,
          periodoFabricaFim: null,
          espessuraPeca: null,
          idCorVidro: null,
          idsProcessos: null,
          idsAplicacoes: null,
          planoCorte: null,
          numeroEtiquetaChapa: null,
          tipoProdutosComposicao: null,
          apenasPecasAguardandoExpedicao: null,
          apenasPecasAguardandoEntradaEstoque: null,
          apenasPecasParadasNaProducao: null,
          apenasPecasRepostas: null,
          periodoConferenciaPedidoInicio: null,
          periodoConferenciaPedidoFim: null,
          tipoFastDelivery: null
        },
        this.filtro
      ),
      setorAtual: null,
      lojaAtual: null
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
     * Busca as rotas para o controle de filtro.
     */
    obterRotas: function () {
      return Servicos.Rotas.obterFiltro();
    },

    /**
     * Busca as situações de produção para o filtro.
     */
    obterSituacoesProducao: function () {
      return Servicos.Producao.obterSituacoes();
    }
  },

  watch: {
    setorAtual: {
      handler: function (atual) {
        this.filtroAtual.idSetor = atual ? atual.id : null;

        if (!atual) {
          this.filtroAtual.periodoSetorInicio = null;
          this.filtroAtual.periodoSetorFim = null;
        }
      },
      deep: true
    },

    lojaAtual: {
      handler: function (atual) {
        this.filtroAtual.idLoja = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstProducao-Filtros-template'
});
