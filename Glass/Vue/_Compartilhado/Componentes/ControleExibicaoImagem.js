﻿Vue.component('controle-exibicao-imagem', {
  mixins: [Mixins.ExecutarTimeout],
  props: {
    /**
     * Identificador do item que contém a imagem.
     * @type {?number}
     */
    idItem: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Tipo do item que contém a imagem.
     * @type {!string}
     */
    tipoItem: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarValores('Produto')
    },

    /**
     * Tamanho máximo permitido para a imagem.
     * @type {tamanhoMaximo}
     *
     * @typedef {Object} tamanhoMaximo
     * @property {?number} altura A altura máxima da imagem.
     * @property {?number} largura A largura máxima da imagem.
     */
    tamanhoMaximo: {
      required: false,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarObjetoOuVazio
    }
  },

  data: function () {
    return {
      uuid: null,
      urlImagem: null,
      legenda: null
    }
  },

  methods: {
    /**
     * Recupera os dados para exibição da imagem do item.
     */
    buscarDadosImagem: function () {
      this.executarTimeout('buscarDadosImagem', function () {
        this.urlImagem = null;
        this.legenda = null;

        var vm = this;

        Servicos.Imagens.obterDados(this.idItem, this.tipoItem)
          .then(function (resposta) {
            vm.urlImagem = resposta.data.urlImagem;
            vm.legenda = resposta.data.legenda;
          })
          .catch(function (erro) {
            if (erro && erro.mensagem) {
              vm.exibirMensagem('Erro ao buscar imagem', erro.mensagem);
            }
          });
      });
    },

    /**
     * Exibe um popup com a imagem em tamanho real.
     */
    abrirJanelaImagem: function () {
      const urlFormatada = this.urlImagem
        .replace(/..\/..\//g, '../')
        .replace(/\?/g, '$')
        .replace(/&/g, '@')
        .replace(/\\/g, '!')

      this.abrirJanela(600, 800, '../Utils/ShowFoto.aspx?path=' + urlFormatada);
    },
  },

  mounted: function () {
    this.buscarDadosImagem();
  },

  computed: {
    /**
     * Propriedade computada com o estilo para a imagem.
     * @type {Object}
     */
    estiloImagem: function() {
      if (!this.tamanhoMaximo) {
        return null;
      }

      return {
        maxHeight: this.tamanhoMaximo.altura,
        maxWidth: this.tamanhoMaximo.largura
      };
    }
  },

  watch: {
    /**
     * Observador para a propriedade 'idItem'.
     * Busca novamente os dados para exibição da imagem.
     */
    idItem: function () {
      this.buscarDadosImagem();
    }
  },

  template: '#ControleExibicaoImagem-template'
})