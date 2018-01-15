using GDA;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class PagtoNotaFiscalDAO : BaseDAO<PagtoNotaFiscal, PagtoNotaFiscalDAO>
    {
        #region Métodos Publicos

        /// <summary>
        /// Recupera os pagamentos de uma nota fiscal
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public List<PagtoNotaFiscal> ObtemPagamentos(int idNf)
        {
            return objPersistence.LoadData("SELECT * FROM pagto_nota_fiscal WHERE idNf = " + idNf);
        }

        /// <summary>
        /// Remove os pagamentos de uma nota fiscal
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idNf"></param>
        public void RemovePagamentos(GDASession sessao, int idNf)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM pagto_nota_fiscal WHERE idNf = " + idNf);
        }

        #endregion
    }
}
