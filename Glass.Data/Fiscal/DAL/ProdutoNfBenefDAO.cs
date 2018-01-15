using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ProdutoNfBenefDAO : BaseDAO<ProdutoNfBenef, ProdutoNfBenefDAO>
	{
        //private ProdutoNfBenefDAO() { }

        /// <summary>
        /// Busca beneficiamentos associados ao produto da nota fiscal.
        /// </summary>
        public ProdutoNfBenef[] ObterPeloProdutoNf(int idProdNf)
        {
            var descricaoBenef = "IF(bc.IdParent>0, CONCAT(bcpai.Descricao, ' ', bc.Descricao), bc.Descricao)";
            var tipoCalcBenef = "IF(bc.IdParent>0, bcpai.TipoCalculo, bc.TipoCalculo)";
            var sql =
                string.Format(@"SELECT pnb .*, {0} AS DescrBenef, {1} AS TipoCalcBenef FROM produto_nf_benef pnb
                        LEFT JOIN benef_config bc ON (pnb.IdBenefConfig=bc.IdBenefConfig) 
                        LEFT JOIN benef_config bcpai ON (bc.IdParent=bcpai.IdBenefConfig) 
                        LEFT JOIN benef_config bcavo ON (bcpai.IdParent=bcavo.IdBenefConfig) 
                    WHERE pnb.IdProdNf={2}", descricaoBenef, tipoCalcBenef, idProdNf);

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public bool PossuiLapidacao(GDASession sessao, uint idProdNf)
        {
            string sql = @"Select Count(*) from produto_nf_benef pnb
                Where (lapLarg>0 Or lapAlt>0) And pnb.idProdNf=" + idProdNf;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, sql).ToString()) > 0;
        }

        public bool PossuiBisote(GDASession sessao, uint idProdNf)
        {
            string sql = @"Select Count(*) from produto_nf_benef pnb
                Where (bisLarg>0 Or bisAlt>0) And pnb.idProdNf=" + idProdNf;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, sql).ToString()) > 0;
        }

        /// <summary>
        /// Exclui todos os beneficiamentos feitos no produto
        /// </summary>
        /// <param name="idProdNf"></param>
        public void DeleteByProdNf(GDASession sessao, uint idProdNf)
        {
            string sql = "Delete From produto_nf_benef Where idProdNf=" + idProdNf;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Importa os beneficiamentos do produto pedido
        /// </summary>
        /// <param name="idProdPed">De qual produto de pedido importar</param>
        /// <param name="idProdPedNf">Qual produto da nf será beneficiado</param>
        public void ImportaProdPedBenef(GDASession sessao, uint idProdPed, uint idProdNf)
        {
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, "Select Count(*) From produto_pedido_benef Where idProdPed=" + idProdPed).ToString()) <= 0)
                return;

            string sql = "Insert Into produto_nf_benef (IdBenefConfig, IdProdNf, Qtd, Valor, LapLarg, LapAlt, " +
                "BisLarg, BisAlt, EspBisote, EspFuro, Custo) (Select IdBenefConfig, " + idProdNf + " as IdProdNf, Qtd, Valor, " +
                "LapLarg, LapAlt, BisLarg, BisAlt, EspBisote, EspFuro, Custo From produto_pedido_benef Where idProdPed=" + idProdPed + ")";

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Importa os beneficiamentos do produto da compra
        /// </summary>
        public void ImportaProdCompraBenef(GDASession session, uint idProdCompra, uint idProdNf)
        {
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(session, "Select Count(*) From produtos_compra_benef Where idProdCompra=" + idProdCompra).ToString()) <= 0)
                return;

            string sql = "Insert Into produto_nf_benef (IdBenefConfig, IdProdNf, Qtd, Valor, LapLarg, LapAlt, " +
                "BisLarg, BisAlt, EspBisote, EspFuro, Custo) (Select IdBenefConfig, " + idProdNf + " as IdProdNf, Qtd, Valor, " +
                "LapLarg, LapAlt, BisLarg, BisAlt, EspBisote, EspFuro, Custo From produtos_compra_benef Where idProdCompra=" + idProdCompra + ")";

            objPersistence.ExecuteCommand(session, sql);
        }
	}
}