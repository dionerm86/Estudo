﻿Vue.component('producao-pecas', {
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista()],
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
      controleAtualizacao: 0,
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
     * Recarrega os produtos de composição ao realizar uma alteração na lista.
     */
    listaAtualizada: function () {
      this.controleAtualizacao++;
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
    }
  },

  computed: {
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
