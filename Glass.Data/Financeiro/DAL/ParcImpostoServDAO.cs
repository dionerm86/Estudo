using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ParcImpostoServDAO : BaseDAO<ParcImpostoServ, ParcImpostoServDAO>
    {
        //private ParcImpostoServDAO() { }

        public ParcImpostoServ[] GetByImpostoServ(uint idImpostoServ)
        {
            return GetByImpostoServ(null, idImpostoServ);
        }

        public ParcImpostoServ[] GetByImpostoServ(GDA.GDASession session, uint idImpostoServ)
        {
            string sql = "select * from parc_imposto_serv where idImpostoServ=" + idImpostoServ + " order by data";
            return objPersistence.LoadData(session, sql).ToList().ToArray();
        }

        public void DeleteByImpostoServ(GDA.GDASession session, uint idImpostoServ)
        {
            objPersistence.ExecuteCommand(session, "delete from parc_imposto_serv where idImpostoServ=" + idImpostoServ);
        }
    }
}
