using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    public class PedagioRodoviarioMDFeDAO : BaseDAO<PedagioRodoviarioMDFe, PedagioRodoviarioMDFeDAO>
    {
        public List<PedagioRodoviarioMDFe> ObterPedagioRodoviarioMDFe(int idRodoviario)
        {
            var sql = @"SELECT pr.*, f.Razaosocial AS NomeFornecedor
                FROM pedagio_rodoviario_mdfe pr
                LEFT JOIN fornecedor f ON (pr.IdFornecedor=f.IdFornec)
                WHERE pr.IdRodoviario={0}";

            return objPersistence.LoadData(string.Format(sql, idRodoviario)).ToList();
        }

        public void DeletarPorIdRodoviario(GDASession sessao, int idRodoviario)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM pedagio_rodoviario_mdfe WHERE IdRodoviario=" + idRodoviario, null);
        }
    }
}
