Vue.component('contarecebida-filtros', {
  mixins: [Mixins.Clonar, Mixins.Merge, Mixins.Comparar],
  props: {
    /**
     * Filtros selecionados para a lista de contas recebidas.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de contas recebidas.
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
          idLiberarPedido: null,
          idAcerto: null,
          idAcertoParcial: null,
          idTrocaDevolucao: null,
          numeroNfe: null,
          idSinal: null,
          numeroCte: null,
          periodoVencimentoInicio: null,
          periodoVencimentoFim: null,
          periodoRecebimentoInicio: null,
          periodoRecebimentoFim: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          recebidaPor: null,
          idLoja: null,
          idVendedor: null,
          idCliente: null,
          nomeCliente: null,
          idVendedorAssociadoCliente: null,
          idVendedorObra: null,
          formasPagamento: null,
          tipoEntrega: null,
          valorRecebidoInicio: null,
          valorRecebidoFim: null,
          idRota: null,
          tiposContabeis: null,
          idComissionado: null,
          observacao: null,
          numeroAutorizacaoCartao: null,
          numeroArquivoRemessa: null,
          buscaArquivoRemessa: null,
          idComissao: null,
          buscaNotaFiscal: null,
          buscarContasAReceber: null,
          buscarContasRenegociadas: null,
          buscarContasDeObra: null,
          buscarContasProtestadas: null,
          buscarContasVinculadas: null,
          ordenacaoFiltro: null
        },
        this.filtro
      ),
      recebidaPorAtual: null,
      vendedorAtual: null,
      lojaAtual: null,
      vendedorAssociadoAtual: null,
      vendedorObraAtual: null,
      tipoEntregaAtual: null,
      rotaAtual: null,
      comissionadoAtual: null
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
     * Retorna os itens para o controle de funcionários caixa diário.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroFuncionariosCaixaDiario: function() {
      return Servicos.Funcionarios.obterCaixaDiario(null, null);
    },

    /**
     * Retorna os itens para o controle de vendedores.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroVendedores: function () {
      return Servicos.Funcionarios.obterVendedores(null, null);
    },

    /**
     * Retorna os itens para o controle de vendedores associados à clientes.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroVendedoresAssociadosAClientes: function () {
      return Servicos.Funcionarios.obterAtivosAssociadosAClientes();
    },

    /**
     * Retorna os itens para o controle de formas de pagamento de conta recebida.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroFormasPagamentoContasRecebidas: function () {
      return Servicos.FormasPagamento.obterFiltroContasRecebidas();
    },

    /**
     * Recupera os tipos de entrega de pedido para exibição nos filtros de conta recebida.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterItensFiltroTiposEntrega: function () {
      return Servicos.Pedidos.obterTiposEntrega();
    },

    /**
     * Retorna os itens para o controle de rotas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroRotas: function (id, codigo) {
      return Servicos.Rotas.obterFiltro(id, codigo);
    },

    /**
     * Retorna os itens para o controle de tipos contábeis.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposContabeis: function (id, codigo) {
      return Servicos.ContasReceber.obterTiposContabeis();
    },

    /**
     * Recupera os comissionados para o controle de comissionados.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroComissionados: function () {
      return Servicos.Comissionados.obterComissionados();
    },

    /**
     * Recupera as opções de busca de contas associadas ou não à notas fiscais.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroBuscaNfe: function () {
      return Servicos.ContasReceber.obterFiltroBuscaNfe();
    }
  },

  mounted: function () {
    this.filtroAtual.ordenacaoFiltro = 1;
    this.filtroAtual.buscaArquivoRemessa = 2;
    this.filtroAtual.buscarContasVinculadas = this.configuracoes.filtrarContasVinculadasPorPadrao;
  },

  watch: {
    /**
     * Observador para a variável 'recebidaPorAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    recebidaPorAtual: {
      handler: function(atual) {
        this.filtroAtual.recebidaPor = atual ? atual.id : null;
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
     * Observador para a variável 'vendedorAssociadoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    vendedorAssociadoAtual: {
      handler: function (atual) {
        this.filtroAtual.idVendedorAssociadoCliente = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'vendedorObraAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    vendedorObraAtual: {
      handler: function (atual) {
        this.filtroAtual.idVendedorObra = atual ? atual.id : null;
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
    },

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
     * Observador para a variável 'comissionadoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    comissionadoAtual: {
      handler: function (atual) {
        this.filtroAtual.idComissionado = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstContasRecebidas-Filtro-template'
});
