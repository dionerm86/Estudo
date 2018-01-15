using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PagtoAcertoDAO : BaseDAO<PagtoAcerto, PagtoAcertoDAO>
    {
        //private PagtoAcertoDAO() { }

        private string Sql(uint idAcerto, bool selecionar)
        {
            string campos = selecionar ? "pa.*, fp.descricao as FormaPagto" : "Count(*)";
            
            string sql = @"
                Select " + campos + @" 
                From pagto_acerto pa 
                    Left Join formapagto fp on (pa.idFormaPagto = fp.idFormaPagto) 
                Where 1";

            if (idAcerto > 0)
                sql += " And pa.idAcerto=" + idAcerto;

            return sql + " order by pa.NumFormaPagto asc";
        }

        public IList<PagtoAcerto> GetByAcerto(uint idAcerto)
        {
            return GetByAcerto(null, idAcerto);
        }

        public IList<PagtoAcerto> GetByAcerto(GDASession session, uint idAcerto)
        {
            return objPersistence.LoadData(session, Sql(idAcerto, true)).ToList();
        }

        public void DeleteByAcerto(uint idAcerto)
        {
            objPersistence.ExecuteCommand("delete from pagto_acerto where idAcerto=" + idAcerto);
        }

        public void AtualizarNumAutCartao(GDASession sessao, int idAcerto, int numFormaPagto, string numAut)
        {
            var sql = @"UPDATE pagto_acerto SET NumAutCartao = ?numAut WHERE IdAcerto = ?idAcerto AND NumFormaPagto = ?numFormaPagto";

            objPersistence.ExecuteCommand(sessao, sql,
                new GDAParameter("?numAut", numAut),
                new GDAParameter("?idAcerto", idAcerto),
                new GDAParameter("?numFormaPagto", numFormaPagto));
        }
    }
}
