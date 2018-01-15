using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Debito
{
    /// <summary>
    /// Assinatura do fluxo de negocio dos debitos.
    /// </summary>
    public interface IDebitoFluxo
    {
        /// <summary>
        /// Pesquisa os meus pedidos.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idPedido"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList<Glass.Api.Debito.IDebitoDescritor> ObterMeusPedidos(int idCliente, int idPedido, int startRow, int pageSize);

    }
}
