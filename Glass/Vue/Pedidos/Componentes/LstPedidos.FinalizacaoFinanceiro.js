Vue.component('pedido-finalizacao-financeiro', {
  mixins: [Mixins.OrdenacaoLista()],
  props: {
    /**
     * Pedido para busca de observações do financeiro.
     * @type {!Object}
     */
    pedido: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  methods: {
    /**
     * Realiza a busca de observações do financeiro de um pedido.
     * @param {!Object} filtroOriginal O filtro usado para a busca.
     * @param {!number} pagina O número da página atual na lista.
     * @param {!number} numeroRegistros O número de registros a serem exibidos na lista.
     * @param {string} ordenacao A ordenação a ser usada na busca.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    buscarObservacoesFinanceiro: function(filtroOriginal, pagina, numeroRegistros, ordenacao) {
      return Servicos.Pedidos.obterListaObservacoesFinanceiro(filtroOriginal.id, pagina, numeroRegistros, ordenacao);
    }
  },

  template: '#LstPedidos-FinalizacaoFinanceiro-template'
});
