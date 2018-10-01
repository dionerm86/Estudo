const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.Patch],

  data: {
    dadosOrdenacao_: {
      campo: 'descricao',
      direcao: 'asc'
    },
    numeroLinhaEdicao: -1,
    inserindo: false,
    corVidro: {},
    corVidroOriginal: {}
  },

  methods: {
    /**
     * Busca a lista de cores de vidro para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Produtos.CoresVidro.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de cores de vidro.
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
     * Inicia o cadastro de cor de vidro.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que uma cor de vidro será editada.
     * @param {Object} corVidro A cor de vidro que será editada.
     * @param {number} numeroLinha O número da linha que contém a cor de vidro que será editada.
     */
    editar: function (corVidro, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(corVidro);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui uma cor de vidro.
     * @param {Object} corVidro A cor de vidro que será excluída.
     */
    excluir: function (corVidro) {
      var vm = this;

      Servicos.Produtos.CoresVidro.excluir(corVidro.id)
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
     * Insere uma nova cor de vidro.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Produtos.CoresVidro.inserir(this.corVidro)
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
     * Atualiza os dados da cor de vidro.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var corVidroAtualizar = this.patch(this.corVidro, this.corVidroOriginal);
      var vm = this;

      Servicos.Produtos.CoresVidro.atualizar(this.corVidro.id, corVidroAtualizar)
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
     * Cancela a edição ou cadastro da cor de vidro.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de cor de vidro.
     * @param {?Object} [processo=null] A cor de vidro que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (corVidro) {
      this.corVidro = {
        id: corVidro ? corVidro.id : null,
        sigla: corVidro ? corVidro.sigla : null,
        descricao: corVidro ? corVidro.descricao : null
      };

      this.corVidroOriginal = this.clonar(this.corVidro);
    },

    /**
     * Função que indica se o formulário de cores de vidro possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de cores de vidro, com base no filtro atual.
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
  }
});
