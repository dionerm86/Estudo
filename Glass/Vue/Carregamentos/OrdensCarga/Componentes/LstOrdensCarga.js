const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('idOrdemCarga', 'desc')],

  data: {
    configuracoes: {},
    filtro: {}
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
      if (!this.configuracoes || !Object.keys(this.configuracoes).length) {
        return Promise.reject();
      }

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
     * Exclui uma ordem de carga
     */
    excluiMovimentacaoEstoqueReal: function (ordemCarga) {
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
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },
  },

  mounted: function () {
    var vm = this;

    //Recuperação das configurações ao carregar o Vue.
    Servicos.Carregamentos.OrdensCarga.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
