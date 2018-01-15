using System.Linq;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class FotosAcertoDAO : BaseDAO<FotosAcerto, FotosAcertoDAO>
    {
        /// <summary>
        /// Retorna todas as fotos do acerto.
        /// </summary>
        public FotosAcerto[] ObterPeloAcerto(int idAcerto)
        {
            var sql =
                string.Format(
                    "SELECT * FROM fotos_acerto WHERE IdAcerto={0}", idAcerto);
            return objPersistence.LoadData(sql).ToArray();
        }
    }
}
