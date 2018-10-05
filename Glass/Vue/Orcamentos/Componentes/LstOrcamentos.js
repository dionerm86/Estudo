const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca os orçamentos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca de orçamentos.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de orçamentos.
     */
    atualizarOrcamentos: function(filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Orcamentos.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna o link para a tela de edição de orçamentos.
     * @param {Object} item O orçamento que será usado para construção do link.
     * @returns {string} O link que redireciona para a edição de orçamentos.
     */
    obterLinkEditarOrcamento: function(item) {
      return '../Cadastros/CadOrcamento.aspx?idorca=' + item.id;
    },

    /**
     * Exibe um relatório de orçamento, de acordo com o tipo desejado.
     * @param {Object} item O orçamento que será exibido.
     */
    abrirRelatorio: function(item) {
      this.abrirJanela(600, 800, '../Relatorios/RelOrcamento.aspx?idOrca=' + item.id);
    },

    /**
     * Exibe o relatório de projetos de um orçamento.
     * @param {Object} item O orçamento que será exibido.
     */
    abrirRelatorioProjeto: function(item) {
      this.abrirJanela(600, 800, '../Cadastros/Projeto/ImprimirProjeto.aspx?idOrcamento=' + item.id);
    },

    /**
     * Exibe o relatório de memória de cálculo de um orçamento.
     * @param {Object} item O pedido que será exibido.
     */
    abrirRelatorioMemoriaCalculo: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=MemoriaCalculoOrcamento&idOrca=' + item.id);
    },

    /**
     * Exibe a tela para anexos das medições associadas ao orçamento.
     * @param {Object} item O orçamento que terá medições com itens anexados.
     */
    abrirAnexosMedicao: function(item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.idsMedicao + '&tipo=medicao');
    },

    /**
     * Exibe a tela para anexos do orçamento.
     * @param {Object} item O orçamento que com itens anexados.
     */
    abrirAnexos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=orcamento');
    },

    /**
     * Retorna o link para a tela de sugestões de clientes.
     * @param {Object} item O orçamento que terá sugestões feitas.
     * @returns {string} O link que redireciona para a sugestão de clientes.
     */
    obterLinkSugestoes: function(item) {
      return '../Listas/LstSugestaoCliente.aspx?idOrcamento=' + item.id;
    },

    /**
     * Retorna o link para a tela de cadastro de orçamento, de acordo com a listagem sendo exibida.
     * @returns {string} O link para a tela de cadastro de orçamento.
     */
    obterLinkInserirOrcamento: function() {
      return '../Cadastros/CadOrcamento.aspx';
    },

    /**
     * Gera um novo pedido a partir do orçamento.
     * @param {Object} item O orçamento a ser utilizado para geração do pedido.
     */
    gerarPedido: function (item) {
      if (!this.perguntar('Gerar pedido', 'Tem certeza que deseja gerar um pedido para este orçamento?')) {
        return;
      }

      if (!item.cliente.id || item.cliente.id === 0) {
        this.exibirMensagem('Erro', 'Para gerar pedido é preciso que o orçamento tenha um cliente associado. Edite o orçamento e informe um cliente cadastrado.');
        return;
      }

      var vm = this;

      Servicos.Orcamentos.gerarPedido(item.id)
        .then(function (resposta) {
          window.location.assign('../Cadastros/CadPedido.aspx?idPedido=' + resposta.data.id);
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem(erro.mensagem);
          }
        });
    },

    /**
     * Gera pedidos agrupados a partir do orçamento.
     * @param {Object} item O orçamento a ser utilizado para geração do pedido.
     */
    gerarPedidosAgrupados: function (item) {
      if (!this.perguntar('Gerar pedido', 'Tem certeza que deseja gerar pedidos para este orçamento?')) {
        return;
      }

      if (!item.cliente.id || item.cliente.id === 0) {
        this.exibirMensagem('Erro', 'Para gerar pedidos é preciso que o orçamento tenha um cliente associado. Edite o orçamento e informe um cliente cadastrado.');
        return;
      }

      var vm = this;

      Servicos.Orcamentos.gerarPedidosAgrupados(item.id)
        .then(function (resposta) {
          window.location.assign('../Listas/LstPedidos.aspx');
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem(erro.mensagem);
          }
        });
    },

    /**
     * Envia email para o cliente com o orçamento.
     * @param {Object} item O orçamento que será enviado por email.
     */
    enviarEmail: function (item) {
      if (!this.perguntar('Enviar e-mail', 'Deseja realmente enviar o e-mail do orçamento?')) {
        return;
      }

      var vm = this;

      Servicos.Orcamentos.enviarEmail(item.id)
        .then(function (resposta) {
          vm.exibirMensagem('Enviar e-mail', 'O e-mail foi adicionado na fila para ser enviado.');
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Força a atualização da lista de orçamentos, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  mounted: function() {
    var vm = this;

    Servicos.Orcamentos.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
