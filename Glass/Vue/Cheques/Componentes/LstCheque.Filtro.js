Vue.component('cheques-filtros', {
  mixins: [Mixins.Clonar, Mixins.Patch, Mixins.Merge, Mixins.Comparar],
  props: {
    /**
     * Filtros selecionados para a lista de cheques.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de cheques.
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
          idLoja: null,
          idPedido: null,
          idLiberacao: null,
          idAcerto: null,
          numeroNfe: null,
          tipo: null,
          numeroCheque: null,
          situacao: null,
          reapresentado: null,
          advogado: null,
          titular: null,
          agencia: null,
          conta: null,
          periodoVencimentoInicio: null,
          periodoVencimentoFim: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          cpfCnpj: null,
          idCliente: null,
          nomeCliente: null,
          idFornecedor: null,
          nomeFornecedor: null,
          valorChequeInicio: null,
          valorChequeFim: null,
          usuarioCadastro: null,
          exibirApenasCaixaDiario: null,
          idsRota: null,
          observacao: null,
          ordenacaoFiltro: null,
          agruparCliente: null
        },
        this.filtro
      ),
      lojaAtual: null
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
     * Retorna os itens para o controle de situações.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoes: function() {
      return Servicos.Cheques.obterSituacoes();
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
      handler: function(atual) {
        this.filtroAtual.idLoja = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a propriedade 'configuracoes'.
     * Inicia o filtro de tipos e realiza o filtro.
     */
    configuracoes: {
      handler: function (atual) {
        this.filtroAtual.tipo = atual ? atual.tipoChequeTerceiros : null;
        this.filtrar();
      },
      deep: true
    }
  },

  template: '#LstCheque-Filtro-template'
});
