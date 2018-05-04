using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade
{
    /// <summary>
    /// Representa os possíveis tipos de registro de rentabilidade.
    /// </summary>
    public enum TipoRegistroRentabilidade : int
    {
        /// <summary>
        /// Identifica que é um indicador financeiro.
        /// </summary>
        [System.ComponentModel.Description("Indicador Financeiro")]
        IndicadorFinaceiro = 1,
        /// <summary>
        /// Identifica que é uma variável do item.
        /// </summary>
        [System.ComponentModel.Description("Variável")]
        VariavelItem,
        /// <summary>
        /// Identifica que é uma expressão.
        /// </summary>
        [System.ComponentModel.Description("Expressão")]
        Expressao
    }

    /// <summary>
    /// Representa um registro do valor associado a rentabilidade.
    /// </summary>
    public interface IRegistroRentabilidade
    {
        #region Propriedades

        /// <summary>
        /// Descritor do registro.
        /// </summary>
        DescritorRegistroRentabilidade Descritor { get; }

        /// <summary>
        /// Tipo do registro.
        /// </summary>
        TipoRegistroRentabilidade Tipo { get; }

        /// <summary>
        /// Valor do registro.
        /// </summary>
        decimal Valor { get; }

        #endregion
    }
}
