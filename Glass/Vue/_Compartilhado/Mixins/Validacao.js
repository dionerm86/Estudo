var Mixins = Mixins || {};

/**
 * Objeto com as funções de validação usadas nos componentes.
 * Foi necessária a criação (ao invés de usar os 'type' presentes no componente)
 * para que sejam considerados válidos também os valores null/undefined.
 */
Mixins.Validacao = (function () {
  var objetoValidacao = {};

  // Objeto com as validações possíveis de serem feitas
  // Essas validações são criadas dinamicamente no construtor do objeto.
  var validacao = [
    {
      nome: 'Numero',
      incluirOuVazio: true,
      validar: function(valor) {
        return typeof valor === 'number';
      }
    },
    {
      nome: 'String',
      incluirOuVazio: true,
      validar: function(valor) {
        return typeof valor === 'string';
      }
    },
    {
      nome: 'Array',
      incluirOuVazio: true,
      validar: function(valor) {
        return Array.isArray(valor);
      }
    },
    {
      nome: 'Funcao',
      incluirOuVazio: false,
      validar: function(valor) {
        return typeof valor === 'function';
      }
    },
    {
      nome: 'Boolean',
      incluirOuVazio: true,
      validar: function(valor) {
        return typeof valor === 'boolean';
      }
    },
    {
      nome: 'Objeto',
      incluirOuVazio: true,
      validar: function(valor) {
        return typeof valor === 'object' && !objetoValidacao.validarVazio(valor);
      }
    },
    {
      nome: 'Data',
      incluirOuVazio: true,
      validar: function(valor) {
        return valor instanceof Date;
      }
    }
  ];

  /**
   * Função que valida se o valor é vazio (null/undefined).
   * @param {*} valor O valor a ser validado.
   * @returns Um valor boolean que indica se o valor é vazio.
   */
  objetoValidacao.validarVazio = function(valor) {
    return valor === null || valor === undefined;
  };

  /**
   * Função que valida o valor, de acordo com a função de validação informada.
   * @param {function} validar A função de validação do item.
   * @param {*} valor O valor a ser validado.
   * @returns Um valor boolean que indica se o valor é válido.
   */
  var validarItem = function(validar) {
    var valor = arguments[1];
    return validar(valor);
  };

  /**
   * Função que valida um valor, que pode ser de acordo uma função de validação informada ou vazio.
   * @param {function} validar A função de validação do item.
   * @param {*} valor O valor a ser validado.
   * @returns Um valor boolean que indica se o valor é válido.
   */
  var validarItemOuVazio = function(validar) {
    var valor = arguments[1];
    return validarItem(validar, valor) || objetoValidacao.validarVazio(valor);
  };

  // Cria dinamicamente o objeto de validação de acordo com as funções definidas no início
  for (var itemValidacao of validacao) {
    var nomeFuncao = 'validar' + itemValidacao.nome;

    objetoValidacao[nomeFuncao] = validarItem.bind(this, itemValidacao.validar);

    if (itemValidacao.incluirOuVazio) {
      objetoValidacao[nomeFuncao + 'OuVazio'] = validarItemOuVazio.bind(this, itemValidacao.validar);
    }
  }

  /**
   * Função que valida se um valor está em uma lista definida.
   * @param {...*} valor A lista de valores que será usada para validação.
   * @returns Retorna uma função que realiza a validação, recebendo um valor e comparando com a lista informada anteriormente.
   */
  objetoValidacao.validarValores = function(valor) {
    var valores = [];
    for (var i = 0, n = arguments.length; i < n; i++) {
      var item = arguments[i];

      if (!Array.isArray(item)) {
        valores.push(item);
      } else {
        valores = valores.concat(item);
      }
    }

    return function(valorValidar) {
      return valores.indexOf(valorValidar) !== -1;
    };
  };

  /**
   * Função que valida se um valor está em uma lista definida ou se é vazio (null/undefined).
   * @param {...*} valor A lista de valores que será usada para validação.
   * @returns Retorna uma função que realiza a validação, recebendo um valor e comparando com a lista informada anteriormente.
   */
  objetoValidacao.validarValoresOuVazio = function(valor) {
    var valores = [];
    for (var i = 0, n = arguments.length; i < n; i++) {
      var item = arguments[i];

      if (!Array.isArray(item)) {
        valores.push(item);
      } else {
        valores = valores.concat(item);
      }
    }

    return validarItemOuVazio.bind(this, objetoValidacao.validarValores(valores));
  };

  return objetoValidacao;
})();
