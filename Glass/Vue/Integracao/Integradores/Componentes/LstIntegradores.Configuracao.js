Vue.component('integradores-configuracao', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Dados da configuração do integrador.
     * @type {Object}
     */
    configuracao: {
      required: true,
      twoWay: false
    }
  },

  data: function () {
    return {
      editando: false
    }
  },

  methods: {

    /**
     * Realiza a edição das configurações.
     **/
    editar: function () {
      this.editando = true;
    },

    /**
     * Cancela a edição da configuração.
     **/
    cancelarEdicao: function () {
      this.editando = false;
    },

    /**
     * Salva os dados da configuração.
     **/
    salvar: function () {
      alert('Ainda não implementado');
    }
  },

  template: '#LstIntegradores-Configuracao-template'
});
