using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA;

namespace Glass.Data.DAL
{
    public class CidadeCargaMDFeDAO : BaseDAO<CidadeCargaMDFe, CidadeCargaMDFeDAO>
    {
        public List<CidadeCargaMDFe> ObterCidadeCargaMDFe(int idManifestoEletronico)
        {
            var sql = @"SELECT cc.*, c.NomeCidade
                FROM cidade_carga_mdfe cc
                LEFT JOIN cidade c ON (cc.IdCidade=c.IdCidade)
                WHERE cc.IdManifestoEletronico={0}";

            return objPersistence.LoadData(string.Format(sql, idManifestoEletronico)).ToList();
        }

        public void DeletarPorIdManifestoEletronico(GDASession sessao, int idManifestoEletronico)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM cidade_carga_mdfe WHERE IdManifestoEletronico=" + idManifestoEletronico, null);
        }
    }
}
