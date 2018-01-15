using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class MaterialProjetoBenefDAO : BaseDAO<MaterialProjetoBenef, MaterialProjetoBenefDAO>
    {
        //private MaterialProjetoBenefDAO() { }

        /// <summary>
        /// Busca beneficiamentos feitos no material
        /// </summary>
        /// <param name="idMaterItemProj"></param>
        /// <returns></returns>
        public List<MaterialProjetoBenef> GetByMaterial(uint idMaterItemProj)
        {
            return GetByMaterial(null, idMaterItemProj);
        }

        /// <summary>
        /// Busca beneficiamentos feitos no material
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idMaterItemProj"></param>
        /// <returns></returns>
        public List<MaterialProjetoBenef> GetByMaterial(GDASession sessao, uint idMaterItemProj)
        {
            string descricaoBenef = "if(bc.idParent>0, Concat(bcpai.Descricao, ' ', bc.Descricao), bc.Descricao)";
            string tipoCalcBenef = "if(bc.idParent>0, bcpai.TipoCalculo, bc.TipoCalculo)";

            string sql = @"
                Select mpb.*, " + descricaoBenef + " as DescrBenef, " + tipoCalcBenef + @" as TipoCalcBenef 
                From material_projeto_benef mpb 
                    Left Join benef_config bc On (mpb.idBenefConfig=bc.idBenefConfig) 
                    Left Join benef_config bcpai On (bc.idParent=bcpai.idBenefConfig) 
                    Left Join benef_config bcavo On (bcpai.idParent=bcavo.idBenefConfig) 
                Where mpb.idMaterItemProj=" + idMaterItemProj;

            return objPersistence.LoadData(sessao, sql);
        }

        /// <summary>
        /// Exclui todos os beneficiamentos feitos no material
        /// </summary>
        /// <param name="idMaterItemProj"></param>
        public void DeleteByMaterial(GDASession sessao, uint idMaterItemProj)
        {
            string sql = "Delete From material_projeto_benef Where idMaterItemProj=" + idMaterItemProj;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Importa os beneficiamentos do produto pedido
        /// </summary>
        /// <param name="idProdPed">De qual produto de pedido importar</param>
        /// <param name="idMaterItemProj">Qual material será beneficiado</param>
        public void ImportaProdPedBenef(uint idProdPed, uint idMaterItemProj)
        {
            // Remove os beneficiamentos já existentes
            objPersistence.ExecuteCommand("delete from material_projeto_benef where idMaterItemProj=" + idMaterItemProj);

            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select Count(*) From produto_pedido_benef Where idProdPed=" + idProdPed).ToString()) <= 0)
                return;

            string sql = "Insert Into material_projeto_benef (IdBenefConfig, IdMaterItemProj, Qtd, Valor, LapLarg, LapAlt, " +
                "BisLarg, BisAlt, EspBisote, EspFuro, Custo) (Select IdBenefConfig, " + idMaterItemProj + " as IdProdPed, Qtd, Valor, " +
                "LapLarg, LapAlt, BisLarg, BisAlt, EspBisote, EspFuro, Custo From produto_pedido_benef Where idProdPed=" + idProdPed + ")";

            objPersistence.ExecuteCommand(sql);

            MaterialItemProjetoDAO.Instance.UpdateValorBenef(idMaterItemProj);
        }
    }
}
