using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class FuncEquipeDAO : BaseDAO<FuncEquipe, FuncEquipeDAO>
	{
        //private FuncEquipeDAO() { }

        public IList<FuncEquipe> GetByEquipe(uint idEquipe)
        {
            string sql = "Select fe.*, f.Nome as NomeFunc, t.Descricao as TipoFunc From func_equipe fe " +
                "Inner Join funcionario f On (fe.IdFunc=f.IdFunc) " + 
                "Inner Join tipo_func t On (f.IdTipoFunc=t.IdTipoFunc) " +
                "Where fe.IdEquipe=" + idEquipe + " Order By f.Nome";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Verificar se o funcionário passado já foi associado à esta equipe
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public bool IsAssociated(uint idFunc, uint idEquipe)
        {
            return objPersistence.ExecuteSqlQueryCount("Select Count(*) From func_equipe Where idFunc=" 
                + idFunc + " And idEquipe=" + idEquipe) > 0;
        }

        /// <summary>
        /// Retorna o número de integrantes de uma equipe.
        /// Usado no cálculo da comissão.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public int GetNumeroIntegrantes(uint idEquipe)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from func_equipe where idEquipe=" + idEquipe);
        }
	}
}