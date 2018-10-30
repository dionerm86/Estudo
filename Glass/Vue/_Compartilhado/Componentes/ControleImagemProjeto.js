Vue.component('controle-imagem-projeto', {
  props: {
    /**
     * Identificador do produto do pedido conferência que contém a imagem.
     * @type {?number}
     */
    idProdutoPedidoConferencia: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Tamanho máximo permitido para a imagem.
     * @type {tamanhoMaximo}
     *
     * @typedef {Object} tamanhoMaximo
     * @property {?number} altura A altura máxima da imagem.
     * @property {?number} largura A largura máxima da imagem.
     */
    tamanhoMaximo: {
      required: false,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarObjetoOuVazio
    }
  },

  template: '#ControleImagemProjeto-template'
})
