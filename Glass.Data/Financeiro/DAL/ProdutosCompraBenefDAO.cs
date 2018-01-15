using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutosCompraBenefDAO : BaseDAO<ProdutosCompraBenef, ProdutosCompraBenefDAO>
    {
        //private ProdutosCompraBenefDAO() { }

        private IList<ProdutosCompraBenef> GetBenefForDescr(uint idProdCompra)
        {
            string sql = "Select pcb.*, if(bc.idParent>0, Concat(bcpai.Descricao, ' ', bc.Descricao), bc.Descricao) as DescrBenef " +
                "From produtos_compra_benef pcb " +
                "Left Join benef_config bc On (ppb.idBenefConfig=bc.idBenefConfig) " +
                "Left Join benef_config bcpai On (bc.idParent=bcpai.idBenefConfig) " +
                "Left Join benef_config bcavo On (bcpai.idParent=bcavo.idBenefConfig) " +
                "Where pcb.idProdCompra=" + idProdCompra;

            return objPersistence.LoadData(sql).ToList();
        }

        public IList<ProdutosCompraBenef> GetByProdCompra(uint idProdCompra)
        {
            string sql = "select * from produtos_compra_benef where idProdCompra=" + idProdCompra;
            return objPersistence.LoadData(sql).ToList();
        }

        public void DeleteByProdCompra(uint idProdCompra)
        {
            DeleteByProdCompra(null, idProdCompra);
        }

        public void DeleteByProdCompra(GDASession session, uint idProdCompra)
        {
            string sql = "delete from produtos_compra_benef where idProdCompra=" + idProdCompra;
            objPersistence.ExecuteCommand(session, sql);
        }

        /// <summary>
        /// Busca string contendo beneficiamentos feitos no produtoPedido
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public string GetDescrBenef(uint idProdCompra)
        {
            string descrBenef = String.Empty;
            
            foreach (ProdutosCompraBenef benef in GetBenefForDescr(idProdCompra))
            {
                if (!Configuracoes.CompraConfig.ExibicaoDescrBenefCustomizada)
                {
                    descrBenef += (benef.Qtde > 0 ? benef.Qtde.ToString() + " " : "") + benef.DescrBenef +
                        Utils.MontaDescrLapBis(benef.BisAlt, benef.BisLarg, benef.LapAlt, benef.LapLarg, benef.EspBisote, null, null, true) + "; ";
                }
                else
                {
                    string tempBenef = benef.DescrBenef;
                    BenefConfig bc = BenefConfigDAO.Instance.GetElement(benef.IdBenefConfig);
                    if (bc.TipoControle == TipoControleBenef.Lapidacao || bc.TipoControle == TipoControleBenef.Bisote)
                    {
                        tempBenef = tempBenef.Substring(0, tempBenef.IndexOf("mm"));
                        tempBenef = tempBenef.Substring(0, tempBenef.LastIndexOf(" "));

                        if (benef.EspBisote > 0)
                            tempBenef += " " + benef.EspBisote + "mm";
                    }

                    if (tempBenef.ToLower().IndexOf("até") > -1)
                    {
                        tempBenef = tempBenef.Substring(0, tempBenef.ToLower().IndexOf("até"));
                        tempBenef = tempBenef.Substring(0, tempBenef.LastIndexOf(" ")) + "; ";
                    }

                    descrBenef += (benef.Qtde > 0 ? benef.Qtde.ToString() + " " : "") + tempBenef + "; ";
                }
            }

            return descrBenef;
        }

        #region Retorna os beneficiamentos para um produto de um pedido

        internal string SqlProdPedBenef(uint idPedido, uint idProdPed, uint idMaterItemProj, uint idBenefConfig, bool selecionar)
        {
            bool isVazio = idPedido == 0 && idProdPed == 0 && idMaterItemProj == 0;
            string campos = selecionar ? "pcb.*" : (isVazio ? "pc.idProdPed, pc.idMaterItemProj, " : "") + 
                "coalesce(count(*) * pc.qtde, 0) as contagem";

            string sql = "select " + campos + " from produtos_compra_benef pcb " +
                "left join produtos_compra pc on (pcb.idProdCompra=pc.idProdCompra) " +
                "left join compra c on (pc.idCompra=c.idCompra) " +
                "where c.situacao<>" + (int)Compra.SituacaoEnum.Cancelada;

            if (idPedido > 0)
                sql += " and c.idPedidoEspelho=" + idPedido;

            if (idProdPed > 0)
                sql += " and pc.idProdPed=" + idProdPed;

            if (idMaterItemProj > 0)
                sql += " and pc.idMaterItemProj=" + idMaterItemProj;
            
            if (idBenefConfig > 0)
                sql += " and pcb.idBenefConfig=" + idBenefConfig +
                    " group by pc.idProdCompra";
            else
                sql += " group by pc.idProdPed, pc.idMaterItemProj, pcb.idBenefConfig";

            return selecionar ? sql : "select " + (isVazio ? "idProdPed, idMaterItemProj, " : "") +
                "coalesce(sum(contagem), 0) as contagem from (" + sql + ") as count" + 
                (isVazio ? " group by idProdPed, idMaterItemProj" : "");
        }

        public int GetCountByPedido(uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlProdPedBenef(idPedido, 0, 0, 0, false));
        }

        public IList<ProdutosCompraBenef> GetByProdPedBenef(uint idProdPed, uint idBenefConfig)
        {
            return objPersistence.LoadData(SqlProdPedBenef(0, idProdPed, 0, idBenefConfig, true)).ToList();
        }

        public int GetCountByProdPedBenef(uint idProdPed, uint idBenefConfig)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlProdPedBenef(0, idProdPed, 0, idBenefConfig, false));
        }

        public IList<ProdutosCompraBenef> GetByMaterItemProjBenef(uint idMaterItemProj, uint idBenefConfig)
        {
            return objPersistence.LoadData(SqlProdPedBenef(0, 0, idMaterItemProj, idBenefConfig, true)).ToList();
        }

        public int GetCountByMaterItemProjBenef(uint idMaterItemProj, uint idBenefConfig)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlProdPedBenef(0, 0, idMaterItemProj, idBenefConfig, false));
        }

        #endregion
    }
}
