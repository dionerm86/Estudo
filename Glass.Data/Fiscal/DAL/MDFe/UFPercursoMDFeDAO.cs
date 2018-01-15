using Glass.Data.Model;
using System.Collections.Generic;
using GDA;

namespace Glass.Data.DAL
{
    public class UFPercursoMDFeDAO :BaseDAO<UFPercursoMDFe, UFPercursoMDFeDAO>
    {
        public List<UFPercursoMDFe> ObterUFPercursoMDFe(int idManifestoEletronico)
        {
            return objPersistence.LoadData(string.Format("SELECT * FROM uf_percurso_mdfe WHERE IdManifestoEletronico={0}", idManifestoEletronico)).ToList();
        }

        public void DeletarPorIdManifestoEletronico(GDASession sessao, int idManifestoEletronico)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM uf_percurso_mdfe WHERE IdManifestoEletronico=" + idManifestoEletronico, null);
        }
    }
}
