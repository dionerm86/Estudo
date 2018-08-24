Vue.component('lista-selecao-lojas', {
  mixins: [Mixins.Comparar],
  props: {
    /**
     * Loja selecionada.
     * @type {?object}
     */
    loja: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Indica se apenas lojas ativas (ou não) serão exibidas.
     * @type {?boolean}
     */
    ativas: {
      required: false,
      default: null,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Indica se o item 'Todas' deve ser exibido.
     * @type {?boolean}
     */
    exibirTodas: {
      required: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  methods: {
    /**
     * Recupera as lojas para exibição no controle.
     * @return {Promise} Uma Promise com o resultado da busca de lojas.
     */
    buscarLojas: function() {
      return Servicos.Lojas.obterParaControle(this.ativas);
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna a loja selecionada e que
     * atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    lojaAtual: {
      get: function() {
        return this.loja;
      },
      set: function(valor) {
        if (!this.equivalentes(valor, this.loja)) {
          this.$emit('update:loja', valor);
        }
      }
    }
  },

  template: '#ListaSelecaoLojas-template'
});
