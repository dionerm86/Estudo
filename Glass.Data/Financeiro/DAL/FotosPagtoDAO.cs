using System.Linq;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class FotosPagtoDAO : BaseDAO<FotosPagto, FotosPagtoDAO>
    {
        /// <summary>
        /// Retorna todas as fotos do pagamento.
        /// </summary>
        public FotosPagto[] GetByPagto(int idPagto)
        {
            var sql =
                string.Format(
                    "SELECT * FROM fotos_pagto WHERE IdPagto={0}", idPagto);
            return objPersistence.LoadData(sql).ToArray();
        }
    }
}
