Vue.component('planos-conta-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de planos de conta.
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
          idGrupoConta: null,
          situacao: null
        },
        this.filtro
      ),
      grupoContaAtual: null,
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
     * Retorna os itens para o controle de grupos de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroGruposConta: function () {
      return Servicos.PlanosConta.Grupos.obterParaControle();
    }
  },

  watch: {
    /**
     * Observador para a variável 'grupoContaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    grupoContaAtual: {
      handler: function (atual) {
        this.filtroAtual.idGrupoConta = atual ? atual.id : null;
      },
      deep: true
    },

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

  template: '#LstPlanosConta-Filtro-template'
});
