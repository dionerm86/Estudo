const app = new Vue({
    el: '#app',
    mixins: [Mixins.Objetos, Mixins.ExecutarTimeout],

    data: {
        inserindo: false,
        editando: false,
        descricao: {},
        situacoes: {},
        dias: {},
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
        }
    },

        /**
     * Função que indica se o formulário de pedido possui valores válidos de acordo com os controles.
     * @param {Object} botao O botão que foi disparado no controle.
     * @returns {boolean} Um valor que indica se o formulário está válido.
     */
    validarFormulario_: function (botao) {
        var form = botao.form || botao;
        while (form.tagName.toLowerCase() !== 'form') {
            form = form.parentNode;
        }

        if (!form.checkValidity() || !this.validarBase_()) {
            return false;
        }

        return true;
    },

    /**
     * Insere a parcela, se possível.
     * @param {Object} event O objeto do evento JavaScript.
     */
    inserirParcela: function (event) {
        var vm = this;

        Servicos.Parcelas.inserir(this.parcela)
          .then(function (resposta) {
              var url = '../Cadastros/CadPedido.aspx?idPedido=' + resposta.data.id;
              var byVend = GetQueryString('byVend');

              if (byVend === '1') {
                  url += '&ByVend=1';
              }

              window.location.assign(url);
          })
          .catch(function (erro) {
              if (erro && erro.mensagem) {
                  vm.exibirMensagem('Erro', erro.mensagem);
              }
          });
    },

    mounted: function () {

        var idParcela = GetQueryString('idParcela');
        var vm = this;

        if (!idParcela) {
            vm.inserindo = true;
        }

        if (idParcela) {
            vm.editando = true;
        }
    }    
});
