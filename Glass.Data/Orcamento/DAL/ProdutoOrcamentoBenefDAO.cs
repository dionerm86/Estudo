using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoOrcamentoBenefDAO : BaseDAO<ProdutoOrcamentoBenef, ProdutoOrcamentoBenefDAO>
    {
        //private ProdutoOrcamentoBenefDAO() { }

        /// <summary>
        /// Busca beneficiamentos feitos no produtoOrcamento
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public ProdutoOrcamentoBenef[] GetByProdutoOrcamento(uint idProd)
        {
            string descricaoBenef = "if(bc.idParent>0, Concat(bcpai.Descricao, ' ', bc.Descricao), bc.Descricao)";
            string sql = "Select pob.*, " + descricaoBenef + @" as DescrBenef From produto_orcamento_benef pob 
                Left Join benef_config bc On (pob.idBenefConfig=bc.idBenefConfig) 
                Left Join benef_config bcpai On (bc.idParent=bcpai.idBenefConfig) 
                Left Join benef_config bcavo On (bcpai.idParent=bcavo.idBenefConfig) 
                Where pob.idProd=" + idProd;

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        /// <summary>
        /// Exclui todos os beneficiamentos feitos no produto
        /// </summary>
        /// <param name="idProd"></param>
        public void DeleteByProdOrca(uint idProd)
        {
            DeleteByProdOrca(null, idProd);
        }

        /// <summary>
        /// Exclui todos os beneficiamentos feitos no produto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProd"></param>
        public void DeleteByProdOrca(GDASession sessao, uint idProd)
        {
            string sql = "Delete From produto_orcamento_benef Where idProd=" + idProd;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Remove um percentual de comissão dos beneficiamentos do orçamento.
        /// </summary>
        public void RemovePercComissaoBenef(GDASession session, uint idOrcamento, float percComissao)
        {
            if (percComissao == 0)
                return;

            objPersistence.ExecuteCommand(session, @"update produto_orcamento_benef set valor=valor-coalesce(valorComissao,0), valorComissao=0 where idProd in (
                select * from (select idProd from produtos_orcamento where idOrcamento=" + idOrcamento + ") as temp)", 
                new GDA.GDAParameter("?pc", (100 - percComissao) / 100));
        }
	}
}
