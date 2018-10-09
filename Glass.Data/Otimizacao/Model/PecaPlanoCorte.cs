using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa uma peça do plano de corte.
    /// </summary>
    [PersistenceClass("peca_plano_corte")]
    public class PecaPlanoCorte : Colosoft.Data.BaseModel
    {
        /// <summary>
        /// Obtém ou define identificador da peça do plano de corte.
        /// </summary>
        [PersistenceProperty("IdPecaPlanoCorte", PersistenceParameterType.IdentityKey)]
        public int IdPecaPlanoCorte { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do plano de corte.
        /// </summary>
        [PersistenceProperty("IdPlanoCorte")]
        [PersistenceForeignKey(typeof(PlanoCorte), nameof(PlanoCorte.IdPlanoCorte))]
        public int IdPlanoCorte { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto pedido produção asssociado.
        /// </summary>
        [PersistenceProperty("IdProdPedProducao")]
        [PersistenceForeignKey(typeof(ProdutoPedidoProducao), nameof(ProdutoPedidoProducao.IdProdPedProducao))]
        public int? IdProdPedProducao { get; set; }

        /// <summary>
        /// Obtém ou define a posição da peça.
        /// </summary>
        [PersistenceProperty("Posicao")]
        public int Posicao { get; set; }

        /// <summary>
        /// Obtém ou define se a peça foi rotacionada.
        /// </summary>
        [PersistenceProperty("Rotacionada")]
        public bool Rotacionada { get; set; }

        /// <summary>
        /// Obtém ou define a forma da peça.
        /// </summary>
        [PersistenceProperty("Forma")]
        public string Forma { get; set; }

        /// <summary>
        /// Obtém ou define a posição geral da peça.
        /// </summary>
        [PersistenceProperty("PosicaoGeral")]
        public int PosicaoGeral { get; set; }
    }
}
