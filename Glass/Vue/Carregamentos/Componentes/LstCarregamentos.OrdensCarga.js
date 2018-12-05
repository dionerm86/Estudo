Vue.component('carregamentos-ordens-carga', {
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('id', 'desc')],
  props: {
    /**
     * Filtros selecionados para a lista de ordem de carga.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de carregamentos.
     * @type {!Object}
     */
    configuracoes: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      totalizadores: {}
    }
  },

  methods: {
    /**
     * Função que busca busca as ordens de carga do carregamento.
     * @returns {Promise} Uma Promise com a busca dos itens, de acordo com o filtro.
     */
    buscarOrdensCarga: function () {
      return Servicos.Carregamentos.OrdensCarga.obterParaListaCarregamento(this.filtro.idCarregamento);
    },

    /**
     * Função que calcula os totais da lista de ordens de carga
     */
    calcularTotais: function () {
      var itens = this.$refs.lista.itens;

      this.totalizadores = {
        pesoTotal: 0,
        pesoPendente: 0,
        metroQuadradoTotal: 0,
        metroQuadradoPendente: 0,
        quantidadePecasTotal: 0,
        quantidadePecasPendente: 0,
        quantidadeVolumes: 0
      };

      for (var i in itens) {
        this.totalizadores.pesoTotal += itens[i].peso.total;
        this.totalizadores.pesoPendente += itens[i].peso.pendente;
        this.totalizadores.metroQuadradoTotal += itens[i].metroQuadrado.total;
        this.totalizadores.metroQuadradoPendente += itens[i].metroQuadrado.pendente;
        this.totalizadores.quantidadePecasTotal += itens[i].quantidadePecas.total;
        this.totalizadores.quantidadePecasPendente += itens[i].quantidadePecas.pendente;
        this.totalizadores.quantidadeVolumes += itens[i].quantidadeVolumes;
      }
    },

    /**
     * Exibe um relatório da ordem de carga, aplicando os filtros da tela.
     * @param {Object} ordemCarga A ordem de carga a ser impressa.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirRelatorioOrdemCarga: function (ordemCarga, exportarExcel) {
      if (!ordemCarga) {
        throw new Error('Ordem de carga é obrigatória.');
      }

      var url = '../Relatorios/RelBase.aspx?rel=OrdemCarga&idOrdemCarga=' + ordemCarga.id;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Desassocia uma ordem de carga de um carregamento.
     * @param {Object} ordemCarga O objeto da ordem de carga a ser desassociada.
     */
    desassociarOrdemCarga: function (ordemCarga) {
      if (!this.perguntar('Confirmação', 'Deseja realmente remover esta OC do carregamento?')) {
        return;
      }

      var vm = this;

      Servicos.Carregamentos.OrdensCarga.desassociarDoCarregamento(ordemCarga.id, ordemCarga.idCarregamento)
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
     * Indica externamente que os itens em exibição foram atualizados.
     */
    atualizouItens: function (numeroItens) {
      this.$emit('atualizou-itens', numeroItens);
      this.calcularTotais();
    },

    /**
     * Força a atualização da lista de ordens de carga, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    },
  },

  template: '#LstCarregamentos-OrdensCarga-template'
});
