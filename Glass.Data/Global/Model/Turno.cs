using GDA;
using Glass.Data.DAL;
using System.ComponentModel;
using Colosoft;

namespace Glass.Data.Model
{
    /// <summary>
    /// Possíveis sequencias do turno.
    /// </summary>
    public enum TurnoSequencia
    {
        /// <summary>
        /// Turno 1.
        /// </summary>
        [Description("1º Turno")]
        Turno1 = 1,
        /// <summary>
        /// Turno 2.
        /// </summary>
        [Description("2º Turno")]
        Turno2,
        /// <summary>
        /// Turno 3.
        /// </summary>
        [Description("3º Turno")]
        Turno3,
        /// <summary>
        /// Turno 4.
        /// </summary>
        [Description("4º Turno")]
        Turno4,
        /// <summary>
        /// Turno 5.
        /// </summary>
        [Description("5º Turno")]
        Turno5
    }

    [PersistenceBaseDAO(typeof(TurnoDAO))]
    [PersistenceClass("turnos")]
    public class Turno : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IdTurno", PersistenceParameterType.IdentityKey)]
        public int IdTurno { get; set; }

        [PersistenceProperty("Descricao")]
        public string Descricao { get; set; }

        [PersistenceProperty("NumSeq")]
        public TurnoSequencia NumSeq { get; set; }

        [PersistenceProperty("Inicio")]
        public string Inicio { get; set; }

        [PersistenceProperty("Termino")]
        public string Termino { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string SequenciaString
        {
            get
            {
                return NumSeq.Translate().Format();
            }
        }

        #endregion
    }
}