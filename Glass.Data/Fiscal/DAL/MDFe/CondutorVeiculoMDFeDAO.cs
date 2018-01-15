using GDA;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class CondutorVeiculoMDFeDAO : BaseDAO<CondutorVeiculoMDFe, CondutorVeiculoMDFeDAO>
    {
        public List<CondutorVeiculoMDFe> ObterCondutorVeiculoMDFe(int idRodoviario)
        {
            var sql = @"SELECT cv.*, f.Nome AS NomeCondutor
                FROM condutor_veiculo_mdfe cv
                LEFT JOIN funcionario f ON (cv.IdCondutor=f.IdFunc)
                WHERE cv.IdRodoviario={0}";

            return objPersistence.LoadData(string.Format(sql, idRodoviario)).ToList();
        }

        public void DeletarPorIdRodoviario(GDASession sessao, int idRodoviario)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM condutor_veiculo_mdfe WHERE IdRodoviario=" + idRodoviario, null);
        }
    }
}
