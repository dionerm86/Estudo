using System;
using System.Collections.Generic;
using System.Text;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.IO;
using System.Linq;
using Glass.Configuracoes;
using Glass.Global;
using Glass.Data.Helper.Calculos;
using Glass.Data.Model.Calculos;

namespace Glass.Data.DAL
{
    public sealed class ProdutosPedidoEspelhoDAO : BaseDAO<ProdutosPedidoEspelho, ProdutosPedidoEspelhoDAO>
    {
        //private ProdutosPedidoEspelhoDAO() { }

        #region Busca produtos para listagem padrão

        private string Sql(uint idProdPed, uint idPedido, uint idAmbientePedido, string grupos, string produtos, bool soProdutosComprados,
            string idsPedidos, bool cadastro, bool selecionar, bool forPcp, bool buscarEtiquetas, bool apenasVisiveis,
            out bool temFiltro, out string filtroAdicional)
        {
            return Sql(idProdPed, idPedido, idAmbientePedido, grupos, produtos, soProdutosComprados, idsPedidos, cadastro,
                selecionar, forPcp, buscarEtiquetas, apenasVisiveis, true, out temFiltro, out filtroAdicional);
        }

        private string Sql(uint idProdPed, uint idPedido, uint idAmbientePedido, string grupos, string produtos, bool soProdutosComprados,
            string idsPedidos, bool cadastro, bool selecionar, bool forPcp, bool buscarEtiquetas, bool apenasVisiveis, bool verificarProdutoComprado,
            out bool temFiltro, out string filtroAdicional)
        {
            return Sql(idProdPed, idPedido, idAmbientePedido, grupos, produtos, soProdutosComprados, idsPedidos, cadastro, selecionar,
                forPcp, buscarEtiquetas, apenasVisiveis, verificarProdutoComprado, false, out temFiltro, out filtroAdicional);
        }

        private string Sql(uint idProdPed, uint idPedido, uint idAmbientePedido, string grupos, string produtos, bool soProdutosComprados,
            string idsPedidos, bool cadastro, bool selecionar, bool forPcp, bool buscarEtiquetas, bool apenasVisiveis, bool verificarProdutoComprado,
            bool ignorarProdutoComposicao, out bool temFiltro, out string filtroAdicional)
        {
            string sqlComprado = "select count(*) from produtos_compra pc left join compra c on (pc.idCompra=c.idCompra) " +
                "where idProdPed=pp.idProdPed and pc.total>0 and c.situacao=" + (int)Compra.SituacaoEnum.Finalizada;

            string campos = selecionar ? @"
                pp.*, ped.idCli as idCliente, p.Descricao as DescrProduto, p.CodInterno, p.IdGrupoProd, p.codOtimizacao, p.idSubgrupoProd, 
                if(p.AtivarAreaMinima=1, Cast(p.AreaMinima as char), '0') as AreaMinima, apl.CodInterno as CodAplicacao, p.custoCompra as custoCompraProduto,
                prc.CodInterno as CodProcesso, ap.ambiente as ambientePedido, ap.descricao as descrAmbientePedido, (" + (verificarProdutoComprado ? sqlComprado : "0") + @")>0 as comprado,
                (ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @") as pedidoMaoObra " : "Count(*)";

            string sql = "Select " + campos + @" From produtos_pedido_espelho pp 
                Left Join ambiente_pedido_espelho ap On (pp.idAmbientePedido=ap.idAmbientePedido)
                Left Join pedido ped On (pp.idPedido=ped.idPedido)
                Left Join produto p On (pp.idProd=p.idProd) 
                Left Join etiqueta_aplicacao apl On (COALESCE(pp.idAplicacao, ap.idAplicacao)=apl.idAplicacao)
                Left Join etiqueta_processo prc On (COALESCE(pp.idProcesso, ap.idProcesso)=prc.idProcesso) Where 1 " + FILTRO_ADICIONAL;

            temFiltro = true;
            StringBuilder fa = new StringBuilder();

            if (idProdPed > 0)
                fa.AppendFormat(" And pp.idProdPed={0}", idProdPed);
            else if (apenasVisiveis)
                fa.Append(" and (pp.invisivelFluxo=false or pp.invisivelFluxo is null)");

            if (idPedido > 0)
                fa.AppendFormat(" And pp.idPedido={0}", idPedido);
            else if (!String.IsNullOrEmpty(idsPedidos))
                fa.AppendFormat(" And pp.idPedido in ({0})", idsPedidos);

            if (!String.IsNullOrEmpty(grupos))
            {
                sql += " and p.idGrupoProd in (" + grupos + ")";
                temFiltro = true;
            }

            if (PedidoConfig.DadosPedido.AmbientePedido && !String.IsNullOrEmpty(produtos))
                fa.AppendFormat(" and pp.idProdPed in ({0})", produtos);
            else if (idAmbientePedido > 0)
                fa.AppendFormat(" and pp.idAmbientePedido={0}", idAmbientePedido);
            else if (PedidoConfig.DadosPedido.AmbientePedido && cadastro)
                sql = "Select pp.* From produtos_pedido_espelho pp Where 0>1";
            else if (!PedidoConfig.DadosPedido.AmbientePedido && cadastro)
                fa.Append(" And pp.idItemProjeto is null");

            if (ignorarProdutoComposicao)
                sql += " AND pp.IdProdPedParent IS NULL";

            if (soProdutosComprados)
                sql += " having comprado=true";

            filtroAdicional = fa.ToString();
            return sql;
        }

        public IList<ProdutosPedidoEspelho> GetList(uint idPedido, uint idAmbientePedido, string sortExpression, int startRow, int pageSize)
        {
            if (CountInPedidoAmbiente(idPedido, idAmbientePedido) == 0)
            {
                var lst = new List<ProdutosPedidoEspelho>();
                lst.Add(new ProdutosPedidoEspelho());
                return lst.ToArray();
            }

            bool temFiltro;
            string filtroAdicional;
            var lstProdPed = LoadDataWithSortExpression(Sql(0, idPedido, idAmbientePedido, null, null, false, null, true,
                 true, false, true, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional),
                 sortExpression, startRow, pageSize);

            PreencheDesmembrarVisible(ref lstProdPed);

            return lstProdPed;
        }

        public int GetCount(uint idPedido, uint idAmbientePedido)
        {
            int count = CountInPedidoAmbiente(idPedido, idAmbientePedido);
            return count == 0 ? 1 : count;
        }

        public IList<ProdutosPedidoEspelho> GetByAmbiente(uint idPedido, uint idAmbientePedido)
        {
            return GetByAmbiente(idPedido, idAmbientePedido, false);
        }

        public IList<ProdutosPedidoEspelho> GetByAmbiente(uint idPedido, uint idAmbientePedido, bool forPcp)
        {
            bool temFiltro;
            string filtroAdicional;
            return objPersistence.LoadData(Sql(0, idPedido, idAmbientePedido, null, null, false, null, true, true,
                forPcp, true, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional)).ToList();
        }

        public IList<ProdutosPedidoEspelho> GetByAmbienteInstalacao(uint idAmbientePedido)
        {
            bool temFiltro;
            string filtroAdicional;
            return objPersistence.LoadData(Sql(0, 0, idAmbientePedido, null, null, false, 
                null, true, true, false,
                true, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional)).ToList();
        }

        // Retorna a quantidade de produtos relacionados ao pedido passado
        public int CountInPedidoAmbiente(uint idPedido, uint idAmbientePedido)
        {
            bool temFiltro;
            string filtroAdicional;
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idPedido, idAmbientePedido, null, null, false, null, true, false,
                false, false, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional));
        }

        public ProdutosPedidoEspelho GetElement(uint idProdPed, bool buscarEtiqueta)
        {
            return GetElement(null, idProdPed, buscarEtiqueta);
        }

        public ProdutosPedidoEspelho GetElement(GDASession session, uint idProdPed, bool buscarEtiqueta)
        {
            bool temFiltro;
            string filtroAdicional;
            return objPersistence.LoadOneData(session, Sql(idProdPed, 0, 0, null, null, false, null, false, true,
                false, buscarEtiqueta, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional));
        }

        private void PreencheDesmembrarVisible(ref IList<ProdutosPedidoEspelho> lstProdPed)
        {
            foreach (var ppe in lstProdPed)
            {
                ppe.DesmembrarVisible = ppe.EditDeleteVisible && ppe.Qtde > 1 && Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)ppe.IdGrupoProd) && 
                    ExecuteScalar<bool>("select count(*)=0 from produto_impressao where idProdPed=" + ppe.IdProdPed);
            }
        }

        #endregion

        #region Busca produtos para arquivo de otimização

        /// <summary>
        /// Busca produtos para arquivo de otimização.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public ProdutosPedidoEspelho GetForArquivoOtimizacao(uint idProdPed)
        {
            return GetForArquivoOtimizacao(null, idProdPed);
        }

        /// <summary>
        /// Busca produtos para arquivo de otimização.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public ProdutosPedidoEspelho GetForArquivoOtimizacao(GDASession sessao, uint idProdPed)
        {
            var exportarInfoEtiquetaOptyWay = Glass.Configuracoes.PCPConfig.ExportarInfoEtiquetaOptyWay;

            var campos = @"pp.*, p.Descricao as DescrProduto, p.codOtimizacao, apl.CodInterno as CodAplicacao, 
                prc.CodInterno as CodProcesso, ppp.pecaReposta, ppp.numEtiqueta, (ped.tipoPedido='" + (int)Pedido.TipoPedidoEnum.MaoDeObra + "') as PedidoMaoObra {0}";

            campos = string.Format(campos, exportarInfoEtiquetaOptyWay ? ", r.codInterno as RotaCliente, pedEsp.DataFabrica, c.NomeCidade, ped.CodCliente" : "");

            string sql = "Select " + campos + @" 
                From produtos_pedido_espelho pp
                    INNER JOIN pedido ped ON (pp.idPedido = ped.idPedido)
                    Left Join produto p On (pp.idProd=p.idProd) 
                    Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao) 
                    Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso) 
                    Left Join produto_pedido_producao ppp on (pp.idProdPed=ppp.idProdPed)
                    {0}
                Where 1";

            if (idProdPed > 0)
                sql += " And pp.idProdPed in (" + idProdPed + ")";
            else
                sql += " and (pp.invisivelFluxo=false or pp.invisivelFluxo is null)";

            sql = string.Format(sql, exportarInfoEtiquetaOptyWay ? @"
                    LEFT JOIN rota_cliente rc ON (ped.idCli = rc.idCliente) 
                    LEFT JOIN rota r ON (rc.idRota = r.idRota)
                    LEFT JOIN pedido_espelho pedEsp ON (ped.idPedido = pedEsp.idPedido)
                    LEFT JOIN cliente cli ON (ped.idCli = cli.id_cli)
                    LEFT JOIN cidade c ON (cli.idCidade = c.idCidade)" : "");

            var r = objPersistence.LoadData(sessao, sql + " Order By ppp.situacao asc").ToList();

            return r.Count > 0 ? r[0] : null;
        }

        #endregion

        #region Busca todos os produtos relacionados ao pedido
        
        /// <summary>
        /// Busca todos os produtos espelho relacionados ao pedido espelho
        /// </summary>
        public IList<ProdutosPedidoEspelho> GetByPedido(uint idPedido, bool buscarEtiquetas)
        {
            return GetByPedido(null, idPedido, buscarEtiquetas);
        }

        /// <summary>
        /// Busca todos os produtos espelho relacionados ao pedido espelho
        /// </summary>
        public IList<ProdutosPedidoEspelho> GetByPedido(GDASession sessao, uint idPedido, bool buscarEtiquetas)
        {
            return GetByPedido(sessao, idPedido, buscarEtiquetas, true);
        }

        /// <summary>
        /// Busca todos os produtos espelho relacionados ao pedido espelho
        /// </summary>
        public IList<ProdutosPedidoEspelho> GetByPedido(GDASession sessao, uint idPedido, bool buscarEtiquetas,
            bool verificarProdutoComprado)
        {
            return GetByPedido(sessao, idPedido, buscarEtiquetas, verificarProdutoComprado, false);
        }

        /// <summary>
        /// Busca todos os produtos espelho relacionados ao pedido espelho
        /// </summary>
        public IList<ProdutosPedidoEspelho> GetByPedido(GDASession sessao, uint idPedido, bool buscarEtiquetas,
            bool verificarProdutoComprado, bool ignorarProdutoComposicao)
        {
            bool temFiltro;
            string filtroAdicional;
            return objPersistence.LoadData(sessao, Sql(0, idPedido, 0, null, null, false, null, false, true, false,
                buscarEtiquetas, true, verificarProdutoComprado, ignorarProdutoComposicao,
                out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional)).ToList();        }

        public IList<ProdutosPedidoEspelho> GetForResumoCorte(string idsPedidos, string grupos, string produtos)
        {
            bool temFiltro;
            string filtroAdicional;
            return objPersistence.LoadData(Sql(0, 0, 0, grupos, produtos, false, idsPedidos, false, true,
                false, true, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional)).ToList();
        }

        #endregion

        #region Busca produtos do grupo vidro relacionados ao pedido

        private string SqlProdEtiq(uint idPedido, uint idProdPed, string descrProd, uint idCorVidro, float espessura, int maoDeObra, string dataIni,
            string dataFim, string dataFabricaIni, string dataFabricaFim, string codProcesso, string codAplicacao,
            bool buscarPecasDeBox, int? pecasRepostas, uint idRota, bool apenasConferFinalizadas, uint idSubgrupoProd, float alturaMin,
            float alturaMax, int larguraMin, int larguraMax, string idsRotas, bool selecionar)
        {
            return SqlProdEtiq(idPedido, idProdPed, descrProd, idCorVidro, espessura, maoDeObra, dataIni, dataFim, dataFabricaIni,
                dataFabricaFim, codProcesso, codAplicacao, buscarPecasDeBox, pecasRepostas, idRota,
                apenasConferFinalizadas, idSubgrupoProd, alturaMin, alturaMax, larguraMin, larguraMax, idsRotas, null, selecionar);
        }

        private string SqlProdEtiq(uint idPedido, uint idProdPed, string descrProd, uint idCorVidro, float espessura, int maoDeObra, string dataIni,
            string dataFim, string dataFabricaIni, string dataFabricaFim, string codProcesso, string codAplicacao,
            bool buscarPecasDeBox, int? pecasRepostas, uint idRota, bool apenasConferFinalizadas, uint idSubgrupoProd, float alturaMin,
            float alturaMax, int larguraMin, int larguraMax, string idsRotas, int? fastDelivery, bool selecionar)
        {
            return SqlProdEtiq(idPedido, idProdPed, descrProd, idCorVidro, espessura, maoDeObra, dataIni, dataFim, dataFabricaIni, dataFabricaFim, 
                codProcesso, codAplicacao, buscarPecasDeBox, pecasRepostas, idRota, apenasConferFinalizadas, idSubgrupoProd, alturaMin, alturaMax,
                larguraMin, larguraMax, idsRotas, fastDelivery, selecionar, null);
        }

        private string SqlProdEtiq(uint idPedido, uint idProdPed, string descrProd, uint idCorVidro, float espessura, int maoDeObra, string dataIni,
            string dataFim, string dataFabricaIni, string dataFabricaFim, string codProcesso, string codAplicacao,
            bool buscarPecasDeBox, int? pecasRepostas, uint idRota, bool apenasConferFinalizadas, uint idSubgrupoProd, float alturaMin,
            float alturaMax, int larguraMin, int larguraMax, string idsRotas, int? fastDelivery, bool selecionar, int? idLoja)
        {
            string campos = selecionar ? @"pp.idProdPed, pp.idPedido, pp.idProd, pp.idItemProjeto, pp.idMaterItemProj, pp.idAmbientePedido, 
                pp.idAplicacao, pp.idProcesso, pp.qtde, pp.valorVendido, pp.altura, pp.alturaReal, pp.largura, pp.larguraReal, 
                pp.totM, pp.totM2Calc, pp.qtdImpresso, pp.total, pp.aliqIcms, pp.valorIcms, pp.ambiente, pp.obs, pp.redondo, 
                pp.valorBenef, pp.pedCli, pp.alturaBenef, pp.larguraBenef, pp.espBenef, pp.valorAcrescimo, pp.valorDesconto, 
                pp.valorAcrescimoProd, pp.valorDescontoProd, pp.percDescontoQtde, pp.valorDescontoQtde, p.Descricao 
                as DescrProduto, p.CodInterno, p.IdGrupoProd, p.idSubgrupoProd, apl.CodInterno as CodAplicacao, prc.CodInterno as 
                CodProcesso, pp.valorDescontoCliente, pp.valorAcrescimoCliente, pp.invisivelFluxo, pp.invisivelAdmin, pp.aliquotaIpi, 
                pp.valorIpi, pp.valorUnitBruto, pp.totalBruto, ped.dataEntrega, ped.dataPedido, cli.nome as nomeCliente, pp.QtdeInvisivel, 
                r.codInterno as RotaCliente, cv.idCorVidro, cv.Descricao as Cor, pp.Espessura, pp.Peso, pp.valorComissao, pedEsp.dataFabrica, pp.IdProdPedParent" : "count(*) as num";

            string where = "";
            string tipoCalculoNaoQtd = "coalesce(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + ")<>" + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd;
            string produtoParaVenda = tipoCalculoNaoQtd + " or (s.produtosEstoque is null or s.produtosEstoque=false)";

            if (buscarPecasDeBox)
                produtoParaVenda = "true";

            if (idPedido > 0)
                where += " And pp.idPedido=" + idPedido;

            if (idLoja > 0)
                //where += " And ped.IdLoja=" + idLoja;
                /* Chamado 48035. */
                where += string.Format(" AND (ped.IdLoja={0} OR ped.TipoPedido={1})", idLoja, (int)Pedido.TipoPedidoEnum.Producao);

            if (idProdPed > 0)
                where += " and pp.idProdPed=" + idProdPed;

            if (apenasConferFinalizadas)
                where += " And pp.idPedido In (Select idPedido From pedido_espelho where situacao<>" + (int)PedidoEspelho.SituacaoPedido.Aberto + ")";

            if (idCorVidro > 0)
                where += " And cv.idCorVidro=" + idCorVidro;

            if (espessura > 0)
                where += " And p.espessura=" + espessura.ToString().Replace(",", ".");

            if (!String.IsNullOrEmpty(descrProd))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descrProd);
                where += " And p.idProd In (" + ids + ")";
            }

            if (!String.IsNullOrEmpty(dataIni))
                where += " and ped.dataEntrega>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                where += " and ped.dataEntrega<=?dataFim";

            if (!String.IsNullOrEmpty(dataFabricaIni))
                where += " and pedEsp.dataFabrica>=?dataFabricaIni";

            if (!String.IsNullOrEmpty(dataFabricaFim))
                where += " and pedEsp.dataFabrica<=?dataFabricaFim";

            switch (fastDelivery)
            {
                case 0:
                    where += " AND (ped.FastDelivery IS NULL OR ped.FastDelivery = 0)";
                    break;

                case 2:
                    where += " and ped.FastDelivery IS NOT NULL AND ped.FastDelivery = 1";
                    break;

                default:
                    break;
            }
            
            if (!String.IsNullOrEmpty(codProcesso) && codProcesso != "0")
                where += " And prc.codInterno=?codProc";

            if (!String.IsNullOrEmpty(codAplicacao) && codAplicacao != "0")
                where += " And apl.codInterno=?codApl";

            if (idRota > 0)
                where += " And r.idRota = " + idRota;
            else if (!string.IsNullOrEmpty(idsRotas))
                where += " AND r.idRota IN (" + idsRotas + ")";

            if (idSubgrupoProd > 0)
                where += " And p.idSubgrupoProd=" + idSubgrupoProd;

            if (alturaMin > 0)
                where += " and if(pp.alturaReal>0, pp.alturaReal, pp.altura)>=" + alturaMin.ToString().Replace(',', '.');

            if (alturaMax > 0)
                where += " and if(pp.alturaReal>0, pp.alturaReal, pp.altura)<=" + alturaMax.ToString().Replace(',', '.');

            if (larguraMin > 0)
                where += " and if(pp.larguraReal>0, pp.larguraReal, pp.largura)>=" + larguraMin;

            if (larguraMax > 0)
                where += " and if(pp.larguraReal>0, pp.larguraReal, pp.largura)<=" + larguraMax;

            var filtroApenasVidros = string.Format(" AND (p.IdGrupoProd={0} AND (ped.TipoPedido={1} OR {2}))", (int)NomeGrupoProd.Vidro, (int)Pedido.TipoPedidoEnum.Producao, produtoParaVenda);

            string sql = String.Empty;

            if (pecasRepostas != 2)
            {
                sql += "Select " + campos + @", null as numEtiqueta, false as pecaReposta, cast(0 as signed) as qtdeReposta
                    From produtos_pedido_espelho pp 
                        Inner Join pedido_espelho pedEsp On (pp.idPedido=pedEsp.idPedido)
                        Inner Join pedido ped On (pp.idPedido=ped.idPedido) 
                        Inner Join cliente cli On (ped.idCli=cli.id_cli) 
                        Left Join rota_cliente rc On (cli.id_Cli=rc.idCliente) 
                        Left Join rota r On (rc.idRota=r.idRota) 
                        Left Join produto p On (pp.idProd=p.idProd) 
                        Left Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd) 
                        Left Join subgrupo_prod s On (p.idSubgrupoProd=s.idSubgrupoProd) 
                        Left Join cor_vidro cv On (p.idCorVidro=cv.idCorVidro) 
                        Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao) 
                        Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso) 
                        LEFT JOIN produtos_pedido_espelho pp1 ON (pp.IdProdPedParent = pp1.IdProdPed)
                        LEFT JOIN produtos_pedido_espelho pp2 ON (pp1.IdProdPedParent = pp2.IdProdPed)
                    Where (pp.invisivelFluxo=false or pp.invisivelFluxo is null) 
                        and COALESCE(pp1.Qtde, 1) * COALESCE(pp2.Qtde, 1) * pp.qtde > pp.qtdImpresso" +
                        " And ped.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + filtroApenasVidros + where + " group by pp.idProdPed";
            }

            if (pecasRepostas == 1 || pecasRepostas == 2)
            {
                if (!String.IsNullOrEmpty(sql))
                    sql += " union all ";

                sql += "Select " + campos + @", group_concat(ppp.numEtiqueta separator '_') as numEtiqueta, 
                        true as pecaReposta, cast(count(*) as signed) as qtdeReposta
                    From produtos_pedido_espelho pp 
                        Inner Join produto_pedido_producao ppp on (pp.idProdPed=ppp.idProdPed and ppp.pecaReposta)
                        Left Join leitura_producao lp on (ppp.idProdPedProducao=lp.idProdPedProducao)
                        Inner Join pedido_espelho pedEsp On (pp.idPedido=pedEsp.idPedido)
                        Inner Join pedido ped On (pp.idPedido=ped.idPedido) 
                        Inner Join cliente cli On (ped.idCli=cli.id_cli) 
                        Left Join rota_cliente rc On (cli.id_Cli=rc.idCliente)
                        Left Join rota r On (rc.idRota=r.idRota)  
                        Left Join produto p On (pp.idProd=p.idProd) 
                        Left Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd) 
                        Left Join subgrupo_prod s On (p.idSubgrupoProd=s.idSubgrupoProd) 
                        Left Join cor_vidro cv On (p.idCorVidro=cv.idCorVidro) 
                        Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao) 
                        Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso) 
                    Where (invisivelFluxo=false or invisivelFluxo is null) and lp.idLeituraProd is null
                        And ped.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + " And (p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @"
                        and (ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + " Or " + produtoParaVenda + "))" + where + " group by pp.idProdPed";
            }
            
            if (maoDeObra == 1)
            {
                string camposUnion = selecionar ? @"pp.idProdPed, pp.idPedido, ape.idProd, pp.idItemProjeto, pp.idMaterItemProj, 
                    pp.idAmbientePedido, pp.idAplicacao, pp.idProcesso, cast(ape.qtde as signed) as qtde, pp.valorVendido, 
                    cast(ape.altura as signed) as altura, cast(ape.altura as signed) as alturaReal, ape.largura, ape.largura as larguraReal, 
                    pp.totM, pp.totM2Calc, ape.qtdeImpresso, pp.total, pp.aliqIcms, pp.valorIcms, ape.ambiente, pp.obs,
                    ape.redondo, pp.valorBenef, pp.pedCli, pp.alturaBenef, pp.larguraBenef, pp.espBenef, pp.valorAcrescimo, 
                    pp.valorDesconto, pp.valorAcrescimoProd, pp.valorDescontoProd, pp.percDescontoQtde, pp.valorDescontoQtde, 
                    ape.Ambiente as DescrProduto, p.CodInterno, p.IdGrupoProd, p.idSubgrupoProd, apl.CodInterno as CodAplicacao, 
                    prc.CodInterno as CodProcesso, null, null, null, null, null, null, null, null,ped.dataEntrega, ped.dataPedido,
                    cli.nome as nomeCliente, null as QtdeInvisivel, r.codInterno as RotaCliente, cv.idCorVidro, cv.Descricao as Cor, pp.Espessura, pp.Peso, 
                    pp.valorComissao, pedEsp.dataFabrica, pp.IdProdPedParent" : "count(*) as num";

                if (pecasRepostas != 2)
                {
                    if (!String.IsNullOrEmpty(sql))
                        sql += " union all ";

                    sql += "Select " + camposUnion + @", null as numEtiqueta, false as pecaReposta, cast(0 as signed) as qtdeReposta
                        From produtos_pedido_espelho pp 
                            Inner Join pedido_espelho pedEsp On (pp.idPedido=pedEsp.idPedido)
                            Inner Join pedido ped On (pp.idPedido=ped.idPedido) 
                            Inner Join cliente cli On (ped.idCli=cli.id_cli) 
                            Inner Join ambiente_pedido_espelho ape On (pp.idAmbientePedido=ape.idAmbientePedido) 
                            Left Join rota_cliente rc On (cli.id_Cli=rc.idCliente) 
                            Left Join rota r On (rc.idRota=r.idRota) 
                            Left Join produto p On (ape.idProd=p.idProd) 
                            Left Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd) 
                            Left Join subgrupo_prod s On (p.idSubgrupoProd=s.idSubgrupoProd) 
                            Left Join cor_vidro cv On (p.idCorVidro=cv.idCorVidro) 
                            Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao) 
                            Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso) 
                        Where (invisivelFluxo=false or invisivelFluxo is null) 
                            And ape.qtde > ape.qtdeImpresso And ped.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + @"
                            And ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + where + " Group by pp.idAmbientePedido";
                }

                if (pecasRepostas == 1 || pecasRepostas == 2)
                {
                    if (!String.IsNullOrEmpty(sql))
                        sql += " union all ";

                    sql += "Select " + camposUnion + @", group_concat(ppp.numEtiqueta separator '_') as numEtiqueta, 
                            true as pecaReposta, cast(count(*) as signed) as qtdeReposta
                        From produtos_pedido_espelho pp 
                            Inner Join produto_pedido_producao ppp on (pp.idProdPed=ppp.idProdPed and ppp.pecaReposta)
                            Left Join leitura_producao lp on (ppp.idProdPedProducao=lp.idProdPedProducao)
                            Inner Join pedido_espelho pedEsp On (pp.idPedido=pedEsp.idPedido)
                            Inner Join pedido ped On (pp.idPedido=ped.idPedido) 
                            Inner Join cliente cli On (ped.idCli=cli.id_cli) 
                            Inner Join ambiente_pedido_espelho ape On (pp.idAmbientePedido=ape.idAmbientePedido) 
                            Left Join rota_cliente rc On (cli.id_Cli=rc.idCliente) 
                            Left Join rota r On (rc.idRota=r.idRota) 
                            Left Join produto p On (ape.idProd=p.idProd) 
                            Left Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd) 
                            Left Join subgrupo_prod s On (p.idSubgrupoProd=s.idSubgrupoProd) 
                            Left Join cor_vidro cv On (p.idCorVidro=cv.idCorVidro) 
                            Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao) 
                            Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso) 
                        Where (invisivelFluxo=false or invisivelFluxo is null) and lp.idLeituraProd is null
                            And ped.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + @"
                            And ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + where + " Group by pp.idAmbientePedido";
                }
            }

            if (!selecionar)
                sql = "select Coalesce(sum(num), 0) from (" + sql + ") as temp";

            // Caso a peça tenha várias reposições, pode ser necessário aumentar o tamanho 
            // do retorno dado pelo group_concat, chamado 8322
            if (pecasRepostas == 1 || pecasRepostas == 2)
                sql = "SET SESSION group_concat_max_len = 1000000;" + sql;

            // Chamado 11754. o comando "SET group_concat_max_len = 4096;", é importante que este comando seja executado
            // para que o group_concat, feito para retornar as etiquetas do produto espelho, busque todas as etiquetas.
            return "SET group_concat_max_len = 4096; " + sql;
        }

        public bool IsProdToEtiq(uint idProdPed)
        {
            string sql = SqlProdEtiq(0, idProdPed, null, 0, 0, 0, null, null, null, null, null, null, false, null, 0, false,
                0, 0, 0, 0, 0, null, false);
            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Busca produtos do grupo vidro relacionados ao pedido, que aida não tenham sido totalmente impressos,
        /// se o usuário logado for Aux. Etiqueta, busca apenas vidros comuns
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<ProdutosPedidoEspelho> GetProdToEtiq(uint idPedido, uint idProcesso, uint idAplicacao, uint idCorVidro,
            float espessura, uint idSubgrupoProd, float alturaMin, float alturaMax, int larguraMin, int larguraMax, int? idLoja)
        {
            string codProc = EtiquetaProcessoDAO.Instance.ObtemCodInterno(idProcesso);
            string codApl = EtiquetaAplicacaoDAO.Instance.ObtemCodInterno(idAplicacao);

            return objPersistence.LoadData(SqlProdEtiq(idPedido, 0, null, idCorVidro, espessura, 1, null, null, null, null, 
                codProc, codApl, false, 1, 0, false, idSubgrupoProd, alturaMin, alturaMax, larguraMin,
                larguraMax, null, null, true, idLoja), GetParam(null, null, null, null, null, codProc, codApl)).ToList();
        }

        /// <summary>
        /// Verifica se o pedido possui peças a serem impressas
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiPecaASerImpressa(GDASession sessao, uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, SqlProdEtiq(idPedido, 0, null, 0, 0, 1, null, null, null, null, null, 
                null, false, null, 0, false, 0, 0, 0, 0, 0, null, false)) > 0;
        }

        /// <summary>
        /// Verifica se o pedido possui peças a serem impressas
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiPecaASerImpressa(uint idPedido)
        {
            return PossuiPecaASerImpressa(null, idPedido);
        }

        /// <summary>
        /// Retorna todas as etiquetas utilizando os filtros passados ordenadas pelas cor/espessura
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="descrProd"></param>
        /// <param name="maoDeObra">Identifica se produtos de pedido mão de obra devem ser buscados</param>
        /// <returns></returns>
        public IList<ProdutosPedidoEspelho> GetEtiqOrdered(uint idPedido, string descrProd, uint idCorVidro, float espessura, int maoDeObra,
            string dataIni, string dataFim, string dataFabricaIni, string dataFabricaFim, string codProcesso, string codAplicacao, int? pecasRepostas)
        {
            return objPersistence.LoadData("Select * From (" + SqlProdEtiq(idPedido, 0, descrProd, idCorVidro, espessura, maoDeObra,
                dataIni, dataFim, dataFabricaIni, dataFabricaFim, codProcesso, codAplicacao, false, pecasRepostas, 0,
                true, 0, 0, 0, 0, 0, null, true) +
                ") as tbl Order By idCorVidro, espessura, IdPedido", GetParam(descrProd, dataIni, dataFim, dataFabricaIni, dataFabricaFim, codProcesso, codAplicacao)).ToList();
        }

        public IList<ProdutosPedidoEspelho> GetListEtiq(uint idPedido, string descrProd, uint idCorVidro, float espessura, int maoDeObra, string dataIni,
            string dataFim, string dataFabricaIni, string dataFabricaFim, string codProcesso, string codAplicacao, int? pecasRepostas, string idsRotas,
            int? fastDelivery, int? idLoja, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlProdEtiq(idPedido, 0, descrProd, idCorVidro, espessura, maoDeObra, dataIni, dataFim,
                dataFabricaIni, dataFabricaFim, codProcesso, codAplicacao, false, pecasRepostas, 0, true, 0, 0,
                0, 0, 0, idsRotas, fastDelivery, true, idLoja), sortExpression, startRow, pageSize,
                GetParam(descrProd, dataIni, dataFim, dataFabricaIni, dataFabricaFim, codProcesso, codAplicacao));
        }

        public int GetCountEtiq(uint idPedido, string descrProd, uint idCorVidro, float espessura, int maoDeObra,
            string dataIni, string dataFim, string dataFabricaIni, string dataFabricaFim, string codProcesso, string codAplicacao, int? pecasRepostas,
            string idsRotas, int? fastDelivery, int? idLoja)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlProdEtiq(idPedido, 0, descrProd, idCorVidro, espessura, maoDeObra, dataIni, dataFim, dataFabricaIni,
                dataFabricaFim, codProcesso, codAplicacao, false, pecasRepostas, 0, true, 0, 0, 0, 0, 0, idsRotas, fastDelivery, false, idLoja),
                GetParam(descrProd, dataIni, dataFim, dataFabricaIni, dataFabricaFim, codProcesso, codAplicacao));
        }

        private GDAParameter[] GetParam(string descrProd, string dataIni, string dataFim, string dataFabricaIni, 
            string dataFabricaFim, string codProcesso, string codAplicacao)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(descrProd))
                lstParam.Add(new GDAParameter("?descrProd", "%" + descrProd + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(dataFabricaIni))
                lstParam.Add(new GDAParameter("?dataFabricaIni", DateTime.Parse(dataFabricaIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFabricaFim))
                lstParam.Add(new GDAParameter("?dataFabricaFim", DateTime.Parse(dataFabricaFim + " 23:59")));

            if (!String.IsNullOrEmpty(codProcesso))
                lstParam.Add(new GDAParameter("?codProc", codProcesso));

            if (!String.IsNullOrEmpty(codAplicacao))
                lstParam.Add(new GDAParameter("?codApl", codAplicacao));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        public IList<ProdutosPedidoEspelho> ObterFilhosComposicaoByEtiqueta(GDASession sessao, string etiqueta, bool somenteVidroPVB)
        {
            var sql = @"
                SELECT ppe.*, p.CodInterno, p.Descricao as DescrProduto
                FROM produto_pedido_producao ppp
                    INNER JOIN produtos_pedido_espelho ppe ON (ppp.IdProdPed = ppe.IdProdPedParent)
                    INNER JOIN produto p ON (p.IdProd = ppe.IdProd)
                    LEFT JOIN grupo_prod gp ON (gp.IdGrupoProd = p.IdGrupoProd)
                    LEFT JOIN subgrupo_prod sgp ON (sgp.IdSubgrupoProd = p.IdSubgrupoProd)
                WHERE ppp.numEtiqueta = ?etq";

            if (somenteVidroPVB)
                sql += " AND (gp.IdGrupoProd = " + (int)NomeGrupoProd.Vidro + " OR sgp.TipoSubgrupo = " + (int)TipoSubgrupoProd.PVB + ")";

            return objPersistence.LoadData(sessao, sql, new GDAParameter("?etq", etiqueta)).ToList();
        }

        public IList<ProdutosPedidoEspelho> ObterFilhosComposicao(GDASession sessao, uint idProdPed)
        {
            var sql = @"
                SELECT ppe.*, p.CodInterno, p.Descricao as DescrProduto
                FROM produtos_pedido_espelho ppe
                    INNER JOIN produto p ON (p.IdProd = ppe.IdProd)
                WHERE ppe.IdProdPedParent = ?idProdPed";

            return objPersistence.LoadData(sessao, sql, new GDAParameter("?idProdPed", idProdPed)).ToList();
        }

        public bool PossuiFilhosComposicao(GDASession sessao, uint idProdPed)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, "SELECT COUNT(*) FROM produtos_pedido_espelho WHERE IdProdPedParent = " + idProdPed) > 0;
        }

        #endregion

        #region Busca produtos para gerar etiquetas na finalização do pedido espelho

        /// <summary>
        /// Busca produtos para gerar etiquetas na finalização do pedido espelho
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<int> ObterIdsParaImpressaoFinalizacaoPCP(GDASession sessao, int idPedido)
        {
            var produtoParaVenda = "COALESCE(s.TipoCalculo, g.TipoCalculo, " + (int)TipoCalculoGrupoProd.Qtd + ") <>" + (int)TipoCalculoGrupoProd.Qtd + " OR COALESCE(s.ProdutosEstoque, 0) = 0";

            var sql = @"
                SELECT pp.IdProdPed
                FROM produtos_pedido_espelho pp
                     INNER JOIN pedido ped On (pp.idPedido=ped.idPedido) 
                     LEFT JOIN produto p On (pp.idProd = p.idProd) 
                     LEFT JOIN grupo_prod g On (p.idGrupoProd = g.idGrupoProd) 
                     LEFT JOIN subgrupo_prod s On (p.idSubgrupoProd = s.idSubgrupoProd) 
                WHERE pp.IdPedido=" + idPedido;

            return ExecuteMultipleScalar<int>(sessao, sql);
        }

        /// <summary>
        /// Busca produtos para gerar etiquetas na finalização do pedido espelho
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObterQtdePecasParaImpressaoFinalizacaoPCP(GDASession sessao, int idPedido)
        {
            // var produtoParaVenda = "(COALESCE(s.TipoCalculo, g.TipoCalculo, " + (int)TipoCalculoGrupoProd.Qtd + ") <>" + (int)TipoCalculoGrupoProd.Qtd + " OR COALESCE(s.ProdutosEstoque, 0) = 0)";
            var produtoParaVenda = " AND g.IdGrupoProd = " + (int)NomeGrupoProd.Vidro;

            var sql = @"              
                            SELECT SUM(qtde) 
                            FROM
                            (
                                SELECT pp.Qtde + (pp.Qtde * (SUM(COALESCE(pp1.Qtde, 0)) + SUM(COALESCE(pp1.Qtde, 0) * COALESCE(child.Qtde, 0)))) as Qtde
                                FROM produtos_pedido_espelho pp
								    INNER JOIN pedido ped On (pp.idPedido=ped.idPedido) 
								    LEFT JOIN produto p On (pp.idProd = p.idProd) 
								    LEFT JOIN grupo_prod g On (p.idGrupoProd = g.idGrupoProd) 
								    LEFT JOIN subgrupo_prod s On (p.idSubgrupoProd = s.idSubgrupoProd) 
								    LEFT JOIN 
                                    (
                                        SELECT pp.IdProdPed, pp.IdProdPedParent, SUM(pp.Qtde) as Qtde
									    FROM produtos_pedido_espelho pp
                                            INNER JOIN pedido ped On (pp.idPedido=ped.idPedido) 
								            LEFT JOIN produto p On (pp.idProd = p.idProd) 
								            LEFT JOIN grupo_prod g On (p.idGrupoProd = g.idGrupoProd) 
								            LEFT JOIN subgrupo_prod s On (p.idSubgrupoProd = s.idSubgrupoProd) 
									    WHERE pp.IdPedido = " + idPedido + @" 
                                            AND pp.IdProdPedParent IS NOT NULL 
                                            AND NOT EXISTS (SELECT * FROM produtos_pedido_espelho WHERE IdProdPedParent IS NOT NULL AND IdProdPed = pp.IdProdPedParent) " + produtoParaVenda + @"
                                        GROUP BY pp.IdProdPed
                                    ) as pp1 ON (pp.IdProdPed = pp1.IdProdPedParent) 
								    LEFT JOIN 
								    (
									    SELECT pp.IdProdPedParent, SUM(pp.Qtde) as Qtde
									    FROM produtos_pedido_espelho pp
                                            INNER JOIN pedido ped On (pp.idPedido=ped.idPedido) 
								            LEFT JOIN produto p On (pp.idProd = p.idProd) 
								            LEFT JOIN grupo_prod g On (p.idGrupoProd = g.idGrupoProd) 
								            LEFT JOIN subgrupo_prod s On (p.idSubgrupoProd = s.idSubgrupoProd) 
										    LEFT JOIN produtos_pedido_espelho pp1 ON (pp.IdProdPedParent = pp1.IdProdPed)
									    WHERE pp.IdPedido = " + idPedido + @" AND pp.IdProdPedParent IS NOT NULL AND pp1.IdProdPedParent IS NOT NULL " + produtoParaVenda + @"
                                        GROUP BY pp.IdProdPedParent
								    ) as child ON (pp1.IdProdPed = child.IdProdPedParent)
                                WHERE pp.IdPedido = " + idPedido + @" AND pp.IdProdPedParent IS NULL " + produtoParaVenda + @"
                                GROUP BY pp.IdProdPed
                            ) as tmp";

            return ExecuteScalar<int>(sessao, sql);
        }

        /// <summary>
        /// Obtem a quantidade que pode ser impressa de um produto de composição
        /// So pode ser impresso quando os filhos estiverem prontos.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public int ObterQtdePecasParaImpressaoComposicao(int idProdPedParent)
        {
            var sql = @"
                SELECT MIN(qtde)
                FROM (
                        SELECT TRUNCATE((SUM(IF(ppp.SituacaoProducao = "+ (int)SituacaoProdutoProducao.Pronto + @", 1, 0)) / ppe.Qtde), 0) as qtde
                        FROM produtos_pedido_espelho ppe
                            INNER JOIN produto_pedido_producao ppp ON (ppe.IdProdPed = ppp.IdProdPed)
                        WHERE ppp.Situacao = " + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @" AND ppe.IdProdPedParent = " + idProdPedParent + @"
                        GROUP BY ppe.IdProdPed
                     ) as tmp";

            return ExecuteScalar<int>(sql);
        }

        #endregion

        #region Busca impressões pendentes

        public IList<ProdutosPedidoEspelho> GetImprPendRpt(uint idPedido, uint idCorVidro, int espessura, string dataIni, string dataFim, bool buscarPecasDeBox,
            bool buscarPecasRepostas, uint idRota, string codProcesso, string codAplicacao, int? idLoja, string sortExpression)
        {
            int? pecasRepostas = buscarPecasRepostas ? 1 : (int?)null;

            return LoadDataWithSortExpression(SqlProdEtiq(idPedido, 0, null, idCorVidro, espessura, 1, dataIni, dataFim, null, null, codProcesso, codAplicacao,
                buscarPecasDeBox, pecasRepostas, idRota, false, 0, 0, 0, 0, 0, null, null, true, idLoja), sortExpression, 0, 0,
                GetParam(null, dataIni, dataFim, null, null, codProcesso, codAplicacao));
        }

        public IList<ProdutosPedidoEspelho> GetListImprPend(uint idPedido, uint idCorVidro, int espessura, string dataIni, string dataFim, bool buscarPecasDeBox,
            bool buscarPecasRepostas, uint idRota, string codProcesso, string codAplicacao, int? idLoja, string sortExpression, int startRow, int pageSize)
        {
            int? pecasRepostas = buscarPecasRepostas ? 1 : (int?)null;

            return LoadDataWithSortExpression(SqlProdEtiq(idPedido, 0, null, idCorVidro, espessura, 1, dataIni, dataFim, null, null, codProcesso, codAplicacao,
                buscarPecasDeBox, pecasRepostas, idRota, false, 0, 0, 0, 0, 0, null, null, true, idLoja), sortExpression, startRow, pageSize, 
                GetParam(null, dataIni, dataFim, null, null, codProcesso, codAplicacao));
        }

        public int GetCountImprPend(uint idPedido, uint idCorVidro, int espessura, string dataIni, string dataFim, bool buscarPecasDeBox, bool buscarPecasRepostas, uint idRota, string codProcesso, string codAplicacao, int? idLoja)
        {
            int? pecasRepostas = buscarPecasRepostas ? 1 : (int?)null;

            object obj = objPersistence.ExecuteScalar(SqlProdEtiq(idPedido, 0, null, idCorVidro, espessura, 1, dataIni, dataFim, null, null, null, null,
                buscarPecasDeBox, pecasRepostas, idRota, false, 0, 0, 0, 0, 0, null, null, false, idLoja), GetParam(null, dataIni, dataFim, null, null, codProcesso, codAplicacao));

            return obj == null || obj.ToString() == string.Empty ? 0 : Conversoes.StrParaInt(obj.ToString());
        }

        #endregion

        #region Busca produtos para impressão individual

        private string SqlImpIndiv(uint idPedido, uint numeroNFe, string numEtiqueta, string descrProd,
            int alturaIni, int alturaFim, int larguraIni, int larguraFim, string codProcesso, string codAplicacao, bool selecionar)
        {
            uint idProdPed = 0, idProdNf = 0;

            if (!String.IsNullOrEmpty(numEtiqueta))
            {
                if (numEtiqueta.ToUpper()[0] == 'N')
                    idProdNf = ProdutosNfDAO.Instance.GetIdByEtiquetaFast(null, numEtiqueta);
                else
                    idProdPed = GetIdProdPedByEtiqueta(numEtiqueta, true);
            }

            string campos = selecionar ? @"prod.idProdPed, prod.idProdNf, prod.idPedido, prod.numeroNFe, prod.idProd,
                prod.idAplicacao, prod.idProcesso, prod.qtde, prod.altura, prod.alturaReal, prod.largura,
                prod.larguraReal, prod.qtdImpresso, prod.redondo, p.Descricao as DescrProduto, p.CodInterno,
                p.IdGrupoProd, p.idSubgrupoProd, apl.CodInterno as CodAplicacao, prc.CodInterno as CodProcesso, 
                null as idItemProjeto, null as idMaterItemProj, null as idAmbientePedido, idAmbientePedidoImpr,
                null as valorVendido, null as totM, null as totM2Calc, null as total, null as aliqIcms, 
                null as valorIcms, null as aliquotaIpi, null as valorIpi, null as ambiente, null as obs, 
                null as valorBenef, null as pedCli, null as alturaBenef, null as larguraBenef, null as espBenef, null as valorAcrescimo,
                null as valorDesconto, null as valorAcrescimoProd, null as valorDescontoProd, null as forma,
                null as percDescontoQtde, null as valorDescontoQtde, null as valorDescontoCliente, null as valorAcrescimoCliente,
                null as invisivelFluxo, null as invisivelAdmin, null as valorUnitBruto, null as totalBruto,
                null as qtdeInvisivel, null as valorComissao, CAST(Coalesce(prod.espessura, p.espessura) AS DECIMAL(12,2)) as espessura, prod.peso" : "count(*)";

            string camposProdPed = selecionar ? @"idProdPed, null as idProdNf, idPedido, null as numeroNFe, idProd, 
                idAplicacao, idProcesso, qtde, altura, alturaReal, largura, larguraReal, qtdImpresso, redondo, 0 as idAmbientePedidoImpr, espessura, peso" :
                "idProdPed, idProd, idAplicacao, idProcesso";

            string camposAmbProdPed = selecionar ? @"null as idProdPed, null as idProdNf, idPedido, null as numeroNFe, idProd, 
                idAplicacao, idProcesso, cast(qtde as signed) as qtde, cast(altura as signed), cast(altura as signed) as alturaReal, largura, 
                largura as larguraReal, qtdeImpresso as qtdImpresso, redondo, idAmbientePedido as idAmbientePedidoImpr, null as espessura, null as peso" :
                "null as idProdPed, idProd, idAplicacao, idProcesso";

            string camposProdNf = selecionar ? @"null, idProdNf, null, nf.numeroNFe, idProd, null, null, qtde,
                altura, altura, largura, largura, qtdImpresso, null, 0 as idAmbientePedidoImpr, null as espessura, peso" : 
                "idProdNf, idProd, null, null";

            string sql = "Select " + campos + @"
                From (
                    select " + camposProdPed + @"
                    from produtos_pedido_espelho
                    where qtdImpresso>0 and coalesce(invisivelFluxo, false)=false {0}

                    union all select " + camposAmbProdPed + @"
                    from ambiente_pedido_espelho
                    where qtdeImpresso>0 {1}

                    union all select " + camposProdNf + @"
                    from produtos_nf pnf
                        inner join nota_fiscal nf on (pnf.idNf=nf.idNf)
                    where qtdImpresso>0 and nf.situacao=" + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros + @" {2}
                ) prod
                    Left Join produto p On (prod.idProd=p.idProd) 
                    Left Join etiqueta_aplicacao apl On (prod.idAplicacao=apl.idAplicacao) 
                    Left Join etiqueta_processo prc On (prod.idProcesso=prc.idProcesso) 
                Where p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro;

            string wherePp = "", whereApp = "", wherePnf = "";

            if (idPedido > 0)
            {
                wherePp += " And idPedido=" + idPedido;
                whereApp += " And idPedido=" + idPedido;
                wherePnf += " And false";
            }

            if (numeroNFe > 0)
            {
                wherePnf += " And nf.numeroNFe=" + numeroNFe;
                wherePp += " And false";
                whereApp += " And false";
            }

            if (idProdPed > 0)
            {
                wherePp += " And idProdPed=" + idProdPed;
                wherePnf += " And false";

                uint? idAmbientePedido = ProdutosPedidoEspelhoDAO.Instance.ObtemIdAmbientePedido(idProdPed);

                if (idAmbientePedido > 0)
                    whereApp += " And idAmbientePedido=" + idAmbientePedido;
                else
                    whereApp += " And false";
            }
            else if (!String.IsNullOrEmpty(numEtiqueta))
            {
                wherePp += " And 0>1";
                whereApp += " And idPedido=" + idPedido;
            }

            if (idProdNf > 0)
                wherePnf += " And idProdNf=" + idProdNf;
            else if (!String.IsNullOrEmpty(numEtiqueta))
                wherePnf += " And 0>1";
            
            if (!String.IsNullOrEmpty(descrProd))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descrProd);
                wherePp += " And idProd In (" + ids + ")";
                whereApp += " And idProd In (" + ids + ")";
                wherePnf += " And idProd In (" + ids + ")";
            }

            if (alturaIni > 0)
            {
                wherePp += " And altura>=" + alturaIni;
                whereApp += " And altura>=" + alturaIni;
                wherePnf += " And altura>=" + alturaIni;
            }

            if (alturaFim > 0)
            {
                wherePp += " And altura<=" + alturaFim;
                whereApp += " And altura<=" + alturaFim;
                wherePnf += " And altura<=" + alturaFim;
            }

            if (larguraIni > 0)
            {
                wherePp += " And largura>=" + larguraIni;
                whereApp += " And largura>=" + larguraIni;
                wherePnf += " And largura>=" + larguraIni;
            }

            if (larguraFim > 0)
            {
                wherePp += " And largura<=" + larguraFim;
                whereApp += " And largura<=" + larguraFim;
                wherePnf += " And largura<=" + larguraFim;
            }

            if (!string.IsNullOrEmpty(codProcesso) && codProcesso != "0")
                sql += " AND prc.codInterno=?codProc";

            if (!string.IsNullOrEmpty(codAplicacao) && codAplicacao != "0")
                sql += " AND apl.codInterno=?codApl";

            return String.Format(sql, wherePp, whereApp, wherePnf);
        }

        public IList<ProdutosPedidoEspelho> GetListImpIndiv(uint idPedido, uint numeroNFe, string numEtiqueta, string descrProd, int alturaIni,
            int alturaFim, int larguraIni, int larguraFim, string codProcesso, string codAplicacao, string sortExpression, int startRow, int pageSize)
        {
            if (FiltroImpIndivVazio(idPedido, numeroNFe, numEtiqueta, descrProd, alturaIni, alturaFim, larguraIni, larguraFim, codProcesso, codAplicacao))
                return null;

            sortExpression = string.IsNullOrEmpty(sortExpression) ? "IdPedido DESC, NumeroNfe DESC" : sortExpression;

            return LoadDataWithSortExpression(SqlImpIndiv(idPedido, numeroNFe, numEtiqueta, descrProd, alturaIni, alturaFim, larguraIni, larguraFim,
                codProcesso, codAplicacao, true), sortExpression, startRow, pageSize, GetParamImpIndiv(descrProd, codProcesso, codAplicacao));
        }

        public int GetCountImpIndiv(uint idPedido, uint numeroNFe, string numEtiqueta, string descrProd, int alturaIni,
            int alturaFim, int larguraIni, int larguraFim, string codProcesso, string codAplicacao)
        {
            if (FiltroImpIndivVazio(idPedido, numeroNFe, numEtiqueta, descrProd, alturaIni, alturaFim, larguraIni, larguraFim, codProcesso, codAplicacao))
                return 0;

            return objPersistence.ExecuteSqlQueryCount(SqlImpIndiv(idPedido, numeroNFe, numEtiqueta, descrProd, alturaIni, alturaFim, larguraIni, larguraFim,
                codProcesso, codAplicacao, false), GetParamImpIndiv(descrProd, codProcesso, codAplicacao));
        }

        private bool FiltroImpIndivVazio(uint idPedido, uint numeroNFe, string numEtiqueta, string descrProd, int alturaIni, int alturaFim,
            int larguraIni, int larguraFim, string codProcesso, string codAplicacao)
        {
            return idPedido == 0 && numeroNFe == 0 && String.IsNullOrEmpty(numEtiqueta) && String.IsNullOrEmpty(descrProd) &&
                alturaIni == 0 && alturaFim == 0 && larguraIni == 0 && larguraFim == 0 && String.IsNullOrEmpty(codProcesso) && String.IsNullOrEmpty(codAplicacao);
        }

        private GDAParameter[] GetParamImpIndiv(string descrProd, string codProcesso, string codAplicacao)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(descrProd))
                lstParam.Add(new GDAParameter("?descrProd", "%" + descrProd + "%"));

            if (!String.IsNullOrEmpty(codProcesso))
                lstParam.Add(new GDAParameter("?codProc", codProcesso));

            if (!String.IsNullOrEmpty(codAplicacao))
                lstParam.Add(new GDAParameter("?codApl", codAplicacao));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Retorna a posição de um prodPed ou prodPed de uma posição

        private string SqlPosicaoProdPed(uint idPedido, int posicao, bool buscarSomenteVisiveis, bool inverterCampos)
        {
            // Ignora as etiquetas que foram canceladas
            string sqlApenasVisiveis = "";
            if (buscarSomenteVisiveis)
                sqlApenasVisiveis = @"Left Join (
                    select {0}, @offset := @offset + (qtde=0) as offset
                    from (
                        select p1.{0}, Coalesce({3}, 0) as qtde
                        from {1} p1
                            inner join produto p2 on (p1.idProd=p2.idProd)
                            {2}
                        where p1.idPedido=" + idPedido + @" and p2.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @"
                        group by p1.{0}
                        order by p1.{0} Asc
                    ) as temp
                ) as o on (pp.{0}=o.{0})";

            string[] campos = new string[] {
                "temp.idProdPed",
                "(@linha := @linha + 1)" + (buscarSomenteVisiveis ? " + temp.offset" : "") + " as linha"
            };

            string campo = !inverterCampos ? campos[0] + ", " + campos[1] : campos[1] + ", " + campos[0];

            string sql = @"
                set @linha := 0, @offset := 0;
                Select " + campo + @"
                from (
                    select pp.idProdPed, pp.idAmbientePedido" + (buscarSomenteVisiveis ? ", o.offset" : "") + @"
                    From produtos_pedido_espelho pp 
                        Inner Join produto p On (pp.idProd=p.idProd)
                        Inner Join pedido ped On (pp.idPedido=ped.idPedido) 
                        " + sqlApenasVisiveis + @"
                    Where pp.idPedido=" + idPedido + (buscarSomenteVisiveis ? @" and coalesce(pp.invisivelFluxo, false)=false 
                        and coalesce(pp.invisivelAdmin, false)=false" : "") + " And (p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro +
                        " Or ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @")
                    Group By pp.{0} Order by pp.{0} Asc
                ) as temp
                Group by {0}";

            if (posicao > 0)
                sql += " Having linha=" + posicao;

            bool isMaoDeObra = PedidoDAO.Instance.IsMaoDeObra(idPedido);
            return String.Format(sql, isMaoDeObra ? "idAmbientePedido" : "idProdPed",
                isMaoDeObra ? "ambiente_pedido_espelho" : "produtos_pedido_espelho",

                // Este join deve ser feito com left pois caso algum ambiente tenha sido removido do produto, os produtos
                // associados ao mesmo são excluídos, fazendo com que o join não buscasse o ambiente utilizando inner join
                // e calculasse incorretamente a posição da etiqueta
                isMaoDeObra ? "left join produtos_pedido_espelho p3 on (p1.idAmbientePedido=p3.idAmbientePedido)" : "",
                isMaoDeObra ? "sum(p3.qtde)" : "p1.qtde");
        }

        /// <summary>
        /// Retorna a posição de um produto no pedido, do grupo vidro ou de pedido mão de obra
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public int GetProdPosition(uint idPedido, uint idProdPed)
        {
            return GetProdPosition(null, idPedido, idProdPed);
        }

        /// <summary>
        /// Retorna a posição de um produto no pedido, do grupo vidro ou de pedido mão de obra
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public int GetProdPosition(GDASession session, uint idPedido, uint idProdPed)
        {
            return ExecuteScalar<int>(session, SqlPosicaoProdPed(idPedido, 0, true, true) + 
                " Having idProdPed=" + idProdPed);
        }

        /// <summary>
        /// Retorna um produto pedido a partir da posição do mesmo no pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public uint GetIdProdPedByEtiqueta(string codEtiqueta)
        {
            return GetIdProdPedByEtiqueta(codEtiqueta, false);
        }
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna um produto pedido a partir da posição do mesmo no pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public uint GetIdProdPedByEtiqueta(string codEtiqueta, bool buscarSomenteVisiveis)
        {
            return GetIdProdPedByEtiqueta(null, codEtiqueta, buscarSomenteVisiveis);
        }

        /// <summary>
        /// Retorna um produto pedido a partir da posição do mesmo no pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public uint GetIdProdPedByEtiqueta(GDASession sessao, string codEtiqueta, bool buscarSomenteVisiveis)
        {
            if (codEtiqueta.IndexOf('-') <= 0)
                throw new Exception("Produto da etiqueta não encontrado. Etiqueta: " + codEtiqueta);

            // Pega o idPedido pelo código da etiqueta
            uint idPedido = Glass.Conversoes.StrParaUint(codEtiqueta.Substring(0, codEtiqueta.IndexOf('-')));

            // Pega a posição do produto no pedido pelo código da etiqueta
            int posicao = Glass.Conversoes.StrParaInt(codEtiqueta.Substring(codEtiqueta.IndexOf('-') + 1, codEtiqueta.IndexOf('.') - codEtiqueta.IndexOf('-') - 1));

            var lstProd = objPersistence.LoadResult(sessao, SqlPosicaoProdPed(idPedido, posicao, buscarSomenteVisiveis, false), null).Select(f => f.GetUInt32(0)).ToList();

            if (lstProd.Count == 0)
                throw new Exception("Produto da etiqueta não encontrado. Etiqueta: " + codEtiqueta);

            return lstProd[0];
        }

        /// <summary>
        /// Retorna um produto pedido a partir da posição do mesmo no pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public ProdutosPedidoEspelho GetProdPedByEtiqueta(string codEtiqueta)
        {
            return GetProdPedByEtiqueta(codEtiqueta, null, false);
        }
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna um produto pedido a partir da posição do mesmo no pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public ProdutosPedidoEspelho GetProdPedByEtiqueta(string codEtiqueta, uint? idProdPed, bool buscarSomenteVisiveis)
        {
            return GetProdPedByEtiqueta(null, codEtiqueta, idProdPed, buscarSomenteVisiveis);
        }

        /// <summary>
        /// Retorna um produto pedido a partir da posição do mesmo no pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public ProdutosPedidoEspelho GetProdPedByEtiqueta(GDASession sessao, string codEtiqueta, uint? idProdPed, bool buscarSomenteVisiveis)
        {
            string sql = @"
                select pp.*, p.Descricao as DescrProduto, p.CodInterno, p.IdGrupoProd, p.idSubgrupoProd, 
                    cv.Descricao as Cor, apl.CodInterno as CodAplicacao, prc.CodInterno as CodProcesso
                From produtos_pedido_espelho pp 
                    Inner Join produto p On (pp.idProd=p.idProd) 
                    Left Join cor_vidro cv On (p.idCorVidro=cv.idCorVidro) 
                    Inner Join pedido ped On (pp.idPedido=ped.idPedido) 
                    Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao) 
                    Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso)
                Where 1";
            
            if (idProdPed > 0)
                sql += " And pp.idProdPed=" + idProdPed;
            else
                sql += " And pp.idProdPed=" + GetIdProdPedByEtiqueta(sessao, codEtiqueta, buscarSomenteVisiveis);

            var ppe = objPersistence.LoadData(sessao, sql).ToList();
            return ppe != null && ppe.Count > 0 ? ppe[0] : null;
        }

        /// <summary>
        /// Retorna o ID do produto do pedido pelo ID do material do projeto.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idMaterItemProj"></param>
        /// <returns></returns>
        public uint GetIdProdPedByMaterItemProj(GDASession sessao, uint idPedido, uint idMaterItemProj)
        {
            object retorno = objPersistence.ExecuteScalar(sessao, "select idProdPed from produtos_pedido_espelho where idPedido=" + idPedido + " and idMaterItemProj=" + idMaterItemProj);
            return retorno != null && retorno.ToString() != "" ? Glass.Conversoes.StrParaUint(retorno.ToString()) : 0;
        }

        #endregion

        #region Verifica se produto já foi adicionado

        /// <summary>
        /// Verifica se produto com a altura e largura passadas já foi adicionado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProd"></param>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="idProcesso"></param>
        /// <param name="idAplicacao"></param>
        /// <returns></returns>
        public bool ExistsInPedido(uint idPedido, uint idProd, Single alturaReal, int largura, uint? idProcesso, uint? idAplicacao)
        {
            string sql = "Select count(*) From produtos_pedido_espelho where (invisivelFluxo=false or invisivelFluxo is null) and idPedido=" + idPedido + " And idProd=" + idProd +
                " And alturaReal=" + alturaReal.ToString().Replace(',', '.') + " And largura=" + largura;

            return objPersistence.ExecuteSqlQueryCount(sql, null) > 0;
        }

        #endregion

        #region Retorna o total dos produtos do pedido

        public string GetTotalByPedido(uint idPedido)
        {
            return objPersistence.ExecuteScalar("Select Round(Sum(Total)) From produtos_pedido_espelho where " +
                "(invisivelFluxo=false or invisivelFluxo is null) and idpedido=" + idPedido).ToString();
        }
        public IEnumerable<decimal> ObterTotalPorProduto(GDASession session, uint idPedido)
        {
            return ExecuteMultipleScalar<decimal>(session, "Select Round((Total+ValorBenef), 2) From produtos_pedido_espelho where idpedido=" + idPedido + "");
        }

        #endregion

        #region Marca a quantidade de determinado item que foi impresso

        /// <summary>
        /// Marca a quantidade de determinado produto que foi impresso
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <param name="qtdImpresso"></param>
        /// <param name="obs"></param>
        public void MarcarImpressao(GDASession session, uint idProdPed, int qtdAImprimir, string obs)
        {
            var salvarObs = !String.IsNullOrEmpty(obs);
            var temProjeto = ObtemValorCampo<int?>("IdItemProjeto", "IdProdPed="+ idProdPed).HasValue;
            var qtde = ObtemQtde(session, idProdPed);
            var qtdImpressoAtual = ObterQtdeImpresso(session, idProdPed);

            // Verifica se a quantidade disponível para imprimir é maior ou igual à quantidade que o usuário solicitiou para imprimir, desde que não seja filha de laminado
            if (ObterIdProdPedParent(session, idProdPed).GetValueOrDefault() == 0 && qtde - qtdImpressoAtual < qtdAImprimir)
                throw new Exception(string.Format(
                    "A quantidade a ser impressa é maior do que a quantidade de etiquetas ainda não impressas. Pedido: {0} IdProdPed: {1} Qtde: {2} Qtd. já impresso: {3} Qtd. a imprimir: {4}",
                    ObtemIdPedido(idProdPed),
                    idProdPed,
                    qtde,
                    qtdImpressoAtual,
                    qtdAImprimir));

            var sql = string.Format("Update produtos_pedido_espelho set qtdImpresso=qtdImpresso+{0} {1} Where idProdPed={2}",
                qtdAImprimir, 
                salvarObs ? !PCPConfig.Etiqueta.NaoExibirObsPecaAoImprimirEtiqueta ? ", obs=?obs" : ", obsgrid=?obs" : "", 
                idProdPed);

            objPersistence.ExecuteCommand(session, sql, new GDAParameter[] { new GDAParameter("?obs", obs) });
        }

        /// <summary>
        /// Atualiza a observação da peça
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <param name="obs"></param>
        public void AtualizaObs(GDASession session, uint idProdPed, string obs)
        {
            string sql = "Update produtos_pedido_espelho set obs=?obs Where idProdPed=" + idProdPed;

            objPersistence.ExecuteCommand(session, sql, new GDAParameter("?obs", obs));
        }

        #endregion

        #region Exclui por pedido

        /// <summary>
        /// Exclui todos os produtos do pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        public void DeleteByPedido(uint idPedido)
        {
            string idsProdPed = String.Join(",",
                ExecuteMultipleScalar<string>("select idProdPed from produto_impressao where idPedido=" + idPedido).ToArray());

            if (String.IsNullOrEmpty(idsProdPed))
                idsProdPed = "0";

            objPersistence.ExecuteCommand("delete from produtos_pedido_espelho Where idPedido=" + idPedido + " and idProdPed not in (" + idsProdPed + @")");

            string idsProdPedEsp = String.Join(",",
                ExecuteMultipleScalar<string>("select idProdPed from produtos_pedido_espelho where idPedido=" + idPedido).ToArray());

            string sql = String.Format(@"
                update produtos_pedido_espelho set invisivelFluxo=true where idPedido={0};
                update produtos_pedido set invisivelFluxo=true where idProdPedEsp in (" + idsProdPedEsp + ")", idPedido);

            objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Busca para impressão do pedido no PCP

        /// <summary>
        /// Busca produtos para mostrar no relatório
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="showAluminio"></param>
        /// <param name="showFerragem"></param>
        /// <param name="showOutros"></param>
        /// <returns></returns>
        public ProdutosPedidoEspelho[] GetForRptPcp(string idPedidos, string grupos, string produtos, bool agruparProduto)
        {
            string dadosAmbiente = @"if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", concat('  ', 
                concat(cast(ap.altura as char), concat('x', concat(cast(ap.largura as char), 
                concat(' - ', concat(cast(ap.qtde as char), ' peça(s)')))))), '')";

            string sql = @"
                Select *, Sum(Qtde) as QtdeSomada, Sum(totM) as TotMSomada, sum(peso) as pesoSomado
                From (
                    Select pp.*, p.Descricao as DescrProduto, p.CodInterno, (ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @") as pedidoMaoObra,
                        p.idGrupoProd, p.idSubgrupoProd, apl.CodInterno as CodAplicacao, prc.CodInterno as CodProcesso, ip.obs as obsProj,
                        concat(ap.ambiente, " + dadosAmbiente + @") as ambientePedido, ap.descricao as DescrAmbientePedido
                    From produtos_pedido_espelho pp 
                        Left Join (select * from produtos_pedido where idpedido in (" + idPedidos + @")) pp_original On (pp.idProdPed=pp_original.idProdPedEsp)
                        Left Join ambiente_pedido_espelho ap On (pp.idAmbientePedido=ap.idAmbientePedido)
                        Left Join item_projeto ip On (ap.idItemProjeto=ip.idItemProjeto)
                        Left Join produto p On (pp.idProd=p.idProd) 
                        Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao) 
                        Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso) 
                        Left Join pedido ped On (pp.idPedido=ped.idPedido) 
                    Where (pp.invisivelFluxo=false or pp.invisivelFluxo is null) and pp.idPedido in (" + idPedidos + @")
                        and (pp_original.invisivelFluxo=false or pp_original.invisivelFluxo is null)";

            if (!String.IsNullOrEmpty(grupos))
                sql += " and p.idGrupoProd in (" + grupos + ")";

            if (PedidoConfig.DadosPedido.AmbientePedido && !String.IsNullOrEmpty(produtos))
                sql += " and pp.idProdPed in (" + produtos + ")";

            sql += @"
                    Group By pp.idPedido, pp.idProdPed
                ) as temp";

            if (PedidoConfig.DadosPedido.AmbientePedido && !String.IsNullOrEmpty(produtos))
                sql += " Group by " + (agruparProduto ? "idProd, Altura, Largura" : "idPedido, idProdPed");
            else
            {
                if (agruparProduto)
                    sql += @" Group By idPedido, if(pedidoMaoObra, idProdPed, idProd), if(pedidoMaoObra, idProdPed, alturaReal), if(pedidoMaoObra, idProdPed, altura), 
                        if(pedidoMaoObra, idProdPed, largura), if(pedidoMaoObra, idProdPed, redondo), if(pedidoMaoObra, idProdPed, (select cast(group_concat(idBenefConfig) as char) 
                        from produto_pedido_espelho_benef where idProdPed=temp.idProdPed order by idBenefConfig asc)), if(pedidoMaoObra, idProdPed, idProcesso), 
                        if(pedidoMaoObra, idProdPed, idAplicacao)";
                else
                    sql += @" Group By idPedido, idProdPed";
            }
            
            sql += " Order by idGrupoProd, DescrProduto, Qtde";

            var lstProdPedEsp = objPersistence.LoadData(sql);
            var lstProdPedEspRetorno = new List<ProdutosPedidoEspelho>();

            GenericBenefCollection lstBenef;

            // Adiciona os beneficiamentos feitos nos produtos como itens do pedido
            foreach (ProdutosPedidoEspelho ppe in lstProdPedEsp)
            {
                if (PedidoDAO.Instance.IsMaoDeObra(ppe.IdPedido) && ppe.IdAmbientePedido != null)
                {
                    int? qtdAmb = AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<int?>("Qtde", "idAmbientePedido=" + ppe.IdAmbientePedido.Value);
                    if (qtdAmb > 1)
                        ppe.TotMSomada *= (float)qtdAmb;
                }

                // Verifica se o produto foi marcado como redondo
                if (ppe.Redondo || (ppe.IdAmbientePedido > 0 && AmbientePedidoEspelhoDAO.Instance.IsRedondo(null, ppe.IdAmbientePedido.Value)))
                {
                    if (!BenefConfigDAO.Instance.CobrarRedondo() && !ppe.DescrProduto.ToLower().Contains("redondo"))
                        ppe.DescrProduto += " REDONDO";

                    ppe.Largura = 0;
                }

                ppe.Peso = (float)ppe.PesoSomado;

                lstProdPedEspRetorno.Add(ppe);

                // Carrega os beneficiamentos deste produto, se houver
                lstBenef = (GenericBenefCollection)ProdutoPedidoEspelhoBenefDAO.Instance.GetByProdutoPedido(ppe.IdProdPed).ToList();

                if (!PedidoConfig.RelatorioPedido.AgruparBenefRelatorio)
                {
                    // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                    foreach (GenericBenef benef in lstBenef)
                    {
                        ProdutosPedidoEspelho prodPed = new ProdutosPedidoEspelho();
                        prodPed.IdAmbientePedido = ppe.IdAmbientePedido;
                        prodPed.IdItemProjeto = ppe.IdItemProjeto;
                        prodPed.IdPedido = ppe.IdPedido;
                        prodPed.QtdeSomada = benef.Qtd > 0 ? benef.Qtd : 1;
                        prodPed.ValorVendido = benef.ValorUnit;
                        prodPed.Total = benef.Valor;
                        prodPed.DescrProduto = " " + benef.DescricaoBeneficiamento +
                            Utils.MontaDescrLapBis(benef.BisAlt, benef.BisLarg, benef.LapAlt, benef.LapLarg, benef.EspBisote, null, null, false);

                        // Repete campos do produto para que a ordenação na impressão do pedido PCP fique correta
                        prodPed.IdGrupoProd = ppe.IdGrupoProd;
                        prodPed.IdProd = ppe.IdProd;
                        prodPed.Altura = ppe.Altura;
                        prodPed.Largura = ppe.Largura;

                        lstProdPedEspRetorno.Add(prodPed);
                    }
                }
                else
                {
                    if (lstBenef.Count > 0)
                    {
                        var prodPed = new ProdutosPedidoEspelho();
                        prodPed.IdAmbientePedido = ppe.IdAmbientePedido;

                        // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                        foreach (GenericBenef benef in lstBenef)
                        {
                            prodPed.IdItemProjeto = ppe.IdItemProjeto;
                            prodPed.IdPedido = ppe.IdPedido;
                            prodPed.QtdeSomada = 0;
                            prodPed.ValorVendido += benef.ValorUnit;
                            prodPed.Total += benef.Valor;
                            string textoQuantidade = (benef.TipoCalculo == TipoCalculoBenef.Quantidade) ? benef.Qtd.ToString() + " " : "";
                            prodPed.DescrProduto += "; " + textoQuantidade + benef.DescricaoBeneficiamento +
                                Utils.MontaDescrLapBis(benef.BisAlt, benef.BisLarg, benef.LapAlt, benef.LapLarg, benef.EspBisote, null, null, false);
                        }

                        prodPed.DescrProduto = " " + prodPed.DescrProduto.Substring(2);

                        // Repete campos do produto para que a ordenação na impressão do pedido PCP fique correta
                        prodPed.IdGrupoProd = ppe.IdGrupoProd;
                        prodPed.IdProd = ppe.IdProd;
                        prodPed.Altura = ppe.Altura;
                        prodPed.Largura = ppe.Largura;

                        lstProdPedEspRetorno.Add(prodPed);
                    }
                }
            }

            return lstProdPedEspRetorno.ToArray();
        }

        #endregion

        #region Busca para compra

        public ProdutosPedidoEspelho GetElementForCompraPcp(uint idProdPed)
        {
            string sql = @"select pp.* from produtos_pedido_espelho pp left join ambiente_pedido_espelho ape on 
                (pp.idAmbientePedido=ape.idAmbientePedido) where idProdPed=" + idProdPed;

            return objPersistence.LoadOneData(sql);
        }

        internal string SqlCompra(string idPedidoEspelho, string idsPedidosEspelho, uint idAmbientePedido,
            string codInternoProd, string descrProd, uint idPedido, bool selecionar)
        {
            return SqlCompra(null, idPedidoEspelho, idsPedidosEspelho, idAmbientePedido, codInternoProd,
                descrProd, idPedido, selecionar);
        }

        internal string SqlCompra(GDASession session, string idPedidoEspelho, string idsPedidosEspelho, uint idAmbientePedido,
            string codInternoProd, string descrProd, uint idPedido, bool selecionar)
        {
            var sqlQtdeComprada = @"
                select cast(coalesce(sum(qtde), 0) as signed integer)
                from produtos_compra pc 
                    left join compra c on (pc.idCompra=c.idCompra)
                where pc.idProdPed=pp.idProdPed
                    and c.situacao<>" + (int)Compra.SituacaoEnum.Cancelada + @"
                    and (pc.NaoCobrarVidro=false or pc.NaoCobrarVidro is null)";

            var sqlQtdeBenefProdPedEsp = @"
                select coalesce(count(*) * pp.qtde, 0)
                from produto_pedido_espelho_benef
                where idProdPed=pp.idProdPed";

            var sqlQtdeBenefCompra = @"
                select coalesce(sum(contagem), 0)
                from (" +
                    ProdutosCompraBenefDAO.Instance.SqlProdPedBenef(0, 0, 0, 0, false) + @"
                ) as temp
                where temp.idProdPed=pp.idProdPed";

            var campos = selecionar ? @"pp.*, p.Descricao as DescrProduto, p.CodInterno, p.IdGrupoProd, ape.Ambiente as AmbientePedido,
                p.idSubgrupoProd, if(p.AtivarAreaMinima=1, Cast(p.AreaMinima as char), '0') as AreaMinima, apl.CodInterno as CodAplicacao, 
                prc.CodInterno as CodProcesso, (" + sqlQtdeComprada + ") as QtdeComprada, (" + sqlQtdeBenefCompra + @") as QtdeBenefCompra, 
                (" + sqlQtdeBenefProdPedEsp + ") as QtdeBenefProdPedEsp, '$$$' as criterio" : "count(*)";

            var criterio = "";
            
            var sql = @"
                Select " + campos + @"
                From produtos_pedido_espelho pp
                    Left Join ambiente_pedido_espelho ape On (pp.idAmbientePedido=ape.idAmbientePedido)
                    Left Join produto p On (pp.idProd=p.idProd)
                    Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao)
                    Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso)
                Where (pp.invisivelFluxo=false or pp.invisivelFluxo is null)";

            if (!string.IsNullOrEmpty(idPedidoEspelho) && idPedidoEspelho != "0")
            {
                sql += " and pp.idPedido=" + idPedidoEspelho;
                criterio += "Pedido: " + idPedidoEspelho + "    ";
            }
            else if (!string.IsNullOrEmpty(idsPedidosEspelho))
            {
                sql += " and pp.idPedido in (" + idsPedidosEspelho + ")";
                criterio += "Pedidos: " + idsPedidosEspelho.Replace(",", ", ") + "    ";
            }

            if (idAmbientePedido > 0)
            {
                sql += " and pp.idAmbientePedido=" + idAmbientePedido;
                criterio += "Ambiente: " + AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<string>(session, "ambiente", "idAmbientePedido=" + idAmbientePedido) + "    ";
            }

            if (!string.IsNullOrEmpty(codInternoProd))
            {
                sql += " and p.codInterno=?codInterno";
                criterio += "Produto: " + ProdutoDAO.Instance.GetDescrProduto(session, codInternoProd) + "    ";
            }
            else if (!string.IsNullOrEmpty(descrProd))
            {
                var ids = ProdutoDAO.Instance.ObtemIds(session, null, descrProd);
                sql += " And p.idProd In (" + ids + ")";
                criterio += "Produto: " + descrProd + "    ";
            }

            if (idPedido > 0)
            {
                sql += " and pp.IdPedido=" + idPedido;
            }

            if (selecionar)
                sql += @"
                    Having (
                        qtdeComprada < pp.qtde
                        or qtdeBenefCompra < qtdeBenefProdPedEsp
                        or (
                            qtdeBenefProdPedEsp = 0
                            and qtdeComprada < pp.qtde
                        )
                    )
                    Order By ape.Ambiente, p.CodInterno";
            else
                sql += @"
                    and (
                        (" + sqlQtdeComprada + @") < pp.qtde
                        or (" + sqlQtdeBenefCompra + @") < (" + sqlQtdeBenefProdPedEsp + @")
                        or (
                            (" + sqlQtdeBenefProdPedEsp + @") = 0
                            and (" + sqlQtdeComprada + @") < pp.qtde
                        )
                    )";

            return sql.Replace("$$$", criterio != null ? criterio.Replace("'", "").Replace("\"", "") : String.Empty);
        }

        private GDAParameter[] GetParamsForCompra(string codInternoProd, string descrProd)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codInternoProd))
                lst.Add(new GDAParameter("?codInterno", codInternoProd));

            if (!String.IsNullOrEmpty(descrProd))
                lst.Add(new GDAParameter("?descrProd", "%" + descrProd + "%"));

            return lst.ToArray();
        }

        public int GetCountForCompra(uint idPedidoEspelho, uint idAmbientePedido)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlCompra(idPedidoEspelho.ToString(), null, idAmbientePedido, null, null, 0, false));
        }

        /// <summary>
        /// Utilizado na tela CadCompraPcp
        /// </summary>
        /// <param name="idsPedidosEspelho"></param>
        /// <returns></returns>
        public ProdutosPedidoEspelho[] GetForCompra(string idsPedidosEspelho)
        {
            return GetForCompra(0, idsPedidosEspelho.Substring(0, idsPedidosEspelho.LastIndexOf(',')), 0, null, null, 0, false);
        }

        public ProdutosPedidoEspelho[] GetForCompra(uint idPedidoEspelho, uint idAmbientePedido, uint idPedido)
        {
            return GetForCompra(idPedidoEspelho, null, idAmbientePedido, null, null, idPedido, false);
        }

        public ProdutosPedidoEspelho[] GetForCompra(uint idPedidoEspelho, string idsPedidosEspelho, uint idAmbientePedido, 
            string codInternoProd, string descrProd, uint idPedido, bool forRpt)
        {
            ProdutosPedidoEspelho[] produtos = objPersistence.LoadData(SqlCompra(idPedidoEspelho.ToString(), idsPedidosEspelho, idAmbientePedido, 
                codInternoProd, descrProd, idPedido, true), GetParamsForCompra(codInternoProd, descrProd)).ToArray();

            if (forRpt)
            {
                List<ProdutosPedidoEspelho> ppe = new List<ProdutosPedidoEspelho>();
                foreach (ProdutosPedidoEspelho p in produtos)
                {
                    if (p.Redondo && !p.DescrProduto.ToLower().Contains("redondo"))
                        p.DescrProduto += " REDONDO";

                    ppe.Add(p);

                    var lstProdPedEsp = ProdutoPedidoEspelhoBenefDAO.Instance.GetForCompraPcp(p.IdProdPed).ToArray();

                    if (lstProdPedEsp != null)
                        foreach (ProdutoPedidoEspelhoBenef b in lstProdPedEsp)
                        {
                            ProdutosPedidoEspelho benef = new ProdutosPedidoEspelho();
                            benef.IdPedido = p.IdPedido;
                            benef.IdProdPed = p.IdProdPed;
                            benef.DescrProduto = " " + b.DescrBenef;
                            benef.Qtde = b.Qtd > 0 ? b.Qtd : 1;
                            benef.QtdeComprada = ProdutosCompraBenefDAO.Instance.GetCountByProdPedBenef(p.IdProdPed, b.IdBenefConfig);

                            if (benef.QtdeComprar > 0)
                                ppe.Add(benef);
                        }
                }

                return ppe.ToArray();
            }
            else
                return produtos;
        }

        public IList<ProdutosPedidoEspelho> GetForCompraList(uint idPedidoEspelho, uint idAmbientePedido,
            string codInternoProd, string descrProd, uint idPedido, string sortExpression, int startRow, int pageSize)
        {
            // Se nenhum filtro tiver sido informado não retorna nada.
            if (idPedido == 0 && idPedidoEspelho == 0 && idAmbientePedido == 0 && String.IsNullOrEmpty(codInternoProd) &&
                String.IsNullOrEmpty(descrProd))
                return null;

            return LoadDataWithSortExpression(SqlCompra(idPedidoEspelho.ToString(), null, idAmbientePedido, codInternoProd, descrProd, idPedido, true), 
                sortExpression, startRow, pageSize, GetParamsForCompra(codInternoProd, descrProd));
        }

        public int GetForCompraCount(uint idPedidoEspelho, uint idAmbientePedido,
            string codInternoProd, string descrProd, uint idPedido)
        {
            // Se nenhum filtro tiver sido informado retorna 0.
            if (idPedido == 0 && idPedidoEspelho == 0 && idAmbientePedido == 0 && String.IsNullOrEmpty(codInternoProd) &&
                String.IsNullOrEmpty(descrProd))
                return 0;

            return objPersistence.ExecuteSqlQueryCount(SqlCompra(idPedidoEspelho.ToString(), null, idAmbientePedido, codInternoProd, descrProd, idPedido, false),
                GetParamsForCompra(codInternoProd, descrProd));
        }

        #endregion

        #region Busca para compra de caixa

        internal string SqlCompraProdBenef(string idsPedidoEspelho, uint idCliente, string nomeCliente, uint idLoja,
            int situacao, string situacaoPedOri, string dataIniFin, string dataFimFin, string idsRota, bool selecionar)
        {
            var campos = selecionar ? @"pp.idPedido, Cast((ppeb.qtd * pp.qtde) As Signed) As Qtde, Cast((pp.altura + bc.acrescimoAltura) As Signed) As Altura,
                Cast((pp.largura + bc.acrescimoLargura) As Signed) As Largura, pp.espessura, p.idProd, p.descricao As DescrProduto, bc.descricao As DescrProdutoBenef,
                cli.id_cli As IdCliente, Coalesce(cli.nomeFantasia, cli.nome, '') As NomeCliente, r.codInterno As RotaCliente, '$$$' As criterio,
                pp.idProdPed, pp.idItemProjeto, pp.idMaterItemProj, pp.idAmbientePedido, pp.idAplicacao, pp.idProcesso, pp.valorVendido, pp.alturaReal,
                pp.larguraReal, pp.totM, pp.totM2Calc, pp.qtdImpresso, pp.total, pp.aliqIcms, pp.valorIcms, pp.aliquotaIpi, pp.valorIpi, pp.ambiente, pp.obs,
                pp.redondo, pp.valorBenef, pp.pedCli, pp.alturaBenef, pp.larguraBenef, pp.espBenef, pp.valorAcrescimo, pp.valorDesconto,
                pp.valorAcrescimoProd, pp.valorDescontoProd, pp.percDescontoQtde, pp.valorDescontoQtde, pp.valorDescontoCliente, ped.DataEntrega as DataEntrega,
                pp.valorAcrescimoCliente, pp.invisivelFluxo, pp.invisivelAdmin, pp.valorUnitBruto, pp.totalBruto, pp.qtdeInvisivel, pp.valorComissao, pp.peso"
                : "count(*)";
            var criterio = "";
            var sql = @"
                Select " + campos + @"
                From produtos_pedido_espelho pp
                    Inner Join produto_pedido_espelho_benef ppeb ON (pp.idProdPed=ppeb.idProdPed)
                    Inner Join benef_config bc ON (ppeb.idBenefConfig=bc.idBenefConfig)
                    Inner Join produto p On (pp.idProd=p.idProd)
                    Left Join pedido ped ON (pp.idPedido=ped.idPedido)
                    Left Join pedido_espelho pe ON (pp.idPedido=pe.idPedido)
                    Left Join cliente cli ON (ped.idCli=cli.id_cli)
                    Left Join rota_cliente rc ON (cli.id_cli=rc.idCliente)
                    Left Join rota r ON (rc.idrota=r.idRota)
                Where !Coalesce(pp.invisivelFluxo, false) And bc.idProd > 0";

            if (!string.IsNullOrEmpty(idsPedidoEspelho))
            {
                sql += string.Format(" AND ped.IdPedido IN ({0})", idsPedidoEspelho);
                criterio += string.Format("Pedido(s): {0}    ", idsPedidoEspelho);
            }
            
            if (idCliente > 0)
            {
                sql += " And ped.idCli=" + idCliente;
                criterio += " Cliente: " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                var ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And ped.idCli In (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            if (idLoja > 0)
            {
                sql += " and ped.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (situacao > 0)
            {
                sql += " and pe.situacao=" + situacao;
                criterio += "Situação PCP: " +
                    (situacao == 1 ? "Aberto" :
                    situacao == 2 ? "Finalizado" :
                    situacao == 3 ? "Impresso" :
                    situacao == 4 ? "Impresso Comum" :
                    "N/A") + "    ";
            }

            if (!String.IsNullOrEmpty(situacaoPedOri))
            {
                sql += " And ped.situacao In (" + situacaoPedOri + ")";
                criterio += "Situação: " + PedidoDAO.Instance.GetSituacaoPedido(situacaoPedOri) + "    ";
            }

            if (!String.IsNullOrEmpty(dataIniFin))
            {
                sql += " And pe.dataConf>=?dataIniFin";
                criterio += "Data Finalização PCP de " + dataIniFin + "    ";
            }

            if (!String.IsNullOrEmpty(dataFimFin))
            {
                sql += " And pe.dataConf<=?dataFimFin";

                if (!String.IsNullOrEmpty(dataIniFin))
                    criterio = criterio.TrimEnd() + " até " + dataFimFin + "    ";
                else
                    criterio += "Data Finalização PCP até " + dataFimFin + "    ";
            }

            return sql.Replace("$$$", criterio != null ? criterio.Replace("'", "").Replace("\"", "") : String.Empty);
        }

        public IList<ProdutosPedidoEspelho> GetListCompraProdBenef(string idsPedidoEspelho, uint idCliente, string nomeCliente, uint idLoja,
            int situacao, string situacaoPedOri, string dataIniFin, string dataFimFin, string idsRota, string sortExpression, int startRow, int pageSize)
        {
            // Se nenhum filtro tiver sido informado não retorna nada.
            if (string.IsNullOrEmpty(idsPedidoEspelho) && idCliente == 0 && String.IsNullOrEmpty(nomeCliente) && idLoja == 0 && situacao == 0 &&
                String.IsNullOrEmpty(situacaoPedOri) && String.IsNullOrEmpty(dataIniFin) && String.IsNullOrEmpty(dataFimFin) && String.IsNullOrEmpty(idsRota))
                return null;

            return LoadDataWithSortExpression(SqlCompraProdBenef(idsPedidoEspelho, idCliente, nomeCliente, idLoja, situacao, situacaoPedOri,
                dataIniFin, dataFimFin, idsRota, true),
                sortExpression, startRow, pageSize, GetParamsCompraProdBenef(nomeCliente, situacaoPedOri, dataIniFin, dataFimFin, idsRota));
        }

        public int GetCountCompraProdBenef(string idsPedidoEspelho, uint idCliente, string nomeCliente, uint idLoja,
            int situacao, string situacaoPedOri, string dataIniFin, string dataFimFin, string idsRota)
        {
            // Se nenhum filtro tiver sido informado não retorna nada.
            if (string.IsNullOrEmpty(idsPedidoEspelho) && idCliente == 0 && String.IsNullOrEmpty(nomeCliente) && idLoja == 0 && situacao == 0 &&
                String.IsNullOrEmpty(situacaoPedOri) && String.IsNullOrEmpty(dataIniFin) && String.IsNullOrEmpty(dataFimFin) && String.IsNullOrEmpty(idsRota))
                return 0;

            return objPersistence.ExecuteSqlQueryCount(SqlCompraProdBenef(idsPedidoEspelho, idCliente, nomeCliente, idLoja, situacao,
                situacaoPedOri, dataIniFin, dataFimFin, idsRota, false),
                GetParamsCompraProdBenef(nomeCliente, situacaoPedOri, dataIniFin, dataFimFin, idsRota));
        }

        public ProdutosPedidoEspelho[] GetForRptCompraProdBenef(string idsPedidoEspelho, uint idCliente, string nomeCliente, uint idLoja,
            int situacao, string situacaoPedOri, string dataIniFin, string dataFimFin, string idsRota)
        {
            // Se nenhum filtro tiver sido informado não retorna nada.
            if (string.IsNullOrEmpty(idsPedidoEspelho) && idCliente == 0 && String.IsNullOrEmpty(nomeCliente) && idLoja == 0 && situacao == 0 &&
                String.IsNullOrEmpty(situacaoPedOri) && String.IsNullOrEmpty(dataIniFin) && String.IsNullOrEmpty(dataFimFin) && String.IsNullOrEmpty(idsRota))
                return null;

            return objPersistence.LoadData(SqlCompraProdBenef(idsPedidoEspelho, idCliente, nomeCliente, idLoja, situacao,
                situacaoPedOri, dataIniFin, dataFimFin, idsRota, true),
                GetParamsCompraProdBenef(nomeCliente, situacaoPedOri, dataIniFin, dataFimFin, idsRota)).ToArray();
        }

        public int GetCountRptCompraProdBenef(string idsPedidoEspelho, uint idCliente, string nomeCliente, uint idLoja,
            int situacao, string situacaoPedOri, string dataIniFin, string dataFimFin, string idsRota)
        {
            // Se nenhum filtro tiver sido informado não retorna nada.
            if (string.IsNullOrEmpty(idsPedidoEspelho) && idCliente == 0 && String.IsNullOrEmpty(nomeCliente) && idLoja == 0 && situacao == 0 &&
                String.IsNullOrEmpty(situacaoPedOri) && String.IsNullOrEmpty(dataIniFin) && String.IsNullOrEmpty(dataFimFin) && String.IsNullOrEmpty(idsRota))
                return 0;

            return objPersistence.ExecuteSqlQueryCount(SqlCompraProdBenef(idsPedidoEspelho, idCliente, nomeCliente, idLoja, situacao,
                situacaoPedOri, dataIniFin, dataFimFin, idsRota, true),
                GetParamsCompraProdBenef(nomeCliente, situacaoPedOri, dataIniFin, dataFimFin, idsRota));
        }

        private GDAParameter[] GetParamsCompraProdBenef(string nomeCliente, string situacaoPedOri, string dataIniFin, string dataFimFin, string idsRota)
        {
            var lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCliente))
                lst.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(situacaoPedOri))
                lst.Add(new GDAParameter("?situacaoPedOri", situacaoPedOri));
            
            if (!String.IsNullOrEmpty(dataIniFin))
                lst.Add(new GDAParameter("?dataIniFin", DateTime.Parse(dataIniFin + " 00:00:00")));
            
            if (!String.IsNullOrEmpty(dataFimFin))
                lst.Add(new GDAParameter("?dataFimFin", DateTime.Parse(dataFimFin + " 00:00:00")));

            return lst.ToArray();
        }

        #endregion

        #region Atualiza total do beneficiamento aplicado neste produto

        /// <summary>
        /// Atualiza os beneficiamentos de um produto.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <param name="beneficiamentos"></param>
        internal void AtualizaBenef(GDASession sessao, uint idProdPed, GenericBenefCollection beneficiamentos, IContainerCalculo container)
        {
            ProdutoPedidoEspelhoBenefDAO.Instance.DeleteByProdPed(sessao, idProdPed);
            foreach (ProdutoPedidoEspelhoBenef ppeb in beneficiamentos.ToProdutosPedidoEspelho(idProdPed))
                if (ppeb.IdBenefConfig > 0)
                    ProdutoPedidoEspelhoBenefDAO.Instance.Insert(sessao, ppeb);

            UpdateValorBenef(sessao, idProdPed, container);
        }

        /// <summary>
        /// Calcula o valor total do produto com o beneficiamento aplicado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPed"></param>
        public void UpdateValorBenef(GDASession sessao, uint idProdPed, IContainerCalculo container)
        {
            var idProd = ObtemValorCampo<int>(sessao, "idProd", "idProdPed=" + idProdPed);
            if (Glass.Configuracoes.Geral.NaoVendeVidro() || !ProdutoDAO.Instance.CalculaBeneficiamento(sessao, idProd))
                return;

            string sql = "Update produtos_pedido_espelho pp set pp.valorBenef=" +
                "Coalesce(Round((Select Sum(ppeb.Valor) from produto_pedido_espelho_benef ppeb Where ppeb.idProdPed=pp.idProdPed), 2), 0) " +
                "Where pp.idProdPed=" + idProdPed;

            objPersistence.ExecuteCommand(sessao, sql);

            // Recalcula o total bruto/valor unitário bruto
            ProdutosPedidoEspelho pp = GetElementByPrimaryKey(sessao, idProdPed);
            UpdateBase(sessao, pp, container);
        }

        #endregion

        #region Retorna todos os produtos de um ambiente

        /// <summary>
        /// Retorna os produtos de um ambiente.
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public ProdutosPedidoEspelho[] GetByAmbiente(uint idAmbientePedido)
        {
            return GetByAmbiente(null, idAmbientePedido);
        }

        /// <summary>
        /// Retorna os produtos de um ambiente.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public ProdutosPedidoEspelho[] GetByAmbiente(GDASession sessao, uint idAmbientePedido)
        {
            bool temFiltro;
            string filtroAdicional;
            return objPersistence.LoadData(sessao, Sql(0, 0, idAmbientePedido, null, null, false, null, false, true,
                false, true, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional)).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public int GetCountByAmbiente(uint idAmbientePedido)
        {
            return GetCountByAmbiente(null, idAmbientePedido);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public int GetCountByAmbiente(GDASession session, uint idAmbientePedido)
        {
            string sql = "select count(*) from produtos_pedido_espelho where (invisivelFluxo=false or " +
                "invisivelFluxo is null) and idAmbientePedido=" + idAmbientePedido;

            return objPersistence.ExecuteSqlQueryCount(session, sql);
        }

        /// <summary>
        /// Retorna os produtos de um ambiente.
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public ProdutosPedidoEspelho[] GetByAmbienteFast(uint idPedido, uint idAmbientePedido)
        {
            return GetByAmbienteFast(null, idPedido, idAmbientePedido);
        }

        /// <summary>
        /// Retorna os produtos de um ambiente.
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public ProdutosPedidoEspelho[] GetByAmbienteFast(GDASession session, uint idPedido, uint idAmbientePedido)
        {
            string sql = @"
                Select pp.*, p.Descricao as DescrProduto
                From produtos_pedido_espelho pp 
                    Inner Join produto p On (pp.idProd=p.idProd) 
                Where (pp.invisivelFluxo=false or pp.invisivelFluxo is null)";

            if (idPedido > 0)
                sql += " and pp.idPedido=" + idPedido;

            if (idAmbientePedido > 0)
                sql += " and pp.idAmbientePedido=" + idAmbientePedido;

            return objPersistence.LoadData(session, sql).ToArray();
        }

        internal ProdutosPedidoEspelho[] GetAllByAmbiente(uint idAmbientePedido)
        {
            bool temFiltro;
            string filtroAdicional;
            return objPersistence.LoadData(Sql(0, 0, idAmbientePedido, null, null, false, null, false, true,
                false, true, false, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional)).ToArray();
        }

        #endregion

        #region Retorna para o relatório de pedidos em conferência

        public ProdutosPedidoEspelho[] GetForRpt(uint idPedido, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente,
            int situacao, string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab,
            string dataIniFin, string dataFimFin, string dataIniConf, string dataFimConf, bool soFinalizados, bool pedidosSemAnexos, bool pedidosAComprar,
            string idsPedidos, string situacaoCnc, string dataIniSituacaoCnc, string dataFimSituacaoCnc, string tipoPedido, string idsRotas)
        {
            return GetForRpt( idPedido,  idCli,  nomeCli,  idLoja,  idFunc,  idFuncionarioConferente,
             situacao,  situacaoPedOri,  idsProcesso,  dataIniEnt,  dataFimEnt,  dataIniFab,  dataFimFab,
             dataIniFin,  dataFimFin,  dataIniConf,  dataFimConf,  soFinalizados,  pedidosSemAnexos,  pedidosAComprar,
             idsPedidos,  situacaoCnc,  dataIniSituacaoCnc,  dataFimSituacaoCnc,  tipoPedido,  idsRotas, 0, 0);
        }

        public ProdutosPedidoEspelho[] GetForRpt(uint idPedido, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente,
            int situacao, string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab,
            string dataIniFin, string dataFimFin, string dataIniConf, string dataFimConf, bool soFinalizados, bool pedidosSemAnexos, bool pedidosAComprar,
            string idsPedidos, string situacaoCnc, string dataIniSituacaoCnc, string dataFimSituacaoCnc, string tipoPedido, string idsRotas, int origemPedido, int pedidosConferidos)
        {
            string ids = "";
            foreach (uint id in PedidoEspelhoDAO.Instance.GetIdsForRpt(idPedido, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente, situacao,
                situacaoPedOri, idsProcesso, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab, dataIniFin, dataFimFin, dataIniConf, dataFimConf, soFinalizados,
                pedidosSemAnexos, pedidosAComprar, idsPedidos, situacaoCnc, dataIniSituacaoCnc, dataFimSituacaoCnc, tipoPedido, idsRotas, null, null, 0, origemPedido, pedidosConferidos))
                ids += "," + id;

            if (ids.Length == 0)
                return new ProdutosPedidoEspelho[0];
            else
                ids = ids.Substring(1);

            string sql = @"
                select ppe.*, cv.Descricao as Cor, ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @" as pedidoMaoObra
                from produtos_pedido_espelho ppe
                    left join pedido ped on (ppe.idPedido=ped.idPedido)
                    left join ambiente_pedido_espelho ape on (ppe.idAmbientePedido=ape.idAmbientePedido)
                    left join produto p on (if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", ape.idProd, ppe.idProd)=p.idProd)
                    left join cor_vidro cv on (p.idCorVidro=cv.idCorVidro)
                where coalesce(ppe.invisivelFluxo, false)=false and ppe.idPedido in (" + ids + ")";

            return objPersistence.LoadData(sql).ToArray();
        }

        public uint? ObterIdProdPedParent(GDASession sessao, uint idProdPed)
        {
            return ObtemValorCampo<uint?>(sessao, "IdProdPedParent", "IdProdPed = " + idProdPed);
        }

        #endregion

        #region Retorna o valor de campos isolados

        public uint? ObtemIdAmbientePedido(uint idProdPed)
        {
            return ObtemIdAmbientePedido(null, idProdPed);
        }

        /// <summary>
        /// Retorna o valor do id ambiente
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public uint? ObtemIdAmbientePedido(GDASession session, uint idProdPed)
        {
            return ObtemValorCampo<uint?>(session, "idAmbientePedido", "idProdPed=" + idProdPed);
        }

        /// <summary>
        /// Retorna o valor do campo forma
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public string ObtemForma(GDASession session, uint idProdPed)
        {
            var forma = ExecuteScalar<string>(session, "Select Coalesce(forma, '') from produtos_pedido_espelho Where idProdPed=" + idProdPed);

            if (String.IsNullOrEmpty(forma))
                forma = ProdutoDAO.Instance.ObtemForma(session, (int)ObtemIdProd(session, idProdPed), null);

            return forma;
        }

        /// <summary>
        /// Obtém o número da etiqueta de reposição
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public string ObtemNumEtiquetaRepos(uint idProdPed)
        {
            string sql = "Select numEtiquetaRepos From produtos_pedido Where idProdPedEsp=" + idProdPed;

            return ExecuteScalar<string>(sql);
        }

        /// <summary>
        /// Obtem o id do pedido.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public uint ObtemIdPedido(GDASession sessao, uint idProdPed)
        {
            return ObtemValorCampo<uint>(sessao, "idPedido", "idProdPed=" + idProdPed);
        }

        /// <summary>
        /// Obtem o id do pedido.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public uint ObtemIdPedido(uint idProdPed)
        {
            return ObtemIdPedido(null, idProdPed);
        }

        /// <summary>
        /// Obtém todos os pedidos dos idProdPed passados
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public string ObtemIdsPedido(string idsProdPed)
        {
            // A variável idsProdPed não pode ser passada como parâmetro, caso seja o group_concat retorna somente o primeiro idPedido da lista
            return ExecuteScalar<string>(@"
                Select Cast(group_concat(distinct idPedido separator ',') as char)
                From produtos_pedido_espelho 
                Where idProdPed In (" + idsProdPed + ")");
        }

        /// <summary>
        /// Obtem a qtde do produto.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public float ObtemQtde(uint idProdPed)
        {
            return ObtemQtde(null, idProdPed);
        }

        /// <summary>
        /// Obtem a qtde do produto.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public float ObtemQtde(GDASession session, uint idProdPed)
        {
            return ObtemValorCampo<float>(session, "qtde", "idProdPed=" + idProdPed);
        }
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém a altura de produção
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public float ObtemAlturaProducao(uint idProdPed)
        {
            return ObtemAlturaProducao(null, idProdPed);
        }

        /// <summary>
        /// Obtém a altura de produção
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public float ObtemAlturaProducao(GDASession sessao, uint idProdPed)
        {
            return ObtemValorCampo<float>(sessao, "if(alturaReal>0, alturaReal, altura)", "idProdPed=" + idProdPed);
        }
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém a largura de produção
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public int ObtemLarguraProducao(uint idProdPed)
        {
            return ObtemLarguraProducao(null, idProdPed);
        }

        public float ObtemAltura(GDASession sessao, uint idProdPed)
        {
            return ObtemValorCampo<float>(sessao, "altura", "idProdPed=" + idProdPed);
        }

        public float ObtemLargura(GDASession sessao, uint idProdPed)
        {
            return ObtemValorCampo<float>(sessao, "Largura", "idProdPed=" + idProdPed);
        }

        /// <summary>
        /// Obtém a largura de produção
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public int ObtemLarguraProducao(GDASession sessao, uint idProdPed)
        {
            return ObtemValorCampo<int>(sessao, "if(larguraReal>0, larguraReal, largura)", "idProdPed=" + idProdPed);
        }

        /// <summary>
        /// Obtém a espessura do produto no pedido.
        /// </summary>
        public float ObterEspessura(GDASession session, uint idProdPed)
        {
            return ObtemValorCampo<float>(session, "Espessura", string.Format("IdProdPed={0}", idProdPed));
        }

        public decimal ObtemValorVendido(uint idProdPed)
        {
            return ObtemValorCampo<decimal>("valorVendido", "idProdPed=" + idProdPed);
        }

        public uint ObtemIdProcesso(uint idProdPed)
        {
            return ObtemIdProcesso(null, idProdPed);
        }

        public uint ObtemIdProcesso(GDASession session, uint idProdPed)
        {
            return ObtemValorCampo<uint>(session, "idProcesso", "idProdPed=" + idProdPed);
        }

        public uint ObtemIdAplicacao(uint idProdPed)
        {
            return ObtemIdAplicacao(null, idProdPed);
        }

        public uint ObtemIdAplicacao(GDASession session, uint idProdPed)
        {
            return ObtemValorCampo<uint>(session, "idAplicacao", "idProdPed=" + idProdPed);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public uint ObtemIdProd(uint idProdPed)
        {
            return ObtemIdProd(null, idProdPed);
        }
        
        public uint ObtemIdProd(GDASession sessao, uint idProdPed)
        {
            return ObtemValorCampo<uint>(sessao, "idProd", "idProdPed=" + idProdPed);
        }

        public string ObtemObs(uint idProdPed)
        {
            return ObtemObs(null, idProdPed);
        }

        public string ObtemObs(GDASession session, uint idProdPed)
        {
            return ObtemValorCampo<string>(session, "Coalesce(obs, '')", "idProdPed=" + idProdPed);
        }

        public float ObtemTotM(uint idProdPed)
        {
            return ObtemTotM(null, idProdPed);
        }

        public float ObtemTotM(GDASession sessao, uint idProdPed)
        {
            return ObtemValorCampo<float>(sessao, "totM", "idProdPed=" + idProdPed);
        }

        public decimal ObtemTotMCalc(GDASession sessao, uint idProdPed)
        {
            return ObtemValorCampo<decimal>(sessao, "totMCalc", "idProdPed=" + idProdPed);
        }

        public uint? ObterIdProdPedParentByEtiqueta(GDASession sessao, string numEtiqueta)
        {
            var sql = @"
                SELECT IdProdPedParent
                FROM produtos_pedido_espelho pp
	                INNER JOIN produto_impressao pi ON (pp.IdProdPed = pi.IdProdPed)
                WHERE pi.NumEtiqueta = ?etq";

            return ExecuteScalar<uint?>(sql, new GDAParameter("?etq", numEtiqueta));
        }

        public int ObterQtdeImpresso(uint idProdPed)
        {
            return ObterQtdeImpresso(null, idProdPed);
        }

        public int ObterQtdeImpresso(GDASession sessao, uint idProdPed)
        {
            return ObtemValorCampo<int>(sessao, "QtdImpresso", "IdProdPed=" + idProdPed);
        }

        #endregion

        #region Recupera os vidros de um pedido espelho

        /// <summary>
        /// Recupera os vidros de um pedido espelho.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<ProdutosPedidoEspelho> GetVidros(uint idPedido, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;
            return LoadDataWithSortExpression(Sql(0, idPedido, 0, ((int)Glass.Data.Model.NomeGrupoProd.Vidro).ToString(), null, false,
                null, false, true, false, true, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional),
                sortExpression, startRow, pageSize);
        }

        /// <summary>
        /// Recupera o número de vidros em um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int GetCountVidros(uint idPedido)
        {
            bool temFiltro;
            string filtroAdicional;
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idPedido, 0, ((int)Glass.Data.Model.NomeGrupoProd.Vidro).ToString(), null, false,
                null, false, true, false, true, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional));
        }

        #endregion

        #region Recupera os vidros de uma peça do pedido espelho

        public IList<ProdutosPedidoEspelho> ObterParaImagemPecaAvulsa(uint idPedido, uint idProdPed, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;
            return LoadDataWithSortExpression(Sql(idProdPed, idPedido, 0, ((int)Glass.Data.Model.NomeGrupoProd.Vidro).ToString(), null, false,
                null, false, true, false, true, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional),
                sortExpression, startRow, pageSize);
        }
        public int ObterParaImagemPecaAvulsaCount(uint idPedido, uint idProdPed)
        {
            bool temFiltro;
            string filtroAdicional;
            return objPersistence.ExecuteSqlQueryCount(Sql(idProdPed, idPedido, 0, ((int)Glass.Data.Model.NomeGrupoProd.Vidro).ToString(), null, false,
                null, false, true, false, true, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional));
        }

        #endregion

        #region Recupera os produtos de mão de obra de um pedido espelho

        /// <summary>
        /// Recupera os produtos de mão de obra de um pedido espelho.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<ProdutosPedidoEspelho> GetMaoDeObra(uint idPedido, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;
            return LoadDataWithSortExpression(Sql(0, idPedido, 0, ((int)Glass.Data.Model.NomeGrupoProd.MaoDeObra).ToString(), null, false,
                null, false, true, false, true, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional),
                sortExpression, startRow, pageSize);
        }

        /// <summary>
        /// Recupera o número de produtos de mão de obra em um pedido espelho.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int GetCountMaoDeObra(uint idPedido)
        {
            bool temFiltro;
            string filtroAdicional;
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idPedido, 0, ((int)Glass.Data.Model.NomeGrupoProd.MaoDeObra).ToString(), null, false,
                null, false, true, false, true, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional));
        }

        #endregion

        #region Desmembrar produtos PCP

        /// <summary>
        /// Desmembra alguns produtos, gerando clones para um produto.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <param name="qtde"></param>
        public void Desmembrar(uint idProdPed, int qtde)
        {
            // Atualiza o produto original
            ProdutosPedidoEspelho prod = GetElementByPrimaryKey(idProdPed);
            prod.Qtde -= qtde;
            Update(prod);

            // Cria o produto separado
            prod.IdProdPed = 0;
            prod.Qtde = qtde;
            Insert(prod);
        }

        #endregion

        #region Recupera um produto pelo material do projeto

        /// <summary>
        /// Recupera um produto pelo material do projeto.
        /// </summary>
        public ProdutosPedidoEspelho[] GetByMaterItemProj(uint idMaterItemProj)
        {
            return GetByMaterItemProj(null, idMaterItemProj);
        }

        /// <summary>
        /// Recupera um produto pelo material do projeto.
        /// </summary>
        public ProdutosPedidoEspelho[] GetByMaterItemProj(GDASession session, uint idMaterItemProj)
        {
            return objPersistence.LoadData(session, "select * from produtos_pedido_espelho where (invisivelFluxo=false or invisivelFluxo is null) " +
                "and idMaterItemProj=" + idMaterItemProj).ToArray();
        }

        #endregion

        #region Insere/Atualiza Produto de Projeto

        /// <summary>
        /// Aplica o acréscimo do ambiente nos produtos.
        /// (Usado ao atualizar o item projeto)
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <param name="valorAcrescimo"></param>
        /// <param name="produtos"></param>
        private void AplicaAcrescimoProdProj(GDASession sessao, PedidoEspelho pedidoEspelho, decimal valorAcrescimo, ProdutosPedidoEspelho[] produtos)
        {
            DescontoAcrescimo.Instance.AplicarAcrescimo(sessao, pedidoEspelho, 2, valorAcrescimo, produtos);
        }

        /// <summary>
        /// Aplica o desconto do ambiente nos produtos.
        /// (Usado ao atualizar o item projeto)
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <param name="valorDesconto"></param>
        /// <param name="produtos"></param>
        private void AplicaDescontoProdProj(GDASession sessao, PedidoEspelho pedidoEspelho, decimal valorDesconto, ProdutosPedidoEspelho[] produtos)
        {
            DescontoAcrescimo.Instance.AplicarDesconto(sessao, pedidoEspelho, 2, valorDesconto, produtos);
        }

        /// <summary>
        /// Aplica a comissão do ambiente nos produtos.
        /// (Usado ao atualizar o item projeto)
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <param name="valorComissao"></param>
        /// <param name="produtos"></param>
        private void AplicaComissaoProdProj(GDASession sessao, PedidoEspelho pedidoEspelho, float percComissao, ProdutosPedidoEspelho[] produtos)
        {
            DescontoAcrescimo.Instance.AplicarComissao(sessao, pedidoEspelho, percComissao, produtos);
        }

        /// <summary>
        /// Insere/Atualiza Produto de Projeto
        /// </summary>
        public uint InsereAtualizaProdProj(uint idPedidoEspelho, uint? idAmbientePedidoEsp, ItemProjeto itemProj, bool medidasAlteradas)
        {
            return InsereAtualizaProdProj(null, idPedidoEspelho, idAmbientePedidoEsp, itemProj, medidasAlteradas);
        }

        /// <summary>
        /// Insere/Atualiza Produto de Projeto
        /// </summary>
        internal uint InsereAtualizaProdProj(GDASession sessao, uint idPedidoEspelho, uint? idAmbientePedidoEsp, ItemProjeto itemProj,
            bool medidasAlteradas)
        {
            var pedidoEspelho = PedidoEspelhoDAO.Instance.GetElement(sessao, idPedidoEspelho);
            return InsereAtualizaProdProj(sessao, pedidoEspelho, idAmbientePedidoEsp, itemProj, medidasAlteradas);
        }

        /// <summary>
        /// Insere/Atualiza Produto de Projeto
        /// </summary>
        internal uint InsereAtualizaProdProj(GDASession sessao, PedidoEspelho pedidoEspelho, uint? idAmbientePedidoEsp, ItemProjeto itemProj,
            bool medidasAlteradas)
        {
            string where = "idAmbientePedido=" + idAmbientePedidoEsp.GetValueOrDefault();

            string sqlAcrescimo = "sum(valorAcrescimo)+sum(valorAcrescimoProd)";
            string sqlDesconto = "sum(valorDesconto)+sum(valorDescontoProd)";

            if (OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento && idAmbientePedidoEsp > 0)
            {
                var ambiente = AmbientePedidoEspelhoDAO.Instance.GetElementByPrimaryKey(idAmbientePedidoEsp.Value);

                if (ambiente.Acrescimo > 0)
                    sqlAcrescimo = "sum(valorAcrescimo)";

                if (ambiente.Desconto > 0)
                    sqlDesconto = "sum(valorDesconto)";
            }

            // Soma o valorAcrescimoProd e valorDescontoProd, pois caso não sejam somados, caso tenha dado acréscimo/desconto no ambiente
            // ao confirmar o projeto no PCP os mesmos estavam sendo perdidos (Chamado 6807)
            decimal valorAcrescimoAplicado = ObtemValorCampo<decimal>(sessao, sqlAcrescimo, where);
            decimal valorDescontoAplicado = ObtemValorCampo<decimal>(sessao, sqlDesconto, where);

            return InsereAtualizaProdProj(sessao, pedidoEspelho, idAmbientePedidoEsp, itemProj, valorAcrescimoAplicado,
                valorDescontoAplicado, pedidoEspelho.PercComissao, true, medidasAlteradas, null);
        }

        /// <summary>
        /// Insere/Atualiza Produto de Projeto
        /// </summary>
        internal uint InsereAtualizaProdProj(GDASession sessao, PedidoEspelho pedidoEspelho, uint? idAmbientePedidoEsp, ItemProjeto itemProj,
            decimal valorAcrescimoAplicado, decimal valorDescontoAplicado, float percComissao, bool atualizaReserva, bool medidasAlteradas,
            Dictionary<int, int> associacaoProdutosPedidoProdutosPedidoEspelho)
        {
            try
            {
                var situacao = (PedidoEspelho.SituacaoPedido)pedidoEspelho.Situacao;

                // Os produtos do pedido espelho não devem ser atualizados caso o comercial já esteja liberado parcialmente ou liberado.
                if (situacao != PedidoEspelho.SituacaoPedido.Processando && situacao != PedidoEspelho.SituacaoPedido.Aberto && situacao != PedidoEspelho.SituacaoPedido.ImpressoComum)
                {
                    return idAmbientePedidoEsp.GetValueOrDefault();
                }

                AmbientePedidoEspelho ambienteEspelho = new AmbientePedidoEspelho();
                ambienteEspelho.IdPedido = pedidoEspelho.IdPedido;
                ambienteEspelho.IdItemProjeto = itemProj.IdItemProjeto;
                ambienteEspelho.Ambiente = !String.IsNullOrEmpty(itemProj.Ambiente) ? itemProj.Ambiente : "Cálculo Projeto";
                
                string descricao = UtilsProjeto.FormataTextoOrcamento(sessao, itemProj);
                if (!String.IsNullOrEmpty(descricao)) ambienteEspelho.Descricao = descricao;

                // Se o ambiente não tiver sido informado, insere ambiente pedido, senão apenas atualiza texto
                if (idAmbientePedidoEsp == 0 || idAmbientePedidoEsp == null)
                    idAmbientePedidoEsp = AmbientePedidoEspelhoDAO.Instance.Insert(sessao, ambienteEspelho);
                else
                {
                    AmbientePedidoEspelho amb = AmbientePedidoEspelhoDAO.Instance.GetElementByPrimaryKey(sessao, idAmbientePedidoEsp.Value);
                    ambienteEspelho.TipoAcrescimo = amb.TipoAcrescimo;
                    ambienteEspelho.TipoDesconto = amb.TipoDesconto;
                    ambienteEspelho.Desconto = amb.Desconto;
                    ambienteEspelho.Acrescimo = amb.Acrescimo;
                    ambienteEspelho.IdAmbientePedido = idAmbientePedidoEsp.Value;
                    ambienteEspelho.Descricao = amb.Descricao;
                    AmbientePedidoEspelhoDAO.Instance.Update(sessao, ambienteEspelho);
                }

                // Dicionário criado para renomear imagens associadas às peças que estão sendo apagadas para que as mesmas sejam associadas às novas peças
                var dicProdPedMater = new Dictionary<uint, uint>();

                // Exclui produto do pedido, caso já tenha sido inserido
                foreach (ProdutosPedidoEspelho ppe in GetByAmbienteFast(sessao, pedidoEspelho.IdPedido, idAmbientePedidoEsp.Value))
                {
                    if (ppe.IdMaterItemProj != null)
                        dicProdPedMater.Add(ppe.IdMaterItemProj.Value, ppe.IdProdPed);

                    Delete(sessao, ppe);
                }

                List<ProdutosPedidoEspelho> prod = new List<ProdutosPedidoEspelho>();

                // Insere materiais do item projeto no ambiente
                foreach (MaterialItemProjeto mip in MaterialItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProj.IdItemProjeto, false))
                {
                    int? pipAltura = null, pipLargura = null;

                    if (mip.IdPecaItemProj > 0)
                    {
                        pipAltura = PecaItemProjetoDAO.Instance.ObtemAltura(sessao, mip.IdPecaItemProj.Value);
                        pipLargura = PecaItemProjetoDAO.Instance.ObtemLargura(sessao, mip.IdPecaItemProj.Value);
                    }

                    ProdutosPedidoEspelho prodPed = new ProdutosPedidoEspelho();
                    prodPed.IdPedido = pedidoEspelho.IdPedido;
                    prodPed.IdAmbientePedido = idAmbientePedidoEsp;
                    prodPed.IdItemProjeto = itemProj.IdItemProjeto;
                    prodPed.IdMaterItemProj = mip.IdMaterItemProj;
                    prodPed.IdProd = mip.IdProd;
                    prodPed.IdProcesso = mip.IdProcesso;
                    prodPed.IdAplicacao = mip.IdAplicacao;
                    prodPed.Redondo = mip.Redondo;
                    prodPed.Qtde = mip.Qtde;
                    prodPed.Altura = mip.Altura;
                    prodPed.Largura = mip.Largura;
                    prodPed.AlturaReal = pipAltura > 0 ? pipAltura.Value : mip.Altura;
                    prodPed.LarguraReal = pipLargura > 0 ? pipLargura.Value : mip.Largura;
                    prodPed.TotM = mip.TotM;
                    prodPed.TotM2Calc = mip.TotM2Calc;
                    prodPed.ValorVendido = mip.Valor;
                    prodPed.Total = mip.Total;
                    prodPed.Espessura = mip.Espessura;
                    prodPed.AliqIcms = mip.AliqIcms;
                    prodPed.ValorIcms = mip.ValorIcms;
                    prodPed.ValorAcrescimo = mip.ValorAcrescimo;
                    prodPed.ValorDesconto = mip.ValorDesconto;
                    prodPed.PedCli = mip.PedCli;
                    prodPed.Obs += " " + mip.Obs;
                    prodPed.Beneficiamentos = mip.Beneficiamentos;
                    prodPed.ValorUnitarioBruto = mip.ValorUnitarioBruto;
                    prodPed.TotalBruto = mip.TotalBruto;

                    // Se for alumínio, o campo altura deve buscar o campo altura de cálculo do alumínio, para que ao calcular o valor
                    // do mesmo, considere o arredondamento padrão que o tipo de cálculo tenha (0.5,1,6)
                    if (GrupoProdDAO.Instance.IsAluminio(ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, (int)prodPed.IdProd)) && mip.AlturaCalc > 0)
                    {
                        prodPed.Altura = mip.AlturaCalc;
                    }

                    DiferencaCliente.Instance.Calcular(sessao, pedidoEspelho, prodPed);
                    prodPed.IdProdPed = InsertFromProjeto(sessao, prodPed, pedidoEspelho);
                    
                    // O valor do material deve ser recalculado caso a configuração AlterarValorUnitarioProduto esteja desabilitada ou
                    // caso a configuração esteja habilitada e o valor novo seja maior que o valor antigo.
                    if ((!PedidoConfig.DadosPedido.AlterarValorUnitarioProduto &&
                        Math.Round(prodPed.ValorVendido, 2, MidpointRounding.AwayFromZero) != Math.Round(mip.Valor, 2, MidpointRounding.AwayFromZero)) ||
                        (PedidoConfig.DadosPedido.AlterarValorUnitarioProduto &&
                        Math.Round(prodPed.ValorVendido, 2, MidpointRounding.AwayFromZero) > Math.Round(mip.Valor, 2, MidpointRounding.AwayFromZero)))
                    {
                        MaterialItemProjeto material = mip;

                        var idObra = PedidoConfig.DadosPedido.UsarControleNovoObra
                            ? PedidoDAO.Instance.GetIdObra(sessao, pedidoEspelho.IdPedido)
                            : null;
                        
                        // Verifica qual preço deverá ser utilizado
                        ProdutoObraDAO.DadosProdutoObra dadosObra = idObra > 0 
                            ? ProdutoObraDAO.Instance.IsProdutoObra(sessao, idObra.Value, mip.IdProd)
                            : null;

                        var container = new ContainerCalculoDTO(pedidoEspelho);
                        container.Reposicao = itemProj.Reposicao;

                        mip.InicializarParaCalculo(sessao, container);

                        material.Valor = dadosObra != null && dadosObra.ProdutoValido
                            ? dadosObra.ValorUnitProduto
                            : (mip as IProdutoCalculo).DadosProduto.ValorTabela();

                        MaterialItemProjetoDAO.Instance.CalcTotais(sessao, ref material, false);
                        MaterialItemProjetoDAO.Instance.UpdateBase(sessao, material);

                        ItemProjetoDAO.Instance.UpdateTotalItemProjeto(sessao, itemProj.IdItemProjeto);
                    }
                    
                    /* Chamado 50709. */
                    if (associacaoProdutosPedidoProdutosPedidoEspelho != null)
                    {
                        var idProdPedIdMaterItemProj = (int)ProdutosPedidoDAO.Instance.GetIdProdPedByMaterItemProj(sessao, pedidoEspelho.IdPedido, mip.IdMaterItemProjOrig.GetValueOrDefault());
 
                        if (!associacaoProdutosPedidoProdutosPedidoEspelho.ContainsKey(idProdPedIdMaterItemProj))
                            associacaoProdutosPedidoProdutosPedidoEspelho.Add(idProdPedIdMaterItemProj, (int)prodPed.IdProdPed);
                    }

                    prod.Add(prodPed);

                    // Altera as imagens que possam ter sido inseridas anteriormente para ficarem associadas às novas peças inseridas
                    // Apaga possiveis arquivos DXF editados anteriormente.
                    if (prodPed.IdMaterItemProj > 0 && dicProdPedMater.ContainsKey(prodPed.IdMaterItemProj.Value))
                        AtualizarEdicaoImagemPecaArquivoMarcacao((int)dicProdPedMater[prodPed.IdMaterItemProj.Value], (int)prodPed.IdProdPed, medidasAlteradas);
                }

                #region Mantém acréscimo/desconto/comissão originais

                ProdutosPedidoEspelho[] prodProj = prod.ToArray();
                
                // Aplica acréscimo, desconto e comissão
                if (valorAcrescimoAplicado > 0)
                    AplicaAcrescimoProdProj(sessao, pedidoEspelho, valorAcrescimoAplicado, prodProj);

                if (valorDescontoAplicado > 0)
                    AplicaDescontoProdProj(sessao, pedidoEspelho, valorDescontoAplicado, prodProj);

                if (percComissao > 0)
                    AplicaComissaoProdProj(sessao, pedidoEspelho, percComissao, prodProj);

                // Atualiza os produtos
                foreach (var p in prodProj)
                    if (valorAcrescimoAplicado > 0 || valorDescontoAplicado > 0 || (percComissao > 0 && PedidoConfig.Comissao.ComissaoAlteraValor))
                    {
                        UpdateCompleto(sessao, pedidoEspelho, p, false, atualizaReserva);
                    }
                    else
                    {
                        CriarClone(sessao, pedidoEspelho, p, atualizaReserva, false);
                    }

                #endregion

                // Verifica se o itemProjeto possui referência do idPedido (Ocorreu de não estar associado)
                if (itemProj.IdPedidoEspelho == null && itemProj.IdPedido == null)
                    objPersistence.ExecuteCommand(sessao, "Update item_projeto Set idPedidoEspelho=" + pedidoEspelho.IdPedido + " Where idItemProjeto=" + itemProj.IdItemProjeto);

                // Aplica acréscimo e desconto no ambiente
                if (OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento
                    && (ambienteEspelho.Acrescimo > 0 || ambienteEspelho.Desconto > 0))
                {
                    var produtosPedidoEspelho = GetByAmbiente(sessao, ambienteEspelho.IdAmbientePedido);

                    if (ambienteEspelho.Acrescimo > 0)
                        AmbientePedidoEspelhoDAO.Instance.AplicarAcrescimo(sessao, pedidoEspelho, ambienteEspelho.IdAmbientePedido, ambienteEspelho.TipoAcrescimo, ambienteEspelho.Acrescimo, produtosPedidoEspelho);

                    if (ambienteEspelho.Desconto > 0)
                        AmbientePedidoEspelhoDAO.Instance.AplicarDesconto(sessao, pedidoEspelho, ambienteEspelho.IdAmbientePedido, ambienteEspelho.TipoDesconto, ambienteEspelho.Desconto, produtosPedidoEspelho);

                    AmbientePedidoEspelhoDAO.Instance.FinalizarAplicacaoAcrescimoDesconto(sessao, pedidoEspelho, produtosPedidoEspelho, true);
                }

                // Atualiza o total do pedido
                PedidoEspelhoDAO.Instance.UpdateTotalPedido(sessao, pedidoEspelho, true);

                return idAmbientePedidoEsp.GetValueOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Recupera todos os produtos de pedido associados ao item de projeto informado, para remover a edição das imagens e de arquivos de marcação.
        /// </summary>
        public void AtualizarEdicaoImagemPecaArquivoMarcacao(GDASession session, int idItemProjeto)
        {
            var idsProdPed = ExecuteMultipleScalar<int?>(session, string.Format("SELECT IdProdPed FROM produtos_pedido_espelho WHERE IdItemProjeto={0}", idItemProjeto));

            if (idsProdPed == null || idsProdPed.Count == 0)
                return;

            foreach (var idProdPed in idsProdPed)
                if (idProdPed > 0)
                    AtualizarEdicaoImagemPecaArquivoMarcacao(idProdPed.Value, null, true);
        }

        /// <summary>
        /// Remove ou mantém a edição das imagens e de arquivos de marcação, de acordo com a configuração ManterImagensEditadasAoConfirmarProjeto e o parâmetro medidasAlteradas.
        /// </summary>
        public void AtualizarEdicaoImagemPecaArquivoMarcacao(int idProdPedAtual, int? idProdPedNovo, bool medidasAlteradas)
        {
            var arquivosImagem = Directory.GetFiles(Utils.GetPecaProducaoPath, string.Format("{0}_*", idProdPedAtual.ToString().PadLeft(10, '0')));

            foreach (var arquivoImagem in arquivosImagem)
            {
                if (idProdPedNovo > 0 && (!medidasAlteradas || ProjetoConfig.GerarPecasComMedidasIncoerentesDaImagemEditada) && ProjetoConfig.ManterImagensEditadasAoConfirmarProjeto)
                    File.Copy(arquivoImagem, arquivoImagem.Replace(idProdPedAtual.ToString().PadLeft(10, '0'), idProdPedNovo.ToString().PadLeft(10, '0')));

                File.Delete(arquivoImagem);
            }

            /* Chamado 49119.
             * Caso as medidas tenham sido alteradas o arquivo deve ser excluído.
             * Caso as medidas NÃO tenham sido alteradas o arquivo deve ser renomeado com o novo IDPRODPED inserido. */
            var caminhoDxf = string.Format("{0}{1}.dxf", PCPConfig.CaminhoSalvarCadProject(true), idProdPedAtual);
            if (File.Exists(caminhoDxf))
            {
                if (idProdPedNovo.GetValueOrDefault() == 0 || medidasAlteradas || !ProjetoConfig.ManterImagensEditadasAoConfirmarProjeto)
                    File.Delete(caminhoDxf);
                else
                    File.Move(caminhoDxf, string.Format("{0}{1}.dxf", PCPConfig.CaminhoSalvarCadProject(true), idProdPedNovo));
            }

            var caminhoSvg = string.Format("{0}{1}.svg", PCPConfig.CaminhoSalvarCadProject(true), idProdPedAtual);
            if (File.Exists(caminhoSvg))
            {
                if (idProdPedNovo.GetValueOrDefault() == 0 || medidasAlteradas || !ProjetoConfig.ManterImagensEditadasAoConfirmarProjeto)
                    File.Delete(caminhoSvg);
                else
                    File.Move(caminhoSvg, string.Format("{0}{1}.svg", PCPConfig.CaminhoSalvarCadProject(true), idProdPedNovo));
            }
        }

        #endregion

        #region Atualiza campos

        ///// <summary>
        ///// Atualiza campo forma
        ///// </summary>
        ///// <param name="idProdPed"></param>
        //public void AtualizaForma(uint idProdPed, string forma)
        //{
        //    string sql = "Update produtos_pedido_espelho Set forma=?forma Where idProdPed=" + idProdPed;

        //    objPersistence.ExecuteCommand(sql, new GDAParameter("?forma", forma));
        //}

        /// <summary>
        /// Atualiza observação do produto do pedido
        /// </summary>
        public void AtualizaObs(uint idProdPed, string obs)
        {
            string sql = "Update produtos_pedido_espelho Set obs=?obs Where idprodped=" + idProdPed;

            objPersistence.ExecuteCommand(sql, new GDAParameter("?obs", obs));
        }

        #endregion

        #region Retorna a quantidade de peças de vidro no pedido passado

        /// <summary>
        /// Retorna a quantidade de peças de vidro no pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObtemQtdPecasVidroPedido(GDASession session, uint idPedido)
        {
            string sql = String.Empty;

            if (!PedidoDAO.Instance.IsMaoDeObra(session, idPedido))
            {
                sql = @"
                    Select Sum(ppe.qtde) 
                    From produtos_pedido_espelho ppe 
                        Inner Join produto p On (ppe.idProd=p.idProd)
                    Where (ppe.invisivelFluxo=false or ppe.invisivelFluxo is null)
                        AND IdProdPedParent IS NULL
                        and p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" 
                        And idPedido=" + idPedido;
            }
            else
                sql = @"Select Sum(ape.qtde) From ambiente_pedido_espelho ape Where idPedido=" + idPedido;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(session, sql).ToString());
        }

        /// <summary>
        /// Retorna a quantidade de peças de vidro de estoque do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObtemQtdPecasVidroEstoquePedido(uint idPedido)
        {
            string sql = String.Empty;

            sql = @"
                Select Sum(ppe.qtde) 
                From produtos_pedido_espelho ppe 
                    Inner Join produto p On (ppe.idProd=p.idProd)
                    Inner Join subgrupo_prod s On (p.idSubgrupoProd=s.idSubgrupoProd)
                    Inner Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd)
                Where (ppe.invisivelFluxo=false or ppe.invisivelFluxo is null)
                    And p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" 
                    And coalesce(s.tipoCalculo, g.tipoCalculo)=" + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @"
                    And s.produtosEstoque=true
                    And idPedido=" + idPedido;

            object obj = objPersistence.ExecuteScalar(sql);

            return obj != null && obj.ToString() != String.Empty ? Glass.Conversoes.StrParaInt(obj.ToString()) : 0;
        }

        #endregion

        #region Recupera o ID do ambiente

        /// <summary>
        /// Recupera o ID do ambiente do produto.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public uint? GetAmbientePedidoByProdPed(uint idProdPed)
        {
            object retorno = objPersistence.ExecuteScalar("select idAmbientePedido from produtos_pedido_espelho where idProdPed=" + idProdPed);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? (uint?)Glass.Conversoes.StrParaUint(retorno.ToString()) : null;
        }

        #endregion

        #region Recupera um ProdutoPedidoEspelho para a imagem da peça

        public ProdutosPedidoEspelho GetForImagemPeca(uint idProdPed)
        {
            return GetForImagemPeca(null, idProdPed);
        }

        public ProdutosPedidoEspelho GetForImagemPeca(GDASession sessao, uint idProdPed)
        {
            string where = "idProdPed=" + idProdPed;

            ProdutosPedidoEspelho retorno = new ProdutosPedidoEspelho();
            retorno.IdProdPed = idProdPed;
            retorno.IdPedido = ObtemValorCampo<uint>(sessao, "idPedido", where);
            retorno.IdItemProjeto = ObtemValorCampo<uint?>(sessao, "idItemProjeto", where);
            retorno.IdMaterItemProj = ObtemValorCampo<uint?>(sessao, "idMaterItemProj", where);
            retorno.IdProd = ObtemValorCampo<uint>(sessao, "idProd", where);

            return retorno;
        }

        #endregion

        #region Verifica se o pedido possui produtos do grupo vidro calculado por m²

        /// <summary>
        /// Verifica se o pedido possui produtos do grupo vidro calculado por m²
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiVidroCalcM2(uint idPedido)
        {
             return PossuiVidroCalcM2(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido possui produtos do grupo vidro calculado por m²
        /// </summary>
        /// <param name="idPedido"></param>
        public bool PossuiVidroCalcM2(GDASession sessao, uint idPedido)
        {
            string sql = @"
                Select Count(*) From produtos_pedido_espelho pp 
                    Inner Join produto p On (pp.idProd=p.idProd)
                    Inner Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd)
                    Left Join subgrupo_prod sg On (p.idSubgrupoProd=sg.idSubgrupoProd)
                Where idPedido=" + idPedido + @" 
                    And (p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.MaoDeObra + @"
                    Or (p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @"
                    And coalesce(sg.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + ") in (" +
                    (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + ")))";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Verifica se a peça possui alguma imagem associada

        /// <summary>
        /// Verifica se a peça possui alguma imagem associada
        /// </summary>
        /// <param name="idPedido"></param>
        public bool PossuiImagemAssociada(uint idProdPed)
        {
            return PossuiImagemAssociada(null, idProdPed);
        }

        /// <summary>
        /// Verifica se a peça possui alguma imagem associada
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        public bool PossuiImagemAssociada(GDASession session, uint idProdPed)
        {
            ProdutosPedidoEspelho ppe = ProdutosPedidoEspelhoDAO.Instance.GetForImagemPeca(session, idProdPed);

            if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(ppe.ImagemUrlSalvar)))
                return true;

            for (int i = 1; i <= 10; i++)
            {
                ppe.Item = i;
                if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(ppe.ImagemUrlSalvarItem)))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Verifica se existe um arquivo dxf editado no cad project
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public bool PossuiEdicaoCadProject(uint idProdPed)
        {
            var caminho = PCPConfig.CaminhoSalvarCadProject(true) + idProdPed + ".dxf";

            return File.Exists(caminho);
        }

        #endregion

        #region Verifica se um produto existe

        /// <summary>
        /// Verifica se um produto existe.
        /// </summary>
        public new bool Exists(GDASession session, uint idProdPed)
        {
            string sql = "select count(*) from produtos_pedido_espelho where idProdPed=" + idProdPed;
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Verifica se o produto é redondo

        /// <summary>
        /// Verifica se o produto é redondo
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public bool IsRedondo(uint idProdPed)
        {
            return IsRedondo(null, idProdPed);
        }

        /// <summary>
        /// Verifica se o produto é redondo
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public bool IsRedondo(GDASession session, uint idProdPed)
        {
            return ExecuteScalar<bool>(session, "Select Count(*)>0 From produtos_pedido_espelho Where redondo=true And idProdPed=" + idProdPed);
        }

        #endregion

        #region Recalcula os valores de um produto

        /// <summary>
        /// Recalcula os valores unitários e totais brutos e líquidos.
        /// </summary>
        internal void RecalcularValores(GDASession session, ProdutosPedidoEspelho prodPed, PedidoEspelho pedidoEspelho,
            bool somarAcrescimoDesconto)
        {
            GenericBenefCollection benef = prodPed.Beneficiamentos;
            decimal valorBenef = prodPed.ValorBenef;

            try
            {
                prodPed.Beneficiamentos = GenericBenefCollection.Empty;
                prodPed.ValorBenef = 0;

                ValorBruto.Instance.Calcular(session, pedidoEspelho, prodPed);
                
                var valorUnitario = ValorUnitario.Instance.RecalcularValor(session, pedidoEspelho, prodPed, !somarAcrescimoDesconto);
                prodPed.ValorVendido = valorUnitario ?? (prodPed as IProdutoCalculo).DadosProduto.ValorTabela();

                ValorTotal.Instance.Calcular(
                    session,
                    pedidoEspelho,
                    prodPed,
                    Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarApenasCalculo,
                    true,
                    prodPed.Beneficiamentos.CountAreaMinimaSession(session)
                );

                // Atualiza o total do produto com os descontos e acréscimos
                if (PedidoConfig.RatearDescontoProdutos)
                {
                    prodPed.Total -= prodPed.ValorDesconto + prodPed.ValorDescontoProd + prodPed.ValorDescontoQtde;
                }

                prodPed.Total += prodPed.ValorAcrescimo + prodPed.ValorAcrescimoProd;
                ValorBruto.Instance.Calcular(session, pedidoEspelho, prodPed);

                if (!PedidoConfig.RatearDescontoProdutos)
                {
                    prodPed.Total -= prodPed.ValorDesconto + prodPed.ValorDescontoProd + prodPed.ValorDescontoQtde;
                }
            }
            finally
            {
                prodPed.Beneficiamentos = benef;
                prodPed.ValorBenef = valorBenef;
            }
        }

        #endregion

        #region Verifica se deve gerar projeto para CNC

        public bool DeveGerarProjCNC(ProdutosPedidoEspelho prodPedEsp)
        {
            return DeveGerarProjCNC(null, prodPedEsp);
        }

        public bool DeveGerarProjCNC(GDASession sessao, ProdutosPedidoEspelho prodPedEsp)
        {
            if (prodPedEsp.IdMaterItemProj.GetValueOrDefault(0) == 0 ||
                MaterialItemProjetoDAO.Instance.ObtemidPecaItemProj(sessao, prodPedEsp.IdMaterItemProj.Value) == 0)
            {
                if (prodPedEsp.IdProcesso.GetValueOrDefault() == 0)
                    return false;

                int tipoProcesso = EtiquetaProcessoDAO.Instance.ObtemTipoProcesso(sessao, prodPedEsp.IdProcesso.GetValueOrDefault());

                //Verifica se a peça avulsa é de instalação. Se não for instalação passa para proxima.
                if (prodPedEsp.IdProcesso.GetValueOrDefault(0) == 0 || tipoProcesso != (int)EtiquetaTipoProcesso.Instalacao)
                    return false;

                // Se o produto avulso for de instalação verifica se o mesmo tem arquivo SAG.
                if (ProdutoDAO.Instance.ObtemIdArquivoMesaCorte(sessao, prodPedEsp.IdProd).GetValueOrDefault() == 0 ||
                    ProdutoDAO.Instance.ObterTipoArquivoMesaCorte(sessao, (int)prodPedEsp.IdProd).GetValueOrDefault() == TipoArquivoMesaCorte.FMLBasico ||
                    tipoProcesso == (int)EtiquetaTipoProcesso.Instalacao)
                    return true;
            }
            else //Se o item for de projeto
            {
                if (prodPedEsp.IdMaterItemProj.GetValueOrDefault() == 0)
                    return false;

                var idPecaItemProj = MaterialItemProjetoDAO.Instance.ObtemidPecaItemProj(sessao, prodPedEsp.IdMaterItemProj.Value);

                if (idPecaItemProj == 0)
                    return false;

                var pecaItemProjeto = PecaItemProjetoDAO.Instance.GetElement(sessao, idPecaItemProj);

                //Se a peça não for instalação pula pra proxima
                if (pecaItemProjeto == null || pecaItemProjeto.Tipo != 1)
                    return false;

                //Se a peça não tiver arquivo de mesa ou se o arquivo de mesa for FML básico ou se a imagem tiver sido editada, deve projetar.
                uint idArquivoMesaCorte = pecaItemProjeto.IdArquivoMesaCorte.GetValueOrDefault();
                if (idArquivoMesaCorte == 0 || pecaItemProjeto.TipoArquivoMesaCorte.GetValueOrDefault() == (int)TipoArquivoMesaCorte.FMLBasico ||
                    PecaItemProjetoDAO.Instance.PossuiFiguraAssociada(sessao, idPecaItemProj) || pecaItemProjeto.ImagemEditada)
                    return true;
            }

            return false;
        }

        #endregion

        #region Verifica se peça possui FML/DXF/SGLASS

        /// <summary>
        /// Verifica se a peça possui FML associado
        /// </summary>
        public bool PossuiFml(GDASession sessao, uint idProdPed, string etiqueta, bool paraDestaqueEtiqueta)
        {
            // Se não tiver Id Produto Pedido não possui FML
            if (idProdPed == 0)
                return false;

            var idMaterItemProj = Instance.ObtemValorCampo<uint>(sessao, "idMaterItemProj", "idProdPed=" + idProdPed);
            var pecaProjMod = PecaItemProjetoDAO.Instance.GetByMaterial(sessao, idMaterItemProj);

            // Se o pedido for importado, verifica se o mesmo possui arquivo FML
            /* Chamado 23840. */
            if (pecaProjMod == null && PCPConfig.EmpresaGeraArquivoFml)
            {
                var idPedido = Instance.ObtemIdPedido(idProdPed);
                var pedidoImportado = PedidoDAO.Instance.IsPedidoImportado(idPedido);

                if (pedidoImportado)
                {
                    string forma;
                    var nomeArquivo = ImpressaoEtiquetaDAO.Instance.ObterNomeArquivo(null, null, TipoArquivoMesaCorte.FML, (int)idProdPed, etiqueta, false, out forma, false);

                    using (Glass.Seguranca.AutenticacaoRemota.Autenticar())
                        try
                        {
                            if (File.Exists(PCPConfig.CaminhoSalvarFml + nomeArquivo))
                                return true;
                        }
                        catch { }
                }
            }

            // Se não tiver peça projeto modelo verifica pelo produto do pedido.
            if (pecaProjMod == null)
            {
                var idProduto = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(sessao, idProdPed);
                var tipoArquivo = ProdutoDAO.Instance.ObterTipoArquivoMesaCorte(sessao, (int)idProduto);
                var flags = FlagArqMesaDAO.Instance.ObtemPorProduto(sessao, (int)idProduto, false);

                return (tipoArquivo == TipoArquivoMesaCorte.FML ||
                    flags.Any(f => f.Descricao == TipoArquivoMesaCorte.FML.ToString())) &&
                    !Instance.PossuiImagemAssociada(idProdPed);
            }
            // Se o prodtuto pedido for de um projeto, recupera através da peça projeto modelo.
            else
            {
                var tipoArquivo = PecaProjetoModeloDAO.Instance.ObtemTipoArquivoMesaCorte(pecaProjMod.IdPecaProjMod);
                var flags = FlagArqMesaDAO.Instance.ObtemPorPecaProjMod((int)pecaProjMod.IdPecaProjMod, !paraDestaqueEtiqueta);

                return (tipoArquivo == TipoArquivoMesaCorte.FML ||
                    flags.Any(f => f.Descricao == TipoArquivoMesaCorte.FML.ToString())) &&
                    !pecaProjMod.ImagemEditada &&
                    !Instance.PossuiImagemAssociada(idProdPed) &&
                    !PecaItemProjetoDAO.Instance.PossuiFiguraAssociada(pecaProjMod.IdPecaItemProj);
            }
        }

        /// <summary>
        /// Verifica se a peça possui DXF associado
        /// </summary>
        public bool PossuiDxf(GDASession sessao, uint idProdPed, string etiqueta)
        {
            // Se não tiver Id Produto Pedido não possui DXF
            if (idProdPed == 0)
                return false;

            var idMaterItemProj = Instance.ObtemValorCampo<uint>(sessao, "idMaterItemProj", "idProdPed=" + idProdPed);
            var pecaProjMod = PecaItemProjetoDAO.Instance.GetByMaterial(sessao, idMaterItemProj);

            // Se o pedido for importado, verifica se o mesmo possui arquivo DXF
            /* Chamado 23840. */
            if (pecaProjMod == null && PCPConfig.EmpresaGeraArquivoDxf)
            {
                var idPedido = Instance.ObtemIdPedido(idProdPed);
                var pedidoImportado = PedidoDAO.Instance.IsPedidoImportado(idPedido);

                if (pedidoImportado)
                {
                    string forma;
                    var nomeArquivo = ImpressaoEtiquetaDAO.Instance.ObterNomeArquivo(null, null, TipoArquivoMesaCorte.DXF, (int)idProdPed, etiqueta, false, out forma, false);

                    using (Glass.Seguranca.AutenticacaoRemota.Autenticar())
                        try
                        {
                            if (File.Exists(PCPConfig.CaminhoSalvarDxf + nomeArquivo))
                                return true;
                        }
                        catch { }
                }
            }

            // Se não tiver peça projeto modelo verifica pelo produto do pedido.
            if (pecaProjMod == null)
            {
                var idProduto = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(sessao, idProdPed);
                var tipoArquivo = ProdutoDAO.Instance.ObterTipoArquivoMesaCorte(sessao, (int)idProduto);
                var flags = FlagArqMesaDAO.Instance.ObtemPorProduto(sessao, (int)idProduto, false);

                return (tipoArquivo == TipoArquivoMesaCorte.DXF || flags.Any(f => f.Descricao == TipoArquivoMesaCorte.DXF.ToString())) &&
                    ((!PossuiImagemAssociada(idProdPed)) ||
                    File.Exists(string.Format("{0}{1}.dxf", PCPConfig.CaminhoSalvarCadProject(true), idProdPed)));
            }
            // Se o prodtuto pedido for de um projeto, recupera através da peça projeto modelo.
            else
            {
                var tipoArquivo = PecaProjetoModeloDAO.Instance.ObtemTipoArquivoMesaCorte(pecaProjMod.IdPecaProjMod);
                var flags = FlagArqMesaDAO.Instance.ObtemPorPecaProjMod((int)pecaProjMod.IdPecaProjMod, true);

                return (tipoArquivo == TipoArquivoMesaCorte.DXF ||
                    flags.Any(f => f.Descricao == TipoArquivoMesaCorte.DXF.ToString())) &&
                    ((!pecaProjMod.ImagemEditada &&
                    !PossuiImagemAssociada(idProdPed) &&
                    !PecaItemProjetoDAO.Instance.PossuiFiguraAssociada(pecaProjMod.IdPecaItemProj)) ||
                    File.Exists(string.Format("{0}{1}.dxf", PCPConfig.CaminhoSalvarCadProject(true), idProdPed)));
            }
        }

        /// <summary>
        /// Verifica se a peça possui SGLASS associado
        /// </summary>
        public bool PossuiSGlass(GDASession sessao, uint idProdPed, string etiqueta)
        {
            // Se não tiver Id Produto Pedido não possui SGlass
            if (idProdPed == 0)
                return false;
            
            using (Glass.Seguranca.AutenticacaoRemota.Autenticar())
            {
                try
                {
                    string forma;
                    var nomeArquivoDxf = ImpressaoEtiquetaDAO.Instance.ObterNomeArquivo(sessao, null, TipoArquivoMesaCorte.DXF, (int)idProdPed, etiqueta, false, out forma, false);
                    var pathSglass = Path.Combine(PCPConfig.CaminhoSalvarProgramSGlass, string.Format("{0}.drawing", Path.GetFileNameWithoutExtension(nomeArquivoDxf)));

                    /* Chamado 62792. */
                    if (File.Exists(pathSglass))
                        return true;
                }
                catch { }
            }

            var idMaterItemProj = Instance.ObtemValorCampo<uint>(sessao, "idMaterItemProj", "idProdPed=" + idProdPed);
            var pecaProjMod = PecaItemProjetoDAO.Instance.GetByMaterial(sessao, idMaterItemProj);

            // Se não tiver peça projeto modelo verifica pelo produto do pedido.
            if (pecaProjMod == null)
            {
                var idProduto = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(sessao, idProdPed);
                var flags = FlagArqMesaDAO.Instance.ObtemPorProduto(sessao, (int)idProduto, false);

                return flags != null && flags.Any(f => f.Descricao.ToLower() == "sglass") && !PossuiImagemAssociada(sessao, idProdPed);
            }
            // Se o prodtuto pedido for de um projeto, recupera através da peça projeto modelo.
            else
            {
                var flags = FlagArqMesaDAO.Instance.ObtemPorPecaProjMod(sessao, (int)pecaProjMod.IdPecaProjMod, true);

                return flags != null && flags.Any(f => f.Descricao.ToLower() == "sglass") && !pecaProjMod.ImagemEditada && !PossuiImagemAssociada(sessao, idProdPed) &&
                    !PecaItemProjetoDAO.Instance.PossuiFiguraAssociada(sessao, pecaProjMod.IdPecaItemProj);
            }
        }

        /// <summary>
        /// Verifica se a peça possui Intermac associado.
        /// </summary>
        public bool PossuiIntermac(GDASession session, int idProdPed, string etiqueta)
        {
            // Se não tiver Id Produto Pedido não possui Intermac.
            if (idProdPed == 0)
            {
                return false;
            }

            using (Glass.Seguranca.AutenticacaoRemota.Autenticar())
            {
                try
                {
                    var forma = string.Empty;
                    var nomeArquivoIntermac = ImpressaoEtiquetaDAO.Instance.ObterNomeArquivo(session, null, TipoArquivoMesaCorte.DXF, idProdPed, etiqueta, true, out forma, false);
                    var caminhoArquivoIntermac = Path.Combine(PCPConfig.CaminhoSalvarIntermac, Path.GetFileNameWithoutExtension(nomeArquivoIntermac));
                    
                    if (File.Exists(caminhoArquivoIntermac))
                    {
                        return true;
                    }
                }
                catch { }
            }

            var idMaterItemProj = Instance.ObtemValorCampo<uint>(session, "IdMaterItemProj", string.Format("IdProdPed={0}", idProdPed));
            var pecaProjMod = PecaItemProjetoDAO.Instance.GetByMaterial(session, idMaterItemProj);

            // Se não tiver peça projeto modelo verifica pelo produto do pedido.
            if (pecaProjMod == null)
            {
                var idProduto = ObtemIdProd(session, (uint)idProdPed);
                var flags = FlagArqMesaDAO.Instance.ObtemPorProduto(session, (int)idProduto, false);

                return flags != null && flags.Any(f => f.Descricao.ToLower() == "intermac") && !PossuiImagemAssociada(session, (uint)idProdPed);
            }
            // Se o prodtuto pedido for de um projeto, recupera através da peça projeto modelo.
            else
            {
                var flags = FlagArqMesaDAO.Instance.ObtemPorPecaProjMod(session, (int)pecaProjMod.IdPecaProjMod, true);
                var pecaPossuiImagemAssociada = PecaItemProjetoDAO.Instance.PossuiFiguraAssociada(session, pecaProjMod.IdPecaItemProj) || PossuiImagemAssociada(session, (uint)idProdPed);
                var pecaPossuiFlagIntermac = flags != null && flags.Any(f => f.Descricao.ToLower() == "intermac");

                return pecaPossuiFlagIntermac && !pecaPossuiImagemAssociada && !pecaProjMod.ImagemEditada;
            }
        }

        #endregion

        #region Verifica se há M2 disponível para o produto

        /// <summary>
        /// Verifica se o pedido de revenda associado ao de produção permite a inserção de um novo item
        /// </summary>
        public bool PedidoReferenciadoPermiteInsercao(GDASession sessao, ProdutosPedidoEspelho prodPed)
        {
            var idPedRef = PedidoDAO.Instance.ObterIdPedidoRevenda(sessao, (int)prodPed.IdPedido);

            if (idPedRef == 0)
                return true;

            var produtosPedProducao = GetByPedido(sessao, prodPed.IdPedido, false);
            var produtosPedRevenda = ProdutosPedidoDAO.Instance.GetByPedido(sessao, (uint)idPedRef);

            var idsProd = ((produtosPedRevenda.Select(f => f.IdProd).Distinct())).Where(f => ProdutoDAO.Instance.ObterProdutoBase(sessao, (int)f) == prodPed.IdProd).ToList();
            if (idsProd == null || idsProd.Count == 0)
                return false;

            uint idCliente = PedidoDAO.Instance.ObtemIdCliente(sessao, prodPed.IdPedido);
            float qtde = prodPed.Qtde;
            int qtdeAmbiente = PedidoDAO.Instance.IsMaoDeObra(sessao, prodPed.IdPedido)
                ? AmbientePedidoDAO.Instance.GetQtde(sessao, prodPed.IdAmbientePedido)
                : 1;
            int alturaBenef = prodPed.AlturaBenef != null ? prodPed.AlturaBenef.Value : 2;
            int larguraBenef = prodPed.LarguraBenef != null ? prodPed.LarguraBenef.Value : 2;
            decimal total = prodPed.Total, custoProd = prodPed.CustoCompraProduto;
            Single totM2 = prodPed.TotM, altura = prodPed.Altura, totM2Calc = prodPed.TotM2Calc;
            bool redondo = prodPed.Redondo ||
                           (prodPed.IdAmbientePedido > 0
                               ? AmbientePedidoDAO.Instance.IsRedondo(sessao, prodPed.IdAmbientePedido.Value)
                               : false);
            var isPedidoProducaoCorte = PedidoDAO.Instance.IsPedidoProducaoCorte(sessao, prodPed.IdPedido);

            ProdutoDAO.Instance.CalcTotaisItemProd(null, idCliente, (int)prodPed.IdProd,
                prodPed.Largura, qtde, qtdeAmbiente, prodPed.ValorVendido, prodPed.Espessura,
                redondo, 0, false, !isPedidoProducaoCorte, ref custoProd, ref altura, ref totM2, ref totM2Calc, ref total,
                alturaBenef, larguraBenef, false, prodPed.Beneficiamentos.CountAreaMinimaSession(sessao), true);

            var totMRevenda = produtosPedRevenda.Where(f => idsProd.Contains(f.IdProd)).Sum(f => f.TotM2Calc);
            var totMProducao = produtosPedProducao.Where(f => f.IdProdPed != prodPed.IdProdPed && f.IdProd == prodPed.IdProd).Sum(f => f.TotM2Calc) + totM2Calc;

            if (totMProducao > totMRevenda)
                return false;

            return true;
        }

        #endregion

        #region Verifica se o produto é Laminado Composição

        /// <summary>
        /// Verifica se o produto de pedido informado é um produto de composição.
        /// </summary>
        public bool IsProdutoLaminadoComposicao(uint idProdPed)
        {
            var idProd = ObtemIdProd(idProdPed);
            var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)idProd);

            return tipoSubgrupo == TipoSubgrupoProd.VidroDuplo || tipoSubgrupo == TipoSubgrupoProd.VidroLaminado;
        }

        #endregion

        #region Métodos sobrescritos

        #region Clone

        internal void RemoverClone(GDASession sessao, IContainerCalculo container, uint idProdPed)
        {
            string idsProdPed = String.Join(",",
            ExecuteMultipleScalar<string>(sessao, "select idProdPed from produtos_pedido_espelho where idPedido=" + container.IdPedido()).ToArray());

            if (string.IsNullOrEmpty(idsProdPed))
                idsProdPed = "0";
            
            // Remove todos os clones que não fazem referência a nenhum produto no PCP
            objPersistence.ExecuteCommand(sessao, 
                string.Format(@"delete from produtos_pedido where invisivelPedido=true and idProdPedEsp is not null and
                    idProdPedEsp not in ({0}) And idPedido={1}", idsProdPed, container.IdPedido()));

            ProdutosPedido clone = ProdutosPedidoDAO.Instance.GetByProdPedEsp(sessao, idProdPed, false);

            // Se o produto do pedido espelho não estiver invisível e se o clone não for o produto original do pedido exclui o clone
            if (objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from produtos_pedido where invisivelPedido=true and idProdPedEsp=" + idProdPed) > 0 &&
                objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from produtos_pedido_espelho where idItemProjeto is null and invisivelFluxo=true and idProdPed=" + idProdPed) == 0)
            {
                objPersistence.ExecuteCommand(sessao, "delete from produtos_pedido where idProdPedEsp=" + idProdPed);
            }
            else
            {
                // Marca o original como invisível para o fluxo
                objPersistence.ExecuteCommand(sessao, "update produtos_pedido set InvisivelFluxo=true" +
                    (objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from produtos_pedido_espelho where invisivelFluxo=true and " +
                    "idProdPed=" + idProdPed) == 0 ? ", idProdPedEsp=null" : "") + " where idProdPedEsp=" + idProdPed);
            }

            // Retira da reserva
            // apenas para liberação porque ainda falta ser tratada a questão de saída de estoque da confirmação
            /*if (clone != null)
            {
                var qtde = clone.Qtde;

                var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(sessao, (int)clone.IdProd);
                var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 || tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;

                if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6)
                {
                    qtde *= clone.Altura;
                }

                var idLoja = (int)PedidoDAO.Instance.ObtemIdLoja(sessao, clone.IdPedido);
                ProdutoLojaDAO.Instance.TirarReserva(sessao, idLoja,
                    new Dictionary<int, float>() { { (int)clone.IdProd, m2 ? clone.TotM : qtde } });
            }*/
        }

        /// <summary>
        /// Cria o clone do produto do pedido espelho.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="prodPedEsp"></param>
        /// <param name="atualizaReserva"></param>
        /// <param name="forcarCriarClone">Define se mesmo que o Config "Criar Clone" esteja marcado o clone deve ser criado.</param>
        internal void CriarClone(GDASession sessao, IContainerCalculo container, ProdutosPedidoEspelho prodPedEsp, bool atualizaReserva, bool forcarCriarClone)
        {
            /* Chamado 22242.
             * O clone sempre deve ser criado quando uma peça for marcada como perdida, caso contrário,
             * não é possível gerar a reposição do pedido, pois, a peça não aparece. */
            //if (!PCPConfig.CriarClone)
            if (!PCPConfig.CriarClone && !forcarCriarClone)
                return;

            // Cria uma cópia do produto na tabela ProdutosPedido, com referência ao produto que está sendo inserido e invisível ao pedido
            ProdutosPedido clone = new ProdutosPedido();
            clone.InvisivelPedido = true;
            clone.IdPedido = prodPedEsp.IdPedido;
            clone.IdProdPedEsp = prodPedEsp.IdProdPed;

            // Se estiver configurado para dar saída automática ao confirmar pedido, cria o clone já marcado como tendo dado saída.
            if (FinanceiroConfig.Estoque.SaidaEstoqueAutomaticaAoConfirmar && !PedidoConfig.LiberarPedido)
                clone.QtdSaida = prodPedEsp.Qtde;

            clone.AliqIcms = prodPedEsp.AliqIcms;
            clone.Altura = prodPedEsp.Altura;
            clone.AlturaReal = prodPedEsp.AlturaReal;
            clone.Espessura = prodPedEsp.Espessura;
            clone.IdAmbientePedido = prodPedEsp.IdAmbientePedido == null ? null :
                AmbientePedidoEspelhoDAO.Instance.ObtemIdAmbientePedidoOriginal(sessao, prodPedEsp.IdAmbientePedido.Value);
            clone.IdAplicacao = prodPedEsp.IdAplicacao;
            clone.IdItemProjeto = prodPedEsp.IdItemProjeto;
            clone.IdProcesso = prodPedEsp.IdProcesso;
            clone.IdProd = prodPedEsp.IdProd;
            clone.Largura = prodPedEsp.Largura;
            clone.Qtde = prodPedEsp.Qtde;
            clone.Total = prodPedEsp.Total;
            clone.TotM = prodPedEsp.TotM;
            clone.TotM2Calc = prodPedEsp.TotM2Calc;
            clone.ValorIcms = prodPedEsp.ValorIcms;
            clone.ValorIpi = prodPedEsp.ValorIpi;
            clone.ValorVendido = prodPedEsp.ValorVendido;
            clone.Redondo = prodPedEsp.Redondo;
            clone.PedCli = prodPedEsp.PedCli;
            clone.Beneficiamentos = prodPedEsp.Beneficiamentos;
            clone.Peso = prodPedEsp.Peso;
            clone.ValorBenef = prodPedEsp.ValorBenef;
            clone.AlturaBenef = prodPedEsp.AlturaBenef;
            clone.LarguraBenef = prodPedEsp.LarguraBenef;
            
            clone.ValorComissao = prodPedEsp.ValorComissao;
            clone.ValorAcrescimo = prodPedEsp.ValorAcrescimo;
            clone.ValorAcrescimoCliente = prodPedEsp.ValorAcrescimoCliente;
            clone.ValorAcrescimoProd = prodPedEsp.ValorAcrescimoProd;
            clone.ValorDesconto = prodPedEsp.ValorDesconto;
            clone.ValorDescontoCliente = prodPedEsp.ValorDescontoCliente;
            clone.ValorDescontoProd = prodPedEsp.ValorDescontoProd;
            clone.ValorDescontoQtde = prodPedEsp.ValorDescontoQtde;
            clone.IdProdPedParent = prodPedEsp.IdProdPedParentOrig;

            // Calcula o custo do produto
            var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(sessao, (int)prodPedEsp.IdProd);
            clone.CustoProd = CalculosFluxo.CalcTotaisItemProdFast(sessao, tipoCalculo, clone.Altura, clone.Largura, clone.Qtde,
                clone.TotM, ProdutoDAO.Instance.ObtemCustoCompra(sessao, (int)clone.IdProd), clone.AlturaBenef.GetValueOrDefault(2), 
                clone.LarguraBenef.GetValueOrDefault(2));

            var idProdPed = ProdutosPedidoDAO.Instance.InsertBase(sessao, clone, container);
            
            // Atualiza a quantidade invisível e o peso do produto clone
            objPersistence.ExecuteCommand(sessao, "update produtos_pedido set qtdeInvisivel=?qtde, peso=?peso where idProdPed=" + idProdPed,
                new GDAParameter("?qtde", prodPedEsp.QtdeInvisivel), new GDAParameter("?peso", prodPedEsp.Peso));

            /*if (atualizaReserva)
            {
                // Coloca em reserva
                var qtde = prodPedEsp.Qtde;

                var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 || tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;

                if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6)
                {
                    qtde *= prodPedEsp.Altura;
                }

                var idLoja = (int)PedidoDAO.Instance.ObtemIdLoja(sessao, prodPedEsp.IdPedido);
                ProdutoLojaDAO.Instance.ColocarReserva(sessao, idLoja,
                    new Dictionary<int, float> { { (int)prodPedEsp.IdProd, m2 ? prodPedEsp.TotM : qtde } });
            }*/
        }

        #endregion

        #region Insert

        public uint InsertBase(GDASession session, ProdutosPedidoEspelho objInsert, PedidoEspelho pedidoEspelho)
        {
            CalculaDescontoEValorBrutoProduto(session, objInsert, pedidoEspelho);

            if (objInsert.Espessura == 0 && ProdutoDAO.Instance.ObtemIdGrupoProd(session, (int)objInsert.IdProd) == (int)NomeGrupoProd.Vidro)
            {
                objInsert.Espessura = ProdutoDAO.Instance.ObtemEspessura(session, (int)objInsert.IdProd);
            }

            // Cria uma cópia do produto na tabela ProdutosPedido, com referência ao produto que está sendo inserido e invisível ao pedido.
            objInsert.IdProdPed = base.Insert(session, objInsert);

            AtualizaBenef(session, objInsert.IdProdPed, objInsert.Beneficiamentos, pedidoEspelho);
            objInsert.RefreshBeneficiamentos();

            return objInsert.IdProdPed;
        }

        private uint InsertFromProjeto(GDASession sessao, ProdutosPedidoEspelho objInsert, PedidoEspelho pedidoEspelho)
        {
            ValorTotal.Instance.Calcular(
                sessao,
                pedidoEspelho,
                objInsert,
                Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarEAtualizarProduto,
                true,
                objInsert.Beneficiamentos.CountAreaMinimaSession(sessao)
            );

            return InsertBase(sessao, objInsert, pedidoEspelho);
        }

        /// <summary>
        /// Atualiza o valor do pedido espelho ao incluir um produto ao mesmo
        /// </summary>
        public uint InsertComTransacao(ProdutosPedidoEspelho objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Insert(transaction, objInsert);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Atualiza o valor do pedido espelho ao incluir um produto ao mesmo
        /// </summary>
        public override uint Insert(ProdutosPedidoEspelho objInsert)
        {
            return Insert(null, objInsert);
        }

        /// <summary>
        /// Atualiza o valor do pedido espelho ao incluir um produto ao mesmo
        /// </summary>
        public override uint Insert(GDASession session, ProdutosPedidoEspelho objInsert)
        {
            var pedidoEspelho = PedidoEspelhoDAO.Instance.GetElement(objInsert.IdPedido);
            return Insert(session, objInsert, pedidoEspelho);
        }

        internal uint Insert(GDASession session, ProdutosPedidoEspelho objInsert, PedidoEspelho pedidoEspelho)
        {
            uint returnValue = 0;
            
            try
            {
                //Valida processo
                if (objInsert.IdSubgrupoProd > 0 && objInsert.IdProcesso.GetValueOrDefault(0) > 0 && !ClassificacaoSubgrupoDAO.Instance.VerificarAssociacaoExistente((int)objInsert.IdSubgrupoProd, (int)objInsert.IdProcesso.GetValueOrDefault(0)))
                    throw new Exception("Este processo não pode ser selecionado para este produto.");

                if (!PedidoReferenciadoPermiteInsercao(session, objInsert))
                    throw new Exception("Não é possível inserir itens diferentes dos inseridos no pedido de revenda associado, ou metragens maiores que as estabelecidas anteriormente.");

                var idLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdLojaPeloProduto(session, (int)objInsert.IdProd);
                if (idLojaSubgrupoProd.GetValueOrDefault(0) > 0 && idLojaSubgrupoProd.Value != PedidoDAO.Instance.ObtemIdLoja(session, objInsert.IdPedido))
                    throw new Exception("Esse produto não pode ser utilizado, pois a loja do seu subgrupo é diferente da loja do pedido.");

                DescontoFormaPagamentoDadosProduto descontoFormPagtoProd = null;
                //Bloqueio de produtos com Grupo e Subgrupo diferentes ao utilizar o controle de desconto por forma de pagamento e dados do produto.
                if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
                {
                    var tipoVenda = pedidoEspelho.TipoVenda;
                    var idFormaPagto = pedidoEspelho.IdFormaPagto;
                    var idTipoCartao = PedidoDAO.Instance.ObtemTipoCartao(session, pedidoEspelho.IdPedido);
                    var idParcela = (pedidoEspelho as IContainerCalculo).IdParcela;
                    var idGrupoProd = objInsert.IdGrupoProd;
                    var idSubgrupoProd = objInsert.IdSubgrupoProd;
                    descontoFormPagtoProd = DescontoFormaPagamentoDadosProdutoDAO.Instance.ObterDescontoFormaPagamentoDadosProduto(session, (uint)tipoVenda, idFormaPagto, idTipoCartao, idParcela, idGrupoProd, idSubgrupoProd);

                    var produtoPedidoEspelhoInserido = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, objInsert.IdPedido, false);
                    if (descontoFormPagtoProd != null && produtoPedidoEspelhoInserido.Count > 0)
                    {
                        if (descontoFormPagtoProd.IdGrupoProd.GetValueOrDefault() > 0 && produtoPedidoEspelhoInserido[0].IdGrupoProd != objInsert.IdGrupoProd)
                            throw new Exception("O grupo do produto novo deve ser igual ao grupo do produto já inserido no pedido ao utilizar o controle de desconto por forma de pagamento e dados do produto.");

                        // Se o DescontoFormaPagamentoDadosProduto tiver subgrupo e o subgrupo do produto atual for diferente de algum já inserido
                        if (descontoFormPagtoProd.IdSubgrupoProd.GetValueOrDefault() > 0 && produtoPedidoEspelhoInserido[0].IdSubgrupoProd != objInsert.IdSubgrupoProd)
                            throw new Exception("O subgrupo do produto novo deve ser igual ao subgrupo do produto já inserido no pedido ao utilizar o controle de desconto por forma de pagamento e dados do produto.");
                    }
                }

                // Verifica se o produto é do grupo vidro.
                if (ProdutoDAO.Instance.IsVidro(session, (int)objInsert.IdProd))
                {
                    // Recupera o id do subgrupo do produto.
                    var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, (int)objInsert.IdProd);
                    if (idSubgrupoProd > 0 && SubgrupoProdDAO.Instance.ObtemTipoCalculo(session, idSubgrupoProd.Value, false) == (int)TipoCalculoGrupoProd.Qtd)
                    {
                        Produto prod = ProdutoDAO.Instance.GetElementByPrimaryKey(session, objInsert.IdProd);

                        // Busca novamente a altura e largura do produto, caso estejam definidas no cadastro de produto
                        if (prod.Altura > 0 || prod.Largura > 0)
                        {
                            objInsert.Altura = (float)prod.Altura;
                            objInsert.AlturaReal = (float)prod.Altura;
                            objInsert.Largura = prod.Largura.GetValueOrDefault();
                        }
                    }
                }

                var isPedidoProducaoCorte = (pedidoEspelho as IContainerCalculo).IsPedidoProducaoCorte;

                if (!objInsert.Redondo && objInsert.IdAmbientePedido > 0 && AmbientePedidoEspelhoDAO.Instance.IsRedondo(session, objInsert.IdAmbientePedido.Value))
                    objInsert.Redondo = true;
                
                ValorTotal.Instance.Calcular(
                    session,
                    pedidoEspelho,
                    objInsert,
                    Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.NaoArredondar,
                    !isPedidoProducaoCorte,
                    objInsert.Beneficiamentos.CountAreaMinimaSession(session)
                );
                
                returnValue = InsertBase(session, objInsert, pedidoEspelho);

                try
                {
                    AtualizaBenef(session, returnValue, objInsert.Beneficiamentos, pedidoEspelho);
                }
                catch { }

                //Calcula Desconto por Forma de Pagamento e Dados do Produto se a configuração estiver habilitada
                if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto && descontoFormPagtoProd != null)
                {
                    pedidoEspelho.Desconto = descontoFormPagtoProd.Desconto;
                    pedidoEspelho.TipoDesconto = 1;
                    PedidoEspelhoDAO.Instance.UpdateDados(session, pedidoEspelho);
                }

                // Cria uma cópia do produto na tabela ProdutosPedido, com referência ao produto que está sendo inserido e invisível ao pedido
                objInsert.IdProdPed = returnValue;
                CriarClone(session, pedidoEspelho, objInsert, true, false);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao incluir Produto no Pedido. Erro: " + ex.Message);
            }

            try
            {
                PedidoEspelhoDAO.Instance.UpdateTotalPedido(session, pedidoEspelho, true);
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message;
                base.Delete(objInsert);
                throw new Exception("Falha ao atualizar Valor do Pedido. Erro: " + msg);
            }

            return returnValue;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Atualiza o valor do pedido ao excluir um produto do mesmo.
        /// </summary>
        public int DeleteComTransacao(ProdutosPedidoEspelho objDelete)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Delete(transaction, objDelete);
                    
                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Atualiza o valor do pedido ao excluir um produto do mesmo
        /// </summary>
        public override int Delete(ProdutosPedidoEspelho objDelete)
        {
            return Delete(null, objDelete);
        }

        /// <summary>
        /// Atualiza o valor do pedido ao excluir um produto do mesmo
        /// </summary>
        public override int Delete(GDASession sessao, ProdutosPedidoEspelho objDelete)
        {
            if (objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From produtos_pedido_espelho Where IdProdPed=" + objDelete.IdProdPed) == 0)
                return 0;

            int returnValue = 0;
            uint idPedido = ObtemIdPedido(sessao, objDelete.IdProdPed);
            var pedidoEspelho = PedidoEspelhoDAO.Instance.GetElement(sessao, idPedido);

            try
            {
                if (String.IsNullOrEmpty(ProdutoImpressaoDAO.Instance.GetIdImpressaoByProdPed(sessao, objDelete.IdProdPed, false)))
                {
                    RemoverClone(sessao, pedidoEspelho, objDelete.IdProdPed);

                    returnValue = base.Delete(sessao, objDelete);
                    ProdutoPedidoEspelhoBenefDAO.Instance.DeleteByProdPed(sessao, objDelete.IdProdPed);
                }
                else
                {
                    returnValue = objPersistence.ExecuteCommand(sessao, "update produtos_pedido_espelho set invisivelFluxo=true " +
                        "where idProdPed=" + objDelete.IdProdPed);

                    RemoverClone(sessao, pedidoEspelho, objDelete.IdProdPed);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao excluir Produto do Pedido. Erro: " + ex.Message);
            }

            try
            {
                PedidoEspelhoDAO.Instance.UpdateTotalPedido(sessao, pedidoEspelho, true);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao atualizar Valor do Pedido. Erro: " + ex.Message);
            }

            return returnValue;
        }

        #endregion

        #region Update

        internal int UpdateBase(GDASession sessao, ProdutosPedidoEspelho objUpdate, IContainerCalculo container)
        {
            CalculaDescontoEValorBrutoProduto(sessao, objUpdate, container);
            return base.Update(sessao, objUpdate);
        }

        private void CalculaDescontoEValorBrutoProduto(GDASession session, ProdutosPedidoEspelho produto, IContainerCalculo container)
        {
            DescontoAcrescimo.Instance.RemoverDescontoQtde(session, container, produto);
            DescontoAcrescimo.Instance.AplicarDescontoQtde(session, container, produto);
            DiferencaCliente.Instance.Calcular(session, container, produto);
            ValorBruto.Instance.Calcular(session, container, produto);
        }

        public int UpdateComTransacao(ProdutosPedidoEspelho objUpdate)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var pedidoEspelho = PedidoEspelhoDAO.Instance.GetElement(transaction, objUpdate.IdPedido);
                    var retorno = Update(transaction, objUpdate, pedidoEspelho);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override int Update(ProdutosPedidoEspelho objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Update(GDASession sessao, ProdutosPedidoEspelho objUpdate)
        {
            var pedidoEspelho = PedidoEspelhoDAO.Instance.GetElement(sessao, objUpdate.IdPedido);
            return Update(sessao, objUpdate, pedidoEspelho);
        }

        internal int Update(GDASession sessao, ProdutosPedidoEspelho objUpdate, IContainerCalculo container)
        {
            return UpdateCompleto(sessao, container, objUpdate, true, true);
        }

        private int UpdateCompleto(GDASession sessao, IContainerCalculo container, ProdutosPedidoEspelho objUpdate, bool removerClone, bool atualizaReserva)
        {
            try
            {
                if (!PedidoReferenciadoPermiteInsercao(sessao, objUpdate))
                    throw new Exception("Não é possível inserir itens diferentes dos inseridos no pedido de revenda associado, ou metragens maiores que as estabelecidas anteriormente.");

                if (!objUpdate.Redondo && objUpdate.IdAmbientePedido > 0 && AmbientePedidoEspelhoDAO.Instance.IsRedondo(sessao, objUpdate.IdAmbientePedido.Value))
                    objUpdate.Redondo = true;
                
                objUpdate.IdProdPedParentOrig = ProdutosPedidoDAO.Instance.ObterIdProdPedParentByEsp(sessao, objUpdate.IdProdPed);
                var isPedidoProducaoCorte = container.IsPedidoProducaoCorte;

                ValorTotal.Instance.Calcular(
                    sessao,
                    container,
                    objUpdate,
                    Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.NaoArredondar,
                    !isPedidoProducaoCorte,
                    objUpdate.Beneficiamentos.CountAreaMinimaSession(sessao)
                );
 
                /* Chamado 28687. */
                if (PedidoDAO.Instance.IsPedidoImportado(sessao, objUpdate.IdPedido) && objUpdate.Obs == null)
                    objUpdate.Obs = ObtemObs(sessao, objUpdate.IdProdPed);

                if (ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, (int)objUpdate.IdProd) == (int)Glass.Data.Model.NomeGrupoProd.Vidro &&
                    objUpdate.Espessura == 0)
                    objUpdate.Espessura = ProdutoDAO.Instance.ObtemEspessura(sessao, (int)objUpdate.IdProd);

                /* Chamado 54462. */
                objUpdate.ValorDescontoQtde = 0;
                UpdateBase(sessao, objUpdate, container);

                try
                {
                    AtualizaBenef(sessao, objUpdate.IdProdPed, objUpdate.Beneficiamentos, container);
                }
                catch { }

                // Atualiza a cópia do produto na tabela ProdutosPedido, com referência ao produto que está sendo inserido e invisível ao pedido
                if (removerClone)
                    RemoverClone(sessao, container, objUpdate.IdProdPed);

                CriarClone(sessao, container, objUpdate, atualizaReserva, false);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao atualizar Produto do Pedido. Erro: " + ex.Message);
            }

            try
            {
                var pedidoEspelho = container as PedidoEspelho
                    ?? PedidoEspelhoDAO.Instance.GetElement(sessao, objUpdate.IdPedido);

                PedidoEspelhoDAO.Instance.UpdateTotalPedido(sessao, pedidoEspelho, true);
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                base.Delete(sessao, objUpdate);
                throw new Exception("Falha ao atualizar Valor do Pedido. Erro: " + msg);
            }

            return 1;
        }

        #endregion

        #endregion
    }
}