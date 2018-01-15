using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.Data.RelDAL
{
    public sealed class PrevFinanPedidosDAO : BaseDAO<PrevFinanPedidos, PrevFinanPedidosDAO>
    {
        //private PrevFinanPedidosDAO() { }

        private string SqlRpt(uint idLoja, string dataIni, string dataFim)
        {
            string campos = @"
                p.idPedido, p.CodCliente, p.DataEntrega as Entrega, p.situacaoProducao, c.Nome as Cliente, c.id_cli as IdCliente,
                f.Nome as Funcionario, l.NomeFantasia as Loja,
                (COALESCE(pe.total, p.total) - IF(p.idSinal > 0 And p.valorEntrada > 0, COALESCE(p.valorEntrada, 0), 0) -
                IF(p.idPagamentoAntecipado > 0 And p.valorPagamentoAntecipado > 0, COALESCE(p.valorPagamentoAntecipado, 0), 0)) AS valor";

            string situacoesPedido = (int)Pedido.SituacaoPedido.Conferido + "," + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao;
            string tiposVenda = (int)Pedido.TipoVendaPedido.APrazo + "," + (int)Pedido.TipoVendaPedido.AVista + "," + (int)Pedido.TipoVendaPedido.Funcionario;
            string tiposPedido = (int)Pedido.TipoPedidoEnum.MaoDeObra + "," + (int)Pedido.TipoPedidoEnum.Revenda + "," + (int)Pedido.TipoPedidoEnum.Venda;

            string sql = @"
                SELECT " + campos + @"
                FROM pedido p
                    LEFT JOIN pedido_espelho pe ON (pe.idPedido = p.idPedido)
                    LEFT JOIN cliente c ON (c.id_cli = p.idCli)
                    LEFT JOIN loja l ON (l.idLoja = p.idLoja)
                    LEFT JOIN funcionario f ON (f.idFunc = p.idFunc)
                WHERE p.situacao IN (" + situacoesPedido + @")
                    AND p.tipoVenda IN (" + tiposVenda + @")
                    AND p.tipoPedido IN (" + tiposPedido + @")";

            if (idLoja > 0)
                sql += " AND p.idLoja = " + idLoja;

            if (!string.IsNullOrEmpty(dataIni))
                sql += " AND p.dataEntrega >= ?dataIni";

            if (!string.IsNullOrEmpty(dataFim))
                sql += " AND p.dataEntrega <= ?dataFim";

            sql += " group by p.IdPedido having valor > 0";

            return sql;
        }

        public IList<PrevFinanPedidos> GetPrevFinanPedidosRpt(uint idLoja, string dataIni, string dataFim)
        {
            var prevFinanPedidos = objPersistence.LoadData(SqlRpt(idLoja, dataIni, dataFim), GetParams(dataIni, dataFim)).ToList();

            for (int i = 0; i < prevFinanPedidos.Count; i++)
                if (prevFinanPedidos[i].Valor == 0)
                    prevFinanPedidos.RemoveAt(i);

            return prevFinanPedidos;
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lista = new List<GDAParameter>();

            lista.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));
            lista.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lista.ToArray();
        }
    }
}
