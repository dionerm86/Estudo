using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    /// <summary>
    /// Representa o objeto de acesso a dados para a rentabilidade do produto da nota fiscal.
    /// </summary>
    public class ProdutoNfRentabilidadeDAO : BaseDAO<ProdutoNfRentabilidade, ProdutoNfRentabilidadeDAO>
    {
        #region Métodos Públicos

        /// <summary>
        /// Recupera os registros de rentabilidade pelo produtos da nota fiscal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdNf"></param>
        /// <returns></returns>
        public IList<ProdutoNfRentabilidade> ObterPorProdutoNf(GDA.GDASession sessao, uint idProdNf)
        {
            return objPersistence.LoadData(sessao, "SELECT * FROM produto_nf_rentabilidade WHERE IdProdNf=?id",
                new GDA.GDAParameter("?id", idProdNf))
                .Select(f =>
                {
                    f.ExistsInStorage = true;
                    return f;
                }).ToList();
        }

        /// <summary>
        /// Apaga os registros associados com o produto da nota fiscal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdNf"></param>
        public void ApagarPorProdutoPedido(GDA.GDASession sessao, uint idProdNf)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM produto_nf_rentabilidade WHERE IdProdNf=?id", new GDA.GDAParameter("?id", idProdNf));
        }

        #endregion
    }
}
