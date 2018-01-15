using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ComissaoContasReceberDAO : BaseDAO<ComissaoContasReceber, ComissaoContasReceberDAO>
    {
        #region Remove as contas associadas a uma comissão

        /// <summary>
        /// Remove as contas associadas a uma comissão
        /// </summary>
        /// <param name="idsContasR"></param>
        public void DeleteByContasRecebidas(GDASession sessao, string idsContasR)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM comissao_contas_receber WHERE IdContaR IN (" + idsContasR + ")");
        }

        /// <summary>
        /// Remove as contas associadas a uma comissão
        /// </summary>
        /// <param name="idsContasR"></param>
        public void DeleteByComissao(GDASession sessao, uint idComissao)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM comissao_contas_receber WHERE IdComissao = " + idComissao);
        }

        #endregion
    }
}
