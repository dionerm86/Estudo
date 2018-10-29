const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista()],

  data: {
    filtro: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
    planoConta: {},
    planoContaOriginal: {},
    grupoContaAtual: {},
    situacaoAtual: {}
  },

  methods: {
    /**
     * Busca a lista de planos de conta para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.PlanosConta.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna os itens para o controle de grupos de plano de conta.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensGrupoConta: function () {
      return Servicos.PlanosConta.Grupos.obterParaControle();
    },

    /**
     * Inicia o cadastro de plano de conta.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um plano de conta será editado.
     * @param {Object} planoConta O plano de conta que será editado.
     * @param {number} numeroLinha O número da linha que contém o plano de conta que será editado.
     */
    editar: function (planoConta, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(planoConta);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um plano de conta.
     * @param {Object} planoConta O plano de conta que será excluído.
     */
    excluir: function (planoConta) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este plano de conta?')) {
        return;
      }

      var vm = this;

      Servicos.PlanosConta.excluir(planoConta.id)
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
     * Insere um novo plano de conta.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.PlanosConta.inserir(this.planoConta)
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
     * Atualiza os dados do plano de conta.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var planoContaAtualizar = this.patch(this.planoConta, this.planoContaOriginal);
      var vm = this;

      Servicos.PlanosConta.atualizar(this.planoConta.id, planoContaAtualizar)
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
     * Cancela a edição ou cadastro do plano de conta.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de plano de conta.
     * @param {?Object} [planoConta=null] O plano de conta que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (planoConta) {
      this.situacaoAtual = planoConta ? this.clonar(planoConta.situacao) : null;
      this.grupoContaAtual = planoConta ? this.clonar(planoConta.grupoConta) : null;

      this.planoConta = {
        id: planoConta ? planoConta.id : null,
        codigo: planoConta ? planoConta.codigo : null,
        nome: planoConta ? planoConta.nome : null,
        exibirDre: planoConta ? planoConta.exibirDre : null,
        idGrupoConta: planoConta && planoConta.grupoConta ? planoConta.grupoConta.id : null,
        nomeGrupoConta: planoConta && planoConta.grupoConta ? planoConta.grupoConta.nome : null,
        situacao: planoConta && planoConta.situacao ? planoConta.situacao.id : null,
        descricaoSituacao: planoConta && planoConta.situacao ? planoConta.situacao.nome : null,
        permissoes: planoConta ? this.clonar(planoConta.permissoes) : null
      };

      this.planoContaOriginal = this.clonar(this.planoConta);
    },

    /**
     * Exibe um relatório com a listagem de planos de conta aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaPlanosConta: function (exportarExcel) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=ListaPlanoContasDesc' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idGrupo', this.filtro.idGrupoConta);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Função que indica se o formulário de planos de conta possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de planos de conta, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  watch: {
    /**
     * Observador para a variável 'grupoContaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    grupoContaAtual: {
      handler: function (atual) {
        if (this.planoConta) {
          this.planoConta.idGrupoConta = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacaoAtual: {
      handler: function (atual) {
        if (this.planoConta) {
          this.planoConta.situacao = atual ? atual.id : null;
        }
      },
      deep: true
    }
  }
});
