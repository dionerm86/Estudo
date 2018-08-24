Vue.component('lista-selecao-id-valor', {
  mixins: [Mixins.Comparar],
  props: {
    /**
     * Item selecionado no controle.
     * @type {?object}
     */
    itemSelecionado: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Função utilizada para a recuperação dos itens para o controle.
     * @type {!function}
     * @returns {Promise}
     */
    funcaoRecuperarItens: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarFuncao
    },

    /**
     * Filtro utilizado para a execução da função de recuperação dos itens para o controle.
     * @type {?object}
     */
    filtroRecuperarItens: {
      required: false,
      twoWay: false,
      default: function () {
        return {}
      },
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Texto exibido caso não exista item selecionado.
     * @type {?string}
     */
    textoSelecionar: {
      required: false,
      twoWay: false,
      default: '',
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * Indica se os itens devem ser ordenados para exibição.
     * @type {?boolean}
     */
    ordenar: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Nome do campo do item que contém o ID.
     * @type {?string}
     */
    campoId: {
      required: false,
      twoWay: false,
      default: 'id',
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * Nome do campo do item que contém o valor a ser exibido.
     * @type {?string}
     */
    campoValor: {
      required: false,
      twoWay: false,
      default: 'nome',
      validator: Mixins.Validacao.validarStringOuVazio
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o item selecionado e que
     * atualiza a propriedade em caso de alteração.
     * @type {object}
     */
    itemSelecionadoAtual: {
      get: function() {
        return this.itemSelecionado;
      },
      set: function(valor) {
        if (!this.equivalentes(valor, this.itemSelecionado)) {
          this.$emit('update:itemSelecionado', valor);
        }
      }
    }
  },

  template: '#ListaSelecaoIdValor-template'
});
