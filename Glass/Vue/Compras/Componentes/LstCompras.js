const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('idCompra', 'desc')],

  data: {
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca as compras para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Compras.obter(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtem o link para edição dos dados da compra.
     * @param {Object} item A compra que será editada.
     */
    obterLinkEditarCompra: function (item) {
      return '../Cadastros/CadCompra.aspx?idCompra=' + item.id;
    },

    /**
     * Obtem o link para edição dos dados da compra.
     */
    obterLinkNovaCompra: function () {
      return '../Cadastros/CadCompra.aspx';
    },

    /**
     * Exibe um os dados detalhados da compra em um relatório.
     * @param {Object} item A compra que será exibida.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirRelatorioCompra: function (item, exportarExcel) {
      var url = '../Relatorios/RelBase.aspx?rel=Compra&idCompra=' + item.id + '&exportarexcel=' + exportarExcel;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Exibe um os dados detalhados da compra em um relatório.
     * @param {Object} item A compra que será exibida.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirRelatorioCompras: function (exportarExcel) {
      var url = '../Relatorios/RelBase.aspx?rel=ListaCompras' + this.formatarFiltros_() + '&exportarexcel=' + exportarExcel;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Cancela uma compra.
     * @param {Object} item A compra que será cancelada.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    cancelar: function (item) {
      this.abrirJanela(200, 420, '../Utils/SetMotivoCancCompra.aspx?idCompra=' + item.id);
    },

    /**
     * Abre um popup com o gerenciamento de fotos para a compra.
     * @param {Object} item A compra que terá as fotos gerenciadas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    abrirGerenciamentoDeFotos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=compra');
    },

    /**
     * Exibe a tela .
     * @param {Object} item A compra que será finalizada.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    produtoChegou: function (item) {
      this.abrirJanela(600, 800, '../Utils/ProdutoCompraChegou.aspx?idCompra=' + item.id);
    },

    /**
     * Abre um popup com o gerenciamento de fotos para a compra.
     * @param {Object} item A compra que terá as fotos gerenciadas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    gerarNFe: function (item) {
      if (!confirm('Deseja gerar a nota fiscal para a compra de número ' + item.id + '?')) {
        return;
      }

      redirectUrl('../Cadastros/CadNotaFiscalGerarCompra.aspx?idCompra=' + item.id);
    },

    /**
     * Reabre uma compra já finalizada.
     * @param {Object} item A compra que será reaberta.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    reabrir: function (item) {
      if (!confirm('Deseja reabrir a compra de número ' + item.id + '?')) {
        return;
      }

      var vm = this;

      return Servicos.Compras.reabrir(item.id)
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
     * Finaliza uma compra.
     * @param {Object} item A compra que será finalizada.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    finalizar: function (item) {
      if (!confirm('Deseja finalizar a compra de número ' + item.id + '?')) {
        return;
      }

      var vm = this;

      return Servicos.Compras.finalizar(item.id)
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
     * Retorna uma string com os filtros selecionados na tela.
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'numCompra', this.filtro.id);
      this.incluirFiltroComLista(filtros, 'numPedido', this.idPedido);
      this.incluirFiltroComLista(filtros, 'nfPedido', this.notaFiscal);
      this.incluirFiltroComLista(filtros, 'idFornec', this.filtro.idFornecedor);
      this.incluirFiltroComLista(filtros, 'nomeFornec', this.filtro.nomeFornecedor);
      this.incluirFiltroComLista(filtros, 'obs', this.filtro.observacao);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);
      this.incluirFiltroComLista(filtros, 'emAtraso', (this.filtro.atrasada || 'false'));
      this.incluirFiltroComLista(filtros, 'dataIni', this.filtro.periodoCadastroInicio);
      this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoCadastroFim);
      this.incluirFiltroComLista(filtros, 'dataFabIni', this.filtro.periodoEntregaFabricaInicio);
      this.incluirFiltroComLista(filtros, 'dataFabFim', this.filtro.periodoEntregaFabricaFim);
      this.incluirFiltroComLista(filtros, 'dataSaidaIni', this.filtro.periodoSaidaInicio);
      this.incluirFiltroComLista(filtros, 'dataSaidaFim', this.filtro.periodoSaidaFim);
      this.incluirFiltroComLista(filtros, 'dataFinIni', this.filtro.periodoFinalizacaoInicio);
      this.incluirFiltroComLista(filtros, 'dataFinFim', this.filtro.periodoFinalizacaoFim);
      this.incluirFiltroComLista(filtros, 'dataEntIni', this.filtro.periodoEntradaInicio);
      this.incluirFiltroComLista(filtros, 'idsGrupoProd', this.filtro.idsGrupoProduto);
      this.incluirFiltroComLista(filtros, 'idSubgrupoProd', this.filtro.idSubgrupoProduto);
      this.incluirFiltroComLista(filtros, 'codProd', this.filtro.codigoProduto);
      this.incluirFiltroComLista(filtros, 'descrProd', this.filtro.descricaoProduto);
      this.incluirFiltroComLista(filtros, 'centroCustoDivergente', (this.filtro.centroDeCustoDivergente || 'false'));
      this.incluirFiltroComLista(filtros, 'agruparPorFornecedor', (this.filtro.agruparRelatorioPorFornecedor || 'false'));
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Atualiza a lista de compras.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },
  },

  mounted: function () {
    var vm = this;

    Servicos.Compras.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  },
});
