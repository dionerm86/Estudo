Vue.component('integradores-logger', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Dados do integrador associado.
     * @type {Object}
     **/
    integrador: {
      required: true,
      twoWay: false
    }
  },

  data: function () {
    return {
      atualizando: false,
      logger: {
        itens: []
      },
    }
  },

  methods: {
    /**
     * Atualiza os dados do logger.
     **/
    atualizar: function () {
      var self = this;
      this.atualizando = true;

      Servicos.Integracao.Integradores.obterLogger(this.integrador.nome)
        .then(function (resposta) {
          self.atualizando = false;

          if (resposta.status == 200) {
            self.logger = resposta.data;
          }
        })
        .catch(function (erro) {
          self.executando = false;
          if (erro && erro.mensagem) {
            self.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },
  },

  mounted: function () {
    this.atualizar();
  },

  template: '#LstIntegradores-Logger-template'
});
