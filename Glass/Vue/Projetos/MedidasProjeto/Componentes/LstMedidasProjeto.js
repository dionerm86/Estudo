const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('descricao', 'asc')],

  data: {
    filtro: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
    medidaProjeto: {},
    medidaProjetoOriginal: {},
    grupoMedidaAtual: {}
  },

  methods: {
    /**
     * Busca a lista de medidas de projeto para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Projetos.Medidas.obter(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna os itens para o controle de grupos de medida de projeto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensGrupoMedida: function() {
      return Servicos.Projetos.GruposMedida.obterParaControle();
    },

    /**
     * Inicia o cadastro de medida de projeto.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que uma medida de projeto será editada.
     * @param {Object} medidaProjeto A medida de projeto que será editada.
     * @param {number} numeroLinha O número da linha que contém o item que será editado.
     */
    editar: function (medidaProjeto, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(medidaProjeto);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui uma medida de projeto.
     * @param {Object} medidaProjeto A medida de projeto que será excluída.
     */
    excluir: function (medidaProjeto) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir esta medida de projeto?')) {
        return;
      }

      var vm = this;

      Servicos.Projetos.Medidas.excluir(medidaProjeto.id)
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
     * Insere uma nova medida de projeto.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Projetos.Medidas.inserir(this.medidaProjeto)
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
     * Atualiza os dados da medida de projeto.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var medidaProjetoAtualizar = this.patch(this.medidaProjeto, this.medidaProjetoOriginal);
      var vm = this;

      Servicos.Projetos.Medidas.atualizar(this.medidaProjeto.id, medidaProjetoAtualizar)
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
     * Cancela a edição ou cadastro da medida de projeto.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de medida de projeto.
     * @param {?Object} [medidaProjeto=null] A medida de projeto que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (medidaProjeto) {
      this.grupoMedidaAtual = medidaProjeto ? this.clonar(medidaProjeto.grupoMedidaProjeto) : null;

      this.medidaProjeto = {
        id: medidaProjeto ? medidaProjeto.id : null,
        nome: medidaProjeto ? medidaProjeto.nome : null,
        valorPadrao: medidaProjeto ? medidaProjeto.valorPadrao : null,
        exibirApenasEmCalculosDeMedidaExata: medidaProjeto ? medidaProjeto.exibirApenasEmCalculosDeMedidaExata : null,
        exibirApenasEmCalculosDeFerragensEAluminios: medidaProjeto ? medidaProjeto.exibirApenasEmCalculosDeFerragensEAluminios : null,
        idGrupoMedidaProjeto: medidaProjeto && medidaProjeto.grupoMedidaProjeto ? medidaProjeto.grupoMedidaProjeto.id : null
      };

      this.medidaProjetoOriginal = this.clonar(this.medidaProjeto);
    },

    /**
     * Função que indica se o formulário de medidas de projeto possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de medidas de projeto, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  watch: {
    /**
     * Observador para a variável 'grupoMedidaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    grupoMedidaAtual: {
      handler: function (atual) {
        if (this.medidaProjeto) {
          this.medidaProjeto.idGrupoMedidaProjeto = atual ? atual.id : null;
        }
      },
      deep: true
    }
  }
});
