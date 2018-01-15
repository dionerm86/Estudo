using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class DocumentoArrecadacaoDAO : BaseDAO<DocumentoArrecadacao, DocumentoArrecadacaoDAO>
    {
        //private DocumentoArrecadacaoDAO() { }

        private string Sql(uint idNf, bool selecionar)
        {
            string campos = selecionar ? "da.*" : "count(*)";
            string sql = "select " + campos + @"
                from documento_arrecadacao da
                where 1";

            if (idNf > 0)
                sql += " and da.idNf=" + idNf;

            return sql;
        }

        public IList<DocumentoArrecadacao> GetList(uint idNf, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idNf) == 0)
                return new DocumentoArrecadacao[] { new DocumentoArrecadacao() };

            return LoadDataWithSortExpression(Sql(idNf, true), sortExpression, startRow, pageSize);
        }

        public int GetCount(uint idNf)
        {
            int retorno = GetCountReal(idNf);
            return retorno > 0 ? retorno : 1;
        }

        public int GetCountReal(uint idNf)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idNf, false));
        }

        public IList<DocumentoArrecadacao> GetForEFD(uint idNf)
        {
            return objPersistence.LoadData(Sql(idNf, true)).ToList();
        }
    }
}
