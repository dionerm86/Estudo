const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    configuracoes: {}
  },

  methods: {
    /**
     * Busca os veículos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Veiculos.obterLista(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém link para a tela de inserção de veículo.
     */
    obterLinkInserirVeiculo: function () {
      return '../Cadastros/CadVeiculos.aspx';
    },

    /**
     * Obtém link para a tela de edição de veículo.
     * @param {Object} item O veículo que será editado.
     */
    obterLinkEditarVeiculo: function (item) {
      return '../Cadastros/CadVeiculos.aspx?placa=' + item.placa;
    },

    /**
     * Exclui um veículo.
     * @param {Object} veiculo O veículo que será excluído.
     */
    excluir: function (item) {
      if (!this.perguntar('Tem certeza que deseja excluir este veículo?')) {
        return;
      }

      var vm = this;

      Servicos.Veiculos.excluir(item.placa)
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
      this.$refs.lista.atualizar();
    }
  },

  mounted: function() {
    var vm = this;

    Servicos.Veiculos.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
