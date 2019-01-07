const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('IDPROPVEIC', 'asc')],

  methods: {
    /**
     * Busca a lista de proprietários de veículo para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.ConhecimentosTransporte.Veiculos.Proprietarios.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém o link para inserção de um novo proprietário de veículo.
     */
    obterLinkInserirProprietarioVeiculo: function () {
      return '../Cadastros/CadProprietarioVeiculo.aspx';
    },

    /**
     * Obtém o link para edição de um proprietário de veículo existente.
     * @param {number} id O identificador do proprietário de veículo que será editado.
     */
    obterLinkEditarProprietarioVeiculo: function (id) {
      return this.obterLinkInserirProprietarioVeiculo() + '?idPropVeiculo=' + id;
    },

    /**
     * Exclui um proprietário de veículo.
     * @param {Object} proprietarioVeiculo O proprietario de veiculo que será excluído.
     */
    excluir: function (proprietarioVeiculo) {
      if (!confirm("Tem certeza que deseja excluir este proprietário?")) {
        return;
      }

      var vm = this;

      Servicos.ConhecimentosTransporte.Veiculos.Proprietarios.excluir(proprietarioVeiculo.id)
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
