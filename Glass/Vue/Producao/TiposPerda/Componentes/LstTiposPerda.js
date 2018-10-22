const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('descricao', 'asc')],

  data: {
    numeroLinhaEdicao: -1,
    inserindo: false,
    tipoPerda: {},
    tipoPerdaOriginal: {},
    setorAtual: {},
    situacaoAtual: {}
  },

  methods: {
    /**
     * Busca a lista de tipos de perda para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Producao.TiposPerda.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém link para a tela de subtipos do tipo de perda informado.
     * @param {Object} item O tipo de perda que será usado para exibir os subtipos.
     */
    obterLinkSubtiposPerda: function (item) {
      return 'CadSubtipoPerda.aspx?idTipoPerda=' + item.id;
    },

    /**
     * Retorna os itens para o controle de setores.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensSetor: function () {
      return Servicos.Producao.Setores.obterParaControle(false, false);
    },

    /**
     * Retorna os itens para o controle de situações.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensSituacao: function () {
      return Servicos.Producao.TiposPerda.obterSituacoes(false, false);
    },

    /**
     * Inicia o cadastro de tipo de perda.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um tipo de perda será editado.
     * @param {Object} tipoPerda O tipo de perda que será editado.
     * @param {number} numeroLinha O número da linha que contém o tipo de perda que será editado.
     */
    editar: function (tipoPerda, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(tipoPerda);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um tipo de perda.
     * @param {Object} tipoPerda O tipo de perda que será excluído.
     */
    excluir: function (tipoPerda) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este tipo de perda?')) {
        return;
      }

      var vm = this;

      Servicos.Producao.TiposPerda.excluir(tipoPerda.id)
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
     * Insere um novo tipo de perda.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Producao.TiposPerda.inserir(this.tipoPerda)
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
     * Atualiza os dados do tipo de perda.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var tipoPerdaAtualizar = this.patch(this.tipoPerda, this.tipoPerdaOriginal);
      var vm = this;

      Servicos.Producao.TiposPerda.atualizar(this.tipoPerda.id, tipoPerdaAtualizar)
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
     * Cancela a edição ou cadastro do tipo de perda.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de tipo de perda.
     * @param {?Object} [tipoPerda=null] O tipo de perda que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (tipoPerda) {
      this.setorAtual = tipoPerda ? this.clonar(tipoPerda.setor) : null;
      this.situacaoAtual = tipoPerda ? this.clonar(tipoPerda.situacao) : null;

      this.tipoPerda = {
        id: tipoPerda ? tipoPerda.id : null,
        nome: tipoPerda ? tipoPerda.nome : null,
        exibirNoPainelDeProducao: tipoPerda ? tipoPerda.exibirNoPainelDeProducao : null,
        situacao: tipoPerda && tipoPerda.situacao ? tipoPerda.situacao.id : null,
        idSetor: tipoPerda && tipoPerda.setor ? tipoPerda.setor.id : null
      };

      this.tipoPerdaOriginal = this.clonar(this.tipoPerda);
    },

    /**
     * Função que indica se o formulário de tipos de perda possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de tipos de perda, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  watch: {
    /**
     * Observador para a variável 'setorAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    setorAtual: {
      handler: function (atual) {
        if (this.tipoPerda) {
          this.tipoPerda.idSetor = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacaoAtual: {
      handler: function (atual) {
        if (this.tipoPerda) {
          this.tipoPerda.situacao = atual ? atual.id : null;
        }
      },
      deep: true
    }
  }
});
