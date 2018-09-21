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
      var filtros = [
        this.incluirFiltro('idPedido', this.filtro.idPedido),
        this.incluirFiltro('idLiberarPedido', this.filtro.idLiberarPedido),
        this.incluirFiltro('idAcerto', this.filtro.idAcerto),
        this.incluirFiltro('idAcertoParcial', this.filtro.idAcertoParcial),
        this.incluirFiltro('idTrocaDev', this.filtro.idTrocaDevolucao),
        this.incluirFiltro('numeroNfe', this.filtro.numeroNfe),
        this.incluirFiltro('idSinal', this.filtro.idSinal),
        this.incluirFiltro('numCte', this.filtro.numeroCte),
        this.incluirFiltro('dtIniVenc', this.filtro.periodoVencimentoInicio),
        this.incluirFiltro('dtFimVenc', this.filtro.periodoVencimentoFim),
        this.incluirFiltro('dtIniRec', this.filtro.periodoRecebimentoInicio),
        this.incluirFiltro('dtFimRec', this.filtro.periodoRecebimentoFim),
        this.incluirFiltro('dataIniCad', this.filtro.periodoCadastroInicio),
        this.incluirFiltro('dataFimCad', this.filtro.periodoCadastroFim),
        this.incluirFiltro('idFuncRecebido', this.filtro.recebidaPor),
        this.incluirFiltro('idLoja', this.filtro.idLoja),
        this.incluirFiltro('idFunc', this.filtro.idVendedor),
        this.incluirFiltro('idCli', this.filtro.idCliente),
        this.incluirFiltro('nomeCli', this.filtro.nomeCliente),
        this.incluirFiltro('idVendedorAssociado', this.filtro.idVendedorAssociadoCliente),
        this.incluirFiltro('idVendedorObra', this.filtro.idVendedorObra),
        this.incluirFiltro('idsFormaPagto', this.filtro.formasPagamento),
        this.incluirFiltro('tipoEntrega', this.filtro.tipoEntrega),
        this.incluirFiltro('valorInicial', this.filtro.valorRecebidoInicio),
        this.incluirFiltro('valorFinal', this.filtro.valorRecebidoFim),
        this.incluirFiltro('idRota', this.filtro.idRota),
        this.incluirFiltro('tipoConta', this.filtro.tiposContabeis),
        this.incluirFiltro('idComissionado', this.filtro.idComissionado),
        this.incluirFiltro('obs', this.filtro.observacao),
        this.incluirFiltro('numAutCartao', this.filtro.numeroAutorizacaoCartao),
        this.incluirFiltro('numArqRemessa', this.filtro.numeroArquivoRemessa),
        this.incluirFiltro('contasCnab', this.filtro.buscaArquivoRemessa),
        this.incluirFiltro('idComissao', this.filtro.idComissao),
        this.incluirFiltro('tipoContasBuscar', this.filtro.buscaNotaFiscal),
        this.incluirFiltro('exibirAReceber', this.filtro.buscarContasAReceber),
        this.incluirFiltro('renegociadas', this.filtro.buscarContasRenegociadas),
        this.incluirFiltro('refObra', this.filtro.buscarContasDeObra),
        this.incluirFiltro('protestadas', this.filtro.buscarContasProtestadas),
        this.incluirFiltro('contasVinculadas', this.filtro.buscarContasVinculadas),
        this.incluirFiltro('ordenar', this.filtro.ordenacaoFiltro)
      ];

      filtros = filtros.filter(function (item) {
        return !!item;
      });

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
