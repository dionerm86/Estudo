using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class PagtoContasReceberDAO : BaseDAO<PagtoContasReceber, PagtoContasReceberDAO>
    {
        public List<PagtoContasReceber> ObtemPagtos(uint idContaR)
        {
            return ObtemPagtos(null, idContaR);
        }

        public List<PagtoContasReceber> ObtemPagtos(GDASession sessao, uint idContaR)
        {
            return objPersistence.LoadData(sessao, @"
                SELECT pcr.*, fp.Descricao AS DescrFormaPagto
                FROM pagto_contas_receber pcr
                    INNER JOIN formapagto fp ON (pcr.IdFormaPagto = fp.IdFormaPagto)
                WHERE IdContaR = " + idContaR).ToList();
        }

        /// <summary>
        /// Obtem as formas de pagamento das contas recebidas informadas
        /// </summary>
        /// <param name="idsContasR"></param>
        /// <returns></returns>
        public List<PagtoContasReceber> ObtemPagtos(string idsContasR)
        {
            if (string.IsNullOrEmpty(idsContasR))
                return null;

            string sql = @"
            SELECT pcr.IdFormaPagto, fp.Descricao as DescrFormaPagto, SUM(pcr.ValorPagto) as ValorPagto
            FROM pagto_contas_receber pcr
                INNER JOIN formapagto fp ON (pcr.IdFormaPagto = fp.IdFormaPagto)
            WHERE pcr.IdContaR IN (" + idsContasR + @")
            GROUP BY pcr.IdFormaPagto";

            return objPersistence.LoadData(sql);
        }

        #region Remove o pagamento de uma conta recebida

        /// <summary>
        /// Remove o pagamento de uma conta recebida
        /// </summary>
        /// <param name="idContaR"></param>
        public void DeleteByIdContaR(uint idContaR)
        {
            DeleteByIdContaR(null, idContaR);
        }

        /// <summary>
        /// Remove o pagamento de uma conta recebida
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idContaR"></param>
        public void DeleteByIdContaR(GDASession session, uint idContaR)
        {
            objPersistence.ExecuteCommand(session, "DELETE FROM pagto_contas_receber WHERE IdContaR = " + idContaR);
        }

        /// <summary>
        /// Remove o pagamento das contas recebidas
        /// </summary>
        /// <param name="idsContaR"></param>
        public void DeleteByIdContaR(string idsContaR)
        {
            DeleteByIdContaR(null, idsContaR);
        }

        /// <summary>
        /// Remove o pagamento das contas recebidas
        /// </summary>
        /// <param name="idsContaR"></param>
        public void DeleteByIdContaR(GDASession session, string idsContaR)
        {
            if (!String.IsNullOrEmpty(idsContaR))
                objPersistence.ExecuteCommand(session, "DELETE FROM pagto_contas_receber WHERE IdContaR IN(" + idsContaR + ")");
        }

        #endregion

        #region Insere pagamentos

        public void ApagarInserirPagamentosAntigos(string sql)
        {
            objPersistence.ExecuteCommand(sql);
        }

        #endregion
    }
}
