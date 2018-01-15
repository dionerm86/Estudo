using System;
using GDA;
using Glass.Data.DAL.CTe;
using Glass.Data.DAL;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(ParticipanteCteDAO))]
    [PersistenceClass("participante_cte")]
    public class ParticipanteCte : Sync.Fiscal.EFD.Entidade.IParticipanteCTe
    {
        #region Enumeradores

        public enum TipoParticipanteEnum
        {
            Emitente = 0,
            Remetente = 1,
            Destinatario = 2,
            Expedidor = 3,
            Recebedor = 4,
            Tomador = 5
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("NUMSEQ", PersistenceParameterType.Key)]
        public int NumSeq { get; set; }

        [PersistenceProperty("TIPOPARTICIPANTE")]
        public TipoParticipanteEnum TipoParticipante { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint? IdLoja { get; set; }

        [PersistenceProperty("IDFORNEC")]
        public uint? IdFornec { get; set; }

        [PersistenceProperty("IDCLIENTE")]
        public uint? IdCliente { get; set; }

        [PersistenceProperty("IDTRANSPORTADOR")]
        public uint? IdTransportador { get; set; }

        [PersistenceProperty("TOMADOR")]
        public bool Tomador { get; set; }

        #endregion

        #region Propriedades Suporte

        public string Emitente
        {
            get 
            {
                if (TipoParticipante == TipoParticipanteEnum.Emitente)
                    return ObtemNomeParticipante();

                return string.Empty;
            }
        }

        public string Destinatario
        {
            get
            {
                if (TipoParticipante == TipoParticipanteEnum.Destinatario)
                    return ObtemNomeParticipante();

                return string.Empty;
            }
        }

        public string Expedidor
        {
            get
            {
                if (TipoParticipante == TipoParticipanteEnum.Expedidor)
                    return ObtemNomeParticipante();

                return string.Empty;
            }
        }

        public string Recebedor
        {
            get
            {
                if (TipoParticipante == TipoParticipanteEnum.Recebedor)
                    return ObtemNomeParticipante();

                return string.Empty;
            }
        }

        public string Remetente
        {
            get
            {
                if (TipoParticipante == TipoParticipanteEnum.Remetente)
                    return ObtemNomeParticipante();

                return string.Empty;
            }
        }

        public string ObtemNomeParticipante()
        {
            if (IdLoja > 0)
            {
                var loja = LojaDAO.Instance.GetElement(IdLoja.GetValueOrDefault());

                if (loja != null)
                    return loja.NomeFantasia;
            }
            else if (IdFornec > 0)
            {
                var fornec = FornecedorDAO.Instance.GetElement(IdFornec.GetValueOrDefault());
                
                if (fornec != null)
                    return fornec.Nomefantasia;
            }
            else if (IdCliente > 0)
            {
                var cliente = ClienteDAO.Instance.GetElement(IdCliente.GetValueOrDefault());
                
                if (cliente != null)
                    return cliente.Nome;
            }
            else if (IdTransportador > 0)
            {
                var transp = TransportadorDAO.Instance.GetElement(IdTransportador.GetValueOrDefault());
                    
                if (transp != null)
                    return transp.NomeFantasia;
            }

            return string.Empty;
        }

        #endregion

        #region IParticipanteCTe Members

        int Sync.Fiscal.EFD.Entidade.IParticipanteCTe.CodigoCTe
        {
            get { return (int)IdCte; }
        }

        bool Sync.Fiscal.EFD.Entidade.IParticipanteCTe.Emitente
        {
            get { return TipoParticipante == TipoParticipanteEnum.Emitente; }
        }

        bool Sync.Fiscal.EFD.Entidade.IParticipanteCTe.Destinatario
        {
            get { return TipoParticipante == TipoParticipanteEnum.Destinatario; }
        }

        bool Sync.Fiscal.EFD.Entidade.IParticipanteCTe.Remetente
        {
            get { return TipoParticipante == TipoParticipanteEnum.Remetente; }
        }

        #endregion

        #region IParticipante Members

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoLoja
        {
            get { return (int?)IdLoja; }
            set { IdLoja = (uint?)value; }
        }

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoCliente
        {
            get { return (int?)IdCliente; }
            set { IdCliente = (uint?)value; }
        }

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoFornecedor
        {
            get { return (int?)IdFornec; }
            set { IdFornec = (uint?)value; }
        }

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoTransportador
        {
            get { return (int?)IdTransportador; }
            set { IdTransportador = (uint?)value; }
        }

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoAdministradoraCartao
        {
            get { return null; }
            set { }
        }

        #endregion
    }
}
