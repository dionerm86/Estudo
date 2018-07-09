var Formatacao = Formatacao || {};
Formatacao.moeda = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL'
});

Formatacao.unidadeMonetaria = function () {
  return Formatacao.moeda.format(0)
    .replace(/0/g, '')
    .replace(/\./g, '')
    .replace(/,/g, '')
    .trim();
}

/**
 * Filtro para formatação de valores como valores monetários.
 * @param {number} valor O valor a ser formatado como moeda.
 * @returns O valor monetário, se possível. Senão, retorna o próprio valor.
 */
Vue.filter('moeda', function (valor) {
  if (valor === null || valor === undefined || typeof valor !== 'number') {
    return valor;
  }

  return Formatacao.moeda.format(valor);
});
