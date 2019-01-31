const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('IdFornada', 'DESC')],

  data: {
    filtro: {},
    pecasEmExibicao: []
  },

  methods: {
    /**
     * Busca as fornadas para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de estoques de produto.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Producao.Fornadas.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Exibe o relatório com detalhamento das fornadas para os filtros préviamente informados.
     * @param {Boolean} exportarExcel Define se o relatório deve ser exportado para um arquivo do excel.
     * @param {Boolean} analitico Define se o relatório a ser gerado é analitico.
     */
    abrirRelatorio: function (analitico, exportarExcel) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?Rel=Fornada&analitico=' + analitico + "&exportarExcel=" + exportarExcel + this.formatarFiltros_());
    },

    /**
     * Retorna uma string com os filtros selecionados na tela.
     * @returns {string} uma string com os parâmetros que serão passados para o relatório.
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idFornada', this.filtro.id);
      this.incluirFiltroComLista(filtros, 'idPedido', this.filtro.idPedido);
      this.incluirFiltroComLista(filtros, 'dataIni', this.filtro.periodoCadastroInicio);
      this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoCadastroFim);
      this.incluirFiltroComLista(filtros, 'numEtiqueta', this.filtro.codigoEtiqueta);
      this.incluirFiltroComLista(filtros, 'espessura', this.filtro.espessura);
      this.incluirFiltroComLista(filtros, 'idsCorVidro', this.filtro.idsCorVidro);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Indica se as peças de fornada estão sendo exibidos em um dado indice da lista.
     * @param {!number} indice O número da linha que está sendo verificada.
     * @returns {boolean} Verdadeiro, se os itens estiverem sendo exibidos.
     */
    verificarExibicaoPecasFornada: function (indice) {
      return this.pecasEmExibicao.indexOf(indice) > -1;
    },

    /**
     * Alterna a exibição das peças de fornada.
     * @param {number} indice O número da linha que terá a exibição de suas peças alternada.
     */
    alternarExibicaoPecas: function (indice) {
      var i = this.pecasEmExibicao.indexOf(indice);

      if (i > -1) {
        this.pecasEmExibicao.splice(i, 1);
      } else {
        this.pecasEmExibicao.push(indice);
      }
    },

    /**
     * Retorna o número de colunas da lista paginada.
     * @returns {number} O número de colunas existente na lista.
     */
    obterNumeroColunasLista: function () {
      return this.$refs.lista.numeroColunas();
    },

    /**
     * Limpa a lista de peças de fornada em exibição ao realizar paginação da lista principal.
     */
    limparPecasEmExibicao: function () {
      this.pecasEmExibicao.splice(0, this.pecasEmExibicao.length);
    },

    /**
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },
  }
});
