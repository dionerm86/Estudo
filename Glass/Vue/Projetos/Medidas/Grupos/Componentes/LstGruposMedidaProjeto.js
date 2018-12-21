const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('nome', 'asc')],

  data: {
    numeroLinhaEdicao: -1,
    inserindo: false,
    grupoMedidaProjeto: {},
  },

  methods: {
    /**
     * Busca a lista de grupos de medida de projeto para a tela.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Projetos.Medidas.Grupos.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Inicia o cadastro do grupo de medida de projeto.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um grupo de medida de projeto será editado.
     * @param {Object} grupoMedidaProjeto O grupo de medida de projeto que será editado.
     * @param {number} numeroLinha O número da linha que contém o processo que será editado.
     */
    editar: function (grupoMedidaProjeto, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(grupoMedidaProjeto);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um grupo de medida de projeto.
     * @param {Object} grupoMedidaProjeto O grupo de medida de projeto que será excluído.
     */
    excluir: function (grupoMedidaProjeto) {
      if (!this.perguntar('Deseja excluir o grupo de medida de projeto (' + grupoMedidaProjeto.nome + ')?')) {
        return;
      }

      var vm = this;

      Servicos.Projetos.Medidas.Grupos.excluir(grupoMedidaProjeto.id)
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
     * Insere um novo grupo de medida de projeto.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Projetos.Medidas.Grupos.inserir(this.grupoMedidaProjeto)
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
     * Atualiza os dados do grupo de medida de projeto atual.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Projetos.Medidas.Grupos.atualizar(vm.grupoMedidaProjeto.id, vm.grupoMedidaProjeto)
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
     * Cancela a edição ou cadastro de grupo de medida de projeto.
     */
    cancelar: function () {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de grupo de medida de projeto.
     * @param {?Object} [grupoMedidaProjeto=null] O grupo de medida de projeto que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (grupoMedidaProjeto) {
      this.grupoMedidaProjeto = {
        id: grupoMedidaProjeto ? grupoMedidaProjeto.id : null,
        nome: grupoMedidaProjeto ? grupoMedidaProjeto.nome : null
      };
    },

    /**
     * Função que indica se o formulário de grupo de medida de projeto possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de grupos de medida de projeto.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },
  },
});
