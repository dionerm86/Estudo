const app = new Vue({
  el: '#app',
  mixins: [Mixins.Data, Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('DescrCorVidro', 'ASC')],

  data: function () {
    return {
      filtro: {
        periodoEntregaPedidoInicio: this.adicionarDias(new Date(), -15),
        periodoEntregaPedidoFim: this.adicionarDias(new Date(), 15)
      },
      chapasEmExibicao: []
    };
  },

  methods: {
    /**
     * Busca a lista de posições de matéria prima.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Produtos.MateriaPrima.Posicao.obterLista(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Exibe o relatório com detalhamento das posições de matéria prima.
     * @param {Boolean} exportarExcel Um valor que define se o relatório será exportado para o excel.
     */
    abrirRelatorio: function (exportarExcel) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?Rel=PosicaoMateriaPrima&exportarExcel=' + exportarExcel);
    },

    /**
     * Indica se as chapas estão sendo exibidas na lista.
     * @param {!number} indice O número da linha que está sendo verificada.
     * @returns {boolean} Verdadeiro, se os itens estiverem sendo exibidos.
     */
    exibindoChapas: function (indice) {
      return this.chapasEmExibicao.indexOf(indice) > -1;
    },

    /**
     * Alterna a exibição das chapas de matéria prima
     * @param {!number} indice O indice onde se encontra o item para as quais as chapas serão exibidas.
     */
    alternarExibicaoChapas: function (indice) {
      var i = this.chapasEmExibicao.indexOf(indice);

      if (i > -1) {
        this.chapasEmExibicao.splice(i, 1);
      } else {
        this.chapasEmExibicao.push(indice);
      }
    },

    /**
     * Retorna o número de colunas da lista paginada.
     * @returns {number} o número de colunas existentes na lista paginada.
     */
    numeroColunasLista: function () {
      return this.$refs.lista.numeroColunas();
    },

    /**
     * Limpa a lista de chapas em exibição ao realizar paginação da lista principal.
     */
    atualizouItens: function () {
      this.chapasEmExibicao.splice(0, this.chapasEmExibicao.length);
    },

    /**
     * Força a atualização da lista posições de matéria prima, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  }
});
