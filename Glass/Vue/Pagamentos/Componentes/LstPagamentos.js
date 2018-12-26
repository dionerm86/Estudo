const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('idPagto', 'desc')],

  data: {
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca os pagamentos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca de pagamentos.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de orçamentos.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Pagamentos.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Exibe as informações detalhadas referentes ao pagamento em uma nova janela.
     * @param {Object} item O pagamento que terá suas informações exibidas.
     */
    abrirRelatorio: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=Pagto&idPagto=' + item.id);
    },

    /**
     * Exibe uma nova janela onde o pagamento pode ser cancelado ao se informar o motivo do cancelamento.
     * @param {Object} item O pagamento que será cancelado.
     */
    abrirMotivoCancelamento: function (item) {
      this.abrirJanela(180, 410, '../Utils/SetMotivoCancPagto.aspx?idPagto=' + item.id);
    },

    /**
     * Exibe uma nova janela onde as fotos anexas ao pagamento informado poderão ser gerenciadas.
     * @param {Object} item O pagamento que terá suas fotos anexas gerenciadas.
     */
    abrirGerenciamentoDeFotosAnexas: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=pagto');
    },

    /**
     * Força a atualização da lista de orçamentos, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  }
});
