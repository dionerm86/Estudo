Vue.component('integradores-itemesquemahistorico', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Dados do item do esquema do histórico.
     * @type {Object}
     */
    itemEsquema: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Integrador pai da operação.
     * @type {Object}
     **/
    integrador: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      podeExibirDetalhes: false,
      atualizando: false,
      itens: [],
      cabecalho: [],
      itensFiltro: [],
      tipoItem: '',
    }
  },

  methods: {

    /**
     * Altera a exibição dos detalhes.
     **/
    alterarExibicaoDetalhes: function () {
      this.podeExibirDetalhes = !this.podeExibirDetalhes;

      if (this.podeExibirDetalhes) {
        this.atualizar();
      }
    },

    /**
     * Carrega o cabeçalho com base no configuração do
     * do item do esquema do histório.
     **/
    carregarCabecalho: function () {

      var cabecalho1 = [];

      this.itemEsquema.identificadores.forEach(function (identificador) {
        cabecalho1.push(identificador.nome);
      });

      cabecalho1.push('Tipo');
      cabecalho1.push('Mensagem');
      cabecalho1.push('Data');

      this.cabecalho = cabecalho1;
    },

    /**
     * Carrega os itens do filtro.
     **/
    carregarItensFiltro: function () {

      this.itensFiltro = [];
      var self = this;
      this.itemEsquema.identificadores.forEach(function (identificador) {
        self.itensFiltro.push({
          identificador: identificador,
          valor: '',
        });
      });
    },

    /**
     * Atualiza os dados da item
     **/
    atualizar: function () {

      if (this.atualizando) {
        return;
      }

      this.itens = [];
      this.atualizando = true;
      var self = this;

      var identificadores = [];

      this.itensFiltro.forEach(function (itemFiltro) {
        identificadores.push(itemFiltro.valor);
      });

      Servicos.Integracao.Integradores.obterItensHistorico(this.integrador.nome, this.itemEsquema.nome, this.tipoItem, identificadores)
        .then(function (resposta) {
          self.atualizando = false;

          if (resposta.status == 200) {
            self.itens = resposta.data;
          }
        })
        .catch(function (erro) {
          self.executando = false;
          if (erro && erro.mensagem) {
            self.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },
  },

  mounted: function () {
    this.carregarCabecalho();
    this.carregarItensFiltro();
  },

  template: '#LstIntegradores-ItemEsquemaHistorico-template'
});
