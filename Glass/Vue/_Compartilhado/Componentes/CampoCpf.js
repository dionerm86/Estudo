Vue.component('campo-cpf', {
  mixins: [Mixins.Objetos],

  props: {
    /**
     * Cpf buscado pelo controle.
     * @type {?string}
     */
    cpf: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarStringOuVazio
    }
  },

  data: function () {
    return {
      cpfValido: true,
    }
  },

  methods: {
    /**
     * Valida o CPF passado.
     * @param {string} cpf O CPF que será validado.
     * @return {Promise} Mensagem de erro de validação.
     */
    validarCpf: function (cpf) {
      var mensagem = this.validar(cpf);

      var campo = this.$refs.campo.$refs.campo.$refs.input;
      campo.setCustomValidity(mensagem);
    },

    /**
     * Valida o CPF.
     * @param {string} cpf O CPF que será validado.
     * @return {Promise} Mensagem de erro de validação;
     */
    validar: function (cpf) {
      var mensagem = 'CPF Inválido!';

      var cpfSemPontuacao = cpf.replace('.', '').replace('.', '').replace('-', '');

      if (cpfSemPontuacao.length != 11) {
        return mensagem;
      }

      var multiplicadores = new Array(11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1);
      var numeros = cpfSemPontuacao.split("");

      if (numeros[0] == numeros[1] && numeros[1] == numeros[2] && numeros[2] == numeros[3] && numeros[3] == numeros[4] &&
          numeros[4] == numeros[5] && numeros[5] == numeros[6] && numeros[6] == numeros[7] && numeros[7] == numeros[8] &&
          numeros[8] == numeros[9]) {
        return mensagem;
      }

      var total = 0;

      for (var i = 0; i < 9; i++) {
        total += parseInt(numeros[i]) * multiplicadores[i + 1];
      }

      total %= 11;

      if (total < 2) {
        if (parseInt(numeros[9]) != 0) {
          return mensagem;
        }
      } else if (parseInt(numeros[9]) != (11 - total)) {
        return mensagem;
      }

      total = 0;

      for (var i = 0; i < 10; i++) {
        total += parseInt(numeros[i]) * multiplicadores[i];
      }

      total %= 11;

      if (total < 2) {
        if (parseInt(numeros[10]) != 0) {
          return mensagem;
        }
      } else if (parseInt(numeros[10]) != (11 - total)) {
        return mensagem;
      }

      return '';
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o cpf normalizado e que
     * atualiza a propriedade em caso de alteração.
     */
    cpfAtual: {
      get: function () {
        return this.cpf
      },
      set: function (valor) {
        if (valor !== this.cpf) {
          this.$emit('update:cpf', valor);
        }
      }
    }
  },

  watch: {
    /**
     * Observador para a variável 'enderecoAtual.cep'.
     * Atualiza o texto do controle em caso de alteração.
     */
    cpfAtual: function (cpf) {
      this.validarCpf(cpf);
    }
  },

  template: '#CampoCpf-template'
});
