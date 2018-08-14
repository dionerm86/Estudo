using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa um plano de otimização.
    /// </summary>
    [PersistenceClass("plano_otimizacao")]
    public class PlanoOtimizacao : Colosoft.Data.BaseModel
    {
        /// <summary>
        /// Obtém ou define o identificador do plano de otimização.
        /// </summary>
        [PersistenceProperty("IdPlanoOtimizacao", PersistenceParameterType.IdentityKey)]
        public int IdPlanoOtimizacao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da solução de otimização pai.
        /// </summary>
        [PersistenceProperty("IdSolucaoOtimizacao")]
        [PersistenceForeignKey(typeof(SolucaoOtimizacao), nameof(SolucaoOtimizacao.IdSolucaoOtimizacao))]
        public int IdSolucaoOtimizacao { get; set; }

        /// <summary>
        /// Obtém ou define o plano de otimização.
        /// </summary>
        [PersistenceProperty("Nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [PersistenceProperty("IdProduto")]
        public int IdProduto { get; set; }
    }
}
