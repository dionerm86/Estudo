const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('codInterno', 'asc')],

  data: {
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca a lista de preço de tabela por cliente.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Produtos.PrecosTabelaCliente.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Exibe o relatório com detalhamento dos preços de tabela por cliente para os filtros previamente informados.
     * @param {Boolean} exportarExcel Um valor que define se o relatório será exportado para o excel.
     */
    abrirRelatorio: function (exportarExcel) {
      this.abrirJanela(600, 800, "../Relatorios/RelBase.aspx?rel=PrecoTabCliente" + this.formatarFiltros_() + "&exportarExcel=" + exportarExcel);
    },

    /**
     * Retorna uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idCli', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCli', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'idGrupo', this.filtro.idGrupoProduto);
      this.incluirFiltroComLista(filtros, 'idsSubgrupo', this.filtro.idsSubgrupoProduto);
      this.incluirFiltroComLista(filtros, 'codInterno', this.filtro.codigoProduto);
      this.incluirFiltroComLista(filtros, 'descrProd', this.filtro.descricaoProduto);
      this.incluirFiltroComLista(filtros, 'tipoValor', this.filtro.tipoValorTabela);
      this.incluirFiltroComLista(filtros, 'alturaInicio', this.filtro.valorAlturaInicio);
      this.incluirFiltroComLista(filtros, 'alturaFim', this.filtro.valorAlturaFim);
      this.incluirFiltroComLista(filtros, 'larguraInicio', this.filtro.valorLarguraInicio);
      this.incluirFiltroComLista(filtros, 'larguraFim', this.filtro.valorLarguraFim);
      this.incluirFiltroComLista(filtros, 'exibirPerc', this.filtro.exibirPercentualDescontoAcrescimo || 'false');
      this.incluirFiltroComLista(filtros, 'produtoDesconto', this.filtro.apenasComDesconto || 'false');
      this.incluirFiltroComLista(filtros, 'incluirBeneficiamento', this.filtro.incluirBeneficiamentosNoRelatorio || 'false');
      this.incluirFiltroComLista(filtros, 'exibirValorOriginal', this.filtro.exibirValorOriginal || 'false');
      this.incluirFiltroComLista(filtros, 'ordenacao', this.filtro.ordenacaoManual);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Força a atualização da lista de grupos de produto, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Produtos.PrecosTabelaCliente.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
