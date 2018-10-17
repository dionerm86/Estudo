const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'asc')],

  data: {
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca os fornecedores para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Fornecedores.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém link para a tela de inserção de fornecedor.
     */
    obterLinkInserirFornecedor: function () {
      return '../Cadastros/CadFornecedor.aspx';
    },

    /**
     * Obtém link para a tela de edição de fornecedor.
     * @param {Object} item O fornecedor que será editado.
     */
    obterLinkEditarFornecedor: function (item) {
      return '../Cadastros/CadFornecedor.aspx?idFornec=' + item.id;
    },

    /**
     * Abre um popup para exibir os preços do fornecedor.
     * @param {Object} item O fornecedor que será usado para consultar os preços.
     */
    abrirPrecosFornecedor: function (item) {
      this.abrirJanela(600, 800, '../Utils/PrecoFornecedor.aspx?idFornec=' + item.id);
    },

    /**
     * Abre um popup para exibir/editar os anexos do fornecedor.
     * @param {Object} item O fornecedor que será usado para exibir/alterar os anexos.
     */
    abrirAnexos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=fornecedor');
    },

    /**
     * Exibe um relatório com os dados do fornecedor.
     * @param {Object} item O fornecedor que será exibido.
     */
    abrirFichaFornecedor: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?Rel=FichaFornecedor&idFornecedor=' + item.id);
    },

    /**
     * Exclui um fornecedor.
     * @param {Object} fornecedor O fornecedor que será excluído.
     */
    excluir: function (fornecedor) {
      if (!this.perguntar('Tem certeza que deseja excluir este fornecedor?')) {
        return;
      }

      var vm = this;

      Servicos.Fornecedores.excluir(fornecedor.id)
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
     * Altera a situação do fornecedor.
     * @param {Object} item O fornecedor que será ativado/inativado.
     */
    alterarSituacao: function (item) {
      if (!this.perguntar("Deseja alterar a situação deste fornecedor?")) {
        return;
      }

      var vm = app;

      Servicos.Fornecedores.alterarSituacao(item.id)
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
     * Exibe um relatório com a listagem de fornecedores aplicando os filtros da tela.
     * @param {Boolean} ficha Define se os fornecedores serão impressos no formato de ficha e não de listagem.
     * @param {Boolean} exportarExcel Define se será gerado um excel ao invés de um PDF.
     */
    abrirListaFornecedores: function (ficha, exportarExcel) {
      var filtroReal = this.formatarFiltros_();

      var url = '../Relatorios/RelBase.aspx?Rel=' + (ficha ? "FichaFornecedor" : "ListaFornecedores") + filtroReal + '&exportarExcel=' + exportarExcel;

      this.abrirJanela(600, 800, url);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idFornecedor', this.filtro.id);
      this.incluirFiltroComLista(filtros, 'nome', this.filtro.nome);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);
      this.incluirFiltroComLista(filtros, 'cnpj', this.filtro.cpfCnpj);
      this.incluirFiltroComLista(filtros, 'credito', this.filtro.comCredito);
      this.incluirFiltroComLista(filtros, 'idConta', this.filtro.idPlanoConta);
      this.incluirFiltroComLista(filtros, 'tipoPagto', this.filtro.idParcela);
      this.incluirFiltroComLista(filtros, 'endereco', this.filtro.endereco);
      this.incluirFiltroComLista(filtros, 'vendedor', this.filtro.vendedor);

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

    Servicos.Fornecedores.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
