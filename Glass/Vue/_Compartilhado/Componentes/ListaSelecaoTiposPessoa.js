Vue.component('lista-selecao-tipos-pessoa', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Tipo pessoa selecionado.
     * @type {?object}
     */
    tipoPessoa: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    }
  },

  methods: {
    /**
     * Recupera os tipos de pessoa para exibição no controle.
     * @return {Promise} Uma Promise com o resultado da busca de situações.
     */
    buscarTiposPessoa: function() {
      return Servicos.Comum.obterTiposPessoa();
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o tipo pessoa selecionado e que
     * atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    tipoPessoaAtual: {
      get: function() {
        return this.tipoPessoa;
      },
      set: function(valor) {
        if (!this.equivalentes(valor, this.tipoPessoa)) {
          this.$emit('update:tipoPessoa', valor);
        }
      }
    }
  },

  template: '#ListaSelecaoTiposPessoa-template'
});
