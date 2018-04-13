using System;
using GDA;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AmbientePedidoEspelhoDAO))]
    [PersistenceClass("ambiente_pedido_espelho")]
    public class AmbientePedidoEspelho : IAmbienteCalculo
    {
        #region Propriedades

        [PersistenceProperty("IDAMBIENTEPEDIDO", PersistenceParameterType.IdentityKey)]
        public uint IdAmbientePedido { get; set; }

        [PersistenceProperty("IDAMBIENTEPEDIDOORIG")]
        public uint? IdAmbientePedidoOriginal { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint? IdProd { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDITEMPROJETO")]
        public uint? IdItemProjeto { get; set; }

        [PersistenceProperty("AMBIENTE")]
        public string Ambiente { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("QTDE")]
        public int? Qtde { get; set; }

        private int? _qtdeImpresso;

        [PersistenceProperty("QTDEIMPRESSO")]
        public int? QtdeImpresso
        {
            get { return _qtdeImpresso != null ? _qtdeImpresso : 0; }
            set { _qtdeImpresso = value; }
        }

        [PersistenceProperty("ALTURA")]
        public int? Altura { get; set; }

        [PersistenceProperty("LARGURA")]
        public int? Largura { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("REDONDO")]
        public bool Redondo { get; set; }

        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }

        private int _tipoDesconto;

        [PersistenceProperty("TIPODESCONTO")]
        public int TipoDesconto
        {
            get
            {
                if (_tipoDesconto == 0) _tipoDesconto = 2;
                return _tipoDesconto;
            }
            set { _tipoDesconto = value; }
        }

        [PersistenceProperty("ACRESCIMO")]
        public decimal Acrescimo { get; set; }

        private int _tipoAcrescimo;

        [PersistenceProperty("TIPOACRESCIMO")]
        public int TipoAcrescimo
        {
            get
            {
                if (_tipoAcrescimo == 0) _tipoAcrescimo = 2;
                return _tipoAcrescimo;
            }
            set { _tipoAcrescimo = value; }
        }

        [PersistenceProperty("IDPROCESSO")]
        public uint? IdProcesso { get; set; }

        [PersistenceProperty("IDAPLICACAO")]
        public uint? IdAplicacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CodOtimizacao", DirectionParameter.InputOptional)]
        public string CodOtimizacao { get; set; }

        [PersistenceProperty("EditDeleteVisible", DirectionParameter.InputOptional)]
        public bool EditDeleteVisible { get; set; }

        [PersistenceProperty("CODINTERNO", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        private decimal _totalProdutos;
        
        [PersistenceProperty("TOTALPRODUTOS", DirectionParameter.InputOptional)]
        public decimal TotalProdutos
        {
            get { return _totalProdutos - (!PedidoConfig.RatearDescontoProdutos ? ValorDescontoAtual : 0); }
            set { _totalProdutos = value; }
        }

        [PersistenceProperty("VALORACRESCIMO", DirectionParameter.InputOptional)]
        public decimal ValorAcrescimo { get; set; }

        [PersistenceProperty("VALORDESCONTO", DirectionParameter.InputOptional)]
        public decimal ValorDesconto { get; set; }

        [PersistenceProperty("OBSPROJ", DirectionParameter.InputOptional)]
        public string ObsProj { get; set; }

        [PersistenceProperty("CODPROCESSO", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [PersistenceProperty("CODAPLICACAO", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        #endregion

        #region Propriedades de Suporte

        #region Métodos internos estáticos

        internal static decimal GetValorDesconto(AmbientePedidoEspelho a)
        {
            return a.TipoDesconto == 1 ? a._totalProdutos * (a.Desconto / 100) : a.Desconto;
        }

        internal static decimal GetValorAcrescimo(AmbientePedidoEspelho a)
        {
            return a.TipoAcrescimo == 1 ? a._totalProdutos * (a.Acrescimo / 100) : a.Acrescimo;
        }

        internal static decimal GetTotalSemDesconto(AmbientePedidoEspelho a)
        {
            return (decimal)a._totalProdutos + (!PedidoConfig.RatearDescontoProdutos ? GetValorDesconto(a) : 0);
        }

        internal static decimal GetTotalSemAcrescimo(AmbientePedidoEspelho a)
        {
            return (decimal)a._totalProdutos - GetValorAcrescimo(a);
        }

        #endregion

        public string PecaVidro
        {
            get { return Ambiente + (Redondo && !BenefConfigDAO.Instance.CobrarRedondo() ? " REDONDO" : "") + (Altura != null && Largura != null ? " " + Altura.Value + "x" + (Redondo ? 0 : Largura.Value) : ""); }
        }

        public string PecaVidroQtd
        {
            get 
            {
                return Qtde + " " + PecaVidro + "   Tot. m²: " + 
                    Glass.Global.CalculosFluxo.ArredondaM2(Largura.Value, Altura.Value, Qtde.Value, IdProd > 0 ? (int)IdProd.Value : 0, Redondo) + 
                    (PedidoConfig.RelatorioPedido.ExibirTotalML ? "   Tot. ML: " + Math.Round((float)(2 * (Altura.Value + Largura.Value) * Qtde.Value) / 1000, 2) : "");
            }
        }

        public string TextoDesconto
        {
            get { return TipoDesconto == 1 ? Desconto + "%" : Desconto.ToString("C"); }
        }

        public string TextoAcrescimo
        {
            get { return TipoAcrescimo == 1 ? Acrescimo + "%" : Acrescimo.ToString("C"); }
        }

        /// <summary>
        /// Retorna o total do ambiente sem acréscimo/desconto.
        /// </summary>
        public decimal TotalBruto
        {
            get { return AmbientePedidoEspelhoDAO.Instance.GetTotalBruto(IdAmbientePedido); }
        }

        public decimal ValorDescontoAtual
        {
            get { return GetValorDesconto(this); }
        }

        public decimal ValorAcrescimoAtual
        {
            get { return GetValorAcrescimo(this); }
        }

        public bool ProjetoVisible
        {
            get { return IdItemProjeto > 0; }
        }

        public string DescrObsProj
        {
            get { return !String.IsNullOrEmpty(ObsProj) ? "(Obs. Projeto: " + ObsProj + ")" : String.Empty; }
        }

        public float TotM
        {
            get
            {
                return Largura > 0 && Altura > 0 && Qtde > 0 && IdProd > 0 ?
                    Glass.Global.CalculosFluxo.ArredondaM2(Largura.Value, Altura.Value, Qtde.Value, (int)IdProd.Value, Redondo) : 0;
            }
        }

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