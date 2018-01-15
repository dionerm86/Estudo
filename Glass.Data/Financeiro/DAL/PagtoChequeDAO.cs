using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PagtoChequeDAO : BaseDAO<PagtoCheque, PagtoChequeDAO>
    {
        //private PagtoChequeDAO() { }

        /// <summary>
        /// Exclui todos os registro cujo idPagto for igual ao passado
        /// </summary>
        /// <param name="idPagto"></param>
        public void DeleteByIdPagto(uint idPagto)
        {
            objPersistence.ExecuteCommand("Delete From pagto_cheque where idPagto=" + idPagto);
        }

        public void DeleteByIdCheque(uint idCheque)
        {
            objPersistence.ExecuteCommand("Delete From pagto_cheque where idCheque=" + idCheque);
        }

        /// <summary>
        /// Retorna em qual pagamento o cheque foi utilizado
        /// </summary>
        /// <param name="idCheque"></param>
        /// <returns></returns>
        public uint GetPagtoByCheque(uint idCheque)
        {
            return GetPagtoByCheque(null, idCheque);
        }

        /// <summary>
        /// Retorna em qual pagamento o cheque foi utilizado
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCheque"></param>
        /// <returns></returns>
        public uint GetPagtoByCheque(GDASession session, uint idCheque)
        {
            string sql = "Select Count(*) from pagto_cheque Where idCheque=" + idCheque;

            if (objPersistence.ExecuteSqlQueryCount(session, sql, null) <= 0)
                return 0;

            sql = "Select idPagto From pagto_cheque Where idCheque=" + idCheque + " Order By idPagtoCheque desc limit 0,1";

            return Glass.Conversoes.StrParaUint(objPersistence.ExecuteScalar(session, sql).ToString());
        }
    }
}
