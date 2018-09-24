const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.Patch],

  data: {
    dadosOrdenacao_: {
      campo: 'id',
      direcao: 'desc'
    },
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca os acertos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de acertos.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Acertos.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de acertos.
     * @param {string} campo O nome do campo pelo qual o resultado será ordenado.
     */
    ordenar: function(campo) {
      if (campo !== this.dadosOrdenacao_.campo) {
        this.dadosOrdenacao_.campo = campo;
        this.dadosOrdenacao_.direcao = '';
      } else {
        this.dadosOrdenacao_.direcao = this.dadosOrdenacao_.direcao === '' ? 'desc' : '';
      }
    },

    /**
     * Abre uma tela de cancelamento de acerto.
     * @param {Object} item O acerto que será cancelado.
     */
    abrirCancelamento: function (item) {
      this.abrirJanela(200, 500, '../Utils/SetMotivoCancReceb.aspx?tipo=acerto&id=' + item.id);
    },

    /**
     * Abre uma tela com a impressão do acerto.
     * @param {Object} item O acerto que será impresso.
     */
    abrirImpressaoAcerto: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=Acerto&idAcerto=' + item.id);
    },

    /**
     * Abre uma tela com as notas promissórias do acerto.
     * @param {Object} item O acerto que terá as notas promissórias exibidas.
     */
    abrirImpressaoNotasPromissorias: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=NotaPromissoria&idAcerto=' + item.id);
    },

    /**
     * Abre uma tela com os anexos do acerto.
     * @param {Object} item O acerto que será usado para visualizar os anexos.
     */
    abrirAnexos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?tipo=acerto&id=' + item.id);
    },

    /**
     * Exibe um relatório com a listagem de acertos aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaAcertos: function (exportarExcel) {
      var url = '../Relatorios/RelBase.aspx?rel=ListaAcerto' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel;

      this.abrirJanela(600, 800, url);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = []
      const incluirFiltro = function (campo, valor) {
        if (valor) {
          filtros.push(campo + '=' + valor);
        }
      }

      incluirFiltro('idAcerto', this.filtro.id);
      incluirFiltro('idPedido', this.filtro.idPedido);
      incluirFiltro('idLiberarPedido', this.filtro.idLiberacao);
      incluirFiltro('idCliente', this.filtro.idCliente);
      incluirFiltro('dataIni', this.filtro.periodoCadastroInicio ? this.filtro.periodoCadastroInicio.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dataFim', this.filtro.periodoCadastroFim ? this.filtro.periodoCadastroFim.toLocaleDateString('pt-BR') : null);
      incluirFiltro('idFormaPagto', this.filtro.idFormaPagamento);
      incluirFiltro('numNotaFiscal', this.filtro.numeroNfe);
      incluirFiltro('protesto', this.filtro.protesto);

      return filtros.length > 0
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  computed: {
    /**
     * Propriedade computada que indica a ordenação para a lista.
     * @type {string}
     */
    ordenacao: function() {
      var direcao = this.dadosOrdenacao_.direcao ? ' ' + this.dadosOrdenacao_.direcao : '';
      return this.dadosOrdenacao_.campo + direcao;
    }
  },

  mounted: function() {
    var vm = this;

    Servicos.Acertos.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
