const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('c.IdCompra', 'desc')],

  data: {
    filtro: {}
  },

  methods: {
    /**
     * Busca as compras de mercadorias para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Compras.Mercadorias.obterListaComprasMercadorias(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém o link para inserção de compras de mercadorias.
     * @returns {string} Uma string com o link para a inserção de compras de mercadoria.
     */
    obterLinkInserirCompraMercadoria: function () {
      return '../Cadastros/CadCompraPcp.aspx';
    },

    /**
     * Exibe os dados detalhados da compra de mercadoria em um relatório.
     * @param {Object} item A compra de mercadoria que será exibida.
     */
    abrirRelatorioComprasMercadorias: function (item) {
      var url = '../Relatorios/RelBase.aspx?rel=Compra&idCompra=' + item.id;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Exibe um prompt para inserir o motivo do cancelamento de uma compra de mercadoria.
     * @param {Object} item A compra de mercadoria que será cancelada.
     */
    cancelar: function (item) {
      var motivo = this.requisitarInformacao('Informe o motivo do cancelamento.');
      if (!motivo) {
        return;
      }

      var vm = this;

      return Servicos.Compras.Mercadorias.cancelar(item.id)
        .then(function (resposta) {
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem)
          }
        });
    },

    /**
     * Abre um popup com o gerenciamento de fotos para a compra de mercadoria.
     * @param {Object} item A compra de mercadoria que terá as fotos gerenciadas.
     */
    abrirGerenciamentoDeFotos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=compra');
    },

    /**
     * Obtém o link para edição de compras de mercadorias.
     * @param {Object} A compra de mercadoria que será editada.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterLinkEditarCompraMercadoria: function (item) {
      return this.obterLinkInserirCompraMercadoria() + '?idCompra=' + item.id;
    },

    /**
     * Abre uma tela informando se o produto chegou.
     * @param {Object} item O identificador da compra de mercadoria.
     */
    obterProdutoChegou: function (item) {
      this.abrirJanela(600, 800, '../Utils/ProdutoCompraChegou.aspx?idCompra=' + item.id);
    },

    /**
     * Obtém o link para gerar a nota fiscal da compra de mercadoria.
     * @param {Object} item A compra de marcadoria que será gerada a nota fiscal.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    gerarNotaFiscal: function (item) {
      if (!this.perguntar('Deseja gerar a nota fiscal para essa compra ' + item.id + '?')) {
        return;
      }

      var vm = this;

      return Servicos.Compras.Mercadorias.gerarNotaFiscal(item.id)
        .then(function (resposta) {
          vm.exibirMensagem(resposta.data.mensagem);
          var idNf = resposta.data.idNf;
          redirectUrl('../Cadastros/CadNotaFiscal.aspx?idNf=' + idNf + '&tipo=3');
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Reabre uma compra de mercadoria já finalizada.
     * @param {Object} item A compra de mercadoria que será reaberta.
     */
    reabrir: function (item) {
      if (!this.perguntar('Deseja reabrir a compra de mercadoria de número ' + item.id + '?')) {
        return;
      }

      var vm = this;

      return Servicos.Compras.Mercadorias.reabrir(item.id)
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
     * Atualiza a lista de compras de mercadorias.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },
  }
});
