Vue.component('lista-selecao-multipla', {
  mixins: [Mixins.UUID],
  props: {
    /**
     * Lista de IDs selecionados no controle.
     * @type {number[]}
     */
    idsSelecionados: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarArrayOuVazio
    },

    /**
     * Função utilizada para a recuperação dos itens para o controle.
     * @type {!function}
     * @returns {Promise}
     */
    funcaoRecuperarItens: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarFuncao
    },

    /**
     * Filtro utilizado para a execução da função de recuperação dos itens para o controle.
     * @type {?object}
     */
    filtroRecuperarItens: {
      required: false,
      twoWay: false,
      default: function () {
        return {}
      },
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Texto exibido quando não houver itens selecionados.
     * @type {?string}
     */
    textoSelecionar: {
      required: false,
      twoWay: false,
      default: 'Selecione...',
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * Indica se itens devem ser ordenados após recuperação.
     * @type {?boolean}
     */
    ordenar: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Nome da propriedade dos itens recuperados que representa o seu identificador.
     * @type {?string}
     */
    campoId: {
      required: false,
      twoWay: false,
      default: 'id',
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * Nome da propriedade dos itens recuperados que representa o seu nome.
     * @type {?string}
     */
    campoNome: {
      required: false,
      twoWay: false,
      default: 'nome',
      validator: Mixins.Validacao.validarStringOuVazio
    }
  },

  data: function() {
    return {
      uuid: null,
      divOpcoes: null,
      itens: [],
      selecionando: false,
      itensSelecionados: ''
    };
  },

  methods: {
    /**
     * Recupera os itens a partir da função definida na propriedade.
     */
    buscar: function() {
      var vm = this;

      this.funcaoRecuperarItens(this.filtroRecuperarItens)
        .then(function(resposta) {
          if (vm.ordenar) {
            resposta.data.sort(function(a, b) {
              return a.nome.localeCompare(b.nome);
            });
          }

          vm.itens = resposta.data;
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }

          vm.itens = [];
        });
    },

    /**
     * Exibe ou esconde a lista de opções para seleção do controle.
     * @param {Object} event O objeto com os dados do evento ocorrido.
     */
    alternarOpcoes: function(event) {
      var controle = event.target;
      while (controle.className !== 'lista-selecao-multipla') {
        controle = controle.parentNode;
      }

      var div = controle.children[1];

      if (div.style.display === 'none') {
        this.divOpcoes = div;
        this.redimensionarDivOpcoes();
        div.style.display = '';

        var vm = this;

        setTimeout(function() {
          document.addEventListener('click', vm.esconderOpcoes);
        }, 1);
      } else {
        this.esconderOpcoes();
      }
    },

    /**
     * Esconde a lista de opções do controle.
     */
    esconderOpcoes: function() {
      if (!this.selecionando) {
        this.divOpcoes.style.display = 'none';
        document.removeEventListener('click', this.esconderOpcoes);
      }
    },

    /**
     * Seleciona um item.
     */
    selecionarItem: function() {
      this.selecionando = true;
    },

    /**
     * Redimensiona a lista de opções, para que sua largura acompanhe a do controle.
     */
    redimensionarDivOpcoes: function() {
      var vm = this;

      this.$nextTick(function() {
        var controle = vm.divOpcoes.parentNode;
        vm.divOpcoes.style.width = controle.offsetWidth + 'px';
        vm.selecionando = false;
      });
    },

    /**
     * Retorna um identificador único para um item.
     * @param {!Object} item O item que será usado para o cálculo do identificador.
     * @returns {string} O identificador único associado ao item.
     */
    idUnico: function(item) {
      return item ? this.uuid + '_' + item[this.campoId] : null;
    },

    /**
     * Atualiza a lista de itens selecionados.
     */
    atualizarItensSelecionados: function() {
      var itens = [];

      for (var i of this.itens) {
        if (this.idsAtuais.indexOf(i[this.campoId]) > -1) {
          itens.push(i[this.campoNome]);
        }
      }

      this.itensSelecionados = itens.join(', ');
    }
  },

  mounted: function () {
    this.uuid = this.gerarUuid();
    this.buscar();
  },

  computed: {
    /**
     * Propriedade computada que contém a lista de IDs selecionados para uso interno do controle.
     * Garante que os ids sejam pelo menos um array vazio para leitura e que
     * a propriedade principal seja atualizada ao atribuir valor nessa propriedade.
     * @type {number[]}
     */
    idsAtuais: {
      get: function() {
        return this.idsSelecionados || [];
      },
      set: function(valor) {
        if (valor !== this.idsSelecionados) {
          this.$emit('update:idsSelecionados', valor);

          var vm = this;

          this.$nextTick(function() {
            vm.atualizarItensSelecionados();
            vm.redimensionarDivOpcoes();
          });
        }
      }
    }
  },

  watch: {
    /**
     * Observador para a propriedade 'filtroRecuperarItens'.
     * Reexecuta a recuperação de itens no caso do filtro ser alterado.
     */
    filtroRecuperarItens: {
      handler: function () {
        this.buscar();
      },
      deep: true
    }
  },

  template: '#ListaSelecaoMultipla-template'
});
