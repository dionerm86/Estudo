using Glass.Data.DAL;
using Glass.Data.RelModel;
using System.Collections.Generic;

namespace Glass.Data.RelDAL
{
    public sealed class FaturamentoCarregamentoaDAO : BaseDAO<FaturamentoCarregamento, FaturamentoCarregamentoaDAO>
    {
        public List<FaturamentoCarregamento> ObterDadosFaturamento(int idCarregamento)
        {
            var sql = @"
                SELECT p.IdCli as 'IdCliente', p.IdFormaPagto, GROUP_CONCAT(p.IdPedido) as 'IdsPedidos'
                FROM pedido p
	                INNER JOIN pedido_ordem_carga poc ON (p.idPedido = poc.idPedido)
	                INNER JOIN ordem_carga oc ON (poc.idOrdemCarga = oc.IdOrdemCarga)
	                INNER JOIN carregamento c ON (oc.idCarregamento = c.idCarregamento)
                WHERE c.idCarregamento = {0}
                GROUP BY p.IdCli, p.IdFormaPagto";

            return objPersistence.LoadData(string.Format(sql, idCarregamento));
        }
    }
}
