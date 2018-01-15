using System;
using GDA;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ImpostoServDAO))]
    [PersistenceClass("imposto_serv")]
    public class ImpostoServ
    {
        #region Enumeradores

        public enum SituacaoEnum
        {
            Aberto = 1,
            Cancelado,
            Finalizado
        }

        public enum TipoPagtoEnum
        {
            AVista = 1,
            APrazo
        }

        #endregion

        #region Propriedades

        [Log(TipoLog.Cancelamento, "Imposto/Serviço")]
        [PersistenceProperty("IDIMPOSTOSERV", PersistenceParameterType.IdentityKey)]
        public uint IdImpostoServ { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("IDFORNEC")]
        public uint IdFornec { get; set; }

        [PersistenceProperty("IDFORMAPAGTO")]
        public uint IdFormaPagto { get; set; }

        [PersistenceProperty("IDCONTA")]
        public uint IdConta { get; set; }

        [PersistenceProperty("TIPOPAGTO")]
        public int TipoPagto { get; set; }

        private int _situacao = (int)SituacaoEnum.Aberto;

        [PersistenceProperty("SITUACAO")]
        public int Situacao
        {
            get { return _situacao; }
            set { _situacao = value; }
        }

        [PersistenceProperty("NUMPARC")]
        public int NumParc { get; set; }

        [PersistenceProperty("VALORPARC")]
        public decimal ValorParc { get; set; }

        [PersistenceProperty("DATABASEVENC")]
        public DateTime? DataBaseVenc { get; set; }

        [PersistenceProperty("DATAFINALIZADA")]
        public DateTime? DataFinalizada { get; set; }

        [PersistenceProperty("TOTAL")]
        public decimal Total { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("CONTABIL")]
        public bool Contabil { get; set; }

        [PersistenceProperty("NF")]
        public string Nf { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("USUCAD")]
        public uint UsuCad { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec { get; set; }

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("DESCRPLANOCONTA", DirectionParameter.InputOptional)]
        public string DescrPlanoConta { get; set; }

        [PersistenceProperty("DESCRUSUCAD", DirectionParameter.InputOptional)]
        public string DescrUsuCad { get; set; }

        [PersistenceProperty("TIPOPAGTOFORNEC", DirectionParameter.InputOptional)]
        public uint? TipoPagtoFornec { get; set; }

        [PersistenceProperty("TEMCONTAPAGA", DirectionParameter.InputOptional)]
        public bool TemContaPaga { get; set; }

        private string _enderecoFornec;

        [PersistenceProperty("ENDERECOFORNEC", DirectionParameter.InputOptional)]
        public string EnderecoFornec
        {
            get { return _enderecoFornec != null ? _enderecoFornec : String.Empty; }
            set { _enderecoFornec = value; }
        }

        private string _numeroFornec;

        [PersistenceProperty("NUMEROFORNEC", DirectionParameter.InputOptional)]
        public string NumeroFornec
        {
            get { return _numeroFornec != null ? _numeroFornec : String.Empty; }
            set { _numeroFornec = value; }
        }

        private string _complFornec;

        [PersistenceProperty("COMPLFORNEC", DirectionParameter.InputOptional)]
        public string ComplFornec
        {
            get { return _complFornec != null ? _complFornec : String.Empty; }
            set { _complFornec = value; }
        }

        private string _bairroFornec;

        [PersistenceProperty("BAIRROFORNEC", DirectionParameter.InputOptional)]
        public string BairroFornec
        {
            get { return _bairroFornec != null ? _bairroFornec : String.Empty; }
            set { _bairroFornec = value; }
        }

        private string _nomeCidadeFornec;

        [PersistenceProperty("NOMECIDADEFORNEC", DirectionParameter.InputOptional)]
        public string NomeCidadeFornec
        {
            get { return _nomeCidadeFornec != null ? _nomeCidadeFornec : String.Empty; }
            set { _nomeCidadeFornec = value; }
        }

        private string _nomeUfFornec;

        [PersistenceProperty("NOMEUFFORNEC", DirectionParameter.InputOptional)]
        public string NomeUfFornec
        {
            get { return _nomeUfFornec != null ? _nomeUfFornec : String.Empty; }
            set { _nomeUfFornec = value; }
        }

        private string _cepFornec;

        [PersistenceProperty("CEPFORNEC", DirectionParameter.InputOptional)]
        public string CepFornec
        {
            get { return _cepFornec != null ? _cepFornec : String.Empty; }
            set { _cepFornec = value; }
        }

        [PersistenceProperty("EMAILFORNEC", DirectionParameter.InputOptional)]
        public string EmailFornec { get; set; }

        [PersistenceProperty("TELCONTFORNEC", DirectionParameter.InputOptional)]
        public string TelcontFornec { get; set; }

        [PersistenceProperty("FAXFORNEC", DirectionParameter.InputOptional)]
        public string FaxFornec { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrPagto { get; set; }

        public string DescrSituacao
        {
            get { return ((SituacaoEnum)_situacao).ToString(); }
        }

        public string DescrTipoPagto
        {
            get
            {
                switch (TipoPagto)
                {
                    case (int)TipoPagtoEnum.AVista: return "À vista";
                    case (int)TipoPagtoEnum.APrazo: return "À prazo";
                    default: return "";
                }
            }
        }

        private DateTime[] _datasParcelas = new DateTime[FinanceiroConfig.Compra.NumeroParcelasCompra];

        public DateTime[] DatasParcelas
        {
            get { return _datasParcelas; }
            set { _datasParcelas = value; }
        }

        private decimal[] _valoresParcelas = new decimal[FinanceiroConfig.Compra.NumeroParcelasCompra];

        public decimal[] ValoresParcelas
        {
            get { return _valoresParcelas; }
            set { _valoresParcelas = value; }
        }

        public bool EditarVisible
        {
            get { return _situacao == (int)SituacaoEnum.Aberto; }
        }

        public bool CancelarVisible
        {
            get { return _situacao != (int)SituacaoEnum.Cancelado; }
        }

        public bool ReabrirVisible
        {
            get { return _situacao == (int)SituacaoEnum.Finalizado && !TemContaPaga; }
        }

        public string EnderecoCompletoFornec
        {
            get { return EnderecoFornec + " " + NumeroFornec + " " + BairroFornec + " - " + NomeCidadeFornec + "/" + NomeUfFornec + " - CEP " + CepFornec; }
        }

        public string FoneFaxFornec
        {
            get { return TelcontFornec + " / " + FaxFornec; }
        }

        /// <summary>
        /// Verifica se deve exibir o centro de custo
        /// </summary>
        public bool ExibirCentroCusto
        {
            get
            {
                return FiscalConfig.UsarControleCentroCusto && CentroCustoDAO.Instance.GetCountReal() > 0 && _situacao == (int)SituacaoEnum.Finalizado;
            }
        }

        /// <summary>
        /// Verifica se o valor do centro de custo foi totalmente informado.
        /// </summary>
        public bool CentroCustoCompleto
        {
            get
            {
                return Total == CentroCustoAssociadoDAO.Instance.ObtemTotalPorImpostoServ((int)IdImpostoServ);
            }
        }

        #endregion
    }
}