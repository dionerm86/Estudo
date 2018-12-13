const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'asc')],

  data: {
    configuracoes: {},
    filtro: {},
    valorATransferirCaixaGeral: 0,
    diaAtual: {
      caixaFechado: false,
      saldo: 0,
      saldoDinheiro: 0,
      existemMovimentacoes: false
    },
    diaAnterior: {
      caixaFechado: true,
      saldo: 0,
      saldoDinheiro: 0,
      dataCaixaAberto: null
    },
    exibirDadosFechamento: false
  },

  methods: {
    /**
     * Busca os produtos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      if (!this.configuracoes || !Object.keys(this.configuracoes).length) {
        return Promise.reject();
      }

      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Caixas.Diario.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Fecha o caixa diário.
     */
    fechar: function () {
      if (!this.perguntar("Confirmar transferência para o Caixa Geral?")) {
        return;
      }

      var vm = this;

      Servicos.Caixas.Diario.fechar(this.filtro.idLoja, {
        valorATransferirCaixaGeral: this.valorATransferirCaixaGeral,
        saldoTela: this.diaAnterior.caixaFechado ? this.diaAtual.saldo : this.diaAnterior.saldo
      })
        .then(function (resposta) {
          vm.obterDadosFechamento(vm.filtro.idLoja);
          vm.exibirEsconderCamposFechamento();
          vm.atualizarLista();
          vm.exibirMensagem('Concluído', 'Transferência concluída.');
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Reabre o caixa diário.
     */
    reabrir: function () {
      if (!this.perguntar("Tem certeza que deseja reabrir o caixa?")) {
        return;
      }

      var vm = this;

      Servicos.Caixas.Diario.reabrir(this.filtro.idLoja)
        .then(function (resposta) {
          vm.exibirMensagem('Concluído', 'Caixa diário reaberto.');
          vm.obterDadosFechamento(vm.filtro.idLoja);
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Método acionado ao alterar a loja nos filtros da tela.
     * @param {!idLoja} idLoja O identificador da loja que foi alterada.
     */
    lojaAlterada: function (idLoja) {
      this.obterDadosFechamento(idLoja);
    },

    /**
     * Recupera dados usados para fechamento do caixa diário.
     * @param {!idLoja} idLoja O identificador da loja.
     */
    obterDadosFechamento: function (idLoja) {
      var vm = this;

      Servicos.Caixas.Diario.obterDadosFechamento(idLoja)
        .then(function (resposta) {
          vm.diaAnterior = resposta.data.diaAnterior;
          vm.diaAtual = resposta.data.diaAtual;
          vm.valorATransferirCaixaGeral = vm.diaAtual.saldo;
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Exibe/esconde campos usados para o fechamento do caixa.
     */
    exibirEsconderCamposFechamento: function () {
      this.exibirDadosFechamento = !this.exibirDadosFechamento;
    },

    /**
     * Exibe um relatório com a listagem de movimentações de caixa diário aplicando os filtros da tela.
     * @param {Boolean} apenasTotalizadores Define se serão impressos apenas os totalizadores das movimentações.
     */
    abrirMovimentacoes: function (apenasTotalizadores) {
      var filtroReal = this.formatarFiltros_();

      var url = '../Relatorios/RelBase.aspx?Rel=CaixaDiario' + filtroReal + '&somenteTotais=' + apenasTotalizadores;

      this.abrirJanela(600, 800, url);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'idFunc', this.filtro.idFuncionario);
      this.incluirFiltroComLista(filtros, 'data', this.filtro.data);

      return filtros.length > 0
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Atualiza a lista
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  },

  mounted: function() {
    var vm = this;

    Servicos.Caixas.Diario.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
