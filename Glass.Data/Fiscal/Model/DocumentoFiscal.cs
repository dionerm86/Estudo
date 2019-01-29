using System;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DocumentoFiscalDAO))]
    [PersistenceClass("documento_fiscal")]
    public class DocumentoFiscal : Sync.Fiscal.EFD.Entidade.IDocumentoFiscal
    {
        #region Propriedades

        [PersistenceProperty("IDDOCFISCAL", PersistenceParameterType.IdentityKey)]
        public uint IdDocFiscal { get; set; }

        [PersistenceProperty("IDNF")]
        public uint IdNf { get; set; }

        [PersistenceProperty("IDLOJA")]
        public int? IdLoja { get; set; }

        [PersistenceProperty("IDTRANSPORTADOR")]
        public int? IdTransportador { get; set; }

        [PersistenceProperty("IDFORNEC")]
        public int? IdFornec { get; set; }

        [PersistenceProperty("ID_CLI")]
        public int? IdCliente { get; set; }

        [PersistenceProperty("IDADMINCARTAO")]
        public int? IdAdminCartao { get; set; }

        [PersistenceProperty("TIPO")]
        public int Tipo { get; set; }

        [PersistenceProperty("EMITENTE")]
        public int Emitente { get; set; }

        [PersistenceProperty("MODELO")]
        public string Modelo { get; set; }

        [PersistenceProperty("SERIE")]
        public string Serie { get; set; }

        [PersistenceProperty("SUBSERIE")]
        public string SubSerie { get; set; }

        [PersistenceProperty("NUMERO")]
        public string Numero { get; set; }

        [PersistenceProperty("DATAEMISSAO")]
        public DateTime DataEmissao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec { get; set; }

        [PersistenceProperty("NOMETRANSP", DirectionParameter.InputOptional)]
        public string NomeTransportador { get; set; }

        [PersistenceProperty("NOMEADMINCARTAO", DirectionParameter.InputOptional)]
        public string NomeAdminCartao { get; set; }

        [PersistenceProperty("CHAVEACESSO", DirectionParameter.InputOptional)]
        public string ChaveAcesso { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrTipo
        {
            get { return DataSourcesEFD.Instance.GetDescrTipoDocumentoFiscal(Tipo); }
        }

        public string DescrEmitente
        {
            get { return DataSourcesEFD.Instance.GetDescrEmitenteDocumentoFiscal(Emitente); }
        }

        private DataSourcesEFD.TipoPartEnum? _tipoPart;

        public int TipoPart
        {
            get
            {
                if (_tipoPart == null)
                {
                    _tipoPart = IdLoja > 0 ? DataSourcesEFD.TipoPartEnum.Loja :
                        IdFornec > 0 ? DataSourcesEFD.TipoPartEnum.Fornecedor :
                        IdTransportador > 0 ? DataSourcesEFD.TipoPartEnum.Transportador :
                        IdAdminCartao > 0 ? DataSourcesEFD.TipoPartEnum.AdministradoraCartao :
                        DataSourcesEFD.TipoPartEnum.Cliente;
                }

                return (int)_tipoPart.Value;
            }
            set { _tipoPart = (DataSourcesEFD.TipoPartEnum)value; }
        }

        public int? IdPart
        {
            get
            {
                switch ((DataSourcesEFD.TipoPartEnum)TipoPart)
                {
                    case DataSourcesEFD.TipoPartEnum.Fornecedor: return IdFornec;
                    case DataSourcesEFD.TipoPartEnum.Loja: return IdLoja;
                    case DataSourcesEFD.TipoPartEnum.Transportador: return IdTransportador;
                    case DataSourcesEFD.TipoPartEnum.AdministradoraCartao: return IdAdminCartao;
                    default: return IdCliente;
                }
            }
            set
            {
                switch ((DataSourcesEFD.TipoPartEnum)TipoPart)
                {
                    case DataSourcesEFD.TipoPartEnum.Fornecedor:
                        IdFornec = value;
                        break;
                    case DataSourcesEFD.TipoPartEnum.Loja:
                        IdLoja = value;
                        break;
                    case DataSourcesEFD.TipoPartEnum.Transportador:
                        IdTransportador = value;
                        break;
                    case DataSourcesEFD.TipoPartEnum.AdministradoraCartao:
                        IdAdminCartao = value;
                        break;
                    default:
                        IdCliente = value;
                        break;
                }
            }
        }

        public string DescrTipoPart
        {
            get { return DataSourcesEFD.Instance.GetDescrTipoParticipante(TipoPart); }
        }

        public string DescrPart
        {
            get
            {
                switch ((DataSourcesEFD.TipoPartEnum)TipoPart)
                {
                    case DataSourcesEFD.TipoPartEnum.Fornecedor: return NomeFornec;
                    case DataSourcesEFD.TipoPartEnum.Loja: return NomeLoja;
                    case DataSourcesEFD.TipoPartEnum.Transportador: return NomeTransportador;
                    case DataSourcesEFD.TipoPartEnum.AdministradoraCartao: return NomeAdminCartao;
                    default: return NomeCliente;
                }
            }
        }

        #endregion

        #region IDocumentoFiscal Members

        Sync.Fiscal.Enumeracao.DocumentoFiscal.Tipo Sync.Fiscal.EFD.Entidade.IDocumentoFiscal.Tipo
        {
            get { return (Sync.Fiscal.Enumeracao.DocumentoFiscal.Tipo)Tipo; }
        }

        Sync.Fiscal.Enumeracao.DocumentoFiscal.Emitente Sync.Fiscal.EFD.Entidade.IDocumentoFiscal.Emitente
        {
            get { return (Sync.Fiscal.Enumeracao.DocumentoFiscal.Emitente)Emitente; }
        }

        #endregion

        #region IParticipante Members

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoLoja
        {
            get { return IdLoja; }
            set { IdLoja = value; }
        }

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoCliente
        {
            get { return IdCliente; }
            set { IdCliente = value; }
        }

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoFornecedor
        {
            get { return IdFornec; }
            set { IdFornec = value; }
        }

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoTransportador
        {
            get { return IdTransportador; }
            set { IdTransportador = value; }
        }

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoAdministradoraCartao
        {
            get { return (int?)IdAdminCartao; }
            set { IdAdminCartao = value; }
        }

        #endregion
    }
}
