using GDA;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class AverbacaoSeguroMDFeDAO : BaseDAO<AverbacaoSeguroMDFe, AverbacaoSeguroMDFeDAO>
    {
        public List<AverbacaoSeguroMDFe> ObterAverbacaoSeguroMDFe(GDASession sessao, int idManifestoEletronico)
        {
            return objPersistence.LoadData(sessao, string.Format("SELECT * FROM averbacao_seguro_mdfe WHERE IdManifestoEletronico={0}", idManifestoEletronico)).ToList();
        }

        public void DeletarPorIdManifestoEletronico(GDASession sessao, int idManifestoEletronico)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM averbacao_seguro_mdfe WHERE IdManifestoEletronico=" + idManifestoEletronico, null);
        }
    }
}
