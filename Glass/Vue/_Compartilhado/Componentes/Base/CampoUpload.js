Vue.component('campo-upload', {
  props: {
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
        var arrayBuffer = this.result;
        var array = new Uint8Array(arrayBuffer);

        var base64String = btoa(String.fromCharCode.apply(null, new Uint8Array(arrayBuffer)));

        vm.$emit('arquivo-selecionado', base64String);
      }
        reader.readAsArrayBuffer(this.$refs.upload.files[0]);
    }
  },
  template:'#CampoUpload-template'
});
