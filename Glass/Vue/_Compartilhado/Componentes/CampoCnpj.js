Vue.component('campo-cnpj', {
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

  methods: {
    /**
     * Valida o CNPJ atual, alterando a validação do campo.
     */
    validarCnpj: function () {
      var cnpj = (this.cnpjAtual || '').replace(/[^\d]+/g, '');

      const validar = function() {
        if (cnpj.length !== 14) {
          return false;
        }

        const numeros = cnpj.split('')
          .map(x => parseInt(x, 10));

        const todosIguais = numeros
          .every(function (numero) {
            return numero === numeros[0];
          });

        if (todosIguais) {
          return false;
        }

        const multiplicadores = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        const validarDigito = function (posicaoDigito, deslocamentoMultiplicador) {
          const digito = numeros[posicaoDigito];

          const digitoCalculado = numeros.slice(0, posicaoDigito)
            .reduce(function (anterior, numero, i) {
              var multiplicador = multiplicadores[deslocamentoMultiplicador + i];
              return anterior + numero * multiplicador;
            }, 0)
            % 11;

          return (digitoCalculado < 2 && digito === 0)
            || (digitoCalculado >= 2 && digito === (11 - digitoCalculado));
        }

        return validarDigito(12, 1) && validarDigito(13, 0);
      };

      var validacao = validar()
        ? ''
        : 'CNPJ Inválido';

      var campo = this.$refs.campo.$refs.campo.$refs.input;
      campo.setCustomValidity(validacao);
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o CNPJ e que
     * atualiza a propriedade em caso de alteração.
     */
    cnpjAtual: {
      get: function () {
        return this.cnpj;
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
     * Observador para a variável 'cnpjAtual'.
     * Realiza a validação para o CNPJ digitado.
     */
    cnpjAtual: function () {
      this.validarCnpj();
    }
  },

  template: '#CampoCnpj-template'
});
