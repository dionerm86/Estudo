const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('nome', 'asc')],

  data: {
    numeroLinhaEdicao: -1,
    inserindo: false,
    seguradora: {},
    seguradoraOriginal: {},
    sequenciaAtual: {}
  },

  methods: {
    /**
     * Busca a lista de seguradoras para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Seguradoras.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Inicia o cadastro de seguradora.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que uma seguradora será editada.
     * @param {Object} seguradora A seguradora que será editada.
     * @param {number} numeroLinha O número da linha que contém a seguradora que será editada.
     */
    editar: function (seguradora, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(seguradora);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui uma seguradora.
     * @param {Object} seguradora A seguradora que será excluída.
     */
    excluir: function (seguradora) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir esta seguradora?')) {
        return;
      }

      var vm = this;

      Servicos.Seguradoras.excluir(seguradora.id)
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
     * Insere uma nova seguradora.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Seguradoras.inserir(this.seguradora)
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
     * Atualiza os dados da seguradora.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var seguradoraAtualizar = this.patch(this.seguradora, this.seguradoraOriginal);
      var vm = this;

      Servicos.Seguradoras.atualizar(this.seguradora.id, seguradoraAtualizar)
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
     * Cancela a edição ou cadastro da seguradora.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de seguradora.
     * @param {?Object} [seguradora=null] A seguradora que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (seguradora) {
      this.seguradora = {
        id: seguradora ? seguradora.id : null,
        nome: seguradora ? seguradora.nome : null,
        cnpj: seguradora ? seguradora.cnpj : null
      };

      this.seguradoraOriginal = this.clonar(this.seguradora);
    },

    /**
     * Função que indica se o formulário de seguradoras possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de seguradoras, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  }
});
