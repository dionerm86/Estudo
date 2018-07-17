using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoTrocadoBenefDAO : BaseDAO<ProdutoTrocadoBenef, ProdutoTrocadoBenefDAO>
    {
        //private ProdutoTrocadoBenefDAO() { }

        /// <summary>
        /// Busca beneficiamentos feitos no produto de troca/dev.
        /// </summary>
        public List<ProdutoTrocadoBenef> GetByProdTrocado(uint idProdTrocado)
        {
            return GetByProdTrocado(null, idProdTrocado);
        }

        /// <summary>
        /// Busca beneficiamentos feitos no produto de troca/dev.
        /// </summary>
        public List<ProdutoTrocadoBenef> GetByProdTrocado(GDASession session, uint idProdTrocado)
        {
            string descricaoBenef = "if(bc.idParent>0, Concat(bcpai.Descricao, ' ', bc.Descricao), bc.Descricao)";
            string tipoCalcBenef = "if(bc.idParent>0, bcpai.TipoCalculo, bc.TipoCalculo)";

            string sql = "Select ptb.*, " + descricaoBenef + " as DescrBenef, " + tipoCalcBenef + " as TipoCalcBenef From produto_trocado_benef ptb " +
                "Left Join benef_config bc On (ptb.idBenefConfig=bc.idBenefConfig) " +
                "Left Join benef_config bcpai On (bc.idParent=bcpai.idBenefConfig) " +
                "Left Join benef_config bcavo On (bcpai.idParent=bcavo.idBenefConfig) " +
                "Where ptb.idProdTrocado=" + idProdTrocado;

            return objPersistence.LoadData(session, sql);
        }

        public void DeleteByProdutoTrocado(uint idProdTrocado)
        {
            DeleteByProdutoTrocado(null, idProdTrocado);
        }

        public void DeleteByProdutoTrocado(GDASession session, uint idProdTrocado)
        {
            objPersistence.ExecuteCommand(session, "delete from produto_trocado_benef where idProdTrocado=" + idProdTrocado);
        }

        public decimal ObterCustoTotalPeloIdProdTrocado(GDASession session, int idProdTrocado)
        {
            return ObtemValorCampo<decimal>(session, "SUM(Custo)", $"IdProdTrocado={ idProdTrocado }");
        }
    }
}
