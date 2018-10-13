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
    },

    /**
     * Define o tipo de agrupamento (numérico) para a impressão do relatório.
     * @type {?number}
     */
    agruparImpressao: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    }
  },

  data: function () {
    return {
      filtroAtual: this.merge(this.filtroVazio(), this.filtro),
      setorAtual: null,
      lojaAtual: null,
      tipoSituacaoProducaoAtual: null,
      situacaoPedidoAtual: null,
      funcionarioAtual: null,
      tipoEntregaAtual: null,
      tipoProdutosComposicaoAtual: null,
      corVidroAtual: null,
      tipoFastDeliveryAtual: null
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
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterRotas: function () {
      return Servicos.Rotas.obterFiltro();
    },

    /**
     * Busca as situações de produção para o filtro.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoesProducao: function () {
      return Servicos.Producao.obterSituacoes();
    },

    /**
     * Busca os tipos de situações de produção para o filtro.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposSituacoesProducao: function () {
      return Servicos.Producao.obterTiposSituacoes();
    },

    /**
     * Busca as situações de pedido.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoesPedido: function () {
      return Servicos.Pedidos.obterSituacoes();
    },

    /**
     * Busca os subgrupos de 'vidro'.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSubgrupos: function (filtro) {
      return Servicos.Produtos.Subgrupos.obterParaControle((filtro || {}).idGrupoProduto);
    },

    /**
     * Busca os tipos de entregas de pedido.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposEntregasPedido: function () {
      return Servicos.Pedidos.obterTiposEntrega();
    },

    /**
     * Busca os tipos de pedido.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposPedido: function () {
      return Servicos.Producao.obterTiposPedido();
    },

    /**
     * Busca os tipos de peças a exibir.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposPecasExibir: function () {
      return Servicos.Producao.obterTiposPecasExibir();
    },

    /**
     * Busca as cores de vidros.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterCoresVidro: function () {
      return Servicos.Produtos.CoresVidro.obterParaControle();
    },

    /**
     * Busca os processos de etiqueta.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterProcessos: function () {
      return Servicos.Processos.obterParaControle();
    },

    /**
     * Busca as aplicações de etiqueta.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterAplicacoes: function () {
      return Servicos.Aplicacoes.obterParaControle();
    },

    /**
     * Busca os tipos de produtos de composição.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposProdutosComposicao: function () {
      return Servicos.Producao.obterTiposProdutosComposicao();
    },

    /**
     * Busca os tipos de 'fast delivery'.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposFastDelivery: function () {
      return Servicos.Producao.obterTiposFastDelivery();
    },

    /**
     * Função que retorna o objeto com o filtro vazio.
     * @returns {Object} O objeto com o filtro vazio.
     */
    filtroVazio: function () {
      return {
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
        larguraPeca: null,
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
      };
    },

    /**
     * Limpa os filtros atuais e recarrega a lista de produção.
     */
    limparFiltros: function () {
      this.filtroAtual = this.filtroVazio();
      this.setorAtual = null;
      this.lojaAtual = null;
      this.tipoSituacaoProducaoAtual = null;
      this.situacaoPedidoAtual = null;
      this.funcionarioAtual = null;
      this.tipoEntregaAtual = null;
      this.tipoProdutosComposicaoAtual = null;
      this.corVidroAtual = null;
      this.tipoFastDeliveryAtual = null;

      this.carregarFiltrosPadrao();

      var vm = this;
      this.$nextTick(function () {
        vm.filtrar();
      });
    },

    /**
     * Carrega os filtros padrão da tela.
     */
    carregarFiltrosPadrao: function () {
      this.tipoSituacaoProducaoAtual = this.configuracoes
        ? { id: this.configuracoes.tipoSituacaoProducao }
        : null;

      this.filtroAtual.tiposPecasExibir = this.configuracoes
        ? this.configuracoes.tiposPecasExibir
        : [];

      this.tipoProdutosComposicaoAtual = this.configuracoes
        ? { id: this.configuracoes.tipoProdutoComposicao }
        : null;
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o valor do agrupamento de impressão e que
     * atualiza a propriedade principal ao ser alterada.
     */
    agruparImpressaoAtual: {
      get: function () {
        return this.agruparImpressao;
      },
      set: function (valor) {
        if (valor !== this.agruparImpressao) {
          this.$emit('update:agruparImpressao', valor);
        }
      }
    },

    /**
     * Propriedade computada com o valor do filtro para a busca de subgrupos.
     */
    filtroSubgrupos: function() {
      return {
        idGrupoProduto: this.configuracoes ? this.configuracoes.idGrupoProdutoVidro : 0
      }
    }
  },

  watch: {
    /**
     * Observador para a propriedade 'configuracoes'.
     * Atualiza os filtros padronizados.
     */
    configuracoes: {
      handler: function () {
        this.carregarFiltrosPadrao();

        var vm = this;
        this.$nextTick(function () {
          vm.filtrar();
        });
      },
      deep: true
    },

    /**
     * Observador para a variável 'setorAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
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

    /**
     * Observador para a variável 'lojaAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
    lojaAtual: {
      handler: function (atual) {
        this.filtroAtual.idLoja = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoSituacaoProducaoAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
    tipoSituacaoProducaoAtual: {
      handler: function (atual) {
        this.filtroAtual.tipoSituacaoProducao = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'situacaoPedidoAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
    situacaoPedidoAtual: {
      handler: function (atual) {
        this.filtroAtual.situacaoPedido = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'funcionarioAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
    funcionarioAtual: {
      handler: function (atual) {
        this.filtroAtual.idVendedorPedido = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoEntregaAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
    tipoEntregaAtual: {
      handler: function (atual) {
        this.filtroAtual.tipoEntregaPedido = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoProdutosComposicaoAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
    tipoProdutosComposicaoAtual: {
      handler: function (atual) {
        this.filtroAtual.tipoProdutosComposicao = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'corVidroAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
    corVidroAtual: {
      handler: function (atual) {
        this.filtroAtual.idCorVidro = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoFastDeliveryAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
    tipoFastDeliveryAtual: {
      handler: function (atual) {
        this.filtroAtual.tipoFastDelivery = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstProducao-Filtros-template'
});
