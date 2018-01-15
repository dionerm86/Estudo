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
    }
}