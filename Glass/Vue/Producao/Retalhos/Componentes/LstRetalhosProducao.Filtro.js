Vue.component('retalhos-producao-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de retalhos de produção.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },
  },

  data: function () {
    return {
      filtroAtual: this.merge(
        {
          codigoProduto: null,
          descricaoProduto: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          periodoUsoInicio: null,
          periodoUsoFim: null,
          situacao: null,
          idsCorVidro: [],
          espessura: null,
          alturaInicio: null,
          alturaFim: null,
          larguraInicio: null,
          larguraFim: null,
          codigoEtiqueta: null,
          observacao: null
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
     * Busca as situações de retalhos de produção para o filtro.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoesRetalhosProducao: function () {
      return Servicos.Producao.Retalhos.obterSituacoesParaFiltro();
    },

    /**
     * Busca as cores de vidro cadastradas.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterCores: function () {
      return Servicos.Produtos.CoresVidro.obterParaControle();
    }
  },

  watch: {
    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
    situacaoAtual: {
      handler: function (atual) {
        this.filtroAtual.situacao = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstRetalhosProducao-Filtro-template'
});
