using GDA;
using Glass.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa um indicador financeiro.
    /// </summary>
    [PersistenceClass("indicador_financeiro")]
    public class IndicadorFinanceiro : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do indicador.
        /// </summary>
        [PersistenceProperty("IDINDICADORFINANCEIRO", PersistenceParameterType.IdentityKey)]
        public int IdIndicadorFinanceiro { get; set; }

        /// <summary>
        /// Nome do indicador.
        /// </summary>
        [Log("Nome")]
        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        /// <summary>
        /// Descrição do indicador financeiro.
        /// </summary>
        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        /// <summary>
        /// Valor.
        /// </summary>
        [Log("Valor")]
        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Formatação de exibição do valor da expressão
        /// </summary>
        [Log("Formatacao")]
        [PersistenceProperty("FORMATACAO")]
        public string Formatacao { get; set; }

        #endregion
    }
}
