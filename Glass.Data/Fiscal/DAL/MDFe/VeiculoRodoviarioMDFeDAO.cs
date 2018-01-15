using GDA;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class VeiculoRodoviarioMDFeDAO : BaseDAO<VeiculoRodoviarioMDFe, VeiculoRodoviarioMDFeDAO>
    {
        public List<VeiculoRodoviarioMDFe> ObterVeiculoRodoviarioMDFe(int idRodoviario)
        {
            return objPersistence.LoadData(string.Format("SELECT * FROM veiculo_rodoviario_mdfe WHERE IdRodoviario={0}", idRodoviario)).ToList();
        }

        public void DeletarPorIdRodoviario(GDASession sessao, int idRodoviario)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM veiculo_rodoviario_mdfe WHERE IdRodoviario=" + idRodoviario, null);
        }
    }
}
