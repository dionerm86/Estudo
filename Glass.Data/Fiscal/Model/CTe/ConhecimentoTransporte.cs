using System;
using System.Collections.Generic;
using System.Linq;
using GDA;
using Glass.Data.DAL;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(ConhecimentoTransporteDAO))]
    [PersistenceClass("conhecimento_transporte")]
    public class ConhecimentoTransporte : ModelBaseCadastro, Sync.Fiscal.EFD.Entidade.ICTe
    {
        #region Enumeradores

        public enum FormaPagtoEnum
        {
            Pago,
            APagar,
            Outros
        }

        public enum TipoEmissaoEnum
        {
            Normal = 1,
            ContingenciaFsda = 5,
            AutorizacaoSvcRs = 7,
            AutorizacaoSvcSp = 8
        }

        public enum TipoCteEnum
        {
            Normal,
            ComplementoValores,
            AnulacaoValores,
            Substituto
        }

        public enum TipoServicoEnum
        {
            Normal,
            Subcontratacao,
            Redespacho,
            RedespachoIntermediario
        }        

        public enum SituacaoEnum
        {
            Aberto = 1,
            Autorizado,
            NaoEmitido,
            Cancelado,
            Inutilizado,            //5
            Denegado,
            ProcessoEmissao,
            ProcessoCancelamento,
            ProcessoInutilizacao,
            FalhaEmitir,            //10
            FalhaCancelar,
            FalhaInutilizar,
            FinalizadoTerceiros
        }

        public enum TipoDocumentoCteEnum
        {
            Saida = 2,
            EntradaTerceiros
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.IdentityKey)]
        public uint IdCte { get; set; }

        //[Log("Natureza da Operação", "CodCompleto", typeof(NaturezaOperacaoDAO), "IdNaturezaOperacao", "ObtemElemento", true)]
        [PersistenceProperty("IDNATUREZAOPERACAO")]
        public uint IdNaturezaOperacao { get; set; }
        
        [PersistenceProperty("IDCIDADECTE")]
        public uint IdCidadeCte { get; set; }

        [PersistenceProperty("IDCIDADEINICIO")]
        public uint IdCidadeInicio { get; set; }
       
        [PersistenceProperty("IDCIDADEFIM")]
        public uint IdCidadeFim { get; set; }

        //[PersistenceProperty("IDCIDADEORIGFRETE")]
        //public uint IdCidadeOrigFrete { get; set; }

        //[PersistenceProperty("IDCIDADEDESTFRETE")]
        //public uint IdCidadeDestFrete { get; set; }

        [PersistenceProperty("IDCTEANTERIOR")]
        public uint? IdCteAnterior { get; set; }

        [PersistenceProperty("NUMEROCTE")]
        public int NumeroCte { get; set; }

        [PersistenceProperty("CODALEATORIO")]
        public string CodAleatorio { get; set; }

        [PersistenceProperty("MODELO")]
        public string Modelo { get; set; }

        [PersistenceProperty("SERIE")]
        public string Serie { get; set; }

        [PersistenceProperty("DATAEMISSAO")]
        public DateTime DataEmissao { get; set; }

        [PersistenceProperty("DATAENTRADASAIDA")]
        public DateTime? DataEntradaSaida { get; set; }

        [PersistenceProperty("TIPOEMISSAO")]
        public int TipoEmissao { get; set; }

        [PersistenceProperty("TIPOCTE")]
        public int TipoCte { get; set; }

        [PersistenceProperty("CHAVEACESSO")]
        public string ChaveAcesso { get; set; }

        [PersistenceProperty("TIPOSERVICO")]
        public int TipoServico { get; set; }

        [PersistenceProperty("RETIRADA")]
        public bool Retirada { get; set; }

        [PersistenceProperty("DETALHESRETIRADA")]
        public string DetalhesRetirada { get; set; }

        [PersistenceProperty("VALORTOTAL")]
        public decimal ValorTotal { get; set; }

        [PersistenceProperty("VALORRECEBER")]
        public decimal ValorReceber { get; set; }

        [PersistenceProperty("INFORMADICIONAIS")]
        public string InformAdicionais { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [PersistenceProperty("MOTIVOCANC")]
        public string MotivoCanc { get; set; }

        [PersistenceProperty("MOTIVOINUT")]
        public string MotivoInut { get; set; }

        [PersistenceProperty("NUMLOTE")]
        public int NumeroLote { get; set; }

        [PersistenceProperty("TIPODOCUMENTOCTE")]
        public int TipoDocumentoCte { get; set; }

        [PersistenceProperty("GerarContasReceber")]
        public bool GerarContasReceber { get; set; }

        #endregion

        #region Propiedades Estendidas

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de suporte

        private uint? _idCfop;
        private List<ParticipanteCte> _participantes;

        public uint IdCfop
        {
            get
            {
                if (_idCfop == null)
                    _idCfop = NaturezaOperacaoDAO.Instance.ObtemIdCfop(IdNaturezaOperacao);

                return _idCfop.GetValueOrDefault();
            }
        }

        public bool ConsSitVisible
        {
            get
            {
                return Situacao == (int)SituacaoEnum.ProcessoEmissao || Situacao == (int)SituacaoEnum.ProcessoInutilizacao ||
                    Situacao == (int)SituacaoEnum.ProcessoCancelamento || Situacao == (int)SituacaoEnum.FalhaEmitir ||
                    Situacao == (int)SituacaoEnum.FalhaCancelar || Situacao == (int)SituacaoEnum.FalhaInutilizar;
            }
        }

        public bool PrintDacteVisible
        {
            get
            {
                var numProtocolo = Glass.Data.DAL.CTe.ProtocoloCteDAO.Instance.GetElement(IdCte, (int)ProtocoloCte.TipoProtocoloEnum.Autorizacao).NumProtocolo;
                return Situacao == (int)SituacaoEnum.Autorizado || ((Situacao == (int)SituacaoEnum.Cancelado ||
                    Situacao == (int)SituacaoEnum.FalhaCancelar) && !String.IsNullOrEmpty(numProtocolo)) ;
            }
        }

        public bool BaixarXmlVisible
        {
            get
            {
                return PrintDacteVisible;
            }
        }

        public Sync.Fiscal.Enumeracao.NFe.NotaCreditoDebito CteCreditoDebito
        {
            get { return TipoDocumentoCte == (int)TipoDocumentoCteEnum.Saida ? Sync.Fiscal.Enumeracao.NFe.NotaCreditoDebito.Debito : Sync.Fiscal.Enumeracao.NFe.NotaCreditoDebito.Credito; }
        }

        public string CodigoNaturezaOperacao
        {
            get { return NaturezaOperacaoDAO.Instance.ObtemCodigoCompleto(IdNaturezaOperacao); }
        }

        public string TipoCteString
        {
            get
            {
                return TipoCte == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoCteEnum.AnulacaoValores ? "Anulacao Valores" :
                       TipoCte == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoCteEnum.ComplementoValores ? "Complemento Valores" :
                       TipoCte == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoCteEnum.Normal ? "Normal" :
                       TipoCte == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoCteEnum.Substituto ? "Substituto" :
                    String.Empty;
            }
        }

        public string TipoEmissaoString
        {
            get
            {
                return TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.AutorizacaoSvcRs ? "Autorizacao SvcRs" :
                       TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.AutorizacaoSvcSp ? "Autorizacao SvcSp" :
                       TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.ContingenciaFsda ? "Contingencia Fsda" :
                       TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.Normal ? "Normal" :
                    String.Empty;
            }
        }

        public string TipoServicoString
        {
            get
            {
                return TipoServico == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoServicoEnum.Normal ? "Normal" :
                       TipoServico == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoServicoEnum.Redespacho ? "Redespacho" :
                       TipoServico == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoServicoEnum.RedespachoIntermediario ? "RedespachoIntermediario" :
                       TipoServico == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoServicoEnum.Subcontratacao ? "Subcontratacao" :
                    String.Empty;
            }
        }

        public string SituacaoString
        {
            get
            {
                return Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Aberto ? "Aberto" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado ? "Autorizado" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.NaoEmitido ? "Não emitido" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Cancelado ? "Cancelado" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Inutilizado ? "Inutilizado" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Denegado ? "Denegado" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.ProcessoEmissao ? "Processo de emissão" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.ProcessoCancelamento ? "Processo de cancelamento" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.ProcessoInutilizacao ? "Processo de inutilização" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaEmitir ? "Falha ao emitir" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaCancelar ? "Falha ao cancelar" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaInutilizar ? "Falha ao inutilizar" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FinalizadoTerceiros ? "Finalizado" :
                    String.Empty;
            }
        }

        public string TipoDocumentoCteString
        {
            get
            {
                switch (TipoDocumentoCte)
                {
                    case (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoDocumentoCteEnum.Saida: return "Saída";
                    case (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoDocumentoCteEnum.EntradaTerceiros: return "Entrada (terceiros)";
                    default: return String.Empty;
                }
            }
        }

        public List<ParticipanteCte> Participantes
        {
            get 
            {
                if(_participantes == null)
                _participantes = ParticipanteCteDAO.Instance.GetParticipanteByIdCte(IdCte);
                return _participantes;
            }
        }

        public string EmitenteCte
        {
            get
            {
                if (Participantes.Any(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Emitente))
                    return Participantes.Where(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Emitente).FirstOrDefault().Emitente;

                return string.Empty;
            }
        }

        public string RemetenteCte
        {
            get
            {
                if (Participantes.Any(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Remetente))
                    return Participantes.Where(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Remetente).FirstOrDefault().Remetente;

                return string.Empty;
            }
        }

        public string DestinatarioCte
        {
            get
            {
                if (Participantes.Any(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Destinatario))
                    return Participantes.Where(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Destinatario).FirstOrDefault().Destinatario;

                return string.Empty;
            }
        }

        public string expedidorCte
        {
            get
            {
                if (Participantes.Any(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Expedidor))
                    return Participantes.Where(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Expedidor).FirstOrDefault().Expedidor;

                return string.Empty;
            }
        }

        public string RecebedorCte
        {
            get
            {
                if (Participantes.Any(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Recebedor))
                    return Participantes.Where(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Recebedor).FirstOrDefault().Recebedor;

                return string.Empty;
            }
        }

        public CobrancaDuplCte CobrancaDuplCte
        {
            get
            {
                return CobrancaDuplCteDAO.Instance.GetElement(IdCte);
            }
        }

        #endregion

        #region ICTe Members

        int Sync.Fiscal.EFD.Entidade.ICTe.Codigo
        {
            get { return (int)IdCte; }
        }

        int Sync.Fiscal.EFD.Entidade.ICTe.CodigoCidade
        {
            get { return (int)IdCidadeInicio; }
        }

        int Sync.Fiscal.EFD.Entidade.ICTe.CodigoCidadeFim
        {
            get { return (int)IdCidadeFim; }
        }

        int? Sync.Fiscal.EFD.Entidade.ICTe.CodigoNaturezaOperacao
        {
            get { return (int)IdNaturezaOperacao; }
        }

        int? Sync.Fiscal.EFD.Entidade.ICTe.CodigoCfop
        {
            get { return null; }
        }

        int Sync.Fiscal.EFD.Entidade.ICTe.NumeroCTe
        {
            get { return NumeroCte; }
        }

        Sync.Fiscal.Enumeracao.CTe.Situacao Sync.Fiscal.EFD.Entidade.ICTe.Situacao
        {
            get { return (Sync.Fiscal.Enumeracao.CTe.Situacao)Situacao; }
        }

        Sync.Fiscal.Enumeracao.InfoAdicionalNFe.TipoCTe Sync.Fiscal.EFD.Entidade.ICTe.TipoCTe
        {
            get { return (Sync.Fiscal.Enumeracao.InfoAdicionalNFe.TipoCTe)TipoCte; }
        }

        string Sync.Fiscal.EFD.Entidade.ICTe.InformacoesAdicionais
        {
            get { return InformAdicionais; }
        }

        bool Sync.Fiscal.EFD.Entidade.ICTe.Saida
        {
            get { return TipoDocumentoCte == (int)TipoDocumentoCteEnum.Saida; }
        }

        bool Sync.Fiscal.EFD.Entidade.ICTe.EntradaTerceiros
        {
            get { return TipoDocumentoCte == (int)TipoDocumentoCteEnum.EntradaTerceiros; }
        }

        #endregion
    }
}
