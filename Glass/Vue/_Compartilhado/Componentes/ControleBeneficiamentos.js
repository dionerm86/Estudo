var Beneficiamentos = Beneficiamentos || {};
Beneficiamentos.configuracoes = null;
Beneficiamentos.controle = {
  itens: null,
  carregando: true
};

Vue.component('controle-beneficiamentos', {
  inheritAttrs: false,
  mixins: [Mixins.Clonar, Mixins.ExecutarTimeout, Mixins.UUID],
  props: {
    /**
     * Beneficiamentos selecionados no controle.
     * @type {?Object[]}
     */
    itensSelecionados: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarArrayOuVazio
    },

    /**
     * Indica se o produto é redondo.
     * @type {?boolean}
     */
    redondo: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Valor total dos beneficiamentos selecionados.
     * @type {!number}
     */
    valorBeneficiamentos: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Identificador do produto atual.
     * @type {!number}
     */
    idProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Identificador do subgrupo do produto atual.
     * @type {!number}
     */
    idSubgrupoProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Identificador da cor do produto atual.
     * @type {!number}
     */
    idCorProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Espessura do produto atual.
     * @type {!number}
     */
    espessuraProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica os beneficiamentos padrão do produto atual.
     * @type {!Object[]}
     */
    beneficiamentosPadrao: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Altura do produto atual.
     * @type {!number}
     */
    alturaProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Largura do produto atual.
     * @type {!number}
     */
    larguraProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Quantidade do produto atual.
     * @type {!number}
     */
    quantidadeProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Quantidade do ambiente atual.
     * @type {?number}
     */
    quantidadeAmbiente: {
      required: true,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Área, em m², do produto atual.
     * @type {!number}
     */
    areaM2Produto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Área para cálculo, em m², do produto atual.
     * @type {!number}
     */
    areaCalculadaM2Produto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Valor unitário do produto atual.
     * @type {!number}
     */
    valorUnitarioProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Custo unitário do produto atual.
     * @type {!number}
     */
    custoUnitarioProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica se o cliente do orçamento/pedido/PCP é de revenda.
     * @type {!boolean}
     */
    clienteRevenda: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Percentual de desconto ou acréscimo do cliente para o produto atual.
     * @type {!number}
     */
    percentualDescontoAcrescimoCliente: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica se o percentual de desconto ou acréscimo do cliente também
     * é válido para os beneficiamentos.
     * @type {!boolean}
     */
    usarDescontoAcrescimoClienteNosBeneficiamentos: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Tipo de entrega do orçamento/pedido/PCP.
     * @type {!number}
     */
    tipoEntrega: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Percentual de comissão do orçamento/pedido/PCP.
     * @type {!number}
     */
    percentualComissao: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Tipo de beneficiamentos para exibição no controle.
     * @type {!string}
     */
    tipoBeneficiamentos: {
      required: false,
      twoWay: false,
      default: 'Venda',
      validator: Mixins.Validacao.validarValores('Todos', 'Venda', 'MaoDeObraEspecial')
    },

    /**
     * Número de colunas para exibição dos beneficiamentos.
     * @type {!number}
     */
    numeroColunasParaExibicao: {
      required: false,
      twoWay: false,
      default: 2,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica se o custo do beneficiamento deve ser exibido para alteração.
     * @type {!boolean}
     */
    exibirValorBeneficiamento: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBoolean
    }
  },

  data: function () {
    const estiloControles = {
      marginRight: 0,
      marginLeft: 0,
      paddingLeft: 'initial',
      alignSelf: 'center'
    };

    const estiloValor = {
      width: '60px',
      border: 'solid 1px Gray',
      backgroundColor: '#DAF1F8'
    };

    const estiloContainerBeneficiamentos = {
      display: 'inline-grid',
      gridTemplateColumns: 'repeat(2, max-content)',
      gridGap: '4px',
      alignItems: 'center'
    };

    return {
      uuid: null,
      beneficiamentos: null,
      beneficiamentosAgrupados: null,
      pronto: false,
      timeoutAtualizacaoItensSelecionados: null,
      estiloControles,
      estiloValor,
      unidadeMonetaria: Formatacao.unidadeMonetaria(),
      exibirBeneficiamentos: false,
      estiloContainerBeneficiamentos
    };
  },

  methods: {
    /**
     * Cria o objeto com os beneficiamentos agrupados, para uso no controle.
     * Agrupa o beneficiamento com o item selecionado correspondente.
     */
    agruparBeneficiamentos: function () {
      var padraoProduto = this.beneficiamentosPadrao
        ? this.beneficiamentosPadrao.slice()
        : [];

      var selecionados = this.itensSelecionados
        ? this.itensSelecionados.slice()
        : [];

      const obterSelecionadosParaBeneficiamento = function (beneficiamento, itensSelecionados) {
        return itensSelecionados.filter(function (itemSelecionado) {
          if (!itemSelecionado) {
            return false;
          }

          if (beneficiamento.id === itemSelecionado.id) {
            return true;
          }

          return beneficiamento.filhos
            && beneficiamento.filhos.filter(function (filho) {
              return filho && filho.id === itemSelecionado.id;
            }).length;
        });
      };

      this.beneficiamentosAgrupados = this.beneficiamentosOrdenados.map(function (item) {
        var itensSelecionados = obterSelecionadosParaBeneficiamento(item, padraoProduto);
        var padrao = itensSelecionados.length > 0;

        if (!itensSelecionados.length) {
          itensSelecionados = obterSelecionadosParaBeneficiamento(item, selecionados);
        }

        return {
          beneficiamento: item,
          itensSelecionados: itensSelecionados || null,
          cobrar: (!item.permitirCobrancaOpcional || Beneficiamentos.configuracoes.cobrancaOpcionalMarcada) && !padrao,
          padrao
        };
      });
    },

    /**
     * Altera os itens selecionados e calcula o valor total dos beneficiamentos.
     */
    alterarItensSelecionados: function () {
      this.executarTimeout('alterarItensSelecionados', function () {
        const dadosCalculo = {
          produto: {
            id: this.idProduto,
            idSubgrupo: this.idSubgrupoProduto,
            idCor: this.idCorProduto,
            espessura: this.espessuraProduto,
            altura: this.alturaProduto,
            largura: this.larguraProduto,
            quantidade: this.quantidadeProduto,
            quantidadeAmbiente: this.quantidadeAmbiente,
            areaM2: this.areaM2Produto,
            areaCalculadaM2: this.areaCalculadaM2Produto,
            valorUnitario: this.valorUnitarioProduto,
            custoUnitario: this.custoUnitarioProduto
          },
          clienteRevenda: this.clienteRevenda,
          valorBeneficiamentoEstaEditavelNoControle: this.exibirValorBeneficiamento,
          percentualDescontoAcrescimoCliente: this.percentualDescontoAcrescimoCliente,
          usarDescontoAcrescimoClienteNosBeneficiamentos: this.usarDescontoAcrescimoClienteNosBeneficiamentos,
          tipoEntrega: this.tipoEntrega,
          percentualComissao: this.percentualComissao
        };

        var selecionados = this.clonar(this.beneficiamentosAgrupados)
          .filter(function (item) {
            return item.itensSelecionados && item.itensSelecionados.length;
          });

        var itensSelecionados = selecionados
          .map(function (item) {
            return item.itensSelecionados;
          })
          .reduce(function (array, item) {
            return array.concat(item);
          }, []);

        var redondo = false;

        if (!itensSelecionados.length) {
          itensSelecionados = null;
          this.$emit('update:valorBeneficiamentos', 0);
        } else {
          var vm = this;

          redondo = selecionados
            .filter(function (item) {
              return item.beneficiamento.nome.toLowerCase() === 'redondo';
            })
            .length > 0;

          if (redondo === this.redondo) {
            Servicos.Beneficiamentos.calcularTotal({
              dadosCalculo,
              beneficiamentos: selecionados
            })
              .then(function (resposta) {
                var valorTotal = 0;
                var itensSelecionados = vm.beneficiamentosAgrupados
                  .filter(function (item) {
                    return item.itensSelecionados && item.itensSelecionados.length;
                  })
                  .map(function (item) {
                    return item.itensSelecionados;
                  })
                  .reduce(function (acumulador, item) {
                    return acumulador.concat(item);
                  }, []);

                for (var calculado of resposta.data) {
                  valorTotal += calculado.valorTotal;

                  for (var item of itensSelecionados) {
                    if (item.id === calculado.idSelecionado) {
                      item.valorUnitario = calculado.valorUnitario;
                      item.valorTotal = calculado.valorTotal;
                      item.custoTotal = calculado.custoTotal;
                      break;
                    }
                  }
                }

                vm.$emit('update:valorBeneficiamentos', valorTotal);
              })
              .catch(function (erro) {
                if (erro && erro.mensagem) {
                  vm.exibirMensagem('Erro', erro.mensagem);
                }

                vm.$emit('update:valorBeneficiamentos', 0);
              });
          }
        }

        if (redondo !== this.redondo) {
          this.$emit('update:redondo', redondo);
        } else {
          this.$emit('update:itensSelecionados', itensSelecionados);
        }
      }, 150);
    },

    /**
     * Retorna um identificador único para o beneficiamento no controle.
     * @param {!object} beneficiamento O beneficiamento que está sendo criado.
     * @param {?string} [complemento=null] Um complemento que pode ser colocado no identificador.
     * @returns {string} O identificador único para o beneficiamento no controle.
     */
    idUnico: function(beneficiamento, complemento) {
      return this.uuid + '_' + beneficiamento.id + (complemento ? '_' + complemento : '');
    },

    /**
     * Retorna um valor que indica se o beneficiamento é cobrado por valor monetário.
     * @param {!object} beneficiamento O beneficiamento que está sendo criado.
     * @returns {boolean} Um valor que indica se a cobrança é monetária.
     */
    tipoCobrancaValor: function(beneficiamento) {
      return beneficiamento.tipoCalculo !== Beneficiamentos.configuracoes.tiposCalculo.porcentagem.id;
    },

    /**
     * Retorna um valor que indica se um beneficiamento usa o tipo de controle 'bisote'.
     * @param {?Object} beneficiamento O beneficiamento que será validado.
     * @returns {boolean} Verdadeiro, se o tipo de controle for 'bisote'.
     */
    bisote: function (beneficiamento) {
      return beneficiamento
        && beneficiamento.tipoControle === Beneficiamentos.configuracoes.tiposControle.bisote.id;
    },

    /**
     * Retorna um valor que indica se um beneficiamento usa o tipo de controle 'lapidacao'.
     * @param {?Object} beneficiamento O beneficiamento que será validado.
     * @returns {boolean} Verdadeiro, se o tipo de controle for 'lapidacao'.
     */
    lapidacao: function (beneficiamento) {
      return beneficiamento
        && beneficiamento.tipoControle === Beneficiamentos.configuracoes.tiposControle.lapidacao.id;
    },

    /**
     * Retorna um valor que indica se um beneficiamento usa o tipo de controle 'listaSelecao'.
     * @param {?Object} beneficiamento O beneficiamento que será validado.
     * @returns {boolean} Verdadeiro, se o tipo de controle for 'listaSelecao'.
     */
    listaSelecao: function (beneficiamento) {
      return beneficiamento
        && beneficiamento.tipoControle === Beneficiamentos.configuracoes.tiposControle.listaSelecao.id;
    },

    /**
     * Retorna um valor que indica se um beneficiamento usa o tipo de controle 'listaSelecaoQuantidade'.
     * @param {?Object} beneficiamento O beneficiamento que será validado.
     * @returns {boolean} Verdadeiro, se o tipo de controle for 'listaSelecaoQuantidade'.
     */
    listaSelecaoQuantidade: function (beneficiamento) {
      return beneficiamento
        && beneficiamento.tipoControle === Beneficiamentos.configuracoes.tiposControle.listaSelecaoQtd.id;
    },

    /**
     * Retorna um valor que indica se um beneficiamento usa o tipo de controle 'quantidade'.
     * @param {?Object} beneficiamento O beneficiamento que será validado.
     * @returns {boolean} Verdadeiro, se o tipo de controle for 'quantidade'.
     */
    quantidade: function (beneficiamento) {
      return beneficiamento
        && beneficiamento.tipoControle === Beneficiamentos.configuracoes.tiposControle.quantidade.id;
    },

    /**
     * Retorna um valor que indica se um beneficiamento usa o tipo de controle 'selecaoMultiplaInclusiva'.
     * @param {?Object} beneficiamento O beneficiamento que será validado.
     * @returns {boolean} Verdadeiro, se o tipo de controle for 'selecaoMultiplaInclusiva'.
     */
    selecaoMultiplaInclusiva: function (beneficiamento) {
      return beneficiamento
        && beneficiamento.tipoControle === Beneficiamentos.configuracoes.tiposControle.selecaoMultiplaInclusiva.id;
    },

    /**
     * Retorna um valor que indica se um beneficiamento usa o tipo de controle 'selecaoMultiplaExclusiva'.
     * @param {?Object} beneficiamento O beneficiamento que será validado.
     * @returns {boolean} Verdadeiro, se o tipo de controle for 'selecaoMultiplaExclusiva'.
     */
    selecaoMultiplaExclusiva: function (beneficiamento) {
      return beneficiamento
        && beneficiamento.tipoControle === Beneficiamentos.configuracoes.tiposControle.selecaoMultiplaExclusiva.id;
    }
  },

  created: function () {
    this.uuid = this.gerarUuid();
    var vm = this;

    if (Beneficiamentos.configuracoes === null) {
      Beneficiamentos.configuracoes = [this];

      Servicos.Beneficiamentos.obterConfiguracoes()
        .then(function (resposta) {
          var controles = Beneficiamentos.configuracoes;
          Beneficiamentos.configuracoes = resposta.data;

          for (var controle of controles) {
            controle.pronto = true;
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }

          Beneficiamentos.configuracoes = {};
        });
    } else if (Array.isArray(Beneficiamentos.configuracoes)) {
      Beneficiamentos.configuracoes.push(this);
    } else {
      this.pronto = true;
    }

    if (Beneficiamentos.controle.itens === null) {
      Beneficiamentos.controle.itens = [this];

      Servicos.Beneficiamentos.obterParaControle(this.tipoBeneficiamentos)
        .then(function (resposta) {
          var controles = Beneficiamentos.controle.itens;

          Beneficiamentos.controle.carregando = false;
          Beneficiamentos.controle.itens = resposta.data;

          for (var controle of controles) {
            controle.beneficiamentos = Beneficiamentos.controle.itens;
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }

          Beneficiamentos.controle.itens = {};
        });
    } else if (Beneficiamentos.controle.carregando && Array.isArray(Beneficiamentos.controle.itens)) {
      Beneficiamentos.controle.itens.push(this);
    } else {
      this.beneficiamentos = Beneficiamentos.controle.itens;
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna os beneficiamentos ordenados para
     * exibição no controle.
     */
    beneficiamentosOrdenados: function() {
      var beneficiamentosTemp = this.beneficiamentos
        ? this.beneficiamentos.slice()
        : [];

      var beneficiamentos = [];
      var offset = Math.ceil(beneficiamentosTemp.length / this.numeroColunasParaExibicao);

      for (var i = 0; i < offset; i++) {
        for (var j = 0; j < this.numeroColunasParaExibicao; j++) {
          var index = i + j * offset;

          if (index < beneficiamentosTemp.length) {
            beneficiamentos.push(beneficiamentosTemp[index]);
          }
        }
      }

      return beneficiamentos;
    },

    /**
     * Propriedade computada que retorna o estilo que será aplicável ao controle
     * de beneficiamentos, alinhando os controles como se fosse uma tabela.
     */
    estiloBeneficiamentos: function () {
      var colunas = this.numeroColunasParaExibicao;
      if (this.exibirValorBeneficiamento) {
        colunas++;
      }

      return {
        display: 'inline-grid',
        gridTemplateColumns: 'repeat(' + (colunas * 2) + ', max-content)',
        gridGap: '4px 8px'
      };
    }
  },

  watch: {
    /**
     * Observador para a variável 'beneficiamentos'.
     * Recria os itens do controle, se necessário.
     */
    beneficiamentos: {
      handler: function () {
        this.agruparBeneficiamentos();
      },
      deep: true
    },

    /**
     * Observador para a variável 'beneficiamentosAgrupados'.
     * Atualiza a propriedade de itens selecionados e recalcula o valor dos beneficiamentos.
     */
    beneficiamentosAgrupados: {
      handler: function () {
        this.alterarItensSelecionados();
      },
      deep: true
    },

    /**
     * Observador para a variável 'redondo'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    redondo: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'idProduto'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    idProduto: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'beneficiamentosPadrao'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    beneficiamentosPadrao: {
      handler: function () {
        this.agruparBeneficiamentos();
      },
      deep: true
    },

    /**
     * Observador para a variável 'alturaProduto'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    alturaProduto: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'larguraProduto'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    larguraProduto: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'quantidadeProduto'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    quantidadeProduto: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'quantidadeAmbiente'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    quantidadeAmbiente: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'areaM2Produto'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    areaM2Produto: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'areaCalculadaM2Produto'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    areaCalculadaM2Produto: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'valorUnitarioProduto'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    valorUnitarioProduto: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'custoUnitarioProduto'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    custoUnitarioProduto: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'clienteRevenda'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    clienteRevenda: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'percentualDescontoAcrescimoCliente'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    percentualDescontoAcrescimoCliente: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'usarDescontoAcrescimoClienteNosBeneficiamentos'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    usarDescontoAcrescimoClienteNosBeneficiamentos: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'tipoEntrega'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    tipoEntrega: function () {
      this.alterarItensSelecionados();
    },

    /**
     * Observador para a variável 'percentualComissao'.
     * Recalcula os beneficiamentos se o valor da variável for alterado.
     */
    percentualComissao: function () {
      this.alterarItensSelecionados();
    }
  },

  template: '#ControleBeneficiamentos-template'
});
