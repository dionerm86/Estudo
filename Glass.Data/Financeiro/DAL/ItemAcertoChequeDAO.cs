using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ItemAcertoChequeDAO : BaseDAO<ItemAcertoCheque, ItemAcertoChequeDAO>
    {
        //private ItemAcertoChequeDAO() { }

        public void DeleteByAcertoCheque(uint idAcertoCheque)
        {
            objPersistence.ExecuteCommand("delete from item_acerto_cheque where idAcertoCheque=" + idAcertoCheque);
        }

        public IList<ItemAcertoCheque> GetByAcertoCheque(uint idAcertoCheque)
        {
            return GetByAcertoCheque(null, idAcertoCheque);
        }

        public IList<ItemAcertoCheque> GetByAcertoCheque(GDASession session, uint idAcertoCheque)
        {
            return objPersistence.LoadData(session, "select * from item_acerto_cheque where idAcertoCheque=" + idAcertoCheque).ToList();
        }

        /// <summary>
        /// Identifica em quais acertos de cheques o cheque passado foi quitado.
        /// </summary>
        public string GetIdsAcertoByCheque(GDASession session, uint idCheque, bool recebido)
        {
            object obj = objPersistence.ExecuteScalar(session, "Select Group_Concat(idAcertoCheque) From item_acerto_cheque Where idCheque=" + idCheque +
                (recebido ? " And valorReceb > 0" : ""));

            return obj == null || obj.ToString() == String.Empty ? "" : obj.ToString();
        }

        public void AtualizaValorRecebCheque(GDASession sessao, uint idAcertoCheque, uint idCheque, decimal valorReceb)
        {
            objPersistence.ExecuteCommand(sessao, "update item_acerto_cheque set valorReceb=?valor where idAcertoCheque=" + idAcertoCheque +
                " and idCheque=" + idCheque, new GDAParameter("?valor", valorReceb));
        }

        /// <summary>
        /// Método que obtem todos os valores recebidos de um cheque através do identificador do cheque.
        /// </summary>
        /// <param name="sessao">Sessão do GDA.</param>
        /// <param name="idCheque">Identificador do Cheque.</param>
        /// <returns>Retorna o a soma dos valores já recebidos em acertos (abertos) do cheque pesquisado.</returns>
        public decimal ObterValorRecebidoCheque(GDASession sessao, uint idCheque)
        {
            if (idCheque == 0)
            {
                return 0;
            }

            var sql = $@"
                SELECT SUM(ValorReceb)
                FROM acerto_cheque ac
                    INNER JOIN item_acerto_cheque iac ON (ac.IdAcertoCheque = iac.IdAcertoCheque)
                WHERE iac.IdCheque = {idCheque}
                    AND ac.Situacao = {(int)AcertoCheque.SituacaoEnum.Aberto}";

            return this.ExecuteScalar<decimal>(sessao, sql);
        }
    }
}