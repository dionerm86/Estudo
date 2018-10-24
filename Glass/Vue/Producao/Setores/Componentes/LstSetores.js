const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('descricao', 'asc')],

  data: {
    configuracoes: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
    setor: {},
    setorOriginal: {},
    tipoAtual: {},
    corAtual: {},
    corTelaAtual: {},
    situacaoAtual: {}
  },

  methods: {
    /**
     * Busca a lista de setores para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Producao.Setores.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna os itens para o controle de tipos de setor.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensTipo: function () {
      return Servicos.Producao.Setores.obterTipos();
    },

    /**
     * Retorna os itens para o controle de cores de setor.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensCor: function () {
      return Servicos.Producao.Setores.obterCores();
    },

    /**
     * Retorna os itens para o controle de cores de tela de setor.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensCorTela: function () {
      return Servicos.Producao.Setores.obterCoresTela();
    },

    /**
     * Retorna os itens para o controle de situações de setor.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensSituacao: function () {
      return Servicos.Producao.Setores.obterSituacoes();
    },

    /**
     * Abre um popup para exibir dados de fornada.
     */
    abrirTabelaFornada: function () {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=AberturaFornada');
    },

    /**
     * Inicia o cadastro de setor.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um setor será editado.
     * @param {Object} setor O setor que será editado.
     * @param {number} numeroLinha O número da linha que contém o setor que será editado.
     */
    editar: function (setor, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(setor);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um setor.
     * @param {Object} setor O setor que será excluído.
     */
    excluir: function (setor) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este setor?')) {
        return;
      }

      var vm = this;

      Servicos.Producao.Setores.excluir(setor.id)
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
     * Insere um novo setor.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Producao.Setores.inserir(this.setor)
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
     * Atualiza os dados do setor.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var setorAtualizar = this.patch(this.setor, this.setorOriginal);
      var vm = this;

      Servicos.Producao.Setores.atualizar(this.setor.id, setorAtualizar)
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
     * Altera a posição do setor.
     * @param {!number} setor O setor que terá a posição alterada.
     * @param {!boolean} acima Define se o setor será movimentado para cima..
     */
    alterarPosicao: function (setor, acima) {
      var vm = this;

      Servicos.Producao.Setores.alterarPosicao(setor.id, acima)
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
     * Cancela a edição ou cadastro do setor.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de setor.
     * @param {?Object} [setor=null] O setor que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (setor) {
      this.tipoAtual = setor && setor.funcoes && setor.funcoes.tipo ? this.clonar(setor.funcoes.tipo) : null;
      this.corAtual = setor && setor.cores && setor.cores.setor ? this.clonar(setor.cores.setor) : null;
      this.corTelaAtual = setor && setor.cores && setor.cores.tela ? this.clonar(setor.cores.tela) : null;
      this.situacaoAtual = setor && setor.situacao ? this.clonar(setor.situacao) : null;

      this.setor = {
        id: setor ? setor.id : null,
        nome: setor ? setor.nome : null,
        codigo: setor ? setor.codigo : null,
        sequencia: setor ? setor.sequencia : null,
        situacao: setor && setor.situacao ? setor.situacao.id : null,
        alturaMaxima: setor && setor.capacidade ? setor.capacidade.alturaMaxima : null,
        larguraMaxima: setor && setor.capacidade ? setor.capacidade.larguraMaxima : null,
        capacidadeDiaria: setor && setor.capacidade ? setor.capacidade.diaria : null,
        ignorarCapacidadeDiaria: setor && setor.capacidade ? setor.capacidade.ignorarCapacidadeDiaria : null,
        corSetor: setor && setor.cores && setor.cores.setor ? setor.cores.setor.id : null,
        corTela: setor && setor.cores && setor.cores.tela ? setor.cores.tela.id : null,
        exibirSetoresLeituraPeca: setor && setor.exibicoes ? setor.exibicoes.setoresLeituraPeca : null,
        exibirNaListaERelatorio: setor && setor.exibicoes ? setor.exibicoes.listaERelatorio : null,
        exibirPainelComercial: setor && setor.exibicoes ? setor.exibicoes.painelComercial : null,
        exibirPainelProducao: setor && setor.exibicoes ? setor.exibicoes.painelProducao : null,
        exibirImagemCompleta: setor && setor.exibicoes ? setor.exibicoes.imagemCompleta : null,
        consultarAntesDaLeitura: setor && setor.exibicoes ? setor.exibicoes.consultarAntesDaLeitura : null,
        tipo: setor && setor.funcoes && setor.funcoes.tipo ? setor.funcoes.tipo.id : null,
        corte: setor && setor.funcoes ? setor.funcoes.corte : null,
        forno: setor && setor.funcoes ? setor.funcoes.forno : null,
        laminado: setor && setor.funcoes ? setor.funcoes.laminado : null,
        entradaEstoque: setor && setor.funcoes ? setor.entradaEstoque : null,
        gerenciarFornada: setor && setor.funcoes ? setor.funcoes.gerenciarFornada : null,
        desafioPerda: setor && setor.perda ? setor.perda.desafio : null,
        metaPerda: setor && setor.perda ? setor.perda.meta : null,
        impedirAvanco: setor && setor.restricoes ? setor.restricoes.impedirAvanco : null,
        informarRota: setor && setor.restricoes ? setor.restricoes.informarRota : null,
        informarCavalete: setor && setor.restricoes ? setor.restricoes.informarCavalete : null,
        permitirLeituraForaRoteiro: setor && setor.restricoes ? setor.restricoes.permitirLeituraForaRoteiro : null,
        tempoLogin: setor && setor.tempoLogin ? setor.tempoLogin.maximo : null,
        tempoAlertaInatividade: setor && setor.tempoLogin ? setor.tempoLogin.alertaInatividade : null
      };

      this.setorOriginal = this.clonar(this.setor);
    },

    /**
     * Função que indica se o formulário de setores possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de setores, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  watch: {
    /**
     * Observador para a variável 'tipoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoAtual: {
      handler: function (atual) {
        if (this.setor) {
          this.setor.tipo = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'corAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    corAtual: {
      handler: function (atual) {
        if (this.setor) {
          this.setor.corSetor = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'corTelaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    corTelaAtual: {
      handler: function (atual) {
        if (this.setor) {
          this.setor.corTela = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacaoAtual: {
      handler: function (atual) {
        if (this.setor) {
          this.setor.situacao = atual ? atual.id : null;
        }
      },
      deep: true
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Producao.Setores.obterConfiguracoesLista().then(function (resposta) {
      vm.configuracoes = resposta.data;
    });
  }
});
