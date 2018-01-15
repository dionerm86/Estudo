using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.RelModel;

namespace Glass.Data.RelDAL
{
    public sealed class CapacidadeProducaoPedidoDAO : BaseDAO<CapacidadeProducaoPedido, CapacidadeProducaoPedidoDAO>
    {
        //private CapacidadeProducaoPedidoDAO() { }

        private CapacidadeProducaoPedido Converte(CapacidadeProducao item)
        {
            return new CapacidadeProducaoPedido()
            {
                IdPedido = item.IdPedido.GetValueOrDefault(),
                IdSetor = item.IdSetor,
                TotM = item.TotM,
                Criterio = item.Criterio
            };
        }

        public IList<CapacidadeProducaoPedido> ObtemListaPedidosCapacidadeProducao(DateTime dataProducao,
            uint idSetor, string sortExpression, int startRow, int pageSize)
        {
            var itens = CapacidadeProducaoDAO.Instance.ObtemListaPedidosCapacidadeProducao(dataProducao, idSetor, 
                sortExpression, startRow, pageSize);

            return itens.Select(x => Converte(x)).ToList();
        }

        public int ObtemNumeroPedidosCapacidadeProducao(DateTime dataProducao, uint idSetor)
        {
            return CapacidadeProducaoDAO.Instance.ObtemNumeroPedidosCapacidadeProducao(dataProducao, idSetor);
        }

        public IList<CapacidadeProducaoPedido> ObtemRelatorioPedidosCapacidadeProducao(DateTime dataProducao, uint idSetor)
        {
            var itens = CapacidadeProducaoDAO.Instance.ObtemRelatorioPedidosCapacidadeProducao(dataProducao, idSetor);
            return itens.Select(x => Converte(x)).ToList();
        }
    }
}
