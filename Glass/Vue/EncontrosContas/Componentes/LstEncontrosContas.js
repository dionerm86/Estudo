const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('idEncontroContas', 'asc')],
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
     * Retorna uma string com os filtros selecionados na tela.
     */
    formatarFiltros_: function () {
      var filtros = []
      const incluirFiltro = function (campo, valor) {
        if (valor) {
          filtros.push(campo + '=' + valor);
        }
      }

      incluirFiltro('idEncontroContas', this.filtro.id);
      incluirFiltro('idCliente', this.filtro.idCliente);
      incluirFiltro('nomeCliente', this.filtro.nomeCliente);
      incluirFiltro('idFornecedor', this.filtro.idFornecedor);
      incluirFiltro('nomeFornecedor', this.filtro.nomeFornecedor);
      incluirFiltro('obs', this.filtro.observacao);
      incluirFiltro('dataCadIni', this.filtro.periodoCadastroInicio ? this.filtro.periodoCadastroInicio.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dataCadFim', this.filtro.periodoCadastroFim ? this.filtro.periodoCadastroFim.toLocaleDateString('pt-BR') : null);

      return filtros.length > 0
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