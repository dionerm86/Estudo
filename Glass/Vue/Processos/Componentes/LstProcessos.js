const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar],

  data: {
    dadosOrdenacao_: {
      campo: 'descricao',
      direcao: 'asc'
    },
    numeroLinhaEdicao: -1,
    inserindo: false,
    processo: {},
    processoOriginal: {},
    aplicacaoAtual: null
  },

  methods: {
    /**
     * Busca a lista de processos de etiqueta para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Processos.obterLista(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de processos.
     * @param {string} campo O nome do campo pelo qual o resultado será ordenado.
     */
    ordenar: function (campo) {
      if (campo !== this.dadosOrdenacao_.campo) {
        this.dadosOrdenacao_.campo = campo;
        this.dadosOrdenacao_.direcao = '';
      } else {
        this.dadosOrdenacao_.direcao = this.dadosOrdenacao_.direcao === '' ? 'desc' : '';
      }
    },

    /**
     * Cria a descrição para os tipos de pedidos que serão exibidos na listagem.
     * @param {Object} processo O objeto com o processo que será exibido.
     * @returns {string} O texto com a descrição dos tipos de pedidos.
     */
    obterDescricaoTiposPedidos: function (processo) {
      if (!processo.tiposPedidos || !processo.tiposPedidos.length) {
        return null;
      }

      var tiposPedidos = processo.tiposPedidos.slice()
        .map(function (item) {
          return item.nome;
        })
        .filter(function (item) {
          return !!item;
        });

      tiposPedidos.sort();
      return tiposPedidos.join(', ');
    },

    /**
     * Inicia o cadastro de processos de etiqueta.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um processo será editado.
     * @param {Object} processo O processo que será editado.
     * @param {number} numeroLinha O número da linha que contém o processo que será editado.
     */
    editar: function (processo, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(processo);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um processo de etiqueta.
     * @param {Object} processo O processo que será excluído.
     */
    excluir: function (processo) {

    },

    /**
     * Insere um novo processo de etiqueta.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }


    },

    /**
     * Atualiza os dados do processo atual.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }


    },

    /**
     * Cancela a edição ou cadastro de processo.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de processo.
     * @param {?Object} [processo=null] O processo que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (processo) {
      this.aplicacaoAtual = processo ? this.clonar(processo.aplicacao) : null;
      this.tipoProcesso = processo ? this.clonar(processo.tipoProcesso) : null;

      var tiposPedidos = function () {
        if (!processo || !processo.tiposPedidos || !processo.tiposPedidos.length) {
          return null;
        }

        return processo.tiposPedidos.slice()
          .map(function (tipoPedido) {
            return tipoPedido.id;
          })
          .filter(function (tipoPedido) {
            return !!tipoPedido;
          });
      };

      this.processo = {
        codigo: processo ? processo.codigo : null,
        descricao: processo ? processo.descricao : null,
        idAplicacao: processo && processo.aplicacao ? processo.aplicacao.id : null,
        destacarNaEtiqueta: processo ? processo.destacarNaEtiqueta : null,
        gerarFormaInexistente: processo ? processo.gerarFormaInexistente : null,
        gerarArquivoDeMesa: processo ? processo.gerarArquivoDeMesa : null,
        numeroDiasUteisDataEntrega: processo ? processo.numeroDiasUteisDataEntrega : null,
        tipoProcesso: processo && processo.tipoProcesso ? processo.tipoProcesso.id : null,
        tiposPedidos: tiposPedidos(),
        situacao: processo && processo.situacao ? processo.situacao.id : null
      };

      this.processoOriginal = this.clonar(this.processo);
    },

    /**
     * Função que indica se o formulário de processos possui valores válidos de acordo com os controles.
     * @param {Object} botao O botão que foi disparado no controle.
     * @returns {boolean} Um valor que indica se o formulário está válido.
     */
    validarFormulario_: function (botao) {
      var form = botao.form || botao;
      while (form.tagName.toLowerCase() !== 'form') {
        form = form.parentNode;
      }

      return form.checkValidity();
    }
  },

  computed: {
    /**
     * Propriedade computada que indica a ordenação para a lista.
     * @type {string}
     */
    ordenacao: function () {
      var direcao = this.dadosOrdenacao_.direcao ? ' ' + this.dadosOrdenacao_.direcao : '';
      return this.dadosOrdenacao_.campo + direcao;
    }
  },

  watch: {
    /**
     * Observador para a variável 'aplicacaoAtual'.
     * Atualiza a aplicação para o processo atual.
     */
    aplicacaoAtual: {
      handler: function (atual) {
        this.processo.idAplicacao = atual ? atual.id : null;
      },
      deep: true
    }
  }
});
