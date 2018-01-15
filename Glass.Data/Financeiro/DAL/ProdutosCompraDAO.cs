using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ProdutosCompraDAO : BaseDAO<ProdutosCompra, ProdutosCompraDAO>
	{
        //private ProdutosCompraDAO() { }

        #region Busca produtos para listagem padrão

        private string Sql(uint idCompra, string idsProdCompra, bool selecionar)
        {
            string campos = selecionar ? 
                "pc.*, p.Descricao as DescrProduto, p.IdGrupoProd, p.idSubgrupoProd, p.CodInterno,  p.LocalArmazenagem as LocalArmazenagem, p.ValorFiscal" : 
                "Count(*)";

            string sql = "Select " + campos + @" From produtos_compra pc 
                Left Join produto p On (pc.idProd=p.idProd) 
                Where 1";
            
            if (idCompra > 0)
                sql += " and pc.idCompra=" + idCompra;

            if (!String.IsNullOrEmpty(idsProdCompra))
                sql += " and pc.idProdCompra in (" + idsProdCompra + ")";

            return sql;
        }

        public ProdutosCompra[] GetForRpt(uint idCompra)
        {
            List<ProdutosCompra> lstProdCompra = objPersistence.LoadData(Sql(idCompra, null, true) + " Order By pc.IdProdCompra Asc");

            if (!FinanceiroConfig.FinanceiroRec.ImprimirCompraComBenef)
            {
                foreach (ProdutosCompra pc in lstProdCompra)
                    foreach (GenericBenef ppb in pc.Beneficiamentos)
                        pc.DescrBeneficiamentos += Utils.MontaDescrLapBis(ppb.BisAlt, ppb.BisLarg, ppb.LapAlt, ppb.LapLarg, ppb.EspBisote, null, null, true);

                 return lstProdCompra.ToArray();
            }

            List<ProdutosCompra> lstProdCompraRetorno = new List<ProdutosCompra>();

            foreach (ProdutosCompra pc in lstProdCompra)
            {
                // Caso esteja marcado para cobrar apenas beneficimentos, zera o valor unitário e o total 
                // para não somar incorretamente na impressão da compra
                if (pc.NaoCobrarVidro)
                {
                    pc.Valor = 0;
                    pc.Total = 0;
                    pc.ValorBenef = 0;
                }

                lstProdCompraRetorno.Add(pc);

                GenericBenefCollection lstBenef = pc.Beneficiamentos;

                if (!PedidoConfig.RelatorioPedido.AgruparBenefRelatorio)
                {
                    string obs = pc.Obs;
                    pc.Obs = "";

                    // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                    foreach (GenericBenef ppb in lstBenef)
                    {
                        ProdutosCompra prodCompra = new ProdutosCompra();
                        prodCompra.IdCompra = idCompra;
                        prodCompra.Qtde = ppb.Qtd;
                        prodCompra.Valor = ppb.ValorUnit;
                        prodCompra.Total = ppb.Valor;
                        prodCompra.ValorBenef = 0;
                        prodCompra.DescrProduto = " " + ppb.DescricaoBeneficiamento +
                            Utils.MontaDescrLapBis(ppb.BisAlt, ppb.BisLarg, ppb.LapAlt, ppb.LapLarg, ppb.EspBisote, null, null, true);

                        lstProdCompraRetorno.Add(prodCompra);
                    }

                    lstProdCompraRetorno[lstProdCompraRetorno.Count - 1].Obs = obs;
                }
                else
                {
                    if (lstBenef.Count > 0)
                    {
                        ProdutosCompra prodCompra = new ProdutosCompra();
                        prodCompra.IdCompra = idCompra;
                        prodCompra.Qtde = 0;
                        prodCompra.ValorBenef = 0;

                        // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                        foreach (GenericBenef ppb in lstBenef)
                        {
                            prodCompra.Valor += ppb.ValorUnit;
                            prodCompra.Total += ppb.Valor;
                            string textoQuantidade = (ppb.TipoCalculo == TipoCalculoBenef.Quantidade) ? ppb.Qtd.ToString() + " " : "";
                            prodCompra.DescrProduto += "; " + textoQuantidade + ppb.DescricaoBeneficiamento +
                                Utils.MontaDescrLapBis(ppb.BisAlt, ppb.BisLarg, ppb.LapAlt, ppb.LapLarg, ppb.EspBisote, null, null, true);
                        }

                        prodCompra.DescrProduto = " " + (prodCompra.DescrProduto.Length > 2 ? prodCompra.DescrProduto.Substring(2) : string.Empty);
                        lstProdCompraRetorno.Add(prodCompra);
                    }
                }
            }

            return lstProdCompraRetorno.ToArray();
        }

        public IList<ProdutosCompra> GetByCompra(uint idCompra)
        {
            return GetByCompra(null, idCompra);
        }

        public IList<ProdutosCompra> GetByCompra(GDASession sessao, uint idCompra)
        {
            return objPersistence.LoadData(sessao, Sql(idCompra, null, true) + " Order By pc.IdProdCompra Asc").ToList();
        }

        public IList<ProdutosCompra> GetByVariasCompras(string idsCompras, bool agruparProdutos, bool agruparSomentePorProduto)
        {
            string sql, where = "pc.idCompra In (" + idsCompras + ")";

            if (!agruparProdutos)
            {
                sql = @"
                    Select pc.*, p.descricao as DescrProduto, p.codInterno, " + /*(!FiscalConfig.NotaFiscalConfig.ConsiderarM2CalcNotaFiscal ?*/ "pc.TotM" /*: "pc.TotM2Calc")*/ + @"
                        as TotM2Nf, pc.Total as TotalNf, Cast(pc.qtde as signed) as QtdNf, pc.ValorBenef as ValorBenefNf, pc.qtde as qtdeOriginal,
                        Cast(pc.idProd As Unsigned Integer) as idProdUsar
                    From produtos_compra pc 
                        Left Join produto p ON (pc.idProd=p.idProd)
                        left join compra comp on (pc.idCompra=comp.idCompra)
                    where " + where + @"
                    Group By pc.idProdCompra";
            }
            else
            {
                sql = @"
                    Select pc.*, p.descricao as DescrProduto, p.codInterno, Sum(" + /*(!FiscalConfig.NotaFiscalConfig.ConsiderarM2CalcNotaFiscal ?*/ "pc.TotM" /*: "pc.TotM2Calc")*/ + @")
                        as TotM2Nf, cast(Sum(pc.Total) as decimal(12,2)) as TotalNf, Cast(Sum(pc.qtde) as signed) as QtdNf, cast(Sum(pc.ValorBenef) as decimal(12,2)) as ValorBenefNf, 
                        p.idGrupoProd, p.idSubgrupoProd, sum(pc.qtde) as qtdeOriginal, Cast(pc.idProd As Unsigned Integer) as idProdUsar
                    From produtos_compra pc
                        left join compra comp on (pc.idCompra=comp.idCompra)
                        Left Join produto p On (pc.idProd=p.idProd)
                    Where " + where + @"
                    Group By pc.idProd, pc.descricaoItemGenerico" +
                        (!agruparSomentePorProduto ? @", (
                        select Cast(group_concat(idBenefConfig) as char)
                        From (select idProdCompra, idBenefConfig from produtos_compra_benef order by idBenefConfig) as pcb
                        Where pcb.idProdCompra=pc.idProdCompra)" : "");
            }

            List<ProdutosCompra> lstProd = objPersistence.LoadData(sql);
            List<ProdutosCompra> lstRetorno = new List<ProdutosCompra>();

            decimal total = 0;

            if (agruparProdutos)
                foreach (ProdutosCompra pc in lstProd)
                {
                    float qtdOriginal = pc.Qtde;
                    pc.Qtde = (int)pc.QtdNf;

                    if (pc.Qtde <= 0)
                        continue;

                    //Recalcula o valor unitario e atribui os valor calculados no sql nas propiedades
                    //que vão ser utilizadas na nf.
                    if (pc.TotM2Nf > 0)
                        pc.TotM = (float)pc.TotM2Nf;

                    //pc.TotM2Calc = FiscalConfig.NotaFiscalConfig.ConsiderarM2CalcNotaFiscal ? (float)pc.TotM2Nf : pc.TotM;
                    pc.Total = (decimal)pc.TotalNf;
                    pc.ValorBenef = (decimal)pc.ValorBenefNf;

                    int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)pc.IdGrupoProd, (int)pc.IdSubgrupoProd, true);

                    if (tipoCalc == (uint)Glass.Data.Model.TipoCalculoGrupoProd.Qtd || tipoCalc == (uint)Glass.Data.Model.TipoCalculoGrupoProd.QtdM2)
                        pc.Valor = pc.Total / (decimal)pc.Qtde;
                    else if (pc.TotM > 0)
                        pc.Valor = pc.Total / (decimal)pc.TotM;
                    else if (pc.Altura > 0)
                        pc.Valor = (pc.Total * 6) / (decimal)(pc.Altura * pc.Qtde);
                    else
                        pc.Valor = pc.Total / (decimal)pc.Qtde;

                    if (pc.Qtde > 0)
                        lstRetorno.Add(pc);

                    total += pc.Total;
                }
            else
                lstRetorno = lstProd;

            return lstRetorno.ToArray();
        }

        public IList<ProdutosCompra> GetByString(string idsProdCompra)
        {
            return GetByString(null, idsProdCompra);
        }

        public IList<ProdutosCompra> GetByString(GDASession session, string idsProdCompra)
        {
            return objPersistence.LoadData(session, Sql(0, idsProdCompra, true) + " Order By pc.IdProdCompra Asc").ToList();
        }

        public IList<ProdutosCompra> GetList(uint idCompra, string sortExpression, int startRow, int pageSize)
        {
            if (CountInCompra(idCompra) == 0)
                return new ProdutosCompra[] { new ProdutosCompra() };

            sortExpression = String.IsNullOrEmpty(sortExpression) ? "pc.IdProdCompra Asc" : sortExpression;

            return LoadDataWithSortExpression(Sql(idCompra, null, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount(uint idCompra)
        {
            int count = CountInCompra(idCompra);
            return count == 0 ? 1 : count;
        }

        public int CountInCompra(uint idCompra)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idCompra, null, false));
        }

        #endregion

        #region Busca produtos que estão sendo comprados

        /// <summary>
        /// Busca produtos que estão sendo comprados
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public IList<ProdutosCompra> GetProdutosComprando(uint idProd, uint idLoja)
        {
            if (idProd == 0)
                return new List<ProdutosCompra>();

            string sql = @"
                select pc.*, p.Descricao as DescrProduto, p.CodInterno, sum(pc.totM) as totMComprando, sum(pc.qtde) as qtdeComprando,
                    c.dataFabrica
                from produtos_compra pc
                    inner join produto p On (pc.idProd=p.idProd)
                    inner Join compra c On (pc.idCompra=c.idCompra)
                where c.Situacao <> " + (int)Compra.SituacaoEnum.Cancelada + @" 
                    and (c.estoqueBaixado=false or c.estoqueBaixado is null) " + (idLoja > 0 ? " and idLoja=" + idLoja : "") + @"
                    and pc.idProd=" + idProd;

            if (idLoja > 0)
                sql += " and c.idLoja=" + idLoja;

            return objPersistence.LoadData(sql + " group by pc.idCompra").ToList();
        }

        #endregion

        #region Atualiza o valor dos beneficiamentos

        /// <summary>
        /// Atualiza o valor dos beneficiamentos.
        /// </summary>
        public void UpdateValorBenef(GDASession session, uint idProdCompra)
        {
            uint idProd = ObtemIdProd(session, idProdCompra);
            if (Glass.Configuracoes.Geral.NaoVendeVidro() || !ProdutoDAO.Instance.CalculaBeneficiamento(session, (int)idProd))
                return;

            string sql = "update produtos_compra pc set valorBenef=round((select sum(coalesce(valor, 0)) from produtos_compra_benef " +
                "where idProdCompra=pc.idProdCompra), 2) where idProdCompra=" + idProdCompra;

            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Recupera os produtos para entrada de estoque

        /// <summary>
        /// Recupera os produtos para entrada de estoque.
        /// </summary>
        /// <param name="idCompra"></param>
        /// <returns></returns>
        public IList<ProdutosCompra> GetForEntradaEstoque(uint idCompra)
        {
            string sql = @"select pc.*, p.descricao as descrProduto from produtos_compra pc 
                inner join compra c on (pc.idCompra=c.idCompra) inner join produto p on (pc.idProd=p.idProd) 
                where c.idCompra=" + idCompra + " and pc.qtdeEntrada<pc.qtde";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Marca entrada de produtos

        /// <summary>
        /// Marca entrada de produto na tabela produtos_compra
        /// </summary>
        public void MarcarEntrada(uint idProdCompra, float qtdEntrada, uint idEntradaEstoque)
        {
            MarcarEntrada(null, idProdCompra, qtdEntrada, idEntradaEstoque);
        }

        /// <summary>
        /// Marca entrada de produto na tabela produtos_compra
        /// </summary>
        public void MarcarEntrada(GDASession session, uint idProdCompra, float qtdEntrada, uint idEntradaEstoque)
        {
            string sql = "Update produtos_compra set qtdeEntrada=Coalesce(qtdeEntrada, 0)+" + qtdEntrada.ToString().Replace(",", ".") + " Where idProdCompra=" + idProdCompra;
            objPersistence.ExecuteCommand(session, sql);

            // Insere um registro na tabela indicando que o produto foi baixado
            if (idEntradaEstoque > 0)
            {
                ProdutoEntradaEstoque novo = new ProdutoEntradaEstoque();
                novo.IdEntradaEstoque = idEntradaEstoque;
                novo.IdProdCompra = idProdCompra;
                novo.QtdeEntrada = qtdEntrada;

                ProdutoEntradaEstoqueDAO.Instance.Insert(session, novo);
            }
        }

        #endregion

        #region Obtém dados do produto

        public uint ObtemIdProd(GDASession session, uint idProdCompra)
        {
            return ObtemValorCampo<uint>(session, "idProd", "idProdCompra=" + idProdCompra);
        }

        public decimal ObterQtde(GDASession session, int idProdCompra)
        {
            return ObtemValorCampo<decimal>(session, "Qtde", string.Format("IdProdCompra={0}", idProdCompra));
        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Atualiza o valor da compra ao incluir um produto à mesma
        /// </summary>
        public override uint Insert(ProdutosCompra objInsert)
        {
            return Insert(null, objInsert);
        }

        /// <summary>
        /// Atualiza o valor da compra ao incluir um produto à mesma
        /// </summary>
        public override uint Insert(GDASession session, ProdutosCompra objInsert)
        {
            uint returnValue = 0;

            try
            {
                if (CompraDAO.Instance.ObtemSituacao(session, objInsert.IdCompra) == (int)Compra.SituacaoEnum.Finalizada)
                    throw new Exception("A compra está finalizada, não é possível incluir produtos.");

                decimal total = objInsert.Total, custoProd = 0;
                float totM2 = objInsert.TotM, altura = objInsert.Altura, totM2Calc = objInsert.TotM;

                Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(session, 0, (int)objInsert.IdProd, objInsert.Largura, objInsert.Qtde, 1, objInsert.Valor, objInsert.Espessura, 
                    objInsert.Redondo, 0, true, true, ref custoProd, ref altura, ref totM2, ref totM2Calc, ref total, false, 
                    objInsert.Beneficiamentos.CountAreaMinimaSession(session));

                objInsert.TotM = totM2;
                objInsert.Total = !objInsert.NaoCobrarVidro ? (decimal)total : 0;

                returnValue = base.Insert(session, objInsert);

                ProdutosCompraBenefDAO.Instance.DeleteByProdCompra(session, objInsert.IdProdCompra);
                foreach (ProdutosCompraBenef b in objInsert.Beneficiamentos.ToProdutosCompra(returnValue))
                {
                    // No cadastro de compra, o valor do custo está sendo buscado mas não o valor de compra
                    if (b.Valor == 0 && b.Custo > 0)
                    {
                        b.Valor = b.Custo;
                        b.ValorUnit = BenefConfigPrecoDAO.Instance.ObtemCustoBenef(session, b.IdBenefConfig, objInsert.Espessura);
                    }

                    ProdutosCompraBenefDAO.Instance.Insert(session, b);
                }

                objInsert.RefreshBeneficiamentos();

                UpdateValorBenef(session, returnValue);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao incluir Produto na Compra. Erro: " + ex.Message);
            }

            try
            {
                CompraDAO.Instance.UpdateTotalCompra(session, objInsert.IdCompra);
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message;
                base.Delete(session, objInsert);
                throw new Exception("Falha ao atualizar Valor da Compra. Erro: " + msg);
            }

            return returnValue;
        }

        /// <summary>
        /// Atualiza o valor da compra ao excluir um produto da mesma
        /// </summary>
        /// <param name="objDelete"></param>
        /// <returns></returns>
        public override int Delete(ProdutosCompra objDelete)
        {
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produtos_compra Where IdProdCompra=" + objDelete.IdProdCompra) == 0)
                return 0;

            if (CompraDAO.Instance.ObtemSituacao(null, objDelete.IdCompra) == (int)Compra.SituacaoEnum.Finalizada)
                throw new Exception("A compra está finalizada, não é possível apagar qualquer produto.");

            int returnValue = 0;

            try
            {
                ProdutosCompraBenefDAO.Instance.DeleteByProdCompra(objDelete.IdProdCompra);
                returnValue = base.Delete(objDelete);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao excluir Produto da Compra. Erro: " + ex.Message);
            }

            try
            {
                CompraDAO.Instance.UpdateTotalCompra(null, ObtemValorCampo<uint>("idCompra", "idProdCompra=" + objDelete.IdProdCompra));
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao atualizar Valor da Compra. Erro: " + ex.Message);
            }

            return returnValue;
        }

        public override int Update(ProdutosCompra objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Update(GDASession session, ProdutosCompra objUpdate)
        {
            try
            {
                if (CompraDAO.Instance.ObtemSituacao(session, objUpdate.IdCompra) == (int)Compra.SituacaoEnum.Finalizada)
                    throw new Exception("A compra está finalizada, não é possível atualizar os produtos.");

                decimal total = objUpdate.Total, custoProd = 0;
                float totM2 = objUpdate.TotM, altura = objUpdate.Altura, totM2Calc = 0;

                Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(session, 0, (int)objUpdate.IdProd, objUpdate.Largura, objUpdate.Qtde, 1, objUpdate.Valor, objUpdate.Espessura, 
                    objUpdate.Redondo, 0, true, true, ref custoProd, ref altura, ref totM2, ref totM2Calc, ref total, false, objUpdate.Beneficiamentos.CountAreaMinimaSession(session));

                objUpdate.TotM = totM2;
                objUpdate.Total = !objUpdate.NaoCobrarVidro ? total : 0;

                base.Update(session, objUpdate);

                ProdutosCompraBenefDAO.Instance.DeleteByProdCompra(session, objUpdate.IdProdCompra);
                foreach (ProdutosCompraBenef b in objUpdate.Beneficiamentos.ToProdutosCompra(objUpdate.IdProdCompra))
                {
                    // No cadastro de compra, o valor do custo está sendo buscado mas não o valor de compra
                    if (b.Valor == 0 && b.Custo > 0)
                    {
                        if (b.ValorUnit == 0 && b.Valor == 0)
                            b.ValorUnit = BenefConfigPrecoDAO.Instance.ObtemCustoBenef(session, b.IdBenefConfig, objUpdate.Espessura);

                         b.Valor = b.Custo;
                    }

                    ProdutosCompraBenefDAO.Instance.Insert(session, b);
                }

                objUpdate.RefreshBeneficiamentos();

                UpdateValorBenef(session, objUpdate.IdProdCompra);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao atualizar Produto do Pedido. Erro: " + ex.Message);
            }

            try
            {
                CompraDAO.Instance.UpdateTotalCompra(session, objUpdate.IdCompra);
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message;

                if (session == null)
                    base.Delete(objUpdate);

                throw new Exception("Falha ao atualizar Valor da Compra. Erro: " + msg);
            }

            return 1;
        }

        #endregion
    }
}