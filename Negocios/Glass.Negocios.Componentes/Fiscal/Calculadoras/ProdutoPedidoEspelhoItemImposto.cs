using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios.Componentes.Calculadoras
{
    /// <summary>
    /// Implementação que encapsula os produtos do pedido para umte item de imposto.
    /// </summary>
    class ProdutoPedidoEspelhoItemImposto : IItemImposto
    {
        #region Propriedades

        /// <summary>
        /// Provedor do código do valor fiscal.
        /// </summary>
        private IProvedorCodValorFiscal ProvedorCodValorFiscal { get; }

        /// <summary>
        /// Produto do pedido.
        /// </summary>
        public Data.Model.ProdutosPedidoEspelho ProdutoPedido { get; }

        /// <summary>
        /// Loja associada.
        /// </summary>
        public Global.Negocios.Entidades.Loja Loja { get; }

        /// <summary>
        /// Natureza de operação vinculada ao item.
        /// </summary>
        public Entidades.NaturezaOperacao NaturezaOperacao { get; }

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
        public Sync.Fiscal.Enumeracao.NFe.FinalidadeEmissao FinalidadeEmissao => 
            Sync.Fiscal.Enumeracao.NFe.FinalidadeEmissao.Normal;

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
        public int? CodValorFiscal =>
            ProvedorCodValorFiscal.ObterCodValorFiscal(Sync.Fiscal.Enumeracao.NFe.TipoDocumento.Saida, Loja, Cst);

        /// <summary>
        /// Margem de valor agregado
        /// </summary>
        public float Mva { get; }

        /// <summary>
        /// Código de situação tributária do IPI
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstIpi? CstIpi =>
            (Sync.Fiscal.Enumeracao.Cst.CstIpi)(int)(NaturezaOperacao?.CstIpi ?? Data.Model.ProdutoCstIpi.SaidaTributada);

        /// <summary>
        /// Aliquota do IPI
        /// </summary>
        public float AliqIpi => ProdutoPedido.AliqIpi;

        /// <summary>
        /// Código de situação tributária do COFINS
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstCofins
        {
            get
            {
                if (NaturezaOperacao?.CstPisCofins.HasValue ?? false)
                    return (Sync.Fiscal.Enumeracao.Cst.CstPisCofins?)NaturezaOperacao.CstPisCofins;

                switch (Loja?.Crt)
                {
                    case Data.Model.CrtLoja.LucroPresumido:
                        return Sync.Fiscal.Enumeracao.Cst.CstPisCofins.OperacaoTributavelAliquotaBasica;
                    case Data.Model.CrtLoja.LucroReal:
                        return Sync.Fiscal.Enumeracao.Cst.CstPisCofins.OperacaoTributavelAliquotaBasica;
                    default:
                        return Sync.Fiscal.Enumeracao.Cst.CstPisCofins.OperacaoSemIncidenciaContribuicao;
                }
            }
        }

        /// <summary>
        /// Aliquota do COFINS
        /// </summary>
        public float AliqCofins => ProdutoPedido.AliqCofins;

        /// <summary>
        /// Código de situação tributária do PIS
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstPis => CstCofins;

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
        public float PercRedBcIcms => NaturezaOperacao?.PercReducaoBcIcms ?? 0f;

        /// <summary>
        /// Percentual de diferimento do ICMS
        /// </summary>
        public float PercDiferimento => NaturezaOperacao?.PercDiferimento ?? 0f;

        /// <summary>
        /// Código de situação tributária do ICMS
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstIcms? Cst
        {
            get
            {
                int value = 0;

                if (int.TryParse(NaturezaOperacao.CstIcms ?? Produto.Cst, out value))
                    return (Sync.Fiscal.Enumeracao.Cst.CstIcms)value;

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
                int value = 0;

                if (int.TryParse(NaturezaOperacao.Csosn ?? Produto.Csosn, out value))
                    return (Sync.Fiscal.Enumeracao.Cst.CsosnIcms)value;

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
        /// <param name="produtoPedido"></param>
        /// <param name="loja"></param>
        /// <param name="naturezaOperacao"></param>
        /// <param name="produto"></param>
        /// <param name="mva"></param>
        public ProdutoPedidoEspelhoItemImposto(
            Data.Model.ProdutosPedidoEspelho produtoPedido,
            Global.Negocios.Entidades.Loja loja,
            Entidades.NaturezaOperacao naturezaOperacao,
            Global.Negocios.Entidades.Produto produto,
            float mva, IProvedorCodValorFiscal provedorCodValorFiscal)
        {
            ProdutoPedido = produtoPedido;
            Loja = loja;
            NaturezaOperacao = naturezaOperacao;
            Produto = produto;
            Mva = mva;
            ProvedorCodValorFiscal = provedorCodValorFiscal;
        }

        #endregion
    }
}
