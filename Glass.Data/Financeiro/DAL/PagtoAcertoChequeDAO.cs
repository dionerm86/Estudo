using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class PagtoAcertoChequeDAO : BaseDAO<PagtoAcertoCheque, PagtoAcertoChequeDAO>
    {
        //private PagtoAcertoChequeDAO() { }

        public void DeleteByAcertoCheque(uint idAcertoCheque)
        {
            objPersistence.ExecuteCommand("delete from pagto_acerto_cheque where idAcertoCheque=" + idAcertoCheque);
        }

        public PagtoAcertoCheque[] GetByAcertoCheque(uint idAcertoCheque)
        {
            return objPersistence.LoadData("select * from pagto_acerto_cheque where idAcertoCheque=" + idAcertoCheque).ToList().ToArray();
        }

        public void AtualizarNumAutCartao(GDASession sessao, int idAcerto, int numFormaPagto, string numAut)
        {
            var sql = @"UPDATE pagto_acerto_cheque SET NumAutCartao = ?numAut WHERE idAcertoCheque = ?idAcertoCheque AND NumFormaPagto = ?numFormaPagto";

            objPersistence.ExecuteCommand(sessao, sql,
                new GDAParameter("?numAut", numAut),
                new GDAParameter("?idAcertoCheque", idAcerto),
                new GDAParameter("?numFormaPagto", numFormaPagto));
        }
    }
}
