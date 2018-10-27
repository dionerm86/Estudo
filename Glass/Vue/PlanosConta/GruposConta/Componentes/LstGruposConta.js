const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista()],

  data: {
    filtro: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
    grupoConta: {},
    grupoContaOriginal: {},
    categoriaContaAtual: {},
    situacaoAtual: {}
  },

  methods: {
    /**
     * Busca a lista de grupos de conta para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.PlanosConta.Grupos.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna os itens para o controle de categorias de conta.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensCategoriaConta: function () {
      return Servicos.PlanosConta.Grupos.Categorias.obterParaControle();
    },

    /**
     * Altera a posição do grupo de conta.
     * @param {!number} grupoConta O grupo de conta que terá a posição alterada.
     * @param {!boolean} acima Define se o setor será movimentado para cima..
     */
    alterarPosicao: function (grupoConta, acima) {
      var vm = this;

      Servicos.PlanosConta.Grupos.alterarPosicao(grupoConta.id, acima)
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
     * Inicia o cadastro de grupo de conta.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um grupo de conta será editado.
     * @param {Object} grupoConta O grupo de conta que será editado.
     * @param {number} numeroLinha O número da linha que contém o grupo de conta que será editado.
     */
    editar: function (grupoConta, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(grupoConta);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um grupo de conta.
     * @param {Object} grupoConta O grupo de conta que será excluído.
     */
    excluir: function (grupoConta) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este grupo de conta?')) {
        return;
      }

      var vm = this;

      Servicos.PlanosConta.Grupos.excluir(grupoConta.id)
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
     * Insere um novo grupo de conta.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.PlanosConta.Grupos.inserir(this.grupoConta)
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
     * Atualiza os dados do grupo de conta.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var grupoContaAtualizar = this.patch(this.grupoConta, this.grupoContaOriginal);
      var vm = this;

      Servicos.PlanosConta.Grupos.atualizar(this.grupoConta.id, grupoContaAtualizar)
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
     * Cancela a edição ou cadastro do grupo de conta.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de grupo de conta.
     * @param {?Object} [grupoConta=null] O grupo de conta que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (grupoConta) {
      this.situacaoAtual = grupoConta ? this.clonar(grupoConta.situacao) : null;
      this.categoriaContaAtual = grupoConta ? this.clonar(grupoConta.categoriaConta) : null;

      this.grupoConta = {
        id: grupoConta ? grupoConta.id : null,
        nome: grupoConta ? grupoConta.nome : null,
        exibirPontoEquilibrio: grupoConta ? grupoConta.exibirPontoEquilibrio : null,
        idCategoriaConta: grupoConta && grupoConta.categoriaConta ? grupoConta.categoriaConta.id : null,
        nomeCategoriaConta: grupoConta && grupoConta.categoriaConta ? grupoConta.categoriaConta.nome : null,
        situacao: grupoConta && grupoConta.situacao ? grupoConta.situacao.id : null,
        descricaoSituacao: grupoConta && grupoConta.situacao ? grupoConta.situacao.nome : null,
        permissoes: grupoConta ? this.clonar(grupoConta.permissoes) : null
      };

      this.grupoContaOriginal = this.clonar(this.grupoConta);
    },

    /**
     * Função que indica se o formulário de grupos de conta possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de grupos de conta, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  watch: {
    /**
     * Observador para a variável 'categoriaContaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    categoriaContaAtual: {
      handler: function (atual) {
        if (this.grupoConta) {
          this.grupoConta.idCategoriaConta = atual ? atual.id : null;
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
        if (this.grupoConta) {
          this.grupoConta.situacao = atual ? atual.id : null;
        }
      },
      deep: true
    }
  }
});
