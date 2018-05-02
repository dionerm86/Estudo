using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Reprsenta a configuração do registro de rentabilidade.
    /// </summary>
    [PersistenceClass("config_registro_rentabilidade")]
    public class ConfigRegistroRentabilidade : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Tipo do registro.
        /// </summary>
        [PersistenceProperty("Tipo", PersistenceParameterType.Key)]
        public int Tipo { get; set; }

        /// <summary>
        /// Identificador do registro.
        /// </summary>
        [PersistenceProperty("IdRegistro", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(ExpressaoRentabilidade), "IdExpressaoRentabilidade")]
        public int IdRegistro { get; set; }

        /// <summary>
        /// Posição do registro.
        /// </summary>
        [PersistenceProperty("Posicao")]
        public int Posicao { get; set; }

        /// <summary>
        /// Identifica se é para exibir no relatório.
        /// </summary>
        [PersistenceProperty("ExibirRelatorio")]
        [Log.Log("Exibir Relatório")]
        public bool ExibirRelatorio { get; set; }

        #endregion
    }
}
