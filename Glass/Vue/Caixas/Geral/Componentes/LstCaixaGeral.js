﻿const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'asc')],

  data: {
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
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'id', this.filtro.id);
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'idFunc', this.filtro.idFuncionario);
      this.incluirFiltroComLista(filtros, 'tipoMov', this.filtro.tipo);
      this.incluirFiltroComLista(filtros, 'DtIni', this.filtro.periodoCadastroInicio);
      this.incluirFiltroComLista(filtros, 'DtFim', this.filtro.periodoCadastroFim);
      this.incluirFiltroComLista(filtros, 'valorIni', this.filtro.valor);
      this.incluirFiltroComLista(filtros, 'valorFim', this.filtro.valor);
      this.incluirFiltroComLista(filtros, 'apenasDinheiro', this.filtro.apenasDinheiro);
      this.incluirFiltroComLista(filtros, 'apenasCheque', this.filtro.apenasCheque);
      this.incluirFiltroComLista(filtros, 'semEstorno', this.filtro.apenasEntradaExcetoEstorno);

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

    Servicos.Caixas.Geral.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
