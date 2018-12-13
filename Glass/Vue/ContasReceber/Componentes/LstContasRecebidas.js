const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    configuracoes: {},
    filtro: {},
    numeroLinhaEdicao: -1,
    contaRecebida: {},
    contaRecebidaAtual: {},
    contaRecebidaOriginal: {}
  },

  methods: {
    /**
     * Busca as contas recebidas para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.ContasReceber.Recebidas.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Indica que uma conta recebida será editada.
     * @param {Object} contaRecebida O item que será editado.
     * @param {number} numeroLinha O número da linha que contém o iten que será editado.
     */
    editar: function (contaRecebida, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(contaRecebida);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Atualiza os dados da conta recebida.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var contaRecebidaAtualizar = this.patch(this.contaRecebida, this.contaRecebidaOriginal);
      var vm = this;

      Servicos.ContasReceber.Recebidas.atualizar(this.contaRecebidaAtual.id, contaRecebidaAtualizar)
        .then(function (resposta) {
          vm.atualizarLista();
          vm.cancelar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Cancela a edição da conta recebida.
     */
    cancelar: function () {
      this.numeroLinhaEdicao = -1;
    },

    /**
     * Exibe um relatório de conta recebida, de acordo com o tipo desejado.
     * @param {Object} item A conta recebida que será cancelada.
     */
    abrirCancelamentoContaRecebida: function(item) {
      this.abrirJanela(200, 500, '../Utils/SetMotivoCancReceb.aspx?tipo=contaR&id=' + item.id);
    },

    /**
     * Exibe o relatório da liberação da conta recebida.
     * @param {Object} item A conta recebida que terá a liberação impressa.
     */
    abrirRelatorioLiberacao: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelLiberacao.aspx?idLiberarPedido=' + item.idLiberarPedido);
    },

    /**
     * Exibe o relatório do pedido da conta recebida.
     * @param {Object} item A conta recebida que terá o pedido referente impresso.
     */
    abrirRelatorioPedido: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelPedido.aspx?idPedido=' + item.idPedido);
    },

    /**
     * Exibe o relatório do acerto parcial da conta recebida.
     * @param {Object} item A conta recebida que terá o acerto parcial referente impresso.
     */
    abrirRelatorioAcertoParcial: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=Acerto&idAcerto=' + item.idAcertoParcial);
    },

    /**
     * Exibe o relatório do sinal da conta recebida.
     * @param {Object} item A conta recebida que terá o sinal referente impresso.
     */
    abrirRelatorioSinal: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=Sinal&idSinal=' + item.idSinal);
    },

    /**
     * Exibe o relatório do encontro de contas da conta recebida.
     * @param {Object} item A conta recebida que terá o encontro de contas referente impresso.
     */
    abrirRelatorioEncontroContas: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=EncontroContas&IdEncontroContas=' + item.idEncontroContas);
    },

    /**
     * Exibe o relatório da obra da conta recebida.
     * @param {Object} item A conta recebida que terá a obra referente impressa.
     */
    abrirRelatorioObra: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=Obra&obraDetalhada=false&idObra=' + item.idObra);
    },

    /**
     * Exibe o relatório do acerto da conta recebida.
     * @param {Object} item A conta recebida que terá o acerto referente impresso.
     */
    abrirRelatorioAcerto: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=Acerto&idAcerto=' + item.idAcerto);
    },

    /**
     * Exibe o relatório da conta recebida.
     * @param {Object} item A conta recebida que será impressa.
     */
    abrirRelatorioContaRecebida: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=ContaRecebida&idContaR=' + item.id);
    },

    /**
     * Exibe um relatório com a listagem de contas recebidas aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaContasRecebidas: function (exportarExcel) {
      var url = '../Relatorios/RelBase.aspx?Rel=ContasRecebidas' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Gerar um arquivo GCon.
     */
    gerarArquivoGCon: function () {
      var url = '../Handlers/ArquivoGCon.ashx?a=1' + this.formatarFiltros_();
      this.abrirJanela(300, 300, url);
    },

    /**
     * Gerar um arquivo Prosoft.
     */
    gerarArquivoProsoft: function () {
      var url = '../Handlers/ArquivoProsoft.ashx?a=1' + this.formatarFiltros_();
      this.abrirJanela(300, 300, url);
    },

    /**
     * Gerar um arquivo Domínio.
     */
    gerarArquivoDominio: function () {
      var url = '../Handlers/ArquivoDominio.ashx?a=1' + this.formatarFiltros_();
      this.abrirJanela(300, 300, url);
    },

    /**
     * Função executada para criação dos objetos necessários para edição de conta recebida.
     * @param {?Object} [contaRecebida=null] A conta recebida que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (contaRecebida) {
      this.contaRecebidaAtual = contaRecebida;

      this.contaRecebida = {
        observacao: contaRecebida ? contaRecebida.observacao : null
      };

      this.contaRecebidaOriginal = this.clonar(this.contaRecebida);
    },

    /**
     * Função que indica se o formulário possui valores válidos de acordo com os controles.
     * @param {Object} botao O botão que foi disparado no controle.
     * @returns {boolean} Um valor que indica se o formulário está válido.
     */
    validarFormulario_: function (botao) {
      var form = botao.form || botao;
      while (form.tagName.toLowerCase() !== 'form') {
        form = form.parentNode;
      }

      return form.checkValidity();
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idPedido', this.filtro.idPedido);
      this.incluirFiltroComLista(filtros, 'idLiberarPedido', this.filtro.idLiberarPedido);
      this.incluirFiltroComLista(filtros, 'idAcerto', this.filtro.idAcerto);
      this.incluirFiltroComLista(filtros, 'idAcertoParcial', this.filtro.idAcertoParcial);
      this.incluirFiltroComLista(filtros, 'idTrocaDev', this.filtro.idTrocaDevolucao);
      this.incluirFiltroComLista(filtros, 'numeroNfe', this.filtro.numeroNfe);
      this.incluirFiltroComLista(filtros, 'idSinal', this.filtro.idSinal);
      this.incluirFiltroComLista(filtros, 'numCte', this.filtro.numeroCte);
      this.incluirFiltroComLista(filtros, 'dtIniVenc', this.filtro.periodoVencimentoInicio);
      this.incluirFiltroComLista(filtros, 'dtFimVenc', this.filtro.periodoVencimentoFim);
      this.incluirFiltroComLista(filtros, 'dtIniRec', this.filtro.periodoRecebimentoInicio);
      this.incluirFiltroComLista(filtros, 'dtFimRec', this.filtro.periodoRecebimentoFim);
      this.incluirFiltroComLista(filtros, 'dataIniCad', this.filtro.periodoCadastroInicio);
      this.incluirFiltroComLista(filtros, 'dataFimCad', this.filtro.periodoCadastroFim);
      this.incluirFiltroComLista(filtros, 'idFuncRecebido', this.filtro.recebidaPor);
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'idFunc', this.filtro.idVendedor);
      this.incluirFiltroComLista(filtros, 'idCli', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCli', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'idVendedorAssociado', this.filtro.idVendedorAssociadoCliente);
      this.incluirFiltroComLista(filtros, 'idVendedorObra', this.filtro.idVendedorObra);
      this.incluirFiltroComLista(filtros, 'idsFormaPagto', this.filtro.formasPagamento);
      this.incluirFiltroComLista(filtros, 'tipoEntrega', this.filtro.tipoEntrega);
      this.incluirFiltroComLista(filtros, 'valorInicial', this.filtro.valorRecebidoInicio);
      this.incluirFiltroComLista(filtros, 'valorFinal', this.filtro.valorRecebidoFim);
      this.incluirFiltroComLista(filtros, 'idRota', this.filtro.idRota);
      this.incluirFiltroComLista(filtros, 'tipoConta', this.filtro.tiposContabeis);
      this.incluirFiltroComLista(filtros, 'idComissionado', this.filtro.idComissionado);
      this.incluirFiltroComLista(filtros, 'obs', this.filtro.observacao);
      this.incluirFiltroComLista(filtros, 'numAutCartao', this.filtro.numeroAutorizacaoCartao);
      this.incluirFiltroComLista(filtros, 'numArqRemessa', this.filtro.numeroArquivoRemessa);
      this.incluirFiltroComLista(filtros, 'contasCnab', this.filtro.buscaArquivoRemessa);
      this.incluirFiltroComLista(filtros, 'idComissao', this.filtro.idComissao);
      this.incluirFiltroComLista(filtros, 'tipoContasBuscar', this.filtro.buscaNotaFiscal);
      this.incluirFiltroComLista(filtros, 'exibirAReceber', this.filtro.buscarContasAReceber);
      this.incluirFiltroComLista(filtros, 'renegociadas', this.filtro.buscarContasRenegociadas);
      this.incluirFiltroComLista(filtros, 'refObra', this.filtro.buscarContasDeObra);
      this.incluirFiltroComLista(filtros, 'protestadas', this.filtro.buscarContasProtestadas);
      this.incluirFiltroComLista(filtros, 'contasVinculadas', this.filtro.buscarContasVinculadas);
      this.incluirFiltroComLista(filtros, 'ordenar', this.filtro.ordenacaoFiltro);

      return filtros.length
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

    Servicos.ContasReceber.Recebidas.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
