const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('dataExportacao', 'DESC')],

  data: {
    filtro: {}
  },

  methods: {
    /**
     * Busca os carregamentos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Exportacao.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    obterSituacaoPedidoExportacao: function (id) {
      return Servicos.Exportacao.obterSituacao(id);
    },

    abrirRelatorio: function (id) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=Exportacao&idExportacao=' + id);
    }
  } 
});
