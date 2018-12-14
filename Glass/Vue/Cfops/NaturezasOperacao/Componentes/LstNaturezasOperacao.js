const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista()],

  data: {
    idCfop: GetQueryString('idCfop'),
    configuracoes: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
    natureza: {},
    naturezaOriginal: {},
    csosnAtual: {},
    cstIcmsAtual: {},
    cstIpiAtual: {},
    cstPisCofinsAtual: {}
  },

  methods: {
    /**
     * Busca a lista de naturezas de operação para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      
      return Servicos.Cfops.NaturezasOperacao.obterLista(this.idCfop, filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna os itens para o controle de CSOSN's.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensCsosn: function () {
      return Servicos.Cfops.NaturezasOperacao.obterCsosns();
    },

    /**
     * Retorna os itens para o controle de CST's de ICMS.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensCstIcms: function () {
      return Servicos.Cfops.NaturezasOperacao.obterCstsIcms();
    },

    /**
     * Retorna os itens para o controle de CST's de IPI.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensCstIpi: function () {
      return Servicos.Cfops.NaturezasOperacao.obterCstsIpi();
    },

    /**
     * Retorna os itens para o controle de CST's de PIS/COFINS.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensCstPisCofins: function () {
      return Servicos.Cfops.NaturezasOperacao.obterCstsPisCofins();
    },

    /**
     * Inicia o cadastro de natureza de operação.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que uma natureza de operação será editada.
     * @param {Object} natureza A natureza de operação que será editada.
     * @param {number} numeroLinha O número da linha que contém a regra de natureza de operação que será editada.
     */
    editar: function (natureza, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(natureza);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui uma natureza de operação.
     * @param {Object} natureza A natureza de operação que será excluída.
     */
    excluir: function (natureza) {
      var vm = this;

      Servicos.Cfops.NaturezasOperacao.excluir(natureza.id)
        .then(function (resposta) {
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Insere uma nova natureza de operação.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      this.natureza.idCfop = GetQueryString('idCfop');

      var vm = this;

      Servicos.Cfops.NaturezasOperacao.inserir(this.natureza)
        .then(function (resposta) {
          vm.atualizarLista();
          vm.cancelar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Atualiza os dados da natureza de operação.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var naturezaAtualizar = this.patch(this.natureza, this.naturezaOriginal);
      var vm = this;

      Servicos.Cfops.NaturezasOperacao.atualizar(this.natureza.id, naturezaAtualizar)
        .then(function (resposta) {
          vm.atualizarLista();
          vm.cancelar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Cancela a edição ou cadastro da natureza de operação.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de natureza de operação.
     * @param {?Object} [natureza=null] A natureza de operação que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (natureza) {
      this.csosnAtual = natureza && natureza.dadosIcms ? this.clonar(natureza.dadosIcms.csosn) : null;
      this.cstIcmsAtual = natureza && natureza.dadosIcms ? this.clonar(natureza.dadosIcms.cstIcms) : null;
      this.cstIpiAtual = natureza && natureza.dadosIpi ? this.clonar(natureza.dadosIpi.cstIpi) : null;
      this.cstPisCofinsAtual = natureza && natureza.dadosPisCofins ? this.clonar(natureza.dadosPisCofins.cstPisCofins) : null;
      
      this.natureza = {
        id: natureza ? natureza.id : null,
        idCfop: natureza ? natureza.idCfop : null,
        codigo: natureza ? natureza.codigo : null,
        mensagem: natureza ? natureza.mensagem : null,
        alterarEstoqueFiscal: natureza ? natureza.alterarEstoqueFiscal : null,
        calculoDeEnergiaEletrica: natureza ? natureza.calculoDeEnergiaEletrica : null,
        ncm: natureza ? natureza.ncm : null,
        dadosIcms: {
          cstIcms: natureza && natureza.dadosIcms && natureza.dadosIcms.cstIcms ? natureza.dadosIcms.cstIcms.codigo : null,
          csosn: natureza && natureza.dadosIcms && natureza.dadosIcms.csosn ? natureza.dadosIcms.csosn.codigo : null,
          calcularIcms: natureza && natureza.dadosIcms ? natureza.dadosIcms.calcularIcms : null,
          calcularIcmsSt: natureza && natureza.dadosIcms ? natureza.dadosIcms.calcularIcmsSt : null,
          ipiIntegraBcIcms: natureza && natureza.dadosIcms ? natureza.dadosIcms.ipiIntegraBcIcms : null,
          debitarIcmsDesoneradoTotalNf: natureza && natureza.dadosIcms ? natureza.dadosIcms.debitarIcmsDesoneradoTotalNf : null,
          percentualReducaoBcIcms: natureza && natureza.dadosIcms ? natureza.dadosIcms.percentualReducaoBcIcms : null,
          percentualDiferimento: natureza && natureza.dadosIcms ? natureza.dadosIcms.percentualDiferimento : null,
          calcularDifal: natureza && natureza.dadosIcms ? natureza.dadosIcms.calcularDifal : null
        },
        dadosIpi: {
          cstIpi: natureza && natureza.dadosIpi && natureza.dadosIpi.cstIpi ? natureza.dadosIpi.cstIpi.id : null,
          calcularIpi: natureza && natureza.dadosIpi ? natureza.dadosIpi.calcularIpi : null,
          freteIntegraBcIpi: natureza && natureza.dadosIpi ? natureza.dadosIpi.freteIntegraBcIpi : null,
          codigoEnquadramentoIpi: natureza && natureza.dadosIpi ? natureza.dadosIpi.codigoEnquadramentoIpi : null
        },
        dadosPisCofins: {
          cstPisCofins: natureza && natureza.dadosPisCofins && natureza.dadosPisCofins.cstPisCofins ? natureza.dadosPisCofins.cstPisCofins.id : null,
          calcularPis: natureza && natureza.dadosPisCofins ? natureza.dadosPisCofins.calcularPis : null,
          calcularCofins: natureza && natureza.dadosPisCofins ? natureza.dadosPisCofins.calcularCofins : null
        }
      };

      this.naturezaOriginal = this.clonar(this.natureza);
    },

    /**
     * Função que indica se o formulário de naturezas de operação possui valores válidos de acordo com os controles.
     * @param {Object} botao O botão que foi disparado no controle.
     * @returns {boolean} Um valor que indica se o formulário está válido.
     */
    validarFormulario_: function (botao) {
      var form = botao.form || botao;
      while (form.tagName.toLowerCase() !== 'form') {
        form = form.parentNode;
      }

      return form.checkValidity();
    },

    /**
     * Força a atualização da lista de natureza de operação, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  },

  watch: {
    /**
     * Observador para a variável 'csosnAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    csosnAtual: {
      handler: function (atual) {
        if (this.natureza && this.natureza.dadosIcms) {
          this.natureza.dadosIcms.csosn = atual ? atual.codigo : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'csosnAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    cstIcmsAtual: {
      handler: function (atual) {
        if (this.natureza && this.natureza.dadosIcms) {
          this.natureza.dadosIcms.cstIcms = atual ? atual.codigo : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'cstIpiAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    cstIpiAtual: {
      handler: function (atual) {
        if (this.natureza && this.natureza.dadosIpi) {
          this.natureza.dadosIpi.cstIpi = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'cstPisCofinsAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    cstPisCofinsAtual: {
      handler: function (atual) {
        if (this.natureza && this.natureza.dadosPisCofins) {
          this.natureza.dadosPisCofins.cstPisCofins = atual ? atual.id : null;
        }
      },
      deep: true
    }
  }
});
