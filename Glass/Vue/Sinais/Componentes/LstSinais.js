const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    filtro: {},
    caixaDiario: false,
    pagamentoAntecipado: false
  },

  methods: {
    /**
     * Busca os sinais para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de sinais.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      filtroUsar.pagamentoAntecipado = this.pagamentoAntecipado;
      return Servicos.Sinais.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna o link para a tela de pagamento de sinal.
     * @returns {string} O link para a tela de efetuar pagamento do sinal.
     */
    obterLinkPagarSinal: function () {
      return '../Cadastros/CadReceberSinal.aspx?antecipado=1&cxDiario=' + (this.caixaDiario ? '1' : '');
    },

    /**
     * Abre uma tela de cancelamento de sinal.
     * @param {Object} item O sinal que será cancelado.
     */
    abrirCancelamento: function (item) {
      this.abrirJanela(200, 500, '../Utils/SetMotivoCancReceb.aspx?tipo=' + (this.pagamentoAntecipado ? 'pagamento antecipado' : 'sinal') + '&id=' + item.id);
    },

    /**
     * Abre uma tela com a impressão do sinal.
     * @param {Object} item O sinal que será impresso.
     */
    abrirImpressaoSinal: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=Sinal&IdSinal=' + item.id);
    },

    /**
     * Abre uma tela com os anexos do sinal.
     * @param {Object} item O sinal que será usado para visualizar os anexos.
     */
    abrirAnexos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?tipo=pagtoAntecipado&id=' + item.id);
    },

    /**
     * Exibe um relatório com a listagem de sinais aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaSinais: function (exportarExcel) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=SinaisRecebidos' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idSinal', this.filtro.id);
      this.incluirFiltroComLista(filtros, 'idPedido', this.filtro.idPedido);
      this.incluirFiltroComLista(filtros, 'idCli', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'dataIni', this.filtro.periodoCadastroInicio);
      this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoCadastroFim);
      this.incluirFiltroComLista(filtros, 'pagtoAntecipado', this.pagamentoAntecipado);
      this.incluirFiltroComLista(filtros, 'ordenacao', this.filtro.ordenacaoFiltro);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  },

  created: function() {
    this.caixaDiario = GetQueryString('cxDiario') == 1;
    this.pagamentoAntecipado = GetQueryString('antecipado') == 1;
  }
});
