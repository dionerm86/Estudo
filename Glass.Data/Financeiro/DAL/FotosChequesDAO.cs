using System.Linq;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class FotosChequesDAO : BaseDAO<FotosCheques, FotosChequesDAO>
    {
        /// <summary>
        /// Retorna todas as fotos do cheque.
        /// </summary>
        public FotosCheques[] ObterPeloCheque(int idCheque)
        {
            var sql =
                string.Format(
                    "SELECT * FROM fotos_cheques WHERE IdCheque={0}", idCheque);
            return objPersistence.LoadData(sql).ToArray();
        }
    }
}
