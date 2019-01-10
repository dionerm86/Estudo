const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('IdRetalhoProducao', 'desc')],

  data: {
    filtro: {}
  },

  methods: {
    /**
     * Busca a lista de retalhos de produção.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Producao.Retalhos.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Redireciona para a tela de cadastro de retalhos de produção.
     */
    obterLinkInserirRetalhoProducao: function () {
      return 'CadRetalhoProducao.aspx';
    },

    /**
     * Abre uma nova janela com a tela de disponibilizar retalhos para produção.
     */
    abrirJanelaDisponibilizarRetalhosProducao: function () {
      this.abrirJanela(600, 800, "../../Utils/DisponibilizarRetalhos.aspx");
    },

    /**
     * Abre uma nova janela com os dados detalhados referentes a um retalho de produção.
     */
    abrirRelatorioIndividual: function (idRetalhoProducao) {
      this.abrirJanela(600, 800, "../../Relatorios/RelEtiquetas.aspx?idRetalhoProducao=" + idRetalhoProducao + '&ind=1');
    },

    /**
     * Abre uma nova janela com os dados detalhados referentes a um retalho de produção.
     */
    abrirDefinicaoMotivoPerda: function (idRetalhoProducao) {
      this.abrirJanela(180, 550, "../../Utils/SetMotivoPerdaRetalho.aspx?idRetalhoProducao=" + idRetalhoProducao);
    },

    /**
     * Abre uma nova janela com os dados detalhados referentes a um retalho de produção.
     */
    abrirDefinicaoMotivoCancelamento: function (idRetalhoProducao) {
      this.abrirJanela(180, 550, "../../Utils/SetMotivoCancRetalho.aspx?idRetalhoProducao=" + idRetalhoProducao);
    },
  
    /**
     * Exibe o relatório detalhado com a lista de retalhos de produção de acordo com os filtros informados.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirRelatorio: function (exportarExcel) {
      this.abrirJanela(600, 800, '../../Relatorios/RelBase.aspx?rel=RetalhosProducao' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'codInterno', this.filtro.codigoProduto);
      this.incluirFiltroComLista(filtros, 'descrProduto', this.filtro.descricaoProduto);
      this.incluirFiltroComLista(filtros, 'dataIni', this.filtro.periodoCadastroInicio);
      this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoCadastroFim);
      this.incluirFiltroComLista(filtros, 'dataUsoIni', this.filtro.periodoUsoInicio);
      this.incluirFiltroComLista(filtros, 'dataUsoFim', this.filtro.periodoUsoFim);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);
      this.incluirFiltroComLista(filtros, 'idsCores', this.filtro.idsCorVidro);
      this.incluirFiltroComLista(filtros, 'espessura', this.filtro.espessura || 0);
      this.incluirFiltroComLista(filtros, 'alturaInicio', this.filtro.alturaInicio || 0);
      this.incluirFiltroComLista(filtros, 'alturaFim', this.filtro.alturaFim || 0);
      this.incluirFiltroComLista(filtros, 'larguraInicio', this.filtro.larguraInicio || 0);
      this.incluirFiltroComLista(filtros, 'larguraFim', this.filtro.larguraFim || 0);
      this.incluirFiltroComLista(filtros, 'numEtiqueta', this.filtro.codigoEtiqueta);
      this.incluirFiltroComLista(filtros, 'observacao', this.filtro.observacao);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Força a atualização da lista de retalhos de produção, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  }
});
