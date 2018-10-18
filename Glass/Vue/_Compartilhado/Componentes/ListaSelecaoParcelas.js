Vue.component('lista-selecao-parcelas', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Parcela selecionada.
     * @type {?number}
     */
    parcela: {
      required: true,
      twoWay: true,
      validador: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * ID do cliente que será usado para filtrar as parcelas
     * @type {?number}
     */
    idCliente: {
      required: false,
      twoWay: false,
      default: null,
      validador: Mixins.Validacao.validarNumeroOuVazio
    }
  },

  methods: {
    /**
     * Recupera as parcelas para exibição no controle.
     * @return {Promise} Uma Promise com o resultado da busca de lojas.
     */
    buscarParcelas: function (filtro) {
      return Servicos.Parcelas.obterParcelasCliente(filtro);
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna a parcela e que
     * atualiza a propriedade em caso de alteração.
     * @type {object}
     */
    parcelaAtual: {
      get: function() {
        return this.parcela;
      },
      set: function(valor) {
        if (!this.equivalentes(valor, this.parcela)) {
          this.$emit('update:parcela', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna o filtro de parcelas do cliente.
     * @type {filtroParcelasCliente}
     *
     * @typedef filtroParcelasCliente
     * @property {?number} idCliente O ID do cliente do pedido.
     */
    filtroParcelasCliente: function () {
      return {
        idCliente: this.idCliente || 0
      };
    }
  },

  template: '#ListaSelecaoParcelas-template'
})
