var Formatacao = Formatacao || {};
Formatacao.decimal = new Intl.NumberFormat('pt-BR', {
  style: 'decimal'
});

/**
 * Filtro para formatação de valores como decimais.
 * @param {number} valor O valor a ser formatado como decimal.
 * @returns O decimal, se possível. Senão, retorna o próprio valor.
 */
Vue.filter('decimal', function (valor) {
  if (valor === null || valor === undefined || typeof valor !== 'number') {
    return valor;
  }

  return Formatacao.decimal.format(valor);
});
