const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.ExecutarTimeout],

  data: {
    inserindo: false,
    editando: false,
    pedido: {},
    pedidoOriginal: {},
    configuracoes: {},
    ambiente: null,
    datasEntrega: {},
    dataEntregaMinima: null,
    clienteAtual: {},
    obraAtual: {},
    deveCalcularDataEntregaMinima: false,
    forcarAtualizacaoDatasEntrega: null,
    clientePermiteAlterarLoja: true,
    lojaAtual: {},
    tipoPedidoAtual: {},
    tipoVendaAtual: {},
    parcelaAtual: {},
    funcionarioCompradorAtual: {},
    tipoEntregaAtual: {},
    formaPagamentoAtual: {},
    tipoCartaoAtual: {},
    vendedorAtual: {},
    transportadorAtual: {},
    medidorAtual: {},
    comissionadoAtual: {},
    descricaoObraAtual: ''
  },

  methods: {
    /**
     * Atualiza os dados do pedido e dos ambientes, se houver.
     */
    atualizarPedidoEAmbientes: function() {
      this.buscarPedido(this.pedido.id);
    },

    /**
     * Busca os dados de um pedido.
     * @param {?number} id O número do pedido.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    buscarPedido: function (id) {
      var vm = this;

      return Servicos.Pedidos.obterDetalhe(id)
        .then(function(resposta) {
          vm.pedido = resposta.data;
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Exibe a tela para inclusão de projeto no pedido.
     */
    incluirProjeto: function() {
      if (!this.pedido.entrega || !this.pedido.entrega.tipo || !this.pedido.entrega.tipo.id) {
        this.exibirMensagem('Projeto', 'Selecione o tipo de entrega antes de inserir um projeto.');
      }

      this.abrirJanela(
        screen.height,
        screen.width,
        '../Cadastros/Projeto/CadProjetoAvulso.aspx?IdPedido=' + this.pedido.id
          + '&IdAmbientePedido='
          + '&idCliente=' + this.pedido.cliente.id
          + '&tipoEntrega=' + this.pedido.entrega.tipo.id
      );
    },

    /**
     * Exibe a tela para inclusão de vários produtos para um pedido de mão-de-obra.
     */
    incluirVariosVidrosMaoDeObra: function() {
      this.abrirJanela(screen.height, screen.width, '../Utils/SetProdMaoObra.aspx?idPedido=' + this.pedido.id);
    },

    /**
     * Exibe a tela com os cálculos de rentabilidade do pedido.
     */
    abrirRentabilidade: function() {
      const url = '../Relatorios/Rentabilidade/VisualizacaoItemRentabilidade.aspx?tipo=pedido&id=' + this.pedido.id;
      this.abrirJanela(500, 700, url);
    },

    /**
     * Exibe a tela para cadastro dos textos para exibição no relatório do pedido.
     */
    abrirTextosPedido: function() {
      this.abrirJanela(500, 700, '../Utils/SelTextoPedido.aspx?idPedido=' + this.pedido.id);
    },

    /**
     * Recupera os vendedores para seleção no cadastro ou edição do pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterVendedores: function () {
      return Servicos.Funcionarios.obterVendedores(this.pedido.idVendedor);
    },

    /**
     * Recupera os funcionarioas que podem fazer pedido para seleção no cadastro ou edição do pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterFuncionariosCompradores: function () {
      return Servicos.Funcionarios.obterCompradores();
    },

    /**
     * Recupera os medidores para seleção no cadastro ou edição do pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterMedidores: function () {
      return Servicos.Funcionarios.obterMedidores();
    },

    /**
     * Recupera os transportadores para seleção no cadastro ou edição do pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterTransportadores: function() {
      return Servicos.Transportadores.obterParaControle();
    },

    /**
     * Recupera os comissionados para seleção no cadastro ou edição do pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterComissionados: function () {
      return Servicos.Comissionados.obterComissionados();
    },

    /**
     * Recupera os tipos de pedido para exibição no cadastro ou edição do pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterTiposPedido: function() {
      return Servicos.Pedidos.obterTiposPedidoPorFuncionario(this.pedidoMaoDeObra, this.pedidoProducao);
    },

    /**
     * Recupera os tipos de venda do cliente para exibição no cadastro ou edição do pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterTiposVendaCliente: function(filtro) {
      return Servicos.Pedidos.obterTiposVendaCliente(filtro);
    },

    /**
     * Recupera as formas de pagamento para exibição no cadastro ou edição do pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterFormasPagamento: function(filtro) {
      return Servicos.Pedidos.obterFormasPagamento(filtro);
    },

    /**
     * Recupera os tipos de cartão para exibição no cadastro ou edição do pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterTiposCartao: function() {
      return Servicos.TiposCartao.obterParaControle();
    },

    /**
     * Recupera os tipos de entrega de pedido para exibição no cadastro ou edição do pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterTiposEntrega: function() {
      return Servicos.Pedidos.obterTiposEntrega();
    },

    /**
     * Busca a lista de obras.
     * @param {?number} id O identificador da obra a ser pesquisada.
     * @param {?string} descricao A descrição da obra a ser pesquisada.
     * @returns {Promise} Uma Promise com os dados das obras encontradas.
     */
    obterObras: function (id, descricao) {
      return Servicos.Obras.obterParaControle(
        id,
        descricao,
        this.pedido.idCliente,
        this.pedido.id ? [this.pedido.id] : null,
        this.configuracoes.situacaoObraConfirmada,
        'PagamentoAntecipado'
      );
    },

    /**
     * Inicia o modo de edição do pedido.
     */
    editar: function() {
      this.iniciarCadastroOuAtualizacao_(this.pedido);
      this.inserindo = false;
      this.editando = true;
    },

    /**
     * Finaliza o pedido atual, se possível.
     */
    finalizar: function() {
      var vm = this;

      Servicos.Pedidos.finalizar(this.pedido.id)
        .then(function (resposta) {
          if (resposta.codigo === 300) {
            vm.enviarValidacaoFinanceiro(resposta.mensagem);
            return;
          }

          if (vm.pedido.gerarPedidoProducaoCorte) {
            vm.criarProducaoRevenda();
          } else {
            vm.abrirImpressaoPedido();
            vm.redirecionarParaListagem();
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Envia o pedido para validação no financeiro.
     */
    enviarValidacaoFinanceiro: function (mensagem) {
      var vm = this;

      if (vm.configuracoes.perguntarVendedorFinalizacaoFinanceiro) {
        if (!vm.perguntar('Pendências financeiras', 'Não foi possível finalizar o pedido. Erro: ' + mensagem + '. Deseja enviar esse pedido para finalização pelo Financeiro?')) {
          return;
        }
      } else {
        vm.exibirMensagem('Pendências financeiras', 'Houve um erro ao finalizar o pedido. Ele será disponibilizado para finalização pelo Financeiro.');
      }

      Servicos.Pedidos.enviarValidacaoFinanceiro(this.pedido.id, mensagem)
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
     * Cria um pedido de produção para o pedido de revenda inserido.
     */
    criarProducaoRevenda: function () {
      var vm = this;

      Servicos.Pedidos.criarProducaoRevenda(this.pedido.id)
        .then(function (resposta) {
          var url = '../Cadastros/CadPedido.aspx?idPedido=' + resposta.data.id;
          var byVend = GetQueryString('byVend');

          if (byVend === '1') {
            url += '&ByVend=1';
          }

          window.location.assign(url);
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Abre um popup com a impressão do pedido.
     */
    abrirImpressaoPedido: function () {
      const url = '../Relatorios/RelPedido.aspx?idPedido=' + this.pedido.id;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Coloca o pedido atual em conferência, se possível.
     */
    colocarEmConferencia: function() {
      if (!this.perguntar('Conferência', 'Mudar pedido para em conferência?')) {
        return false;
      }

      if (this.pedido.entrada === 0
        && !this.perguntar('Conferência', 'O sinal não foi inserido, clique em "Cancelar" para inserir o sinal do pedido ou em "OK" para continuar.')) {

        return false;
      }

      if (this.pedido.total === 0) {
        this.exibirMensagem('Conferência', 'O pedido não possui valor total, insira um produto "Conferência" com o valor total do Pedido.');
        return false;
      }

      var vm = this;

      Servicos.Pedidos.colocarEmConferencia(this.pedido.id)
        .then(function (resposta) {
          vm.redirecionarParaListagem();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        })
    },

    /**
     * Confirma o pedido e gera a sua conferência, redirecionando para a tela de conferência.
     * @param {Boolean} finalizarConferencia Define se a conferência deverá ser finalizada.
     */
    confirmarGerandoConferencia: function(finalizarConferencia) {
      var vm = this;

      Servicos.Pedidos.confirmarGerandoConferencia(this.pedido.id, finalizarConferencia)
        .then(function (resposta) {
          if (resposta.codigo === 300) {
            vm.enviarValidacaoFinanceiro(resposta.mensagem);
            return;
          }

          if (vm.pedido.gerarPedidoProducaoCorte) {
            vm.criarProducaoRevenda();
            return;
          }

          if (finalizarConferencia) {
            vm.abrirImpressaoPedido();
          } else {
            var url = '../Cadastros/CadPedidoEspelho.aspx?idPedido=' + resposta.data.id + '&finalizar=1';
            window.location.assign(url);
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
              vm.exibirMensagem('Erro', erro.mensagem);
              vm.redirecionarParaListagem();
          }
        });
    },

    /**
     * Insere o pedido, se possível.
     * @param {Object} event O objeto do evento JavaScript.
     */
    inserirPedido: function(event) {
      if (!this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Pedidos.inserir(this.pedido)
        .then(function(resposta) {
          var url = '../Cadastros/CadPedido.aspx?idPedido=' + resposta.data.id;
          var byVend = GetQueryString('byVend');

          if (byVend === '1') {
            url += '&ByVend=1';
          }

          window.location.assign(url);
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Atualiza o pedido, se possível.
     * @param {Object} event O objeto do evento JavaScript.
     */
    atualizarPedido: function(event) {
      if (!this.validarFormulario_(event.target)) {
        return;
      }

      var pedidoAtualizar = this.patch(this.pedido, this.pedidoOriginal);

      if (this.tipoVendaAtual && this.tipoVendaAtual.id === this.configuracoes.tipoVendaAPrazo) {
        pedidoAtualizar.formaPagamento = pedidoAtualizar.formaPagamento || {};
        pedidoAtualizar.formaPagamento.parcelas = pedidoAtualizar.formaPagamento.parcelas || {};
        pedidoAtualizar.formaPagamento.parcelas.detalhes = this.pedido.formaPagamento.parcelas.detalhes;
      }

      var vm = this;

      Servicos.Pedidos.atualizar(this.pedido.id, pedidoAtualizar)
        .then(function (resposta) {
          vm.atualizarPedidoEAmbientes();
          vm.cancelar();
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Recupera a data de entrega mínima e a data de entrega de fast delivery do pedido.
     * @param {boolean} forcarAtualizacao Indica se a data de entrega deve ser atualizada, mesmo se não fosse anteriormente.
     */
    calcularDatasEntrega: function(forcarAtualizacao) {
      if (!this.deveCalcularDataEntregaMinima) {
        return;
      }

      var vm = this;

      Servicos.Pedidos.obterDatasEntrega(
        this.pedido.id,
        this.pedido.idCliente,
        this.pedido.tipo,
        this.pedido.entrega.tipo,
        this.pedido.dataPedido
      )
        .then(function(resposta) {
          vm.datasEntrega = resposta.data;
          var dataMinima = vm.pedido.fastDelivery
            ? vm.datasEntrega.dataFastDelivery
            : vm.datasEntrega.dataMinimaCalculada;

          if (vm.pedido.entrega.data === null || dataMinima > vm.pedido.entrega.data || forcarAtualizacao) {
            vm.pedido.entrega.data = dataMinima;
          }

          vm.dataEntregaMinima = vm.datasEntrega.dataMinimaPermitida;
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Calcula as datas de entrega, garantindo que apenas uma chamada será efetuada caso
     * haja várias solicitações em um período próximo de tempo.
     * @param {boolean} forcarAtualizacao Indica se a data de entrega deve ser atualizada, mesmo se não fosse anteriormente.
     */
    iniciarCalculoDatasEntrega: function (forcarAtualizacao) {
      this.forcarAtualizacaoDatasEntrega =
        this.forcarAtualizacaoDatasEntrega === null
          ? forcarAtualizacao
          : this.forcarAtualizacaoDatasEntrega || forcarAtualizacao;

      this.executarTimeout('iniciarCalculoDatasEntrega', function () {
        this.calcularDatasEntrega(this.forcarAtualizacaoDatasEntrega);
        this.forcarAtualizacaoDatasEntrega = null;
      });
    },

    /**
     * Alterações no fast delivery, se for marcado, verifica se pode ser usado,
     * caso possa, preenche o campo pedido.dataEntrega com a data de fastDelivery, caso contrário, preenche com a data de entrega mínima calculada.
     */
    alterarFastDelivery: function () {
      if (this.pedido.fastDelivery.aplicado) {
        var vm = this;

        Servicos.Pedidos.verificarFastDelivery(this.pedido.id)
          .then(function() {
            if (vm.configuracoes.numeroDiasUteisDataEntregaPedido > 0) {
              vm.pedido.entrega.data = vm.datasEntrega.dataFastDelivery;
            }
          })
          .catch(function(erro) {
            vm.pedido.entrega.data = vm.datasEntrega.dataMinimaCalculada;

            if (erro && erro.mensagem) {
              vm.exibirMensagem('Erro', erro.mensagem);
            }
          });
      } 

      this.dataEntregaMinima = this.datasEntrega.dataMinimaCalculada;
    },

    /**
     * Alterações no pedido ao atualizar o vendedor.
     */
    alterarVendedor: function() {
      if (this.pedido && this.pedido.idVendedor) {
        var vm = this;

        Servicos.Funcionarios.obterDataTrabalho(this.pedido.idVendedor)
          .then(function(resposta) {
            vm.pedido.dataPedido = resposta.data.data;
          })
          .catch(function(erro) {
            if (erro && erro.mensagem) {
              vm.exibirMensagem('Erro', erro.mensagem);
            }
          });
      }
    },

    /**
     * Preenche o endereço que do cliente que serão entregues os produtos.
     */
    preencherEnderecoObra: function() {
      if (this.clienteAtual && this.clienteAtual.enderecoEntrega) {
        this.pedido.enderecoObra.cep = this.clienteAtual.enderecoEntrega.cep;
        this.pedido.enderecoObra.logradouro = this.clienteAtual.enderecoEntrega.logradouro;
        this.pedido.enderecoObra.bairro = this.clienteAtual.enderecoEntrega.bairro;
        this.pedido.enderecoObra.cidade = this.clienteAtual.enderecoEntrega.cidade;
      }
    },

    /**
     * Preenche valores padrões de acordo com o tipo de pedido.
     */
    preencherCamposPadroesPorTipoDePedido: function() {
      if (!this.inserindo) {
        return;
      }

      if (this.pedidoMaoDeObra) {
        this.pedido.tipo = this.configuracoes.tipoPedidoMaoDeObra;
      } else if (this.pedidoProducao) {
        this.pedido.tipo = this.configuracoes.tipoPedidoProducao;
        this.pedido.tipoVenda = this.configuracoes.tipoVendaAVista;
        this.pedido.cliente.id = this.configuracoes.idClienteProducao;
      } else {
        this.pedido.tipo = this.configuracoes.tipoPedidoPadrao;
      }
    },

    /**
     * Valida o preenchimento do desconto no pedido.
     */
    validarDesconto: function () {
      var vm = this;

      if (!this.pedido.tipoVenda) {
        return;
      }

      Servicos.Pedidos.validarDesconto(
        this.pedido.id,
        this.pedido.desconto.valor || 0,
        this.pedido.desconto.tipo,
        this.pedido.tipoVenda,
        this.pedido.formaPagamento ? this.pedido.formaPagamento.id : null,
        this.pedido.formaPagamento ? this.pedido.formaPagamento.idTipoCartao || 0 : null,
        this.pedido.formaPagamento && this.pedido.formaPagamento.parcelas ? this.pedido.formaPagamento.parcelas.id : null
      )
        .then(function (resposta) {
          if (vm.pedido.desconto.tipo !== 1) {
            vm.pedido.desconto.tipo = 1;
          }

          if (vm.pedido.desconto.valor !== resposta.data.descontoPermitido) {
            vm.pedido.desconto.valor = resposta.data.descontoPermitido;
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }

          vm.pedido.desconto.valor = 0;
        });
    },

    /**
     * Cancela a edição ou cadastro de pedido.
     */
    cancelar: function() {
      if (this.editando) {
        this.pedido = this.clonar(this.pedidoOriginal);
        this.editando = false;
        this.deveCalcularDataEntregaMinima = false;
      } else if (this.inserindo) {
        this.redirecionarParaListagem();
      }
    },

    /**
     * Redireciona a tela atual para a tela de listagem correspondente.
     */
    redirecionarParaListagem: function() {
      var idRelDinamico = GetQueryString('idRelDinamico');
      var byVend = GetQueryString('byVend');
      var url = '../Listas/LstPedidos.aspx';

      if (idRelDinamico) {
        url = '../Relatorios/Dinamicos/ListaDinamico.aspx?id=' + idRelDinamico;
      } else if (byVend === '1') {
        url += '?ByVend=1';
      }

      window.location.assign(url);
    },

    /**
     * Inicia um cadastro ou edição de produto, criando o objeto para 'bind'.
     * @param {Object} item O produto que será usado como base, no caso de edição.
     */
    iniciarCadastroOuAtualizacao_: function(item) {
      this.pedidoOriginal = item ? this.clonar(item) : {};

      this.clienteAtual = {
        id: item && item.cliente ? item.cliente.id : null,
        nome: item && item.cliente ? item.cliente.nome : null
      };

      this.obraAtual = {
        id: item && item.obra ? item.obra.id : null,
        idCliente: item && item.obra ? item.obra.idCliente : null,
        descricao: item && item.obra ? item.obra.descricao : null,
        saldo: item && item.obra ? item.obra.saldo : null,
        totalPedidosAbertosObra: item && item.obra ? item.obra.totalPedidosAbertosObra : null
      };

      this.lojaAtual = {
        id: item && item.loja ? item.loja.id : null
      };

      this.tipoPedidoAtual = {
        id: item && item.tipo ? item.tipo.id : null
      };

      this.tipoVendaAtual = {
        id: item && item.tipoVenda ? item.tipoVenda.id : null
      };

      this.parcelaAtual = {
        id: item && item.formaPagamento && item.formaPagamento.parcelas ? item.formaPagamento.parcelas.id : null,
        dias: item && item.formaPagamento && item.formaPagamento.parcelas ? item.formaPagamento.parcelas.dias : null,
        numeroParcelas:
          item && item.formaPagamento && item.formaPagamento.parcelas
            ? item.formaPagamento.parcelas.numeroParcelas
            : null
      };

      this.funcionarioCompradorAtual = {
        id: item && item.funcionarioComprador ? item.funcionarioComprador.id : null
      };

      this.tipoEntregaAtual = item && item.entrega ? item.entrega.tipo : null;

      this.formaPagamentoAtual = {
        id: item && item.formaPagamento ? item.formaPagamento.id : null
      };

      this.tipoCartaoAtual = {
        id: item && item.formaPagamento ? item.formaPagamento.idTipoCartao : null
      };

      this.vendedorAtual = {
        id: item && item.vendedor ? item.vendedor.id : null
      };

      this.transportadorAtual = {
        id: item && item.transportador ? item.transportador.id : null
      };

      this.medidorAtual = {
        id: item && item.medidor ? item.medidor.id : null
      };

      this.comissionadoAtual = {
        id: item && item.comissionado ? item.comissionado.id : null
      };

      this.pedido = {
        id: item && item.id ? item.id : null,
        idCliente: item && item.cliente ? item.cliente.id : null,
        idLoja: item && item.loja ? item.loja.id : null,
        idObra: item && item.obra ? item.obra.id : null,
        nomeCliente: item && item.cliente ? item.cliente.nome : null,
        podeEditarCliente: item && item.permissoes ? item.permissoes.alterarCliente : true,
        dataPedido: item && item.DataPedido ? new Date(item.dataPedido) : new Date(),
        fastDelivery: item && item.fastDelivery ? item.fastDelivery.aplicado : null,
        codigoPedidoCliente: item ? item.codigoPedidoCliente : null,
        deveTransferir: item ? item.deveTransferir : null,
        tipo: item && item.tipo ? item.tipo.id : null,
        tipoVenda: item && item.tipoVenda ? item.tipoVenda.id : null,
        idVendedor: item && item.vendedor ? item.vendedor.id : null,
        idMedidor: item && item.medidor ? item.medidor.id : null,
        idFuncionarioComprador: item && item.funcionarioComprador ? item.funcionarioComprador.id : null,
        idTransportador: item && item.transportador ? item.transportador.id : null,
        idPedidoRevenda: item ? item.idPedidoRevenda : null,
        gerarPedidoCorte: item ? item.gerarPedidoCorte : null,
        entrega: {
          tipo: item && item.entrega && item.entrega.tipo ? item.entrega.tipo.id : null,
          data: item && item.entrega ? item.entrega.data : null,
          valor: item && item.entrega ? item.entrega.valor : null
        },
        formaPagamento: {
          id: item && item.formaPagamento ? item.formaPagamento.id : null,
          idTipoCartao: item && item.formaPagamento ? item.formaPagamento.idTipoCartao : null,
          parcelas: {
            id: item && item.formaPagamento && item.formaPagamento.parcelas ? item.formaPagamento.parcelas.id : null,
            dias:
              item && item.formaPagamento && item.formaPagamento.parcelas ? item.formaPagamento.parcelas.dias : null,
            numeroParcelas:
              item && item.formaPagamento && item.formaPagamento.parcelas
                ? item.formaPagamento.parcelas.numeroParcelas
                : 3,
            detalhes:
              item && item.formaPagamento && item.formaPagamento.parcelas && item.formaPagamento.parcelas.detalhes
                ? item.formaPagamento.parcelas.detalhes.slice()
                : []
          }
        },
        desconto: {
          tipo: item && item.desconto ? item.desconto.tipo : null,
          valor: item && item.desconto ? item.desconto.valor : null
        },
        acrescimo: {
          tipo: item && item.acrescimo ? item.acrescimo.tipo : null,
          valor: item && item.acrescimo ? item.acrescimo.valor : null
        },
        sinal: {
          id: item && item.sinal ? item.sinal.id : null,
          valor: item && item.sinal ? item.sinal.valor : null
        },
        comissionado: {
          id: item && item.comissionado ? item.comissionado.id : null,
          percentualComissao: item && item.comissionado ? item.comissionado.percentualComissao : null
        },
        observacao: item ? item.observacao : null,
        observacaoLiberacao: item ? item.observacaoLiberacao : null,
        enderecoObra: {
          logradouro: item && item.enderecoObra ? item.enderecoObra.logradouro : null,
          bairro: item && item.enderecoObra ? item.enderecoObra.bairro : null,
          cidade: item && item.enderecoObra ? item.enderecoObra.cidade : null,
          cep: item && item.enderecoObra ? item.enderecoObra.cep : null
        },
        total: item ? item.total : null,
        valorEntrada: item && item.sinal ? item.sinal.valor : null,
        textoSinal: item ? item.textoSinal : null
      };

      if (this.inserindo) {
        this.tipoEntregaAtual.id = this.configuracoes.tipoEntregaPadrao;
        this.vendedorAtual.id = usuarioLogado.id;
      }

      this.deveCalcularDataEntregaMinima = true;
      this.calcularDatasEntrega(false);
      this.alterarVendedor();
      this.preencherCamposPadroesPorTipoDePedido();
    },

    /**
     * Função que indica se o formulário de pedido possui valores válidos de acordo com os controles.
     * @param {Object} botao O botão que foi disparado no controle.
     * @returns {boolean} Um valor que indica se o formulário está válido.
     */
    validarFormulario_: function(botao) {
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
     * Função que indica se o pedido pode ser inserido ou editado.
     * @returns {boolean} Um valor que indica se o formulário está válido.
     */
    validarBase_: function() {
      if (this.obraAtual && this.obraAtual.idCliente !== this.pedido.idCliente) {
        this.exibirMensagem('Clientes diferentes', 'O cliente da obra selecionada é diferente do cliente do pedido.');
        return false;
      }

      return true;
    },

    /**
     * Função que indica se o usuário pode ou não editar pedido do tipo de Reposição
     * @returns {boolean} Um valor que indica se o o pedido pode ser editado pelo usuário de acordo com a pemissão.
     */
    verificarPermissaoEdicaoReposicao: function () {
      return this.pedido.tipoVenda === this.configuracoes.tipoVendaReposicao
        && this.configuracoes.emitirPedidoReposicao;
    },

    /**
     * Função que indica se o usuário pode ou não editar pedido do tipo de Reposição
     * @returns {boolean} Um valor que indica se o o pedido pode ser editado pelo usuário de acordo com a pemissão.
     */
    verificarPermissaoEdicaoGarantia: function () {
      return this.pedido.tipoVenda === this.configuracoes.tipoVendaGarantia
        && this.configuracoes.emitirPedidoGarantia;
    }
  },

  mounted: function() {
    var id = GetQueryString('idPedido');
    var vm = this;

    Servicos.Pedidos.obterConfiguracoesDetalhe(id || 0)
      .then(function(resposta) {
        vm.configuracoes = resposta.data;

        if (!id) {
          vm.inserindo = true;
          vm.iniciarCadastroOuAtualizacao_();
        }
      })
      .catch(function(erro) {
        if (erro && erro.mensagem) {
          vm.exibirMensagem('Erro', erro.mensagem);
        }
      });

    if (id) {
      this.buscarPedido(id)
        .then(function () {
          if (!vm.pedido || !vm.pedido.permissoes.podeEditar) {
            vm.redirecionarParaListagem();
          } else if (!vm.verificarPermissaoEdicaoReposicao() || !vm.verificarPermissaoEdicaoGarantia()) {
            vm.exibirMensagem('Bloqueio', 'Você não possui permissão para alterar pedidos do tipo ' + vm.pedido.tipoVenda === this.configuracoes.tipoVendaGarantia ? 'Garantia' : 'Reposição' + 'Contate o administrador');
            vm.redirecionarParaListagem();
          }
        });
    }
  },

  computed: {
    /**
     * Propriedade computada que indica se o pedido é de mão-de-obra.
     * @type {boolean}
     */
    pedidoMaoDeObra: function() {
      return (
        (this.pedido && this.pedido.tipo === this.configuracoes.tipoPedidoMaoDeObra) || GetQueryString('maoObra') === '1'
      );
    },

    /**
     * Propriedade computada que indica se o pedido é de produção.
     * @type {boolean}
     */
    pedidoProducao: function() {
      return (
        (this.pedido && this.pedido.tipo === this.configuracoes.tipoPedidoProducao) || GetQueryString('producao') === '1'
      );
    },

    /**
     * Propriedade computada que indica se o pedido é de mão-de-obra.
     * @type {boolean}
     */
    pedidoMaoDeObraEspecial: function() {
      return (
        (this.pedido && this.pedido.tipo === this.configuracoes.tipoPedidoMaoDeObraEspecial) || GetQueryString('maoObraEspecial') === '1'
      );
    },

    /**
     * Propriedade computada que indica se o pedido é de produção para corte.
     * @type {boolean}
     */
    pedidoProducaoCorte: function() {
      return this.pedidoProducao && this.pedido.idPedidoRevenda > 0;
    },

    /**
     * Propriedade computada que retorna o total a ser usado para o cálculo das parcelas.
     * @type {boolean}
     */
    totalParaCalculoParcelas: function() {
      return this.pedido.total - (this.pedido.sinal.valor || 0);
    },

    /**
     * Propriedade computada que retorna o filtro de tipos de venda por cliente.
     * @type {filtroTiposVendaCliente}
     *
     * @typedef filtroTiposVendaCliente
     * @property {?number} idCliente O ID do cliente do pedido.
     */
    filtroTiposVendaCliente: function() {
      return {
        idCliente: (this.pedido || {}).idCliente || 0
      };
    },

    /**
     * Propriedade computada que retorna o filtro de tipos de venda por cliente.
     * @type {filtroFormasPagamento}
     *
     * @typedef filtroFormasPagamento
     * @property {?number} idCliente O ID do cliente do pedido.
     * @property {?number} tipoVenda O tipo de venda do pedido.
     */
    filtroFormasPagamento: function() {
      return {
        idCliente: (this.pedido || {}).idCliente || 0,
        tipoVenda: (this.pedido || {}).tipoVenda || ''
      };
    },

    /**
     * Propriedade computada que retorna se o controle de número de parcelas deve ser criado na tela
     */
    vIfNumeroParcelas: function() {
      return this.editando && this.pedido && this.pedido.tipoVenda === this.configuracoes.tipoVendaAPrazo;
    },

    /**
     * Propriedade computada que retorna se o controle de parcelas deve ser criado na tela
     */
    vIfControleParcelas: function() {
      return this.vIfNumeroParcelas;
    },

    /**
     * Propriedade computada que retorna se o controle de forma de pagamento deve ser criado na tela.
     */
    vIfFormaPagamento: function() {
      return this.pedido.tipoVenda === this.configuracoes.tipoVendaAPrazo
        || this.pedido.tipoVenda === this.configuracoes.tipoVendaReposicao
        || this.pedido.tipoVenda === this.configuracoes.tipoVendaGarantia
        || (this.configuracoes.usarControleDescontoFormaPagamentoDadosProduto
          && this.pedido.tipoVenda === this.configuracoes.tipoVendaAVista);
    },

    /**
     * Propriedade computada que retorna se será exibido o valor da entrada para preenchimento ou exibição
     */
    vIfValorEntrada: function() {
      return (
        this.pedido
          && (!this.pedido.tipoVenda
            || this.pedido.tipoVenda === this.configuracoes.tipoVendaAPrazo
            || (this.pedido.tipoVenda === this.configuracoes.tipoVendaAVista
              && this.configuracoes.usarLiberacaoPedido))
      );
    },

    /**
     * Propriedade computada que retorna se será exibido o valor da entrada para preenchimento
     */
    vIfCampoEntrada: function() {
      return this.pedido.sinal === null || this.pedido.sinal.id === 0;
    },

    /**
     * Propriedade computada que retorna se o desconto será exibido
     */
    vIfCampoDesconto: function() {
      return (
        this.pedido.tipoVenda === this.configuracoes.tipoVendaAVista ||
        !this.configuracoes.descontoApenasAVista ||
        (this.descontoPedidoUmaParcela && this.pedido.formaPagamento.parcelas.numeroParcelas === 1)
      );
    },

    /**
     * Propriedade computada que retorna se a obra será exibida na tela.
     */
    vIfTipoVendaObra: function() {
      return this.pedido.tipoVenda === this.configuracoes.tipoVendaObra
    },

    /**
     * Propriedade computada que retorna se o desconto será desabilitado
     */
    disabledCampoDesconto: function() {
      return !(
        !this.configuracoes.descontoApenasAVista
          || this.pedido.tipoVenda === this.configuracoes.tipoPedidoVenda
          || this.pedido.formaPagamento.parcelas.parcelaAVista
      );
    },

    /**
     * Propriedade computada que retorna se o ajuste de layout para o transportador será exibido.
     */
    vIfAjusteLayoutTransportador: function () {
      var exibir = (this.vIfFormaPagamento && this.vIfValorEntrada)
        || this.vIfTipoVendaObra;

      if (this.configuracoes.exibirValorFretePedido) {
        exibir = !exibir;
      }

      return exibir;
    },

    /**
     * Propriedade computada que retorna se o ajuste de layout para a observação será exibido.
     */
    vIfAjusteLayoutObservacao: function () {
      if (!this.pedido) {
        return false;
      }

      return !this.configuracoes.exibirDeveTransferir
        && !this.pedido.funcionarioComprador
        && !this.pedido.transportador;
    }
  },

  watch: {
    /**
     * Observador para a variável 'clienteAtual'.
     * Altera os dados no pedido referentes ao cliente.
     */
    clienteAtual: {
      handler: function(atual, anterior) {
        if (!this.pedido) {
          return;
        }

        this.pedido.desconto.tipo = 2;
        this.pedido.desconto.valor = 0;
        this.pedido.comissionado.id = null;
        this.pedido.comissionado.percentualComissao = 0;
        this.preencherEnderecoObra();

        if (atual && (this.inserindo || (anterior && anterior.id && atual.id !== anterior.id))) {
          this.pedido.idCliente = atual.id;

          if (atual.comissionado) {
            this.comissisonadoAtual = atual.comissionado;
          }

          if (atual.idVendedor) {
            this.vendedorAtual = {
              id: atual.idVendedor
            };
          }

          if (this.configuracoes.usarComissaoNoPedido && atual.percentualComissao) {
            this.pedido.percentualComissao = atual.percentualComissao;
          }

          if (!anterior || (atual.id !== anterior.id && atual.idTransportador)) {
            this.transportadorAtual = {
              id: atual.idTransportador
            };
          }

          if (atual.idLoja) {
            this.lojaAtual = {
              id: atual.idLoja
            };
          }

          if (atual.tipoVenda) {
            this.tipoVendaAtual = {
              id: atual.tipoVenda
            };
          }

          if (atual.idFormaPagamento) {
            this.formaPagamentoAtual = {
              id: atual.idFormaPagamento
            };
          }

          if (atual.parcela) {
            this.parcelaAtual = atual.parcela;
          }

          if (atual.rota && atual.rota.entregaBalcao && this.configuracoes.tipoEntregaBalcao) {
            this.pedido.entrega.tipo = this.configuracoes.tipoEntregaBalcao;
          }

          if (atual.rota && atual.rota.entrega && !atual.rota.entregaBalcao && this.configuracoes.tipoEntregaEntrega) {
            this.pedido.entrega.tipo = this.configuracoes.tipoEntregaEntrega;
          }

          this.clientePermiteAlterarLoja = !atual.idLoja;
          this.pedido.deveTransferir = !this.clientePermiteAlterarLoja;

          if (atual.observacaoLiberacao) {
            this.pedido.observacaoLiberacao = atual.observacaoLiberacao;
          }
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'pedido.idCliente'.
     * Altera a data de entrega do pedido e a data mínima do controle de data de entrega.
     */
    'pedido.idCliente': {
      handler: function (atual, anterior) {
        if (!this.pedido) {
          return;
        }

        this.iniciarCalculoDatasEntrega(anterior && atual && anterior.id !== atual);
      }
    },

    /**
     * Observador para a variável 'pedido.tipo'.
     * Altera a data de entrega do pedido e a data mínima do controle de data de entrega.
     */
    'pedido.tipo': {
      handler: function (atual, anterior) {
        if (!this.pedido) {
          return;
        }

        this.iniciarCalculoDatasEntrega(anterior && atual && anterior.id !== atual);
      }
    },

    /**
     * Observador para a variável 'pedido.tipoEntrega'.
     * Altera a data de entrega do pedido e a data mínima do controle de data de entrega.
     */
    'pedido.tipoEntrega': {
      handler: function (atual, anterior) {
        if (!this.pedido) {
          return;
        }

        this.iniciarCalculoDatasEntrega(anterior && atual && anterior.id !== atual.id);
        this.preencherEnderecoObra();
      }
    },

    /**
     * Observador para a variável 'pedido.dataPedido'.
     * Altera a data de entrega do pedido e a data mínima do controle de data de entrega.
     */
    'pedido.dataPedido': {
      handler: function (atual, anterior) {
        if (!this.pedido) {
          return;
        }

        this.iniciarCalculoDatasEntrega(anterior && atual && anterior.format('dd/mm/yyyy') != atual.format('dd/mm/yyyy'));
      }
    },

    /**
     * Observador para a variável 'pedido.fastDelivery'.
     * Altera a data de entrega do pedido com base no 'Fast Delivery'.
     */
    'pedido.fastDelivery': function() {
      this.alterarFastDelivery();
    },

    /**
     * Observador para a variável 'pedido.desconto'.
     * Executa o método de validação de desconto.
     */
    'pedido.desconto': {
      handler: function () {
        this.validarDesconto();
      },
      deep: true
    },

    /**
     * Observador para a variável 'lojaAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
    lojaAtual: {
      handler: function(atual) {
        this.pedido.idLoja = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoPedidoAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
    tipoPedidoAtual: {
      handler: function(atual) {
        this.pedido.tipo = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoVendaAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
    tipoVendaAtual: {
      handler: function(atual, anterior) {
        this.pedido.tipoVenda = atual ? atual.id : null;
        this.obraAtual = null;

        if (atual && anterior && !this.vIfNumeroParcelas) {
          this.pedido.parcelas = null;
        }

        this.validarDesconto();
      },
      deep: true
    },

    /**
     * Observador para a variável 'parcelaAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
    parcelaAtual: {
      handler: function(atual) {
        this.pedido.formaPagamento.parcelas = atual;
        this.validarDesconto();
      },
      deep: true
    },

    /**
     * Observador para a variável 'funcionarioCompradorAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
    funcionarioCompradorAtual: {
      handler: function(atual) {
        this.pedido.idFuncionarioComprador = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoEntregaAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
    tipoEntregaAtual: {
      handler: function(atual) {
        this.pedido.entrega.tipo = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'formaPagamentoAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
    formaPagamentoAtual: {
      handler: function(atual) {
        this.pedido.formaPagamento.id = atual ? atual.id : null;

        this.validarDesconto();
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoCartaoAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
    tipoCartaoAtual: {
      handler: function(atual) {
        this.pedido.formaPagamento.idTipoCartao = atual ? atual.id : null;
        this.validarDesconto();
      },
      deep: true
    },

    /**
     * Observador para a variável 'vendedorAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
    vendedorAtual: {
      handler: function(atual) {
        this.pedido.idVendedor = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'transportadorAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
    transportadorAtual: {
      handler: function(atual) {
        this.pedido.idTransportador = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'medidorAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
    medidorAtual: {
      handler: function(atual) {
        this.pedido.idMedidor = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'comissionadoAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
    comissionadoAtual: {
      handler: function (atual) {
        if (this.pedido.comissisonado) {
          this.pedido.comissisonado.id = atual ? atual.id : null;
        }
      },
      deep: true
    },


    /**
     * Observador para a variável 'obraAtual'.
     * Atualiza o pedido com o ID do item selecionado.
     */
    obraAtual: {
      handler: function(atual) {
        this.pedido.idObra = atual ? atual.id : null;
      },
      deep: true
    }
  }
});
