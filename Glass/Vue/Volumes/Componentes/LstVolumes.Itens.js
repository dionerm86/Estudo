Vue.component('volumes-itens', {
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('id', 'desc')],
  props: {
    /**
     * Filtros selecionados para a lista de volumes.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de volumes.
     * @type {!Object}
     */
    configuracoes: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      totalizadores: {}
    }
  },

  methods: {
    /**
     * Função que busca busca os volumes associados aos pedidos.
     * @returns {Promise} Uma Promise com a busca dos itens, de acordo com o filtro.
     */
    buscarVolumes: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Volumes.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Abre tela para edição do volume.
     * @param {Object} volume O volume que será alterado.
     */
    abrirTelaEdicaoVolume: function (volume) {
      if (!volume) {
        throw new Error('Volume é obrigatório.');
      }

      if (!volume.idPedido) {
        throw new Error('Pedido não encontrado.');
      }

      this.abrirJanela(600, 800, '../Cadastros/CadItensVolume.aspx?popup=true&idPedido=' + volume.idPedido + '&idVolume=' + volume.id);
    },

    /**
     * Função que calcula os totais da lista de volumes
     */
    calcularTotais: function () {
      var itens = this.$refs.lista.itens;

      this.totalizadores = {
        quantidadePecasTotal: 0,
        pesoTotal: 0,
        metroQuadradoTotal: 0
      };

      for (var i in itens) {
        this.totalizadores.quantidadePecasTotal += itens[i].quantidadePecas;
        this.totalizadores.pesoTotal += itens[i].peso;
        this.totalizadores.metroQuadradoTotal += itens[i].metroQuadrado;
      }
    },

    /**
     * Exibe um relatório para imprimir a etiqueta de volume.
     * @param {Object} volume O volume que terá a etiqueta impressa.
     */
    abrirImpressaoEtiquetaVolume: function (volume) {
      if (!volume) {
        throw new Error('Volume é obrigatório.');
      }

      var url = '../Relatorios/RelEtiquetaVolume.aspx?rel=EtqVolume&idVolume=' + volume.id;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Exclui um volume.
     * @param {Object} volume O volume a ser excluído.
     */
    excluir: function (volume) {
      if (!this.perguntar('Confirmação', 'Deseja realmente excluir este volume?')) {
        return;
      }

      var vm = this;

      Servicos.Volumes.excluir(volume.id)
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
     * Indica externamente que os itens em exibição foram atualizados.
     */
    atualizouItens: function (numeroItens) {
      this.$emit('atualizou-itens', numeroItens);
      this.calcularTotais();
    },

    /**
     * Força a atualização da lista de volumes, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },
  },

  template: '#LstVolumes-Itens-template'
});
