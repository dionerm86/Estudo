const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.ExecutarTimeout],

  data: {
    inserindo: false,
    editando: false,
    situacaoAtual: {},
    formaPagtoAtual: {},
    dias: {},
    parcela: {},
    parcelaOriginal: {},
    tipoPagto: {}
  },

  methods: {

      /**
        * Retorna os itens para o filtro de situação.
        * @returns {Promise} Uma Promise com o resultado da busca.
        */
  obterSituacoes: function () {
    return Servicos.Parcelas.situacoes();
  },

      /**
            * Retorna os itens para o filtro de situação.
            * @returns {Promise} Uma Promise com o resultado da busca.
            */
      obterFormaPagto: function () {
          return Servicos.Parcelas.formaPagto();
      },

      iniciarCadastroOuAtualizacao_: function (item) {
          debugger;
          this.parcelaOriginal = item ? this.clonar(item) : {};
          this.situacaoAtual = item ? this.clonar(item.situacao) : null;

          this.formaPagtoAtual = {
              id: item && item.tipoPagto ? item.tipoPagto.id : null,
              nome: item && item.tipoPagto ? item.tipoPagto.nome : null
          };

          this.parcela = {
              id: item && item.id ? item.id : null,
              descricao: item ? item.descricao : null,
              parcelaPadrao: item ? item.parcelaPadrao : null,
              situacao: item ? item.situacao : null,
            situacao: item ? item.situacao : null
          }
      },

      /**
       * Insere a parcela, se possível.
       * @param {Object} event O objeto do evento JavaScript.
       */
      inserirParcela: function () {
          var vm = this;

          Servicos.Parcelas.inserir(this.parcela)
            .then(function (resposta) {
                var url = '../Listas/LstParcelas.aspx';

                window.location.assign(url);
            })
            .catch(function (erro) {
                if (erro && erro.mensagem) {
                    vm.exibirMensagem('Erro', erro.mensagem);
                }
            });
      },

      /**
        * Insere a parcela, se possível.
        * @param {Object} event O objeto do evento JavaScript.
        */
      atualizarParcela: function () {
          var vm = this;
          debugger;
          Servicos.Parcelas.atualizar(vm.parcela.id, vm.parcela)
            .then(function (resposta) {
                var url = '../Listas/LstParcelas.aspx';

                window.location.assign(url);
            })
            .catch(function (erro) {
                if (erro && erro.mensagem) {
                    vm.exibirMensagem('Erro', erro.mensagem);
                }
            });
      },

      /**
        * cancela a edição ou inserção da parcela.
        * @param {Object} event O objeto do evento JavaScript.
        */
      cancelar: function () {
          var url = '../Listas/LstParcelas.aspx';
          window.location.assign(url);
      },

      /**
        * Busca os dados de uma parcela.
        * @param {?number} id O id da parcela.
        * @returns {Promise} Uma promise com o resultado da busca.
        */
      buscarParcela: function (id) {
          var vm = this;

          return Servicos.Parcelas.obterParcela(id)
            .then(function (resposta) {
                vm.iniciarCadastroOuAtualizacao_(resposta.data);
            })
            .catch(function (erro) {
                if (erro && erro.mensagem) {
                    vm.exibirMensagem('Erro', erro.mensagem);
                }
            });
      },
  },

  watch: {
    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacaoAtual: {
      handler: function (atual) {
        if (this.parcela) {
          this.parcela.situacao = atual ? atual.id : null;
        }
      },
      deep: true
    },

    formaPagamentoAtual: {
      handler: function (atual) {
        if (this.parcela) {
          this.parcela.formaPagto = atual ? atual.id : null;
        }
      },

      deep: true
    }
  },

  mounted: function () {
      debugger;
      var idParcela = GetQueryString('idParcela');
      var vm = this;

      if (!idParcela) {
          vm.inserindo = true;
          vm.iniciarCadastroOuAtualizacao_();
      }

      if (idParcela) {
          vm.editando = true;
          this.buscarParcela(idParcela)
            .then(function () {
                if (!vm.parcela) {
                    var url = '../Listas/LstParcelas.aspx';

                    window.location.assign(url);
                }
            });
      }
  }
});
