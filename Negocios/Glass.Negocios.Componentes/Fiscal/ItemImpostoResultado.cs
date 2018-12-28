using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;

namespace Glass.Fiscal.Negocios.Componentes
{

    /// <summary>
    /// Representa o resultado do item do calculo de imposto.
    /// </summary>
    class ItemImpostoResultado : IItemCalculoImpostoResultado, IProdutoIcmsSt
    {
        #region Propriedades

        /// <summary>
        /// Referência do item.
        /// </summary>
        public IItemImposto Referencia { get; }

        /// <summary>
        /// Natureza de operação.
        /// </summary>
        public Entidades.NaturezaOperacao NaturezaOperacao => Referencia.NaturezaOperacao;

        /// <summary>
        /// Produto.
        /// </summary>
        public Global.Negocios.Entidades.Produto Produto => Referencia.Produto;

        /// <summary>
        /// Valor total do item.
        /// </summary>
        public decimal Total => Referencia.Total;

        /// <summary>
        /// Valor do frete.
        /// </summary>
        public decimal ValorFrete { get; set; }

        /// <summary>
        /// Valor do seguro.
        /// </summary>
        public decimal ValorSeguro { get; set; }

        /// <summary>
        /// Valor das outras despesas.
        /// </summary>
        public decimal ValorOutrasDespesas { get; set; }

        /// <summary>
        /// Valor de desconto.
        /// </summary>
        public decimal ValorDesconto { get; set; }

        /// <summary>
        /// Código do valor fiscal
        /// </summary>
        public int? CodValorFiscal => Referencia.CodValorFiscal;

        /// <summary>
        /// Código de situação tributária do ICMS
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstIcms? Cst => Referencia.Cst;

        /// <summary>
        /// Percentual de redução da base de calc. do ICMS
        /// </summary>
        public float PercRedBcIcms => Referencia.PercRedBcIcms;

        /// <summary>
        /// Percentual de diferimento do ICMS
        /// </summary>
        public float PercDiferimento => Referencia.PercDiferimento;

        /// <summary>
        /// Valor do IOF.
        /// </summary>
        public decimal ValorIof => Referencia.ValorIof;

        /// <summary>
        /// Valor da despesa aduaneira.
        /// </summary>
        public decimal ValorDespesaAduaneira => Referencia.ValorDespesaAduaneira;

        /// <summary>
        /// Valor do IPI.
        /// </summary>
        public decimal ValorIpi { get; set; }

        /// <summary>
        /// Aliquota do IPI.
        /// </summary>
        public float AliqIpi { get; set; }

        /// <summary>
        /// Aliquota do ICMS.
        /// </summary>
        public float AliqIcms { get; set; }

        /// <summary>
        /// Base de calc. do ICMS
        /// </summary>
        public decimal BcIcms { get; set; }

        /// <summary>
        /// Base de calc. do ICMS, sem percentual de redução.
        /// </summary>
        public decimal BcIcmsSemReducao { get; set; }

        /// <summary>
        /// Valor do ICMS.
        /// </summary>
        public decimal ValorIcms { get; set; }

        /// <summary>
        /// Aliquota do fundo de combate a pobreza.
        /// </summary>
        public float AliqFcp { get; set; }

        /// <summary>
        /// Base de calc. do FCP
        /// </summary>
        public decimal BcFcp { get; set; }

        /// <summary>
        /// Valor do fundo de combate a probreza.
        /// </summary>
        public decimal ValorFcp { get; set; }

        /// <summary>
        /// Aliquota do ICMS ST
        /// </summary>
        public float AliqIcmsSt { get; set; }

        /// <summary>
        /// Base de calc. do ICMS ST
        /// </summary>
        public decimal BcIcmsSt { get; set; }

        /// <summary>
        /// Valor do calc. do ICMS ST.
        /// </summary>
        public decimal ValorIcmsSt { get; set; }

        /// <summary>
        /// Aliquota do FCP ST
        /// </summary>
        public float AliqFcpSt { get; set; }

        /// <summary>
        /// Valor do FCP ST
        /// </summary>
        public decimal ValorFcpSt { get; set; }

        /// <summary>
        /// Base de calc. do FCP ST
        /// </summary>
        public decimal BcFcpSt { get; set; }

        /// <summary>
        /// Código de situação tributária do PIS
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstPis => Referencia.CstPis;

        /// <summary>
        /// Base de calc. do PIS
        /// </summary>
        public decimal BcPis { get; set; }

        /// <summary>
        /// Aliquota do PIS
        /// </summary>
        public float AliqPis { get; set; }

        /// <summary>
        /// Valor do PIS.
        /// </summary>
        public decimal ValorPis { get; set; }

        /// <summary>
        /// Código de situação tributária do COFINS
        /// </summary>
        public Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstCofins => Referencia.CstCofins;

        /// <summary>
        /// Aliquota do COFINS
        /// </summary>
        public float AliqCofins { get; set; }

        /// <summary>
        /// Base de calc. do COFINS
        /// </summary>
        public decimal BcCofins { get; set; }

        /// <summary>
        /// Valor do Cofins.
        /// </summary>
        public decimal ValorCofins { get; set; }

        /// <summary>
        /// Identifica se deve calcular a aliquota do ICMS ST.
        /// </summary>
        public bool CalcularAliquotaIcmsSt { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="calcularAliquotaIcmsSt">Identifica se deve calcular a aliquota do ICMS ST.</param>
        public ItemImpostoResultado(IItemImposto item, bool calcularAliquotaIcmsSt)
        {
            Referencia = item;
            AliqIpi = item.AliqIpi;
            AliqIcms = item.AliqIcms;
            AliqFcp = item.AliqFcp;
            AliqIcmsSt = item.AliqIcmsSt;
            AliqFcpSt = item.AliqFcpSt;
            AliqPis = item.AliqPis;
            AliqCofins = item.AliqCofins;
            CalcularAliquotaIcmsSt = calcularAliquotaIcmsSt;
        }

        #endregion

        #region Membros de IProdutoIcmsSt

        int IProdutoIcmsSt.IdProd => Produto.IdProd;

        decimal IProdutoIcmsSt.Total => Total;

        float IProdutoIcmsSt.MvaProdutoNf => (float)Referencia.Mva;

        float IProdutoIcmsSt.AliquotaIcms => AliqIcms;

        decimal IProdutoIcmsSt.ValorIcms => ValorIcms;

        float IProdutoIcmsSt.AliquotaIpi => AliqIpi;

        decimal IProdutoIcmsSt.ValorIpi => ValorIpi;

        float IProdutoIcmsSt.AliquotaIcmsSt => AliqIcmsSt;

        decimal IProdutoIcmsSt.ValorDesconto => ValorDesconto;

        decimal IProdutoIcmsSt.ValorFrete => ValorFrete;

        decimal IProdutoIcmsSt.ValorSeguro => ValorSeguro;

        decimal IProdutoIcmsSt.ValorOutrasDespesas => ValorOutrasDespesas;

        float IProdutoIcmsSt.PercentualReducaoBaseCalculo => PercRedBcIcms;

        #endregion
    }
}
