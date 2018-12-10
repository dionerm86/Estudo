const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca os transportadores para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Transportadores.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém link para a tela de inserção de transportador.
     */
    obterLinkInserirTransportador: function () {
      return '../Cadastros/CadTransportador.aspx';
    },

    /**
     * Obtém link para a tela de edição de transportador.
     * @param {Object} item O transportador que será editado.
     */
    obterLinkEditarTransportador: function (item) {
      return '../Cadastros/CadTransportador.aspx?idTransp=' + item.id;
    },

    /**
     * Exclui um transportador.
     * @param {Object} transportador O transportador que será excluído.
     */
    excluir: function (transportador) {
      if (!this.perguntar('Tem certeza que deseja excluir este transportador?')) {
        return;
      }

      var vm = this;

      Servicos.Transportadores.excluir(transportador.id)
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
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  },

  mounted: function() {
    var vm = this;

    Servicos.Transportadores.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
