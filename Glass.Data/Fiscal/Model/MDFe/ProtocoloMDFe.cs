using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [Serializable]
    [PersistenceBaseDAO(typeof(ProtocoloMDFeDAO))]
    [PersistenceClass("protocolo_mdfe")]
    public class ProtocoloMDFe
    {
        #region Enumeradores

        public enum TipoProtocoloEnum
        {
            Autorizacao,
            Cancelamento,
            Encerramento
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDMANIFESTOELETRONICO", PersistenceParameterType.Key)]
        public int IdManifestoEletronico { get; set; }

        [PersistenceProperty("TIPOPROTOCOLO", PersistenceParameterType.Key)]
        public int TipoProtocolo { get; set; }

        [PersistenceProperty("NUMPROTOCOLO")]
        public string NumProtocolo { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("NUMRECIBO")]
        public string NumRecibo { get; set; }

        #endregion
    }
}
