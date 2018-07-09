Vue.component('pedido-ambientes', {
  mixins: [Mixins.Clonar, Mixins.Patch, Mixins.Comparar],

  props: {
    /**
     * O ambiente selecionado.
     * @type {?Object}
     */
    ambiente: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Objeto com os dados do pedido atual.
     * @type {!Object}
     */
    pedido: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de pedidos.
     * @type {!Object}
     */
    configuracoes: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Indica se o pedido atual é mão-de-obra.
     * @type {?boolean}
     */
    pedidoMaoDeObra: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  data: function() {
    return {
      refresh_: 0,
      inserindo: false,
      ambientePedido: {},
      ambientePedidoOriginal: {},
      numeroLinhaEdicao: -1,
      dadosOrdenacao_: {
        campo: '',
        direcao: ''
      },
      produtoAtual: null,
      dadosValidacaoProduto: {},
      processoAtual: null,
      aplicacaoAtual: null,
      exibir: false
    };
  },

  methods: {
    /**
     * Busca os ambientes do pedido atual, com base nos dados de filtro, página, número de registros e ordenação.
     * @param {!Object} filtroOriginal O filtro que será usado para a busca.
     * @param {!number} pagina O número da página que será exibida no resultado.
     * @param {!number} numeroRegistros O número de registros que serão retornados no resultado.
     * @param {string} ordenacao A ordenação usada para o resultado.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    buscarAmbientes: function(filtroOriginal, pagina, numeroRegistros, ordenacao) {
      if (!filtroOriginal.idPedido) {
        return Promise.reject();
      }

      return Servicos.Pedidos.Ambientes.obter(filtroOriginal.idPedido, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Método executado quando ocorre a atualização no número de itens da lista interna.
     * @param {!number} numeroItens O número de itens que foram carregados no controle interno.
     */
    listaAtualizada: function(numeroItens) {
      this.exibir = numeroItens > 0;
    },

    /**
     * Realiza a ordenação dos ambientes de acordo com um campo específico.
     * @param {!string} campo O campo pelo qual a ordenação será feita.
     */
    ordenar: function(campo) {
      if (campo !== this.dadosOrdenacao_.campo) {
        this.dadosOrdenacao_.campo = campo;
        this.dadosOrdenacao_.direcao = '';
      } else {
        this.dadosOrdenacao_.direcao = this.dadosOrdenacao_.direcao === '' ? 'desc' : '';
      }
    },

    /**
     * Exibe os produtos de um ambiente na tela de pedido.
     * @param {!number} id O identificador do ambiente.
     * @param {!string} nome O nome do ambiente.
     * @param {?Object} [produtoMaoDeObra=null] Dados do produto (para ambientes de pedidos mão-de-obra).
     */
    exibirProdutos: function(id, nome, produtoMaoDeObra) {
      this.ambienteAtual = {
        id,
        nome,
        quantidade: produtoMaoDeObra ? produtoMaoDeObra.quantidade : 0
      };
    },

    /**
     * Exibe os dados de um projeto na tela de pedido.
     * @param {!Object} item O ambiente que representa o projeto no pedido.
     */
    exibirProjeto: function (item) {
      if (!this.pedido.entrega || !this.pedido.entrega.tipo || !this.pedido.entrega.tipo.id) {
        this.exibirMensagem('Projeto', 'Selecione o tipo de entrega antes de inserir um projeto.');
      }

      this.abrirJanela(
        screen.height,
        screen.width,
        '../Cadastros/Projeto/CadProjetoAvulso.aspx?IdPedido=' + this.pedido.id
          + '&IdAmbientePedido=' + item.id
          + '&idCliente=' + this.pedido.cliente.id
          + '&tipoEntrega=' + this.pedido.entrega.tipo.id
      );
    },

    /**
     * Altera a lista para o modo de edição de um ambiente.
     * @param {!Object} item O ambiente que será editado.
     * @param {!number} linha A linha da tabela que será alterada para o modo de edição.
     */
    editar: function(item, linha) {
      if (!item || linha === null || linha === undefined) {
        return;
      }

      this.inserindo = false;
      this.iniciarCadastroOuAtualizacao_(item);
      this.numeroLinhaEdicao = linha;
    },

    /**
     * Exclui um ambiente, se possível.
     * @param {!Object} item O ambiente que será excluído.
     */
    excluir: function(item) {
      if (!item || !this.perguntar('Remover ambiente', 'Excluir este ambiente fará com que todos os produtos do mesmo sejam excluídos também, confirma exclusão?')) {
        return;
      }

      var vm = this;

      Servicos.Pedidos.Ambientes.excluir(this.pedido.id, item.id)
        .then(function(resposta) {
          if (vm.ambienteAtual && vm.ambienteAtual.id === item.id) {
            vm.ambienteAtual = null;
          }

          vm.refresh_++;
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Atualiza os dados de um ambiente, se possível.
     * @param {!Object} event O objeto com o evento JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var idAmbienteAtualizar = this.ambientePedido.id;
      var ambienteAtualizar = this.patch(this.ambientePedido, this.ambientePedidoOriginal);

      var vm = this;
      var vmAmbienteAtual = this.ambienteAtual;

      Servicos.Pedidos.Ambientes.atualizar(this.pedido.id, idAmbienteAtualizar, ambienteAtualizar)
        .then(function(resposta) {
          if (vm.ambienteAtual && vm.ambienteAtual.id === idAmbienteAtualizar) {
            vm.ambienteAtual = null;
            vm.$nextTick(function() {
              vm.ambienteAtual = vmAmbienteAtual;
            });
          }

          vm.refresh_++;
          vm.cancelar();
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Cancela uma edição ou cadastro.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Altera o ambiente para o modo de cadastro, criando o objeto para 'bind'.
     */
    iniciarCadastro: function() {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Insere um ambiente no pedido, se possível.
     * @param {!Object} event O objeto com o evento JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Pedidos.Ambientes.inserir(this.pedido.id, this.ambientePedido)
        .then(function(resposta) {
          vm.refresh_++;
          vm.inserindo = false;
          vm.exibirProdutos(resposta.data.id, vm.ambientePedido.nome, vm.ambientePedido.produtoMaoDeObra);
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Cria o objeto que será usado como 'bind' na edição ou cadastro de ambiente.
     * @param {?Object} [item] O item que será usado como base para os valores, no caso de atualização.
     */
    iniciarCadastroOuAtualizacao_: function(item) {
      this.dadosValidacaoProduto = {
        idPedido: this.pedido.id,
        idLoja: this.pedido.loja.id,
        tipoPedido: this.pedido.tipo.id,
        tipoVenda: this.pedido.tipoVenda.id,
        ambiente: true
      };

      this.ambientePedidoOriginal = {
        id: item ? item.id : null,
        nome: item ? item.nome : null,
        produtoMaoDeObra: {
          id: item && item.produtoMaoDeObra ? item.produtoMaoDeObra.id : null,
          codigoInterno: item && item.produtoMaoDeObra ? item.produtoMaoDeObra.codigoInterno : null,
          quantidade: item && item.produtoMaoDeObra ? item.produtoMaoDeObra.quantidade : null,
          altura: item && item.produtoMaoDeObra ? item.produtoMaoDeObra.altura : null,
          largura: item && item.produtoMaoDeObra ? item.produtoMaoDeObra.largura : null,
          processo: {
            id:
              item && item.produtoMaoDeObra && item.produtoMaoDeObra.processo
                ? item.produtoMaoDeObra.processo.id
                : null,
            codigo:
              item && item.produtoMaoDeObra && item.produtoMaoDeObra.processo
                ? item.produtoMaoDeObra.processo.codigo
                : null
          },
          aplicacao: {
            id:
              item && item.produtoMaoDeObra && item.produtoMaoDeObra.aplicacao
                ? item.produtoMaoDeObra.aplicacao.id
                : null,
            codigo:
              item && item.produtoMaoDeObra && item.produtoMaoDeObra.aplicacao
                ? item.produtoMaoDeObra.aplicacao.codigo
                : null
          },
          redondo: item && item.produtoMaoDeObra ? item.produtoMaoDeObra.redondo : false
        },
        descricao: item ? item.descricao : null,
        totalProdutos: item ? item.totalProdutos : null,
        acrescimo: {
          tipo: item && item.acrescimo ? item.acrescimo.tipo : 2,
          valor: item && item.acrescimo ? item.acrescimo.valor : null
        },
        desconto: {
          tipo: item && item.desconto ? item.desconto.tipo : 2,
          valor: item && item.desconto ? item.desconto.valor : null
        }
      };

      this.ambientePedido = this.clonar(this.ambientePedidoOriginal);
      this.ambientePedido.produtoMaoDeObra.id = null;
      this.produtoAtual = null;
    },

    /**
     * Função que indica se o formulário de ambientes possui valores válidos de acordo com os controles.
     * @param {Object} botao O botão que foi disparado no controle.
     * @returns {boolean} Um valor que indica se o formulário está válido.
     */
    validarFormulario_: function(botao) {
      var form = botao.form || botao;
      while (form.tagName.toLowerCase() !== 'form') {
        form = form.parentNode;
      }

      return form.checkValidity();
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o ambiente atual
     * e que atualiza a propriedade em caso de alteração do valor.
     * @type {number}
     */
    ambienteAtual: {
      get: function() {
        return this.ambiente;
      },
      set: function(valor) {
        if (!this.equivalentes(valor, this.ambiente)) {
          this.$emit('update:ambiente', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna a ordenação a ser utilizada na lista.
     * @type {string}
     */
    ordenacao: function() {
      var direcao = this.dadosOrdenacao_.direcao ? ' ' + this.dadosOrdenacao_.direcao : '';
      return this.dadosOrdenacao_.campo + direcao;
    },

    /**
     * Propriedade computada que retorna o filtro atual para os produtos.
     * @type {filtro}
     *
     * @typedef filtro
     * @property {number} idPedido O identificador do pedido.
     * @property {number} refresh Um identificador para atualização dos dados.
     */
    filtro: function() {
      return {
        idPedido: this.pedido.id,
        refresh: this.refresh_
      };
    }
  },

  watch: {
    /**
     * Observador para a variável 'produtoAtual'.
     * Atualiza os dados do ambiente ao alterar o produto.
     */
    produtoAtual: {
      handler: function(atual) {
        if (!this.ambientePedido || !this.ambientePedido.produtoMaoDeObra) {
          return;
        }

        this.ambientePedido.produtoMaoDeObra.id = atual ? atual.id : null;
        this.ambientePedido.nome = atual ? atual.descricao : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'processoAtual'.
     * Atualiza os dados do ambiente ao alterar o processo.
     */
    processoAtual: {
      handler: function(atual) {
        if (
          !this.ambientePedido ||
          !this.ambientePedido.produtoMaoDeObra ||
          !this.ambientePedido.produtoMaoDeObra.processo
        ) {
          return;
        }

        this.ambientePedido.processo.id = atual ? atual.id : null;
        this.ambientePedido.processo.codigo = atual ? atual.codigo : null;

        if (atual && atual.idAplicacao) {
          this.aplicacaoAtual = {
            id: atual.idAplicacao,
            codigo: atual.codigoAplicacao
          };
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'aplicacaoAtual'.
     * Atualiza os dados do ambiente ao alterar a aplicação.
     */
    aplicacaoAtual: {
      handler: function(atual) {
        if (
          !this.ambientePedido ||
          !this.ambientePedido.produtoMaoDeObra ||
          !this.ambientePedido.produtoMaoDeObra.aplicacao
        ) {
          return;
        }

        this.ambientePedido.aplicacao.id = atual ? atual.id : null;
        this.ambientePedido.aplicacao.codigo = atual ? atual.codigo : null;
      },
      deep: true
    }
  },

  template: '#CadPedido-Ambientes-template'
});
