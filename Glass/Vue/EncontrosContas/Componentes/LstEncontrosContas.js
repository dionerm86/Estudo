const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('idEncontroContas', 'asc')],
  data: {
    filtro: {}
  },

  methods: {
    /**
     * Busca os encontros de contas para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de acertos.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.EncontrosContas.obterListaEncontroContas(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtem o link para inserção de encontros de contas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterLinkInserirEncontrosContas: function () {
      return '../Cadastros/CadEncontroContas.aspx';
    },

    /**
     * Abre uma tela com a impressão do encontro de contas.
     * @param {Object} item O encontro de contas que será impresso.
     */
    imprimirEncontrosContas: function (item) {
      var url = '../Relatorios/RelBase.aspx?rel=EncontroContas&IdEncontroContas=' + item.id;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Obtem o link para editar um encontro de contas.
     * @returns {Promise} Uma Promisse com o resultado da busca.
     */
    editar: function (item) {
      return obterLinkInserir() + '&idEncontroContas=' + item.id;
    },

    /**
     * Cancela um encontro de contas.
     * @param {Object} item O encontro de conta que será cancelado.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    cancelar: function (item) {
      return this.abrirJanela(150, 450, '../Utils/SetMotivoCancEncontroContas.aspx?idEncontroContas=' + item.id);
    },

    /**
     * Formata os filtros para utilização na url.
     * @returns {string} Uma string com os filtros selecionados na tela.
     */
    formatarFiltros_: function () {
      var filtros = []

      this.incluirFiltroComLista(filtros, 'idEncontroContas', this.filtro.id);
      this.incluirFiltroComLista(filtros, 'idCliente', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCliente', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'idFornecedor', this.filtro.idFornecedor);
      this.incluirFiltroComLista(filtros, 'nomeFornecedor', this.filtro.nomeFornecedor);
      this.incluirFiltroComLista(filtros, 'obs', this.filtro.observacao);
      this.incluirFiltroComLista(filtros, 'dataCadIni', this.filtro.periodoCadastroInicio ? this.filtro.periodoCadastroInicio.toLocaleDateString('pt-BR') : null);
      this.incluirFiltroComLista(filtros, 'dataCadFim', this.filtro.periodoCadastroFim ? this.filtro.periodoCadastroFim.toLocaleDateString('pt-BR') : null);

      return filtros.length
        ? '&' + filtro.join('&')
        : '';
    },

    /**
     * Exibe um relatório com a listagem de encontros de contas aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaEncontrosContas: function (exportaExcel) {
      var url = '../Relatorios/RelBase.aspx?rel=ListaEncontroContas' + this.formatarFiltros_() + '&exportarexcel=' + exportaExcel;
      this.abrirJanela(600, 800, url);
    }    
  }
});