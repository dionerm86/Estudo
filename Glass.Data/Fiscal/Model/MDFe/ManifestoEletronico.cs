using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Glass.Data.Model
{
    #region Enumeradores

    public enum SituacaoEnum
    {
        [Description("Aberto")]
        Aberto = 1,

        [Description("Autorizado")]
        Autorizado,

        [Description("Cancelado")]
        Cancelado,

        [Description("Encerrado")]
        Encerrado,

        [Description("Contingência Offline")]
        ContingenciaOffline,

        [Description("Processo de emissão")]
        ProcessoEmissao,

        [Description("Processo de cancelamento")]
        ProcessoCancelamento,

        [Description("Processo de encerramento")]
        ProcessoEncerramento,

        [Description("Falha ao emitir")]
        FalhaEmitir,

        [Description("Falha ao cancelar")]
        FalhaCancelar,

        [Description("Falha ao encerrar")]
        FalhaEncerrar,

        [Description("Falha")]
        Falha
    }

    public enum TipoEmitenteEnum
    {
        /// <summary>
        /// Prestador de serviço de transporte
        /// </summary>
        [Description("Prestador de serviço de transporte")]
        PrestadorServicoTrasporte = 1,

        /// <summary>
        /// Transportador de carga própria
        /// </summary>
        [Description("Transportador de carga própria")]
        TransportadorCargaPropria,

        /// <summary>
        /// Prestador de serviço de transporte que emitirá CT-e globalizado.
        /// </summary>
        [Description("Prestador de serviço de transporte CT-e globalizado")]
        PrestadorServicoTrasporteCTeGlobalizado
    }

    public enum TipoTransportadorEnum
    {
        /// <summary>
        /// MDFe não se encaixa em nenhum Tipo
        /// </summary>
        [Description("Nenhum")]
        Nenhum,

        /// <summary>
        /// Empresas de Transporte de Cargas
        /// </summary>
        [Description("ETC - Empresas de Transporte de Cargas")]
        ETC,

        /// <summary>
        /// Transportador Autônomo de Carga
        /// </summary>
        [Description("TAC - Transportador Autônomo de Carga")]
        TAC,

        /// <summary>
        /// Cooperativa de Transporte de Cargas
        /// </summary>
        [Description("CTC - Cooperativa de Transporte de Cargas")]
        CTC
    }

    public enum ModalEnum
    {
        [Description("Rodoviário")]
        Rodoviario = 1,

        [Description("Aéreo")]
        Aereo,

        [Description("Aquaviário")]
        Aquaviario,
            
        [Description("Ferroviário")]
        Ferroviario
    }

    public enum TipoEmissao
    {
        [Description("Normal")]
        Normal = 1,

        [Description("Contingência")]
        Contingencia
    }

    public enum ResponsavelEnum
    {
        /// <summary>
        /// Emitente do MDF-e
        /// </summary>
        [Description("Emitente")]
        Emitente = 1,

        /// <summary>
        /// Responsável pela contratação do serviço de transporte
        /// </summary>
        [Description("Contratante")]
        Contratante
    }

    public enum CodigoUnidadeEnum
    {
        [Description("KG")]
        KG = 1,

        [Description("TON")]
        TON
    }

    #endregion

    [PersistenceBaseDAO(typeof(ManifestoEletronicoDAO))]
    [PersistenceClass("manifesto_eletronico")]
    public class ManifestoEletronico : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDMANIFESTOELETRONICO", PersistenceParameterType.IdentityKey)]
        public int IdManifestoEletronico { get; set; }

        [PersistenceProperty("SITUACAO")]
        public SituacaoEnum Situacao { get; set; }

        [PersistenceProperty("CHAVEACESSO")]
        public string ChaveAcesso { get; set; }

        [PersistenceProperty("NUMLOTE")]
        public int NumLote { get; set; }

        #region IDENTIFICAÇÃO MDF-e

        [PersistenceProperty("TIPOEMITENTE")]
        public TipoEmitenteEnum TipoEmitente { get; set; }

        [PersistenceProperty("TIPOTRANSPORTADOR")]
        public TipoTransportadorEnum TipoTransportador { get; set; }

        [PersistenceProperty("MODELO")]
        public int Modelo { get; set; }

        [PersistenceProperty("SERIE")]
        public int Serie { get; set; }

        [PersistenceProperty("NUMEROMANIFESTOELETRONICO")]
        public int NumeroManifestoEletronico { get; set; }

        [PersistenceProperty("CODIGOALEATORIO")]
        public int CodigoAleatorio { get; set; }

        [PersistenceProperty("MODAL")]
        public ModalEnum Modal { get; set; }

        [PersistenceProperty("DATAEMISSAO")]
        public DateTime DataEmissao { get; set; }

        [PersistenceProperty("TIPOEMISSAO")]
        public TipoEmissao TipoEmissao { get; set; }

        [PersistenceProperty("UFINICIO")]
        public string UFInicio { get; set; }

        [PersistenceProperty("UFFIM")]
        public string UFFim { get; set; }

        [PersistenceProperty("DATAINICIOVIAGEM")]
        public DateTime DataInicioViagem { get; set; }

        #endregion

        #region SEGURO CARGA

        [PersistenceProperty("RESPONSAVELSEGURO")]
        public ResponsavelEnum ResponsavelSeguro { get; set; }

        [PersistenceProperty("IDSEGURADORA")]
        [PersistenceForeignKey(typeof(Cte.Seguradora), "IdSeguradora")]
        public int IdSeguradora { get; set; }

        [PersistenceProperty("NUMEROAPOLICE")]
        public string NumeroApolice { get; set; }

        #endregion

        #region TOTALIZADORES DA CARGA

        [PersistenceProperty("VALORCARGA")]
        public decimal ValorCarga { get; set; }

        [PersistenceProperty("CODIGOUNIDADE")]
        public CodigoUnidadeEnum CodigoUnidade { get; set; }

        [PersistenceProperty("QUANTIDADECARGA")]
        public decimal QuantidadeCarga { get; set; }

        #endregion

        #region INFORMAÇÕES ADICIONAIS

        [PersistenceProperty("INFORMACOESADICIONAISFISCO")]
        public string InformacoesAdicionaisFisco { get; set; }

        [PersistenceProperty("INFORMACOESCOMPLEMENTARES")]
        public string InformacoesComplementares { get; set; }

        #endregion

        [PersistenceProperty("MOTIVOCANCELAMENTO")]
        public string MotivoCancelamento { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string Emitente
        {
            get
            {
                if (Participantes.Any(f => f.TipoParticipante == TipoParticipanteEnum.Emitente))
                    return Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault().Emitente;

                return string.Empty;
            }
        }

        public string Contratante
        {
            get
            {
                if (Participantes.Any(f => f.TipoParticipante == TipoParticipanteEnum.Contratante))
                    return Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Contratante).FirstOrDefault().Contratante;

                return string.Empty;
            }
        }

        public bool EditarVisible
        {
            get
            {
                return (Situacao == SituacaoEnum.Aberto || Situacao == SituacaoEnum.FalhaEmitir);
            }
        }

        public bool ImprimirDAMDFEVisible
        {
            get { return (Situacao == SituacaoEnum.Autorizado || Situacao == SituacaoEnum.ContingenciaOffline ||
                    Situacao == SituacaoEnum.Encerrado); }
        }

        public bool EmitirMDFeOfflineVisible
        {
            get
            {
                return (Situacao == SituacaoEnum.ContingenciaOffline);
            }
        }

        public bool CancelarVisible
        {
            get
            {
                return (Situacao == SituacaoEnum.Autorizado || Situacao == SituacaoEnum.FalhaCancelar);
            }
        }

        public bool EncerrarVisible
        {
            get
            {
                return (Situacao == SituacaoEnum.Autorizado || Situacao == SituacaoEnum.FalhaEncerrar);
            }
        }

        public bool ConsultarSituacaoVisible
        {
            get
            {
                return (Situacao == SituacaoEnum.ProcessoEmissao || Situacao == SituacaoEnum.ProcessoCancelamento || Situacao == SituacaoEnum.ProcessoEncerramento ||
                    Situacao == SituacaoEnum.FalhaEmitir || Situacao == SituacaoEnum.FalhaCancelar || Situacao == SituacaoEnum.FalhaEncerrar);
            }
        }

        public string SituacaoString
        {
            get
            {
                return Situacao == SituacaoEnum.Aberto ? "Aberto" :
                    Situacao == SituacaoEnum.Autorizado ? "Autorizado" :
                    Situacao == SituacaoEnum.Cancelado ? "Cancelado" :
                    Situacao == SituacaoEnum.Encerrado ? "Encerrado" :
                    Situacao == SituacaoEnum.ContingenciaOffline ? "Contingência Offline" :
                    Situacao == SituacaoEnum.ProcessoEmissao ? "Processo de emissão" :
                    Situacao == SituacaoEnum.ProcessoCancelamento ? "Processo de cancelamento" :
                    Situacao == SituacaoEnum.ProcessoEncerramento ? "Processo de encerramento" :
                    Situacao == SituacaoEnum.FalhaEmitir ? "Falha ao emitir" :
                    Situacao == SituacaoEnum.FalhaCancelar ? "Falha ao cancelar" :
                    Situacao == SituacaoEnum.FalhaEncerrar ? "Falha ao encerrar" :
                    Situacao == SituacaoEnum.Falha ? "Falha" :
                    string.Empty;
            }
        }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMESEGURADORA", DirectionParameter.InputOptional)]
        public string NomeSeguradora { get; set; }

        #endregion

        private List<CidadeCargaMDFe> _cidadesCarga = null;
        public List<CidadeCargaMDFe> CidadesCarga
        {
            get
            {
                if (_cidadesCarga == null)
                {
                    _cidadesCarga = CidadeCargaMDFeDAO.Instance.ObterCidadeCargaMDFe(IdManifestoEletronico);
                }
                return _cidadesCarga;
            }
            set
            {
                _cidadesCarga = value;
            }
        }

        private List<UFPercursoMDFe> _uFsPercurso = null;
        public List<UFPercursoMDFe> UFsPercurso
        {
            get
            {
                if (_uFsPercurso == null)
                {
                    _uFsPercurso = UFPercursoMDFeDAO.Instance.ObterUFPercursoMDFe(IdManifestoEletronico);
                }
                return _uFsPercurso;
            }
            set
            {
                _uFsPercurso = value;
            }
        }

        private List<ParticipanteMDFe> _participantes = null;
        public List<ParticipanteMDFe> Participantes
        {
            get
            {
                if (_participantes == null)
                {
                    _participantes = ParticipanteMDFeDAO.Instance.ObterParticipanteMDFe(IdManifestoEletronico);
                }
                return _participantes;
            }
            set
            {
                _participantes = value;
            }
        }

        private RodoviarioMDFe _rodoviario = null;
        public RodoviarioMDFe Rodoviario
        {
            get
            {
                if (_rodoviario == null)
                {
                    _rodoviario = RodoviarioMDFeDAO.Instance.ObterRodoviarioPeloManifesto(IdManifestoEletronico);
                }
                return _rodoviario;
            }
            set
            {
                _rodoviario = value;
            }
        }

        private List<AverbacaoSeguroMDFe> _averbacaoSeguro = null;
        public List<AverbacaoSeguroMDFe> AverbacaoSeguro
        {
            get
            {
                if (_averbacaoSeguro == null)
                {
                    _averbacaoSeguro = AverbacaoSeguroMDFeDAO.Instance.ObterAverbacaoSeguroMDFe(null, IdManifestoEletronico);
                }
                return _averbacaoSeguro;
            }
            set
            {
                _averbacaoSeguro = value;
            }
        }

        private List<CidadeDescargaMDFe> _cidadesDescarga = null;
        public List<CidadeDescargaMDFe> CidadesDescarga
        {
            get
            {
                if (_cidadesDescarga == null)
                {
                    _cidadesDescarga = CidadeDescargaMDFeDAO.Instance.ObterCidadeDescargaPeloManifestoEletronico(null, IdManifestoEletronico);
                }
                return _cidadesDescarga;
            }
            set
            {
                _cidadesDescarga = value;
            }
        }
    }
}
