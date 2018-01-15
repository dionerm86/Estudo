using System.Linq;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class FotosObraDAO : BaseDAO<FotosObra, FotosObraDAO>
    {
        /// <summary>
        /// Retorna todas as fotos da obra.
        /// </summary>
        public FotosObra[] ObterPelaObra(int idObra)
        {
            var sql =
                string.Format(
                    "SELECT * FROM fotos_obra WHERE idObra={0}", idObra);
            return objPersistence.LoadData(sql).ToArray();
        }
    }
}
