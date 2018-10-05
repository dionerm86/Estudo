const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.Patch],

  data: {
    dadosOrdenacao_: {
      campo: 'nome',
      direcao: 'asc'
    },
    configuracoes: {},
    condutor: {},
    condutorOriginal: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
  },

  methods: {
    /**
     * Busca a lista de condutores para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Condutores.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de condutores.
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
     * Inicia o cadastro de condutores.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um condutores será editado.
     * @param {Object} O condutor que será editado.
     * @param {number} numeroLinha O número da linha que contém o condutor que será editado.
     */
    editar: function (condutor, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(condutor);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um condutor de etiqueta.
     * @param {Object} O condutor que será excluído.
     */
    excluir: function (condutor) {
      var vm = this;

      Servicos.Condutores.excluir(condutor.id)
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
     * Insere um novo condutor.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Condutores.inserir(this.condutor)
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
     * Atualiza os dados do condutor atual.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var condutorAtualizar = this.patch(this.condutor, this.condutorOriginal);
      var vm = this;

      Servicos.Condutores.atualizar(this.condutor.id, condutorAtualizar)
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
     * Força a atualização da lista de codnutores, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    },

    /**
     * Cancela a edição ou cadastro de condutor.
     */
    cancelar: function () {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de condutor.
     * @param {?Object} [condutor=null] O condutor que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (condutor) {

      this.condutor = {
        id: condutor ? condutor.id : null,
        nome: condutor ? condutor.nome : null,
        cpfCnpj: condutor ? condutor.cpfCnpj : null,
      };

      this.condutorOriginal = this.clonar(this.condutor);
    },

    /**
     * Função que indica se o formulário de condutores possui valores válidos de acordo com os controles.
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
