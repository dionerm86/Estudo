const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    configuracoes: {},
    filtro: {},
    numeroLinhaEdicao: -1,
    inserindo: false
  },

  methods: {
    /**
     * Busca os produtos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de CFOP's.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Cfops.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Abre um popup para consultar as naturezas de operação do CFOP.
     * @param {Object} item O CFOP que será usado para buscar as naturezas de operação.
     */
    abrirNaturezaOperacao: function (item) {
      this.abrirJanela(600, 800, '../Listas/LstNaturezaOperacao.aspx?idCfop=' + item.id);
    },

    /**
     * Exibe um relatório com a listagem de CFOP's, aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaCfops: function (exportarExcel) {
      var filtroReal = this.formatarFiltros_();

      var url = '../Relatorios/RelBase.aspx?Rel=ListaCfop' + filtroReal + '&exportarExcel=' + exportarExcel;

      this.abrirJanela(600, 800, url);
    },

    /**
     * Inicia o cadastro de CFOP.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um CFOP será editado.
     * @param {Object} cfop O CFOP que será editado.
     * @param {number} numeroLinha O número da linha que contém o CFOP que será editado.
     */
    editar: function (cfop, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(cfop);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um CFOP.
     * @param {Object} cfop O CFOP que será excluído.
     */
    excluir: function (cfop) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este CFOP?')) {
        return;
      }

      var vm = this;

      Servicos.Cfops.excluir(cfop.id)
        .then(function (resposta) {
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Cancela a edição ou cadastro do CFOP.
     */
    cancelar: function () {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Retornar uma string com os filtros selecionados na tela.
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'codInterno', this.filtro.codigo);
      this.incluirFiltroComLista(filtros, 'descricao', this.filtro.descricao);
      this.incluirFiltroComLista(filtros, 'idTipoCfop', this.filtro.idTipoCfop);
      this.incluirFiltroComLista(filtros, 'tipoMercadoria', this.filtro.tipoMercadoria);
      this.incluirFiltroComLista(filtros, 'alterarEstoqueTerceiros', this.filtro.alterarEstoqueTerceiros);
      this.incluirFiltroComLista(filtros, 'alterarEstoqueCliente', this.filtro.alterarEstoqueCliente);
      this.incluirFiltroComLista(filtros, 'obs', this.filtro.obs);
      this.incluirFiltroComLista(filtros, 'orderBy', this.filtro.ordenacaoFiltro);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Atualiza a lista de CFOP's.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  mounted: function() {
    var vm = this;

    Servicos.Cfops.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
