Vue.component('modelos-projeto-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de modelos de projeto.
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
          descricao: null,
          idGrupoModelo: null,
          situacao: null
        },
        this.filtro
      ),
      grupoAtual: null,
      situacaoAtual: null
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
    obterItensFiltroGrupo: function () {
      return Servicos.Projetos.Grupos.obterParaControle();
    },

    /**
     * Busca as situações de modelo de projeto para o filtro.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterItensFiltroSituacao: function () {
      return Servicos.Projetos.Modelos.obterSituacoes();
    }
  },

  watch: {
    /**
     * Observador para a variável 'grupoAtual'.
     * Atualiza o filtro com os dados do item selecionado.
     */
    grupoAtual: {
      handler: function (atual) {
        this.filtroAtual.idGrupoModelo = atual ? atual.id : null;
      },
      deep: true
    },

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

  template: '#LstModelosProjeto-Filtro-template'
});
