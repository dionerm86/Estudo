using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class ParticipanteCte
    {
        private Glass.Data.Model.Cte.ParticipanteCte _participanteCte;

        #region construtores

        public ParticipanteCte()
            : this(new Glass.Data.Model.Cte.ParticipanteCte())
        {
        }

        internal ParticipanteCte(Glass.Data.Model.Cte.ParticipanteCte participanteCte)
        {
            _participanteCte = participanteCte ?? new Glass.Data.Model.Cte.ParticipanteCte();
        }

        #endregion

        #region Propriedades

        public uint IdCte
        {
            get { return _participanteCte.IdCte; }
            set { _participanteCte.IdCte = value; }
        }

        public int NumSeq
        {
            get { return _participanteCte.NumSeq; }
            set { _participanteCte.NumSeq = value; }
        }

        public Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum TipoParticipante
        {
            get { return _participanteCte.TipoParticipante; }
            set { _participanteCte.TipoParticipante = value; }
        }

        public uint? IdLoja
        {
            get { return _participanteCte.IdLoja; }
            set { _participanteCte.IdLoja = value; }
        }

        public uint? IdFornec
        {
            get { return _participanteCte.IdFornec; }
            set { _participanteCte.IdFornec = value; }
        }

        public uint? IdCliente
        {
            get { return _participanteCte.IdCliente; }
            set { _participanteCte.IdCliente = value; }
        }

        public uint? IdTransportador
        {
            get { return _participanteCte.IdTransportador; }
            set { _participanteCte.IdTransportador = value; }
        }

        public bool Tomador
        {
            get { return _participanteCte.Tomador; }
            set { _participanteCte.Tomador = value; }
        }

        public string DescricaoTipoParticipante
        {
            get { return TipoParticipante.ToString(); }
        }

        public string NomeParticipante
        {
            get
            {
                return IdLoja > 0 ? Glass.Data.DAL.LojaDAO.Instance.GetNome(IdLoja.Value) :
                    IdFornec > 0 ? Glass.Data.DAL.FornecedorDAO.Instance.GetNome(IdFornec.Value) :
                    IdCliente > 0 ? Glass.Data.DAL.ClienteDAO.Instance.GetNome(IdCliente.Value) :
                    IdTransportador > 0 ? Glass.Data.DAL.TransportadorDAO.Instance.GetNome(IdTransportador.Value) :
                    String.Empty;
            }
        }

        #endregion
    }
}
