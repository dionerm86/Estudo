using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa um plano de corte.
    /// </summary>
    [PersistenceClass("plano_corte")]
    public class PlanoCorte : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Obtém ou define o identificador do plano de corte.
        /// </summary>
        [PersistenceProperty("IdPlanoCorte", PersistenceParameterType.IdentityKey)]
        public int IdPlanoCorte { get; set; }

        [PersistenceProperty("IdPlanoOtimizacao")]
        [PersistenceForeignKey(typeof(PlanoOtimizacao), "IdPlanoOtimizacao")]
        public int IdPlanoOtimizacao { get; set; }

        /// <summary>
        /// Obtém ou define a posição.
        /// </summary>
        [PersistenceProperty("Posicao")]
        public int Posicao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da chapa.
        /// </summary>
        [PersistenceProperty("IdChapa")]
        public string IdChapa { get; set; }

        /// <summary>
        /// Obtém ou define a largura do plano.
        /// </summary>
        [PersistenceProperty("Largura")]
        public double Largura { get; set; }

        /// <summary>
        /// Obtém ou define a altura do plano.
        /// </summary>
        [PersistenceProperty("Altura")]
        public double Altura { get; set; }

        #endregion
    }
}
