Vue.component('controle-beneficiamentos-setor', {
  mixins: [Mixins.UUID, Mixins.Beneficiamentos, Mixins.Objetos],
  props: {
    /**
     * Identificadores dos beneficiamentos selecionados.
     * @type {number[]}
     */
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
      pais: []
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
    },

    /**
     * Retorna um valor que indica se o beneficiamento possui filhos.
     * @param {object} beneficiamento O beneficiamento que está sendo verificado.
     * @returns {boolean} Verdadeiro, se o beneficiamento possuir filhos.
     */
    temFilhos: function(beneficiamento) {
      return beneficiamento && beneficiamento.filhos && beneficiamento.filhos.length;
    }
  },

  mounted: function() {
    this.uuid = this.gerarUuid();
  },

  computed: {
    /**
     * Propriedade computada que retorna os identificadores dos beneficiamentos selecionados
     * de forma normalizada e que atualiza a propriedade principal em caso de alteração.
     */
    idsSelecionadosAtuais: {
      get: function () {
        return this.idsSelecionados || [];
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.idsSelecionados)) {
          this.$emit('update:idsSelecionados', valor);

          if (valor && valor.length) {
            var pais = new Set(valor.concat(this.pais)
              .map(function (id) {
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

                return beneficiamento ? beneficiamento.id : null;
              }, this)
              .filter(function (id) {
                return id;
              })
            );

            this.pais = [...pais];
          } else {
            this.pais = [];
          }
        }
      }
    }
  },

  watch: {
    /**
     * Observador para a variável 'pais'.
     * Atualiza a lista de beneficiamentos selecionados se houver remoção de um pai.
     */
    pais: {
      handler: function (atual, anterior) {
        var removido = anterior.find(function (item) {
          return atual.indexOf(item) === -1;
        });

        if (!removido) {
          return;
        }

        var filhosBeneficiamento = this.beneficiamentos
          .find(function (item) {
            return item.id === removido;
          })
          .filhos || [];

        filhosBeneficiamento = filhosBeneficiamento.map(function (item) {
          return item.id;
        });

        this.idsSelecionados = this.idsSelecionados.filter(function (id) {
          return filhosBeneficiamento.indexOf(id) === -1;
        });
      },
      deep: true
    }
  },

  template: '#ControleBeneficiamentosSetor-template'
});
