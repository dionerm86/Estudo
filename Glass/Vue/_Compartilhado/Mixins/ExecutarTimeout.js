var Mixins = Mixins || {};

Mixins.ExecutarTimeout = {
  data: function () {
    return {
      timeouts__: {}
    }
  },

  methods: {
    /**
     * Executa uma função após um tempo de timeout, garantindo que múltiplas execuções
     * não ocorram dentro desse tempo determinado.
     * @param {!string} nome O nome da execução.
     * @param {!function} executar A função que será executada.
     * @param {?number} [tempoEspera=50] O intervalo de tempo, em ms, para espera entre as chamadas.
     */
    executarTimeout: function (nome, executar, tempoEspera) {
      if (!nome) {
        throw new Error('Nome é obrigatório.');
      }

      if (!executar) {
        throw new Error('Função a ser executada é obrigatória.');
      }

      if (tempoEspera === null || tempoEspera === undefined) {
        tempoEspera = 50;
      }

      if (this.timeouts__[nome]) {
        clearTimeout(this.timeouts__[nome]);
      }

      var vm = this;

      this.timeouts__[nome] = setTimeout(
        function () {
          executar.call(vm);
          vm.timeouts__[nome] = null;
        },
        tempoEspera
      );
    }
  },

  computed: {
    /**
     * Propriedade computada que indica se alguma pesquisa por timeout está ocorrendo.
     * @type {!boolean}
     */
    executandoBuscaTimeout: function () {
      if (!this.timeouts__) {
        return false;
      }

      for (var timeout in this.timeouts__) {
        if (this.timeouts__[timeout]) {
          return true;
        }
      }

      return false;
    }
  }
};
