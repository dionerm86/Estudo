/**
 * Filtro para formatação de valores boolean.
 * @param {boolean} valor O valor boolean a ser formatado.
 * @returns '✓', caso o boolean seja verdadeiro. Senão, ''.
 */
Vue.filter('indicaMarcado', function (valor) {
  if (valor === null || valor === undefined) {
    return '';
  }

  if (typeof valor !== 'boolean') {
    return valor;
  }

  return valor ? '\u2713' : '';
});
