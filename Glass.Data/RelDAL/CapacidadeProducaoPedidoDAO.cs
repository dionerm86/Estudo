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

        public IList<CapacidadeProducaoPedido> ObtemListaPedidosCapacidadeProducao(DateTime dataProducao, string horaInicial, string horaFinal, uint idSetor, string sortExpression, int startRow,
            int pageSize)
        {
            return CapacidadeProducaoDAO.Instance.ObtemListaPedidosCapacidadeProducao(dataProducao, horaInicial, horaFinal, idSetor, sortExpression, startRow, pageSize).Select(x => Converte(x)).ToList();
        }

        public int ObtemNumeroPedidosCapacidadeProducao(DateTime dataProducao, string horaInicial, string horaFinal, uint idSetor)
        {
            return CapacidadeProducaoDAO.Instance.ObtemNumeroPedidosCapacidadeProducao(dataProducao, horaInicial, horaFinal, idSetor);
        }

        public IList<CapacidadeProducaoPedido> ObtemRelatorioPedidosCapacidadeProducao(DateTime dataProducao, string horaInicial, string horaFinal, uint idSetor)
        {
            return CapacidadeProducaoDAO.Instance.ObtemRelatorioPedidosCapacidadeProducao(dataProducao, horaInicial, horaFinal, idSetor).Select(x => Converte(x)).ToList();
        }
    }
}
