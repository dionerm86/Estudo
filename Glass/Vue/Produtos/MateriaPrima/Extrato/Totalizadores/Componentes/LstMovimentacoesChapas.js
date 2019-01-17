const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('IdCorVidro', 'ASC')],

  data: {
    filtro: {
      periodoMovimentacaoInicio: new Date(),
      periodoMovimentacaoFim: new Date()
    },
    movimentacoesEmExibicao: []
  },

  methods: {
    /**
     * Busca os itens para o extrato de movimentação de chapas de vidro.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de produtos.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Produtos.MateriaPrima.Extrato.Totalizadores.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Exibe o relatório com detalhamento das movimentações de chapas de vidro.
     * @param {Boolean} exportarExcel Define se o relatório é para ser exibido na tela ou exportado para o excel.
     * @param {Boolean} exibirDetalhes Define se o relatório que será gerado é detalhado ou não.
     */
    abrirRelatorio: function (exportarExcel, exibirDetalhes) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=MovChapa' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel + '&exibirDetalhes=' + exibirDetalhes);
    },

    /**
     * Indica se as movimentações estão sendo exibidas na lista.
     * @param {!number} indice O número da linha que está sendo verificada.
     * @returns {boolean} Verdadeiro, se os itens estiverem sendo exibidos.
     */
    exibindoMovimentacoes: function (indice) {
      return this.movimentacoesEmExibicao.indexOf(indice) > -1;
    },

    /**
     * Alterna a exibição das movimentações.
     */
    alternarExibicaoMovimentacoes: function (indice) {
      var i = this.movimentacoesEmExibicao.indexOf(indice);

      if (i > -1) {
        this.movimentacoesEmExibicao.splice(i, 1);
      } else {
        this.movimentacoesEmExibicao.push(indice);
      }
    },

    /**
     * Limpa a lista de movimentações em exibição ao realizar paginação da lista principal.
     */
    atualizouItens: function () {
      this.movimentacoesEmExibicao.splice(0, this.movimentacoesEmExibicao.length);
    },

    /**
     * Retorna uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idsCorVidro', this.filtro.idsCorVidro);
      this.incluirFiltroComLista(filtros, 'espessura', this.filtro.espessura);
      this.incluirFiltroComLista(filtros, 'altura', this.filtro.altura);
      this.incluirFiltroComLista(filtros, 'largura', this.filtro.largura);
      this.incluirFiltroComLista(filtros, 'dataIni', this.filtro.periodoMovimentacaoInicio);
      this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoMovimentacaoFim);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Retorna o número de colunas da lista paginada.
     * @type {number}
     */
    numeroColunasLista: function () {
      return this.$refs.lista.numeroColunas();
    },

    /**
     * Atualiza a lista de produtos
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  }
});
