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
    corFerragem: {},
    corFerragemOriginal: {}
  },

  methods: {
    /**
     * Busca a lista de cores de ferragem para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Produtos.CoresFerragem.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de cores de ferragem.
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
     * Inicia o cadastro de cor de ferragem.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que uma cor de ferragem será editada.
     * @param {Object} corFerragem A cor de ferragem que será editada.
     * @param {number} numeroLinha O número da linha que contém a cor de ferragem que será editada.
     */
    editar: function (corFerragem, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(corFerragem);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui uma cor de ferragem.
     * @param {Object} corFerragem A cor de ferragem que será excluída.
     */
    excluir: function (corFerragem) {
      var vm = this;

      Servicos.Produtos.CoresFerragem.excluir(corFerragem.id)
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
     * Insere uma nova cor de ferragem.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Produtos.CoresFerragem.inserir(this.corFerragem)
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
     * Atualiza os dados da cor de ferragem.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var corFerragemAtualizar = this.patch(this.corFerragem, this.corFerragemOriginal);
      var vm = this;

      Servicos.Produtos.CoresFerragem.atualizar(this.corFerragem.id, corFerragemAtualizar)
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
     * Cancela a edição ou cadastro da cor de ferragem.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de cor de ferragem.
     * @param {?Object} [processo=null] A cor de ferragem que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (corFerragem) {
      this.corFerragem = {
        id: corFerragem ? corFerragem.id : null,
        sigla: corFerragem ? corFerragem.sigla : null,
        descricao: corFerragem ? corFerragem.descricao : null
      };

      this.corFerragemOriginal = this.clonar(this.corFerragem);
    },

    /**
     * Função que indica se o formulário de cores de ferragem possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de cores de ferragem, com base no filtro atual.
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
