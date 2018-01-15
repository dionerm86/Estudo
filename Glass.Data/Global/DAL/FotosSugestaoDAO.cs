using System.Linq;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class FotosSugestaoDAO : BaseDAO<FotosSugestao, FotosSugestaoDAO>
    {
        /// <summary>
        /// Retorna todas as fotos da sugestão.
        /// </summary>
        public FotosSugestao[] ObterPelaSugestao(int idSugestao)
        {
            var sql = string.Format("SELECT * FROM fotos_sugestao WHERE IdSugestao={0}", idSugestao);
            return objPersistence.LoadData(sql).ToArray();
        }
    }
}
