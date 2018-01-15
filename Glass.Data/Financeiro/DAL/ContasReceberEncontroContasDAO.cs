using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ContasReceberEncontroContasDAO : BaseDAO<ContasReceberEncontroContas, ContasReceberEncontroContasDAO>
    {
        //private ContasReceberEncontroContasDAO() { }

        #region Métodos de retorno de itens

        private string Sql(uint idEncontroContas, bool selecionar)
        {
            //            string campos = selecionar ? @"crec.idEncontroContas, crec.IdContaR, cr.valorVec as ValorVenc, cr.DataVec as DataVenc,
            //                                           cr.NumParc, cr.NumParcMax" : "COUNT(*)";
            //            string sql = @"SELECT " + campos + @"
            //                           FROM contas_receber_encontro_contas crec
            //                           LEFT JOIN contas_receber cr ON (crec.IdContaR = cr.IdContaR)
            //                           WHERE 1";

            string sql = @"SELECT " + (selecionar ? "*" : "COUNT(*)") + @"
                           FROM contas_receber_encontro_contas crec
                           WHERE 1";

            if (idEncontroContas > 0)
                sql += " AND crec.idEncontroContas=" + idEncontroContas;

            return sql;
        }

        public IList<ContasReceberEncontroContas> GetList(uint idEncontroContas, string sortExpression, int startRow, int pageSize)
        {
            if (GetListCountReal(idEncontroContas) == 0)
                return new ContasReceberEncontroContas[] { new ContasReceberEncontroContas() };

            return LoadDataWithSortExpression(Sql(idEncontroContas, true), sortExpression, startRow, pageSize).ToList();
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

        public IList<ContasReceberEncontroContas> GetListForRpt(uint idEncontroContas)
        {
            return objPersistence.LoadData(Sql(idEncontroContas, true)).ToList();
        }

        public IList<ContasReceberEncontroContas> GetByIdEncontroContas(uint idEncontroContas)
        {
            if (idEncontroContas == 0)
                return null;

            return objPersistence.LoadData(Sql(idEncontroContas, true)).ToList();
        }

        #endregion

        #region Vinculo contas receber com o encontro

        /// <summary>
        /// Verifica se a conta a receber tem vinculo com o encontro
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <param name="idContaR"></param>
        /// <returns></returns>
        public bool TemVinculo(uint idEncontroContas, uint idContaR)
        {
            string sql = @"SELECT COUNT(*) 
                           FROM contas_receber_encontro_contas crec
                           WHERE crec.idEncontroContas=" + idEncontroContas + " AND crec.idContaR=" + idContaR;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Cria o vinculo da conta a receber com o encontro.
        /// </summary>
        /// <param name="idEncontroContas"></param>
        public void GeraVinculoContaReceber(GDASession sessao, uint idEncontroContas)
        {
            string sql = @"UPDATE contas_receber set recebida=true, dataRec=?dtNow, valorRec=0
                           WHERE idContaR IN (SELECT idContaR 
                                               FROM contas_receber_encontro_contas
                                               WHERE idEncontroContas=" + idEncontroContas + @")";

            objPersistence.ExecuteCommand(sessao, sql, new GDA.GDAParameter[] { new GDA.GDAParameter("?dtNow", DateTime.Now) });
        }

        /// <summary>
        /// Remove o vinculo da conta a receber com o encontro.
        /// </summary>
        public void RemoveVinculoContaReceber(uint idEncontroContas)
        {
            RemoveVinculoContaReceber(null, idEncontroContas);
        }

        /// <summary>
        /// Remove o vinculo da conta a receber com o encontro.
        /// </summary>
        public void RemoveVinculoContaReceber(GDASession session, uint idEncontroContas)
        {
            string sql = @"UPDATE contas_receber set recebida=false, dataRec=null, valorRec=null
                           WHERE idContaR IN (SELECT idContaR 
                                               FROM contas_receber_encontro_contas
                                               WHERE idEncontroContas=" + idEncontroContas + @")";

            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Verifica contas a receber

        /// <summary>
        /// Retorna uma lista de ids se houver alguma conta a receber
        /// associada ao encontro que já foi recebida
        /// </summary>
        public string ValidaContasReceber(GDASession session, uint idEncontroContas)
        {
            string sql = @"SELECT GROUP_CONCAT(crec.idContaR)
                           FROM contas_receber_encontro_contas crec
                           LEFT JOIN contas_receber cr ON (crec.idContaR = cr.idContaR)
                           WHERE cr.recebida = true AND crec.idEncontroContas = " + idEncontroContas;

            return ExecuteScalar<string>(session, sql);
        }

        /// <summary>
        /// Informa se o encontro de contas possui pelo menos uma conta a receber.
        /// </summary>
        public bool ValidaExistenciaContasReceber(uint idEncontroContas)
        {
            var sql =
                string.Format(
                    @"SELECT COUNT(*)
                    FROM contas_receber_encontro_contas crec
                    WHERE crec.IdEncontroContas = {0}", idEncontroContas);

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
            string sql = @"DELETE FROM contas_receber_encontro_contas where idEncontroContas=" + idEncontroContas;
            objPersistence.ExecuteCommand(session, sql);
        }

        /// <summary>
        /// Apaga todos os vinculos de uma conta
        /// </summary>
        public void DeleteByIdContaR(uint idContaR)
        {
            DeleteByIdContaR(null, idContaR);
        }

        /// <summary>
        /// Apaga todos os vinculos de uma conta
        /// </summary>
        public void DeleteByIdContaR(GDASession session, uint idContaR)
        {
            string sql = @"DELETE FROM contas_receber_encontro_contas where idContaR=" + idContaR;
            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion
    }
}
