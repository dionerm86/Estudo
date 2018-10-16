const app = new Vue({
  el: '#app',
  mixins: [Mixins.FiltroQueryString],
  data: {
    filtro: {},
    configuracoes: {},
    agruparImpressao: null,
    contagem: null,
    exibirContagem: false,
    dadosProducao: null
  },

  methods: {
    /**
     * Força a atualização das peças da tela.
     */
    atualizarPecas: function () {
      this.$refs.pecas.atualizarLista();
    },

    /**
     * Recupera a lista de peças para a tela de consulta de produção.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca de peças.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    buscarPecas: function (filtro, pagina, numeroRegistros, ordenacao) {
      if (!this.configuracoes || !Object.keys(this.configuracoes).length) {
        return Promise.reject();
      }

      return Servicos.Producao.obterLista(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Busca os dados de contagem de peças com base no filtro atual.
     */
    realizarContagemPecas: function () {
      this.contagem = null;

      if (!this.filtro || !Object.keys(this.filtro).length) {
        return;
      }

      var vm = this;

      Servicos.Producao.obterContagemPecas(this.filtro)
        .then(function (resposta) {
          vm.contagem = resposta.data;
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Contagem de peças', erro.mensagem);
          }
        });
    },

    /**
     * Busca os dados de produção do pedido filtrado.
     */
    atualizarDadosProducao: function () {
      this.dadosProducao = null;

      if (!this.filtro || !Object.keys(this.filtro).length || !this.filtro.idPedido) {
        return;
      }

      var vm = this;

      Servicos.PedidosConferencia.obterDadosProducao(this.filtro.idPedido)
        .then(function (resposta) {
          vm.dadosProducao = resposta.data;
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Dados de produção do pedido ' + this.filtro.idPedido, erro.mensagem);
          }
        });
    },

    /**
     * Define a exibição da contagem de peças apenas se houver peças em exibição.
     */
    atualizouItens: function (numeroItens) {
      this.exibirContagem = numeroItens > 0;
    },

    /**
     * Abre o relatório geral da tela.
     * @param {boolean} exportarExcel Indica se o relatório será exportado para o Excel ou se será aberto como PDF.
     */
    abrirRelatorioGeral: function(exportarExcel) {
      const url = '../../Relatorios/RelBase.aspx?rel=Producao'
        + (this.agruparImpressao == '3' ? 'Contagem' : '');

      this.abrirJanela(600, 800, url + this.formatarFiltros_(exportarExcel));
    },

    /**
     * Abre o relatório por setor da tela.
     * @param {boolean} exportarExcel Indica se o relatório será exportado para o Excel ou se será aberto como PDF.
     */
    abrirRelatorioSetor: function (exportarExcel) {
      const url = '../../Relatorios/RelBase.aspx?rel=Producao'
        + (this.agruparImpressao == '3' ? 'Contagem' : '');

      this.abrirJanela(600, 800, url + this.formatarFiltros_(exportarExcel) + '&setorFiltrado=true');
    },

    /**
     * Abre o relatório por roteiro da tela.
     * @param {boolean} exportarExcel Indica se o relatório será exportado para o Excel ou se será aberto como PDF.
     */
    abrirRelatorioRoteiro: function (exportarExcel) {
      const url = '../../Relatorios/RelBase.aspx?rel=ProducaoPassou';
      this.abrirJanela(600, 800, url + this.formatarFiltros_(exportarExcel) + '&setorFiltrado=true');
    },

    /**
     * Abre o relatório de pedidos da tela.
     * @param {boolean} exportarExcel Indica se o relatório será exportado para o Excel ou se será aberto como PDF.
     */
    abrirRelatorioPedidos: function (exportarExcel) {
      const url = '../../Relatorios/RelBase.aspx?rel=ProducaoPedidos';
      this.abrirJanela(600, 800, url + this.formatarFiltros_(exportarExcel) + '&setorFiltrado=true');
    },

    /**
     * Formata os filtros da tela para exibição dos relatórios.
     * @param {boolean} exportarExcel Indica se o relatório será exportado para o Excel ou se será aberto como PDF.
     */
    formatarFiltros_: function (exportarExcel) {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idPedido', this.filtro.idPedido);
      this.incluirFiltroComLista(filtros, 'idLiberarPedido', this.filtro.idLiberacaoPedido);
      this.incluirFiltroComLista(filtros, 'idCarregamento', this.filtro.idCarregamento);
      this.incluirFiltroComLista(filtros, 'idPedidoImportado', this.filtro.idPedidoImportado);
      this.incluirFiltroComLista(filtros, 'codCliente', this.filtro.codigoPedidoCliente);
      this.incluirFiltroComLista(filtros, 'codRota', this.filtro.idsRotas);
      this.incluirFiltroComLista(filtros, 'idCliente', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCliente', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'idImpressao', this.filtro.idImpressao);
      this.incluirFiltroComLista(filtros, 'numEtiqueta', this.filtro.numeroEtiquetaPeca);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacoesProducao);
      this.incluirFiltroComLista(filtros, 'idSetor', this.filtro.idSetor);
      this.incluirFiltroComLista(filtros, 'dataIni', this.filtro.periodoSetorInicio);
      this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoSetorFim);
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'tiposSituacoes', this.filtro.tipoSituacaoProducao);
      this.incluirFiltroComLista(filtros, 'situacaoPedido', this.filtro.situacaoPedido);
      this.incluirFiltroComLista(filtros, 'idsSubgrupos', this.filtro.idsSubgrupos);
      this.incluirFiltroComLista(filtros, 'idsBenef', this.filtro.idsBeneficiamentos);
      this.incluirFiltroComLista(filtros, 'idFunc', this.filtro.idVendedorPedido);
      this.incluirFiltroComLista(filtros, 'tipoEntrega', this.filtro.tipoEntregaPedido);
      this.incluirFiltroComLista(filtros, 'dataIniEnt', this.filtro.periodoEntregaInicio);
      this.incluirFiltroComLista(filtros, 'dataFimEnt', this.filtro.periodoEntregaFim);
      this.incluirFiltroComLista(filtros, 'largura', this.filtro.larguraPeca);
      this.incluirFiltroComLista(filtros, 'altura', this.filtro.alturaPeca);
      this.incluirFiltroComLista(filtros, 'tipoPedido', this.filtro.tiposPedidos);
      this.incluirFiltroComLista(filtros, 'pecasProdCanc', this.filtro.tiposPecasExibir);
      this.incluirFiltroComLista(filtros, 'dataIniFabr', this.filtro.periodoFabricaInicio);
      this.incluirFiltroComLista(filtros, 'dataFimFabr', this.filtro.periodoFabricaFim);
      this.incluirFiltroComLista(filtros, 'espessura', this.filtro.espessuraPeca);
      this.incluirFiltroComLista(filtros, 'idCorVidro', this.filtro.idCorVidro);
      this.incluirFiltroComLista(filtros, 'idsProc', this.filtro.idsProcessos);
      this.incluirFiltroComLista(filtros, 'idsApl', this.filtro.idsAplicacoes);
      this.incluirFiltroComLista(filtros, 'planoCorte', this.filtro.planoCorte);
      this.incluirFiltroComLista(filtros, 'numEtiquetaChapa', this.filtro.numeroEtiquetaChapa);
      this.incluirFiltroComLista(filtros, 'produtoComposicao', this.filtro.tipoProdutosComposicao);
      this.incluirFiltroComLista(filtros, 'aguardExpedicao', this.filtro.apenasPecasAguardandoExpedicao);
      this.incluirFiltroComLista(filtros, 'aguardEntrEstoque', this.filtro.apenasPecasAguardandoEntradaEstoque);
      this.incluirFiltroComLista(filtros, 'pecaParadaProducao', this.filtro.apenasPecasParadasNaProducao);
      this.incluirFiltroComLista(filtros, 'pecasRepostas', this.filtro.apenasPecasRepostas);
      this.incluirFiltroComLista(filtros, 'dataIniConfPed', this.filtro.periodoConferenciaPedidoInicio);
      this.incluirFiltroComLista(filtros, 'dataFimConfPed', this.filtro.periodoConferenciaPedidoFim);
      this.incluirFiltroComLista(filtros, 'fastDelivery', this.filtro.tipoFastDelivery);
      this.incluirFiltroComLista(filtros, 'agrupar', this.agruparImpressao);
      this.incluirFiltroComLista(filtros, 'exportarExcel', exportarExcel);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Producao.obterConfiguracoesConsulta()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  },

  computed: {
    /**
     * Propriedade computada que indica se os controles de impressão de
     * setor deverão ser exibidos.
     * @type {boolean}
     */
    exibirImpressoesSetor: function () {
      return this.filtro
        && this.filtro.idSetor
        && this.configuracoes
        && this.configuracoes.setoresExibicao
        && this.configuracoes.setoresExibicao.length
        && this.configuracoes.setoresExibicao.some(function (item) {
          return item && item.id === this.filtro.idSetor;
        }, this);
    },

    /**
     * Propriedade computada que indica se os controles de impressão de
     * roteiros deverão ser exibidos.
     * @type {boolean}
     */
    exibirImpressoesRoteiro: function () {
      return this.configuracoes
        && this.configuracoes.setoresExibicao
        && this.configuracoes.setoresExibicao.length
        && this.configuracoes.setoresExibicao.some(function (item) {
          return item && item.pertencenteARoteiro;
        });
    }
  },

  watch: {
    /**
     * Observador para a variável 'filtro'.
     * Realiza a contagem de peças ao alterar o filtro atual.
     */
    filtro: {
      handler: function () {
        this.realizarContagemPecas();
        this.atualizarDadosProducao();
      },
      deep: true
    }
  }
});
