Vue.component('pedidos-exportacao-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de exportacao de pedidos.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      filtroAtual: this.merge(
        {
          id: null,
          idPedido: null,
          situacao: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
        },
        this.filtro
      ),
      situacaoAtual: null
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
     * Retorna os itens para o controle de situações.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacao: function () {
      return Servicos.Exportacao.obterFiltroSituacoes();
    }
  },

  watch: {
    /**
      * Observador para a variável 'situacaoAtual'.
      * Atualiza o filtro com o ID do item selecionado.
      */
    situacaoAtual: {
      handler: function (atual) {
        this.filtroAtual.situacao = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstPedidosExportacao-Filtro-template'
});
