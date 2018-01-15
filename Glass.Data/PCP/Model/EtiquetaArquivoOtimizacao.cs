using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(EtiquetaArquivoOtimizacaoDAO))]
    [PersistenceClass("etiqueta_arquivo_otimizacao")]
    public class EtiquetaArquivoOtimizacao
    {
        #region Propriedades

        [PersistenceProperty("IDETIQARQUIVOOTIMIZ", PersistenceParameterType.IdentityKey)]
        public uint IdEtiqArquivoOtimiz { get; set; }

        [PersistenceProperty("IDARQUIVOOTIMIZ")]
        public uint IdArquivoOtimiz { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("NUMETIQUETA")]
        public string NumEtiqueta { get; set; }

        [PersistenceProperty("TEMARQUIVOOTIMIZACAO")]
        public bool TemArquivoOtimizacao { get; set; }

        #endregion
    }
}