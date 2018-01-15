using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class PecaModeloBenefDAO : BaseDAO<PecaModeloBenef, PecaModeloBenefDAO>
    {
        //private PecaModeloBenefDAO() { }

        public IList<PecaModeloBenef> GetByPecaProjMod(uint idPecaProjMod)
        {
            string sql = "select * from peca_modelo_benef where idPecaProjMod=" + idPecaProjMod;
            return objPersistence.LoadData(sql).ToList();
        }

        public void DeleteByPecaProjMod(GDA.GDASession session, uint idPecaProjMod)
        {
            string sql = "delete from peca_modelo_benef where idPecaProjMod=" + idPecaProjMod;
            objPersistence.ExecuteCommand(session, sql);
        }
    }
}
