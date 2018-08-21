/**
 * Filtro para formatação de valores boolean.
 * @param {boolean} valor O valor boolean a ser formatado.
 * @returns 'Sim', caso o boolean seja verdadeiro. Senão, 'Não'.
 */
Vue.filter('simNao', function (valor) {
  if (valor === null || valor === undefined) {
    return '';
  }

  if (typeof valor !== 'boolean') {
    return valor;
  }

  return valor ? 'Sim' : 'Não';
});
