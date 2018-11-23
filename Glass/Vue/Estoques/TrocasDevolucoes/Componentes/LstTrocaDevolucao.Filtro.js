Vue.component('trocas-devolucoes-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de contas recebidas.
     * @type {Object}
     */
    filtro: {
      required: false,
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

  data: function () {
    return {
      filtroAtual: this.merge(
        {
          codigo: null,
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
        },
        this.filtro
      ),
      vendedorAtual: null,
      lojaAtual: null,
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
     * Observador para a variável 'vendedorAssociadoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    vendedorAssociadoAtual: {
      handler: function (atual) {
        this.filtroAtual.idVendedorAssociadoCliente = atual ? atual.id : null;
      },
      deep: true
    },
  },

  template: '#LstTrocaDevolucao-Filtro-template'
});
