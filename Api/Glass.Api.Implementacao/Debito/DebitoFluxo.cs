using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Debito
{
    /// <summary>
    /// Implementação da regra de negocio do débito.
    /// </summary>
    public class DebitoFluxo : Glass.Api.Debito.IDebitoFluxo
    {
        /// <summary>
        /// Pesquisa os meus pedidos.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idPedido"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<Glass.Api.Debito.IDebitoDescritor> ObterMeusPedidos(int idCliente, int idPedido, int startRow, int pageSize)
        {
            return Glass.Data.DAL.ContasReceberDAO.Instance.GetDebitosListParceiros
                ((uint)idCliente, (uint)idPedido, 0, true, 0, "DataVec Desc", startRow, pageSize)
                .Select(f => new Glass.Api.Implementacao.Debito.DebitoDescritor(f))
                .ToList<Glass.Api.Debito.IDebitoDescritor>();
        }
    }
}
