var Percentual = Percentual || {};
Percentual.formatar = new Intl.NumberFormat('pt-BR', {
  style: 'percent',
  maximumFractionDigits: 3
});

/**
 * Filtro para formatação de valores como percentuais.
 * @param {number} valor O valor a ser formatado como percentual.
 * @returns O percentual, se possível. Senão, retorna o próprio valor.
 */
Vue.filter('percentual', function (valor) {
  if (valor === null || valor === undefined || typeof valor !== 'number') {
    return valor;
  }

  return Percentual.formatar.format(valor / 100);
});
