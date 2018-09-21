const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.FiltroQueryString],

  data: {
    dadosOrdenacao_: {
      campo: 'id',
      direcao: 'desc'
    },
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca os produtos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de produtos.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Produtos.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
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
     * Retorna o link para a tela de inserção de produtos.
     */
    obterLinkInserirProduto: function (item) {
      return '../Cadastros/CadProduto.aspx';
    },

    /**
     * Retorna o link para a tela de edição de produtos.
     * @param {Object} item O produto que será usado para construção do link.
     * @returns {string} O link que redireciona para a edição de produtos.
     */
    obterLinkEditarProduto: function(item) {
      return '../Cadastros/CadProduto.aspx?idProd=' + item.id;
    },

    /**
     * Abre um popup para cadastrar preços de fornecedores para o produto informado.
     * @param {Object} item O produto que será usado para cadastrar os preços de fornecedor.
     */
    abrirPrecoFornecedor: function (item) {
      this.abrirJanela(600, 800, '../Utils/PrecoFornecedor.aspx?idProd=' + item.id);
    },

    /**
     * Abre um popup para exibir preços anteriores do produto.
     * @param {Object} item O produto que será usado para gerar o relatório.
     */
    abrirPrecoAnterior: function (item) {
      this.abrirJanela(300, 400, '../Utils/ShowPrecoAnterior.aspx?idProd=' + item.id);
    },

    /**
     * Abre um popup para cadastrar o desconto por quantidade do produto.
     * @param {Object} item O produto que será usado para cadastrar o desconto.
     */
    abrirDescontoPorQuantidade: function (item) {
      this.abrirJanela(600, 800, '../Cadastros/CadDescontoQtde.aspx?idProd=' + item.id);
    },

    /**
     * Abre um popup para exibir um relatório de reserva do produto.
     * @param {Object} item O produto que será usado para exibir a reserva.
     */
    abrirReserva: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=EstoqueProdutos&TipoColunas=1&idProd=' + item.id + '&agrupar=false');
    },

    /**
     * Abre um popup para exibir um relatório de liberação do produto.
     * @param {Object} item O produto que será usado para exibir a liberação.
     */
    abrirLiberacao: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=EstoqueProdutos&TipoColunas=2&idProd=' + item.id + '&agrupar=false');
    },

    /**
     * Abre um popup para exibir opções de importação/exportação de preços de produto.
     */
    abrirImportacaoExportacaoPrecos: function () {
      this.abrirJanela(400, 600, '../Utils/ExportarPrecoProduto.aspx');
    },

    /**
     * Exibe um relatório com a impressão dos dados produto informado.
     * @param {Object} item O produto que será impresso.
     */
    abrirImpressaoProduto: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?Rel=FichaProdutos&idProduto=' + item.id + '&colunas=0');
    },

    /**
     * Exibe um relatório com a listagem de produtos aplicando os filtros da tela.
     * @param {Boolean} ficha Define se os produtos serão impressos no formato de ficha e não de listagem.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaProdutos: function (ficha, exportarExcel) {
      var filtroReal = this.formatarFiltros_();

      var url = '../Relatorios/RelBase.aspx?Rel=' + (ficha ? 'Ficha' : '') + 'Produtos' + filtroReal + '&exportarExcel=' + exportarExcel;

      this.abrirJanela(600, 800, url);
    },

    /**
     * Gera o relatório de preços de produtos para alteração.
     */
    abrirExportacaoPrecosProdutos: function () {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=ProdutosPreco'
        + this.formatarFiltros_()
        + '&exportarExcel=true');
    },

    /**
     * Exclui o produto.
     * @param {Object} item O produto que será excluído.
     */
    excluir: function (item) {
      if (!this.perguntar("Tem certeza que deseja excluir este produto?")) {
        return;
      }

      var vm = this;

      Servicos.Produtos.excluir(item.id)
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
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'codInterno', this.filtro.codigo);
      this.incluirFiltroComLista(filtros, 'descr', this.filtro.descricao);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);
      this.incluirFiltroComLista(filtros, 'idGrupo', this.filtro.idGrupo);
      this.incluirFiltroComLista(filtros, 'idSubgrupo', this.filtro.idSubgrupo);
      this.incluirFiltroComLista(filtros, 'alturaInicio', this.filtro.valorAlturaInicio);
      this.incluirFiltroComLista(filtros, 'alturaFim', this.filtro.valorAlturaFim);
      this.incluirFiltroComLista(filtros, 'larguraInicio', this.filtro.valorLarguraInicio);
      this.incluirFiltroComLista(filtros, 'larguraFim', this.filtro.valorLarguraFim);
      this.incluirFiltroComLista(filtros, 'orderBy', this.filtro.ordenacaoFiltro);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Atualiza a lista de produtos
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

    Servicos.Produtos.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
