using GDA;
using Glass.Data.DAL;
using System;
using System.ComponentModel;

namespace Glass.Data.Model
{
    #region Enumeradores

    public enum TipoParticipanteEnum
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

    #endregion

    [Serializable]
    [PersistenceBaseDAO(typeof(ParticipanteMDFeDAO))]
    [PersistenceClass("participante_mdfe")]
    public class ParticipanteMDFe
    {
        #region Propriedades

        [PersistenceProperty("IDMANIFESTOELETRONICO")]
        [PersistenceForeignKey(typeof(ManifestoEletronico), "IdManifestoEletronico")]
        public int IdManifestoEletronico { get; set; }

        [PersistenceProperty("NUMSEQ")]
        public int NumSeq { get; set; }

        [PersistenceProperty("TIPOPARTICIPANTE")]
        public TipoParticipanteEnum TipoParticipante { get; set; }

        [PersistenceProperty("IDLOJA")]
        public int? IdLoja { get; set; }

        [PersistenceProperty("IDFORNECEDOR")]
        public int? IdFornecedor { get; set; }

        [PersistenceProperty("IDCLIENTE")]
        public int? IdCliente { get; set; }

        [PersistenceProperty("IDTRANSPORTADOR")]
        public int? IdTransportador { get; set; }

        [PersistenceProperty("TOMADOR")]
        public bool Tomador { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string Emitente
        {
            get
            {
                if (TipoParticipante == TipoParticipanteEnum.Emitente)
                    return ObtemNomeParticipante();

                return string.Empty;
            }
        }

        public string Contratante
        {
            get
            {
                if (TipoParticipante == TipoParticipanteEnum.Contratante)
                    return ObtemNomeParticipante();

                return string.Empty;
            }
        }

        public string ObtemNomeParticipante()
        {
            if (IdLoja > 0)
            {
                var loja = LojaDAO.Instance.GetElement((uint)IdLoja.GetValueOrDefault());

                if (loja != null)
                    return loja.NomeFantasia;
            }
            else if (IdFornecedor > 0)
            {
                var fornec = FornecedorDAO.Instance.GetElement((uint)IdFornecedor.GetValueOrDefault());

                if (fornec != null)
                    return fornec.Nomefantasia;
            }
            else if (IdCliente > 0)
            {
                var cliente = ClienteDAO.Instance.GetElement((uint)IdCliente.GetValueOrDefault());

                if (cliente != null)
                    return cliente.Nome;
            }
            else if (IdTransportador > 0)
            {
                var transp = TransportadorDAO.Instance.GetElement((uint)IdTransportador.GetValueOrDefault());

                if (transp != null)
                    return transp.NomeFantasia;
            }

            return string.Empty;
        }

        #endregion
    }
}
