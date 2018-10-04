const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos],

  data: {
    dadosOrdenacao_: {
      campo: 'id',
      direcao: 'asc'
    },
    numeroLinhaEdicao: -1,
    inserindo: false,
    tipoCliente: {},
    tipoClienteOriginal: {}
  },

  methods: {
    /**
     * Busca a lista de tipos de cliente.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Clientes.Tipos.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de tipos de cliente.
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
     * Inicia o cadastro do tipo de cliente.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um tipo de cliente será editado.
     * @param {Object} tipoCliente O tipo de cliente que será editado.
     * @param {number} numeroLinha O número da linha que contém o tipo de cliente que será editado.
     */
    editar: function (tipoCliente, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(tipoCliente);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um tipo de cliente.
     * @param {Object} tipoCliente O tipo de cliente que será excluído.
     */
    excluir: function (tipoCliente) {
      var vm = this;

      Servicos.Clientes.Tipos.excluir(tipoCliente.id)
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
     * Insere um novo tipo de cliente.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Clientes.Tipos.inserir(this.tipoCliente)
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
     * Atualiza os dados do tipo de cliente.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var tipoClienteAtualizar = this.patch(this.tipoCliente, this.tipoClienteOriginal);
      var vm = this;

      Servicos.Clientes.Tipos.atualizar(this.tipoCliente.id, tipoClienteAtualizar)
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
     * Cancela a edição ou cadastro do tipo de cliente.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de tipo de cliente.
     * @param {?Object} [tipoCliente=null] O tipo de cliente que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (tipoCliente) {
      this.tipoCliente = {
        id: tipoCliente ? tipoCliente.id : null,
        descricao: tipoCliente ? tipoCliente.descricao : null,
        cobrarAreaMinima: tipoCliente ? tipoCliente.cobrarAreaMinima : null
      };

      this.tipoClienteOriginal = this.clonar(this.tipoCliente);
    },

    /**
     * Função que indica se o formulário de tipos de cliente possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de tipos de cliente, com base no filtro atual.
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
