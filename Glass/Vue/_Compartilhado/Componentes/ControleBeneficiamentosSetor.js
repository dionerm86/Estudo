Vue.component('controle-beneficiamentos-setor', {
  mixins: [Mixins.UUID, Mixins.Beneficiamentos, Mixins.Objetos],
  props: {
    idsSelecionados: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarArrayOuVazio
    }
  },

  data: function () {
    const estiloGrid = function(colunas) {
      return {
        display: 'inline-grid',
        gridTemplateColumns: 'repeat(' + colunas + ', max-content)',
        gridGap: '2px 8px',
        justifyItems: 'left'
      };
    };

    return {
      uuid: null,
      estiloGridPrincipal: estiloGrid(3),
      estiloGridFilhos: estiloGrid(4),
      selecionados: [],
      filhosSelecionados: []
    };
  },

  methods: {
    /**
     * Retorna um identificador único para o beneficiamento no controle.
     * @param {!object} beneficiamento O beneficiamento que está sendo criado.
     * @param {?string} [complemento=null] Um complemento que pode ser colocado no identificador.
     * @returns {string} O identificador único para o beneficiamento no controle.
     */
    idUnico: function(beneficiamento, complemento) {
      return this.uuid + '_' + beneficiamento.id + (complemento ? '_' + complemento : '');
    }
  },

  mounted: function() {
    this.uuid = this.gerarUuid();
  },

  computed: {
    idsSelecionadosAtuais: {
      get: function () {
        var idsPais = this.selecionados.slice().filter(function (id) {
          var beneficiamento = this.beneficiamentos.find(function (item) {
            return item.id === id;
          });

          return beneficiamento && !(beneficiamento.filhos && beneficiamento.filhos.length);
        }, this);

        var idsFilhos = this.filhosSelecionados.slice();

        return idsPais.concat(idsFilhos);
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.idsSelecionadosAtuais)) {
          this.$emit('update:idsSelecionados', valor);

          if (valor && valor.length) {
            var pais = new Set(valor.filter(function (id) {
              var beneficiamento = this.beneficiamentos.find(function (item) {
                if (item.id === id) {
                  return true;
                }

                if (item.filhos && item.filhos.length) {
                  for (var filho of item.filhos) {
                    if (filho.id === id) {
                      return true;
                    }
                  }
                }

                return false;
              });

              return !!beneficiamento;
            }, this));

            this.selecionados = [...pais];
            this.filhosSelecionados = valor.filter(function (id) {
              return this.selecionados.indexOf(id) === -1;
            }, this);
          } else {
            this.selecionados = [];
            this.filhosSelecionados = [];
          }
        }
      }
    }
  },

  watch: {
    selecionados: {
      handler: function (atual, anterior) {
        var removido = anterior.filter(function (item) {
          return atual.indexOf(item) === -1;
        });

        if (!removido) {
          return;
        }

        var beneficiamento = this.beneficiamentos.find(function (item) {
          return item.id === removido;
        });

        if (beneficiamento && beneficiamento.filhos && beneficiamento.filhos.length) {
          var idsFilhos = beneficiamento.filhos.map(function (item) {
            return item.id;
          });

          this.filhosSelecionados = this.filhosSelecionados
            .slice()
            .filter(function (item) {
              return idsFilhos.indexOf(item.id) === -1;
            });
        }
      },
      deep: true
    }
  },

  template: '#ControleBeneficiamentosSetor-template'
});
