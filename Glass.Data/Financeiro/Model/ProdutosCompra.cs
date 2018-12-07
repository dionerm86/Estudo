using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutosCompraDAO))]
	[PersistenceClass("produtos_compra")]
	public class ProdutosCompra
    {
        #region Propriedades

        [PersistenceProperty("IdProdCompra", PersistenceParameterType.IdentityKey)]
        public uint IdProdCompra { get; set; }

        [PersistenceProperty("IDCOMPRA")]
        public uint IdCompra { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("IDPRODPED")]
        public uint? IdProdPed { get; set; }

        [PersistenceProperty("IDMATERITEMPROJ")]
        public uint? IdMaterItemProj { get; set; }

        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        [PersistenceProperty("QTDEENTRADA")]
        public float QtdeEntrada { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        private decimal _total;

		[PersistenceProperty("TOTAL")]
        public decimal Total
		{
			get { return _total; }
			set { _total = Math.Round(value, 2); }
		}

        [PersistenceProperty("ALTURA")]
        public float Altura { get; set; }

        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [PersistenceProperty("TOTM")]
        public float TotM { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("REDONDO")]
        public bool Redondo { get; set; }

        [PersistenceProperty("VALORBENEF")]
        public decimal ValorBenef { get; set; }

        [PersistenceProperty("ESPESSURA")]
        public float Espessura { get; set; }

        [PersistenceProperty("NAOCOBRARVIDRO")]
        public bool NaoCobrarVidro { get; set; }

        [PersistenceProperty("DESCRICAOITEMGENERICO")]
        public string DescricaoItemGenerico { get; set; }
 
        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _descrProduto;

        [PersistenceProperty("DescrProduto", DirectionParameter.InputOptional)]
        public string DescrProduto
        {
            get { return ProdutoDAO.Instance.GetDescrProduto(_descrProduto, DescricaoItemGenerico); }
            set { _descrProduto = value; }
        }

        [PersistenceProperty("CodInterno", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("IdGrupoProd", DirectionParameter.InputOptional)]
        public uint IdGrupoProd { get; set; }

        [PersistenceProperty("IdSubgrupoProd", DirectionParameter.InputOptional)]
        public uint IdSubgrupoProd { get; set; }

        [PersistenceProperty("LocalArmazenagem", DirectionParameter.InputOptional)]
        public string LocalArmazenagem { get; set; }

        [PersistenceProperty("QtdeComprando", DirectionParameter.InputOptional)]
        public double QtdeComprando { get; set; }

        [PersistenceProperty("TotMComprando", DirectionParameter.InputOptional)]
        public double TotMComprando { get; set; }

        [PersistenceProperty("ValorFiscal", DirectionParameter.InputOptional)]
        public decimal ValorFiscal { get; set; }

        [PersistenceProperty("DATAFABRICA", DirectionParameter.InputOptional)]
        public DateTime? DataFabrica { get; set; }

        #endregion

        #region Propriedades de Suporte

        public float Peso
        {
            get
            {
                return (float)Math.Round(Utils.CalcPeso((int)IdProd, Espessura, TotM, Qtde, Altura, false), 2);
            }
        }

        public string CodDescrProd
        {
            get { return CodInterno + " - " + DescrProduto; }
        }

        public string TotalM2String
        {
            get { return TotM.ToString(); }
            set { TotM = value != null ? Single.Parse(value.Replace("R$", string.Empty).Replace(" ", string.Empty).Replace('.', ','), System.Globalization.NumberStyles.AllowDecimalPoint) : 0; }
        }

        public int TipoCalc
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, (int)IdGrupoProd, (int)IdSubgrupoProd, false); }
        }

        public bool AlturaEnabled
        {
            get { return Glass.Calculos.AlturaEnabled(TipoCalc); }
        }

        public bool LarguraEnabled
        {
            get { return Glass.Calculos.LarguraEnabled(TipoCalc); }
        }

        public string IsVidro
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd).ToString().ToLower(); }
        }

        public bool BenefVisible
        {
            get
            {
                return (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd) || Geral.UsarBeneficiamentosTodosOsGrupos) && !Geral.NaoVendeVidro();
            }
        }

        public float QtdeEntradaRestante
        {
            get { return Qtde - QtdeEntrada; }
        }

        public string QtdeEntradaEstoque
        {
            get
            {
                return QtdeEntradaRestante.ToString();
            }
        }

        public float QtdMarcadaEntrada { get; set; }

        public float Comprando
        {
            get
            {
                return (float)(TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                    TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto ? TotMComprando : QtdeComprando);
            }
        }

        public string DescricaoComprando
        {
            get
            {
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo(TipoCalc, true);
                return Comprando + descrTipoCalculo;
            }
        }

        public string ValorFiscalString
        {
            get
            {
                return ValorFiscal.ToString();
            }
        }

        #endregion

        #region Propriedades do Beneficiamento

        private List<ProdutosCompraBenef> _beneficiamentos = null;

        public GenericBenefCollection Beneficiamentos
        {
            get
            {
                try
                {
                    if (!ProdutoDAO.Instance.CalculaBeneficiamento((int)IdProd))
                        _beneficiamentos = new List<ProdutosCompraBenef>();

                    if (_beneficiamentos == null)
                        _beneficiamentos = new List<ProdutosCompraBenef>(ProdutosCompraBenefDAO.Instance.GetByProdCompra(IdProdCompra));
                }
                catch
                {
                    _beneficiamentos = new List<ProdutosCompraBenef>();
                }

                return _beneficiamentos;
            }
            set { _beneficiamentos = value; }
        }

        /// <summary>
        /// Recarrega a lista de beneficiamentos do banco de dados.
        /// </summary>
        public void RefreshBeneficiamentos()
        {
            _beneficiamentos = null;
        }

        private string _descrBeneficiamentos;

         public string DescrBeneficiamentos
         {
            get 
            {
                if (!String.IsNullOrEmpty(_descrBeneficiamentos))
                    return _descrBeneficiamentos;

                return Beneficiamentos.DescricaoBeneficiamentos;
            }
            set { _descrBeneficiamentos = value; }
         }

        #endregion

        #region Propriedades da Nota Fiscal

        [PersistenceProperty("QTDNF", DirectionParameter.InputOptional)]
        public long QtdNf { get; set; }

        [PersistenceProperty("TOTM2NF", DirectionParameter.InputOptional)]
        public double TotM2Nf { get; set; }

        [PersistenceProperty("TOTALNF", DirectionParameter.InputOptional)]
        public decimal TotalNf { get; set; }

        [PersistenceProperty("VALORBENEFNF", DirectionParameter.InputOptional)]
        public decimal ValorBenefNf { get; set; }

        /// <summary>
        /// Propriedade usada para salvar a qtd do produto para verificar com a qtd liberada quando for gerar a nota.
        /// </summary>
        [PersistenceProperty("QTDEORIGINAL", DirectionParameter.InputOptional)]
        public double QtdeOriginal { get; set; }

        #endregion
    }
}
