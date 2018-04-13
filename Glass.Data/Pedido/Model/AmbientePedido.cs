using System;
using GDA;
using Glass.Data.DAL;
using System.Xml.Serialization;
using Glass.Configuracoes;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AmbientePedidoDAO))]
    [PersistenceClass("ambiente_pedido")]
    public class AmbientePedido : IAmbienteCalculo
    {
        #region Construtores

        public AmbientePedido()
        {
            TipoAcrescimo = 2;
            TipoDesconto = 2;
        }

        #endregion

        #region Propriedades

        [Log("Identificador do Ambiente")]
        [PersistenceProperty("IDAMBIENTEPEDIDO", PersistenceParameterType.IdentityKey)]
        public uint IdAmbientePedido { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint? IdProd { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDITEMPROJETO")]
        public uint? IdItemProjeto { get; set; }

        [PersistenceProperty("AMBIENTE")]
        public string Ambiente { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("QTDE")]
        public int? Qtde { get; set; }

        [PersistenceProperty("ALTURA")]
        public int? Altura { get; set; }

        [PersistenceProperty("LARGURA")]
        public int? Largura { get; set; }

        [PersistenceProperty("REDONDO")]
        public bool Redondo { get; set; }

        [PersistenceProperty("TIPOACRESCIMO")]
        public int TipoAcrescimo { get; set; }

        [PersistenceProperty("ACRESCIMO")]
        public decimal Acrescimo { get; set; }

        [PersistenceProperty("TIPODESCONTO")]
        public int TipoDesconto { get; set; }

        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }

        [PersistenceProperty("IDPROCESSO")]
        public uint? IdProcesso { get; set; }

        [PersistenceProperty("IDAPLICACAO")]
        public uint? IdAplicacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        private decimal _totalProdutos;

        [XmlIgnore]
        [PersistenceProperty("TOTALPRODUTOS", DirectionParameter.InputOptional)]
        public decimal TotalProdutos
        {
            get { return _totalProdutos - (!PedidoConfig.RatearDescontoProdutos ? ValorDescontoAtual : 0); }
            set { _totalProdutos = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("VALORACRESCIMO", DirectionParameter.InputOptional)]
        public decimal ValorAcrescimo { get; set; }

        [XmlIgnore]
        [PersistenceProperty("VALORDESCONTO", DirectionParameter.InputOptional)]
        public decimal ValorDesconto { get; set; }

        [PersistenceProperty("CODINTERNO", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [XmlIgnore]
        [PersistenceProperty("OBSPROJ", DirectionParameter.InputOptional)]
        public string ObsProj { get; set; }

        [PersistenceProperty("CODPROCESSO", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [PersistenceProperty("CODAPLICACAO", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        private double _totM;

        [XmlIgnore]
        [PersistenceProperty("TOTM", DirectionParameter.InputOptional)]
        public double TotM
        {
            get
            {
                if (IdProd != null)
                {
                    float espessura = ProdutoDAO.Instance.ObtemEspessura((int)IdProd.Value);
                    return Math.Round(Glass.Global.CalculosFluxo.ArredondaM2(Largura.Value, Altura.Value, Qtde.Value, (int)IdProd.Value, Redondo, espessura, false), 2);
                }
                else
                    return _totM;
            }
            set { _totM = value; }
        }

        [PersistenceProperty("Ncm", DirectionParameter.InputOptional)]
        public string Ncm { get; set; }

        #endregion

        #region Propriedades de Suporte

        #region Métodos internos estáticos

        internal static decimal GetValorDesconto(AmbientePedido a)
        {
            return a.TipoDesconto == 1 ? (decimal)a._totalProdutos * (a.Desconto / 100) : a.Desconto;
        }

        internal static decimal GetValorAcrescimo(AmbientePedido a)
        {
            return a.TipoAcrescimo == 1 ? (decimal)a._totalProdutos * (a.Acrescimo / 100) : a.Acrescimo;
        }

        internal static decimal GetTotalSemDesconto(AmbientePedido a)
        {
            return (decimal)a._totalProdutos + (!PedidoConfig.RatearDescontoProdutos ? GetValorDesconto(a) : 0);
        }

        internal static decimal GetTotalSemAcrescimo(AmbientePedido a)
        {
            return (decimal)a._totalProdutos - GetValorAcrescimo(a);
        }

        #endregion

        [XmlIgnore]
        public string PecaVidro
        {
            get 
            {
                string altLarg = (Altura != null && Largura != null ? " " +
                    (PedidoConfig.EmpresaTrabalhaAlturaLargura ? Altura : Largura) + "x" + (Redondo ? 0 :
                    PedidoConfig.EmpresaTrabalhaAlturaLargura ? Largura : Altura) : "");

                return Ambiente + (Redondo && !BenefConfigDAO.Instance.CobrarRedondo() ? " REDONDO" : "") + altLarg; 
            }
        }

        [XmlIgnore]
        public string PecaVidroQtd
        {
            get
            {
                return Qtde + " " + PecaVidro + "   Tot. m²: " +
                    Glass.Global.CalculosFluxo.ArredondaM2(Largura.GetValueOrDefault(), Altura.GetValueOrDefault(), Qtde.GetValueOrDefault(), IdProd > 0 ? (int)IdProd.Value : 0, Redondo) +
                    (PedidoConfig.RelatorioPedido.ExibirTotalML ? "   Tot. ML: " + Math.Round((float)(2 * (Altura.GetValueOrDefault() + Largura.GetValueOrDefault()) * Qtde.GetValueOrDefault()) / 1000, 2) : "");
            }
        }

        [Log("Desconto")]
        [XmlIgnore]
        public string TextoDesconto
        {
            get { return TipoDesconto == 1 ? Desconto + "%" : Desconto.ToString("C"); }
        }

        [Log("Acrescimo")]
        [XmlIgnore]
        public string TextoAcrescimo
        {
            get { return TipoAcrescimo == 1 ? Acrescimo + "%" : Acrescimo.ToString("C"); }
        }

        /// <summary>
        /// Retorna o total do ambiente sem acréscimo/desconto.
        /// </summary>
        [XmlIgnore]
        public decimal TotalBruto
        {
            get { return AmbientePedidoDAO.Instance.GetTotalBruto(IdAmbientePedido); }
        }

        [XmlIgnore]
        public decimal ValorDescontoAtual
        {
            get { return GetValorDesconto(this); }
        }

        [XmlIgnore]
        public decimal ValorAcrescimoAtual
        {
            get { return GetValorAcrescimo(this); }
        }

        [XmlIgnore]
        public bool ProjetoVisible
        {
            get { return IdItemProjeto > 0; }
        }

        [XmlIgnore]
        public string DescrObsProj
        {
            get { return !String.IsNullOrEmpty(ObsProj) ? "(Obs. Projeto: " + ObsProj + ")" : String.Empty; }
        }

        [XmlIgnore]
        public string ImagemProjModPath { get; set; }

        #endregion

        #region IAmbienteCalculo

        uint IAmbienteCalculo.Id
        {
            get { return IdAmbientePedido; }
        }

        int IAmbienteCalculo.TipoDesconto
        {
            get { return TipoDesconto; }
        }

        decimal IAmbienteCalculo.Desconto
        {
            get { return Desconto; }
        }

        int IAmbienteCalculo.TipoAcrescimo
        {
            get { return TipoAcrescimo; }
        }

        decimal IAmbienteCalculo.Acrescimo
        {
            get { return Acrescimo; }
        }

        #endregion
    }
}