const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.Patch],

  data: {
    dadosOrdenacao_: {
      campo: 'descricao',
      direcao: 'asc'
    },
    numeroLinhaEdicao: -1,
    inserindo: false,
    feriado: {},
    feriadoOriginal: {}
  },

  methods: {
    /**
     * Busca a lista de feriados para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Datas.Feriados.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de feriados.
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
     * Inicia o cadastro de feriado.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um feriado será editado.
     * @param {Object} feriado O feriado que será editado.
     * @param {number} numeroLinha O número da linha que contém o feriado que será editado.
     */
    editar: function (feriado, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(feriado);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um feriado.
     * @param {Object} feriado O feriado que será excluído.
     */
    excluir: function (feriado) {
      var vm = this;

      Servicos.Datas.Feriados.excluir(feriado.id)
        .then(function (resposta) {
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Insere um novo feriado.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Datas.Feriados.inserir(this.feriado)
        .then(function (resposta) {
          vm.atualizarLista();
          vm.cancelar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Atualiza os dados do feriado.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var feriadoAtualizar = this.patch(this.feriado, this.feriadoOriginal);
      var vm = this;

      Servicos.Datas.Feriados.atualizar(this.feriado.id, feriadoAtualizar)
        .then(function (resposta) {
          vm.atualizarLista();
          vm.cancelar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Cancela a edição ou cadastro do feriado.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de feriado.
     * @param {?Object} [feriado=null] O feriado que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (feriado) {
      this.feriado = {
        id: feriado ? feriado.id : null,
        descricao: feriado ? feriado.descricao : null,
        dia: feriado ? feriado.dia : null,
        mes: feriado ? feriado.mes : null
      };

      this.feriadoOriginal = this.clonar(this.feriado);
    },

    /**
     * Função que indica se o formulário de feriados possui valores válidos de acordo com os controles.
     * @param {Object} botao O botão que foi disparado no controle.
     * @returns {boolean} Um valor que indica se o formulário está válido.
     */
    validarFormulario_: function (botao) {
      var form = botao.form || botao;
      while (form.tagName.toLowerCase() !== 'form') {
        form = form.parentNode;
      }

      return form.checkValidity();
    },

    /**
     * Força a atualização da lista de feriados, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
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
  }
});
