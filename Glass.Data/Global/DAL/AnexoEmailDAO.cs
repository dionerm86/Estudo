using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class AnexoEmailDAO : BaseDAO<AnexoEmail, AnexoEmailDAO>
    {
        //private AnexoEmailDAO() { }

        public AnexoEmail[] GetByEmail(uint idEmail)
        {
            string sql = "select * from anexo_email where idEmail=" + idEmail;
            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public void DeleteByEmail(uint idEmail)
        {
            objPersistence.ExecuteCommand("delete from anexo_email where idEmail=" + idEmail);
        }
    }
}
