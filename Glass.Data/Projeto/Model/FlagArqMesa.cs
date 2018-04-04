using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FlagArqMesaDAO))]
    [PersistenceClass("flag_arq_mesa")]
    public class FlagArqMesa : Colosoft.Data.BaseModel
    {
        #region Propiedades

        /// <summary>
        /// Identificador do flag
        /// </summary>
        [PersistenceProperty("IdFlagArqMesa", PersistenceParameterType.IdentityKey)]
        public int IdFlagArqMesa { get; set; }

        /// <summary>
        /// Descrição do flag
        /// </summary>
        [PersistenceProperty("Descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Indica se o flag é padrão para todas as peças do sistema
        /// </summary>
        [PersistenceProperty("Padrao")]
        public bool Padrao { get; set; }

        /// <summary>
        /// Indica em qual tipo de arquivo o flag sera informado
        /// </summary>
        [PersistenceProperty("TipoArquivo")]
        public TipoArquivoMesaCorte? TipoArquivo { get; set; }

        [Log("Situação da Flag")]
        [PersistenceProperty("SITUACAO")]
        public Situacao Situacao { get; set; }

        #endregion
    }
}
