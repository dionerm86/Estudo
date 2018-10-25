Vue.component('campo-cnpj', {
  mixins: [Mixins.Objetos],

  props: {
    /**
     * CNPJ buscado pelo controle.
     * @type {?string}
     */
    cnpj: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarStringOuVazio
    }
  },

  data: function () {
    return {
      cnpjValido: true,
    }
  },

  methods: {
    /**
     * Valida o CNPJ passado.
     * @param {string} cnpj O CNPJ que será validado.
     * @return {Promise} Mensagem de erro de validação.
     */
    validarCnpj: function (cnpj) {
      var mensagem = this.validar(cnpj);

      var campo = this.$refs.campo.$refs.campo.$refs.input;
      campo.setCustomValidity(mensagem);
    },

    /**
     * Valida o CNPJ.
     * @param {string} cnpj O CNPJ que será validado.
     * @return {Promise} Mensagem de erro de validação;
     */
    validar: function (cnpj) {
      var mensagem = 'CNPJ Inválido!';

      var cnpjSemPontuacao = cnpj.replace('.', '').replace('.', '').replace('.', '').replace('-', '').replace('/', '');

      if (cnpjSemPontuacao.length != 14) {
        return mensagem;
      }

      var multiplicadores = new Array(6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2);
      var numeros = cnpjSemPontuacao.split("");

      var total = 0;
      for (var i = 0; i < 12; i++) {
        total += parseInt(numeros[i]) * multiplicadores[i + 1];
      }

      total %= 11;

      if (total < 2) {
        if (parseInt(numeros[12]) != 0) {
          return mensagem;
        }
      } else if (parseInt(numeros[12]) != (11 - total)) {
        return mensagem;
      }

      total = 0;
      for (var j = 0; j < 13; j++) {
        total += parseInt(numeros[j]) * multiplicadores[j];
      }

      total %= 11;

      if (total < 2) {
        if (parseInt(numeros[13]) != 0) {
          return mensagem;
        }
      } else if (parseInt(numeros[13]) != (11 - total)) {
        return mensagem;
      }

      return '';
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o cnpj normalizado e que
     * atualiza a propriedade em caso de alteração.
     */
    cnpjAtual: {
      get: function () {
        return this.cnpj
      },
      set: function (valor) {
        if (valor !== this.cnpj) {
          this.$emit('update:cnpj', valor);
        }
      }
    }
  },

  watch: {
    /**
     * Observador para a variável cnpjAtual.
     * Atualiza o texto do controle em caso de alteração.
     */
    cnpjAtual: function (cnpj) {
      this.validarCnpj(cnpj);
    }
  },

  template: '#CampoCnpj-template'
});
