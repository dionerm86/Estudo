using GDA;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa a associação da etiqueta com o arquivo de corte.
    /// </summary>
    [PersistenceBaseDAO(typeof(Glass.Data.DAL.EtiquetaArquivoCorteDAO))]
    [PersistenceClass("etiqueta_arquivo_corte")]
    public class EtiquetaArquivoCorte
    {
        #region Propriedades

        /// <summary>
        /// Número da etiqueta.
        /// </summary>
        [PersistenceProperty("NUMETIQUETA", PersistenceParameterType.Key)]
        public string NumEtiqueta { get; set; }

        /// <summary>
        /// Nome do arquivo associado.
        /// </summary>
        [PersistenceProperty("NOMEARQUIVO")]
        public string NomeArquivo { get; set; }

        #endregion
    }
}
