const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('nome', 'asc')],

  data: {
    numeroLinhaEdicao: -1,
    inserindo: false,
    fabricanteFerragem: {},
    fabricanteFerragemOriginal: {}
  },

  methods: {
    /**
     * Busca a lista de fabricantes de ferragem para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Projetos.Ferragens.Fabricantes.obterLista(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Inicia o cadastro de fabricante de ferragem.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um fabricante de ferragem será editado.
     * @param {Object} fabricanteFerragem O fabricante de ferragem que será editado.
     * @param {number} numeroLinha O número da linha que contém o fabricante de ferragem que será editado.
     */
    editar: function (fabricanteFerragem, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(fabricanteFerragem);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um fabricante de ferragem.
     * @param {Object} fabricanteFerragem O fabricante de ferragem que será excluído.
     */
    excluir: function (fabricanteFerragem) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este fabricante de ferragem?')) {
        return;
      }

      var vm = this;

      Servicos.Projetos.Ferragens.Fabricantes.excluir(fabricanteFerragem.id)
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
     * Insere um novo fabricante de ferragem.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Projetos.Ferragens.Fabricantes.inserir(this.fabricanteFerragem)
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
     * Atualiza os dados do fabricante de ferragem.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var fabricanteFerragemAtualizar = this.patch(this.fabricanteFerragem, this.fabricanteFerragemOriginal);
      var vm = this;

      Servicos.Projetos.Ferragens.Fabricantes.atualizar(this.fabricanteFerragem.id, fabricanteFerragemAtualizar)
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
     * Cancela a edição ou cadastro do fabricante de ferragem.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de fabricante de ferragem.
     * @param {?Object} [fabricanteFerragem=null] O fabricante de ferragem que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (fabricanteFerragem) {
      this.fabricanteFerragem = {
        id: fabricanteFerragem ? fabricanteFerragem.id : null,
        nome: fabricanteFerragem ? fabricanteFerragem.nome : null,
        site: fabricanteFerragem ? fabricanteFerragem.site : null
      };

      this.fabricanteFerragemOriginal = this.clonar(this.fabricanteFerragem);
    },

    /**
     * Função que indica se o formulário de fabricantes de ferragem possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de fabricantes de ferragem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  }
});
