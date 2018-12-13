const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos],

  data: {
    filtro: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
    categoriaConta: {},
    categoriaContaOriginal: {},
    tipoAtual: {},
    situacaoAtual: {}
  },

  methods: {
    /**
     * Busca a lista de categorias de conta para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.PlanosConta.Categorias.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna os itens para o controle de tipos de categoria de conta.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensTipo: function () {
      return Servicos.PlanosConta.Categorias.obterTipos();
    },

    /**
     * Altera a posição da categoria de conta.
     * @param {!number} categoriaConta A categoria de conta que terá a posição alterada.
     * @param {!boolean} acima Define se o item será movimentado para cima.
     */
    alterarPosicao: function (categoriaConta, acima) {
      var vm = this;

      Servicos.PlanosConta.Categorias.alterarPosicao(categoriaConta.id, acima)
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
     * Inicia o cadastro de categoria de conta.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que uma categoria de conta será editada.
     * @param {Object} categoriaConta A categoria de conta que será editada.
     * @param {number} numeroLinha O número da linha que contém a categoria de conta que será editada.
     */
    editar: function (categoriaConta, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(categoriaConta);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui uma categoria de conta.
     * @param {Object} categoriaConta O grupo de conta que será excluído.
     */
    excluir: function (categoriaConta) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir esta categoria de conta?')) {
        return;
      }

      var vm = this;

      Servicos.PlanosConta.Categorias.excluir(categoriaConta.id)
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
     * Insere uma nova categoria de conta.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.PlanosConta.Categorias.inserir(this.categoriaConta)
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
     * Atualiza os dados da categoria de conta.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var categoriaContaAtualizar = this.patch(this.categoriaConta, this.categoriaContaOriginal);
      var vm = this;

      Servicos.PlanosConta.Categorias.atualizar(this.categoriaConta.id, categoriaContaAtualizar)
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
     * Cancela a edição ou cadastro da categoria de conta.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de categoria de conta.
     * @param {?Object} [categoriaConta=null] A categoria de conta que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (categoriaConta) {
      this.situacaoAtual = categoriaConta ? this.clonar(categoriaConta.situacao) : null;
      this.tipoAtual = categoriaConta ? this.clonar(categoriaConta.tipo) : null;

      this.categoriaConta = {
        id: categoriaConta ? categoriaConta.id : null,
        nome: categoriaConta ? categoriaConta.nome : null,
        tipo: categoriaConta && categoriaConta.tipo ? categoriaConta.tipo.id : null,
        situacao: categoriaConta && categoriaConta.situacao ? categoriaConta.situacao.id : null
      };

      this.categoriaContaOriginal = this.clonar(this.categoriaConta);
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
     * Força a atualização da lista de categorias de conta, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  },

  watch: {
    /**
     * Observador para a variável 'tipoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoAtual: {
      handler: function (atual) {
        if (this.categoriaConta) {
          this.categoriaConta.tipo = atual ? atual.id : null;
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
        if (this.categoriaConta) {
          this.categoriaConta.situacao = atual ? atual.id : null;
        }
      },
      deep: true
    }
  }
});
