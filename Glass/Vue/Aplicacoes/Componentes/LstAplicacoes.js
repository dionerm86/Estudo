const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.Patch],

  data: {
    dadosOrdenacao_: {
      campo: 'descricao',
      direcao: 'asc'
    },
    configuracoes: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
    aplicacao: {},
    aplicacaoOriginal: {},
    situacaoAtual: null
  },

  methods: {
    /**
     * Recupera a lista de aplicações (etiqueta) para uso no controle de busca.
     * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Aplicacoes.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de aplicacoes.
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
     * Cria a descrição para os tipos de pedidos que serão exibidos na listagem.
     * @param {Object} aplicacao O objeto com a aplicação que será exibida.
     * @returns {string} O texto com a descrição dos tipos de pedidos.
     */
    obterDescricaoTiposPedidos: function (aplicacao) {
      if (!aplicacao.tiposPedidos || !aplicacao.tiposPedidos.length) {
        return null;
      }

      var tiposPedidos = aplicacao.tiposPedidos.slice()
        .map(function (item) {
          return item.nome;
        })
        .filter(function (item) {
          return !!item;
        });

      tiposPedidos.sort();
      return tiposPedidos.join(', ');
    },

    /**
     * Inicia o cadastro de aplicações de etiqueta.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que uma aplicação será editado.
     * @param {Object} aplicacao A aplicacao que será editado.
     * @param {number} numeroLinha O número da linha que contém a aplicacao que será editado.
     */
    editar: function (aplicacao, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(aplicacao);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui uma aplicação de etiqueta.
     * @param {Object} aplicacao A aplicação que será excluída.
     */
    excluir: function (aplicacao) {
      var vm = this;

      Servicos.Aplicacoes.excluir(aplicacao.id)
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
     * Insere uma nova aplicação de etiqueta.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Aplicacoes.inserir(this.aplicacao)
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
     * Atualiza os dados da aplicaçao atual.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var aplicacaoAtualizar = this.patch(this.aplicacao, this.aplicacaoOriginal);
      var vm = this;

      Servicos.Aplicacoes.atualizar(this.aplicacao.id, aplicacaoAtualizar)
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
     * Cancela a edição ou cadastro de aplicação.
     */
    cancelar: function () {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de aplicação.
     * @param {?Object} [aplicacao=null] A aplicação que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (aplicacao) {
      this.situacaoAtual = aplicacao ? this.clonar(aplicacao.situacao) : this.configuracoes.situacaoPadrao;

      var tiposPedidos = function () {
        if (!aplicacao || !aplicacao.tiposPedidos || !aplicacao.tiposPedidos.length) {
          return null;
        }

        return aplicacao.tiposPedidos.slice()
          .map(function (tipoPedido) {
            return tipoPedido.id;
          })
          .filter(function (tipoPedido) {
            return !!tipoPedido;
          });
      };

      this.aplicacao = {
        id: aplicacao ? aplicacao.id : null,
        codigo: aplicacao ? aplicacao.codigo : null,
        descricao: aplicacao ? aplicacao.descricao : null,
        destacarNaEtiqueta: aplicacao ? aplicacao.destacarNaEtiqueta : null,
        gerarFormaInexistente: aplicacao ? aplicacao.gerarFormaInexistente : null,
        naoPermitirFastDelivery: aplicacao ? aplicacao.naoPermitirFastDelivery : null,
        numeroDiasUteisDataEntrega: aplicacao ? aplicacao.numeroDiasUteisDataEntrega : null,
        tiposPedidos: tiposPedidos(),
        situacao: aplicacao && aplicacao.situacao ? aplicacao.situacao.id : this.configuracoes.situacaoPadrao.id
      };

      this.aplicacaoOriginal = this.clonar(this.aplicacao);
    },

    /**
     * Função que indica se o formulário de aplicações possui valores válidos de acordo com os controles.
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
     * Recupera os tipos de pedido para seleção no controle.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterTiposPedido: function () {
      var vm = this;

      return Servicos.Pedidos.obterTiposPedido()
        .then(function (resposta) {
          if (resposta.data) {
            resposta.data = resposta.data
              .filter(function (item) {
                return vm.configuracoes.tiposPedidosIgnorar.indexOf(item.id) === -1;
              });
          }

          return resposta;
        });
    },

    /**
     * Recupera as situações de aplicação para seleção no controle.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return Servicos.Aplicacoes.obterSituacoes();
    },

    /**
     * Força a atualização da lista de aplicações, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  computed: {
    /**
     * Propriedade computada para controlar a ordenação da Lista.
     * @type {string}
     */
    ordenacao: function () {
      var direcao = this.dadosOrdenacao_.direcao ? ' ' + this.dadosOrdenacao_.direcao : '';
      return this.dadosOrdenacao_.campo + direcao;
    }
  },

  watch: {
    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza a situação para a aplicacao atual.
     */
    situacaoAtual: {
      handler: function (atual) {
        this.aplicacao.situacao = atual ? atual.id : null;
      },
      deep: true
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Aplicacoes.obterConfiguracoes()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
})
