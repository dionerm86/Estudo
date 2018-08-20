using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa um retalho do plano de corte.
    /// </summary>
    [PersistenceClass("retalho_plano_corte")]
    public class RetalhoPlanoCorte : Colosoft.Data.BaseModel
    {
        /// <summary>
        /// Obtém ou define o identificador do retalho.
        /// </summary>
        [PersistenceProperty("IdRetalhoPlanoCorte", PersistenceParameterType.IdentityKey)]
        public int IdRetalhoPlanoCorte { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do plano de corte pai.
        /// </summary>
        [PersistenceProperty("IdPlanoCorte")]
        [PersistenceForeignKey(typeof(PlanoCorte), nameof(PlanoCorte.IdPlanoCorte))]
        public int IdPlanoCorte { get; set; }

        /// <summary>
        /// Obtém ou define o retalho de produção associado.
        /// </summary>
        [PersistenceProperty("IdRetalhoProducao")]
        [PersistenceForeignKey(typeof(RetalhoProducao), nameof(RetalhoProducao.IdRetalhoProducao))]
        public int? IdRetalhoProducao { get; set; }

        /// <summary>
        /// Obtém a posição.
        /// </summary>
        [PersistenceProperty("Posicao")]
        public int Posicao { get; set; }

        /// <summary>
        /// Obtém ou define a largura o retalho.
        /// </summary>
        [PersistenceProperty("Largura")]
        public double Largura { get; set; }

        /// <summary>
        /// Obtém ou define a altura do retalho.
        /// </summary>
        [PersistenceProperty("Altura")]
        public double Altura { get; set; }

        /// <summary>
        /// Obtém ou define se o retalho é reaproveitavel.
        /// </summary>
        [PersistenceProperty("Reaproveitavel")]
        public bool Reaproveitavel { get; set; }
    }
}
