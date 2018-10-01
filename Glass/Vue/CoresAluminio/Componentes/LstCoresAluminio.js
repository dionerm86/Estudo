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
    corAluminio: {},
    corAluminioOriginal: {}
  },

  methods: {
    /**
     * Busca a lista de cores de alumínio para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Produtos.CoresAluminio.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de cores de alumínio.
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
     * Inicia o cadastro de cor de alumínio.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que uma cor de alumínio será editada.
     * @param {Object} corAluminio A cor de alumínio que será editada.
     * @param {number} numeroLinha O número da linha que contém a cor de alumínio que será editada.
     */
    editar: function (corAluminio, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(corAluminio);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui uma cor de alumínio.
     * @param {Object} corAluminio A cor de alumínio que será excluída.
     */
    excluir: function (corAluminio) {
      var vm = this;

      Servicos.Produtos.CoresAluminio.excluir(corAluminio.id)
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
     * Insere uma nova cor de alumínio.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Produtos.CoresAluminio.inserir(this.corAluminio)
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
     * Atualiza os dados da cor de alumínio.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var corAluminioAtualizar = this.patch(this.corAluminio, this.corAluminioOriginal);
      var vm = this;

      Servicos.Produtos.CoresAluminio.atualizar(this.corAluminio.id, corAluminioAtualizar)
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
     * Cancela a edição ou cadastro da cor de alumínio.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de cor de alumínio.
     * @param {?Object} [processo=null] A cor de alumínio que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (corAluminio) {
      this.corAluminio = {
        id: corAluminio ? corAluminio.id : null,
        sigla: corAluminio ? corAluminio.sigla : null,
        descricao: corAluminio ? corAluminio.descricao : null
      };

      this.corAluminioOriginal = this.clonar(this.corAluminio);
    },

    /**
     * Função que indica se o formulário de cores de alumínio possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de cores de alumínio, com base no filtro atual.
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
