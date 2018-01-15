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
    }
}
