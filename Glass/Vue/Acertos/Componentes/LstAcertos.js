const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('id', 'desc')],
  data: {
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
      this.$refs.lista.atualizar(true);
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
