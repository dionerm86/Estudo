using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PedidoReposicaoDAO))]
    [PersistenceClass("pedido_reposicao")]
    public class PedidoReposicao
    {
        #region Propriedades

        [PersistenceProperty("IDPEDREPOS", PersistenceParameterType.IdentityKey)]
        public uint IdPedRepos { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("ASSUNTO")]
        public string Assunto { get; set; }

        [PersistenceProperty("SOLUCAO")]
        public string Solucao { get; set; }

        [PersistenceProperty("DATACLIENTEINFORMADO")]
        public DateTime? DataClienteInformado { get; set; }

        /// <summary>
        /// Indica se o pedido pode ser utilizado como troca.
        /// </summary>
        [PersistenceProperty("PODEUTILIZARTROCA")]
        public bool PodeUtilizarTroca { get; set; }

        #endregion
    }
}