Vue.component('pendencia-carregamento-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de carregamentos pendentes.
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
          idCarregamento: null,
          idCliente: null,
          nomeCliente: null,
          idLoja: null,
          periodoPrevisaoSaidaInicio: null,
          periodoPrevisaoSaidaFim: null,
          idsRota: [],
          ignorarPedidosVendaTransferencia: false,
          idClienteExterno: null,
          nomeClienteExterno: null,
          idsRotaExterna: []
        },
        this.filtro,        
      ),
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
  },

  watch: {
    /**
     * Observador para a propriedade 'configuracoes'.
     * Atualiza os filtros padronizados.
     */
    configuracoes: {
      handler: function () {
        this.filtroAtual.idCarregamento = GetQueryString('idCarregamento');
        this.filtroAtual.ordenacao = 1;

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
      handler: function (atual) {
        this.filtroAtual.idLoja = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#ListaPendenciaCarregamento-Filtro-template'
});
