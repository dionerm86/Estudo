Vue.component('lista-selecao-uf', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Uf selecionadas no controle.
     * @type {string[]}
     */
    ufSelecionada: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarStringOuVazio
    }
  },

  data: function () {
    return {
      listaUfs: null,
    }
  },

  methods: {

    /**
     * Busca as UFs e as transforma para uso no controle de lista de seleção múltipla.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    buscarUfs: function () {
      var vm = this;

      Servicos.Cidades.listarUfs()
        .then(function (resposta) {
          var ufAtual = vm.ufAtual;

          resposta.data.sort(function (a, b) {
            return a.localeCompare(b);
          });

          vm.listaUfs = resposta.data;

          if (ufAtual) {
            vm.ufAtual = ufAtual;
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }

          vm.listaUfs = [];
        });
    }
  },

  computed: {
    /**
     * Propriedade computada que formata as UFs selecionadas para uso no controle de
     * seleção múltipla e que atualiza a propriedade 'ufs' ao ser alterada.
     * @type {string}
     */
    ufAtual: {

      get: function () {
        return this.ufSelecionada;
      },
      set: function (valor) {
        if (valor !== this.ufSelecionada) {
          this.$emit('update:ufSelecionada', valor);
        }
      }
    }
  },

  mounted: function () {
    this.buscarUfs();
  },

  template: '#ListaSelecaoUf-template'
});
