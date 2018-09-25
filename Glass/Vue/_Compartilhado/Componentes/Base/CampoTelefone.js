var Telefones = Telefones || {};
Telefones.dddsNonoDigito = [
  11, 12, 13, 14, 15, 16, 17, 18, 19,
  21, 22, 24, 27, 28,
  31, 32, 33, 34, 35, 37, 38,
  41, 42, 43, 44, 45, 46, 47, 48, 49,
  51, 53, 54, 55,
  61, 62, 63, 64, 65, 66, 67, 68, 69,
  71, 73, 74, 75, 77, 79,
  81, 82, 83, 84, 85, 86, 87, 88, 89,
  91, 92, 93, 94, 95, 96, 97, 98, 99,
];

Vue.component('campo-telefone', {
  props: {
    /**
     * O telefone que será exibido no controle.
     * @type {String}
     */
    telefone: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarStringOuVazio
    }
  },

  methods: {
    /**
     * Obtém a máscara que será usada para o campo, com base no DDD informado.
     * @param {string} valor O valor atual do campo de telefone.
     * @returns {String[]|Regex[]} Um array com os campos usados para a máscara e validação da entrada.
     */
    obterMascara: function(valor) {
      var mascara = [
        '(', /[1-9]/, /\d/, ')',
        ' ', /[1-9]/, /\d/, /\d/, /\d/,
        '-',
        /\d/, /\d/, /\d/, /\d/
      ];

      if (valor) {
        var ddd = parseInt(valor.match(/\d+/), 10);
        if (Telefones.dddsNonoDigito.indexOf(ddd) > -1) {
          mascara.splice(5, 0, /[1-9]/);
        }
      }

      return mascara;
    }
  },

  computed: {
    /**
     * Propriedade computada que normaliza o valor do telefone e que
     * atualiza a propriedade se alterada.
     */
    telefoneAtual: {
      get: function () {
        return this.telefone || '';
      },
      set: function (valor) {
        if (valor !== this.telefone) {
          this.$emit('update:telefone', valor);
        }
      }
    }
  },

  template: '#CampoTelefone-template'
});
