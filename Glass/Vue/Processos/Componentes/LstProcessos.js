const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('descricao', 'asc')],

  data: {
    configuracoes: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
    processo: {},
    processoOriginal: {},
    aplicacaoAtual: null,
    tipoProcessoAtual: null,
    situacaoAtual: null
  },

  methods: {
    /**
     * Busca a lista de processos de etiqueta para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Processos.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Cria a descrição para os tipos de pedidos que serão exibidos na listagem.
     * @param {Object} processo O objeto com o processo que será exibido.
     * @returns {string} O texto com a descrição dos tipos de pedidos.
     */
    obterDescricaoTiposPedidos: function (processo) {
      if (!processo.tiposPedidos || !processo.tiposPedidos.length) {
        return null;
      }

      var tiposPedidos = processo.tiposPedidos.slice()
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
     * Inicia o cadastro de processos de etiqueta.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um processo será editado.
     * @param {Object} processo O processo que será editado.
     * @param {number} numeroLinha O número da linha que contém o processo que será editado.
     */
    editar: function (processo, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(processo);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um processo de etiqueta.
     * @param {Object} processo O processo que será excluído.
     */
    excluir: function (processo) {
      var vm = this;

      Servicos.Processos.excluir(processo.id)
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
     * Insere um novo processo de etiqueta.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Processos.inserir(this.processo)
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
     * Atualiza os dados do processo atual.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var processoAtualizar = this.patch(this.processo, this.processoOriginal);
      var vm = this;

      Servicos.Processos.atualizar(this.processo.id, processoAtualizar)
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
     * Cancela a edição ou cadastro de processo.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de processo.
     * @param {?Object} [processo=null] O processo que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (processo) {
      this.aplicacaoAtual = processo ? this.clonar(processo.aplicacao) : null;
      this.tipoProcessoAtual = processo ? this.clonar(processo.tipoProcesso) : null;
      this.situacaoAtual = processo ? this.clonar(processo.situacao) : this.configuracoes.situacaoPadrao;

      var tiposPedidos = function () {
        if (!processo || !processo.tiposPedidos || !processo.tiposPedidos.length) {
          return null;
        }

        return processo.tiposPedidos.slice()
          .map(function (tipoPedido) {
            return tipoPedido.id;
          })
          .filter(function (tipoPedido) {
            return !!tipoPedido;
          });
      };

      this.processo = {
        id: processo ? processo.id : null,
        codigo: processo ? processo.codigo : null,
        descricao: processo ? processo.descricao : null,
        idAplicacao: processo && processo.aplicacao ? processo.aplicacao.id : null,
        destacarNaEtiqueta: processo ? processo.destacarNaEtiqueta : null,
        gerarFormaInexistente: processo ? processo.gerarFormaInexistente : null,
        gerarArquivoDeMesa: processo ? processo.gerarArquivoDeMesa : null,
        forcarGerarSag: processo ? processo.forcarGerarSag : null,
        numeroDiasUteisDataEntrega: processo ? processo.numeroDiasUteisDataEntrega : null,
        tipoProcesso: processo && processo.tipoProcesso ? processo.tipoProcesso.id : null,
        tiposPedidos: tiposPedidos(),
        situacao: processo && processo.situacao ? processo.situacao.id : this.configuracoes.situacaoPadrao.id
      };

      this.processoOriginal = this.clonar(this.processo);
    },

    /**
     * Função que indica se o formulário de processos possui valores válidos de acordo com os controles.
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
     * Recupera os tipos de processo para seleção no controle.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterTiposProcesso: function() {
      return Servicos.Processos.obterTiposProcesso();
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
     * Recupera as situações de processo para seleção no controle.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return Servicos.Processos.obterSituacoes();
    },

    /**
     * Força a atualização da lista de processos, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },

    /**
     * Desmarca o campo de geração de arquivo SAG se não for gerar arquivo de mesa.
     */
    desmarcarGerarSag: function () {
      if (!this.processo.gerarArquivoDeMesa) {
        this.processo.forcarGerarSag = false;
      }
    }
  },

  watch: {
    /**
     * Observador para a variável 'aplicacaoAtual'.
     * Atualiza a aplicação para o processo atual.
     */
    aplicacaoAtual: {
      handler: function (atual) {
        this.processo.idAplicacao = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoProcessoAtual'.
     * Atualiza o tipo para o processo atual.
     */
    tipoProcessoAtual: {
      handler: function (atual) {
        this.processo.tipoProcesso = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza a situação para o processo atual.
     */
    situacaoAtual: {
      handler: function (atual) {
        this.processo.situacao = atual ? atual.id : null;
      },
      deep: true
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Processos.obterConfiguracoes()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
