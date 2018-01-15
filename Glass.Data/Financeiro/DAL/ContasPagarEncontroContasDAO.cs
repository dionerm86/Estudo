using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ContasPagarEncontroContasDAO
        : BaseDAO<ContasPagarEncontroContas, ContasPagarEncontroContasDAO>
    {
        //private ContasPagarEncontroContasDAO() { }

        #region Métodos de retorno de itens

        private string Sql(uint idEncontroContas, bool selecionar)
        {
            string sql = @"SELECT " + (selecionar ? "*" : "COUNT(*)") + @"
                           FROM contas_pagar_encontro_contas crec
                           WHERE 1";

            if (idEncontroContas > 0)
                sql += " AND crec.idEncontroContas=" + idEncontroContas;

            return sql;
        }

        public IList<ContasPagarEncontroContas> GetList(uint idEncontroContas, string sortExpression, int startRow, int pageSize)
        {
            if (GetListCountReal(idEncontroContas) == 0)
                return new ContasPagarEncontroContas[] { new ContasPagarEncontroContas() };

            return LoadDataWithSortExpression(Sql(idEncontroContas, true), sortExpression, startRow, pageSize);
        }

        public int GetListCount(uint idEncontroContas)
        {
            int retorno = GetListCountReal(idEncontroContas);
            return retorno > 0 ? retorno : 1;
        }

        public int GetListCountReal(uint idEncontroContas)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idEncontroContas, false));
        }

        public ContasPagarEncontroContas[] GetListForRpt(uint idEncontroContas)
        {
            return objPersistence.LoadData(Sql(idEncontroContas, true)).ToArray();
        }

        public ContasPagarEncontroContas[] GetByIdEncontroContas(uint idEncontroContas)
        {
            if (idEncontroContas == 0)
                return null;

            return objPersistence.LoadData(Sql(idEncontroContas, true)).ToArray();
        }

        #endregion

        #region Vinculo contas a pagar com o encontro

        /// <summary>
        /// Verifica se a conta a pagar tem vinculo com o encontro
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <param name="idContaPG"></param>
        /// <returns></returns>
        public bool TemVinculo(uint idEncontroContas, uint idContaPG)
        {
            string sql = @"SELECT COUNT(*) 
                           FROM contas_pagar_encontro_contas crec
                           WHERE crec.idEncontroContas=" + idEncontroContas + " AND crec.idContaPG=" + idContaPG;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Cria o vinculo da conta a pagar com o encontro.
        /// </summary>
        /// <param name="idEncontroContas"></param>
        public void GeraVinculoContaPagar(GDASession sessao, uint idEncontroContas)
        {
            string sql = @"UPDATE contas_pagar set paga=true, dataPagto=?dtNow, valorPago=0
                           WHERE idContaPg IN (SELECT idContaPg 
                                               FROM contas_pagar_encontro_contas
                                               WHERE idEncontroContas=" + idEncontroContas + @")";

            objPersistence.ExecuteCommand(sessao, sql, new GDA.GDAParameter[] { new GDA.GDAParameter("?dtNow", DateTime.Now) });
        }

        /// <summary>
        /// Remove o vinculo da conta a pagar com o encontro.
        /// </summary>
        public void RemoveVinculoContaPagar(uint idEncontroContas)
        {
            RemoveVinculoContaPagar(null, idEncontroContas);
        }

        /// <summary>
        /// Remove o vinculo da conta a pagar com o encontro.
        /// </summary>
        public void RemoveVinculoContaPagar(GDASession session, uint idEncontroContas)
        {
            string sql = @"UPDATE contas_pagar set paga=false, dataPagto=null, valorPago=null
                           WHERE idContaPg IN (SELECT idContaPg 
                                               FROM contas_pagar_encontro_contas
                                               WHERE idEncontroContas=" + idEncontroContas + @")";

            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Verifica contas a pagar

        /// <summary>
        /// Retorna uma lista de ids se houver alguma conta a pagar
        /// associada ao encontro que já foi paga
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <returns></returns>
        public string ValidaContasPagar(uint idEncontroContas)
        {
            string sql = @"SELECT GROUP_CONCAT(cpec.idContaPg)
                           FROM contas_pagar_encontro_contas cpec
                           LEFT JOIN contas_pagar cr ON (cpec.idContaPg = cr.idContaPg)
                           WHERE cr.paga = true AND cpec.idEncontroContas = " + idEncontroContas;

            return ExecuteScalar<string>(sql);
        }

        /// <summary>
        /// Informa se o encontro de contas possui pelo menos uma conta a pagar.
        /// </summary>
        public bool ValidaExistenciaContasPagar(uint idEncontroContas)
        {
            var sql =
                string.Format(
                    @"SELECT COUNT(*)
                    FROM contas_pagar_encontro_contas cpec
                    WHERE cpec.IdEncontroContas = {0}", idEncontroContas);

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Apaga todos os vinculos de um encontro
        /// </summary>
        public void DeleteByIdEncontroContas(uint idEncontroContas)
        {
            DeleteByIdEncontroContas(null, idEncontroContas);
        }

        /// <summary>
        /// Apaga todos os vinculos de um encontro
        /// </summary>
        public void DeleteByIdEncontroContas(GDASession session, uint idEncontroContas)
        {
            string sql = @"DELETE FROM contas_pagar_encontro_contas where idEncontroContas=" + idEncontroContas;
            objPersistence.ExecuteCommand(session, sql);
        }

        /// <summary>
        /// Apaga todos os vinculos de uma conta
        /// </summary>
        public void DeleteByIdContaPg(uint idContaPg)
        {
            DeleteByIdContaPg(null, idContaPg);
        }

        /// <summary>
        /// Apaga todos os vinculos de uma conta
        /// </summary>
        public void DeleteByIdContaPg(GDASession session, uint idContaPg)
        {
            string sql = @"DELETE FROM contas_pagar_encontro_contas where idContaPg=" + idContaPg;
            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion
    }
}
