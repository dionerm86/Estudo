using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Fiscal.Negocios;

namespace Glass.Pedido.Negocios.Componentes
{
    /// <summary>
    /// Implementação que encapsula o produtos do pedido para um item de imposto.
    /// </summary>
    class ProdutosPedidoItemImposto : Fiscal.Negocios.IItemImposto
    {
        #region Propriedades

        /// <summary>
        /// Produto do pedido.
        /// </summary>
        public Data.Model.ProdutosPedido ProdutoPedido { get; }

        /// <summary>
        /// Natureza de operação vinculada ao item.
        /// </summary>
        public Fiscal.Negocios.Entidades.NaturezaOperacao NaturezaOperacao { get; }

        /// <summary>
        /// Produto.
        /// </summary>
        public Global.Negocios.Entidades.Produto Produto { get; }

        /// <summary>
        /// Valor total.
        /// </summary>
        public decimal Total => ProdutoPedido.Total;

        /// <summary>
        /// Finalidade de emissão
        /// </summary>
        public Sync.Fiscal.Enumeracao.NFe.FinalidadeEmissao FinalidadeEmissao => Sync.Fiscal.Enumeracao.NFe.FinalidadeEmissao.Normal;

        /// <summary>
        /// Valor do IOF
        /// </summary>
        public decimal ValorIof => 0;

        /// <summary>
        /// Valor da despesa aduaneira
        /// </summary>
        public decimal ValorDespesaAduaneira => 0;

        /// <summary>
        /// Código do valor fiscal
        /// </summary>
        public int? CodValorFiscal => ProdutoPedido.CodValorFiscal;

        /// <summary>
        /// Margem de valor agregado
        /// </summary>
        public decimal Mva => ProdutoPedido.Mva;

        /// <summary>
        /// Código de situação tributária do IPI
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstIpi? CstIpi => ProdutoPedido.CstIpi;

        /// <summary>
        /// Aliquota do IPI
        /// </summary>
        public float AliqIpi => ProdutoPedido.AliqIpi;

        /// <summary>
        /// Código de situação tributária do COFINS
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstCofins => ProdutoPedido.CstCofins;

        /// <summary>
        /// Aliquota do COFINS
        /// </summary>
        public float AliqCofins => ProdutoPedido.AliqCofins;

        /// <summary>
        /// Código de situação tributária do PIS
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstPis => ProdutoPedido.CstPis;

        /// <summary>
        /// Aliquota do PIS
        /// </summary>
        public float AliqPis => ProdutoPedido.AliqPis;

        /// <summary>
        /// Aliquota do ICMS ST
        /// </summary>
        public float AliqIcmsSt => ProdutoPedido.AliqIcmsSt;

        /// <summary>
        /// Aliquota do ICMS
        /// </summary>
        public float AliqIcms => ProdutoPedido.AliqIcms;

        /// <summary>
        /// Aliquota do FCP
        /// </summary>
        public float AliqFcp => ProdutoPedido.AliqFcp;

        /// <summary>
        /// Aliquota do FCP ST
        /// </summary>
        public float AliqFcpSt => ProdutoPedido.AliqFcpSt;

        /// <summary>
        /// Percentual de redução da base de calc. do ICMS
        /// </summary>
        public float PercRedBcIcms => ProdutoPedido.PercRedBcIcms;

        /// <summary>
        /// Percentual de diferimento do ICMS
        /// </summary>
        public decimal PercDiferimento => 0;

        /// <summary>
        /// Código de situação tributária do ICMS
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstIcms? Cst => (Sync.Fiscal.Enumeracao.Cst.CstIcms)ProdutoPedido.Cst;

        /// <summary>
        /// Código de situação tributária do ICMS SN
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CsosnIcms? Csosn => (Sync.Fiscal.Enumeracao.Cst.CsosnIcms)ProdutoPedido.Csosn;

        /// <summary>
        /// Verifica se os impostos devem ser calculados
        /// </summary>
        /// <returns></returns>
        public bool CalcularImpostos => true;

        /// <summary>
        /// Indica se possui pedidos
        /// </summary>
        public bool PossuiPedidos => true;

        /// <summary>
        /// Indica se possui compras
        /// </summary>
        public bool PossuiCompras => false;

        /// <summary>
        /// Indica se a nota fiscal está sendo importada pelo sistema.
        /// </summary>
        public bool NotaFiscalImportadaSistema => false;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="produtoPedido"></param>
        /// <param name="naturezaOperacao"></param>
        /// <param name="produto"></param>
        public ProdutosPedidoItemImposto(
            Data.Model.ProdutosPedido produtoPedido, 
            Fiscal.Negocios.Entidades.NaturezaOperacao naturezaOperacao,
            Global.Negocios.Entidades.Produto produto)
        {
            ProdutoPedido = produtoPedido;
            NaturezaOperacao = naturezaOperacao;
            Produto = produto;
        }

        #endregion
    }
}
