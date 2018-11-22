Vue.component('producao-pecas', {
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('id', 'desc')],
  props: {
    /**
     * Filtros selecionados para a lista de produção.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de produção.
     * @type {!Object}
     */
    configuracoes: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Função que as peças de produção para exibição no controle.
     * @param {!Object} filtro O filtro informado pelo controle lista-paginada.
     * @param {!number} pagina O número da página que está sendo exibida no controle lista-paginada.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na tela.
     * @param {string} ordenacao O campo pelo qual o resultado será ordenado.
     * @returns {Promise} Uma Promise com a busca dos itens de venda, de acordo com o filtro.
     */
    buscarPecas: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarFuncao
    },
  },

  data: function() {
    return {
      filhosEmExibicao: []
    }
  },

  methods: {
    /**
     * Função que busca busca os produtos filhos de peças de produção.
     * @param {!Object} filtro O filtro informado pelo controle lista-paginada.
     * @param {!number} pagina O número da página que está sendo exibida no controle lista-paginada.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na tela.
     * @param {string} ordenacao O campo pelo qual o resultado será ordenado.
     * @returns {Promise} Uma Promise com a busca das peças, de acordo com o filtro.
     */
    buscarFilhos: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Producao.obterProdutosComposicao(filtro.idPecaPai, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Ordena as leituras de produção de uma peça para exibição na lista de consulta.
     * @return {Object[]} As leituras ordenadas para exibição.
     */
    ordenarLeituras: function (peca) {
      if (!peca || !peca.leituras || !peca.leituras.length) {
        return [];
      }

      return this.setoresExibir
        .map(function (setor) {
          var leituraSetorPeca = peca.leituras.find(function (leitura) {
            return leitura.setor && leitura.setor.id === setor.id;
          });

          return leituraSetorPeca || {
            setor: {
              id: setor.id,
              nome: setor.nome,
              obrigatorio: true
            },
            data: null
          };
        }, this);
    },

    /**
     * Desfaz a última leitura de produção da peça informada.
     * @param {!Object} peca A peça de produção que terá a leitura desfeita.
     */
    desfazerUltimaLeituraPeca: function (peca) {
      if (!peca || !this.confirmar('Voltar peça', 'Confirma remoção desta peça desta situação?')) {
        return;
      }

      var vm = this;

      Servicos.Producao.desfazerUltimaLeituraPeca(peca.id)
        .then(function () {
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Abre o relatório de pedido de uma peça de produção.
     * @param {!Object} peca A peça de produção que terá o relatório de pedido aberto.
     */
    abrirRelatorioPedido: function (peca) {
      const url = '../../Relatorios/RelPedido.aspx?idPedido=' + peca.pedido.id;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Exibe o botão de relatório de pedido de uma peça de produção.
     * @param {!Object} peca A peça de produção que será verificada.
     */
    exibirRelatorioPedido: function (peca) {
      const producao = GetQueryString('producao') == '1';
      return !producao && this.exibirRelatorioPedidoPcp(peca) && peca.permissoes.relatorioPedido;
    },

    /**
     * Abre o relatório de pedido de uma peça de produção.
     * @param {!Object} peca A peça de produção que terá o relatório de pedido aberto.
     */
    abrirRelatorioPedidoPcp: function (peca) {
      const url = '../../Relatorios/RelBase.aspx?rel=PedidoPcp&idPedido=' + peca.pedido.id;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Exibe o botão de relatório de pedido de conferência de uma peça de produção.
     * @param {!Object} peca A peça de produção que será verificada.
     */
    exibirRelatorioPedidoPcp: function (peca) {
      return peca && peca.pedido && peca.permissoes;
    },

    /**
     * Exibe os anexos do pedido de uma peça.
     * @param {!Object} peca A peça de produção que terá os anexos de pedido exibidos.
     */
    abrirAnexosPedido(peca) {
      const url = '../CadFotos.aspx?id=' + peca.pedido.id + '&tipo=pedido';
      this.abrirJanela(600, 700, url);
    },

    /**
     * Exibe a tela para realizar a parada de produção da peça desejada.
     * @param {!Object} peca A peça de produção que poderá ser parada na produção.
     */
    pararPecaProducao: function (peca) {
      if (!peca || !peca.situacaoProducao || !peca.produtoPedido) {
        return;
      }

      const pergunta = 'Tem certeza que deseja '
        + (peca.situacaoProducao.pecaParada ? 'retornar esta peça para' : 'parar esta peça na')
        + ' produção?\n'
        + peca.produtoPedido.descricaoCompleta;

      if (!this.perguntar(pergunta)) {
        return;
      }

      this.abrirJanela(200, 405, '../../Utils/SetMotivoPararPecaProducao.aspx?vue=true&popup=true&idProdPedProducao=' + peca.id, null, true, false);
    },

    /**
     * Exibe os detalhes de reposição de uma peça de produção.
     * @param {!Object} peca A peça de produção que terá os detalhes de reposição exibidos.
     */
    abrirDetalhesReposicao: function (peca) {
      const url = '../../Utils/DetalhesReposicaoPeca.aspx?idProdPedProducao=' + peca.id;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Exibe o log de estorno de carregamentos para uma peça.
     * @param {!Object} peca A peça de produção que terá o log exibido.
     */
    abrirLogEstornoCarregamento: function (peca) {
      const url = '../../Utils/ShowEstornoCarregamento.aspx?idProdPedProducao=' + peca.id;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Indica externamente que os itens em exibição foram atualizados.
     * Também realiza a limpeza da variável que controla a exibição de filhos.
     */
    atualizouItens: function (numeroItens) {
      this.filhosEmExibicao.splice(0, this.filhosEmExibicao.length);
      this.$emit('atualizou-itens', numeroItens);
    },

    /**
     * Indica se os filhos (produtos de composição) estão sendo exibidos na lista.
     * @param {!number} indice O número da linha que está sendo verificada.
     * @returns {boolean} Verdadeiro, se os itens estiverem sendo exibidos.
     */
    exibindoFilhos: function (indice) {
      return this.filhosEmExibicao.indexOf(indice) > -1;
    },

    /**
     * Alterna a exibição dos filhos (produtos de composição).
     */
    alternarExibicaoFilhos: function (indice) {
      var i = this.filhosEmExibicao.indexOf(indice);

      if (i > -1) {
        this.filhosEmExibicao.splice(i, 1);
      } else {
        this.filhosEmExibicao.push(indice);
      }
    },

    /**
     * Retorna o número de colunas da lista paginada.
     * @type {number}
     */
    numeroColunasLista: function () {
      return this.$refs.lista.numeroColunas();
    },

    /**
     * Força a atualização da lista de peças, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    },
  },

  computed: {
    /**
     * Propriedade computada que retorna os setores para exibição ordenados.
     * @type {Object[]}
     */
    setoresExibir: function () {
      if (!this.configuracoes || !this.configuracoes.setoresExibicao) {
        return [];
      }

      return this.configuracoes.setoresExibicao
        .slice()
        .sort(function (a, b) {
          return a.ordem - b.ordem;
        });
    }
  },

  template: '#LstProducao-Pecas-template'
});
