Vue.component('campo-busca-cidade', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * ID da cidade selecionada.
     * @type {object}
     */
    cidade: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

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

  data: function() {
    return {
      idAtual: (this.cidade || {}).id || 0,
      nomeAtual: (this.cidade || {}).nome || '',
      ufs: []
    };
  },

  methods: {
    /**
     * Busca a lista de UFs.
     */
    buscarUfs: function() {
      var vm = this;

      Servicos.Cidades.listarUfs()
        .then(function (resposta) {
          var ufAtual = vm.ufAtual;

          resposta.data.sort(function(a, b) {
            return a.localeCompare(b);
          });

          vm.ufs = resposta.data;

          if (ufAtual) {
            vm.ufAtual = ufAtual;
          }
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }

          vm.ufs = [];
        });
    },

    /**
     * Busca a lista de cidades para uma UF específica.
     * @param {number} id O ID do item que será buscado.
     * @param {string} nome O nome da cidade para filtro.
     * @returns {Promise} Uma Promise com o resultado da busca de cidades.
     */
    buscarCidades: function(id, nome) {
      if (!id && !nome) {
        return Promise.resolve();
      }
      
      var uf = this.ufAtual;

      return Servicos.Cidades.obterListaPorUf(uf, id, nome)
        .then(function (resposta) {
          if (resposta.data) {
            resposta.data = resposta.data.map(item => {
              item.uf = uf;
              return item;
            });
          }

          return resposta;
        });
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna a cidade e que
     * atualiza a propriedade em caso de alteração.
     * @type {object}
     */
    cidadeAtual: {
      get: function () {
        return this.cidade;
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.cidade)) {
          this.$emit('update:cidade', valor);
        }
      }
    },

    ufAtual: {
      get: function () {
        return this.uf;
      },
      set: function (valor) {
        this.cidadeAtual = null;

        if (valor !== this.uf) {
          this.$emit("update:uf", valor);
        }
      }
    }
  },

  watch: {
    cidade: {
      handler: function (valor) {
        this.cidadeAtual = valor;
      },
      deep: true
    },

    uf: function (valor) {
      this.ufAtual = valor;
    }
  },

  mounted: function() {
    this.buscarUfs();
  },

  template: '#CampoBuscaCidade-template'
});
