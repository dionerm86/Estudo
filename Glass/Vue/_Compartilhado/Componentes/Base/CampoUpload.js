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
     * Recupera o arquivo selecionado e transforma-o em uma string base64.
     * Depois, dispara o evento do controle informando o arquivo convertido.
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
