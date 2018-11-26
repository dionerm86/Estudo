const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('descricao', 'asc')],

  data: {
    configuracoes: {},
    filtro: {},
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
      return Servicos.TrocasDevolucoes.obterListaTrocaDevolucao(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém link para a tela de inserção da troca/devolucao.
     */
    obterLinkInserirTrocaDevolucao: function () {
      return '../Cadastros/CadTrocaDev.aspx';
    },

    /**
     * Obtém link para a tela de edição da troca/devolucao.
     * @param {Object} item A troca/devolucao que será editada.
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
     * Força a atualização da lista de parcelas, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    },

    /**
     * cancela uma troca/devolucao se possivel.
     * @param {Object} item A troca/devolucao que será cancelada.
     */
    cancelar: function (item) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja cancelar está troca/devolução?')) {
        return;
      }

      var vm = this;

      Servicos.TrocasDevolucoes.excluir(item.id)
        .then(function (resposta) {
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },


    /**
    * Exibe a tela de anexos da nota fiscal.
    * @param {Object} item A Nota que será usado para abertura da tela.
    */
    abrirAnexos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=trocaDevolucao');
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'numeroNfe', this.filtro.numeroNfe);
      this.incluirFiltroComLista(filtros, 'idPedido', this.filtro.idPedido);
      this.incluirFiltroComLista(filtros, 'modelo', this.filtro.modelo);
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'idCliente', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCliente', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'tipoFiscal', this.filtro.tipoFiscal);
      this.incluirFiltroComLista(filtros, 'idFornec', this.filtro.idFornecedor);
      this.incluirFiltroComLista(filtros, 'nomeFornec', this.filtro.nomeFornecedor);
      this.incluirFiltroComLista(filtros, 'codRota', this.filtro.codigoRota);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);
      this.incluirFiltroComLista(filtros, 'dataIni', this.filtro.periodoEmissaoInicio);
      this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoEmissaoFim);
      this.incluirFiltroComLista(filtros, 'idsCfop', this.filtro.idsCfop);
      this.incluirFiltroComLista(filtros, 'tiposCfop', this.filtro.tiposCfop);
      this.incluirFiltroComLista(filtros, 'dataEntSaiIni', this.filtro.periodoEntradaSaidaInicio);
      this.incluirFiltroComLista(filtros, 'dataEntSaiFim', this.filtro.periodoEntradaSaidaFim);
      this.incluirFiltroComLista(filtros, 'formaPagto', this.filtro.tipoVenda);
      this.incluirFiltroComLista(filtros, 'idsFormaPagtoNotaFiscal', this.filtro.idsFormaPagamento);
      this.incluirFiltroComLista(filtros, 'tipoNf', this.filtro.tipoDocumento);
      this.incluirFiltroComLista(filtros, 'finalidade', this.filtro.finalidade);
      this.incluirFiltroComLista(filtros, 'formaEmissao', this.filtro.tipoEmissao);
      this.incluirFiltroComLista(filtros, 'infCompl', this.filtro.informacaoComplementar);
      this.incluirFiltroComLista(filtros, 'codInternoProd', this.filtro.codigoInternoProduto);
      this.incluirFiltroComLista(filtros, 'descrProd', this.filtro.descricaoProduto);
      this.incluirFiltroComLista(filtros, 'lote', this.filtro.lote);
      this.incluirFiltroComLista(filtros, 'valorInicial', this.filtro.valorNotaFiscalInicio);
      this.incluirFiltroComLista(filtros, 'valorFinal', this.filtro.valorNotaFiscalFim);
      this.incluirFiltroComLista(filtros, 'ordenar', this.filtro.ordenacaoFiltro);
      this.incluirFiltroComLista(filtros, 'agrupar', this.filtro.agrupar);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Exibe um relatório com a listagem das parcelas.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaTrocaDevolucao: function (exportarExcel) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=Parcelas&exportarExcel=' + exportarExcel);
    },

    abrirListaControlePerdasExternas: function (exportarExcel) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=Parcelas&exportarExcel=' + exportarExcel);
    },
  },

  mounted: function () {
    var vm = this;

    Servicos.Estoques.obterConfiguracoesListaTrocaDevolucoes()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
