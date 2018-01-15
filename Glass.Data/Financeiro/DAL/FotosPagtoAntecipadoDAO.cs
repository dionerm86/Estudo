using System.Linq;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class FotosPagtoAntecipadoDAO : BaseDAO<FotosPagtoAntecipado, FotosPagtoAntecipadoDAO>
    {
        /// <summary>
        /// Retorna todas as fotos do pagamento antecipado.
        /// </summary>
        public FotosPagtoAntecipado[] ObterPeloPagtoAntecipado(int idPagtoAntecipado)
        {
            var sql =
                string.Format(
                    "SELECT * FROM fotos_pagto_antecipado WHERE IdPagtoAntecipado={0}", idPagtoAntecipado);
            return objPersistence.LoadData(sql).ToArray();
        }
    }
}
