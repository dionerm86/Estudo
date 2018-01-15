using System.Linq;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class FotosConciliacaoBancariaDAO : BaseDAO<FotosConciliacaoBancaria, FotosConciliacaoBancariaDAO>
    {
        //private FotosConciliacaoBancariaDAO() { }

        /// <summary>
        /// Retorna todas as fotos da conciliação bancária.
        /// </summary>
        /// <param name="idConciliacaoBancaria"></param>
        /// <returns></returns>
        public FotosConciliacaoBancaria[] GetByConciliacaoBancaria(uint idConciliacaoBancaria)
        {
            string sql = "select * from fotos_conciliacao_bancaria where idConciliacaoBancaria=" + idConciliacaoBancaria;
            return objPersistence.LoadData(sql).ToArray();
        }
    }
}
