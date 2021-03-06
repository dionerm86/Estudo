﻿Vue.component('lista-itens-venda', {
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista()],
  props: {
    /**
     * Indica se a venda atual é de mão-de-obra.
     * @type {boolean}
     */
    maoDeObra: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Indica se a venda atual é de produção para corte.
     * @type {boolean}
     */
    producaoCorte: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Indica se a venda atual é de mão-de-obra especial.
     * @type {boolean}
     */
    maoDeObraEspecial: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Filtro para a pesquisa de itens.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * ID do cliente da venda atual.
     * @type {Object}
     */
    idCliente: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica se o cliente da venda atual é de revenda.
     * @type {Object}
     */
    clienteRevenda: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Tipo de entrega da venda atual.
     * @type {Object}
     */
    tipoEntrega: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Percentual de comissão da venda atual.
     * @type {Object}
     */
    percentualComissao: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Dados utilizados para a validação de produto.
     * @type {Object}
     */
    dadosValidacaoProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Tipo de validação de produto.
     * @type {Object}
     */
    tipoValidacaoProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * Identificador do ambiente (se necessário).
     * @type {number}
     */
    idAmbiente: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Quantidade de itens no ambiente (para vendas mão-de-obra).
     * @type {number}
     */
    quantidadeAmbiente: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica se os dados de etiqueta (processo e aplicação) são obrigatórios pelo roteiro de produção.
     * @type {?boolean}
     */
    obrigarProcessoEAplicacaoRoteiro: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Função que busca os itens de venda para serem exibidos no controle.
     * @param {!Object} filtro O filtro informado pelo controle lista-paginada.
     * @param {!number} pagina O número da página que está sendo exibida no controle lista-paginada.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na tela.
     * @param {string} ordenacao O campo pelo qual o resultado será ordenado.
     * @returns {Promise} Uma Promise com a busca dos itens de venda, de acordo com o filtro.
     */
    buscarItensVenda: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarFuncao
    },

    /**
     * Função executada ao iniciar a edição de um item de venda.
     * @type {Function}
     * @param {Object} item O item que está sendo editado.
     * @returns {boolean} Verdadeiro, se o item puder ser editado.
     */
    editar: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarFuncaoOuVazio
    },

    /**
     * Função executada ao iniciar a exclusão de um item de venda.
     * @type {Function}
     * @param {Object} item O item que está sendo excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarFuncaoOuVazio
    },

    /**
     * Função executada ao iniciar a atualização de um item de venda.
     * @type {Function}
     * @param {Object} item O item que está sendo atualizado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarFuncaoOuVazio
    },

    /**
     * Função executada ao iniciar a inclusão de um item de venda.
     * @type {Function}
     * @param {Object} item O item que está sendo inserido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarFuncaoOuVazio
    },

    /**
     * Indica se a lista está sendo exibida para um produto de composição.
     * @type {boolean}
     */
    permitirInserir: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Função executada ao definir que o popup de falta de estoque deve ser exibido.
     * @type {Function}
     * @param {number} idProduto O identificador do produto atual.
     * @param {number} altura A altura do popup.
     * @param {number} largura A largura do popup.
     */
    exibirPopupFaltaEstoque: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarFuncaoOuVazio
    },

    /**
     * Indica se as colunas de processo e aplicação serão exibidas na lista.
     * @type {boolean}
     */
    exibirColunasProcessoEAplicacao: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Indica se a empresa vende vidro (caso não venda, esconde algumas colunas).
     * @type {boolean}
     */
    empresaVendeVidro: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  data: function () {
    const estiloContainerBeneficiamentos = {
      display: 'inline-grid',
      gridTemplateColumns: 'repeat(2, max-content)',
      gridGap: '4px',
      alignItems: 'center'
    };

    return {
      controleAtualizacao: 0,
      inserindo: false,
      itemVenda: {},
      itemVendaOriginal: {},
      numeroLinhaEdicao: -1,
      exibirDescontoQuantidade: false,
      produtoAtual: null,
      dadosValidacaoProdutoAtual: this.dadosValidacaoProduto,
      numeroBeneficiamentosParaAreaMinima: 0,
      processoAtual: null,
      aplicacaoAtual: null,
      processoFilhasAtual: null,
      aplicacaoFilhasAtual: null,
      filhosEmExibicao: [],
      exibirBeneficiamentos: false,
      estiloContainerBeneficiamentos
    };
  },

  methods: {
    /**
     * Função que busca os produtos filhos de itens de venda compostos/laminados.
     * @param {!Object} filtro O filtro informado pelo controle lista-paginada.
     * @param {!number} pagina O número da página que está sendo exibida no controle lista-paginada.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na tela.
     * @param {string} ordenacao O campo pelo qual o resultado será ordenado.
     * @returns {Promise} Uma Promise com a busca dos itens de venda, de acordo com o filtro.
     */
    buscarFilhos: function (filtro, pagina, numeroRegistros, ordenacao) {
      var novoFiltro = this.clonar(this.filtro || {});
      novoFiltro.idProdutoPai = filtro.idProdutoPai;

      return this.buscarItensVenda(novoFiltro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Inicia a edição do produto, se possível.
     * @param {Object} item O produto que será editado.
     * @param {number} linha O número da linha da tabela que entrará em modo de edição.
     */
    editar_: function (item, linha) {
      if (this.editar && !this.editar(item)) {
        return;
      }

      this.inserindo = false;
      this.iniciarCadastroOuAtualizacao_(item);
      this.numeroLinhaEdicao = linha;
    },

    /**
     * Exclui um produto, se possível.
     * @param {Object} item O produto que será excluído.
     */
    excluir_: function (item) {
      if (this.excluir) {
        var vm = this;

        this.excluir(item)
          .then(function (resposta) {
            vm.controleAtualizacao++;
          })
          .catch(function (erro) {
            if (erro && erro.mensagem) {
              vm.exibirMensagem('Erro', erro.mensagem);
            }
          });
      }
    },

    /**
     * Inicia o cadastro de um produto.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Insere um produto, se possível.
     * @param {Object} event O objeto com o evento JavaScript.
     */
    inserir_: function (event) {
      if (!event || !this.validarFormulario_(event.target, true)) {
        return;
      }

      if (this.inserir) {
        this.itemVenda.idAmbiente = this.idAmbiente;
        var vm = this;

        this.inserir(this.itemVenda)
          .then(function (resposta) {
            vm.controleAtualizacao++;
            vm.cancelar();
          })
          .catch(function (erro) {
            if (erro && erro.mensagem) {
              vm.exibirMensagem('Erro', erro.mensagem);
            }
          });
      }
    },

    /**
     * Atualiza um produto, se possível.
     * @param {Object} event O objeto com o evento JavaScript.
     */
    atualizar_: function (event) {
      if (!event || !this.validarFormulario_(event.target, true)) {
        return;
      }

      if (this.atualizar) {
        var itemVendaAtualizar = this.patch(this.itemVenda, this.itemVendaOriginal);
        itemVendaAtualizar.id = this.itemVenda.id;
        itemVendaAtualizar.beneficiamentos = this.clonar(this.itemVenda.beneficiamentos);

        var vm = this;

        this.atualizar(itemVendaAtualizar)
          .then(function (resposta) {
            vm.controleAtualizacao++;
            vm.cancelar();
          })
          .catch(function (erro) {
            if (erro && erro.mensagem) {
              vm.exibirMensagem('Erro', erro.mensagem);
            }
          });
      }
    },

    /**
     * Cancela a edição ou cadastro de produto.
     */
    cancelar: function () {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Inicia um cadastro ou edição de produto, criando o objeto para 'bind'.
     * @param {Object} item O produto que será usado como base, no caso de edição.
     */
    iniciarCadastroOuAtualizacao_: function (item) {
      this.exibirBeneficiamentos = false;

      this.produtoAtual = item && item.produto
        ? {
          id: item.produto.id,
          codigo: item.produto.codigo
        }
        : null;

      this.processoAtual = item ? this.clonar(item.processo) : null;
      this.aplicacaoAtual = item ? this.clonar(item.aplicacao) : null;

      this.itemVendaOriginal = {
        id: item ? item.id : null,
        idAmbiente: item ? item.idAmbiente : null,
        produto: {
          id: item && item.produto ? item.produto.id : null,
          espessura: item && item.produto ? item.produto.espessura : null,
          codigo: item && item.produto ? item.produto.codigo : null,
          descricao: item && item.produto ? item.produto.descricao : null,
        },
        quantidade: item ? item.quantidade : null,
        descontoPorQuantidade: {
          percentual: item && item.descontoPorQuantidade ? item.descontoPorQuantidade.percentual : null,
          valor: item && item.descontoPorQuantidade ? item.descontoPorQuantidade.valor : null
        },
        largura: item ? item.largura : null,
        altura: {
          paraCalculo: item && item.altura ? item.altura.paraCalculo : null,
          real: item && item.altura ? item.altura.real : null
        },
        areaEmM2: {
          paraCalculo: item && item.areaEmM2 ? item.areaEmM2.paraCalculo : null,
          real: item && item.areaEmM2 ? item.areaEmM2.real : null
        },
        valorUnitario: item ? item.valorUnitario : null,
        idProcesso: item && item.processo ? item.processo.id : null,
        idAplicacao: item && item.aplicacao ? item.aplicacao.id : null,
        idProcessoFilhas: null,
        idAplicacaoFilhas: null,
        codigoPedidoCliente: item ? item.codigoPedidoCliente : null,
        total: item ? item.total : null,
        beneficiamentos: {
          valor: item && item.beneficiamentos ? item.beneficiamentos.valor : null,
          altura: item && item.beneficiamentos ? item.beneficiamentos.altura : null,
          largura: item && item.beneficiamentos ? item.beneficiamentos.largura : null,
          redondo: item && item.beneficiamentos ? item.beneficiamentos.redondo : null,
          itens: item && item.beneficiamentos && item.beneficiamentos.itens ? item.beneficiamentos.itens.slice() : []
        },
        composicao: {
          possuiFilhos: item && item.composicao ? item.composicao.possuiFilhos : null,
          aplicarBeneficiamentosProdutosFilhos: item && item.composicao ? item.composicao.aplicarBeneficiamentosProdutosFilhos : null
        }
      };

      if (this.dadosValidacaoProdutoAtual) {
        this.dadosValidacaoProdutoAtual.areaM2DesconsiderarObra = this.itemVendaOriginal.areaEmM2.real;
      }

      this.itemVenda = this.clonar(this.itemVendaOriginal);
      this.itemVenda.produto.id = null;
    },

    /**
     * Função que indica se o formulário de itens de venda possui valores válidos de acordo com os controles.
     * @param {Object} botao O botão que foi disparado no controle.
     * @param {?boolean} [reportar=false] Indica se a validação deve ser reportada na tela.
     * @returns {boolean} Um valor que indica se o formulário está válido.
     */
    validarFormulario_: function (botao, reportar) {
      var form = botao.form || botao;
      while (form.tagName.toLowerCase() !== 'form') {
        form = form.parentNode;
      }

      var mensagemProcesso = '';
      var mensagemAplicacao = '';

      if (this.produtoAtual && this.exibirColunasProcessoEAplicacao && this.produtoAtual.exigirProcessoEAplicacao) {
        if (!this.processoAtual || !this.processoAtual.id) {
          mensagemProcesso = this.obrigarProcessoEAplicacaoRoteiro
            ? 'É obrigatório informar o processo caso algum setor seja to tipo "Por Roteiro" ou "Por Benef.".'
            : 'Informe o processo.';
        }

        if (!this.aplicacaoAtual || !this.aplicacaoAtual.id) {
          mensagemAplicacao = this.obrigarProcessoEAplicacaoRoteiro
            ? 'É obrigatório informar a aplicação caso algum setor seja to tipo "Por Roteiro" ou "Por Benef.".'
            : 'Informe a aplicação.';
        }

        const alterarCampo = function (tipo, mensagem, refs) {
          try {
            var campo = refs[tipo].$refs.base.$refs.campoBusca.$refs.campo;

            if (campo) {
              campo.setCustomValidity(mensagem);

              if (reportar) {
                campo.reportValidity();
              }
            }
          } catch (erro) {}
        }

        alterarCampo('processo', mensagemProcesso, this.$refs);
        alterarCampo('aplicacao', mensagemAplicacao, this.$refs);
      }

      return form.checkValidity();
    },

    /**
     * Recarrega os produtos de composição ao realizar uma alteração na lista.
     */
    listaAtualizada: function () {
      this.controleAtualizacao++;
    },

    /**
     * Indica se os filhos (produtos de composição) estão sendo exibidos na lista.
     * @param {!number} indice O número da linha que está sendo verificada.
     * @returns {boolean} Verdadeiro, se os itens estiverem sendo exibidos.
     */
    exibindoFilhos: function (indice) {
      return this.filhosEmExibicao.indexOf(indice) > -1;
    },

    /**
     * Alterna a exibição dos filhos (produtos de composição).
     */
    alternarExibicaoFilhos: function (indice) {
      var i = this.filhosEmExibicao.indexOf(indice);

      if (i > -1) {
        this.filhosEmExibicao.splice(i, 1);
      } else {
        this.filhosEmExibicao.push(indice);
      }
    },

    /**
     * Realiza a validação do estoque para o produto atual.
     */
    validarEstoque: function () {
      if (!this.produtoAtual || !this.produtoAtual.estoque) {
        return;
      }

      if (this.produtoAtual.estoque.quantidadeAtual === 0) {
        this.exibirMensagem('Estoque', 'Não há nenhuma peça deste produto no estoque.');
        return;
      }

      var valorComparar;
      switch (this.produtoAtual.estoque.unidade) {
        case "m²":
          valorComparar = this.itemVenda.areaEmM2.real;
          break;

        case "ml":
          valorComparar = this.itemVenda.altura.paraCalculo * this.itemVenda.quantidade;
          break;

        default:
          valorComparar = this.itemVenda.quantidade;
          break;
      }

      var estoqueMenor = valorComparar > this.produtoAtual.estoque.quantidadeAtual;

      if (estoqueMenor) {
        var barras = this.produtoAtual.estoque.numeroBarrasAluminio > 0
          ? ' (' + this.produtoAtual.estoque.numeroBarrasAluminio + ' barras)'
          : '';

        var mensagem = 'Há apenas '
          + this.produtoAtual.estoque.quantidadeAtual
          + ' '
          + this.produtoAtual.estoque.unidade
          + barras
          + ' deste produto no estoque.';

        this.itemVenda.quantidade = null;
        this.exibirMensagem('Estoque', mensagem);
      }

      if (this.produtoAtual.estoque.exibirPopupFaltaEstoque
        && (estoqueMenor || this.produtoAtual.estoque.quantidadeReal <= 0)) {

        this.exibirPopupFaltaEstoque(this.produtoAtual.id, 400, 600);
      }
    },

    /**
     * Retorna o número de colunas da lista paginada.
     * @type {number}
     */
    numeroColunasLista: function () {
      return this.$refs.lista.numeroColunas();
    },

    /**
     * Limpa a lista de filhos exibidos ao paginar a lista.
     */
    atualizouItens: function () {
      this.filhosEmExibicao.splice(0, this.filhosEmExibicao.length);
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o tipo para busca no controle de beneficiamentos.
     * @type {string}
     */
    tipoBeneficiamentos: function () {
      return this.maoDeObraEspecial ? 'MaoDeObraEspecial' : 'Venda';
    },

    /**
     * Propriedade computada para exibição do controle de beneficiamentos.
     * @type {boolean}
     */
    vIfControleBeneficiamentos: function () {
      return this.itemVenda
        && this.itemVenda.beneficiamentos
        && this.itemVenda.altura
        && this.itemVenda.areaEmM2
        && this.produtoAtual
        && this.produtoAtual.exibirBeneficiamentos
        && this.empresaVendeVidro;
    },

    /**
     * Propriedade computada com o tipo de validação para uso nos produtos de composição.
     * @type {string}
     */
    tipoValidacaoProdutoInterno: function () {
      var tipoValidacao = 'Produto' + this.tipoValidacaoProduto;
      return tipoValidacao.replace('ProdutoProduto', 'Produto');
    },

    /**
     * Propriedade computada para exibição dos controles de processo e aplicação dos produtos filhos (no caso de composição).
     * @type {boolean}
     */
    vIfProcessoAplicacaoProdutosFilhos: function () {
      return this.inserindo
        && this.exibirColunasProcessoEAplicacao
        && this.produtoAtual
        && this.produtoAtual.composicao
        && this.produtoAtual.composicao.possuiFilhos;
    }
  },

  watch: {
    /**
     * Observador para a variável 'produtoAtual'.
     * Atualiza os dados do produto pedido ao alterar o produto.
     */
    produtoAtual: {
      handler: function (atual) {
        if (!this.itemVenda || !this.itemVenda.produto) {
          return;
        }

        this.itemVenda.produto.id = atual ? atual.id : null;
        this.itemVenda.produto.espessura = atual ? atual.espessura : null;
        this.itemVenda.composicao = atual ? atual.composicao : null;

        if (atual && atual.altura && atual.altura.valor) {
          this.itemVenda.altura.paraCalculo = atual.altura.valor;
          this.itemVenda.altura.real = atual.altura.valor;
        }

        if (atual && atual.largura && atual.largura.valor) {
          this.itemVenda.largura = atual.largura.valor;
        }

        this.itemVenda.valorUnitario = atual ? atual.valorUnitario : null;
        this.validarEstoque();
      },
      deep: true
    },

    /**
     * Observador para a variável 'processoAtual'.
     * Atualiza os dados do produto pedido ao alterar o processo.
     */
    processoAtual: {
      handler: function (atual) {
        this.validarFormulario_(this.$el);

        if (!this.itemVenda) {
          return;
        }

        this.itemVenda.idProcesso = atual ? atual.id : null;

        if (atual && atual.aplicacao) {
          this.aplicacaoAtual = {
            id: atual.aplicacao.id,
            codigo: atual.aplicacao.codigo
          };
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'aplicacaoAtual'.
     * Atualiza os dados do produto pedido ao alterar a aplicação.
     */
    aplicacaoAtual: {
      handler: function (atual) {
        this.validarFormulario_(this.$el);

        if (!this.itemVenda) {
          return;
        }

        this.itemVenda.idAplicacao = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'processoFilhasAtual'.
     * Atualiza os dados do produto pedido ao alterar o processo.
     */
    processoFilhasAtual: {
      handler: function (atual) {
        if (!this.itemVenda) {
          return;
        }

        this.itemVenda.idProcessoFilhas = atual ? atual.id : null;

        if (atual && atual.aplicacao) {
          this.aplicacaoFilhasAtual = {
            id: atual.aplicacao.id,
            codigo: atual.aplicacao.codigo
          };
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'aplicacaoFilhasAtual'.
     * Atualiza os dados do produto pedido ao alterar a aplicação.
     */
    aplicacaoFilhasAtual: {
      handler: function (atual) {
        if (!this.itemVenda) {
          return;
        }

        this.itemVenda.idAplicacaoFilhas = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'itemVenda.altura'.
     * Valida o estoque para o produto atual.
     */
    'itemVenda.altura': {
      handler: function(atual) {
        this.validarEstoque();

        if (!this.dadosValidacaoProdutoAtual) {
          return;
        }

        this.dadosValidacaoProdutoAtual.altura = atual ? atual.real : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'itemVenda.quantidade'.
     * Valida o estoque para o produto atual.
     */
    'itemVenda.quantidade': function() {
      this.validarEstoque();
    },

    /**
     * Observador para a variável 'itemVenda.areaEmM2'.
     * Valida o estoque para o produto atual.
     */
    'itemVenda.areaEmM2': {
      handler: function() {
        this.validarEstoque();
      },
      deep: true
    },

    /**
     * Observador para a variável 'itemVenda.descontoPorQuantidade'.
     * Atualiza o percentual de desconto por quantidade no objeto de dados adicionais.
     */
    'itemVenda.descontoPorQuantidade': {
      handler: function (atual) {
        if (!this.dadosValidacaoProdutoAtual) {
          return;
        }

        this.dadosValidacaoProdutoAtual.percentualDescontoQuantidade = atual ? atual.percentual : 0;
      },
      deep: true
    },

    /**
     * Observador para a variável 'controleAtualizacao'.
     * Emite um evento informando que a lista de produtos foi alterada.
     */
    controleAtualizacao: function () {
      this.$emit('lista-atualizada');
    },

    /**
     * Observador para a variável 'dadosValidacaoProduto'.
     * Atualiza a variável interna com o novo valor.
     */
    dadosValidacaoProduto: {
      handler: function (atual) {
        this.dadosValidacaoProdutoAtual = atual;
      },
      deep: true
    }
  },

  template: '#ListaItensVenda-template'
});
