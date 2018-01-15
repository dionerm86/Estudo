using GDA;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa a associação da etiqueta com o arquivo de corte.
    /// </summary>
    [PersistenceBaseDAO(typeof(Glass.Data.DAL.EtiquetaExportacaoDAO))]
    [PersistenceClass("etiqueta_exportacao")]
    public class EtiquetaExportacao
    {
        #region Propriedades

        /// <summary>
        /// Identificador do produto pedido
        /// </summary>
        [PersistenceProperty("IdProdPed", PersistenceParameterType.Key)]
        public int IdProdPed { get; set; }

        /// <summary>
        /// Número da etiqueta.
        /// </summary>
        [PersistenceProperty("NUMETIQUETA", PersistenceParameterType.Key)]
        public string NumEtiqueta { get; set; }

        #endregion

    }
}
