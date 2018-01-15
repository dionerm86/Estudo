using GDA;
using System.ComponentModel;

namespace Glass.Data.Model
{
    /// <summary>
    /// Tipos de otimização do sistema
    /// </summary>
    public enum TipoOtimizacao
    {
        /// <summary>
        /// Não definido.
        /// </summary>
        [Description("Não definido")]
        Nenhum,
        /// <summary>
        /// Alumínio.
        /// </summary>
        [Description("Alumínio")]
        Aluminio,
        /// <summary>
        /// Vidro.
        /// </summary>
        [Description("Vidro")]
        Vidro 
    }

    [PersistenceClass("otimizacao")]
    public class Otimizacao : ModelBaseCadastro
    {
        #region Propiedades

        [PersistenceProperty("IdOtimizacao", PersistenceParameterType.IdentityKey)]
        public int IdOtimizacao { get; set; }

        [PersistenceProperty("Tipo")]
        public TipoOtimizacao Tipo { get; set; }

        #endregion
    }
}
