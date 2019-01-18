const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString],

  data: {
    filtro: {}
  },

  methods: {
    /**
     * Busca a lista de chapas disponíveis.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Produtos.MateriaPrima.ChapasDisponiveis.obterLista(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Recupera o objeto com as cores.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterItensFiltroCor: function () {
      return Servicos.Produtos.CoresVidro.obterParaControle();
    },

    /**
     * Retornar uma string com os filtros selecionados na tela.
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idFornecedor', this.filtro.idFornecedor);
      this.incluirFiltroComLista(filtros, 'nomeFornecedor', this.filtro.nomeFornecedor);
      this.incluirFiltroComLista(filtros, 'codigoProduto', this.filtro.codigoProduto);
      this.incluirFiltroComLista(filtros, 'descricaoProduto', this.filtro.descricaoProduto);
      this.incluirFiltroComLista(filtros, 'numeroNotaFiscal', this.filtro.numeroNotaFiscal);
      this.incluirFiltroComLista(filtros, 'lote', this.filtro.lote);
      this.incluirFiltroComLista(filtros, 'altura', this.filtro.altura);
      this.incluirFiltroComLista(filtros, 'largura', this.filtro.largura);
      this.incluirFiltroComLista(filtros, 'idsCorVidro', this.filtro.idsCorVidro);
      this.incluirFiltroComLista(filtros, 'espessura', this.filtro.espessura);
      this.incluirFiltroComLista(filtros, 'codigoEtiqueta', this.filtro.codigoEtiqueta);
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Recupera o objeto com os dados para impressão.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    abrirRelatorio: function (exportarExcel) {
      var url = '../Relatorios/RelBase.aspx?rel=ChapasDisponiveis' + this.formatarFiltros_() + '&exportarexcel=' + exportarExcel;
      this.abrirJanela(600, 800, url);
    },
  }
});
