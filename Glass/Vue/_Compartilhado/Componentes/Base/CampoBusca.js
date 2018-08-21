Vue.component('campo-busca', {
  inheritAttrs: false,
  mixins: [Mixins.Comparar, Mixins.ExecutarTimeout],
  props: {
    /**
     * O item que está selecionado atualmente no controle.
     * @type {Object}
     */
    itemSelecionado: {
      required: false,
      twoWay: true,
      default: null,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * O ID do item selecionado atualmente no controle.
     * @type {?number}
     */
    id: {
      required: false,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * O nome do item selecionado atualmente no controle.
     * @type {?string}
     */
    nome: {
      required: false,
      twoWay: true,
      default: '',
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * O nome do campo que corresponde ao 'ID' a ser exibido no controle.
     * @type {?string}
     */
    campoId: {
      required: false,
      twoWay: false,
      default: 'id',
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * O nome do campo que corresponde ao 'nome' a ser exibido no controle.
     * @type {?string}
     */
    campoNome: {
      required: false,
      twoWay: false,
      default: 'nome',
      validator: Mixins.Validacao.validarOuVazio
    },

    /**
     * Função que realiza a busca dos itens a partir do nome.
     * @type {!function}
     * @param {number} id O ID do item que será buscado.
     * @param {string} nome O nome que será usado para a busca.
     * @returns {Promise}
     */
    funcaoBuscarItens: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarFuncao
    },

    /**
     * Indica se os itens serão pré-carregados ao iniciar o controle ou
     * se a busca será feita sob demanda.
     * @type {?boolean}
     */
    preCarregarItens: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  data: function () {
    var vm = this;

    const valor = function (valorInicial, nomeCampo, valorDefault) {
      if (valorInicial) {
        return valorInicial;
      }

      if ((vm.itemSelecionado || {})[nomeCampo]) {
        return (vm.itemSelecionado || {})[nomeCampo];
      }

      return valorDefault;
    }

    var id = valor(this.id, this.campoId, 0);
    var nome = valor(this.nome, this.campoNome, '');

    return {
      idAtual: id,
      nomeAtual: nome,
      itens: [],
      buscando: false,
      selecionando: false,
      itemSelecionado_: this.itemSelecionado,
      pesquisando: false
    };
  },

  methods: {
    /**
     * Exibe os itens que podem ser selecionados.
     */
    exibir: function() {
      this.buscando = true;
      this.$el.children[1].style.minWidth = (this.$el.clientWidth - 2) + 'px';
      this.$el.children[1].style.maxWidth = (this.$el.clientWidth * 2) + 'px';
    },

    /**
     * Realiza a busca dos itens com base no nome informado no controle.
     * @param {number} id O ID do item que será buscado.
     * @param {string} nome O nome que será usado para a busca.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    buscar: function(id, nome) {
      if ((id || nome) && this.preCarregarItens) {
        var itemSelecionar = this.buscarItem(id, nome);
        if (itemSelecionar) {
          this.selecionar(itemSelecionar);
        }

        this.pesquisando = false;
        return Promise.resolve();
      }

      var vm = this;

      return this.funcaoBuscarItens(id, nome)
        .then(function (resposta) {
          vm.itens = resposta ? resposta.data : [];

          if (vm.itens.length === 0) {
            vm.itemSelecionadoAtual = null;
          } else if (vm.itens.length === 1) {
            vm.selecionar(vm.itens[0]);
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }

          vm.itens = [];
          vm.itemSelecionadoAtual = null;
        })
        .then(function () {
          vm.pesquisando = false;
        });
    },

    /**
     * Filtra dentre os itens pré-carregados se há apenas um item a ser selecionado.
     * @param {number} id O ID do item que será buscado.
     * @param {string} nome O nome que será usado para a busca.
     * @returns {?Object} O item encontrado, caso seja único.
     */
    buscarItem: function(id, nome) {
      if (!this.preCarregarItens || !(id || nome)) {
        return null;
      }

      var itemSelecionar = this.itens.filter(function (item) {
        return (id && item[this.campoId] == id)
          || (nome && item[this.campoNome] === nome);
      }, this);

      return itemSelecionar && itemSelecionar.length === 1
        ? itemSelecionar[0]
        : null;
    },

    /**
     * Marca um item como selecionado.
     * @param {Object} item O item que será selecionado.
     */
    selecionar: function(item) {
      if (this.idAtual !== item.id || !this.equivalentes(item, this.itemSelecionadoAtual)) {
        this.selecionando = true;

        this.itemSelecionadoAtual = item;
        this.idAtual = item[this.campoId];
        this.nomeAtual = item[this.campoNome];

        if (!this.preCarregarItens) {
          this.itens = [item];
        }

        this.$nextTick(function() {
          this.selecionando = false;
        });
      }
    },

    /**
     * Inicia a busca dos dados a partir do nome.
     * Disparado quando o nome é atualizado, interna ou externamente.
     * @param {string} nome O nome usado para a consulta.
     */
    fazerBuscaPeloNome: function (nome) {
      if (!nome) {
        return;
      }

      if (!this.selecionando) {
        this.pesquisando = true;

        if (!this.executandoBuscaTimeout && !this.preCarregarItens) {
          this.itens = [];
        }

        this.executarTimeout('fazerBuscaPeloNome', function () {
          this.idAtual = 0;
          this.buscar(null, nome);
        }, 500);
      }
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o item selecionado atualmente no controle e
     * que atualiza a propriedade se alterada.
     * @type {Object}
     */
    itemSelecionadoAtual: {
      get: function() {
        if (this && !this.itemSelecionado && !this.itemSelecionado_ && this.id && this.nome) {
          this.itemSelecionado_ = {};
          this.itemSelecionado_[this.campoId] = this.id;
          this.itemSelecionado_[this.campoNome] = this.nome;
        }

        return this.itemSelecionado || this.itemSelecionado_ || null;
      },
      set: function(valor) {
        if (!this.equivalentes(valor, this.itemSelecionado_)) {
          this.itemSelecionado_ = valor;
          this.$emit('update:itemSelecionado', valor);
        }
      }
    },

    /**
     * Propriedade computada com os itens a serem exibidos no controle como selecionáveis.
     * @returns {Object[]}
     */
    itensFiltrados: function() {
      var filtrados = [];
      var selecionado = this.itemSelecionadoAtual;

      if (this.nomeAtual) {
        if (this.preCarregarItens) {
          if (this.idAtual !== 0 && this.nomeAtual === selecionado[this.campoNome]) {
            for (var i in this.itens) {
              if (this.itens[i][this.campoId] === this.idAtual) {
                filtrados.push(this.itens[i]);
                break;
              }
            }
          }

          if (filtrados.length === 0) {
            var nomeComparar = this.nomeAtual ? this.nomeAtual.toLocaleLowerCase() : null;

            if (nomeComparar) {
              for (var i in this.itens) {
                var nomeItem = this.itens[i][this.campoNome].toLocaleLowerCase();

                if (nomeItem.indexOf(nomeComparar) > -1) {
                  filtrados.push(this.itens[i]);
                }
              }
            }
          }
        } else {
          filtrados = this.itens;
        }

        filtrados = filtrados.slice(0, 5);

        if (filtrados.length === 1) {
          this.selecionar(filtrados[0]);
        }
      } else {
        this.idAtual = 0;
        this.itemSelecionadoAtual = null;
      }

      return filtrados;
    }
  },

  mounted: function () {
    var vm = this;

    const selecionarItemAtual = function () {
      if (vm.idAtual) {
        vm.buscar(vm.idAtual, vm.nomeAtual);
      }
      else if (vm.nomeAtual) {
        vm.buscar(null, vm.nomeAtual).then(function () {
          if (vm.idAtual) {
            var itemAtual = null;

            for (var i in vm.itens) {
              if (i[vm.campoId] === vm.idAtual) {
                itemAtual = i;
                break;
              }
            }

            if (itemAtual) {
              vm.selecionar(itemAtual);
            }
          }
        });
      }
    };

    if (this.preCarregarItens) {
      this.buscar().then(selecionarItemAtual);
    } else {
      selecionarItemAtual();
    }
  },

  watch: {
    /**
     * Observador para a variável 'id'.
     * Limpa os itens selecionáveis se o ID atual for alterado para zero,
     * no caso de não haver itens pré-carregados.
     */
    id: function(atual, anterior) {
      if (atual === 0 && anterior !== 0 && !this.preCarregarItens) {
        this.itens = [];
      } else if (atual != 0 && atual != anterior) {
        this.buscar(atual);
      }
    },

    /**
     * Observador para a variável 'nome'.
     * Refaz a busca em caso de alteração do nome com o ID zerado (alteração externa).
     */
    nome: function (atual) {
      this.nomeAtual = atual;

      if (atual) {
        this.fazerBuscaPeloNome(atual);
      }
    },

    /**
     * Observador para a variável 'itemSelecionado'.
     * Atualiza os dados do controle para refletir o novo item.
     */
    itemSelecionado: function (atual) {
      if (this.selecionando) {
        return;
      }

      if (atual) {
        this.selecionar(atual);
      } else {
        this.idAtual = 0;
        this.nomeAtual = '';
      }
    },

    /**
     * Observador para a variável 'nomeAtual'.
     * Realiza o filtro pelo nome.
     */
    nomeAtual: function (atual) {
      this.fazerBuscaPeloNome(atual);
    }
  },

  template: '#CampoBusca-template'
});
