const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar],

  data: {
    dadosOrdenacao_: {
      campo: 'id',
      direcao: 'asc'
    },
    configuracoes: {},
    filtro: {},
    valorATransferirCaixaGeral: 0,
    diaAtual: {
      caixaFechado: true,
      saldo: 0,
      saldoDinheiro: 0,
      existemMovimentacoes: true
    },
    diaAnterior: {
      caixaFechado: true,
      saldo: 0,
      saldoDinheiro: 0,
      dataCaixaAberto: null
    }
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
     * Realiza a ordenação da lista de produtos.
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
     * Fecha o caixa diário.
     */
    fechar: function () {
      if (!this.perguntar("Confirmar transferência para o Caixa Geral?")) {
        return;
      }

      var vm = this;

      Servicos.Caixas.Diario.fechar(filtro.idLoja, {
        valorATransferirCaixaGeral: this.valorATransferirCaixaGeral,
        saldoTela: this.diaAnterior.caixaFechado ? this.diaAtual.saldo : this.diaAnterior.saldo
      })
        .then(function (resposta) {
          vm.exibirMensagem('Concluído', 'Transferência concluída.');
          vm.obterDadosFechamento();
          vm.atualizarLista();
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

      Servicos.Caixas.Diario.reabrir(filtro.idLoja)
        .then(function (resposta) {
          vm.exibirMensagem('Concluído', 'Caixa diário reaberto.');
          vm.obterDadosFechamento();
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
          vm.diaAnterior = resposta.diaAnterior;
          vm.diaAtual = resposta.diaAtual;
          vm.valorATransferirCaixaGeral = vm.diaAtual.saldo;
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
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
      var filtros = []
      const incluirFiltro = function (campo, valor) {
        if (valor) {
          filtros.push(campo + '=' + valor);
        }
      }

      incluirFiltro('idLoja', this.filtro.idLoja);
      incluirFiltro('idFunc', this.filtro.idFuncionario);
      incluirFiltro('data', this.filtro.data);

      return filtros.length > 0
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Atualiza a lista
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

    Servicos.Caixas.Diario.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});