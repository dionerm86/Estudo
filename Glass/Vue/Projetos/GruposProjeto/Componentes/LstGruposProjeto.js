const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('descricao', 'asc')],

  data: {
    numeroLinhaEdicao: -1,
    inserindo: false,
    grupoProjeto: {},
    grupoProjetoOriginal: {},
    situacaoAtual: {}
  },

  methods: {
    /**
     * Busca a lista de grupos de projeto para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Projetos.Grupos.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Inicia o cadastro de grupo de projeto.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um grupo de projeto será editado.
     * @param {Object} grupoProjeto O grupo de projeto que será editado.
     * @param {number} numeroLinha O número da linha que contém o grupo de projeto que será editado.
     */
    editar: function (grupoProjeto, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(grupoProjeto);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um grupo de projeto.
     * @param {Object} grupoProjeto O grupo de projeto que será excluído.
     */
    excluir: function (grupoProjeto) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este grupo de projeto?')) {
        return;
      }

      var vm = this;

      Servicos.Projetos.Grupos.excluir(grupoProjeto.id)
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
     * Insere um novo grupo de projeto.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Projetos.Grupos.inserir(this.grupoProjeto)
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
     * Atualiza os dados do grupo de projeto.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var grupoProjetoAtualizar = this.patch(this.grupoProjeto, this.grupoProjetoOriginal);
      var vm = this;

      Servicos.Projetos.Grupos.atualizar(this.grupoProjeto.id, grupoProjetoAtualizar)
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
     * Cancela a edição ou cadastro do grupo de projeto.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de grupo de projeto.
     * @param {?Object} [grupoProjeto=null] O grupo de projeto que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (grupoProjeto) {
      this.situacaoAtual = grupoProjeto ? this.clonar(grupoProjeto.situacao) : null;

      this.grupoProjeto = {
        id: grupoProjeto ? grupoProjeto.id : null,
        nome: grupoProjeto ? grupoProjeto.nome : null,
        boxPadrao: grupoProjeto ? grupoProjeto.boxPadrao : null,
        esquadria: grupoProjeto ? grupoProjeto.esquadria : null,
        situacao: grupoProjeto && grupoProjeto.situacao ? grupoProjeto.situacao.id : null
      };

      this.grupoProjetoOriginal = this.clonar(this.grupoProjeto);
    },

    /**
     * Função que indica se o formulário de grupos de projeto possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de grupos de projeto, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  },

  watch: {
    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacaoAtual: {
      handler: function (atual) {
        if (this.grupoProjeto) {
          this.grupoProjeto.situacao = atual ? atual.id : null;
        }
      },
      deep: true
    }
  }
});
