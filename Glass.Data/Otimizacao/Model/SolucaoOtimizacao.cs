using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa uma solução de otimização.
    /// </summary>
    [PersistenceClass("solucao_otimizacao")]
    public class SolucaoOtimizacao : Colosoft.Data.BaseModel
    {
        #region Properties

        /// <summary>
        /// Obtém ou define o identifiador da solução.
        /// </summary>
        [PersistenceProperty("IdSolucaoOtimizacao", PersistenceParameterType.IdentityKey)]
        public int IdSolucaoOtimizacao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do arquivo de otimização.
        /// </summary>
        [PersistenceProperty("IdArquivoOtimiz")]
        [PersistenceForeignKey(typeof(ArquivoOtimizacao), nameof(ArquivoOtimizacao.IdArquivoOtimizacao))]
        public int IdArquivoOtimizacao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador único da solução.
        /// </summary>
        [PersistenceProperty("Uid")]
        public Guid Uid { get; set; } = Guid.NewGuid();

        #endregion
    }
}
