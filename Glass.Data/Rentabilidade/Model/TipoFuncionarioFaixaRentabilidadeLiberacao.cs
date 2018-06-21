using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa a associada do tipo de funcionário com a faixa de rentabilidade para liberação.
    /// </summary>
    [PersistenceClass("tipo_funcionario_faixa_rentabilidade_liberacao")]
    public class TipoFuncionarioFaixaRentabilidadeLiberacao : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador da faixa de rentabiliade pai.
        /// </summary>
        [PersistenceProperty("IDFAIXARENTABILIDADELIBERACAO", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(FaixaRentabilidadeLiberacao), nameof(FaixaRentabilidadeLiberacao.IdFaixaRentabilidadeLiberacao))]
        public int IdFaixaRentabilidadeLiberacao { get; set; }

        /// <summary>
        /// Identificador do tipo de funcionário associado.
        /// </summary>
        [PersistenceProperty("IDTIPOFUNCIONARIO", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(TipoFuncionario), nameof(TipoFuncionario.IdTipoFuncionario))]
        public int IdTipoFuncionario { get; set; }

        #endregion
    }
}
