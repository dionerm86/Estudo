Vue.component('producao-filtros', {
  mixins: [Mixins.Clonar, Mixins.Merge, Mixins.Comparar],
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
      )
    };
  },

  methods: {
    /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
    filtrar: function () {
      var novoFiltro = this.clonar(this.filtroAtual, true);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }
    },

    /**
     * Busca as rotas para o controle de filtro.
     */
    obterRotas: function () {
      return Servicos.Rotas.obterFiltro();
    }
  },

  template: '#LstProducao-Filtros-template'
});
