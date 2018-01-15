using GDA;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class CiotRodoviarioMDFeDAO : BaseDAO<CiotRodoviarioMDFe, CiotRodoviarioMDFeDAO>
    {
        public List<CiotRodoviarioMDFe> ObterCiotRodoviarioMDFe(int idRodoviario)
        {
            return objPersistence.LoadData(string.Format("SELECT * FROM ciot_rodoviario_mdfe WHERE IdRodoviario={0}", idRodoviario)).ToList();
        }

        public void DeletarPorIdRodoviario(GDASession sessao, int idRodoviario)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM ciot_rodoviario_mdfe WHERE IdRodoviario=" + idRodoviario, null);
        }
    }
}
