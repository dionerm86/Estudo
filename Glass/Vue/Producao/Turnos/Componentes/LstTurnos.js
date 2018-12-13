const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('descricao', 'asc')],

  data: {
    numeroLinhaEdicao: -1,
    inserindo: false,
    turno: {},
    turnoOriginal: {},
    sequenciaAtual: {}
  },

  methods: {
    /**
     * Busca a lista de turnos para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Producao.Turnos.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna os itens para o controle de sequências de turno.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensSequencia: function () {
      return Servicos.Producao.Turnos.obterSequencias();
    },

    /**
     * Inicia o cadastro de turno.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um turno será editado.
     * @param {Object} turno O turno que será editado.
     * @param {number} numeroLinha O número da linha que contém o turno que será editado.
     */
    editar: function (turno, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(turno);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um turno.
     * @param {Object} turno O turno que será excluído.
     */
    excluir: function (turno) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este turno?')) {
        return;
      }

      var vm = this;

      Servicos.Producao.Turnos.excluir(turno.id)
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
     * Insere um novo turno.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Producao.Turnos.inserir(this.turno)
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
     * Atualiza os dados do turno.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var turnoAtualizar = this.patch(this.turno, this.turnoOriginal);
      var vm = this;

      Servicos.Producao.Turnos.atualizar(this.turno.id, turnoAtualizar)
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
     * Cancela a edição ou cadastro do turno.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de turno.
     * @param {?Object} [turno=null] O turno que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (turno) {
      this.sequenciaAtual = turno ? this.clonar(turno.sequencia) : null;

      this.turno = {
        id: turno ? turno.id : null,
        nome: turno ? turno.nome : null,
        inicio: turno ? turno.inicio : null,
        termino: turno ? turno.termino : null,
        sequencia: turno && turno.sequencia ? turno.sequencia.id : null
      };

      this.turnoOriginal = this.clonar(this.turno);
    },

    /**
     * Função que indica se o formulário de turnos possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de turnos, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  },

  watch: {
    /**
     * Observador para a variável 'sequenciaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    sequenciaAtual: {
      handler: function (atual) {
        if (this.turno) {
          this.turno.sequencia = atual ? atual.id : null;
        }
      },
      deep: true
    }
  }
});
