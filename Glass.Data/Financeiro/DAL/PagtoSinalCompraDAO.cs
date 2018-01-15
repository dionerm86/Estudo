using System.Linq;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PagtoSinalCompraDAO : BaseDAO<PagtoSinalCompra, PagtoSinalCompraDAO>
    {
        //private PagtoSinalCompraDAO() { }

        private string Sql(uint idSinalCompra, bool selecionar)
        {
            string campos = selecionar ? "ps.*, if(ps.idFormaPagto!=" + (uint)Glass.Data.Model.Pagto.FormaPagto.Obra + ", fp.descricao, 'Obra') as DescrFormaPagto" : "Count(*)";
            string sql = "select " + campos + @" from pagto_sinal_compra ps 
                left join formapagto fp on (ps.idFormaPagto=fp.idFormaPagto) 
                where ps.idSinalCompra=" + idSinalCompra + " order by ps.NumFormaPagto asc";

            return sql;
        }

        public PagtoSinalCompra[] GetBySinalCompra(GDASession session, uint idSinalCompra)
        {
            return objPersistence.LoadData(session, Sql(idSinalCompra, true)).ToArray();
        }

        public void DeleteBySinal(GDASession session, uint idSinalCompra)
        {
            objPersistence.ExecuteCommand(session, "delete from pagto_sinal_compra where idSinalCompra=" + idSinalCompra);
        }
    }
}
