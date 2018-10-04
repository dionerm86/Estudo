const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos],

  data: {
    dadosOrdenacao_: {
      campo: 'id',
      direcao: 'desc'
    },
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
     * Realiza a ordenação da lista de veículos.
     * @param {string} campo O nome do campo pelo qual o resultado será ordenado.
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

  computed: {
    /**
     * Propriedade computada que indica a ordenação para a lista.
     * @type {string}
     */
    ordenacao: function() {
      var direcao = this.dadosOrdenacao_.direcao ? ' ' + this.dadosOrdenacao_.direcao : '';
      return this.dadosOrdenacao_.campo + direcao;
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
