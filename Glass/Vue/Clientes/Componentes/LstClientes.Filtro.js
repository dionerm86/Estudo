Vue.component('cliente-filtros', {
  mixins: [Mixins.Clonar, Mixins.Merge, Mixins.Comparar],
  props: {
    /**
     * Filtros selecionados para a lista de clientes.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function() {
    return {
      filtroAtual: this.merge(
        {
          id: null,
          nomeCliente: null,
          cpfCnpj: null,
          idLoja: null,
          telefone: null,
          uf: null,
          idCidade: null,
          bairro: null,
          endereco: null,
          tipo: null,
          situacao: null,
          codigoRota: null,
          idVendedor: null,
          tipoFiscal: null,
          formasPagamento: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          periodoSemCompraInicio: null,
          periodoSemCompraFim: null,
          periodoInativadoInicio: null,
          periodoInativadoFim: null,
          idTabelaDescontoAcrescimo: null,
          apenasSemRota: null
        },
        this.filtro
      ),
      situacaoAtual: null,
      lojaAtual: null,
      cidadeAtual: null,
      rotaAtual: null,
      tabelaDescontoAcrescimoAtual: null
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
     * Retorna os itens para o controle de situações de cliente.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoesCliente: function() {
      return Servicos.Clientes.obterSituacoes();
    },

    /**
     * Retorna os itens para o controle de tipo de cliente.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTipo: function () {
      return Servicos.Clientes.obterTipos();
    },

    /**
     * Retorna os itens para o controle de tipo fiscal de cliente.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTipoFiscal: function () {
      return Servicos.Clientes.obterTiposFiscal();
    },

    /**
     * Retorna os itens para o controle de formas de pagamento
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroFormasPagamento: function () {
      return Servicos.FormasPagamento.obterFiltro();
    },

    /**
     * Retorna os itens para o controle de vendedores.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroVendedores: function () {
      return Servicos.Funcionarios.obterAtivosAssociadosAClientes();
    },

    /**
     * Retorna os itens para o controle de tabelas de desconto/acréscimo.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTabelaDescontoAcrescimo: function () {
      return Servicos.TabelasDescontoAcrescimoCliente.obterFiltro();
    },

    /**
     * Retorna os itens para o controle de rotas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroRotas: function () {
      return Servicos.Rotas.obterFiltro();
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
     * Observador para a variável 'cidadeAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    cidadeAtual: {
      handler: function(atual) {
        this.filtroAtual.idCidade = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'rotaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    rotaAtual: {
      handler: function (atual) {
        this.filtroAtual.codigoRota = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tabelaDescontoAcrescimoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tabelaDescontoAcrescimoAtual: {
      handler: function (atual) {
        this.filtroAtual.idTabelaDescontoAcrescimo = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstClientes-Filtro-template'
});
