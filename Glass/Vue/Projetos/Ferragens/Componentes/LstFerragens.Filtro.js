Vue.component('ferragens-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de ferragens.
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
          codigo: null,
          nome: null,
          idFabricante: null
        },
        this.filtro
      ),
      fabricanteAtual: null
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
     * Busca os grupos de projeto para o filtro.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterItensFiltroFabricante: function () {
      return Servicos.Projetos.Ferragens.Fabricantes.obterParaControle();
    }
  },

  watch: {
    /**
     * Observador para a variável 'fabricanteAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
    fabricanteAtual: {
      handler: function (atual) {
        this.filtroAtual.idFabricante = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstFerragens-Filtro-template'
});
