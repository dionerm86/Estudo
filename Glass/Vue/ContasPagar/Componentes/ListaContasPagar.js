const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('dataVenc', 'asc')],

  data: {
    numeroLinhaEdicao: -1,
    contaAPagar: {},
    filtro: {},
    contaAPagarAtual: {
      fornecedor: {},
      transportador: {},
      formaPagamento: {},
      parcela: {},
      datas: {},
      observacoes: {},
      permissoes: {}
    },
    tipoPlanoConta: 2,
    formaPagamentoAtual: {},
    planoContaAtual: {}
  },

  methods: {
    /**
     * Busca a lista de contas a pagar para a tela.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.ContasPagar.obterLista(filtro, pagina, numeroRegistros, ordenacao);
    },

    abrirRelatorio: function (exportarExcel) {
      this.abrirJanela(600, 800, 'RelBase.aspx?rel=ContasPagar' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idContaPg', this.filtro.id);
      this.incluirFiltroComLista(filtros, 'idCompra', this.filtro.idCompra);
      this.incluirFiltroComLista(filtros, 'nf', this.filtro.numeroNotaFiscal);
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'idCustoFixo', this.filtro.idCustoFixo);
      this.incluirFiltroComLista(filtros, 'idImpostoServ', this.filtro.idImpostoServico);
      this.incluirFiltroComLista(filtros, 'idPagtoRestante', this.filtro.idPagamentoRestante);
      this.incluirFiltroComLista(filtros, 'idFornec', this.filtro.idFornecedor);
      this.incluirFiltroComLista(filtros, 'nomeFornec', this.filtro.nomeFornecedor);
      this.incluirFiltroComLista(filtros, 'dtIni', this.filtro.periodoVencimentoInicio);
      this.incluirFiltroComLista(filtros, 'dtFim', this.filtro.periodoVencimentoFim);
      this.incluirFiltroComLista(filtros, 'dataCadIni', this.filtro.periodoCadastroInicio);
      this.incluirFiltroComLista(filtros, 'dataCadFim', this.filtro.periodoCadastroFim);
      this.incluirFiltroComLista(filtros, 'idsFormaPagto', this.filtro.idsFormaPagamento);
      this.incluirFiltroComLista(filtros, 'valorInicial', this.filtro.valorInicial);
      this.incluirFiltroComLista(filtros, 'valorFinal', this.filtro.valorFinal);
      this.incluirFiltroComLista(filtros, 'incluirCheques', this.filtro.buscarCheques);
      this.incluirFiltroComLista(filtros, 'tipo', this.filtro.tipo);
      this.incluirFiltroComLista(filtros, 'previsaoCustoFixo', this.filtro.buscarPrevisaoCustoFixo);
      this.incluirFiltroComLista(filtros, 'comissao', this.filtro.apenasContasDeComissao);
      this.incluirFiltroComLista(filtros, 'planoConta', this.filtro.planoConta);
      this.incluirFiltroComLista(filtros, 'custoFixo', this.filtro.apenasContasDeCustoFixo);
      this.incluirFiltroComLista(filtros, 'contasSemValor', this.filtro.apenasContasComValorAPagar);
      this.incluirFiltroComLista(filtros, 'dtBaixadoIni', this.filtro.periodoPagamentoInicio);
      this.incluirFiltroComLista(filtros, 'dtBaixadoFim', this.filtro.periodoPagamentoFim);
      this.incluirFiltroComLista(filtros, 'dtNfCompraIni', this.filtro.periodoNotaFiscalInicio);
      this.incluirFiltroComLista(filtros, 'dtNfCompraFim', this.filtro.periodoNotaFiscalFim);
      this.incluirFiltroComLista(filtros, 'numCte', this.filtro.numeroCte);
      this.incluirFiltroComLista(filtros, 'idTransportadora', this.filtro.idTransportadora);
      this.incluirFiltroComLista(filtros, 'nomeTransportadora', this.filtro.nomeTransportadora);
      this.incluirFiltroComLista(filtros, 'idFuncComissao', this.filtro.idFuncionarioComissao);
      this.incluirFiltroComLista(filtros, 'idComissao', this.filtro.idComissao);
      this.incluirFiltroComLista(filtros, 'agrupar', this.filtro.agruparImpressaoPor);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Indica que uma conta a pagar será editada.
     * @param {Object} grupoMedidaProjeto O grupo de medida de projeto que será editado.
     * @param {number} numeroLinha O número da linha que contém o processo que será editado.
     */
    editar: function (contaAPagar, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(contaAPagar);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Atualiza os dados da conta a pagar atual.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function (id, event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.ContasPagar.atualizar(id, vm.contaAPagar)
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
     * Cancela a edição da conta a pagar.
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
     * Retorna os itens para o controle de formas de pagamento.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterFormasPagamentoCompras: function () {
      return Servicos.FormasPagamento.obterFormasPagamentoCompra();
    },

    /**
     * Função executada para criação dos objetos necessários para edição de uma conta a pagar.
     * @param {?Object} [contaAPagar=null] A conta a pagar que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (contaAPagar) {
      this.contaAPagarAtual = contaAPagar;

      this.contaAPagar = {
        idPlanoConta: contaAPagar ? contaAPagar.idPlanoConta : null,
        idFormaPagamento: contaAPagar ? contaAPagar.formaPagamento.id : null,
        dataVencimento: contaAPagar ? contaAPagar.datas.vencimento : null,
        observacao: contaAPagar ? contaAPagar.observacoes.contaAPagar : null,
      };
    },

    /**
     * Função que indica se o formulário de grupo de medida de projeto possui valores válidos de acordo com os controles.
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
    formaPagamentoAtual: {
      handler: function (atual) {
        this.contaAPagar.idFormaPagamento = atual ? atual.id : null;
      },
      deep: true
    },

    planoContaAtual: {
      handler: function (atual) {
        this.contaAPagar.idPlanoConta = atual ? atual.id : null;
      },
      deep: true
    }
  }
});
