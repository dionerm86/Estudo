Vue.component('carregamentos-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de carregamentos.
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
          idRota: null,
          idMotorista: null,
          idLoja: null,
          idOrdemCarga: null,
          idPedido: null,
          placa: null,
          situacaoCarregamento: null,
          periodoPrevisaoSaidaInicio: null,
          periodoPrevisaoSaidaFim: null,
          ordenacao: null
        },
        this.filtro
      ),
      rotaAtual: null,
      motoristaAtual: null,
      veiculoAtual: null,
      situacaoCarregamentoAtual: null,
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
     * Retorna os itens para o controle de motoristas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroMotoristas: function () {
      return Servicos.Funcionarios.obterMotoristas();
    },

    /**
     * Retorna os itens para o controle de veículos.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroVeiculos: function () {
      return Servicos.Veiculos.obterFiltro();
    },

    /**
     * Retorna os itens para o controle de situações de carregamento.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoesCarregamento: function () {
      return Servicos.Carregamentos.obterSituacoes();
    }
  },

  watch: {
    /**
     * Observador para a propriedade 'configuracoes'.
     * Atualiza os filtros padronizados.
     */
    configuracoes: {
      handler: function () {
        this.filtroAtual.id = GetQueryString('idCarregamento');
        this.filtroAtual.ordenacao = 1;

        var vm = this;
        this.$nextTick(function () {
          vm.filtrar();
        });
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
     * Observador para a variável 'motoristaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    motoristaAtual: {
      handler: function(atual) {
        this.filtroAtual.idMotorista = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    veiculoAtual: {
      handler: function(atual) {
        this.filtroAtual.placa = atual ? atual.codigo : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'situacaoCarregamentoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacaoCarregamentoAtual: {
      handler: function(atual) {
        this.filtroAtual.situacaoCarregamento = atual ? atual.id : null;
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
    }
  },

  template: '#LstCarregamentos-Filtro-template'
});
