using GDA;
using Glass.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa um calculo de rentabilidade.
    /// </summary>
    [PersistenceClass("expressao_rentabilidade")]
    public class ExpressaoRentabilidade : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do calculo.
        /// </summary>
        [PersistenceProperty("IDEXPRESSAORENTABILIDADE", PersistenceParameterType.IdentityKey)]
        public int IdExpressaoRentabilidade { get; set; }

        /// <summary>
        /// Posição da expressão.
        /// </summary>
        [PersistenceProperty("POSICAO")]
        public int Posicao { get; set; }

        /// <summary>
        /// Nome do calculo.
        /// </summary>
        [Log("Nome")]
        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        /// <summary>
        /// Descrição da expressão.
        /// </summary>
        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        /// <summary>
        /// Expressão.
        /// </summary>
        [Log("Expressão")]
        [PersistenceProperty("EXPRESSAO")]
        public string Expressao { get; set; }

        /// <summary>
        /// Formatação de exibição do valor da expressão
        /// </summary>
        [Log("Formatação")]
        [PersistenceProperty("FORMATACAO")]
        public string Formatacao { get; set; }

        /// <summary>
        /// Identifica se a expressão soma na fórmula da rentabilidade.
        /// </summary>
        [Log("Soma formula rentabilidade")]
        [PersistenceProperty("SOMAFORMULARENTABILIDADE")]
        public bool SomaFormulaRentabilidade { get; set; }

        #endregion
    }
}
