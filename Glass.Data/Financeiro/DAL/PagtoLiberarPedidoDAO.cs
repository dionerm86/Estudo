using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PagtoLiberarPedidoDAO : BaseDAO<PagtoLiberarPedido, PagtoLiberarPedidoDAO>
    {
        //private PagtoLiberarPedidoDAO() { }

        private string Sql(uint idLiberarPedido, bool selecionar)
        {
            string campos = selecionar ? "plp.*, if(plp.idFormaPagto=" + (uint)Glass.Data.Model.Pagto.FormaPagto.Obra + ", 'Obra', if(plp.idFormaPagto=" + 
                (uint) Glass.Data.Model.Pagto.FormaPagto.Credito + ", 'Crédito', fp.descricao)) as DescrFormaPagto" : "Count(*)";
            string sql = "select " + campos + " from pagto_liberar_pedido plp " +
                "left join formapagto fp on (plp.idFormaPagto=fp.idFormaPagto) " +
                "where 1";

            if (idLiberarPedido > 0)
                sql += " And idLiberarPedido=" + idLiberarPedido;

            return sql + " order by plp.NumFormaPagto asc";
        }

        public IList<PagtoLiberarPedido> GetByLiberacao(uint idLiberarPedido)
        {
            return GetByLiberacao(null, idLiberarPedido);
        }

        public IList<PagtoLiberarPedido> GetByLiberacao(GDASession sessao, uint idLiberarPedido)
        {
            return objPersistence.LoadData(sessao, Sql(idLiberarPedido, true)).ToList();
        }

        public void DeleteByLiberacao(GDASession sessao, uint idLiberarPedido)
        {
            objPersistence.ExecuteCommand(sessao, "delete from pagto_liberar_pedido where idLiberarPedido=" + idLiberarPedido);
        }

        public string ObtemFormaPagtoParaNf(uint idNf)
        {
            var sql = @"
                SELECT DISTINCT fp.descricao
                FROM pedidos_nota_fiscal pnf
                    LEFT JOIN pagto_liberar_pedido plp ON (pnf.IdLiberarPedido = plp.IdLiberarPedido)
                    LEFT JOIN formapagto fp ON (plp.IdFormaPagto = fp.IdFormaPagto)
                WHERE pnf.idnf = " + idNf + @"
                    AND plp.idliberarPedido IS NOT NULL";

            var dados = ExecuteMultipleScalar<string>(sql);

            return string.Join(", ", dados.ToArray());
        }

        public void AtualizarNumAutCartao(GDASession sessao, int idLiberarPedido, int numFormaPagto, string numAut)
        {
            var sql = @"UPDATE pagto_liberar_pedido SET NumAutCartao = ?numAut WHERE IdLiberarPedido = ?idLib AND NumFormaPagto = ?numFormaPagto";

            objPersistence.ExecuteCommand(sessao, sql,
                new GDAParameter("?numAut", numAut),
                new GDAParameter("?idLib", idLiberarPedido),
                new GDAParameter("?numFormaPagto", numFormaPagto));
        }
    }
}
