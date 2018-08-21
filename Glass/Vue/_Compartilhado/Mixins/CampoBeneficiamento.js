var Mixins = Mixins || {};

/**
 * Objeto com os métodos comuns aos campos de beneficiamento.
 */
Mixins.CampoBeneficiamento = {
  methods: {
    /**
     * Cria um beneficiamento para inclusão na lista de itens selecionados.
     * @param {?Object} dados Um objeto com os dados que serão preenchidos do objeto.
     * @returns {Object} Um objeto com os dados padrão para os beneficiamentos selecionados.
     */
    criarBeneficiamento: function(dados) {
      return {
        id: dados ? dados.id || 0 : 0,
        altura: dados ? dados.altura || null : null,
        largura: dados ? dados.largura || null : null,
        espessura: dados ? dados.espessura || null : null,
        quantidade: dados ? dados.quantidade || null : null,
        valorUnitario: dados ? dados.valorUnitario || null : null,
        valorTotal: dados ? dados.valorTotal || null : null,
        custoTotal: dados ? dados.custoTotal || null : null
      };
    }
  }
};
