const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('IDPROPVEIC', 'asc')],

  methods: {
    /**
     * Busca a lista de associações de proprietários com veículos para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.ConhecimentosTransporte.Veiculos.Proprietarios.Associacoes.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém o link para inserção de uma nova associação entre proprietário e veículo.
     */
    obterLinkInserirAssociacaoProprietarioVeiculo: function () {
      return '../Cadastros/CadAssociarPropVeic.aspx';
    },

    /**
     * Obtém o link para edição de uma associação entre proprietário e veículo existente.
     * @param {number} id O identificador do proprietário de veículo.
     * @param {string} placa A placa do veículo.
     */
    obterLinkEditarAssociacaoProprietarioVeiculo: function (associacaoProprietarioVeiculo) {
      return this.obterLinkInserirAssociacaoProprietarioVeiculo() + '?idPropVeiculo=' +
        associacaoProprietarioVeiculo.proprietario.id + "&placa=" + associacaoProprietarioVeiculo.placaVeiculo;
    },

    /**
     * Exclui uma associação entre proprietário e veículo.
     * @param {Object} associacaoProprietarioVeiculo A associção entre proprietario e veiculo que será excluída.
     */
    excluir: function (associacaoProprietarioVeiculo) {
      if (!confirm("Tem certeza que deseja desfazer associação?")) {
        return;
      }

      var vm = this;

      Servicos.ConhecimentosTransporte.Veiculos.Proprietarios.Associacoes.excluir(associacaoProprietarioVeiculo.proprietario.id, associacaoProprietarioVeiculo.placaVeiculo)
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
     * Força a atualização da lista de processos, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  }
});
