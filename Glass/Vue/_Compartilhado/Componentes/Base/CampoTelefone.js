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
     * @returns {String[] | Regex[]} Um array com os campos usados para a máscara e validação da entrada.
     */
    obterMascara: function(valor) {
      var mascara = [
        '(', /[1-9]/, /[1-9]/, ')',
        ' ', /[1-9]/, /\d/, /\d/, /\d/,
        '-',
        /\d/, /\d/, /\d/, /\d/, /\d/
      ];
      
      var texto = (valor || '').replace(/[^\d]/g, '');

      if (texto.length === 11) {
        mascara.splice(5, 0, /[1-9]/);
        mascara.splice(mascara.length - 1, 1);
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
