using System;

namespace Glass.Data.Seguranca.Model
{
    /// <summary>
    /// Armazena os dados do token usado para monitorar o acesso ao sistema.
    /// </summary>
    [GDA.PersistenceClass("Token")]
    public class Token : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do token.
        /// </summary>
        [GDA.PersistenceProperty("IDTOKEN", GDA.PersistenceParameterType.IdentityKey)]
        public int IdToken { get; set; }

        /// <summary>
        /// Identificador do funcionário associado.
        /// </summary>
        [GDA.PersistenceProperty]
        [GDA.PersistenceForeignKey(typeof(Data.Model.Funcionario), "IdFunc")]
        public int? IdFunc { get; set; }

        /// <summary>
        /// Identificador do cliente associado.
        /// </summary>
        [GDA.PersistenceProperty("ID_CLI")]
        [GDA.PersistenceForeignKey(typeof(Data.Model.Cliente), "IdCli")]
        public int? IdCli { get; set; }

        /// <summary>
        /// Hash que representa o token.
        /// </summary>
        [GDA.PersistenceProperty]
        public string Hash { get; set; }

        /// <summary>
        /// Data de criação.
        /// </summary>
        [GDA.PersistenceProperty]
        public DateTime DataCad { get; set; }

        /// <summary>
        /// Quando o token expira.
        /// </summary>
        [GDA.PersistenceProperty]
        public DateTime Expira { get; set; }

        #endregion
    }
}
