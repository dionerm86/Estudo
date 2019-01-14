Vue.component('perdas-chapas-vidro-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de perdas de chapas de vidro.
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
          idTipoPerda: null,
          idSubtipoPerda: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          codigoEtiqueta: null
        },
        this.filtro
      ),
      tipoPerdaAtual: null,
      subtipoPerdaAtual: null
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
     * Busca os tipos de perda para o filtro.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposPerda: function () {
      return Servicos.Producao.TiposPerda.obterParaFiltro();
    },

    /**
     * Busca os subtipos de perda para o filtro.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSubtiposPerda: function (filtro) {
      var idTipoPerda = (filtro || {}).idTipoPerda || 0;
      return Servicos.Producao.TiposPerda.SubtiposPerda.obterParaFiltro(idTipoPerda);
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o filtro de subtipos de perda.
     * @type {filtroSubtiposPerda}
     *
     * @typedef filtroSubtiposPerda
     * @property {?number} idTipoPerda O ID do tipo de perda.
     */
    filtroSubtiposPerda: function () {
      return {
        idTipoPerda: (this.filtroAtual || {}).idTipoPerda || 0
      };
    }
  },

  watch: {
    /**
     * Observador para a variável 'tipoPerdaAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
    tipoPerdaAtual: {
      handler: function (atual) {
        this.filtroAtual.idTipoPerda = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'subtipoPerdaAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
    subtipoPerdaAtual: {
      handler: function (atual) {
        this.filtroAtual.idSubtipoPerda = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstPerdasChapasVidro-Filtro-template'
});
