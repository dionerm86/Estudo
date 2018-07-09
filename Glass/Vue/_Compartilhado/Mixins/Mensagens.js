/**
 * Objeto com o mixin de mensagens utilizadas pelos componentes.
 */
Vue.mixin({
  methods: {
    /**
     * Função para exibição de uma mensagem simples ao usuário.
     * @param {string} titulo O título a ser exibido para a mensagem.
     * @param {?string} [texto] O texto da mensagem.
     */
    exibirMensagem: function (titulo, texto) {
      if (titulo && !texto) {
        texto = titulo;
        titulo = null;
      }

      alert(texto);
    },

    /**
     * Função para exibição de uma pergunta (Sim/Não) ao usuário.
     * @param {string} titulo O título a ser exibido para a mensagem.
     * @param {?string} [texto] O texto da mensagem.
     * @returns Um valor boolean que indica a resposta do usuário à pergunta (true para 'Sim').
     */
    perguntar: function (titulo, texto) {
      if (titulo && !texto) {
        texto = titulo;
        titulo = null;
      }

      return confirm(texto);
    }
  }
});
