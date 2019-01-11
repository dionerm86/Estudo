const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('dataVenc', 'asc')],

  data: {
    configuracoes: {},
    numeroLinhaEdicao: -1,
    contaPaga: {},
    contaPagaAtual: {
      fornecedor: {},
      transportador: {},
      valoresPagamento: {},
      parcela: {},
      datas: {},
      observacoes: {},
      permissoes: {}
    },
    filtro: {},
    tipoPlanoConta: 2,
    planoContaAtual: {}
  },

  methods: {
    /**
     * Busca a lista de contas a pagas para a tela.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.ContasPagar.Pagas.obterLista(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Exibe um relatório com dados detalhados referentes as contas pagas.
     * @param {boolean} exportarExcel Define se o relatório será gerado na tela ou exportado para o excel.
     */
    abrirRelatorio: function (exportarExcel, exportarGCON, exportarProsoft, exportarDominio) {
      if (exportarGCON) {
        window.open('../Handlers/ArquivoGCon.ashx?idContaPg=' + (this.filtro && this.filtro.id ? this.filtro.id : '') + this.formatarFiltros_(exportarGCON, exportarProsoft, exportarDominio) + '&exportarExcel=' + exportarExcel);
      }
      else if (exportarProsoft) {
        window.open('../Handlers/ArquivoProsoft.ashx?idContaPg=' + (this.filtro && this.filtro.id ? this.filtro.id : '') + this.formatarFiltros_(exportarGCON, exportarProsoft, exportarDominio) + '&exportarExcel=' + exportarExcel);
      }
      else if (exportarDominio) {
        window.open('../Handlers/ArquivoDominio.ashx?idContaPg=' + (this.filtro && this.filtro.id ? this.filtro.id : '') + this.formatarFiltros_(exportarGCON, exportarProsoft, exportarDominio) + '&exportarExcel=' + exportarExcel);
      }
      else {
        this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?Rel=ContasPagas' + this.formatarFiltros_(exportarGCON, exportarProsoft, exportarDominio) + '&exportarExcel=' + exportarExcel);
      }
    },

    /**
     * Retorna uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function (exportarGCON, exportarProsoft, exportarDominio) {
      var filtros = [];

      nomeFiltroNotaFiscal = !exportarGCON && !exportarProsoft ? 'nf' : 'numeroNFe';
      nomeFiltroPeriodoPagamentoInicio = exportarGCON || exportarProsoft ? 'dtIniRec' : 'dtIniPago';
      nomeFiltroPeriodoPagamentoFim = exportarGCON || exportarProsoft ? 'dtFimRec' : 'dtFimPago';
      nomeFiltroIdsFormaPagamento = exportarGCON || exportarProsoft ? 'idFormaPagto' : 'formaPagto';
      nomeFiltroPlanoConta = exportarGCON || exportarProsoft ? 'idConta' : 'planoConta';
      nomeFiltroIdImpostoServico = exportarGCON || exportarProsoft ? 'idImpServ' : 'idImpostoServ';

      this.incluirFiltroComLista(filtros, 'idContaPg', this.filtro.id || 0);
      this.incluirFiltroComLista(filtros, 'idCompra', this.filtro.idCompra || 0);
      this.incluirFiltroComLista(filtros, 'idComissao', this.filtro.idComissao || 0);
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja || 0);
      this.incluirFiltroComLista(filtros, 'idCustoFixo', this.filtro.idCustoFixo || 0);
      this.incluirFiltroComLista(filtros, nomeFiltroIdImpostoServico, this.filtro.idImpostoServico || 0);
      this.incluirFiltroComLista(filtros, 'idFornec', this.filtro.idFornecedor || 0);
      this.incluirFiltroComLista(filtros, 'nomeFornec', this.filtro.nomeFornecedor || '');
      this.incluirFiltroComLista(filtros, nomeFiltroNotaFiscal, this.filtro.numeroNotaFiscal);
      this.incluirFiltroComLista(filtros, 'numCte', this.filtro.numeroCte || 0);
      this.incluirFiltroComLista(filtros, 'formaPagto', this.filtro.idsFormaPagamento || '');
      this.incluirFiltroComLista(filtros, 'dataIniCad', this.filtro.periodoCadastroInicio || '');
      this.incluirFiltroComLista(filtros, 'dataFimCad', this.filtro.periodoCadastroFim || '');
      this.incluirFiltroComLista(filtros, nomeFiltroPeriodoPagamentoInicio, this.filtro.periodoPagamentoInicio || '');
      this.incluirFiltroComLista(filtros, nomeFiltroPeriodoPagamentoFim, this.filtro.periodoPagamentoFim || '');
      this.incluirFiltroComLista(filtros, 'dtIniVenc', this.filtro.periodoVencimentoInicio || '');
      this.incluirFiltroComLista(filtros, 'dtFimVenc', this.filtro.periodoVencimentoFim || '');
      this.incluirFiltroComLista(filtros, 'valorInicial', this.filtro.valorVencimemtoInicial || 0);
      this.incluirFiltroComLista(filtros, 'valorFinal', this.filtro.valorVencimemtoFinal || 0);
      this.incluirFiltroComLista(filtros, 'tipo', this.filtro.tipo || 0);
      this.incluirFiltroComLista(filtros, 'comissao', this.filtro.apenasContasDeComissao || 'false');
      this.incluirFiltroComLista(filtros, 'renegociadas', this.filtro.buscarRenegociadas || 'false');
      this.incluirFiltroComLista(filtros, 'jurosMulta', this.filtro.buscarContasComJurosMulta || 'false');
      this.incluirFiltroComLista(filtros, 'planoConta', this.filtro.planoConta || '');
      this.incluirFiltroComLista(filtros, 'custoFixo', this.filtro.apenasContasDeCustoFixo || 'false');
      this.incluirFiltroComLista(filtros, 'exibirAPagar', this.filtro.buscarContasPagar || 'false');
      this.incluirFiltroComLista(filtros, 'observacao', this.filtro.observacao || '');
      this.incluirFiltroComLista(filtros, 'agrupar', this.filtro.agruparImpressaoContasPagasPor || '');
      this.incluirFiltroComLista(filtros, 'receber', false);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Indica que uma conta paga será editada.
     * @param {Object} contaPaga A conta paga que será editada.
     * @param {number} numeroLinha O número da linha que contém a conta a pagar que será editada.
     */
    editar: function (contaPaga, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(contaPaga);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Atualiza os dados da conta paga atual.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function (id, event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.ContasPagar.Pagas.atualizar(id, vm.contaPaga)
        .then(function (resposta) {
          vm.exibirMensagem(resposta.data.mensagem);
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
     * Cancela a edição da conta paga.
     */
    cancelar: function () {
      this.numeroLinhaEdicao = -1;
    },

    /**
     * Retorna os itens para o controle de planos de conta.
     * @param {Object} tipo O tipo de plano de conta (credito/debito).
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterPlanosConta: function () {
      return Servicos.PlanosConta.obterParaControle(this.tipoPlanoConta);
    },

    /**
     * Função executada para criação dos objetos necessários para edição de uma conta paga.
     * @param {?Object} [contaPaga=null] A conta paga que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (contaPaga) {
      this.contaPagaAtual = contaPaga;

      this.contaPaga = {
        idPlanoConta: contaPaga ? contaPaga.idPlanoConta : null,
        observacao: contaPaga && contaPaga.observacao ? contaPaga.observacao.contaPaga : null
      };
    },

    /**
     * Função que indica se o formulário de contas pagas possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de contas a pagar.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },
  },

  watch: {
    /**
     * Observador para a variável 'planoContaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    planoContaAtual: {
      handler: function (atual) {
        this.contaPaga.idPlanoConta = atual ? atual.id : null;
      },
      deep: true
    },
  },

  mounted: function () {
    var vm = this;

    Servicos.ContasPagar.Pagas.obterConfiguracoes()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  },
});
