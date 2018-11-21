const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    configuracoes: {},
    filtro: {},
    volumesEmExibicao: []
  },

  methods: {
    /**
     * Busca os pedidos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Pedidos.obterListaVolumes(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Abre tela para geração de volume.
     * @param {Object} item O pedido que irá gerar o volume.
     */
    abrirTelaGerarVolume: function (item) {
      this.abrirJanela(600, 800, '../Cadastros/CadItensVolume.aspx?popup=true&idPedido=' + item.idPedido + '&idVolume=' + item.id);
    },

    /**
     * Abre tela para geração de volume.
     * @param {Object} item O pedido que irá gerar o volume.
     */
    abrirTelaVisualizarVolume: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelEtiquetaVolume.aspx?rel=EtqVolume&idVolume=' + item.id);
    },

    /**
     * Exclui um carregamento.
     * @param {Object} carregamento O carregamento que será excluído.
     */
    excluir: function (carregamento) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este carregamento?')) {
        return;
      }

      var vm = this;

      Servicos.Carregamentos.excluir(carregamento.id)
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
     * Indica se os volumes estão sendo exibidas na lista.
     * @param {!number} indice O número da linha que está sendo verificada.
     * @returns {boolean} Verdadeiro, se os itens estiverem sendo exibidos.
     */
    exibindoVolumes: function (indice) {
      return this.volumesEmExibicao.indexOf(indice) > -1;
    },

    /**
     * Alterna a exibição dos volumes.
     */
    alternarExibicaoVolumes: function (indice) {
      var i = this.volumesEmExibicao.indexOf(indice);

      if (i > -1) {
        this.volumesEmExibicao.splice(i, 1);
      } else {
        this.volumesEmExibicao.push(indice);
      }
    },

    /**
     * Retorna o número de colunas da lista paginada.
     * @type {number}
     */
    numeroColunasLista: function () {
      return this.$refs.lista.numeroColunas();
    },

    /**
     * Atualiza a lista de pedidos.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    },

    /**
     * Limpa a lista de pedidos em exibição ao realizar paginação da lista principal.
     */
    atualizouItens: function () {
      this.volumesEmExibicao.splice(0, this.volumesEmExibicao.length);
    }
  },

  mounted: function() {
    var vm = this;

    Servicos.Pedidos.obterConfiguracoesListaVolumes()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
