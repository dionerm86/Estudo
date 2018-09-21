const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.FiltroQueryString],

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
     * Busca as liberações para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca de liberações.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de liberações.
     */
    atualizarLiberacoes: function(filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Liberacoes.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de liberações.
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
     * Exibe um relatório de liberação, de acordo com o tipo desejado.
     * @param {Object} item A liberação que será exibida.
     * @param {Boolean} relatorioCompleto Define se será gerado o relatório com todas as vias.
     * @param {Boolean} processoEnvioEmail Define se será usado o mesmo processo para geração de relatório que é usado ao enviar email para o cliente.
     */
    abrirRelatorio: function(item, relatorioCompleto, processoEnvioEmail) {
      this.abrirJanela(600, 800, '../Relatorios/RelLiberacao.aspx?idLiberarPedido=' + item.id + "&relatorioCompleto=" + relatorioCompleto + "&EnvioEmail=" + processoEnvioEmail);
    },

    /**
     * Exibe o relatório das notas promissórias de uma liberação.
     * @param {Object} item O pedido que será exibido.
     */
    abrirRelatorioNotasPromissorias: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=NotaPromissoria&idLiberarPedido=' + item.id);
    },

    /**
     * Exibe a tela para anexos da liberação.
     * @param {Object} item A liberação que se deseja anexar arquivos.
     */
    abrirAnexos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=liberacao');
    },

    /**
     * Exibe a tela para gerenciar anexos de várias liberação.
     */
    abrirAnexosVariasLiberacoes: function () {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=0&tipo=liberacao');
    },

    /**
     * Exibe a tela para cancelar a liberação.
     * @param {Object} item A liberação que se deseja cancelar.
     */
    abrirCancelamentoLiberacao: function (item) {
      this.abrirJanela(300, 500, '../Utils/SetMotivoCancLib.aspx?idLiberarPedido=' + item.id);
    },

    /**
     * Exibe um relatório com a listagem de liberações aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaLiberacoes: function (exportarExcel) {
      var url = '../Relatorios/RelBase.aspx?Rel=ListaLiberacao' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Exibe uma tela com totalizadores das liberações aplicando os filtros da tela.
     */
    abrirTotaisLiberacoes: function () {
      var url = '../Utils/ListaTotalLiberacao.aspx?t=1' + this.formatarFiltros_();
      this.abrirJanela(60, 160, url);
    },

    /**
     * Redireciona para a tela de geração de NF-e.
     * @param {Object} item A liberação a partir da qual será gerada uma NF-e.
     */
    obterLinkGerarNfe: function (item) {
      return '../Cadastros/CadNotaFiscalGerar.aspx?idLiberarPedido=' + item.id;
    },

    /**
     * Reenvia email para o cliente com a liberação.
     * @param {Object} item A liberação que será enviada por email.
     */
    reenviarEmail: function (item) {
      if (!this.perguntar('Reenviar e-mail', 'Deseja realmente reenviar o e-mail da liberação?')) {
        return;
      }

      var vm = this;

      Servicos.Liberacoes.reenviarEmail(item.id)
        .then(function (resposta) {
          vm.exibirMensagem('Reenvio de e-mail', 'O e-mail foi adicionado na fila para ser reenviado.');
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

      this.incluirFiltroComLista(filtro, 'idLiberarPedido', this.filtro.id);
      this.incluirFiltroComLista(filtro, 'idPedido', this.filtro.idPedido);
      this.incluirFiltroComLista(filtro, 'numeroNfe', this.filtro.numeroNfe);
      this.incluirFiltroComLista(filtro, 'situacao', this.filtro.situacao);
      this.incluirFiltroComLista(filtro, 'idCliente', this.filtro.idCliente);
      this.incluirFiltroComLista(filtro, 'nomeCliente', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtro, 'liberacaoNf', this.filtro.liberacaoComSemNotaFiscal);
      this.incluirFiltroComLista(filtro, 'dataIni', this.filtro.periodoCadastroInicio);
      this.incluirFiltroComLista(filtro, 'dataFim', this.filtro.periodoCadastroFim);
      this.incluirFiltroComLista(filtro, 'idFunc', this.filtro.idFuncionario);
      this.incluirFiltroComLista(filtro, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtro, 'dataIniCanc', this.filtro.periodoCancelamentoInicio);
      this.incluirFiltroComLista(filtro, 'dataFimCanc', this.filtro.periodoCancelamentoFim);

      return filtros.length
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

    Servicos.Liberacoes.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
