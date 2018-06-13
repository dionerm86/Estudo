using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    public class ProdutoNfCustoRentabilidadeDAO : BaseDAO<ProdutoNfCustoRentabilidade, ProdutoNfCustoRentabilidadeDAO>
    {
        #region Métodos Públicos

        /// <summary>
        /// Recupera os registros de rentabilidade pelo custo do produto da nota fiscal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdNfCusto"></param>
        /// <returns></returns>
        public IList<ProdutoNfCustoRentabilidade> ObterPorProdutoNf(GDA.GDASession sessao, int idProdNfCusto)
        {
            return objPersistence.LoadData(sessao, "SELECT * FROM produto_nf_custo_rentabilidade WHERE IdProdNfCusto=?id",
                new GDA.GDAParameter("?id", idProdNfCusto))
                .Select(f =>
                {
                    f.ExistsInStorage = true;
                    return f;
                }).ToList();
        }

        /// <summary>
        /// Apaga os registros associados com custo do produto da nota fiscal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdNfCusto"></param>
        public void ApagarPorProdutoNfCusto(GDA.GDASession sessao, int idProdNfCusto)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM produto_nf_custo_rentabilidade WHERE IdProdNf=?id", new GDA.GDAParameter("?id", idProdNfCusto));
        }

        #endregion
    }
}
