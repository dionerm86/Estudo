Vue.component('ordens-carga-pedidos', {
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('idPedido', 'desc')],
  props: {
    /**
     * Filtros selecionados para a lista de pedidos associados a ordem de carga.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de ordens de carga.
     * @type {!Object}
     */
    configuracoes: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  methods: {
    /**
     * Função que busca busca os pedidos da ordem de carga.
     * @returns {Promise} Uma Promise com a busca dos itens, de acordo com o filtro.
     */
    buscarPedidos: function () {
      return Servicos.Carregamentos.OrdensCarga.obterListaPedidosPorOrdemCarga(this.filtro.idOrdemCarga);
    },    

    /**
     * Desassocia um pedido de uma ordem de carga.
     * @param {Object} pedido O objeto do pedido a ser desassociado.
     */
    desassociarPedido: function (pedido) {
      if (!this.perguntar('Confirmação', 'Deseja realmente remover este pedido da OC?')) {
        return;
      }

      var vm = this;
      var idOrdemCarga = this.filtro.item.id;

      Servicos.Carregamentos.OrdensCarga.desassociarPedidoOrdemCarga(idOrdemCarga, pedido.id)
        .then(function (resposta) {
          this.exibirMensagem(resposta.data.mensagem)
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Força a atualização da lista de pedidos, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },
  },

  template: '#LstOrdensCarga-Pedidos-template'
});
