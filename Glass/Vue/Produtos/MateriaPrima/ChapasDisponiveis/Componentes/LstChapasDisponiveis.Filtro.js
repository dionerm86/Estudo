Vue.component('chapas-disponiveis-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de chapas disponíveis.
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
          idsCorVidro: null,
          espessura: null,
          idFornecedor: null,
          nomeFornecedor: null,
          numeroNotaFiscal: null,
          lote: null,
          altura: null,
          largura: null,
          codigoProduto: null,
          descricaoProduto: null,
          codigoEtiqueta: null,
          idLoja: null
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
    filtrar: function () {
      var novoFiltro = this.clonar(this.filtroAtual);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }
    }
  },

  watch: {
  /**
   * Observador para a variável 'lojaAtual'.
   * Atualiza o filtro com o ID do item selecionado.
   */
    lojaAtual: {
      handler: function (atual) {
        this.filtroAtual.idLoja = atual ? atual.id : null;
      }
    }
  }, 

  template: '#LstChapasDisponiveis-Filtro-template'
});
