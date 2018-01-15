using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Glass.Data.Model;

namespace Glass.Api.Projeto
{
    /// <summary>
    /// Representa o material do item de projeto.
    /// </summary>
    public class MaterialItemProjeto : IMaterialItemProjeto
    {
        #region Contrutores

        public MaterialItemProjeto()
        {

        }

        public MaterialItemProjeto(IMaterialItemProjeto mip)
        {
            IdMaterItemProj = Guid.NewGuid();
            IdProd = mip.IdProd;
            IdProcesso = mip.IdProcesso;
            IdAplicacao = mip.IdAplicacao;
            Qtde = mip.Qtde;
            Valor = mip.Valor;
            Altura = mip.Altura;
            AlturaCalc = mip.AlturaCalc;
            Largura = mip.Largura;
            TotM = mip.TotM;
            Total = mip.Total;
            Espessura = mip.Espessura;
            TotalM2Calc = mip.TotM2Calc;
            ValorBenef = mip.ValorBenef;
            Redondo = mip.Redondo;
            ValorAcrescimo = mip.ValorAcrescimo;
            ValorDesconto = mip.ValorDesconto;
            ValorDescontoCliente = mip.ValorDescontoCliente;
            ValorAcrescimoCliente = mip.ValorAcrescimoCliente;
            GrauCorte = mip.GrauCorte;
            AliqIcms = mip.AliqIcms;
            ValorIcms = mip.ValorIcms;
            Custo = mip.Custo;
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do material.
        /// </summary>
        public Guid IdMaterItemProj { get; set; }

        /// <summary>
        /// Identificador do material de origem.
        /// </summary>
        public Guid? IdMaterItemProjOrig { get; set; }

        /// <summary>
        /// Identificador do material do modelo de projeto.
        /// </summary>
        public uint? IdMaterProjMod { get; set; }

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        public uint IdProd { get; set; }

        /// <summary>
        /// Identificador da peça do item de projeto associado.
        /// </summary>
        public Guid? IdPecaItemProj { get; set; }

        /// <summary>
        /// Identificador do processo.
        /// </summary>
        public uint? IdProcesso { get; set; }

        /// <summary>
        /// Identificador da aplicação.
        /// </summary>
        public uint? IdAplicacao { get; set; }

        /// <summary>
        /// Quantidade.
        /// </summary>
        public float Qtde { get; set; }

        /// <summary>
        /// Valor.
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Altura.
        /// </summary>
        public float Altura { get; set; }

        /// <summary>
        /// Altura calculada.
        /// </summary>
        public float AlturaCalc { get; set; }

        /// <summary>
        /// Largura.
        /// </summary>
        public int Largura { get; set; }

        /// <summary>
        /// Total de m².
        /// </summary>
        public float TotM { get; set; }

        /// <summary>
        /// Total.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Espessura.
        /// </summary>
        public float Espessura { get; set; }

        /// <summary>
        /// Total de m² calculado.
        /// </summary>
        public float TotalM2Calc { get; set; }

        /// <summary>
        /// Valor do beneficiamento.
        /// </summary>
        public decimal ValorBenef { get; set; }

        /// <summary>
        /// Identifica se é redondo.
        /// </summary>
        public bool Redondo { get; set; }

        /// <summary>
        /// Observação.
        /// </summary>
        public string Obs { get; set; }

        /// <summary>
        /// Valor unitário bruto.
        /// </summary>
        public decimal ValorUnitarioBruto { get; set; }

        /// <summary>
        /// Total bruto.
        /// </summary>
        public decimal TotalBruto { get; set; }

        /// <summary>
        /// Grua de corte.
        /// </summary>
        public GrauCorteEnum? GrauCorte { get; set; }
        
        /// <summary>
        /// Aliquota ICMS.
        /// </summary>
        public float AliqIcms { get; set; }

        /// <summary>
        /// Valor do ICMS.
        /// </summary>
        public decimal ValorIcms { get; set; }

        /// <summary>
        /// Custo.
        /// </summary>
        public decimal Custo { get; set; }

        /// <summary>
        /// Valor de acréscimo.
        /// </summary>
        public decimal ValorAcrescimo { get; set; }

        /// <summary>
        /// Valor de desconto.
        /// </summary>
        public decimal ValorDesconto { get; set; }

        /// <summary>
        /// Valor de desconto do cliente.
        /// </summary>
        public decimal ValorDescontoCliente { get; set; }

        /// <summary>
        /// Valor de acréscimo do cliente.
        /// </summary>
        public decimal ValorAcrescimoCliente { get; set; }

        #endregion

        #region IMaterialItemProjeto Members

        float IMaterialItemProjeto.TotM2Calc
        {
            get
            {
                return TotalM2Calc;
            }

            set
            {
                TotalM2Calc = value;
            }
        }

        #endregion
    }
}