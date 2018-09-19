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
    totalizadores: {}
  },

  methods: {
    /**
     * Busca os itens para exibição na lista.
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

      this.obterTotaisPorFormaPagamento(filtro);

      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Caixas.Geral.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da listagem.
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
     * Recupera totais por forma de pagamento, usando os mesmos filtros da tela.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     */
    obterTotaisPorFormaPagamento: function (filtro) {
      var vm = this;

      var filtroUsar = this.clonar(filtro || {});

      Servicos.Caixas.Geral.obterTotaisPorFormaPagamento(filtroUsar)
        .then(function (resposta) {
          vm.totalizadores = resposta.data;
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
     * @param {Boolean} exportarExcel Define se será gerado um excel ao invés de um PDF.
     */
    abrirMovimentacoes: function (apenasTotalizadores, exportarExcel) {
      var filtroReal = this.formatarFiltros_();

      var url = '../Relatorios/RelBase.aspx?rel=CaixaGeral' + filtroReal + '&somenteTotais=' + apenasTotalizadores + '&exportarExcel=' + exportarExcel;

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

      incluirFiltro('id', this.filtro.id);
      incluirFiltro('idLoja', this.filtro.idLoja);
      incluirFiltro('idFunc', this.filtro.idFuncionario);
      incluirFiltro('tipoMov', this.filtro.tipo);
      incluirFiltro('DtIni', this.filtro.periodoCadastroInicio ? this.filtro.periodoCadastroInicio.toLocaleDateString('pt-BR') : null);
      incluirFiltro('DtFim', this.filtro.periodoCadastroFim ? this.filtro.periodoCadastroFim.toLocaleDateString('pt-BR') : null);
      incluirFiltro('valorIni', this.filtro.valor);
      incluirFiltro('valorFim', this.filtro.valor);
      incluirFiltro('apenasDinheiro', this.filtro.apenasDinheiro);
      incluirFiltro('apenasCheque', this.filtro.apenasCheque);
      incluirFiltro('semEstorno', this.filtro.apenasEntradaExcetoEstorno);

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

    Servicos.Caixas.Geral.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});