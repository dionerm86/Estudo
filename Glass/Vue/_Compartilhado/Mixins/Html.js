var Mixins = Mixins || {};

/**
 * Objeto com funções base para encoding e decoding de HTML.
 */
Mixins.Html = (function ($) {
  var container = $ ? $('<div/>') : document.createElement('textarea');

  return {
    methods: {
      /**
       * Transforma um texto para um formato HTML.
       * @param {string} texto O texto a ser convertido.
       * @returns O texto convertido para HTML.
       */
      htmlEncode: function (texto) {
        if ($) {
          return container.text(texto).html();
        } else {
          container.value = texto;
          return container.innerHTML;
        }
      },

      /**
       * Transforma um HTML para texto.
       * @param {string} html O HTML a ser convertido.
       * @returns O HTML convertido para texto.
       */
      htmlDecode: function (html) {
        if ($) {
          return container.html(html).text();
        } else {
          container.innerHTML = html;
          return container.value;
        }
      }
    }
  }
})(window.jQuery);
