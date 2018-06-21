using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa o funcionário associado com faixa de rentabilidade para liberação.
    /// </summary>
    [PersistenceClass("funcionario_faixa_rentabilidade_liberacao")]
    public class FuncionarioFaixaRentabilidadeLiberacao : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador da faixa.
        /// </summary>
        [PersistenceProperty("IDFAIXARENTABILIDADELIBERACAO", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(FaixaRentabilidadeLiberacao), nameof(FaixaRentabilidadeLiberacao.IdFaixaRentabilidadeLiberacao))]
        public int IdFaixaRentabilidadeLiberacao { get; set; }

        /// <summary>
        /// Identificador do funcionário associado.
        /// </summary>
        [PersistenceProperty("IDFUNC", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Funcionario), nameof(Funcionario.IdFunc))]
        public int IdFunc { get; set; }

        #endregion
    }
}
