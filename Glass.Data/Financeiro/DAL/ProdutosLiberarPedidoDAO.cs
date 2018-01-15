using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ProdutosLiberarPedidoDAO : BaseDAO<ProdutosLiberarPedido, ProdutosLiberarPedidoDAO>
    {
        //private ProdutosLiberarPedidoDAO() { }

        private string Sql(uint idLiberarPedido, uint idPedido, string idsLiberarPedido)
        {
            string qtdeProd = "(if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + ", coalesce(ae.qtde, a.qtde, 1), pp.qtde))";
            string sql = @"
                select plp.*, lp.idCliente, Cast(sum(plp.qtde) as decimal(12,2)) as QtdeTotal, p.Descricao as DescrProduto, p.codInterno, pp.Qtde as QtdeProd, pp.peso as PesoProd, 
                    ((pp.TotM/" + qtdeProd + ") * (sum(plp.qtde) / IF(ped.TipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + ", pp.Qtde, 1))) as TotM2, ((pp.TotM2Calc/" + qtdeProd +
                    ") * (sum(plp.qtde) / IF(ped.TipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", pp.Qtde, 1))) as totM2Calc, coalesce(pp.Espessura, p.Espessura) as EspessuraProd,
                    pp.Total as Total, if(ped.valorIcms>0, pp.ValorIcms, 0) as ValorIcmsProd, if(ped.valorIpi>0, pp.valorIpi, 0) as ValorIpiProd,
                    pp.ValorBenef, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.redondo, pp.Redondo) as redondo, ped.tipoPedido, ped.tipoVenda, pp.pedCli, pp.Altura as Altura, pp.Largura as Largura, ppe.alturaReal, ppe.larguraReal,
                    if(!pp.invisivelPedido, a.qtde, ae.qtde) as qtdeAmbiente, ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @" as pedidoMaoDeObra, 
                    a.idAmbientePedido, ae.idAmbientePedido as idAmbientePedidoEspelho, pp.idProd as idProd, p.idGrupoProd, p.idSubgrupoProd, 
                    a.ambiente, ae.ambiente as ambientePedidoEspelho, apl.CodInterno as CodAplicacao, prc.CodInterno as CodProcesso, 
                    g.descricao as descrGrupoProd, s.descricao as descrSubgrupoProd, (coalesce(pp.valorDesconto,0) + coalesce(pp.valorDescontoProd,0) + 
                    coalesce(pp.valorDescontoQtde,0) + coalesce(pp.valorDescontoCliente,0)) as valorDescontoTotal, pp.valorUnitBruto, pp.totalBruto, pp.alturaBenef, pp.larguraBenef,
                    pp.percDescontoQtde
                from produtos_liberar_pedido plp 
                    left join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido)
                    left join produtos_pedido pp on (plp.idProdPed=pp.idProdPed)
                    left join produto p on (pp.idProd=p.idProd)
                    left join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                    left join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd)
                    left join ambiente_pedido a on (pp.idAmbientePedido=a.idAmbientePedido)
                    left join produtos_pedido_espelho ppe on (pp.idProdPedEsp=ppe.idProdPed)
                    left join ambiente_pedido_espelho ae on (ppe.idAmbientePedido=ae.idAmbientePedido)
                    left join pedido ped on (pp.idPedido=ped.idPedido)
                    Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao)
                    Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso)
                where 1 ";

            if (idLiberarPedido > 0)
                sql += " and plp.idLiberarPedido=" + idLiberarPedido;
            else if (idPedido > 0)
                sql += " and plp.idPedido=" + idPedido;

            if (!String.IsNullOrEmpty(idsLiberarPedido))
                sql += " and plp.idLiberarPedido in (" + idsLiberarPedido + ")";

            sql += " group by plp.idProdPed";

            return sql;
        }

        public ProdutosLiberarPedido[] GetByLiberarPedido(uint idLiberarPedido)
        {
            return GetByLiberarPedido(null, idLiberarPedido, true);
        }

        internal ProdutosLiberarPedido[] GetByLiberarPedido(GDASession sessao, uint idLiberarPedido, bool getBeneficiamentos)
        {
            List<ProdutosLiberarPedido> retorno = objPersistence.LoadData(sessao, Sql(idLiberarPedido, 0, null));

            if (getBeneficiamentos)
            {
                foreach (ProdutosLiberarPedido plp in retorno)
                    try
                    {
                        if (ProdutoPedidoBenefDAO.Instance.PossuiBeneficiamento(sessao, plp.IdProdPed))
                            plp.DescrBeneficiamentos = ProdutosPedidoDAO.Instance.GetBeneficiamentos(sessao, plp.IdProdPed).DescricaoBeneficiamentos;
                    }
                    catch { }
            }

            return retorno.ToArray();
        }

        public ProdutosLiberarPedido[] GetByPedido(uint idPedido)
        {
            List<ProdutosLiberarPedido> retorno = objPersistence.LoadData(Sql(0, idPedido, null));

            foreach (ProdutosLiberarPedido plp in retorno)
                try
                {
                    if (ProdutoPedidoBenefDAO.Instance.PossuiBeneficiamento(null, plp.IdProdPed))
                        plp.DescrBeneficiamentos = ProdutosPedidoDAO.Instance.GetBeneficiamentos(null, plp.IdProdPed).DescricaoBeneficiamentos;
                }
                catch { }

            return retorno.ToArray();
        }

        public float GetQtdeByProdPed(uint idProdPed, uint? idProdPedProducao)
        {
            string sqlBase = @"select coalesce(sum(qtdeCalc), 0) from produtos_liberar_pedido
                where qtdeCalc>0 and idProdPed=" + idProdPed;

            string sql = sqlBase;
            if (idProdPedProducao > 0)
                sql += " and idProdPedProducao=" + idProdPedProducao;

            float retorno = ExecuteScalar<float>(sql);
            return retorno > 0 ? retorno : ExecuteScalar<float>(sqlBase + " and idProdPedProducao is null");
        }

        public decimal ObterQtde(GDASession session, int idProdLiberarPedido)
        {
            return ObtemValorCampo<decimal>(session, "Qtde", string.Format("IdProdLiberarPedido={0}", idProdLiberarPedido));
        }

        public uint ObtemIdProdLiberarPedido(uint idLiberarPedido, uint idProdPed)
        {
            return ObtemIdProdLiberarPedido(null, idLiberarPedido, idProdPed);
        }

        public uint ObtemIdProdLiberarPedido(GDASession session, uint idLiberarPedido, uint idProdPed)
        {
            return ObtemValorCampo<uint>(session, "idProdLiberarPedido", "idLiberarPedido=" + idLiberarPedido + " And idProdPed=" + idProdPed);
        }

        public void DeleteByLiberarPedido(uint idLiberarPedido)
        {
            objPersistence.ExecuteCommand("delete from produtos_liberar_pedido where idLiberarPedido=" + idLiberarPedido);
        }

        public ProdutosLiberarPedido[] GetForRpt(uint idLiberarPedido)
        {
            string sql = !Liberacao.RelatorioLiberacaoPedido.OrdenarProdutosPeloCodInterno ? "Select * From (" + Sql(idLiberarPedido, 0, null) + ") as tbl Order By totM2 Desc" :
                Sql(idLiberarPedido, 0, null) + " Order By p.codInterno Asc";

            List<ProdutosLiberarPedido> retorno = objPersistence.LoadData(sql).ToList();
            
            foreach (ProdutosLiberarPedido plp in retorno)
            {
                // Alterações neste trecho devem ser feitas também em ProdutosPedidoDAO.GetForRpt(uint, bool, bool)
                if (plp.Redondo)
                {
                    if (!PedidoDAO.Instance.IsMaoDeObra(plp.IdPedido) && !plp.DescrProduto.ToLower().Contains("redondo"))
                        plp.DescrProduto += " REDONDO";

                    plp.LarguraProd = 0;
                    plp.LarguraReal = 0;
                }    

                try
                {
                    uint? idProdPedEsp = ProdutosPedidoDAO.Instance.ObterIdProdPedEsp(plp.IdProdPed);

                    // Empresas que devem exibir o número da etiqueta na impressão da liberação
                    if (FinanceiroConfig.DadosLiberacao.ExibirNumeroEtiquetaLiberacao && idProdPedEsp != null)
                    {
                        var exibirTodasEtiquetas = !FinanceiroConfig.DadosLiberacao.ExibirAsQuatroPrimeirasEtiquetasNaLiberacao;

                        string etiquetas = ProdutoPedidoProducaoDAO.Instance.GetEtiquetasByIdProdPedLiberacao(idProdPedEsp.Value, idLiberarPedido);
                        if (String.IsNullOrEmpty(etiquetas))
                            etiquetas = ProdutoPedidoProducaoDAO.Instance.GetEtiquetasByIdProdPed(idProdPedEsp.Value);

                        if (!String.IsNullOrEmpty(etiquetas))
                        {
                            plp.NumEtiquetas = "Etiqueta" + (etiquetas.IndexOf(", ") > -1 ? "s" : "") + ": ";

                            var etqs = etiquetas.Split(',');

                            for (int i = 0; i < etqs.Length; i++)
                            {
                                if (!exibirTodasEtiquetas && i == 4) break;

                                plp.NumEtiquetas += etqs[i].Trim() + ", ";
                            }

                            plp.NumEtiquetas = plp.NumEtiquetas.Trim().Trim(',');
                        }
                    }

                    if (ProdutoPedidoBenefDAO.Instance.PossuiBeneficiamento(null, plp.IdProdPed))
                    {
                        GenericBenefCollection benef = new GenericBenefCollection(ProdutoPedidoBenefDAO.Instance.GetByProdutoPedido(plp.IdProdPed));
                        plp.DescrBeneficiamentos = benef.DescricaoBeneficiamentos;
                    }

                    // Exibe o percentual de desconto por qtd concatenado com a descrição
                    if (!Geral.NaoVendeVidro() && plp.PercDescontoQtde > 0)
                        plp.DescrProduto += "\r\n(Desc. Prod.: " + plp.PercDescontoQtde + "%)";

                    //Exibe as etiquetad de cavalete
                    if (PCPConfig.ControleCavalete && idProdPedEsp.GetValueOrDefault(0) > 0)
                    {
                        var numCavaletes = ProdutoPedidoProducaoDAO.Instance.GetCavaletesByIdProdPed(idProdPedEsp.Value);
                        if (!string.IsNullOrEmpty(numCavaletes))
                            plp.NumCavaletes = "Cavalete" + (numCavaletes.IndexOf(", ") > -1 ? "s" : "") + ": " + numCavaletes;
                    }
                }
                catch { }
            }

            return retorno.ToArray();
        }

        /// <summary>
        /// Retorna os IDs dos pedidos de uma liberação.
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public string GetIdsPedidoByLiberacaoString(uint idLiberarPedido)
        {
            return GetIdsPedidoByLiberacaoString(null, idLiberarPedido);
        }

        /// <summary>
        /// Retorna os IDs dos pedidos de uma liberação.
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public string GetIdsPedidoByLiberacaoString(GDASession sessao, uint idLiberarPedido)
        {
            return objPersistence.ExecuteScalar(sessao, @"select Cast(group_concat(distinct idPedido) as char) from produtos_liberar_pedido 
                where idLiberarPedido=" + idLiberarPedido).ToString();
        }

        /// <summary>
        /// Retorna os IDs dos pedidos de uma liberação.
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public uint[] GetIdsPedidoByLiberacao(uint idLiberarPedido)
        {
            string ids = GetIdsPedidoByLiberacaoString(idLiberarPedido);

            return Array.ConvertAll<string, uint>(ids.Split(','), new Converter<string, uint>(
                delegate(string id)
                {
                    uint r;
                    return uint.TryParse(id, out r) ? r : 0;
                }
            ));
        }

        public decimal GetValorIcmsForLiberacao(uint idCliente, uint idProdutoPedido, float qntdeLiberada)
        {
            return GetValorIcmsForLiberacao(null, idCliente, idProdutoPedido, qntdeLiberada);
        }

        public decimal GetValorIcmsForLiberacao(GDASession session, uint idCliente, uint idProdutoPedido, float qntdeLiberada)
        {
            ProdutosPedido pp = ProdutosPedidoDAO.Instance.GetElement(session, idProdutoPedido);

            if (Liberacao.Impostos.CalcularIcmsLiberacao && (ClienteDAO.Instance.IsCobrarIcmsSt(session, idCliente) || PedidoDAO.Instance.CobrouICMSST(session, pp.IdPedido)))
            {
                if (pp.ValorIcms == 00 || pp.Qtde == 0)
                    return 0;

                return (pp.ValorIcms / Convert.ToDecimal(pp.Qtde)) * Convert.ToDecimal(qntdeLiberada);
            }

            return 0;
        }

        public decimal GetValorIpiForLiberacao(uint idCliente, uint idProdutoPedido, float qntdeLiberada)
        {
            return GetValorIpiForLiberacao(null, idCliente, idProdutoPedido, qntdeLiberada);
        }

        public decimal GetValorIpiForLiberacao(GDASession session, uint idCliente, uint idProdutoPedido, float qntdeLiberada)
        {
            ProdutosPedido pp = ProdutosPedidoDAO.Instance.GetElement(session, idProdutoPedido);

            if (Liberacao.Impostos.CalcularIpiLiberacao && (ClienteDAO.Instance.IsCobrarIpi(session, idCliente) || PedidoDAO.Instance.CobrouIPI(session, pp.IdPedido)))
            {
                if (pp.ValorIpi == 00 || pp.Qtde == 0)
                    return 0;

                return (pp.ValorIpi / Convert.ToDecimal(pp.Qtde)) * Convert.ToDecimal(qntdeLiberada);
            }

            return 0;
        }

        public IList<ProdutosLiberarPedido> GetByLiberacoes(IEnumerable<LiberarPedido> liberacoes)
        {
            string idsLiberarPedido = String.Join(",", Array.ConvertAll<LiberarPedido, string>(
                Glass.MetodosExtensao.ToArray(liberacoes), x => x.IdLiberarPedido.ToString()));

            return GetByLiberacoes(idsLiberarPedido);
        }

        public IList<ProdutosLiberarPedido> GetByLiberacoes(string idsLiberarPedido)
        {
            return GetByLiberacoes(null, idsLiberarPedido);
        }

        public IList<ProdutosLiberarPedido> GetByLiberacoes(GDASession sessao, string idsLiberarPedido)
        {
            return String.IsNullOrEmpty(idsLiberarPedido) ? new List<ProdutosLiberarPedido>() : 
                objPersistence.LoadData(sessao, Sql(0, 0, idsLiberarPedido)).ToList();
        }

        #region Métodos sobrescritos

        public override uint Insert(GDASession sessao, ProdutosLiberarPedido objInsert)
        {
            string sqlVerificar = "select count(*) from produtos_liberar_pedido where idLiberarPedido=" + objInsert.IdLiberarPedido +
                " and idProdPed=" + objInsert.IdProdPed + (objInsert.IdProdPedProducao > 0 ? " and idProdPedProducao=" + objInsert.IdProdPedProducao : "");

            if (objPersistence.ExecuteSqlQueryCount(sessao, sqlVerificar) > 0)
                return 0;

            return base.Insert(sessao, objInsert);
        }

        public override uint Insert(ProdutosLiberarPedido objInsert)
        {
            return Insert(null, objInsert);
        }

        #endregion
    }
}
