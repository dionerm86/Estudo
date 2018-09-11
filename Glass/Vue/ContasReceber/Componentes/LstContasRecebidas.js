const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.Patch],

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
      var filtros = []
      const incluirFiltro = function (campo, valor) {
        if (valor) {
          filtros.push(campo + '=' + valor);
        }
      }

      incluirFiltro('idPedido', this.filtro.idPedido);
      incluirFiltro('idLiberarPedido', this.filtro.idLiberarPedido);
      incluirFiltro('idAcerto', this.filtro.idAcerto);
      incluirFiltro('idAcertoParcial', this.filtro.idAcertoParcial);
      incluirFiltro('idTrocaDev', this.filtro.idTrocaDevolucao);
      incluirFiltro('numeroNfe', this.filtro.numeroNfe);
      incluirFiltro('idSinal', this.filtro.idSinal);
      incluirFiltro('numCte', this.filtro.numeroCte);
      incluirFiltro('dtIniVenc', this.filtro.periodoVencimentoInicio ? this.filtro.periodoVencimentoInicio.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dtFimVenc', this.filtro.periodoVencimentoFim ? this.filtro.periodoVencimentoFim.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dtIniRec', this.filtro.periodoRecebimentoInicio ? this.filtro.periodoRecebimentoInicio.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dtFimRec', this.filtro.periodoRecebimentoFim ? this.filtro.periodoRecebimentoFim.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dataIniCad', this.filtro.periodoCadastroInicio ? this.filtro.periodoCadastroInicio.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dataFimCad', this.filtro.periodoCadastroFim ? this.filtro.periodoCadastroFim.toLocaleDateString('pt-BR') : null);
      incluirFiltro('idFuncRecebido', this.filtro.recebidaPor);
      incluirFiltro('idLoja', this.filtro.idLoja);
      incluirFiltro('idFunc', this.filtro.idVendedor);
      incluirFiltro('idCli', this.filtro.idCliente);
      incluirFiltro('nomeCli', this.filtro.nomeCliente);
      incluirFiltro('idVendedorAssociado', this.filtro.idVendedorAssociadoCliente);
      incluirFiltro('idVendedorObra', this.filtro.idVendedorObra);
      incluirFiltro('idsFormaPagto', this.filtro.formasPagamento);
      incluirFiltro('tipoEntrega', this.filtro.tipoEntrega);
      incluirFiltro('valorInicial', this.filtro.valorRecebidoInicio);
      incluirFiltro('valorFinal', this.filtro.valorRecebidoFim);
      incluirFiltro('idRota', this.filtro.idRota);
      incluirFiltro('tipoConta', this.filtro.tiposContabeis);
      incluirFiltro('idComissionado', this.filtro.idComissionado);
      incluirFiltro('obs', this.filtro.observacao);
      incluirFiltro('numAutCartao', this.filtro.numeroAutorizacaoCartao);
      incluirFiltro('numArqRemessa', this.filtro.numeroArquivoRemessa);
      incluirFiltro('contasCnab', this.filtro.buscaArquivoRemessa);
      incluirFiltro('idComissao', this.filtro.idComissao);
      incluirFiltro('tipoContasBuscar', this.filtro.buscaNotaFiscal);
      incluirFiltro('exibirAReceber', this.filtro.buscarContasAReceber);
      incluirFiltro('renegociadas', this.filtro.buscarContasRenegociadas);
      incluirFiltro('refObra', this.filtro.buscarContasDeObra);
      incluirFiltro('protestadas', this.filtro.buscarContasProtestadas);
      incluirFiltro('contasVinculadas', this.filtro.buscarContasVinculadas);
      incluirFiltro('ordenar', this.filtro.ordenacaoFiltro);

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

    Servicos.ContasReceber.Recebidas.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
