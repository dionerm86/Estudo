using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class ProtocoloCte
    {
        private Glass.Data.Model.Cte.ProtocoloCte _protocoloCte;
                
        #region contrutores  
      
        public ProtocoloCte()
        {
            _protocoloCte = new Glass.Data.Model.Cte.ProtocoloCte();
        }

        public ProtocoloCte(Glass.Data.Model.Cte.ProtocoloCte protocoloCte)
        {
            _protocoloCte = protocoloCte ?? new Glass.Data.Model.Cte.ProtocoloCte();
        }

        #endregion

        #region propriedades


        public uint IdCte
        {
            get { return _protocoloCte.IdCte; }
            set { _protocoloCte.IdCte = value; }
        }

        public int TipoProtocolo
        {
            get { return _protocoloCte.TipoProtocolo; }
            set { _protocoloCte.TipoProtocolo = value; }
        }

        public string TipoProtocoloString
        {
            get
            {
                return TipoProtocolo == (int)Glass.Data.Model.Cte.ProtocoloCte.TipoProtocoloEnum.Autorizacao ? "Autorizacao" :
                       TipoProtocolo == (int)Glass.Data.Model.Cte.ProtocoloCte.TipoProtocoloEnum.Denegacao ? "Denegacao" :
                       TipoProtocolo == (int)Glass.Data.Model.Cte.ProtocoloCte.TipoProtocoloEnum.Cancelamento ? "Cancelamento" :
                       TipoProtocolo == (int)Glass.Data.Model.Cte.ProtocoloCte.TipoProtocoloEnum.Inutilizacao ? "Inutilizacao" :
                    String.Empty;
            }
        }

        public string NumProtocolo
        {
            get { return _protocoloCte.NumProtocolo; }
            set { _protocoloCte.NumProtocolo = value; }
        }

        public DateTime DataCad
        {
            get { return _protocoloCte.DataCad; }
            set { _protocoloCte.DataCad = value; }
        }

        public string NumRecibo { get; set; }

        #endregion
    }
}
