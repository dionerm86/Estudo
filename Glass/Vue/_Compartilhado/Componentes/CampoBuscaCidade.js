Vue.component('campo-busca-cidade', {
  mixins: [Mixins.Comparar],
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
      ufAtual: (this.cidade || {}).uf || this.uf || '',
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

      return Servicos.Cidades.obterListaPorUf(this.ufAtual, id, nome);
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
    }
  },

  watch: {
    /**
     * Observador para a variável 'ufAtual'.
     * Limpa as variáveis de ID e nome atual se alterar a UF.
     */
    ufAtual: function(atual) {
      this.cidadeAtual = null;
      
      if (atual !== this.uf) {
        this.$emit("update:uf", atual);
      }
    }
  },

  mounted: function() {
    this.buscarUfs();
  },

  template: '#CampoBuscaCidade-template'
});