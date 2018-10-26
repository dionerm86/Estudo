Vue.component('integradores-jobintegracao', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Dados do job de integração associada.
     * @type {Object}
     */
    job: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Integrador pai da operação.
     * @type {Object}
     **/
    integrador: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      podeExibirDetalhes: false,
      executando: false
    }
  },

  methods: {

    /**
     * Altera a exibição dos detalhes.
     **/
    alterarExibicaoDetalhes: function () {
      this.podeExibirDetalhes = !this.podeExibirDetalhes;
    },

    /**
     * Executa o job no servidor.
     **/
    executar: function () {

      var self = this;

      this.executando = true;
      Servicos.Integracao.Integradores.executarJob(this.integrador.nome, this.job.nome)
        .then(function (resultado) {
          self.executando = false;
          self.exibirMensagem('Sucesso', 'Job executado.');
        })
        .catch(function (erro) {
          self.executando = false;
          if (erro && erro.mensagem) {
            self.exibirMensagem('Erro', erro.mensagem);
          }
        });
    }
  },

  computed: {
    /**
     * Obtém a descrição situação do job.
     * @returns {String} Texto da situação.
     **/
    situacao: function () {

      switch (this.job.situacao) {
        case 'NaoIniciado':
          return 'Não iniciado';
        default:
          return this.job.situacao;
      }
    },

    /**
     * Obtém um valor que indica se o job possui falha.
     * @returns {Boolean} True caso o job esteja com falha.
     **/
    possuiFalha: function () {
      return this.job.situacao != 'Executado';
    }
  },

  template: '#LstIntegradores-JobIntegracao-template'
});
