Vue.component('integradores-integrador', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Dados do integrador associado.
     * @type {Object}
     */
    integrador: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto,
    }
  },

  data: function () {
    return {
      podeExibirConfiguracao: false,
      podeExibirOperacoes: false,
      podeExibirOpcoes: false,
      podeExibirJobs: false,
      podeExibirLogger: false,
      podeExibirHistorico: false,
    };
  },

  methods: {

    /**
     * Altera a exibição da configuração.
     **/
    alterarExibicaoConfiguracao: function () {
      this.podeExibirConfiguracao = !this.podeExibirConfiguracao;
    },

    /**
     * Altera a exibição das operações.
     **/
    alterarExibicaoOperacoes: function () {
      this.podeExibirOperacoes = !this.podeExibirOperacoes;
    },

    /**
     * Altera a exibição das opções para o integrador.
     **/
    alterarExibicaoOpcoes: function () {
      this.podeExibirOpcoes = !this.podeExibirOpcoes;
    },

    /**
     * Altera a exibição dos jobs.
     **/
    alterarExibicaoJobs: function () {
      this.podeExibirJobs = !this.podeExibirJobs;
    },

    /**
     * Altera a exibição do logger.
     **/
    alterarExibicaoLogger: function () {
      this.podeExibirLogger = !this.podeExibirLogger;
    },

    /**
     * Altera a exibição do histórico.
     **/
    alterarExibicaoHistorico: function () {
      this.podeExibirHistorico = !this.podeExibirHistorico;
    }
  },

  template: '#LstIntegradores-Integrador-template'
});
