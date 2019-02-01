const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('idAcertoCheque', 'desc')],
  data: function () {
    return {
      filtro: {
        buscarAcertosCaixaDiario: GetQueryString('caixaDiario') == 'true' ? true : false,
        buscarAcertosChequesProprios: GetQueryString('pagto')
      }
    }
  },

  methods: {
    /**
     * Busca os acertos de cheques para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de acertos.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.AcertosCheques.obterListaAcertoCheque(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Cancela um acerto de cheque.
     * @param {Object} item O acerto de cheque que será cancelado.
     */
    cancelar: function (item) {
      this.abrirJanela(200, 500, '../Utils/SetMotivoCancReceb.aspx?tipo=acertoCheque&id=' + item.id);
    },

    /**
     * Exibe os dados detalhados do acerto de cheque selecionado.
     * @param {Object} item O acerto de cheque que será exibido.
     */
    abrirRelatorio: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=AcertoCheque&idAcertoCheque=' + item.id);
    },

    /**
     * Formata os filtros para utilização na url.
     * @returns {string} Uma string com os filtros selecionados na tela.
     */
    formatarFiltros_: function () {
      var filtros = []

      this.incluirFiltroComLista(filtros, 'idAcertoCheque', this.filtro.id);
      this.incluirFiltroComLista(filtros, 'idFunc', this.filtro.idFuncionario);
      this.incluirFiltroComLista(filtros, 'idCliente', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCliente', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'dataIni', this.filtro.periodoCadastroInicio);
      this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoCadastroFim);
      this.incluirFiltroComLista(filtros, 'chequesProprios', this.filtro.buscarAcertosChequesProprios);
      this.incluirFiltroComLista(filtros, 'chequesCaixaDiario', this.filtro.buscarAcertosCaixaDiario);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Exibe os dados detalhados do acerto de cheque em um relatório.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaAcertosCheques: function (exportaExcel) {
      var url = '../Relatorios/RelBase.aspx?Rel=AcertoChequesDevolvidos' + this.formatarFiltros_() + '&exportarExcel=' + exportaExcel;
      this.abrirJanela(600, 800, url);
    }    
  }
});