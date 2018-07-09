Vue.component('pedido-produtos', {
  mixins: [Mixins.Clonar, Mixins.Patch],
  props: {
    /**
     * O ambiente que está selecionado.
     * @type {Object}
     */
    ambiente: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Os dados do pedido atual.
     * @type {Object}
     */
    pedido: {
      required: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * As configurações da tela de pedidos.
     * @type {Object}
     */
    configuracoes: {
      required: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Indica se o pedido atual é de mão-de-obra.
     * @type {boolean}
     */
    pedidoMaoDeObra: {
      required: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Indica se o pedido atual é de produção para corte.
     * @type {boolean}
     */
    pedidoProducaoCorte: {
      required: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Indica se o pedido atual é de mão-de-obra especial.
     * @type {boolean}
     */
    pedidoMaoDeObraEspecial: {
      required: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  data: function() {
    return {
      refresh_: 0,
      inserindo: false,
      produtoPedido: {},
      produtoPedidoOriginal: {},
      numeroLinhaEdicao: -1,
      exibirDescontoQuantidade: false,
      dadosOrdenacao_: {
        campo: '',
        direcao: ''
      },
      produtoAtual: null,
      dadosValidacaoProduto: {},
      numeroBeneficiamentosParaAreaMinima: 0,
      processoAtual: null,
      aplicacaoAtual: null
    };
  },

  methods: {
    /**
     * Busca os produtos para o componente.
     * @param {!Object} filtroOriginal O filtro original informado pelo controle lista-paginada.
     * @param {!number} pagina O número da página que está sendo exibida no controle lista-paginada.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na tela.
     * @param {string} ordenacao O campo pelo qual o resultado será ordenado.
     * @returns {Promise} Uma Promise com a busca dos produtos do pedido, de acordo com o filtro.
     */
    buscarProdutos: function(filtroOriginal, pagina, numeroRegistros, ordenacao) {
      if (!filtroOriginal.idPedido) {
        return Promise.reject();
      }

      return Servicos.Pedidos.Produtos.obter(
        filtroOriginal.idPedido,
        filtroOriginal.idAmbiente,
        pagina,
        numeroRegistros,
        ordenacao
      );
    },

    /**
     * Atualiza os dados para ordenação do resultado.
     * @param {string} campo O campo pelo qual o resultado deve ser ordenado.
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
     * Inicia a edição do produto, se possível.
     * @param {Object} item O produto que será editado.
     * @param {number} linha O número da linha da tabela que entrará em modo de edição.
     */
    editar: function(item, linha) {
      if (!item.permissoes.editar) {
        var mensagem = 'Não é possível editar esse produto porque o pedido possui desconto.\n'
          + 'Aplique o desconto apenas ao terminar o cadastro dos produtos.\n'
          + 'Para continuar, remova o desconto do pedido.';

        this.exibirMensagem('Editar produto', mensagem)
        return;
      }

      this.inserindo = false;
      this.iniciarCadastroOuAtualizacao_(item);
      this.numeroLinhaEdicao = linha;
    },

    /**
     * Exclui um produto, se possível.
     * @param {Object} item O produto que será excluído.
     */
    excluir: function(item) {
      if (!item.permissoes.editar) {
        var mensagem = 'Não é possível remover esse produto porque o pedido possui desconto.\n'
          + 'Aplique o desconto apenas ao terminar o cadastro dos produtos.\n'
          + 'Para continuar, remova o desconto do pedido.';

        this.exibirMensagem('Remover produto', mensagem);
        return;
      }

      if (!this.perguntar('Remover produto', 'Deseja remover esse produto do pedido?')) {
        return;
      }

      var vm = this;

      Servicos.Pedidos.Produtos.excluir(this.pedido.id, item.id)
        .then(function(resposta) {
          vm.refresh_++;
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Inicia o cadastro de um produto.
     */
    iniciarCadastro: function() {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Insere um produto, se possível.
     * @param {Object} event O objeto com o evento JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Pedidos.Produtos.inserir(this.pedido.id, this.produtoPedido)
        .then(function (resposta) {
          vm.refresh_++;
          vm.cancelar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Atualiza um produto, se possível.
     * @param {Object} event O objeto com o evento JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var idProdutoPedidoAtualizar = this.produtoPedido.id;
      var produtoPedidoAtualizar = this.patch(this.produtoPedido, this.produtoPedidoOriginal);
      produtoPedidoAtualizar.beneficiamentos = this.clonar(this.produtoPedido.beneficiamentos);

      var vm = this;

      Servicos.Pedidos.Produtos.atualizar(this.pedido.id, idProdutoPedidoAtualizar, produtoPedidoAtualizar)
        .then(function(resposta) {
          vm.refresh_++;
          vm.cancelar();
        })
        .catch(function(erro) {
          const mensagemAmbienteObrigatorio = 'O ambiente de pedido é obrigatório.';
          if (erro && erro.mensagem && erro.mensagem !== mensagemAmbienteObrigatorio) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Cancela a edição ou cadastro de produto.
     */
    cancelar: function() {
      this.inserindo = false;
      this.numeroLinhaEdicao = -1;
    },

    /**
     * Inicia um cadastro ou edição de produto, criando o objeto para 'bind'.
     * @param {Object} item O produto que será usado como base, no caso de edição.
     */
    iniciarCadastroOuAtualizacao_: function (item) {
      this.produtoAtual = {
        id: item && item.produto ? item.produto.id : null,
        codigo: item && item.produto ? item.produto.codigo : null
      };

      this.processoAtual = {
        id: item && item.processo ? item.processo.id : null,
        codigo: item && item.processo ? item.processo.codigo : null
      };

      this.aplicacaoAtual = {
        id: item && item.aplicacao ? item.aplicacao.id : null,
        codigo: item && item.aplicacao ? item.aplicacao.codigo : null
      };

      this.produtoPedidoOriginal = {
        id: item ? item.id : null,
        produto: {
          id: item && item.produto ? item.produto.id : null,
          espessura: item && item.produto ? item.produto.espessura : null,
          codigo: item && item.produto ? item.produto.codigo : null,
          descricao: item && item.produto ? item.produto.descricao : null,
        },
        quantidade: item ? item.quantidade : null,
        descontoPorQuantidade: {
          percentual: item && item.descontoPorQuantidade ? item.descontoPorQuantidade.percentual : null,
          valor: item && item.descontoPorQuantidade ? item.descontoPorQuantidade.valor : null
        },
        largura: item ? item.largura : null,
        altura: {
          paraCalculo: item && item.altura ? item.altura.paraCalculo : null,
          real: item && item.altura ? item.altura.real : null
        },
        areaEmM2: {
          paraCalculo: item && item.areaEmM2 ? item.areaEmM2.paraCalculo : null,
          real: item && item.areaEmM2 ? item.areaEmM2.real : null
        },
        valorUnitario: item ? item.valorUnitario : null,
        processo: {
          id: item && item.processo ? item.processo.id : null,
          codigo: item && item.processo ? item.processo.codigo : null
        },
        aplicacao: {
          id: item && item.aplicacao ? item.aplicacao.id : null,
          codigo: item && item.aplicacao ? item.aplicacao.codigo : null
        },
        codigoPedidoCliente: item ? item.codigoPedidoCliente : null,
        total: item ? item.total : null,
        beneficiamentos: {
          valor: item && item.beneficiamentos ? item.beneficiamentos.valor : null,
          altura: item && item.beneficiamentos ? item.beneficiamentos.altura : null,
          largura: item && item.beneficiamentos ? item.beneficiamentos.largura : null,
          redondo: item && item.beneficiamentos ? item.beneficiamentos.redondo : null,
          itens: item && item.beneficiamentos && item.beneficiamentos.itens ? item.beneficiamentos.itens.slice() : []
        }
      };

      this.dadosValidacaoProduto = {
        idPedido: this.pedido.id,
        idObra: this.pedido.obra ? this.pedido.obra.id : null,
        idLoja: this.pedido.loja.id,
        tipoPedido: this.pedido.tipo.id,
        tipoVenda: this.pedido.tipoVenda.id,
        ambiente: false,
        tipoEntrega: this.pedido.entrega.tipo.id,
        cliente: {
          id: this.pedido.cliente.id,
          revenda: this.pedido.cliente.revenda
        },
        areaM2DesconsiderarObra: this.produtoPedidoOriginal.areaEmM2.real
      };

      this.produtoPedido = this.clonar(this.produtoPedidoOriginal);
      this.produtoPedido.produto.id = null;
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

      if (!form.checkValidity()) {
        return false;
      }

      if (this.produtoAtual && this.produtoAtual.exigirProcessoEAplicacao) {
        var mensagem;

        if (!this.aplicacaoAtual) {
          mensagem = this.configuracoes.obrigarProcessoEAplicacaoRoteiro
            ? 'É obrigatório informar a aplicação caso algum setor seja to tipo "Por Roteiro" ou "Por Benef.".'
            : 'Informe a aplicação.';

          this.exibirMensagem('Aplicação não informada', mensagem);
          return false;
        }

        if (!this.processoAtual) {
          mensagem = this.configuracoes.obrigarProcessoEAplicacaoRoteiro
            ? 'É obrigatório informar o processo caso algum setor seja to tipo "Por Roteiro" ou "Por Benef.".'
            : 'Informe o processo.';

          this.exibirMensagem('Processo não informado', mensagem);
          return false;
        }
      }

      return true;
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o campo para realizar a ordenação do resultado.
     * @type {string}
     */
    ordenacao: function() {
      var direcao = this.dadosOrdenacao_.direcao ? ' ' + this.dadosOrdenacao_.direcao : '';
      return this.dadosOrdenacao_.campo + direcao;
    },

    /**
     * Propriedade computada que retorna o filtro usado para a busca de produtos.
     * @type {filtro}
     *
     * @typedef filtro
     * @property {number} idPedido O número do pedido atual.
     * @property {?number} idAmbiente O código do ambiente atual.
     * @property {number} refresh Um identificador para atualização da lista.
     */
    filtro: function() {
      return {
        idPedido: this.pedido.id,
        idAmbiente: this.ambiente ? this.ambiente.id : null,
        refresh: this.refresh_
      };
    },

    /**
     * Propriedade computada que retorna o tipo para busca no controle de beneficiamentos.
     * @type {string}
     */
    tipoBeneficiamentos: function () {
      return this.pedidoMaoDeObraEspecial ? 'MaoDeObraEspecial' : 'Venda';
    },

    /**
     * Propriedade computada para exibição do controle de beneficiamentos.
     * @type {boolean}
     */
    vIfControleBeneficiamentos: function () {
      return this.produtoPedido
        && this.produtoPedido.beneficiamentos
        && this.produtoPedido.altura
        && this.produtoPedido.areaEmM2
        && this.produtoAtual
        && this.pedido
        && this.pedido.cliente
        && this.pedido.entrega
        && this.produtoAtual.exibirBeneficiamentos;
    }
  },

  watch: {
    /**
     * Observador para a variável 'produtoAtual'.
     * Atualiza os dados do produto pedido ao alterar o produto.
     */
    produtoAtual: {
      handler: function(atual) {
        if (!this.produtoPedido || !this.produtoPedido.produto) {
          return;
        }

        this.produtoPedido.produto.id = atual ? atual.id : null;
        this.produtoPedido.produto.espessura = atual ? atual.espessura : null;

        if (atual && atual.altura && atual.altura.valor) {
          this.produtoPedido.altura.paraCalculo = atual.altura.valor;
          this.produtoPedido.altura.real = atual.altura.valor;
        }

        if (atual && atual.largura && atual.largura.valor) {
          this.produtoPedido.largura = atual.largura.valor;
        }

        this.produtoPedido.valorUnitario = atual ? atual.valorUnitario : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'processoAtual'.
     * Atualiza os dados do produto pedido ao alterar o processo.
     */
    processoAtual: {
      handler: function(atual) {
        if (!this.produtoPedido || !this.produtoPedido.processo) {
          return;
        }

        this.produtoPedido.processo.id = atual ? atual.id : null;
        this.produtoPedido.processo.codigo = atual ? atual.codigo : null;

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
     * Atualiza os dados do produto pedido ao alterar a aplicação.
     */
    aplicacaoAtual: {
      handler: function(atual) {
        if (!this.produtoPedido || !this.produtoPedido.aplicacao) {
          return;
        }

        this.produtoPedido.aplicacao.id = atual ? atual.id : null;
        this.produtoPedido.aplicacao.codigo = atual ? atual.codigo : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'produtoPedido.descontoPorQuantidade'.
     * Atualiza o percentual de desconto por quantidade no objeto de dados adicionais.
     */
    'produtoPedido.descontoPorQuantidade': function(atual) {
      if (!this.dadosValidacaoProduto) {
        return;
      }

      this.dadosValidacaoProduto.percentualDescontoQuantidade = atual ? atual.percentual : 0;
    },

    /**
     * Observador para a variável 'refresh_'.
     * Emite um evento informando que a lista de produtos foi alterada.
     */
    'refresh_': function () {
      this.$emit('updated');
    }
  },

  template: '#CadPedido-Produtos-template'
});
