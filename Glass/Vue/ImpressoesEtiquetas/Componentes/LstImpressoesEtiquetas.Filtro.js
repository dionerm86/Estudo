Vue.component('impressoes-etiquetas-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de impressões de etiquetas.
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
          idPedido: null,
          numeroNotaFiscal: null,
          planoCorte: null,
          loteProdutoNotaFiscal: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          tipoImpressao: null,
          codigoEtiqueta: null
        },
        this.filtro
      ),
      tipoAtual: null
    };
  },

  methods: {
    /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
    filtrar: function() {
      var novoFiltro = this.clonar(this.filtroAtual);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }
    },

    /**
     * Retorna os itens para o controle de situações de fornecedor.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTipos: function () {
      return Servicos.ImpressoesEtiquetas.obterTipos();
    }
  },

  watch: {
    /**
     * Observador para a variável 'tipoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoAtual: {
      handler: function (atual) {
        this.filtroAtual.tipoImpressao = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstImpressoesEtiquetas-Filtro-template'
});
