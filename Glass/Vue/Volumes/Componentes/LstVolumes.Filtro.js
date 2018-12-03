Vue.component('volumes-filtros', {
  mixins: [Mixins.Data, Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de volumes.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela.
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
          idVolume: null,
          idCliente: null,
          nomeCliente: null,
          periodoEntregaPedidoInicio: null,
          periodoEntregaPedidoFim: null,
          idLoja: null,
          idRota: null,
          situacoesPedidoVolume: null,
          situacoesVolume: null,
          tipoEntrega: null,
          idClienteExterno: null,
          nomeClienteExterno: null,
          idsRotaExterna: null,
          carregarItensAutomaticamente: false
        },
        this.filtro
      ),
      lojaAtual: null,
      rotaAtual: null,
      tipoEntregaAtual: null,
      intervaloAtualizacaoLista: null
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
     * Retorna os itens para o controle de rotas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroRotas: function () {
      return Servicos.Rotas.obterFiltro(null, null);
    },

    /**
     * Retorna os itens para o controle de situações de volume do pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoesPedidoVolume: function () {
      return Servicos.Pedidos.obterSituacoesVolume();
    },

    /**
     * Retorna os itens para o controle de situações de volume.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoesVolume: function () {
      return Servicos.Volumes.obterSituacoes();
    },

    /**
     * Retorna os itens para o controle de tipos de entrega de pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposEntrega: function () {
      return Servicos.Pedidos.obterTiposEntrega();
    }
  },

  watch: {
    /**
     * Observador para a propriedade 'configuracoes'.
     * Atualiza os filtros padronizados.
     */
    configuracoes: {
      handler: function () {
        this.filtroAtual.situacoesPedidoVolume = this.configuracoes
          ? this.configuracoes.situacoesPedidoVolume
          : [];

        var dataAtual = new Date();
        this.filtroAtual.periodoEntregaPedidoInicio = this.adicionarMeses(dataAtual, -1);
        this.filtroAtual.periodoEntregaPedidoFim = dataAtual;
        this.filtroAtual.carregarItensAutomaticamente = true;
        
        var vm = this;
        this.$nextTick(function () {
          vm.filtrar();
        });
      },
      deep: true
    },

    /**
     * Observador para a variável 'lojaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    lojaAtual: {
      handler: function(atual) {
        this.filtroAtual.idLoja = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'rotaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    rotaAtual: {
      handler: function(atual) {
        this.filtroAtual.idRota = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoEntregaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoEntregaAtual: {
      handler: function (atual) {
        this.filtroAtual.tipoEntrega = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstVolumes-Filtro-template'
});
