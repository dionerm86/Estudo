Vue.component('lista-paginada', {
  props: {
    /**
     * Função utilizada para a recuperação dos itens para o controle.
     * @type {!function}
     * @param {Object} filtro O filtro que será usado para a busca de itens.
     * @param {number} paginaAtual O número da página atual da lista.
     * @param {number} numeroRegistros O número de itens a serem exibidos na lista.
     * @param {string} ordenacao O campo (e direção, se houver) para ordenação do resultado.
     * @returns {Promise}
     */
    funcaoRecuperarItens: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarFuncao
    },

    /**
     * Filtro utilizado para a função de recuperação de itens.
     * @type {Object}
     */
    filtro: {
      required: false,
      twoWay: false,
      default: function() {
        return {};
      },
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Número de registros que serão exibidos por página.
     * @type {?number}
     */
    numeroRegistros: {
      required: false,
      twoWay: false,
      default: 10,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Ordenação atual para a lista de itens.
     * @type {?string}
     */
    ordenacao: {
      required: false,
      twoWay: false,
      default: '',
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * Texto exibido quando não houver itens encontrados após busca.
     * @type {?string}
     */
    mensagemListaVazia: {
      required: false,
      twoWay: false,
      default: 'Não foram encontrados itens para o filtro especificado.',
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * Número da linha que está sendo editada em um determinado momento.
     * @type {?number}
     */
    linhaEditando: {
      required: false,
      twoWay: false,
      default: -1,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica se deve ser exibida a linha para inclusão de novo registro.
     * @type {?boolean}
     */
    exibirInclusao: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },
  },

  data: function() {
    return {
      paginaAtual: 1,
      ultimaPagina: 0,
      itens: []
    };
  },

  methods: {
    /**
     * Função que realiza a paginação da lista.
     * @param {number} pagina O número da página que será exibida.
     */
    paginar: function(pagina) {
      pagina = Math.max(pagina, 1);
      pagina = Math.min(pagina, this.ultimaPagina);
      this.paginaAtual = pagina;
      this.linhaEditando = -1;
    },

    /**
     * Função que realiza a ordenação da lista.
     * @param {string} campo O nome do campo pelo qual será feita a ordenação.
     */
    ordenar: function(campo) {
      this.ordenacao = campo;
    },

    /**
     * Função que busca os itens da lista, com base no filtro, página, número de registros
     * e ordenação atual.
     */
    atualizar: function() {
      var vm = this;

      this.indicarBloqueio();

      this.funcaoRecuperarItens(this.filtro, this.paginaAtual, this.numeroRegistros, this.ordenacao)
        .then(function(resposta) {
          var links = resposta.headers.link;

          links = !links
            ? null
            : links
                .split(',')
                .filter(function(l) {
                  return l.indexOf('; rel="last"') > -1;
                })
                .map(function(l) {
                  return l.trim().substring(1, l.indexOf('>;') - 1);
                })[0];

          vm.ultimaPagina = !links ? vm.paginaAtual : parseInt(GetQueryString('pagina', links), 10) || vm.paginaAtual;

          vm.itens = resposta.data || [];
          vm.$emit('atualizou-itens', vm.itens.length);
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }

          vm.itens = [];
        })
        .then(function () {
          vm.finalizarBloqueio();
        });
    },

    /**
     * Função que retorna o estilo da linha alternada, com base no número da linha.
     * @param {number} index O número da linha que será estilizada.
     * @return {string} O nome da classe CSS que será utilizada pela linha.
     */
    estiloLinhaAlternada: function(index) {
      return index % 2 === 0 ? '' : 'alt';
    }
  },

  watch: {
    /**
     * Observador para a variável 'filtro'.
     * Realiza a atualização dos dados da lista, exibindo novamente a primeira página.
     */
    filtro: {
      handler: function() {
        this.paginaAtual = 1;
        this.atualizar();
      },
      deep: true
    },

    /**
     * Observador para a variável 'paginaAtual'.
     * Realiza a atualização dos dados da lista.
     */
    paginaAtual: function() {
      this.atualizar();
    },

    /**
     * Observador para a variável 'ordenacao'.
     * Realiza a atualização dos dados da lista.
     */
    ordenacao: function() {
      this.atualizar();
    }
  },

  computed: {
    /**
     * Propriedade computada que indica o número de colunas da lista.
     * Usada para os números de página, que devem ter um 'colspan' com o tamanho da lista.
     * @type {number}.
     */
    numeroColunas: function() {
      return this.$slots.cabecalho.filter(function(h) {
        return h.tag;
      }).length;
    },

    /**
     * Propriedade computada com a lista de páginas que serão exibidas na lista.
     * Calculada a partir da página atual, pode ter links ou texto (com base no retorno).
     * @type {dadosPagina[]}.
     *
     * @typedef {Object} dadosPagina
     * @property {number|string} numero O 'número' da página. Pode ser um texto com '...'.
     * @property {boolean} link Indica se a página atual é um link.
     */
    paginas: function() {
      const paginas = [];
      const numeroPaginasExibirAntesEDepois = 10;

      const primeiraPaginaExibir = Math.max(this.paginaAtual - numeroPaginasExibirAntesEDepois, 1);
      const ultimaPaginaExibir = Math.min(this.paginaAtual + numeroPaginasExibirAntesEDepois, this.ultimaPagina);

      const novaPagina = function(numero) {
        return {
          numero: numero,
          link: !isNaN(numero)
        };
      };

      if (primeiraPaginaExibir > 1) {
        paginas.push(novaPagina(1));

        if (primeiraPaginaExibir > 3) {
          paginas.push(novaPagina('...'));
        } else if (primeiraPaginaExibir === 3) {
          paginas.push(novaPagina(2));
        }
      }

      for (var i = primeiraPaginaExibir; i <= ultimaPaginaExibir; i++) {
        paginas.push(novaPagina(i));
      }

      if (ultimaPaginaExibir < this.ultimaPagina) {
        if (ultimaPaginaExibir < this.ultimaPagina - 2) {
          paginas.push(novaPagina('...'));
        } else if (ultimaPaginaExibir == this.ultimaPagina - 2) {
          paginas.push(novaPagina(this.ultimaPagina - 1));
        }

        paginas.push(novaPagina(this.ultimaPagina));
      }

      return paginas;
    }
  },

  mounted: function() {
    this.atualizar();
  },

  template: '#ListaPaginada-template'
});
