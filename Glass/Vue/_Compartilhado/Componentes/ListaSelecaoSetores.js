Vue.component('lista-selecao-setores', {
  mixins: [Mixins.Comparar],
  props: {
    /**
     * Setor selecionado.
     * @type {?object}
     */
    setor: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Indica se o setor de impressão de etiqueta deve ser incluído no controle.
     * @type {boolean}
     */
    incluirSetorImpressao: {
      required: false,
      default: true,
      validator: Mixins.Validacao.validarBoolean
    },

    /**
     * Indica se deverá ser incluído um setor 'Etiqueta não impressa' no controle.
     * @type {boolean}
     */
    incluirEtiquetaNaoImpressa: {
      required: false,
      default: false,
      validator: Mixins.Validacao.validarBoolean
    }
  },

  methods: {
    /**
     * Recupera os setores para exibição no controle.
     * @return {Promise} Uma Promise com o resultado da busca de setores.
     */
    buscarSetores: function () {
      return Servicos.Producao.Setores.obterParaControle(
        this.incluirSetorImpressao,
        this.incluirEtiquetaNaoImpressa
      );
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o setor selecionado e que
     * atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    setorAtual: {
      get: function() {
        return this.setor;
      },
      set: function(valor) {
        if (!this.equivalentes(valor, this.setor)) {
          this.$emit('update:setor', valor);
        }
      }
    }
  },

  template: '#ListaSelecaoSetores-template'
});
