using System.Collections.Generic;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class SeguradoraDAO : BaseDAO<Seguradora, SeguradoraDAO>
    {
        //private SeguradoraDAO() { }

        #region Busca padrão

        private string Sql(uint idSeguradora, string nomeSeguradora, bool selecionar)
        {
            string sql = "Select * From seguradora Where 1";

            if(!selecionar)
                sql = "Select count(*) From seguradora Where 1";

            if (idSeguradora > 0)
                sql += " And IDSEGURADORA=" + idSeguradora;
            if(!string.IsNullOrEmpty(nomeSeguradora))
                sql += " And NOMESEGURADORA=" + nomeSeguradora;
            return sql;
        }

        public Seguradora GetElement(uint idSeguradora)
        {
            return GetElement(null, idSeguradora);
        }

        public Seguradora GetElement(GDASession session, uint idSeguradora)
        {
            try
            {
                return objPersistence.LoadOneData(session, Sql(idSeguradora, "", true));
            }
            catch
            {
                return new Seguradora();
            }
        }

        public Seguradora GetElementByName(string nomeSeguradora)
        {
            return objPersistence.LoadOneData(Sql(0, nomeSeguradora, true));
        }

        public IList<Seguradora> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, "", true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, "", false), null);
        }

        #endregion

        public string ObtemNomeSeguradora(uint idSeguradora)
        {
            return ObtemValorCampo<string>("nomeSeguradora", "idSeguradora=" + idSeguradora);
        }

        public override uint Insert(Seguradora objInsert)
        {
            uint idSeguradora = base.Insert(objInsert);

            return idSeguradora;
        }

        public override int Update(Seguradora objUpdate)
        {
            return base.Update(objUpdate);
        }
    }
}
