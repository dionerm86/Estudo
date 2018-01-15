using GDA;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class FlagArqMesaPecaProjModDAO : BaseDAO<FlagArqMesaPecaProjMod, FlagArqMesaPecaProjModDAO>
    {
        public void DeleteByPecaProjMod(GDA.GDASession session, int idPecaProjMod)
        {
            string sql = "delete from flag_arq_mesa_peca_projeto_modelo where idPecaProjMod=" + idPecaProjMod;
            objPersistence.ExecuteCommand(session, sql);
        }

        public IList<FlagArqMesaPecaProjMod> ObtemPorPecaProjMod(int idPecaProjMod)
        {
            return ObtemPorPecaProjMod(null, idPecaProjMod);
        }

        public IList<FlagArqMesaPecaProjMod> ObtemPorPecaProjMod(GDASession session, int idPecaProjMod)
        {
            return objPersistence.LoadData(session, "SELECT * FROM flag_arq_mesa_peca_projeto_modelo where idPecaProjMod=" + idPecaProjMod).ToList();
        }
    }
}
