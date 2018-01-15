using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public class CavaleteDAO : BaseDAO<Cavalete, CavaleteDAO>
    {
        /// <summary>
        /// Verifica se o cavalete existe
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public bool CavaleteExiste(GDASession sessao, string codInterno)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, "SELECT COUNT(*) FROM cavalete WHERE CodInterno = ?cod", new GDAParameter("?cod", codInterno)) > 0;
        }

        /// <summary>
        /// Recupera o identificador do cavalete
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public int? ObterIdCavalete(GDASession sessao, string codInterno)
        {
            return ExecuteScalar<int?>(sessao, "SELECT IdCavalete FROM cavalete WHERE CodInterno = ?cod", new GDAParameter("?cod", codInterno));
        }
    }
}
