using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class ProdutoPedidoEspelhoBenefDAO : BaseDAO<ProdutoPedidoEspelhoBenef, ProdutoPedidoEspelhoBenefDAO>
	{
        //private ProdutoPedidoEspelhoBenefDAO() { }

        private string Sql(uint idPedido, uint idProdPed, bool selecionar)
        {
            string campos = selecionar ? "ppb.*, if(bc.idParent>0, Concat(bcpai.Descricao, ' ', bc.Descricao), bc.Descricao) as DescrBenef, " +
                "bc.TipoCalculo as TipoCalculoBenef" : "count(*)";

            string sql = @"
                Select " + campos + @"
                From produto_pedido_espelho_benef ppb
                    Left Join benef_config bc On (ppb.idBenefConfig=bc.idBenefConfig) 
                    Left Join benef_config bcpai On (bc.idParent=bcpai.idBenefConfig) 
                    Left Join benef_config bcavo On (bcpai.idParent=bcavo.idBenefConfig) 
                Where 1";

            if (idPedido > 0)
                sql += " and ppb.idProdPed in (select idProdPed from produtos_pedido_espelho where idPedido=" + idPedido + ")";

            if (idProdPed > 0)
                sql += " and ppb.idProdPed=" + idProdPed;

            return sql;
        }

        /// <summary>
        /// Busca beneficiamentos feitos no produtoPedido
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public IList<ProdutoPedidoEspelhoBenef> GetByProdutoPedido(uint idProdPed)
        {
            return GetByProdutoPedido(null, idProdPed);
        }

        /// <summary>
        /// Busca beneficiamentos feitos no produtoPedido
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public IList<ProdutoPedidoEspelhoBenef> GetByProdutoPedido(GDASession session, uint idProdPed)
        {
            return objPersistence.LoadData(session, Sql(0, idProdPed, true)).ToList();
        }

        /// <summary>
        /// Verifica se o produto do pedido possui beneficiamento
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public bool PossuiBeneficiamento(uint idProdPed)
        {
            return PossuiBeneficiamento(null, idProdPed);
        }

        /// <summary>
        /// Verifica se o produto do pedido possui beneficiamento
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public bool PossuiBeneficiamento(GDASession session, uint idProdPed)
        {
            return ExecuteScalar<bool>(session, "Select Count(*) > 0 From produto_pedido_espelho_benef Where idProdPed=" + idProdPed);
        }

        /// <summary>
        /// Busca string contendo beneficiamentos feitos no produtoPedido
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public string GetDescrBenef(uint idProdPed)
        {
            return GetDescrBenef(null, idProdPed);
        }

        /// <summary>
        /// Busca string contendo beneficiamentos feitos no produtoPedido
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public string GetDescrBenef(GDASession session, uint idProdPed)
        {
            return GetDescrBenef(session, idProdPed, false);
        }

        /// <summary>
        /// Busca string contendo beneficiamentos feitos no produtoPedido
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        public string GetDescrBenef(uint idProdPed, bool etiqueta)
        {
            return GetDescrBenef((GDASession)null, idProdPed, etiqueta);
        }

        /// <summary>
        /// Busca string contendo beneficiamentos feitos no produtoPedido
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idProdPed"></param>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        public string GetDescrBenef(GDASession session, uint idProdPed, bool etiqueta)
        {
            return GetDescrBenef(session, (uint?)null, idProdPed, etiqueta);
        }

        /// <summary>
        /// Busca string contendo beneficiamentos feitos no produtoPedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        public string GetDescrBenef(uint? idPedido, uint idProdPed, bool etiqueta)
        {
            return GetDescrBenef(null, idPedido, idProdPed, etiqueta);
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
            string descrBenef = String.Empty;

            // Verifica se essa peça possui beneficiamentos
            if (!PossuiBeneficiamento(session, idProdPed))
                return String.Empty;

            List<ProdutoPedidoEspelhoBenef> beneficiamentos = objPersistence.LoadData(session, Sql(0, idProdPed, true));

            if (idPedido.GetValueOrDefault(0) == 0)
                idPedido = ProdutosPedidoEspelhoDAO.Instance.ObtemIdPedido(session, idProdPed);

            var isMaoDeObra = PedidoDAO.Instance.IsMaoDeObra(session, idPedido.Value);

            string complAltura = !etiqueta || isMaoDeObra ? "" :
                " (" + ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<float>(session, "altura", "idProdPed=" + idProdPed) + ")";

            string complLargura = !etiqueta || isMaoDeObra ? "" :
                " (" + ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<float>(session, "largura", "idProdPed=" + idProdPed) + ")";

            uint idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idProdPed=" + idProdPed);

            foreach (ProdutoPedidoEspelhoBenef benef in beneficiamentos)
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
                        (!String.IsNullOrEmpty(bcp.DescricaoBenef) ? bcp.DescricaoBenef : benef.DescrBenef) +
                        Utils.MontaDescrLapBis(benef.BisAlt, benef.BisLarg, benef.LapAlt, benef.LapLarg, benef.EspBisote, null, null, false);

                    descrBenef += !String.IsNullOrEmpty(descricao) ? descricao + "; " : "";
                }
                else
                {                  
                    string tempBenef = benef.DescrBenef;
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

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna a descrição dos beneficiamentos associados à esta peça por setor
        /// </summary>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public string GetDescrBySetor(uint idProdPed, int idSetor)
        {
            return GetDescrBySetor(null, idProdPed, idSetor);
        }

        /// <summary>
        /// Retorna a descrição dos beneficiamentos associados à esta peça por setor
        /// </summary>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public string GetDescrBySetor(GDASession sessao, uint idProdPed, int idSetor)
        {
            string sql = @"
                Select Cast(group_concat(bc.descricao separator ', ') as char) as descrBenef
                From produto_pedido_espelho_benef ppeb
                    Inner Join benef_config bc On (bc.idBenefConfig=ppeb.idBenefConfig)
                Where bc.idBenefConfig In (Select idBenefConfig From setor_benef Where idSetor=" + idSetor + @")
                    And idProdPed=" + idProdPed + @"
                Group By ppeb.idProdPed";

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            return obj != null && obj.ToString() != String.Empty ? "(" + obj.ToString() + ")" : String.Empty;
        }

        /// <summary>
        /// Exclui todos os beneficiamentos feitos no produto
        /// </summary>
        /// <param name="idProdPed"></param>
        public void DeleteByProdPed(GDASession sessao, uint idProdPed)
        {
            string sql = "Delete From produto_pedido_espelho_benef Where idProdPed=" + idProdPed;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Importa os beneficiamentos do produto pedido
        /// </summary>
        /// <param name="idProdPed">De qual produto de pedido importar</param>
        /// <param name="idProdPedEsp">Qual produto do pedido espelho será beneficiado</param>
        public void ImportaProdPedBenef(GDASession sessao, uint idProdPed, uint idProdPedEsp)
        {
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, "Select Count(*) From produto_pedido_benef Where idProdPed=" + idProdPed).ToString()) <= 0)
                return;

            string sql = @"Insert Into produto_pedido_espelho_benef (IdBenefConfig, IdProdPed, Qtd, Valor, LapLarg, LapAlt, 
                BisLarg, BisAlt, EspBisote, EspFuro, Custo, valorComissao, valorAcrescimo, valorAcrescimoProd, valorDesconto, valorDescontoProd, ValorUnit)
                Select IdBenefConfig, {0} as IdProdPed, Qtd, Valor, LapLarg, LapAlt, BisLarg, BisAlt, EspBisote, EspFuro, Custo, valorComissao, 
                valorAcrescimo, valorAcrescimoProd, valorDesconto, valorDescontoProd, ValorUnit From produto_pedido_benef Where idProdPed={1}";

            objPersistence.ExecuteCommand(sessao, String.Format(sql, idProdPedEsp, idProdPed));

            ProdutosPedidoEspelhoDAO.Instance.UpdateValorBenef(sessao, idProdPedEsp);
        }

        /// <summary>
        /// Importa os beneficiamentos do materialItemProjeto
        /// </summary>
        /// <param name="idMaterItemProj">De qual materialItemProjeto importar</param>
        /// <param name="idProdPedEsp">Qual produto do pedido espelho será beneficiado</param>
        public void ImportaMaterialProjBenef(uint idMaterItemProj, uint idProdPedEsp)
        {
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select Count(*) From material_projeto_benef Where idMaterItemProj=" + idMaterItemProj).ToString()) <= 0)
                return;

            string sql = @"Insert Into produto_pedido_espelho_benef (IdBenefConfig, IdProdPed, Qtd, Valor, LapLarg, LapAlt, 
                BisLarg, BisAlt, EspBisote, EspFuro) Select IdBenefConfig, {0} as IdProdPed, Qtd, Valor, 
                LapLarg, LapAlt, BisLarg, BisAlt, EspBisote, EspFuro From material_projeto_benef Where idMaterItemProj={1}";

            objPersistence.ExecuteCommand(String.Format(sql, idProdPedEsp, idMaterItemProj));

            ProdutosPedidoEspelhoDAO.Instance.UpdateValorBenef(idProdPedEsp);
        }

        /// <summary>
        /// Retorna os beneficiamentos do produto.
        /// Usado na tela de compra PCP.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public IList<ProdutoPedidoEspelhoBenef> GetForCompraPcp(uint idProdPed)
        {
            if (idProdPed == 0)
                return null;

            return objPersistence.LoadData(Sql(0, idProdPed, true)).ToList();
        }

        public int GetCountForCompraPcp(uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idPedido, 0, false));
        }

        /// <summary>
        /// Retorna o total bruto dos beneficiamentos de um ambiente de pedido.
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public decimal GetValorBrutoForAmbiente(uint idAmbientePedido)
        {
            string where = "idProdPed in (select * from (select idProdPed from produtos_pedido_espelho " +
                "where idAmbientePedido=" + idAmbientePedido + ") as temp)";

            return ObtemValorCampo<decimal>(@"sum(valor-valorComissao-valorAcrescimo-valorAcrescimoProd+
                valorDesconto+valorDescontoProd)", where);
        }

        public bool PossuiSerigrafia(GDASession session, uint idProdPed)
        {
            var sql = @"
                SELECT COUNT(*)
                    FROM produtos_pedido_espelho ppe
	                    INNER JOIN produto_pedido_espelho_benef ppeb ON (ppe.IdProdPed = ppeb.IdProdPed)
	                    INNER JOIN benef_config bc ON (ppeb.IdBenefConfig = bc.IdBenefConfig)
    	                LEFT JOIN benef_config bc1 ON (bc1.IdBenefConfig = bc.IdParent)
                    WHERE (bc.Descricao LIKE '%Serigrafia%' OR bc1.Descricao LIKE '%Serigrafia%')
                        AND ppe.IdProdPed = " + idProdPed;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        public bool PossuiPintura(GDASession session, uint idProdPed)
        {
            var sql = @"
                SELECT COUNT(*)
                    FROM produtos_pedido_espelho ppe
	                    INNER JOIN produto_pedido_espelho_benef ppeb ON (ppe.IdProdPed = ppeb.IdProdPed)
	                    INNER JOIN benef_config bc ON (ppeb.IdBenefConfig = bc.IdBenefConfig)
    	                LEFT JOIN benef_config bc1 ON (bc1.IdBenefConfig = bc.IdParent)
                    WHERE (bc.Descricao LIKE '%pintura%' OR bc1.Descricao LIKE '%pintura%')
                        AND ppe.IdProdPed = " + idProdPed;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        public bool PossuiBeneficiamento(GDASession session, uint idProdPed, int idBenefConfig)
        {
            var sql = @"
                SELECT count(*)
                FROM produto_pedido_espelho_benef ppeb
	                INNER JOIN benef_config bc ON (ppeb.IdBenefConfig = bc.IdBenefConfig)
                WHERE idprodped = " + idProdPed + " AND(bc.IdBenefConfig = " + idBenefConfig + " OR bc.IdParent = " + idBenefConfig + ")";

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }
	}
}