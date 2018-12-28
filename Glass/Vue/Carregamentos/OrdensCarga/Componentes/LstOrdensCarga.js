const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('idOrdemCarga', 'desc')],

  data: {
    configuracoes: {},
    filtro: {},
    pedidosEmExibicao: []
  },

  methods: {
    /**
     * Busca as ordens de carga para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de estoques de produto.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Carregamentos.OrdensCarga.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Exibe o relatório com detalhamento das ordens de carga para os filtros préviamente informados.
     * @param {Object} exportarExcel Define se o relatório é para ser exibido na tela ou exportado para o excel.
     */
    abrirRelatorio: function (exportarExcel) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=ListaOrdemCarga' + this.formatarFiltros_() + "&exportarExcel=" + exportarExcel);
    },

    /**
     * Retorna uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idOC', this.filtro.id);
      this.incluirFiltroComLista(filtros, 'idCli', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCli', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'tipo', this.filtro.tiposOrdemCarga);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacoesOrdemCarga);
      this.incluirFiltroComLista(filtros, 'idRota', this.filtro.idRota);
      this.incluirFiltroComLista(filtros, 'dtEntPedIni', this.filtro.periodoEntregaPedidoInicio);
      this.incluirFiltroComLista(filtros, 'dtEntPedFin', this.filtro.periodoEntregaPedidoFim);
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'idCarregamento', this.filtro.idCarregamento);
      this.incluirFiltroComLista(filtros, 'idPedido', this.filtro.idPedido);
      this.incluirFiltroComLista(filtros, 'codRotasExternas', this.filtro.idsRotasExternas);
      this.incluirFiltroComLista(filtros, 'idClienteExterno', this.filtro.idClienteExterno);
      this.incluirFiltroComLista(filtros, 'nomeClienteExterno', this.filtro.nomeClienteExterno);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Indica se os pedidos estão sendo exibidos na lista.
     * @param {!number} indice O número da linha que está sendo verificada.
     * @returns {boolean} Verdadeiro, se os itens estiverem sendo exibidos.
     */
    exibindoPedidos: function (indice) {
      return this.pedidosEmExibicao.indexOf(indice) > -1;
    },

    /**
     * Exclui uma ordem de carga
     */
    excluir: function (ordemCarga) {
      if (!this.perguntar("Deseja realmente excluir esta OC?")) {
        return;
      }

      var vm = this;

      Servicos.Carregamentos.OrdensCarga.excluir(ordemCarga.id)
        .then(function (resposta) {
          vm.exibirMensagem(resposta.data.mensagem);
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Exibe o relatório da ordem de carga.
     * @param {Object} item A ordem de carga que será exibida para impressão.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirRelatorioOrdemCarga: function (item) {
      var url = '../Relatorios/RelBase.aspx?rel=OrdemCarga&idOrdemCarga=' + item.id;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Obtem o link para o cadastro de ordens de carga.
     */
    obterLinkInserirOrdemCarga: function () {
      return '../Cadastros/CadOrdemCarga.aspx';
    },

    /**
     * Alterna a exibição dos pedidos.
     */
    alternarExibicaoPedidos: function (indice) {
      var i = this.pedidosEmExibicao.indexOf(indice);

      if (i > -1) {
        this.pedidosEmExibicao.splice(i, 1);
      } else {
        this.pedidosEmExibicao.push(indice);
      }
    },

    /**
     * Retorna o número de colunas da lista paginada.
     * @type {number}
     */
    numeroColunasLista: function () {
      return this.$refs.lista.numeroColunas();
    },

    /**
     * Abre um popup para incluir pedidos na ordem de carga.
     * @param {Object} item a ordem de carga que será usada para incluir pedidos.
     */
    abrirInclusaoPedido: function (item) {
      var vm = this;

      Servicos.Carregamentos.OrdensCarga.verificarPermissaoParaAssociarPedidosNaOrdemDeCarga(item.id)
        .then(function () {
          vm.abrirJanela(600, 800, '../Cadastros/CadItensCarregamento.aspx?popup=true&idCarregamento=' + item.id);
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem(erro.mensagem);
          }
        });
    },

    /**
     * Limpa a lista de pedidos em exibição ao realizar paginação da lista principal.
     */
    atualizouItens: function () {
      this.pedidosEmExibicao.splice(0, this.pedidosEmExibicao.length);
    },

    /**
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },
  },

  mounted: function () {
    var vm = this;

    Servicos.Carregamentos.OrdensCarga.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
