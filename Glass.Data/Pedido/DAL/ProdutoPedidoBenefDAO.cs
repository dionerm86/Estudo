using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class ProdutoPedidoBenefDAO : BaseDAO<ProdutoPedidoBenef, ProdutoPedidoBenefDAO>
	{
        //private ProdutoPedidoBenefDAO() { }

        private string Sql(uint idPedido, uint idProdPed, bool selecionar)
        {
            string campos = selecionar ? "ppb.*, if(bc.idParent>0, Concat(bcpai.Descricao, ' ', bc.Descricao), bc.Descricao) as DescrBenef, " +
                "bc.TipoCalculo as TipoCalculoBenef" : "count(*)";

            string sql = @"
                Select " + campos + @"
                From produto_pedido_benef ppb
                    Left Join benef_config bc On (ppb.idBenefConfig=bc.idBenefConfig) 
                    Left Join benef_config bcpai On (bc.idParent=bcpai.idBenefConfig) 
                    Left Join benef_config bcavo On (bcpai.idParent=bcavo.idBenefConfig) 
                Where 1";

            if (idPedido > 0)
                sql += " and ppb.idProdPed in (select idProdPed from produtos_pedido where idPedido=" + idPedido + ")";

            if (idProdPed > 0)
                sql += " and ppb.idProdPed=" + idProdPed;

            return sql;
        }

        /// <summary>
        /// Busca beneficiamentos feitos no produtoPedido
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public ProdutoPedidoBenef[] GetByProdutoPedido(uint idProdPed)
        {
            string descricaoBenef = "if(bc.idParent>0, Concat(bcpai.Descricao, ' ', bc.Descricao), bc.Descricao)";
            string tipoCalcBenef = "if(bc.idParent>0, bcpai.TipoCalculo, bc.TipoCalculo)";
            string sql = "Select ppb.*, " + descricaoBenef + " as DescrBenef, " + tipoCalcBenef + @" as TipoCalcBenef From produto_pedido_benef ppb 
                Left Join benef_config bc On (ppb.idBenefConfig=bc.idBenefConfig) 
                Left Join benef_config bcpai On (bc.idParent=bcpai.idBenefConfig) 
                Left Join benef_config bcavo On (bcpai.idParent=bcavo.idBenefConfig) 
                Where ppb.idProdPed=" + idProdPed;

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        /// <summary>
        /// Exclui todos os beneficiamentos feitos no produto
        /// </summary>
        /// <param name="idProdPed"></param>
        public void DeleteByProdPed(uint idProdPed)
        {
            DeleteByProdPed(null, idProdPed);
        }

        /// <summary>
        /// Exclui todos os beneficiamentos feitos no produto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPed"></param>
        public void DeleteByProdPed(GDASession sessao, uint idProdPed)
        {
            string sql = "Delete From produto_pedido_benef Where idProdPed=" + idProdPed;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Verifica se o produto do pedido possui beneficiamento
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public bool PossuiBeneficiamento(GDASession sessao, uint idProdPed)
        {
            string sql = "Select Count(*) From produto_pedido_benef Where idProdPed=" + idProdPed;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, null) > 0;
        }

        /// <summary>
        /// Busca string contendo beneficiamentos feitos no produtoPedido
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        public string GetDescrBenef(GDASession session, uint? idPedido, uint idProdPed, bool etiqueta)
        {
            var descrBenef = string.Empty;

            // Verifica se essa peça possui beneficiamentos
            if (!PossuiBeneficiamento(session, idProdPed))
                return string.Empty;

            var beneficiamentos = objPersistence.LoadData(session, Sql(0, idProdPed, true)).ToList();

            if (idPedido.GetValueOrDefault(0) == 0)
                idPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(session, idProdPed);

            var isMaoDeObra = PedidoDAO.Instance.IsMaoDeObra(session, idPedido.Value);

            var complAltura = !etiqueta || isMaoDeObra ? "" : " (" + ProdutosPedidoDAO.Instance.ObtemValorCampo<float>(session, "altura", "idProdPed=" + idProdPed) + ")";

            var complLargura = !etiqueta || isMaoDeObra ? "" : " (" + ProdutosPedidoDAO.Instance.ObtemValorCampo<float>(session, "largura", "idProdPed=" + idProdPed) + ")";

            var idProd = ProdutosPedidoDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idProdPed=" + idProdPed);

            foreach (ProdutoPedidoBenef benef in beneficiamentos)
            {
                //Se no cadastro do beneficiamento estiver marcado para não exibir a descrição na etiqueta,
                //pula para o proximo
                if (BenefConfigDAO.Instance.NaoExibirDescrImpEtiqueta(session, benef.IdBenefConfig))
                    continue;

                if (benef.IdBenefConfig == 0)
                    continue;

                if (!Configuracoes.CompraConfig.ExibicaoDescrBenefCustomizada)
                {
                    BenefConfigPreco bcp = BenefConfigPrecoDAO.Instance.GetByIdBenefConfig(session, benef.IdBenefConfig, idProd);

                    var descricao = (benef.Qtd > 0 ? benef.Qtd.ToString() + " " : "") +
                        (!string.IsNullOrEmpty(bcp.DescricaoBenef) ? bcp.DescricaoBenef : benef.DescrBenef) +
                        Utils.MontaDescrLapBis(benef.BisAlt, benef.BisLarg, benef.LapAlt, benef.LapLarg, benef.EspBisote, null, null, false);

                    descrBenef += !string.IsNullOrEmpty(descricao) ? descricao + "; " : "";
                }
                else
                {
                    var tempBenef = benef.DescrBenef;
                    if (tempBenef.ToLower().IndexOf("até") > -1)
                    {
                        tempBenef = tempBenef.Substring(0, tempBenef.ToLower().IndexOf("até"));
                        tempBenef = tempBenef.Substring(0, tempBenef.LastIndexOf(" "));
                    }

                    tempBenef += Utils.MontaDescrLapBis(benef.BisAlt, benef.BisLarg, benef.LapAlt, benef.LapLarg, benef.EspBisote, complAltura, complLargura, false);

                    descrBenef += (benef.Qtd > 0 ? benef.Qtd.ToString() + " " : "") + tempBenef.Trim() + "; ";
                }
            }

            return descrBenef;
        }

        public decimal ObterCustoTotalPeloIdProdPed(GDASession session, int idProdPed)
        {
            return ObtemValorCampo<decimal>(session, "SUM(Custo)", $"IdProdPed={ idProdPed }");
        }
    }
}