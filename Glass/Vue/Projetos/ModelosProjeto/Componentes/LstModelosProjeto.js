const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'asc')],

  data: {
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca os modelos de projeto para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Projetos.Modelos.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém link para a tela de inserção de modelo de projeto.
     */
    obterLinkInserirModeloProjeto: function () {
      return 'CadModeloProjeto.aspx';
    },

    /**
     * Obtém link para a tela de edição de modelo de projeto.
     * @param {Object} item O modelo de projeto que será editado.
     */
    obterLinkEditarModeloProjeto: function (item) {
      return this.obterLinkInserirModeloProjeto() + '?idProjetoModelo=' + item.id;
    },

    /**
     * Abre uma tela para edição da posição das peças do modelo de projeto.
     * @param {Object} item O modelo de projeto que terá as peças editadas.
     */
    abrirTelaPosicaoPecasModeloProjeto: function (item) {
      this.abrirJanela(600, 800, 'CadPosicaoPecaModelo.aspx?idProjetoModelo=' + item.id);
    },

    /**
     * Exibe um relatório com detalhes do modelo de projeto informado.
     * @param {Object} item O modelo de projeto que será exibido.
     */
    abrirRelatorioModeloProjeto: function (item) {
      this.abrirJanela(600, 800, '../../Relatorios/Projeto/RelBase.aspx?Rel=ImpressaoModeloProjeto&projModeloCod=' + item.codigo);
    },

    /**
     * Exibe um relatório com a listagem de modelos de projeto aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se será gerado um excel ao invés de um PDF.
     */
    abrirListaModelosProjeto: function (exportarExcel) {
      var filtroReal = this.formatarFiltros_();

      var url = '../../Relatorios/Projeto/RelBase.aspx?Rel=LstModeloProjeto' + filtroReal + '&exportarExcel=' + exportarExcel;

      this.abrirJanela(600, 800, url);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'cod', this.filtro.codigo);
      this.incluirFiltroComLista(filtros, 'desc', this.filtro.descricao);
      this.incluirFiltroComLista(filtros, 'grupo', this.filtro.idGrupoModelo);
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
  }
});
