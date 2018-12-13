const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    configuracoes: {},
    filtro: {},
    agruparPorFuncionario: false,
    agruparPorFuncionarioAssociado: false
  },

  methods: {
    /**
     * Busca a lista de rotas para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Estoques.TrocasDevolucoes.obterListaTrocaDevolucao(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém link para a tela de inserção da troca/devolução.
     */
    obterLinkInserirTrocaDevolucao: function () {
      const popup = GetQueryString('popup') ? '?popup=1' : '';
      return '../Cadastros/CadTrocaDev.aspx' + popup;
    },

    /**
     * Obtém link para a tela de edição da troca/devolução.
     * @param {Object} item A troca/devolução que será editada.
     */
    obterLinkEditarTrocaDevoluca: function (item) {
      return '../Cadastros/CadTrocaDev.aspx?idTrocaDev=' + item.id;
    },

    /**
     * Exibe a tela de anexos da liberação de um pedido.
     * @param {Object} item O pedido que será usado para abertura da tela.
     */
    abrirAnexosTrocaDevolucao: function (item) {
      if (item && item.id) {
        this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=trocaDevolucao');
      }
    },

    /**
     * Exibe o relatório da liberação da conta recebida.
     * @param {Object} item A conta recebida que terá a liberação impressa.
     */
    abrirRelatorioTrocaDevolucao: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=TrocaDevolucao&idTrocaDevolucao=' + item.id);
    },

    /**
     * Força a atualização da lista de trocas/devoluções, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },

    /**
     * Cancela uma troca/devolução se possivel.
     * @param {Object} item A troca/devolução que será cancelada.
     */
    cancelar: function (item) {
      this.abrirJanela(150, 420, '../Utils/SetMotivoCancTroca.aspx?idTrocaDev=' + item.id);
    },


    /**
    * Exibe a tela de anexos da nota fiscal.
    * @param {Object} item A Nota que será usado para abertura da tela.
    */
    abrirAnexos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=trocadevolucao');
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     * @param {String} [nomeAdicionalAgrupar=''] Um texto a ser adicionado no nome do parâmetro de agrupamento.
     * @returns {String} Uma string com os filtros convertidos para uso na querystring.
     */
    formatarFiltros_: function (nomeAdicionalAgrupar) {
      var filtros = [];

      if (!nomeAdicionalAgrupar) {
        nomeAdicionalAgrupar = '';
      }

      this.incluirFiltroComLista(filtros, 'idTrocaDevolucao', this.filtro.id);
      this.incluirFiltroComLista(filtros, 'idPedido', this.filtro.idPedido);
      this.incluirFiltroComLista(filtros, 'tipo', this.filtro.tipo);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);
      this.incluirFiltroComLista(filtros, 'idCli', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCli', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'dataIni', this.filtro.periodoTrocaInicio);
      this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoTrocaFim);
      this.incluirFiltroComLista(filtros, 'idsFunc', this.filtro.idsFuncionario);
      this.incluirFiltroComLista(filtros, 'idsFuncionarioAssociadoCliente', this.filtro.idsFuncionarioAssociadoCliente);
      this.incluirFiltroComLista(filtros, 'idProduto', this.filtro.idProduto);
      this.incluirFiltroComLista(filtros, 'alturaMin', this.filtro.alturaMinima);
      this.incluirFiltroComLista(filtros, 'alturaMax', this.filtro.alturaMaxima);
      this.incluirFiltroComLista(filtros, 'larguraMin', this.filtro.larguraMinima);
      this.incluirFiltroComLista(filtros, 'larguraMax', this.filtro.larguraMaxima);
      this.incluirFiltroComLista(filtros, 'idOrigemTrocaDevolucao', this.filtro.idOrigemTrocaDevolucao);
      this.incluirFiltroComLista(filtros, 'idTipoPerda', this.filtro.idTipoPerda);
      this.incluirFiltroComLista(filtros, 'idSetor', this.filtro.idSetor);
      this.incluirFiltroComLista(filtros, 'tipoPedido', this.filtro.tipoPedido);
      this.incluirFiltroComLista(filtros, 'idGrupo', this.filtro.idGrupoProduto);
      this.incluirFiltroComLista(filtros, 'idSubgrupo', this.filtro.idSubgrupoProduto);
      this.incluirFiltroComLista(filtros, 'agrupar' + nomeAdicionalAgrupar, this.agruparPorFuncionario.toString());
      this.incluirFiltroComLista(filtros, 'agruparFuncionarioAssociadoCliente', this.agruparPorFuncionarioAssociado.toString());

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Exibe um relatório com a listagem das parcelas.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaTrocaDevolucao: function (exportarExcel) {
      const filtros = this.formatarFiltros_();
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=ListaTrocaDevolucao&exportarExcel=' + exportarExcel + filtros);
    },

    abrirListaControlePerdasExternas: function () {
      const filtros = this.formatarFiltros_('Func');
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=ControlePerdasExternas' + filtros);
    },
  },

  mounted: function () {
    var vm = this;

    Servicos.Estoques.TrocasDevolucoes.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
