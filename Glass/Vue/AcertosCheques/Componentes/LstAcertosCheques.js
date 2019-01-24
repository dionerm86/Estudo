const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('idAcertoCheque', 'desc')],
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
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    cancelar: function (item) {
      this.abrirJanela(200, 500, '../Utils/SetMotivoCancReceb.aspx?tipo=acertoCheque&id=' + item.id);
    },

    /**
     * Exibe os dados detalhados do acerto de cheque.
     * @param {Object} id O acerto de cheque que será exibido.
     */
    abrirRelatorio: function (id) {
      var url = '../Relatorios/RelBase.aspx?rel=AcertoCheque&idAcertoCheque=' + id;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Formata os filtros para utilização na url.
     */
    formatarFiltros_: function () {
      var filtros = []
      const incluirFiltro = function (campo, valor) {
        if (valor) {
          filtros.push(campo + '=' + valor);
        }
      }

      incluirFiltro('idAcertoCheque', this.filtro.id);
      incluirFiltro('idFunc', this.filtro.idFuncionario);
      incluirFiltro('idCliente', this.filtro.idCliente);
      incluirFiltro('nomeCliente', this.filtro.nomeCliente);
      incluirFiltro('dataIni', this.filtro.periodoCadastroInicio ? this.filtro.periodoCadastroInicio.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dataFim', this.filtro.periodoCadastroFim ? this.filtro.periodoCadastroFim.toLocaleDateString('pt-BR') : null);
      incluirFiltro('chequesProprios', this.filtro.buscarAcertosChequesProprios);
      incluirFiltro('chequesCaixaDiario', this.filtro.buscarAcertosCaixaDiario);

      return filtros.length > 0
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Exibe os dados detalhados do acerto de cheque em um relatório.
     * @param {Object} item O acerto de cheque que será exibido.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaAcertosCheques: function (exportaExcel) {
      var url = '../Relatorios/RelBase.aspx?Rel=AcertoChequesDevolvidos' + this.formatarFiltros_() + '&exportarExcel=' + exportaExcel;
      this.abrirJanela(600, 800, url);
    }    
  },

  mounted: function () {
    if (this.filtro.buscarAcertosChequesProprios && this.filtro.buscarAcertosChequesProprios == 1) {
      document.title = 'Acerto de Cheques Próprios Devolvidos/Abertos';
    }
  }
});