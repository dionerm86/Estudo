Vue.component('ordenscarga-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de ordens de carga.
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
          id: null,
          idCarregamento: null,
          idCliente: null,
          nomeCliente: null,
          idLoja: null,
          idRota: null,
          idPedido: null,
          periodoEntregaPedidoInicio: null,
          periodoEntregaPedidoFim: null,
          situacoesOrdemCarga: [],
          tiposOrdemCarga: [],
          idClienteExterno: null,
          nomeClienteExterno: null,
          idsRotasExternas: []
        },
        this.filtro
      ),
      rotaAtual: null,
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
     * Retorna os itens para o controle de rotas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroRotas: function () {
      return Servicos.Rotas.obterFiltro(null, null);
    },

    /**
     * Retorna os itens para o controle de rotas externas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroRotasExternas: function () {
      return Servicos.Rotas.obterFiltroRotasExternas();
    },

    /**
     * Retorna os itens para o controle de tipos de ordens de carga.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposOrdemCarga: function () {
      return Servicos.Carregamentos.OrdensCarga.obterTiposParaControle();
    },

    /**
     * Retorna os itens para o controle de situações de ordem de carga.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoesOrdemCarga: function () {
      return Servicos.Carregamentos.OrdensCarga.obterSituacoesParaControle();
    }
  },

  watch: {
    /**
     * Observador para a variável 'rotaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    rotaAtual: {
      handler: function (atual) {
        this.filtroAtual.idRota = atual ? atual.id : null;
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

  template: '#LstOrdensCarga-Filtro-template'
});
