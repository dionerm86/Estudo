Vue.component('integradores-operacaointegracao', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Dados da operação de integração associada.
     * @type {Object}
     */
    operacao: {
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
      parametros: [],
      testando: false,
      executando: false,
      podeExibirParametros: false
    }
  },

  methods: {
    /**
     * Carrega os parametros da operação
     */
    carregarParametros: function () {

      var parametros1 = [];
      this.operacao.parametros.forEach(function (parametro) {
        parametros1.push({
          definicao: parametro,
          valor: parametro.valorPadrao
        });
      });

      this.parametros = parametros1;
    },

    /**
     * Altera a exibição dos parâmetros.
     **/
    alterarExibicaoParametros: function () {
      this.podeExibirParametros = !this.podeExibirParametros;
    },

    /**
     * Ativa a configuração de teste da operação.
     **/
    testar: function () {
      this.testando = true;
    },

    /**
     * Cancela o teste da operação.
     **/
    cancelarTeste: function () {
      this.testando = false;
    },

    /**
     * Executa a operação no servidor.
     **/
    executar: function () {

      var parametros2 = [];

      this.parametros.forEach(function (parametro) {
        parametros2.push(parametro.valor);
      });

      var self = this;

      this.executando = true;
      Servicos.Integracao.Integradores.executarOperacao(this.integrador.nome, this.operacao.nome, parametros2)
        .then(function (resposta) {
          self.executando = false;

          switch (resposta.status) {
            case 200:
              self.exibirMensagem('Sucesso', 'Operação executada.');
              break;
            case 400:
              self.exibirMensagem('Sucesso', 'Perminssão negada para a execução da operação.');
              break;
          }
        })
        .catch(function (erro) {
          self.executando = false;
          if (erro && erro.mensagem) {
            self.exibirMensagem('Erro', erro.mensagem);
          }
        });
    }
  },

  mounted: function() {
    this.carregarParametros();
  },

  template: '#LstIntegradores-OperacaoIntegracao-template'
});
