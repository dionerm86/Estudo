Vue.component('campo-busca-uf', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * UF selecionada.
     * @type {string}
     */
    uf: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarStringOuVazio
    }
  },

  data: function () {
    return {
      ufAtual: this.uf || '',
      ufs: []
    };
  },

  methods: {
    /**
     * Busca a lista de UFs.
     */
    buscarUfs: function () {
      var vm = this;

      Servicos.Cidades.listarUfs()
        .then(function (resposta) {
          var ufAtual = vm.ufAtual;

          resposta.data.sort(function (a, b) {
            return a.localeCompare(b);
          });

          vm.ufs = resposta.data;

          if (ufAtual) {
            vm.ufAtual = ufAtual;
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }

          vm.ufs = [];
        });
    },

    watch: {
      /**
       * Observador para a variável 'ufAtual'.
       * Limpa as variáveis de ID e nome atual se alterar a UF.
       */
      ufAtual: function (atual) {
        if (atual !== this.uf) {
          this.$emit("update:uf", atual);
        }
      }
    },

    template: '#CampoBuscaUf-template'
  }});
