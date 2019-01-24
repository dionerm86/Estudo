Vue.component('movimentacoes-chapas-movimentacoes', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Lista de movimentações a serem exibidas na listagem.
     * @type {Object}
     */
    movimentacoes: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  template: '#LstMovimentacoesChapas-Movimentacoes-template'
});
