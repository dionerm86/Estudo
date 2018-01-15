using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Projeto.Negocios
{
    /// <summary>
    /// Representa um contexto de referÊncia do CalcEngine.
    /// </summary>
    public class CalcEngineReferenceContext
    {
        #region Propriedades

        /// <summary>
        /// Nome do contexto.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descrição do contexto.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Valores de referencia do contexto.
        /// </summary>
        public CalcEngineReferenceValue[] ReferenceValues { get; set; }

        #endregion
    }

    /// <summary>
    /// Representa um valor de referência do CalcEngine.
    /// </summary>
    public class CalcEngineReferenceValue
    {
        #region Propriedades

        /// <summary>
        /// Caminho usado para chegar até o valor
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Códigos das ferragens
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Valor da Constante
        /// </summary>
        public double Value { get; set; }

        #endregion
    }

    /// <summary>
    /// Armazena as informaçoes do contexto de referencia do CalcEngine.
    /// </summary>
    public class CalcEngineReferenceContextInfo
    {
        #region Propriedades

        /// <summary>
        /// Nome do contexto.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descrição do contexto.
        /// </summary>
        public string Description { get; set; }

        #endregion
    }

    /// <summary>
    /// Assinatura do serviço de referências do CalcEngine.
    /// </summary>
    public interface ICalcEngineReferencesService
    {
        /// <summary>
        /// Recupera o contexto pelo nome informado.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        CalcEngineReferenceContext GetContext(string name);

        /// <summary>
        /// Recupera os contextos de referência.
        /// </summary>
        /// <returns></returns>
        IEnumerable<CalcEngineReferenceContextInfo> GetContexts();

    }
}
