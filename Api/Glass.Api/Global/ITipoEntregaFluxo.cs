using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Global
{
    /// <summary>
    /// Assinatura da entidade de negocio tipo de entrega.
    /// </summary>
    public interface ITipoEntrega
    {
        /// <summary>
        /// Identificador.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Descrição.
        /// </summary>
        string Descricao { get;}
    }

    /// <summary>
    /// Assinatura do fluxo de negocio do tipo de entrega.
    /// </summary>
    public interface ITipoEntregaFluxo
    {
        /// <summary>
        /// Recupera os tipos de entrega.
        /// </summary>
        /// <returns></returns>
        IList<ITipoEntrega> ObterTiposEntrega();

        /// <summary>
        /// Recupera os tipos de entrega de parceiros.
        /// </summary>
        /// <returns></returns>
        IList<ITipoEntrega> ObterTiposEntregaParceiros();

        /// <summary>
        /// Recupera o tipo de entrega padrão.
        /// </summary>
        /// <returns></returns>
        int ObterTipoEntregaPadrao();     
    }
}
