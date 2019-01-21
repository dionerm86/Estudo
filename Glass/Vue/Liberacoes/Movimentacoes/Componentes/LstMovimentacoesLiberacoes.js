const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('IdLiberarPedido', 'DESC')],

  data: {
    filtro: {
      periodoCadastroInicio: new Date(),
      periodoCadastroFim: new Date()
    }
  },

  methods: {
    /**
     * Busca as movimentações de liberações para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Liberacoes.MovimentacoesLiberacoes.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela.
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idCliente', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCliente', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'idFuncionario', this.filtro.idFuncionario);
      this.incluirFiltroComLista(filtros, 'periodoCadastroInicio', this.filtro.periodoCadastroInicio);
      this.incluirFiltroComLista(filtros, 'periodoCadastroFim', this.filtro.periodoCadastroFim);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Exibe um relatório com a listagem de movimentações de liberações, aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaMovimentacoesLiberacoes: function (semValor, exportarExcel) {
      var url = 'RelBase.aspx?rel=LiberarPedidoMov' + (semValor ? 'semValor' : '') + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel;
      this.abrirJanela(600, 800, url);
    }
  }
});
