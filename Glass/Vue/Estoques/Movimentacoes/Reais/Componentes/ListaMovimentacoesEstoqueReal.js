﻿const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('idMovEstoque', 'desc')],

  data: {
    filtro: {},
    configuracoes: {},
    inserindo: false,
    movimentacao: {},
    corlinha: null,
    exibirInclusao: false,
    listaNaoVazia: false,
    tipoMovimentacaoAtual: {}
  },

  methods: {
    /**
     * Busca os estoques de produto para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de estoques de produto.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      if (!this.configuracoes || !Object.keys(this.configuracoes).length) {
        return Promise.reject();
      }

      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Estoques.Movimentacoes.Reais.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna os itens para o controle de tipos de movimentação.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposMovimentacao: function () {
      return Servicos.Estoques.Movimentacoes.TiposMovimentacao.obterParaControle();
    },

    /**
     * Exibe a tela log de cancelamento do extrato de estoque.
     */
    abrirLogCancelamento: function () {
      this.abrirJanela(600, 800, '../Utils/ShowLogCancelamento.aspx?tabela=' + this.configuracoes.codigoTabela);
    },

    /**
     * Exibe o relatório com detalhamento do extrato de estoque para os filtros préviamente informados.
     * @param {Object} item O pedido que será exibido.
     */
    abrirRelatorio: function (exportarExcel) {
      if (Object.keys(this.filtro).length === 0 || !this.filtro.idLoja) {
        return this.exibirMensagem("É necessário informar a loja e um produto antes de gerar o relatório de movimentação.")
      }

      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=ExtratoEstoque' + this.formatarFiltros_() + "&exportarExcel=" + exportarExcel);
    },

    /**
     * Exibe o relatório com detalhamento das movimentações do estoque real para os filtros préviamente informados.
     * @param {Object} item O pedido que será exibido.
     */
    abrirRelatorioMovimentacao: function (exportarExcel) {
      if (Object.keys(this.filtro).length === 0 || !this.filtro.codigoProduto) {
        return this.exibirMensagem("É necessário informar um produto antes de gerar o relatório de movimentação.")
      }

      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=MovEstoque' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel);
    },

    /**
     * Exibe um relatório com o inventário para o produto préviamente informado.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirRelatorioInventario: function (exportarExcel) {
      if (Object.keys(this.filtro).length === 0 || !this.filtro.codigoProduto) {
        return this.exibirMensagem("É necessário informar um produto antes de gerar o relatório de inventário.")
      }

      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=MovEstoqueTotal' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'codInterno', this.filtro.codigoProduto);
      this.incluirFiltroComLista(filtros, 'descricao', this.filtro.descricaoProduto);
      this.incluirFiltroComLista(filtros, 'dataIni', this.filtro.periodoMovimentacaoInicio);
      this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoMovimentacaoFim);
      this.incluirFiltroComLista(filtros, 'tipoMov', this.filtro.tipoMovimentacao);
      this.incluirFiltroComLista(filtros, 'situacaoProd', this.filtro.situacaoProduto);
      this.incluirFiltroComLista(filtros, 'idsGrupoProd', this.filtro.idsGrupoProduto);
      this.incluirFiltroComLista(filtros, 'idsSubgrupoProd', this.filtro.idsSubgrupoProduto);
      this.incluirFiltroComLista(filtros, 'idCorVidro', this.filtro.idCorVidro);
      this.incluirFiltroComLista(filtros, 'idCorFerragem', this.filtro.idCorFerragem);
      this.incluirFiltroComLista(filtros, 'idCorAluminio', this.filtro.idCorAluminio);
      this.incluirFiltroComLista(filtros, 'naoBuscarEstoqueZero', this.filtro.naoBuscarProdutosComEstoqueZerado.toString());
      this.incluirFiltroComLista(filtros, 'apenasLancamentosManuais', this.filtro.apenasLancamentosManuais.toString());
      this.incluirFiltroComLista(filtros, 'usarValorFiscal', this.filtro.usarValorFiscalDoProdutoNoInventario.toString());

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Exclui uma movimentação do estoque real (lançamento manual)
     */
    excluiMovimentacaoEstoqueReal: function (Movimentacao) {
      if (!this.perguntar("Deseja excluir esta movimentação?")) {
        return;
      }

      var vm = this;

      Servicos.Estoques.Movimentacoes.Reais.excluir(Movimentacao.id)
        .then(function (resposta) {
          vm.exibirMensagem(resposta.data.mensagem);
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },

    /**
     * Inicia o cadastro de movimentação.
     */
    iniciarCadastro: function () {
      this.iniciarCadastro_();
      this.inserindo = true;
    },

    /**
     * Cancela o inicio do cadastro de movimentação.
     */
    cancelar: function () {
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de movimentação.     
     */
    iniciarCadastro_: function () {
      var item = this.$refs.lista.itens[0];

      this.movimentacao = {
        idProduto: item.produto.id,
        idLoja: this.filtro.idLoja,
        dataMovimentacao: null,
        quantidade: null,
        valor: null,
        tipoMovimentacao: null,
        observacao: null
      };
    },

    /**
     * Insere uma nova movimentação do estoque real (lançamento manual).
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Estoques.Movimentacoes.Reais.inserir(this.movimentacao)
        .then(function (resposta) {
          vm.exibirMensagem(resposta.data.mensagem)
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
     * Função que indica se o formulário de movimentação possui valores válidos de acordo com os controles.
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
     * Verifica se a lista se encontra vazia (para exibição da inclusão de movimentação somente caso a lista não se encontre vazia).
     * @param {number} numeroItens O número de itens existentes na lista.
     */
    atualizouItens: function (numeroItens) {
      this.listaNaoVazia = numeroItens > 0;
    },
  },

  watch: {
    tipoMovimentacaoAtual: {
      handler: function (atual) {
        this.movimentacao.tipoMovimentacao = atual ? atual.id : null;
      },
      deep: true
    }
  },

  mounted: function () {
    var vm = this;

    //Recuperação das configurações ao carregar o Vue.
    Servicos.Estoques.Movimentacoes.Reais.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
