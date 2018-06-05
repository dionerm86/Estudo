using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios.Componentes.Calculadoras
{
    /// <summary>
    /// Implementação que encapsula o produto da nota fiscal para um item de imposto.
    /// </summary>
    class ProdutoNfItemImposto : IItemImposto
    {
        #region Propriedades

        /// <summary>
        /// Produto do pedido.
        /// </summary>
        public Data.Model.ProdutosNf ProdutoNf { get; }

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
        public decimal Total => ProdutoNf.Total;

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
        public int? CodValorFiscal => (int?)ProdutoNf.CodValorFiscal;

        /// <summary>
        /// Margem de valor agregado
        /// </summary>
        public float Mva => ProdutoNf.Mva;

        /// <summary>
        /// Código de situação tributária do IPI
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstIpi? CstIpi => (Sync.Fiscal.Enumeracao.Cst.CstIpi?)ProdutoNf.CstIpi;

        /// <summary>
        /// Aliquota do IPI
        /// </summary>
        public float AliqIpi => ProdutoNf.AliqIpi;

        /// <summary>
        /// Código de situação tributária do COFINS
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstCofins => 
            (Sync.Fiscal.Enumeracao.Cst.CstPisCofins ? )ProdutoNf.CstCofins;

        /// <summary>
        /// Aliquota do COFINS
        /// </summary>
        public float AliqCofins => ProdutoNf.AliqCofins;

        /// <summary>
        /// Código de situação tributária do PIS
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstPis => 
            (Sync.Fiscal.Enumeracao.Cst.CstPisCofins ? )ProdutoNf.CstPis;

        /// <summary>
        /// Aliquota do PIS
        /// </summary>
        public float AliqPis => ProdutoNf.AliqPis;

        /// <summary>
        /// Aliquota do ICMS ST
        /// </summary>
        public float AliqIcmsSt => ProdutoNf.AliqIcmsSt;

        /// <summary>
        /// Aliquota do ICMS
        /// </summary>
        public float AliqIcms => ProdutoNf.AliqIcms;

        /// <summary>
        /// Aliquota do FCP
        /// </summary>
        public float AliqFcp => ProdutoNf.AliqFcp;

        /// <summary>
        /// Aliquota do FCP ST
        /// </summary>
        public float AliqFcpSt => ProdutoNf.AliqFcpSt;

        /// <summary>
        /// Percentual de redução da base de calc. do ICMS
        /// </summary>
        public float PercRedBcIcms => ProdutoNf.PercRedBcIcms;

        /// <summary>
        /// Percentual de diferimento do ICMS
        /// </summary>
        public float PercDiferimento => ProdutoNf.PercDiferimento;

        /// <summary>
        /// Código de situação tributária do ICMS
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstIcms? Cst
        {
            get
            {
                var cst = 0;
                if (int.TryParse(ProdutoNf.Cst, out cst))
                    return (Sync.Fiscal.Enumeracao.Cst.CstIcms)cst;

                return null;
            }
        }

        /// <summary>
        /// Código de situação tributária do ICMS SN
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CsosnIcms? Csosn
        {
            get
            {
                var csosn = 0;
                if (int.TryParse(ProdutoNf.Csosn, out csosn))
                    return (Sync.Fiscal.Enumeracao.Cst.CsosnIcms)csosn;

                return null;
            }
        }

        /// <summary>
        /// Verifica se os impostos devem ser calculados
        /// </summary>
        /// <returns></returns>
        public bool CalcularImpostos => true;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="produtoNf"></param>
        /// <param name="naturezaOperacao"></param>
        /// <param name="produto"></param>
        public ProdutoNfItemImposto(
            Data.Model.ProdutosNf produtoNf,
            Fiscal.Negocios.Entidades.NaturezaOperacao naturezaOperacao,
            Global.Negocios.Entidades.Produto produto)
        {
            ProdutoNf = produtoNf;
            NaturezaOperacao = naturezaOperacao;
            Produto = produto;
        }

        #endregion
    }
}
