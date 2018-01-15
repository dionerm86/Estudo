using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class DepartamentoDAO : BaseDAO<Departamento, DepartamentoDAO>
    {
        //private DepartamentoDAO() { }

        #region Busca padrão

        private string Sql(uint idDepartamento, string nome, bool selecionar)
        {
            string campos = selecionar ? "d.*" : "count(*)";

            string sql = "select " + campos + @"
                from departamento d
                where 1";

            if (idDepartamento > 0)
                sql += " and d.idDepartamento=" + idDepartamento;

            if (!String.IsNullOrEmpty(nome))
                sql += " and d.nome like ?nome";

            return sql;
        }

        private GDAParameter[] GetParams(string nome)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nome))
                lst.Add(new GDAParameter("?nome", "%" + nome + "%"));

            return lst.ToArray();
        }

        public IList<Departamento> GetList(uint idDepartamento, string nome, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idDepartamento, nome) == 0)
                return new Departamento[] { new Departamento() };

            return LoadDataWithSortExpression(Sql(idDepartamento, nome, true), sortExpression, startRow, pageSize, GetParams(nome));
        }

        public int GetCount(uint idDepartamento, string nome)
        {
            int count = GetCountReal(idDepartamento, nome);
            return count > 0 ? count : 1;
        }

        public int GetCountReal(uint idDepartamento, string nome)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idDepartamento, nome, false), GetParams(nome));
        }

        #endregion

        /// <summary>
        /// Retorna o nome do departamento.
        /// </summary>
        /// <param name="idDepartamento"></param>
        /// <returns></returns>
        public string GetNome(uint idDepartamento)
        {
            string sql = "select nome from departamento where idDepartamento=" + idDepartamento;
            object retorno = objPersistence.ExecuteScalar(sql);
            return retorno != null && retorno != DBNull.Value ? retorno.ToString() : null;
        }

        public override int Delete(Departamento objDelete)
        {
            string sql = "select count(*) from func_departamento where idDepartamento=" + objDelete.IdDepartamento;
            if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
                throw new Exception("Há funcionário(s) associado(s) ao mesmo.");

            return base.Delete(objDelete);
        }
    }
}
