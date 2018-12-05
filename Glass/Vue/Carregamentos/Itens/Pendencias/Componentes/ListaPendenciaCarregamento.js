const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('idCarregamento', 'desc')],

  data: {
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca os carregamentos pendentes para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obter: function (filtro, pagina, numeroRegistros, ordenacao) {
      if (!this.configuracoes || !Object.keys(this.configuracoes).length) {
        return Promise.reject();
      }

      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Carregamentos.Itens.Pendencias.obter(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna as rotas associadas ao carregamento.
     * @param {Object} carregamento O carregamento que terá as rotas listadas.
     * @returns {string} As rotas associadas ao carregamento informado.
     */
    obterRotas: function (carregamento) {
      if (!carregamento || !carregamento.rotas) {
        return '';
      }

      return carregamento.rotas.slice()
        .join(', ');
    },

    /**
     * Exibe um relatório do carregamento, aplicando os filtros da tela.
     * @param {Object} item O carregamento que será exibido para impressão.
     */
    abrirRelatorioCarregamentoPendente: function (item) {
      var url = '../Relatorios/RelBase.aspx?rel=PendenciaCarregamento&idCarregamento=' + item.id + '&idCliente=' + item.cliente.id +
        '&idClienteExterno=' + item.clienteExterno.id + '&nomeClienteExterno=' + item.clienteExterno.nome;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Exibe um relatório com a listagem de carregamentos, aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaCarregamentosPendentes: function (exportarExcel) {
      var url = '../Relatorios/RelBase.aspx?rel=ListaPendenciaCarregamento' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela.
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idCarregamento', this.filtro.idCarregamento);
      this.incluirFiltroComLista(filtros, 'idCliente', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCliente', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'dataSaidaIni', this.filtro.periodoPrevisaoSaidaInicio);
      this.incluirFiltroComLista(filtros, 'dataSaidaFim', this.filtro.periodoPrevisaoSaidaFim);
      this.incluirFiltroComLista(filtros, 'rotas', this.filtro.idsRota);
      this.incluirFiltroComLista(filtros, 'ignorarPedidoVendaTransferencia', this.filtro.ignorarPedidosVendaTransferencia);
      this.incluirFiltroComLista(filtros, 'idClienteExterno', this.filtro.idClienteExterno);
      this.incluirFiltroComLista(filtros, 'nomeClienteExterno', this.filtro.nomeClienteExterno);
      this.incluirFiltroComLista(filtros, 'codRotasExternas', this.filtro.idsRotaExterna);
      this.incluirFiltroComLista(filtros, 'ordenacao', this.filtro.ordenacao);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Atualiza a lista de carregamentos.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    },
  },

  mounted: function () {
    var vm = this;

    Servicos.Carregamentos.Itens.Pendencias.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  },
});
