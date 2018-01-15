namespace Glass.Data.RelModel
{
    /// <summary>
    /// Representa o registro do relatório de DIPJ.
    /// </summary>
    [GDA.PersistenceBaseDAO(typeof(RelDAL.DipjDAO))]
    [GDA.PersistenceClass("dipj")]
    public class Dipj
    {
        #region Propriedades

        /// <summary>
        /// Identificação.
        /// </summary>
        [GDA.PersistenceProperty("Id")]
        public string Id { get; set; }

        /// <summary>
        /// Nome.
        /// </summary>
        [GDA.PersistenceProperty("Nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Valor contábil.
        /// </summary>
        [GDA.PersistenceProperty("ValorContabil")]
        public decimal ValorContabil { get; set; }
        
        /// <summary>
        /// Valor do IPI.
        /// </summary>
        [GDA.PersistenceProperty("ValorIPI")]
        public decimal ValorIPI { get; set; }

        /// <summary>
        /// Valor contábil do IPI.
        /// </summary>
        [GDA.PersistenceProperty("ValorContabilIPI")]
        public decimal ValorContabilIPI { get; set; }

        #endregion
    }
}
