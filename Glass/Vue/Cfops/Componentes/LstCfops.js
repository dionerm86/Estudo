const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    configuracoes: {},
    filtro: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
    cfop: {},
    cfopOriginal: {},
    tipoCfopAtual: {},
    tipoMercadoriaAtual: {}
  },

  methods: {
    /**
     * Busca os CFOP's para exibição na lista.
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
     * Retorna os itens para o controle de tipos de CFOP.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensTipoCfop: function () {
      return Servicos.Cfops.obterTiposCfop(false);
    },

    /**
     * Retorna os itens para o controle de tipos de mercadoria.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensTipoMercadoria: function () {
      return Servicos.Cfops.obterTiposMercadoria(false);
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
     * Insere um novo CFOP.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Cfops.inserir(this.cfop)
        .then(function (resposta) {
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
     * Indica que um CFOP será editado.
     * @param {Object} cfop O CFOP que será editado.
     * @param {number} numeroLinha O número da linha que contém o CFOP que será editado.
     */
    editar: function (cfop, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(cfop);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Atualiza os dados do CFOP.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var cfopAtualizar = this.patch(this.cfop, this.cfopOriginal);
      var vm = this;

      Servicos.Cfops.atualizar(this.cfop.id, cfopAtualizar)
        .then(function (resposta) {
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
     * Inicia o cadastro de CFOP.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de CFOP.
     * @param {?Object} [cfop=null] O CFOP que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (cfop) {
      this.tipoCfopAtual = cfop ? this.clonar(cfop.tipoCfop) : null;
      this.tipoMercadoriaAtual = cfop && cfop.tipoMercadoria ? this.clonar(cfop.tipoMercadoria) : null;

      this.cfop = {
        id: cfop ? cfop.id : null,
        codigo: cfop ? cfop.codigo : null,
        nome: cfop ? cfop.nome : null,
        idTipoCfop: cfop && cfop.tipoCfop ? cfop.tipoCfop.id : null,
        tipoMercadoria: cfop && cfop.tipoMercadoria ? cfop.tipoMercadoria.id : null,
        alterarEstoqueTerceiros: cfop ? cfop.alterarEstoqueTerceiros : null,
        alterarEstoqueCliente: cfop ? cfop.alterarEstoqueCliente : null,
        observacao: cfop ? cfop.observacao : null
      };

      this.cfopOriginal = this.clonar(this.cfop);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela.
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'codInterno', this.filtro.codigo);
      this.incluirFiltroComLista(filtros, 'descricao', this.filtro.descricao);
      this.incluirFiltroComLista(filtros, 'orderBy', this.filtro.ordenacaoFiltro);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Função que indica se o formulário de CFOP's possui valores válidos de acordo com os controles.
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
     * Atualiza a lista de CFOP's.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  },

  mounted: function() {
    var vm = this;

    Servicos.Cfops.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  },

  watch: {
    /**
     * Observador para a variável 'tipoCfopAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoCfopAtual: {
      handler: function (atual) {
        if (this.cfop) {
          this.cfop.idTipoCfop = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoMercadoriaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoMercadoriaAtual: {
      handler: function (atual) {
        if (this.cfop) {
          this.cfop.tipoMercadoria = atual ? atual.id : null;
        }
      },
      deep: true
    }
  }
});
