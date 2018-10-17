Vue.component('pedidoconferencia-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de pedidos em conferência.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de pedidos em conferência.
     * @type {!Object}
     */
    configuracoes: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function() {
    return {
      filtroAtual: this.merge(
        {
          idPedido: null,
          idCliente: null,
          nomeCliente: null,
          idLoja: null,
          idVendedor: null,
          idConferente: null,
          situacao: null,
          situacaoPedidoComercial: null,
          idsProcesso: null,
          periodoEntregaInicio: null,
          periodoEntregaFim: null,
          periodoFabricaInicio: null,
          periodoFabricaFim: null,
          periodoFinalizacaoConferenciaInicio: null,
          periodoFinalizacaoConferenciaFim: null,
          periodoCadastroConferenciaInicio: null,
          periodoCadastroConferenciaFim: null,
          periodoCadastroPedidoInicio: null,
          periodoCadastroPedidoFim: null,
          pedidosSemAnexo: null,
          pedidosAComprar: null,
          situacaoCnc: null,
          periodoProjetoCncInicio: null,
          periodoProjetoCncFim: null,
          tiposPedido: null,
          idsRota: null,
          origemPedido: null,
          pedidoImportacaoConferido: null,
          tipoVenda: null
        },
        this.filtro
      ),
      lojaAtual: null,
      situacaoAtual: null,
      vendedorAtual: null,
      conferenteAtual: null,
      tipoVendaAtual: null
    };
  },

  methods: {
    /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
    filtrar: function() {
      var novoFiltro = this.clonar(this.filtroAtual);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }
    },

    /**
     * Retorna os itens para o controle de vendedores.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroVendedores: function() {
      return Servicos.Funcionarios.obterVendedores();
    },

    /**
     * Retorna os itens para o controle de conferentes.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroConferentes: function () {
      return Servicos.Funcionarios.obterConferentes();
    },

    /**
     * Retorna os itens para o controle de situações de pedido comercial.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoesPedidoComercial: function () {
      return Servicos.PedidosConferencia.obterSituacoesPedidoComercial();
    },

    /**
     * Retorna os itens para o controle de situações de pedido em conferência.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoes: function () {
      return Servicos.PedidosConferencia.obterSituacoes();
    },

    /**
     * Retorna os itens para o controle de processos.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroProcessos: function () {
      return Servicos.Processos.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de tipos de pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposPedido: function () {
      return Servicos.Pedidos.obterTiposPedido();
    },

    /**
     * Retorna os itens para o controle do filtro de tipos de venda.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposVenda: function () {
      return Servicos.Pedidos.obterTiposVenda();
    },

    /**
     * Retorna os itens para o controle de rotas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroRotas: function (id, codigo) {
      return Servicos.Rotas.obterFiltro(id, codigo);
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
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacaoAtual: {
      handler: function (atual) {
        this.filtroAtual.situacao = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'vendedorAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    vendedorAtual: {
      handler: function (atual) {
        this.filtroAtual.idVendedor = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'conferenteAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    conferenteAtual: {
      handler: function (atual) {
        this.filtroAtual.idConferente = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoVendaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoVendaAtual: {
      handler: function (atual) {
        this.filtroAtual.tipoVenda = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstPedidosConferencia-Filtro-template'
});
