using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class CorFerragemDAO : BaseDAO<CorFerragem, CorFerragemDAO>
	{
        //private CorFerragemDAO() { }

        private string SqlList(bool selecionar)
        {
            string campos = selecionar ? "*" : "Count(*)";

            string sql = "Select " + campos + " From cor_ferragem";

            return sql;
        }

        public IList<CorFerragem> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
            {
                var lst = new List<CorFerragem>();
                lst.Add(new CorFerragem());
                return lst.ToArray();
            }

            string filtro = String.IsNullOrEmpty(sortExpression) ? "Descricao" : sortExpression;

            return LoadDataWithSortExpression(SqlList(true), filtro, startRow, pageSize, false);
        }

        public int GetCountReal()
        {
            return GetCountWithInfoPaging(SqlList(true), false);
        }

        public int GetCount()
        {
            int count = GetCountReal();
            return count == 0 ? 1 : count;
        }

        public string GetNome(uint idCorFerragem)
        {
            return GetNome(null, idCorFerragem);
        }

        public string GetNome(GDASession session, uint idCorFerragem)
        {
            string sql = "select descricao from cor_ferragem where idCorFerragem=" + idCorFerragem;
            return objPersistence.ExecuteScalar(session, sql).ToString();
        }

        public string GetSigla(uint idCorFerragem)
        {
            return ObtemValorCampo<string>("coalesce(sigla, descricao)", "idCorFerragem=" + idCorFerragem);
        }

        public uint? GetIdByDescr(string descricao)
        {
            return ObtemValorCampo<uint?>("idCorFerragem", "descricao=?descr", new GDAParameter("?descr", descricao));
        }

        public override int DeleteByPrimaryKey(GDASession sessao, int key)
        {
            if (objPersistence.ExecuteSqlQueryCount(sessao, "Select * From produto where idCorFerragem=" + key) > 0)
                throw new Exception("Existem produtos associados à esta cor de ferragem, exclua os mesmos antes de excluir esta cor.");

            if (objPersistence.ExecuteSqlQueryCount(sessao, "Select * From item_projeto where idCorFerragem=" + key) > 0)
                throw new Exception("Esta cor não pode ser excluída, pois existem cálculos de projeto associados à mesma.");

            return base.DeleteByPrimaryKey(sessao, key);
        }

        public GenericModel[] GetForConfig()
        {
            List<GenericModel> retorno = new List<GenericModel>();
            foreach (CorFerragem c in GetAll())
                retorno.Add(new GenericModel(c.IdCorFerragem, c.Descricao));

            return retorno.ToArray();
        }
	}
}