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
    estadoCivilAtual: {},
  },

  methods: {
    /**
     * Busca os dados de um funcionário.
     * @param {?number} id O identificador do funcionário.
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
      return Servicos.Comum.obterSituacoes();
    },

    /**
     * Retorna os itens para o controle de tipos do funcionário.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterTiposFuncionario: function () {
      return Servicos.Funcionarios.obterTiposFuncionario();
    },

    /**
     * Retorna os dados da imagem do funcionário em uma string base64.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    fotoSelecionada: function (arquivo) {
      this.funcionario.documentosEDadosPessoais.foto = arquivo;
    },

    /**
     * Recupera os estados civis para exibição no cadastro ou edição do funcionário.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterEstadosCivil: function () {
      return Servicos.Funcionarios.obterEstadosCivil();
    },

    /**
     * Recupera os tipos de funcionário para exibição no cadastro ou edição do funcionário.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensTipoPedido: function () {
      return Servicos.Pedidos.obterTiposPedido();
    },

    /**
 * Recupera os tipos de funcionário para exibição no cadastro ou edição do funcionário.
 * @returns {Promise} Uma Promise com o resultado da busca.
 */
    obterItensTipofuncionário: function () {
      return Servicos.funcionários.obterTiposfuncionário();
    },

    /**
     * Retorna os itens para o controle de setores.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensSetor: function () {
      return Servicos.Producao.Setores.obterParaControle(
       false,
       false
     );
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
        id: item && item.tipoFuncionario ? item.tipoFuncionario.id : null
      };

      this.situacaoAtual = {
        id: item && item.situacao ? item.situacao.id : null
      };

      this.estadoCivilAtual = {
        id: item && item.documentosEDadosPessoais && item.documentosEDadosPessoais.estadoCivil ?
          item.documentosEDadosPessoais.estadoCivil : null
      };

      this.funcionario = {
        id: item && item.id ? item.id : null,
        idTipoFuncionario: item && item.tipoFuncionario ? item.tipoFuncionario.id : null,
        idsSetores: item && item.idsSetores ? item.idsSetores : [],
        nome: item && item.nome ? item.nome : null,
        idLoja: item && item.loja ? item.loja.id : null,
        numeroDiasParaAtrasarPedidos: item ? item.numeroDiasParaAtrasarPedidos : 0,
        numeroPdv: item ? item.numeroPdv : 0,
        idsTiposPedidos: item && item.idsTiposPedidos ? item.idsTiposPedidos : [],
        observacao: item && item.observacao ? item.observacao : null,
        adminSync: item && item.adminSync ? item.adminSync : false,
        urlImagem: item && item.urlImagem ? item.urlImagem : null,
        possuiImagem: item && item.possuiImagem ? item.possuiImagem : false,
        contatos: {
          telefoneResidencial: item && item.contatos ? item.contatos.telefoneResidencial : null,
          telefoneCelular: item && item.contatos ? item.contatos.telefoneCelular : null,
          telefoneContato: item && item.contatos ? item.contatos.telefoneContato : null,
          email: item && item.contatos ? item.contatos.email : null,
          ramal: item && item.contatos ? item.contatos.ramal : null,
        },
        acesso: {
          login: item && item.acesso ? item.acesso.login : null,
          senha: item && item.acesso ? item.acesso.senha : null,
        },
        documentosEDadosPessoais: {
          rg: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.rg : null,
          cpf: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.cpf : null,
          funcao: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.funcao : null,
          dataNascimento: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.dataNascimento : null,
          dataEntrada: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.dataEntrada : null,
          dataSaida: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.dataSaida : null,
          salario: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.salario : 0,
          gratificacao: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.gratificacao : 0,
          numeroCTPS: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.numeroCTPS : null,
          auxilioAlimentacao: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.auxilioAlimentacao : 0,
          numeroPis: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.numeroPis : null,
          registrado: item && item.documentosEDadosPessoais ? item.documentosEDadosPessoais.registrado : false,
        },
        endereco: {
          logradouro: item && item.endereco ? item.endereco.logradouro : null,
          complemento: item && item.endereco ? item.endereco.complemento : null,
          bairro: item && item.endereco ? item.endereco.bairro : null,
          cep: item && item.endereco ? item.endereco.cep : null,
          cidade: {
            id: item && item.endereco && item.endereco.cidade ? item.endereco.cidade.id : null,
            nome: item && item.endereco && item.endereco.cidade ? item.endereco.cidade.nome : null,
            uf: item && item.endereco && item.endereco.cidade ? item.endereco.cidade.uf : null
          }
        },
        permissoes: {
          utilizarChat: item && item.permissoes ? item.permissoes.utilizarChat : false,
          habilitarControleUsuarios: item && item.permissoes ? item.permissoes.habilitarControleUsuarios : false,
          enviarEmailPedidoConfirmadoVendedor: item && item.permissoes ? item.permissoes.enviarEmailPedidoConfirmadoVendedor : false,
        }
      };
    },

    /**
     * Função que indica se o formulário de funcionário possui valores válidos de acordo com os controles.
     * @param {Object} botao O botão que foi disparado no controle.
     * @returns {boolean} Um valor que indica se o formulário está válido.
     */
    validarFormulario_: function (botao) {
      var form = botao.form || botao;
      while (form.tagName.toLowerCase() !== 'form') {
        form = form.parentNode;
      }

      if (!form.checkValidity()) {
        return false;
      }

      return true;
    },

    /**
     * Insere o funcionário, se possível.
     * @param {Object} event O objeto do evento JavaScript.
     */
    inserirFuncionario: function (event) {
      if (!this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Funcionarios.inserir(this.funcionario)
        .then(function (resposta) {
          var url = '../Listas/LstFuncionario.aspx';

          window.location.assign(url);
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Atualiza o funcionário, se possível.
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
      * Cancela a edição ou cadastro de funcionário.
      */
    cancelar: function () {
      if (this.editando) {
        this.funcionario = this.clonar(this.funcionarioOriginal);
        this.editando = false;
        this.redirecionarParaListagem();
      } else if (this.inserindo) {
        this.redirecionarParaListagem();
      }
    },

    /**
      * Abre a tela de troca de senha do funcionário.
      */
    alterarSenha: function () {
      var url = '../Utils/TrocarSenha.aspx?IdFunc=' + this.funcionario.id;

      this.abrirJanela(150, 300, url);
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
          if (!vm.funcionario || !vm.configuracoes.podeCadastrarFuncionario) {
            vm.redirecionarParaListagem();
          }
          else if (vm.funcionario.adminSync && !vm.funcionario.permissoes.adminSync) {
            vm.redirecionarParaListagem();
          }
          else {
            vm.iniciarCadastroOuAtualizacao_(vm.funcionario);
            vm.inserindo = false;
            vm.editando = true;
          }
        });
    }
  },

  watch: {

    /**
   * Observador para a variável 'tipoFuncionarioAtual'.
   * Atualiza o funcionário com o ID do item selecionado.
   */
    tipoFuncionarioAtual: {
      handler: function (atual) {
        this.funcionario.idTipoFuncionario = atual ? atual.id : null;
      },
      deep: true
    },

    /**
   * Observador para a variável 'situacaoAtual'.
   * Atualiza o funcionário com o ID do item selecionado.
   */
    situacaoAtual: {
      handler: function (atual) {
        this.funcionario.situacao = atual ? atual.id : null;
      },
      deep: true
    },

    /**
   * Observador para a variável 'lojaAtual'.
   * Atualiza o funcionário com o ID do item selecionado.
   */
    lojaAtual: {
      handler: function (atual) {
        this.funcionario.idLoja = atual ? atual.id : null;
      },
      deep: true
    },
    /**
   * Observador para a variável 'situacaoAtual'.
   * Atualiza o funcionário com o ID do item selecionado.
   */
    estadoCivilAtual: {
      handler: function (atual) {
        this.funcionario.documentosEDadosPessoais.estadoCivil = atual ? atual.id : null;
      },
      deep: true
    }
  }
});
