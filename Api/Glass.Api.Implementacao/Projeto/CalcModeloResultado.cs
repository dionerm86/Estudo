using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Projeto
{
    /// <summary>
    /// Representa o resultado do cálculo do modelo.
    /// </summary>
    public class CalcModeloResultado : Glass.Api.Projeto.ICalcModeloResultado
    {
        #region Properties

        /// <summary>
        /// Identificador do projeto.
        /// </summary>
        public int IdProjeto { get; }

        /// <summary>
        /// Identificador do item projeto.
        /// </summary>
        public int IdItemProjeto { get; }

        /// <summary>
        /// Valor do item projeto.
        /// </summary>
        public string ValorItemProjeto { get; }

        /// <summary>
        /// Valor do projeto.
        /// </summary>
        public string ValorProjeto { get; }

        /// <summary>
        /// Metragem do item.
        /// </summary>
        public string M2ItemProjeto { get; }

        /// <summary>
        /// Valor do m2.
        /// </summary>
        public string ValorM2ItemProjeto { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <param name="idItemProjeto"></param>
        /// <param name="valorProjeto"></param>
        /// <param name="valorItemProjeto"></param>
        /// <param name="m2ItemProjeto"></param>
        /// <param name="valorM2ItemProjeto"></param>
        public CalcModeloResultado(int idProjeto, int idItemProjeto, string valorProjeto, string valorItemProjeto, string m2ItemProjeto, string valorM2ItemProjeto)
        {
            IdProjeto = idProjeto;
            IdItemProjeto = idItemProjeto;
            ValorProjeto = valorProjeto;
            ValorItemProjeto = valorItemProjeto;
            M2ItemProjeto = m2ItemProjeto;
            ValorM2ItemProjeto = valorM2ItemProjeto;
        }

        #endregion

    }
}
