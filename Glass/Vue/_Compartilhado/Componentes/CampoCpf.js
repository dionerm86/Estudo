Vue.component('campo-cpf', {
  props: {
    /**
     * CPF buscado pelo controle.
     * @type {?string}
     */
    cpf: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarStringOuVazio
    }
  },

  methods: {
    /**
     * Valida o CPF atual, alterando a validação do campo.
     */
    validarCpf: function () {
      var cpf = (this.cpfAtual || '').replace(/[^\d]+/g, '');

      const validar = function () {
        if (cpf.length !== 11) {
          return false;
        }

        const numeros = cpf.split('')
          .map(x => parseInt(x, 10));

        const todosIguais = numeros
          .every(function (numero) {
            return numero === numeros[0];
          });

        if (todosIguais) {
          return false;
        }

        const multiplicadores = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        const validarDigito = function (posicaoDigito, deslocamentoMultiplicador) {
          const digito = numeros[posicaoDigito];

          const digitoCalculado = numeros.slice(0, posicaoDigito)
            .reduce(function (anterior, numero, i) {
              var multiplicador = multiplicadores[deslocamentoMultiplicador + i];
              return anterior + numero * multiplicador;
            }, 0)
            % 11;

          return digitoCalculado < 2
            ? digito === 0
            : digito === (11 - digitoCalculado);
        }

        return validarDigito(9, 1) && validarDigito(10, 0);
      };

      var validacao = validar()
        ? ''
        : 'CPF Inválido';

      var campo = this.$refs.campo.$refs.campo.$refs.input;
      campo.setCustomValidity(validacao);
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o CPF e que
     * atualiza a propriedade em caso de alteração.
     */
    cpfAtual: {
      get: function () {
        return this.cpf;
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
     * Observador para a variável 'cpfAtual'.
     * Realiza a validação para o CPF digitado.
     */
    cpfAtual: function () {
      this.validarCpf();
    }
  },

  template: '#CampoCpf-template'
});
