const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.ExecutarTimeout],

  data: {
    inserindo: false,
    editando: false,
    funcionario: {},
    funcionarioOriginal: {},
    configuracoes: {},
    tipoFuncionarioAtual: null,
    situacaoAtual: {},
    lojaAtual: {},
  },

  methods: {
    /**
     * Busca os dados de um pedido.
     * @param {?number} id O número do pedido.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    buscarFuncionario: function (id) {
      var vm = this;

      return Servicos.Funcionarios.obterFuncionario(id)
        .then(function (resposta) {
          vm.funcionario = resposta.data;
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Retorna os itens para o controle de situações do funcionário.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return Servicos.Funcionarios.obterSituacoes();
    },

    /**
     * Retorna os itens para o controle de situações do funcionário.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterTiposFuncionario: function () {
      return Servicos.Funcionarios.obterTiposFuncionario();
    },

    /**
     * Recupera os tipos de pedido para exibição no cadastro ou edição do pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterUfs: function () {
      return Servicos.Cidades.listarUfs();
    },

    /**
     * Inicia um cadastro ou edição de produto, criando o objeto para 'bind'.
     * @param {Object} item O produto que será usado como base, no caso de edição.
     */
    iniciarCadastroOuAtualizacao_: function (item) {
      this.funcionarioOriginal = item ? this.clonar(item) : {};

      this.lojaAtual = {
        id: item && item.loja ? item.loja.id : null
      };

      this.tipoFuncionarioAtual = {
        id: item && item.tipoFuncionarioAtual ? item.tipoFuncionarioAtual.id : null
      };

      this.situacalAtual = {
        id: item && item.situacalAtual ? item.situacalAtual.id : null
      };

      this.funcionario = {
        id: item && item.id ? item.id : null,
        idTipoFuncionario: item && item.idTipoFuncionario ? item.idTipoFuncionario : null,
        idsSetores:item && item.idsSetores ? item.idsSetores : null,
        nome: item && item.nome ? item.nome : null,
        idLoja: item && item.loja ? item.loja.id : null,
        idSituacao: item && item.situacao ? item.situacao.id : true,
        numeroDiasParaAtrasarPedidos: item ? item.numeroDiasParaAtrasarPedidos : null,
        numeroPdv: item ? item.numeroPdv : null,
        idsTiposPedidos: item && item.idsTiposPedidos ? item.idsTiposPedidos : null,
        observacao: item && item.observacao ? item.observacao : null,
        endereco:{
          logradouro: item && item.endereco ? item.endereco.logradouro : null,
          complemento: item && item.endereco ? item.endereco.complemento : null,
          bairro: item && item.endereco ? item.endereco.bairro : null,
          cidade: item && item.endereco ? item.endereco.cidade.nome : null,
          cep: item && item.endereco ? item.endereco.cep : null,
        },
        contatos :{
          telefoneResidencial: item && item.contatos ? item.contatos.telefoneResidencial : null,
          telefoneCelular: item && item.contatos ? item.contatos.telefoneCelular : null,
          telefoneContato: item && item.contatos ? item.contatos.telefoneContato : null,
          email: item && item.contatos ? item.contatos.email : null,
          ramal: item && item.contatos ? item.contatos.ramal : null,
        },
        acesso :{
          login: item && item.acesso ? item.acesso.login : true,
          senha: item && item.acesso ? item.acesso.senha : true,
        },
        documentosEDadosPessoais :{
          rg: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.rg : null,
          cpf: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.cpf : null,
          funcao: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.funcao : null,
          idEstadoCivil: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.estadoCivil.id : null,
          dataNascimento: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.dataNascimento : null,
          dataEntrada: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.dataEntrada : null,
          dataSaida: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.dataSaida : null,
          salario: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.salario : null,
          gratificacao: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.gratificacao : null,
          numeroCTPS: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.numeroCTPS : null,
          auxilioAlimentacao: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.auxilioAlimentacao : null,
          numeroPis: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.numeroPis : null,
          registrado: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.registrado : false,
        },
        permissoes: {
          enviarEmailPedidoFinalizado: item && item.permissoes ? item.permissoes.enviarEmailPedidoFinalizado : false,
          utilizarChat: item && item.permissoes ? item.permissoes.utilizarChat : false,
          habilitarControleUsuarios: item && item.permissoes ? item.permissoes.habilitarControleUsuarios : false,
        }
      };
    },

    /**
     * Função que indica se o formulário de pedido possui valores válidos de acordo com os controles.
     * @param {Object} botao O botão que foi disparado no controle.
     * @returns {boolean} Um valor que indica se o formulário está válido.
     */
    validarFormulario_: function (botao) {
      var form = botao.form || botao;
      while (form.tagName.toLowerCase() !== 'form') {
        form = form.parentNode;
      }

      if (!form.checkValidity() || !this.validarBase_()) {
        return false;
      }

      return true;
    },

    /**
     * Insere o pedido, se possível.
     * @param {Object} event O objeto do evento JavaScript.
     */
    inserirFuncionario: function (event) {
      if (!this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Funcionarios.inserir(this.funcionario)
        .then(function (resposta) {
          var url = '../Cadastros/LstFuncionario.aspx';

          window.location.assign(url);
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
 * Inicia o modo de edição do pedido.
 */
    editar: function () {
      this.iniciarCadastroOuAtualizacao_(this.funcionario);
      this.inserindo = false;
      this.editando = true;
    },

    /**
     * Atualiza o pedido, se possível.
     * @param {Object} event O objeto do evento JavaScript.
     */
    atualizarFuncionario: function (event) {
      if (!this.validarFormulario_(event.target)) {
        return;
      }

      var funcionarioAtualizar = this.patch(this.funcionario, this.funcionarioOriginal);

      var vm = this;

      Servicos.Funcionarios.atualizar(this.funcionario.id, funcionarioAtualizar)
        .then(function (resposta) {
          vm.cancelar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
      * Cancela a edição ou cadastro de pedido.
      */
    cancelar: function () {
      if (this.editando) {
        this.funcionario = this.clonar(this.funcionarioOriginal);
        this.editando = false;
      } else if (this.inserindo) {
        this.redirecionarParaListagem();
      }
    },

    /**
     * Redireciona a tela atual para a tela de listagem correspondente.
     */
    redirecionarParaListagem: function () {
      var idRelDinamico = GetQueryString('idRelDinamico');
      var url = '../Listas/LstFuncionario.aspx';

      if (idRelDinamico) {
        url = '../Relatorios/Dinamicos/ListaDinamico.aspx?id=' + idRelDinamico;
      }

      window.location.assign(url);
    },
  },
    mounted: function () {
      var id = GetQueryString('idFunc');
      var vm = this;

      Servicos.Funcionarios.obterConfiguracoesDetalhe(id || 0)
        .then(function (resposta) {
          vm.configuracoes = resposta.data;

          if (!id) {
            vm.inserindo = true;
            vm.iniciarCadastroOuAtualizacao_();
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });

      if (id) {
        this.buscarFuncionario(id)
          .then(function () {
            if (!vm.funcionario || !vm.funcionario.permissoes.podeEditar) {
              vm.redirecionarParaListagem();
            }
          });
      }
    },
    watch: {
      /**
     * Observador para a variável 'obraAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
      tipoFuncionarioAtual: {
        handler: function (atual) {
          this.funcionario.idTipoFuncionario = atual ? atual.id : null;
        },
        deep: true
      },

      /**
     * Observador para a variável 'obraAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
      situacaoAtual: {
        handler: function (atual) {
          this.funcionario.idSituacao = atual ? atual.id : null;
        },
        deep: true
      },

      /**
     * Observador para a variável 'obraAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
      lojaAtual: {
        handler: function (atual) {
          this.funcionario.idLoja = atual ? atual.id : null;
        },
        deep: true
      }
    }
});
