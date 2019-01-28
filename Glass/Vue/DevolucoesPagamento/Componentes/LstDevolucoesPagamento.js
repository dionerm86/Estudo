const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('dp.idDevolucaoPagto', 'Desc')],

  data: {
    filtro: {}
  },

  methods: {
    /**
     * Busca as devoluções de pagamentos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.DevolucoesPagamento.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém o link para geração de uma nova devolução de pagamento.
     * @returns {string} O link para geração da devolução de pagamento.
     */
    obterLinkInserirDevolucao: function () {
      var caixaDiario = GetQueryString('caixaDiario') == 'true';
      return ('../Cadastros/CadDevolucaoPagto.aspx' + caixaDiario ? '?caixaDiario=true' : '');
    },

    /**
     * Cancela uma devolução de pagamento.
     * @param {Object} item A devolução que será cancelada.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    cancelar: function (item) {
      this.abrirJanela(200, 500, '../Utils/SetMotivoCancReceb.aspx?tipo=devolucaoPagto&id=' + item.id);
    },

    /**
     * Exibe os dados detalhados da devolução do pagamento em um relatório.
     * @param {number} id A devolução do pagamento que será exibida.
     */
    abrirRelatorioDevolucao: function (id) {
      var url = '../Relatorios/RelBase.aspx?rel=DevolucaoPagto&idDevolucaoPagto=' + id;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Abre um popup com o gerenciamento de fotos para a devolução do pagamento.
     * @param {Object} item A devolução do pagamento que terá as fotos gerenciadas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    abrirGerenciamentoDeFotos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=devolucaoPagto');
    },

    /**
     * Exibe um relatório com a listagem de devoluções de pagamento.
     */
    abrirListaDevolucoes: function () {
      var url = '../Relatorios/RelBase.aspx?rel=ListaDevolucaoPagto&idCliente=' + this.formatarFiltros_();
      this.abrirJanela(600, 800, url);
    },

    /**
     * Formata os filtros para utilização na url.
     * @returns {string} Uma string com os filtros selecionados na tela.
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idCliente', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCliente', this.nomeCliente);
      this.incluirFiltroComLista(filtros, 'dataIni', this.periodoCadastroInicio);
      this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoCadastroFim);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },
  }
});
