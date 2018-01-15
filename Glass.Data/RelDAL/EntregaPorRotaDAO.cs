using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.RelDAL
{
    public sealed class EntregaPorRotaDAO : BaseDAO<EntregaPorRota, EntregaPorRotaDAO>
    {
        //private EntregaPorRotaDAO() { }

        public IList<EntregaPorRota> ObterLista(uint idRota, string dataIni, string dataFim, string idsLiberacao)
        {
            string sql = @"Select l.IdLiberarPedido As IdLiberacao, CONCAT(c.id_cli,' - ',coalesce(c.Nome, c.NomeFantasia)) As NomeCliente,
                ci.NomeCidade As NomeCidade, l.IdLiberarPedido As Liberacao, pp.IdProdPed, Cast(SUM(pl.Qtde) as decimal (12,2)) As Qtde
                From liberarpedido l Inner Join cliente c On(l.IDCLIENTE=c.Id_Cli)
                Inner Join cidade ci ON(ci.IdCidade=c.IdCidade)
                Inner Join produtos_liberar_pedido pl On(l.IDLIBERARPEDIDO=pl.IDLIBERARPEDIDO)
                Inner Join produtos_pedido pp ON(pp.IdProdPed=pl.IDPRODPED)
                Inner Join produto p On(p.IDPROD=pp.IDPROD)
                Inner Join rota_cliente rc On (rc.`IDCLIENTE`=c.Id_Cli)
                Where rc.IDROTA=?rota And l.Situacao = 1";

            if (!string.IsNullOrEmpty(dataIni))
                sql += " And l.DataLiberacao>=?dataIni";
            if (!string.IsNullOrEmpty(dataFim))
                sql += " And l.DataLiberacao<=?dataFim";

            if (!string.IsNullOrEmpty(idsLiberacao))
                sql += " And l.IdLiberacao In(" + idsLiberacao + ")";

            sql += " Group By l.IdLiberarPedido Order By rc.NumSeq, Coalesce(c.Nome, c.NomeFantasia, '') Asc, l.idLiberarPedido Asc";

            var lista = objPersistence.LoadData(sql, new GDAParameter("?rota", idRota), 
                new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")),
                new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59"))).ToList();

            // Recalcula o valor liberado em cada liberação e o peso das peças
            foreach (EntregaPorRota item in lista)
            {
                item.Valor = (decimal)PedidoDAO.Instance.GetTotalLiberado(0, item.IdLiberacao.ToString());

                foreach (ProdutosLiberarPedido p in ProdutosLiberarPedidoDAO.Instance.GetForRpt(item.IdLiberacao))
                    item.Peso += p.Peso;
            }

            return lista;
        }

        public IList<EntregaPorRota> ObterListaRpt(uint idRota, string dataIni, string dataFim, string idsLiberacao)
        {
            string sql = @"
                Select l.IdLiberarPedido As IdLiberacao, CONCAT(c.id_cli,' - ',coalesce(c.Nome, c.NomeFantasia)) As NomeCliente, ci.NomeCidade As NomeCidade,
                    cast(group_concat(Distinct l.IdLiberarPedido) as char) As IdsLiberacao, pp.IdProdPed, 
                    cast(group_concat(Distinct pl.IdPedido) as char) As IdsPedido,
                    Cast(SUM(pl.Qtde) as decimal (12,2)) As Qtde
                From liberarpedido l Inner Join cliente c On(l.IDCLIENTE=c.Id_Cli)
                    Inner Join cidade ci ON(ci.IdCidade=c.IdCidade)
                    Inner Join produtos_liberar_pedido pl On(l.`IDLIBERARPEDIDO`=pl.`IDLIBERARPEDIDO`)
                    Inner Join `produtos_pedido` pp ON(pp.`IdProdPed`=pl.`IDPRODPED`)
                    Inner Join produto p On(p.`IDPROD`=pp.`IDPROD`)
                    Inner Join rota_cliente rc On (rc.`IDCLIENTE`=c.Id_Cli)
                Where rc.IDROTA=?rota And l.Situacao = 1";

            if (!string.IsNullOrEmpty(dataIni))
                sql += " And l.DataLiberacao>=?dataIni";
            if (!string.IsNullOrEmpty(dataFim))
                sql += " And l.DataLiberacao<=?dataFim";

            if (!string.IsNullOrEmpty(idsLiberacao))
                sql += " And l.IdLiberarPedido In(" + idsLiberacao + ")";

            sql += " Group By c.Id_Cli Order By rc.NumSeq Asc";

            var lista = objPersistence.LoadData(sql, new GDAParameter("?rota", idRota), 
                new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")),
                new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59"))).ToList();

            // Recalcula o valor liberado em cada liberação e o peso das peças
            foreach (EntregaPorRota item in lista)
            {
                string[] idsLiberarPedido = item.IdsLiberacao.Split(',');

                // Como o agrupamento é feito por cliente, é necessário somar o valor liberado e o peso em cada liberação deste cliente
                foreach (string idLib in idsLiberarPedido)
                {
                    item.Valor += (decimal)PedidoDAO.Instance.GetTotalLiberado(0, idLib);

                    foreach (ProdutosLiberarPedido p in ProdutosLiberarPedidoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(idLib)))
                        item.Peso += p.Peso;
                }
            }

            return lista;
        }
    }
}
