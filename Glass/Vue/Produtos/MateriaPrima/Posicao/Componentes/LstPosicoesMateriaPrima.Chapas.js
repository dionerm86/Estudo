Vue.component('posicoes-materia-prima-chapas', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de chapas associadas a posição de matéria prima.
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
      totalizadores: {}
    }
  },

  methods: {
    /**
     * Busca a lista de chapas de matéria prima.
     * @param {filtro} filtro Os filtros para busca das chapas de matéria prima.
     */
    buscarChapas: function (filtro) {
      return Servicos.Produtos.MateriaPrima.obterLista(filtro);
    },
    
    /**
     * Função que calcula os totais da lista de chapas de matéria prima
     */
    calcularTotais: function () {
      var itens = this.$refs.lista.itens;

      this.totalizadores = {
        quantidade: 0,
        metroQuadradoDisponivel: 0
      };

      for (var i in itens) {
        this.totalizadores.quantidade += itens[i].quantidade;
        this.totalizadores.metroQuadradoDisponivel += itens[i].totalMetroQuadrado;
      }
    },

    /**
     * Indica externamente que os itens em exibição foram atualizados.
     * @param {!number} numeroItens O número de itens retornados para exibição.
     */
    atualizouItens: function (numeroItens) {
      this.$emit('atualizou-itens', numeroItens);
      this.calcularTotais();
    }
  },

  template: '#LstPosicoesMateriaPrima-Chapas-template'
});
