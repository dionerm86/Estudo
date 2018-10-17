const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'asc')],

  data: {
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca os comissionados para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Comissionados.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém link para a tela de inserção de comissionado.
     */
    obterLinkInserirComissionado: function () {
      return '../Cadastros/CadComissionado.aspx';
    },

    /**
     * Obtém link para a tela de edição de comissionado.
     * @param {Object} item O comissionado que será editado.
     */
    obterLinkEditarComissionado: function (item) {
      return '../Cadastros/CadComissionado.aspx?idComissionado=' + item.id;
    },

    /**
     * Exclui um comissionado.
     * @param {Object} comissionado O comissionado que será excluído.
     */
    excluir: function (comissionado) {
      if (!this.perguntar('Tem certeza que deseja excluir este comissionado?')) {
        return;
      }

      var vm = this;

      Servicos.Comissionados.excluir(comissionado.id)
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
     * Exibe um relatório com a listagem de comissionados aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se será gerado um excel ao invés de um PDF.
     */
    abrirListaComissionados: function (exportarExcel) {
      var filtroReal = this.formatarFiltros_();

      var url = '../Relatorios/RelBase.aspx?rel=ListaComissionado' + filtroReal + '&exportarExcel=' + exportarExcel;

      this.abrirJanela(600, 800, url);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'nome', this.filtro.nome);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);

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

  mounted: function() {
    var vm = this;

    Servicos.Comissionados.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
