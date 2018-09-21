const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.Patch, Mixins.FiltroQueryString],

  data: {
    dadosOrdenacao_: {
      campo: 'id',
      direcao: 'desc'
    },
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
     * Realiza a ordenação da lista de contas recebidas.
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

      this.incluirFiltroComLista(filtro, 'idPedido', this.filtro.idPedido);
      this.incluirFiltroComLista(filtro, 'idLiberarPedido', this.filtro.idLiberarPedido);
      this.incluirFiltroComLista(filtro, 'idAcerto', this.filtro.idAcerto);
      this.incluirFiltroComLista(filtro, 'idAcertoParcial', this.filtro.idAcertoParcial);
      this.incluirFiltroComLista(filtro, 'idTrocaDev', this.filtro.idTrocaDevolucao);
      this.incluirFiltroComLista(filtro, 'numeroNfe', this.filtro.numeroNfe);
      this.incluirFiltroComLista(filtro, 'idSinal', this.filtro.idSinal);
      this.incluirFiltroComLista(filtro, 'numCte', this.filtro.numeroCte);
      this.incluirFiltroComLista(filtro, 'dtIniVenc', this.filtro.periodoVencimentoInicio);
      this.incluirFiltroComLista(filtro, 'dtFimVenc', this.filtro.periodoVencimentoFim);
      this.incluirFiltroComLista(filtro, 'dtIniRec', this.filtro.periodoRecebimentoInicio);
      this.incluirFiltroComLista(filtro, 'dtFimRec', this.filtro.periodoRecebimentoFim);
      this.incluirFiltroComLista(filtro, 'dataIniCad', this.filtro.periodoCadastroInicio);
      this.incluirFiltroComLista(filtro, 'dataFimCad', this.filtro.periodoCadastroFim);
      this.incluirFiltroComLista(filtro, 'idFuncRecebido', this.filtro.recebidaPor);
      this.incluirFiltroComLista(filtro, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtro, 'idFunc', this.filtro.idVendedor);
      this.incluirFiltroComLista(filtro, 'idCli', this.filtro.idCliente);
      this.incluirFiltroComLista(filtro, 'nomeCli', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtro, 'idVendedorAssociado', this.filtro.idVendedorAssociadoCliente);
      this.incluirFiltroComLista(filtro, 'idVendedorObra', this.filtro.idVendedorObra);
      this.incluirFiltroComLista(filtro, 'idsFormaPagto', this.filtro.formasPagamento);
      this.incluirFiltroComLista(filtro, 'tipoEntrega', this.filtro.tipoEntrega);
      this.incluirFiltroComLista(filtro, 'valorInicial', this.filtro.valorRecebidoInicio);
      this.incluirFiltroComLista(filtro, 'valorFinal', this.filtro.valorRecebidoFim);
      this.incluirFiltroComLista(filtro, 'idRota', this.filtro.idRota);
      this.incluirFiltroComLista(filtro, 'tipoConta', this.filtro.tiposContabeis);
      this.incluirFiltroComLista(filtro, 'idComissionado', this.filtro.idComissionado);
      this.incluirFiltroComLista(filtro, 'obs', this.filtro.observacao);
      this.incluirFiltroComLista(filtro, 'numAutCartao', this.filtro.numeroAutorizacaoCartao);
      this.incluirFiltroComLista(filtro, 'numArqRemessa', this.filtro.numeroArquivoRemessa);
      this.incluirFiltroComLista(filtro, 'contasCnab', this.filtro.buscaArquivoRemessa);
      this.incluirFiltroComLista(filtro, 'idComissao', this.filtro.idComissao);
      this.incluirFiltroComLista(filtro, 'tipoContasBuscar', this.filtro.buscaNotaFiscal);
      this.incluirFiltroComLista(filtro, 'exibirAReceber', this.filtro.buscarContasAReceber);
      this.incluirFiltroComLista(filtro, 'renegociadas', this.filtro.buscarContasRenegociadas);
      this.incluirFiltroComLista(filtro, 'refObra', this.filtro.buscarContasDeObra);
      this.incluirFiltroComLista(filtro, 'protestadas', this.filtro.buscarContasProtestadas);
      this.incluirFiltroComLista(filtro, 'contasVinculadas', this.filtro.buscarContasVinculadas);
      this.incluirFiltroComLista(filtro, 'ordenar', this.filtro.ordenacaoFiltro);

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

    Servicos.ContasReceber.Recebidas.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
