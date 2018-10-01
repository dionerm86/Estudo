Vue.component('pedido-filtros', {
  mixins: [Mixins.Clonar, Mixins.Merge, Mixins.Comparar],
  props: {
    /**
     * Filtros selecionados para a lista de pedidos.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de pedidos.
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
          id: null,
          idOrcamento: null,
          codigoPedidoCliente: null,
          idCliente: null,
          nomeCliente: null,
          valorPedidoMinimo: null,
          valorPedidoMaximo: null,
          idLoja: null,
          situacao: null,
          situacaoProducao: null,
          tipoPedido: null,
          fastDelivery: null,
          origem: 0,
          idCidade: null,
          nomeBairro: null,
          endereco: null,
          complemento: null,
          alturaProduto: null,
          larguraProduto: null,
          diferencaDiasEntreProntoELiberado: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          periodoFinalizacaoInicio: null,
          periodoFinalizacaoFim: null,
          codigoUsuarioFinalizacao: null,
          tipoVenda: null,
          observacao: null,
          observacaoLiberacao: null,
          vendedorFixo: null,
          tipoPedidoFixo: null
        },
        this.filtro
      ),
      situacaoAtual: null,
      lojaAtual: null,
      situacaoProducaoAtual: null,
      tipoVendaAtual: null,
      cidadeAtual: null
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
     * Retorna os itens para o controle de situações de pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoesPedido: function() {
      return Servicos.Pedidos.obterSituacoes();
    },

    /**
     * Retorna os itens para o controle de situações de produção.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoesProducao: function() {
      return Servicos.Producao.obterSituacoes();
    },

    /**
     * Retorna os itens para o controle de tipos de pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposPedido: function() {
      return Servicos.Pedidos.obterTiposPedido();
    },

    /**
     * Retorna os itens para o controle de funcionários de finalização.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    recuperarUsuariosFinalizacao: function() {
      return Servicos.Funcionarios.obterFinalizacaoPedidos();
    },

    /**
     * Retorna os itens para o controle do filtro de tipos de venda.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposVenda: function() {
      return Servicos.Pedidos.obterTiposVenda();
    }
  },

  watch: {
    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacaoAtual: {
      handler: function(atual) {
        this.filtroAtual.situacao = atual ? atual.id : null;
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
     * Observador para a variável 'situacaoProducaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacaoProducaoAtual: {
      handler: function(atual) {
        this.filtroAtual.situacaoProducao = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoVendaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoVendaAtual: {
      handler: function(atual) {
        this.filtroAtual.tipoVenda = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'cidadeAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    cidadeAtual: {
      handler: function(atual) {
        this.filtroAtual.idCidade = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstPedidos-Filtro-template'
});
