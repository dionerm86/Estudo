const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'asc')],

  data: {
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca as ferragens para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Produtos.ChapasVidro.Perdas.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Cancela uma perda de chapa de vidro.
     * @param {!number} idPerdaChapaVidro O identificador da perda de chapa de vidro que será cancelada.
     */
    cancelar: function (idPerdaChapaVidro) {
      if (!confirm("Tem certeza que deseja cancelar esta perda?")) {
        return;
      }

      var vm = this;

      return Servicos.Produtos.ChapasVidro.Perdas.cancelar(idPerdaChapaVidro)
        .then(function (resposta) {
          vm.exibirMensagem(resposta.data.mensagem);
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
  }
});
