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
      atualizar_: 0,
      dadosValidacaoProduto: {},
      controleTooltipAtual: null
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
        filtroOriginal.idProdutoPai,
        pagina,
        numeroRegistros,
        ordenacao
      );
    },

    /**
     * Inicia a edição do produto, se possível.
     * @param {Object} item O produto que será editado.
     * @returns {boolean} Verdadeiro, se o produto puder ser editado.
     */
    editar: function(item) {
      if (!item.permissoes.editar) {
        var mensagem = 'Não é possível editar esse produto porque o pedido possui desconto.\n'
          + 'Aplique o desconto apenas ao terminar o cadastro dos produtos.\n'
          + 'Para continuar, remova o desconto do pedido.';

        this.exibirMensagem('Editar produto', mensagem)
        return false;
      }

      return true;
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
        return Promise.reject();
      }

      if (!this.perguntar('Remover produto', 'Deseja remover esse produto do pedido?')) {
        return Promise.reject();
      }

      return Servicos.Pedidos.Produtos.excluir(this.pedido.id, item.id);
    },

    /**
     * Insere um produto, se possível.
     * @param {Object} item O produto de pedido que será inserido.
     */
    inserir: function (item) {
      return Servicos.Pedidos.Produtos.inserir(this.pedido.id, item);
    },

    /**
     * Atualiza um produto, se possível.
     * @param {Object} item O produto de pedido que será atualizado.
     */
    atualizar: function (item) {
      return Servicos.Pedidos.Produtos.atualizar(this.pedido.id, item.id, item)
        .catch(function(erro) {
          const mensagemAmbienteObrigatorio = 'O ambiente de pedido é obrigatório.';
          if (erro && erro.mensagem && erro.mensagem === mensagemAmbienteObrigatorio) {
            Promise.reject();
          } else {
            Promise.reject(erro);
          }
        });
    },

    /**
     * Atualiza o filtro de produtos, para recarregar a lista.
     * Além disso, emite um evento para que o pedido também seja recarregado.
     */
    listaAtualizada: function () {
      this.atualizar_++;
      this.$emit('lista-atualizada');
    },

    /**
     * Exibe a tela com as imagens de peça de um item de venda.
     */
    abrirImagemPecas: function (item) {
      this.abrirJanela(
        600,
        800,
        '../Utils/SelImagemPeca.aspx?tipo=pedido'
          + '&idPedido=' + this.pedido.id
          + '&idProdPed=' + item.id
          + '&pecaAvulsa=' + !item.possuiFilhos
      );
    },

    /**
     * Salva a observação de um produto de pedido.
     * @param {Object} item O produto que será atualizado.
     * @param {Object} event O objeto com o evento JavaScript.
     */
    salvarObservacao: function (item, event) {
      if (!item || !event) {
        return;
      }

      var vm = this;

      Servicos.Pedidos.Produtos.salvarObservacao(this.pedido.id, item.id, item.observacao)
        .then(function (resposta) {
          if (vm.controleTooltipAtual) {
            vm.controleTooltipAtual.fechar();
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        })
    },

    /**
     * Função executada ao definir que o popup de falta de estoque deve ser exibido.
     * @param {number} idProduto O identificador do produto atual.
     * @param {number} altura A altura do popup.
     * @param {number} largura A largura do popup.
     */
    exibirPopupEstoque: function (idProduto, altura, largura) {
      this.abrirJanela(
        altura,
        largura,
        '../Utils/DadosEstoque.aspx'
          * '?idProd=' + idProduto
          + "&idPedido=" + this.pedido.id
      );
    },

    /**
     * Indica que um controle de tooltip está sendo exibido.
     * @param {!Object} tooltip O controle de tooltip que está sendo exibido.
     */
    mostrarTooltip: function (tooltip) {
      if (this.controleTooltipAtual) {
        this.controleTooltipAtual.fechar();
      }

      this.controleTooltipAtual = tooltip;
    },

    /**
     * Indica que o controle de tooltip atual está fechado.
     */
    esconderTooltip: function () {
      this.controleTooltipAtual = null;
    }
  },

  computed: {
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
        atualizar: this.atualizar_
      };
    },

    /**
     * Propriedade computada que retorna o ID do cliente do pedido.
     * @type {number}
     */
    idCliente: function () {
      return this.pedido && this.pedido.cliente
        ? this.pedido.cliente.id
        : null;
    },

    /**
     * Propriedade computada que retorna se o cliente do pedido é revenda.
     * @type {number}
     */
    clienteRevenda: function () {
      return this.pedido && this.pedido.cliente
        ? this.pedido.cliente.revenda
        : null;
    },

    /**
     * Propriedade computada que retorna o tipo de entrega do pedido.
     * @type {number}
     */
    tipoEntrega: function () {
      return this.pedido && this.pedido.entrega && this.pedido.entrega.tipo
        ? this.pedido.entrega.tipo.id
        : null;
    },

    /**
     * Propriedade computada que retorna o percentual de comissão do pedido.
     * @type {number}
     */
    percentualComissao: function () {
      return this.pedido && this.pedido.comissionado && this.pedido.comissionado.comissao
        ? this.pedido.comissionado.comissao.percentual
        : null;
    }
  },

  watch: {
    /**
     * Observador para a variável 'pedido'.
     * Atualiza os dados de validação de produto.
     */
    pedido: {
      handler: function(atual) {
        this.dadosValidacaoProduto = {
          idPedido: atual ? atual.id : null,
          idObra: atual && atual.obra ? atual.obra.id : null,
          idLoja: atual && atual.loja ? atual.loja.id : null,
          tipoPedido: atual && atual.tipo ? atual.tipo.id : null,
          tipoVenda: atual && atual.tipoVenda ? atual.tipoVenda.id : null,
          ambiente: false,
          tipoEntrega: atual && atual.entrega && atual.entrega.tipo ? atual.entrega.tipo.id : null,
          cliente: {
            id: atual && atual.cliente ? atual.cliente.id : null,
            revenda: atual && atual.cliente ? atual.cliente.revenda : null
          }
        };
      },
      deep: true
    }
  },

  template: '#CadPedido-Produtos-template'
});
