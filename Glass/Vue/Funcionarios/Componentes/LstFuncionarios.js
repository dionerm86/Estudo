const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.FiltroQueryString],

  data: {
    dadosOrdenacao_: {
      campo: 'nome',
      direcao: 'asc'
    },
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca os funcionários para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca de funcionários.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de funcionários.
     */
    atualizarFuncionarios: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Funcionarios.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de funcionários.
     * @param {string} campo O nome do campo pelo qual o resultado será ordenado.
     */
    ordenar: function (campo) {
      if (campo !== this.dadosOrdenacao_.campo) {
        this.dadosOrdenacao_.campo = campo;
        this.dadosOrdenacao_.direcao = '';
      } else {
        this.dadosOrdenacao_.direcao = this.dadosOrdenacao_.direcao === '' ? 'desc' : '';
      }
    },

    /**
     * Retorna o link para a tela de edição de funcionário.
     * @param {Object} item O funcionário que será usado para construção do link.
     * @returns {string} O link que redireciona para a edição de funcionários.
     */
    obterLinkEditarFuncionario: function (item) {
      return '../Cadastros/CadFuncionario.aspx?idFunc=' + item.id;
    },

    /**
     * Retorna o link para a tela de cadastro de funcionário, de acordo com a listagem sendo exibida.
     * @returns {string} O link para a tela de cadastro de funcionário.
     */
    obterLinkInserirFuncionario: function () {
      return '../Cadastros/CadFuncionario.aspx';
    },

    /**
     * Exclui o Funcionário.
     * @param {Object} item O funcionario que será excluído.
     */
    excluir: function (item) {
      if (!this.perguntar("Tem certeza que deseja excluir este funcionário?")) {
        return;
      }

      var vm = this;

      Servicos.Funcionarios.excluir(item.id)
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
     * Exibe um relatório com os dados do funcionário.
     * @param {Object} item O funcionário que será exibido.
     */
    abrirRelatorio: function () {
      var filtroReal = this.formatarFiltros_();
      this.abrirJanela(600, 800, "../Relatorios/RelBase.aspx?rel=ListaFuncionarios" + filtroReal);
    },
    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'nome', this.filtro.nome);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);
      this.incluirFiltroComLista(filtros, 'registrado', this.filtro.apenasRegistrados);
      this.incluirFiltroComLista(filtros, 'idTipoFunc', this.filtro.idTipoFuncionario);
      this.incluirFiltroComLista(filtros, 'idSetorFunc', this.filtro.idSetor);
      this.incluirFiltroComLista(filtros, 'dataNascIni', this.filtro.periodoDataNascimentoInicio);
      this.incluirFiltroComLista(filtros, 'dataNascFim', this.filtro.periodoDataNascimentoFim);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },
    /**
     * Atualiza a lista de funcionários
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
    ordenacao: function () {
      var direcao = this.dadosOrdenacao_.direcao ? ' ' + this.dadosOrdenacao_.direcao : '';
      return this.dadosOrdenacao_.campo + direcao;
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Funcionarios.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
