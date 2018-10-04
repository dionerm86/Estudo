Vue.component('campo-busca-etiqueta-processo', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Processo selecionado.
     * @type {?Object}
     */
    processo: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Identificadores dos subgrupos para validar o processo.
     * @type {number[]}
     */
    idsSubgruposParaValidacao: {
      required: false,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarArrayOuVazio
    }
  },

  data: function() {
    return {
      idProcesso: (this.processo || {}).id || 0,
      codigoProcesso: (this.processo || {}).codigo
    };
  },

  methods: {
    /**
     * Busca os processos para o controle.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    buscarProcessos: function() {
      return Servicos.Processos.obterParaControle();
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o processo
     * e que atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    processoAtual: {
      get: function () {
        return this.processo;
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.processo)) {
          this.$emit('update:processo', valor);
          this.idProcesso = valor ? valor.id : 0;
          this.codigoProcesso = valor ? valor.codigo : null;
        }

        if (this.idsSubgruposParaValidacao && this.idsSubgruposParaValidacao.length) {
          var campo = this.$refs.base.$refs.campoBusca.$refs.campo;

          if (campo) {
            campo.setCustomValidity('');
            campo.reportValidity();

            if (valor) {
              Servicos.Processos.validarSubgrupos(valor.id, this.idsSubgruposParaValidacao)
                .catch(function (erro) {
                  if (erro && erro.mensagem && campo) {
                    campo.setCustomValidity(erro.mensagem);
                    campo.reportValidity();
                  }
                });
            }
          }
        }
      }
    }
  },

  template: '#CampoBuscaEtiquetaProcesso-template'
});
