using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class FuncDepartamentoDAO : BaseDAO<FuncDepartamento, FuncDepartamentoDAO>
    {
        //private FuncDepartamentoDAO() { }

        private string Sql(uint idDepartamento, uint idFunc, bool selecionar)
        {
            string campos = selecionar ? "fd.*, d.nome as nomeDepartamento, f.nome as nomeFunc" : "count(*)";
            string sql = "select " + campos + @"
                from func_departamento fd
                    left join departamento d on (fd.idDepartamento=d.idDepartamento)
                    left join funcionario f on (fd.idFunc=f.idFunc)
                where 1";

            if (idDepartamento > 0)
                sql += " and fd.idDepartamento=" + idDepartamento;

            if (idFunc > 0)
                sql += " and fd.idFunc=" + idFunc;

            return sql;
        }

        public IList<FuncDepartamento> GetByDepartamento(uint idDepartamento)
        {
            return objPersistence.LoadData(Sql(idDepartamento, 0, true)).ToList();
        }

        public FuncDepartamento[] GetByFunc(uint idFunc)
        {
            return objPersistence.LoadData(Sql(0, idFunc, true)).ToList().ToArray();
        }

        /// <summary>
        /// Recupera os nomes dos departamentos por funcionário.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public string GetNomesDepartamentosByFunc(uint idFunc)
        {
            string retorno = "";
            foreach (FuncDepartamento f in GetByFunc(idFunc))
                retorno += f.NomeDepartamento + ", ";

            return retorno.TrimEnd(' ', ',');
        }
    }
}
