const app = new Vue({
    el: '#app',
    mixins: [Mixins.Objetos, Mixins.ExecutarTimeout],

    data: {
        inserindo: false,
        editando: false,
        situacoes: {},
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

        /**
         * Insere a parcela, se possível.
         * @param {Object} event O objeto do evento JavaScript.
         */
        inserirParcela: function () {
            debugger;
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

        iniciarCadastroOuAtualizacao_: function (item) {
            debugger;
            this.parcelaOriginal = item ? this.clonar(item) : {};

            this.tipoPagto = {
                id: item && item.tipoPagto ? item.tipoPagto.id : null,
                nome: item && item.tipoPagto ? item.tipoPagto.nome : null
            };

            this.situacoes = {
                id: item && item.situacoes ? item.situacoes.id : null,
                nome: item && item.situacoes ? item.situacoes.nome : null
            };

            this.parcela = {
                id: item && item.id ? item.id : null,
                descricao: item ? item.descricao : null,
                parcelaPadrao: item ? item.parcelaPadrao : null
            }
        },

        /**
         * Busca os dados de uma parcela.
         * @param {?number} id O número do pedido.
         * @returns {Promise} Uma promise com o resultado da busca.
         */
        buscarParcela: function (id) {
            debugger;
            var vm = this;

            return Servicos.Parcelas.obterParcela(id)
              .then(function (resposta) {
                  vm.parcela = resposta.data;
              })
              .catch(function (erro) {
                  if (erro && erro.mensagem) {
                      vm.exibirMensagem('Erro', erro.mensagem);
                  }
              });
        },
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
