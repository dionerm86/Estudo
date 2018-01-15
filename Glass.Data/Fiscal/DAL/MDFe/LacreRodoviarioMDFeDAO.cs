using GDA;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class LacreRodoviarioMDFeDAO : BaseDAO<LacreRodoviarioMDFe, LacreRodoviarioMDFeDAO>
    {
        public List<LacreRodoviarioMDFe> ObterLacreRodoviarioMDFe(int idRodoviario)
        {
            return objPersistence.LoadData(string.Format("SELECT * FROM lacre_rodoviario_mdfe WHERE IdRodoviario={0}", idRodoviario)).ToList();
        }

        public void DeletarPorIdRodoviario(GDASession sessao, int idRodoviario)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM lacre_rodoviario_mdfe WHERE IdRodoviario=" + idRodoviario, null);
        }
    }
}
