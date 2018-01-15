using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoTrocaDevolucaoBenefDAO : BaseDAO<ProdutoTrocaDevolucaoBenef, ProdutoTrocaDevolucaoBenefDAO>
    {
        //private ProdutoTrocaDevolucaoBenefDAO() { }

        /// <summary>
        /// Busca beneficiamentos feitos no produto de troca/dev.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public List<ProdutoTrocaDevolucaoBenef> GetByProdTrocaDev(uint idProdTrocaDev)
        {
            string descricaoBenef = "if(bc.idParent>0, Concat(bcpai.Descricao, ' ', bc.Descricao), bc.Descricao)";
            string tipoCalcBenef = "if(bc.idParent>0, bcpai.TipoCalculo, bc.TipoCalculo)";

            string sql = "Select ptdb.*, " + descricaoBenef + " as DescrBenef, " + tipoCalcBenef + " as TipoCalcBenef From produto_troca_dev_benef ptdb " +
                "Left Join benef_config bc On (ptdb.idBenefConfig=bc.idBenefConfig) " +
                "Left Join benef_config bcpai On (bc.idParent=bcpai.idBenefConfig) " +
                "Left Join benef_config bcavo On (bcpai.idParent=bcavo.idBenefConfig) " +
                "Where ptdb.idProdTrocaDev=" + idProdTrocaDev;

            return objPersistence.LoadData(sql);
        }

        public void DeleteByProdutoTrocaDev(uint idProdTrocaDev)
        {
            DeleteByProdutoTrocaDev(null, idProdTrocaDev);
        }

        public void DeleteByProdutoTrocaDev(GDASession session, uint idProdTrocaDev)
        {
            objPersistence.ExecuteCommand(session, "delete from produto_troca_dev_benef where idProdTrocaDev=" + idProdTrocaDev);
        }
    }
}
