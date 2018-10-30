Vue.component('lista-selecao-situacoes', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Situação selecionada.
     * @type {?object}
     */
    situacao: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    }
  },

  methods: {
    /**
     * Recupera as situações para exibição no controle.
     * @return {Promise} Uma Promise com o resultado da busca de situações.
     */
    buscarSituacoes: function() {
      return Servicos.Comum.obterSituacoes();
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna a situação selecionada e que
     * atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    situacaoAtual: {
      get: function() {
        return this.situacao;
      },
      set: function(valor) {
        if (!this.equivalentes(valor, this.situacao)) {
          this.$emit('update:situacao', valor);
        }
      }
    }
  },

  template: '#ListaSelecaoSituacoes-template'
});
