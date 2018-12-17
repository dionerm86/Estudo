const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('descricao', 'asc')],

  data: {
    inserindo: false,
    tipoFuncionario: {}
  },

  methods: {
    /**
     * Busca a lista de tipos de funcionário para a tela.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Funcionarios.Tipos.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Inicia o cadastro do tipo de funcionário.
     */
    iniciarCadastro: function () {
      this.iniciarCadastro_();
      this.inserindo = true;
    },

    /**
     * Exclui um tipo de funcionário.
     * @param {Object} tipoFuncionario O tipo de funcionário que será excluído.
     */
    excluir: function (tipoFuncionario) {
      if (!this.perguntar('Deseja excluir o tipo de funcionário (' + tipoFuncionario.descricao + ')?')) {
        return;
      }

      var vm = this;

      Servicos.Funcionarios.Tipos.excluir(tipoFuncionario.id)
        .then(function (resposta) {
          vm.exibirMensagem(resposta.data.mensagem);
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Insere um novo tipo de funcionário.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Funcionarios.Tipos.inserir(this.tipoFuncionario)
        .then(function (resposta) {
          vm.exibirMensagem(resposta.data.mensagem);
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
     * Cancela o cadastro do tipo de funcionário.
     */
    cancelar: function () {
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para cadastro do tipo de funcionário.
     */
    iniciarCadastro_: function () {
      this.tipoFuncionario = {
        descricao: null
      };
    },

    /**
     * Função que indica se o formulário de tipo de funcionário possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de tipos de funcionário.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },
  },
});
