using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class CorAluminioDAO : BaseDAO<CorAluminio, CorAluminioDAO>
	{
        //private CorAluminioDAO() { }

        private string SqlList(bool selecionar)
        {
            string campos = selecionar ? "*" : "count(*)";

            string sql = "Select " + campos + " From cor_aluminio";

            return sql;
        }

        public IList<CorAluminio> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
            {
                var lst = new List<CorAluminio>();
                lst.Add(new CorAluminio());
                return lst.ToArray();
            }

            string filtro = String.IsNullOrEmpty(sortExpression) ? "Descricao" : sortExpression;

            return LoadDataWithSortExpression(SqlList(true), filtro, startRow, pageSize, false);
        }

        public int GetCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(false));
        }

        public int GetCount()
        {
            int count = GetCountReal();
            return count == 0 ? 1 : count;
        }

        public string GetNome(uint idCorAluminio)
        {
            return GetNome(null, idCorAluminio);
        }

        public string GetNome(GDASession session, uint idCorAluminio)
        {
            string sql = "select descricao from cor_aluminio where idCorAluminio=" + idCorAluminio;
            return objPersistence.ExecuteScalar(session, sql).ToString();
        }

        public string GetSigla(uint idCorAluminio)
        {
            return ObtemValorCampo<string>("coalesce(sigla, descricao)", "idCorAluminio=" + idCorAluminio);
        }

        public uint? GetIdByDescr(string descricao)
        {
            return ObtemValorCampo<uint?>("idCorAluminio", "descricao=?descr", new GDAParameter("?descr", descricao));
        }

        public override int DeleteByPrimaryKey(GDASession sessao, int key)
        {
            if (objPersistence.ExecuteSqlQueryCount(sessao, "Select * From produto where idCorAluminio=" + key) > 0)
                throw new Exception("Existem produtos associados à esta cor de alumínio, exclua os mesmos antes de excluir esta cor.");

            if (objPersistence.ExecuteSqlQueryCount(sessao, "Select * From item_projeto where idCorAluminio=" + key) > 0)
                throw new Exception("Esta cor não pode ser excluída, pois existem cálculos de projeto associados à mesma.");

            return base.DeleteByPrimaryKey(sessao, key);
        }

        public GenericModel[] GetForConfig()
        {
            List<GenericModel> retorno = new List<GenericModel>();
            foreach (CorAluminio c in GetAll())
                retorno.Add(new GenericModel(c.IdCorAluminio, c.Descricao));

            return retorno.ToArray();
        }
	}
}