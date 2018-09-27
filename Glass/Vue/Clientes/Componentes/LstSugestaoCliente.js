const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.FiltroQueryString],

  data: {
    dadosOrdenacao_: {
      campo: 'id',
      direcao: 'asc'
    },
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca as Sugestões do Cliente para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.SugestaoCliente.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    voltar: function(){
      return "../../LstCliente.aspx";
    },

    /**
     * Obtém o título da página caso tenha sido aberta a partir de um pedido ou orçamento.
     */
    obterTitulo: function(){
      var idCliente = GetQueryString('idCliente');

      if (idCliente)
        return "Cliente: ";
    },

    /**
     * Realiza a ordenação da lista de sugestões de pedidos.
     * @param {string} campo O nome do campo pelo qual o resultado será ordenado.
     */
    ordenar: function (campo) {
      if (campo !== this.dadosOrdenacao_.campo) {
        this.dadosOrdenacao_.campo = campo;
        this.dadosOrdenacao_.direcao = '';
      } else {
        this.dadosOrdenacao_.direcao = this.dadosOrdenacao_.direcao === '' ? 'desc' : '';
      }
    },

    /**
     * Retorna o link para a tela de Inserção de sugestões.
     */
    obterLinkInserirSugestao: function () {
      var idCliente = GetQueryString('idCliente');
      var idPedido = GetQueryString('idPedido');
      var idOrcamento = GetQueryString('idOrcamento');

      if (idCliente)
        return "../Cadastros/CadSugestaoCliente.aspx?idCliente=" + idCliente;
      else if (idPedido)
        return "../Cadastros/CadSugestaoCliente.aspx?idPedido=" + idPedido;
      else if (idOrcamento)
        return "../Cadastros/CadSugestaoCliente.aspx?idOrcamento=" + idOrcamento;
      else
        return "../Cadastros/CadSugestaoCliente.aspx";
    },

    /**
     * Exibe a tela para anexos na Sugestão.
     * @param {Object} item O pedido que terá itens anexados.
     */
    abrirAnexos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=sugestao');
    },

    /**
     * Exibe o relatório da listagem de sugestões
     */
    abrirListaSugestoes: function (exportarExcel) {
      var url = '../Relatorios/RelBase.aspx?rel=ListaSugestaoCliente' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel;

      this.abrirJanela(600, 800, url);
    },

    /**
     * Recupera se a tela foi aberta a partir de um Cliente
     */
    verificarOrigemCliente: function () {
      var idCliente = GetQueryString('idCliente');

      if (idCliente)
        return false;

      return true;
    },

    /**
     * Exibe a tela para realizar o cancelamento de uma sugestão.
     * @param {Object} item sugestão que será cancelada.
     */
    cancelarSugestao: function (item) {
      var vm = this;

      Servicos.SugestaoCliente.cancelar(item.id)
        .then(function (resposta) {
          alert(resposta.data.mensagem);
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idSug', this.filtro.id);
      this.incluirFiltroComLista(filtros, 'idCli', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCli', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'desc', this.filtro.descricao);
      this.incluirFiltroComLista(filtros, 'idFunc', this.filtro.idFuncionario);
      this.incluirFiltroComLista(filtros, 'dataIni', this.filtro.periodoCadastroInicio);
      this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoCadastroFim);
      this.incluirFiltroComLista(filtros, 'tipo', this.filtro.tipo);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);
      this.incluirFiltroComLista(filtros, 'idRota', this.filtro.idRota);
      this.incluirFiltroComLista(filtros, 'idPedido', this.filtro.idPedido);
      this.incluirFiltroComLista(filtros, 'idOrcamento', this.filtro.idOrcamento);
      this.incluirFiltroComLista(filtros, 'idVendedorAssoc', this.filtro.idVendedorAssociado);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },
  },

  mounted: function() {
    var vm = this;

    Servicos.SugestaoCliente.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  },

  computed: {
    /**
     * Propriedade computada que indica a ordenação para a lista.
     * @type {string}
     */
    ordenacao: function () {
      var direcao = this.dadosOrdenacao_.direcao ? ' ' + this.dadosOrdenacao_.direcao : '';
      return this.dadosOrdenacao_.campo + direcao;
    }
  },

});
