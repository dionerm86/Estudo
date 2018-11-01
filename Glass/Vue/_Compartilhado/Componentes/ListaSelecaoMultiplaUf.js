Vue.component('lista-selecao-multipla-uf', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * UFs selecionadas no controle.
     * @type {string[]}
     */
    ufs: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarArrayOuVazio
    }
  },

  data: function() {
    return {
      listaUfs: null
    }
  },

  methods: {
    /**
     * Busca as UFs e as transforma para uso no controle de lista de seleção múltipla.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    buscarUfs: function () {
      var vm = this;

      return Servicos.Cidades.listarUfs()
        .then(function (resposta) {
          vm.listaUfs = resposta.data
            .sort(function (a, b) {
              return a.localeCompare(b);
            })
            .map(function (uf, index) {
              return {
                id: index,
                nome: uf
              };
            });

          return {
            data: vm.listaUfs
          };
        });
    }
  },

  computed: {
    /**
     * Propriedade computada que formata as UFs selecionadas para uso no controle de
     * seleção múltipla e que atualiza a propriedade 'ufs' ao ser alterada.
     * @type {string}
     */
    ufsAtuais: {
      get: function () {
        return this.ufs
          ? (this.listaUfs || this.ufs)
              .filter(function (uf) {
                return this.ufs.indexOf(uf.nome) > -1;
              }, this)
              .map(function (uf) {
                return uf.id;
              })
          : null;
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.ufsAtuais)) {
          const valorAtualizar = valor
            ? this.listaUfs
              .filter(function (uf) {
                return valor.indexOf(uf.id) > -1;
              })
              .map(function (uf) {
                return uf.nome;
              })
            : null;

          this.$emit('update:ufs', valorAtualizar);
        }
      }
    }
  },

  template: '#ListaSelecaoMultiplaUf-template'
});
