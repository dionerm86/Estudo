/**
 * Objeto com o mixin de mensagens utilizadas pelos componentes.
 */
Vue.mixin({
  methods: {
    /**
     * Exibe uma janela nova no sistema, na forma de um popup.
     * @param {!number} altura A altura da nova janela.
     * @param {!number} largura A largura da nova janela.
     * @param {!string} url A URL da janela que será aberta.
     * @param {?Object} [opener=window] A janela responsável pela abertura da nova janela.
     * @param {?boolean} [exibirBotaoFechar=true] Para os modais, deve-se exibir o botão de 'fechar'?
     */
    abrirJanela: function (altura, largura, url, opener, exibirBotaoFechar) {
      openWindow(altura, largura, url, opener, exibirBotaoFechar);
    }
  }
});
