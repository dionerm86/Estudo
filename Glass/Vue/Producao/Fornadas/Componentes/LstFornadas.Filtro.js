Vue.component('fornadas-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de fornadas.
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
          codigoEtiqueta: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          idsCorVidro: null,
          espessura: null
        },
        this.filtro
      )
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
     * Busca as cores de vidros para uso no controle.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterItensFiltroCoresVidro: function () {
      return Servicos.Produtos.CoresVidro.obterParaControle();
    }
  },

  template: '#LstFornadas-Filtro-template'
});
