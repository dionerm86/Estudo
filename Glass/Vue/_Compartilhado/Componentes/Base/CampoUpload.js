Vue.component('campo-upload', {
  props: {
    /**
     * Tipo de arquivos que podem ser buscados no controle.
     * @type {String}
     */
    tipoArquivo: {
      required: false,
      twoWay: false,
      default: '*',
      validator: Mixins.Validacao.validarStringOuVazio
    }
  },

  methods: {
    /**
     * Recupera a imagem passada e transforma-a em uma string base64.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    arquivoSelecionado: function () {
      var vm = this;
      var reader = new FileReader();

      reader.onload = function () {
        var array = new Uint8Array(this.result);
        var base64String = btoa(String.fromCharCode.apply(null, array));

        vm.$emit('arquivo-selecionado', base64String);
      }

      reader.readAsArrayBuffer(this.$refs.upload.files[0]);
    }
  },
  template: '#CampoUpload-template'
});
