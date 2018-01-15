using GDA;
using Glass.Data.Model.Cte;

namespace Glass.Data.DAL.CTe
{
    public sealed class EfdCteDAO : BaseDAO<EfdCte, EfdCteDAO>
    {
        //private EfdCteDAO() { }

        private string Sql(uint idCte)
        {
            return @"select e.*, pcc.codInterno as codInternoContaContabil
                from efd_cte e
                    left join plano_conta_contabil pcc on (e.idContaContabil=pcc.idContaContabil)
                where e.idCte=" + idCte;
        }

        public EfdCte GetElement(uint idCte)
        {
            return objPersistence.LoadOneData(Sql(idCte));
        }

        public void Delete(GDASession sessao, uint idCte)
        {
            objPersistence.ExecuteCommand(sessao, "delete from efd_cte where idCte=" + idCte);
        }

        public void Delete(uint idCte)
        {
            Delete(null, idCte);
        }
    }
}
