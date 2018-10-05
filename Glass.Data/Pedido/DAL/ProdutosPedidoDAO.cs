using GDA;
using Glass.Configuracoes;
using Glass.Data.Helper;
using Glass.Data.Helper.Calculos;
using Glass.Data.Model;
using Glass.Data.RelDAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class ProdutosPedidoDAO : BaseDAO<ProdutosPedido, ProdutosPedidoDAO>
    {
        //private ProdutosPedidoDAO() { }

        #region Busca produtos para listagem padrão

        private string Sql(string idsPedidos, uint idPedido, uint idAmbientePedido, uint idProdPed, uint idProdPedEsp, bool cadastro,
            bool fluxo, bool liberacao, bool exportacao, bool forcarTabelasAdicionais, bool soProdutosComprados, bool ignorarFiltroProdComposicao, bool prodComposicao,
            uint idProdPedParent, bool selecionar)
        {
            return Sql(null, idsPedidos, idPedido, idAmbientePedido, idProdPed, idProdPedEsp, cadastro, fluxo, liberacao, exportacao,
                forcarTabelasAdicionais, soProdutosComprados, ignorarFiltroProdComposicao, prodComposicao, idProdPedParent, selecionar);
        }

        private string Sql(GDASession sessao, string idsPedidos, uint idPedido, uint idAmbientePedido, uint idProdPed, uint idProdPedEsp, bool cadastro,
            bool fluxo, bool liberacao, bool exportacao, bool forcarTabelasAdicionais, bool soProdutosComprados, bool ignorarFiltroProdComposicao, bool prodComposicao, uint idProdPedParent, bool selecionar)
        {
            return Sql(sessao, idsPedidos, idPedido, idAmbientePedido, idProdPed, idProdPedEsp, cadastro, fluxo, liberacao, exportacao,
                forcarTabelasAdicionais, soProdutosComprados, false, ignorarFiltroProdComposicao, prodComposicao, idProdPedParent, selecionar);
        }

        private string Sql(GDASession sessao, string idsPedidos, uint idPedido, uint idAmbientePedido, uint idProdPed, uint idProdPedEsp, bool cadastro,
            bool fluxo, bool liberacao, bool exportacao, bool forcarTabelasAdicionais, bool soProdutosComprados, bool naoBuscarGrupoSubgrupoVolume, bool ignorarFiltroProdComposicao, bool prodComposicao,
            uint idProdPedParent, bool selecionar)
        {
            uint idCliente = idPedido > 0 ? PedidoDAO.Instance.ObtemIdCliente(sessao, idPedido) :
                !String.IsNullOrEmpty(idsPedidos) ? PedidoDAO.Instance.ObtemIdCliente(sessao, Glass.Conversoes.StrParaUint(idsPedidos.Split(',')[0])) : 0;

            var idPedidoVerificarLiberacaoPedidosProntos = idPedido > 0 ? idPedido : !String.IsNullOrEmpty(idsPedidos) ? idsPedidos.Split(',').FirstOrDefault().StrParaUint() : 0;

            bool isClienteRota = RotaClienteDAO.Instance.IsClienteAssociado(sessao, idCliente);
            var idLoja = PedidoDAO.Instance.ObtemIdLoja(null, idPedidoVerificarLiberacaoPedidosProntos);
            var naoIgnorar = idLoja > 0 ? !LojaDAO.Instance.GetIgnorarLiberarProdutosProntos(null, idLoja) : true;
            bool liberarProdutosProntos = (Liberacao.DadosLiberacao.LiberarProdutosProntos && naoIgnorar) && !(Liberacao.DadosLiberacao.LiberarClienteRota && isClienteRota);

            bool usarTabelasAdicionais = (fluxo && forcarTabelasAdicionais) || (liberacao && fluxo && liberarProdutosProntos &&
                PCPConfig.ControlarProducao);

            string sqlProdutoTabela = "if(ped.TipoEntrega in (" + (int)Pedido.TipoEntregaPedido.Balcao + ", " +
                (int)Pedido.TipoEntregaPedido.Entrega + "), p.ValorBalcao, p.ValorObra)";

            string campoTipoSetorProducao = !usarTabelasAdicionais ? "" :
                (Liberacao.DadosLiberacao.LiberarClienteRota ? "if(ped.idCli in (select idCliente from rota_cliente), " + (int)SituacaoProdutoProducao.Pronto + ", " : "") +
                "if(ppp.Situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + ", ppp.situacaoProducao, ppp1.situacaoProducao)" + (Liberacao.DadosLiberacao.LiberarClienteRota ? ")" : "");

            string campos = selecionar ? @"pp.*, p.Descricao as DescrProduto, p.CodInterno, p.IdGrupoProd, g.descricao as descrGrupoProd,
                mip.idMaterProjMod, mip.IdPecaItemProj, p.idSubgrupoProd, if(p.AtivarAreaMinima=1,
                Cast(p.AreaMinima as char), '0') as AreaMinima, apl.CodInterno as CodAplicacao, prc.CodInterno as CodProcesso,
                Coalesce(no.CodInterno, cfop.CodInterno) as CodNaturezaOperacao,
                Round(0, 2) as AliqICMSProd, p.Cst, ap.Ambiente, ap.Descricao as DescrAmbiente,
                " + sqlProdutoTabela + @" as ValorProdutoTabela, p.custoCompra as custoCompraProduto,
                (Select Sum(Coalesce(pi.qtdeInstalada, 0)) From produtos_instalacao pi Where pi.idProdPed=pp.idProdPed) as qtdeInstalada" + (usarTabelasAdicionais ?
                ", ppe.idAmbientePedido as idAmbientePedidoEspelho, cast(" + campoTipoSetorProducao + @" as signed) as TipoSetorProducao,
                cast(if(ppp.numEtiqueta is not null, 1, null) as signed) as qtdeEtiquetas" : "" + ", c.Nome as NomeCliente") : "Count(*)";

            string sql = "Select " + campos + @" From produtos_pedido pp
                Left Join pedido ped On (pp.idPedido=ped.idPedido)
                Left Join cliente c on(c.ID_CLI=ped.IDCLI)
                Left Join produto p On (pp.idProd=p.idProd)
                Left Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd)
                Left Join subgrupo_prod sg On (p.idSubgrupoProd=sg.idSubgrupoProd)
                Left Join ambiente_pedido ap On (pp.idAmbientePedido=ap.idAmbientePedido)
                Left Join material_item_projeto mip On (pp.idMaterItemProj=mip.idMaterItemProj)
                Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao)
                Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso)
                Left Join natureza_operacao no On (pp.IdNaturezaOperacao=no.IdNaturezaOperacao)
                Left Join cfop cfop On (no.IdCfop=cfop.IdCfop) " +
                (usarTabelasAdicionais ?
                    @"Left Join produtos_pedido_espelho ppe on (pp.idProdPedEsp=ppe.idProdPed)
                    Left Join ambiente_pedido_espelho ape on (ppe.idAmbientePedido=ape.idAmbientePedido)
                    Left Join produto_pedido_producao ppp on (pp.idProdPedEsp=ppp.idProdPed)
                    Left Join produtos_pedido pp1 on (ppp.idProdPedProducao=pp1.idProdPedProdRepos)
                    Left Join produto_pedido_producao ppp1 on (ppp1.idProdPed=pp1.idProdPedEsp)
                    Left Join setor s on (ppp.idSetor=s.idSetor)
                    Left Join setor s1 on (ppp1.idSetor=s1.idSetor) " : "") +
                "Where 1";

            if (!String.IsNullOrEmpty(idsPedidos))
                sql += " and pp.idPedido in (" + idsPedidos + ")";
            else if (idPedido > 0)
                sql += " and pp.idPedido=" + idPedido;

            if (idAmbientePedido > 0)
                sql += " And (pp.idAmbientePedido=" + idAmbientePedido + (usarTabelasAdicionais ? " or ppe.idAmbientePedido=" + idAmbientePedido : "") + ")";
            // Se a empresa trabalha com ambiente de pedido e o mesmo não tiver sido informado, não busca nada
            else if (PedidoConfig.DadosPedido.AmbientePedido && cadastro)
                sql = "Select pp.* From produtos_pedido pp Where 0>1";
            else if (!PedidoConfig.DadosPedido.AmbientePedido && cadastro)
                sql += " And pp.idItemProjeto is null";

            if (idProdPed > 0)
                sql += " and pp.idProdPed=" + idProdPed;
            else if (idProdPedEsp > 0)
                sql += " and pp.idProdPedEsp=" + idProdPedEsp;

            if (!soProdutosComprados && idProdPed == 0 && idProdPedEsp == 0)
            {
                if (!fluxo || (!exportacao && !PCPConfig.UsarConferenciaFluxo))
                    sql += " and (pp.InvisivelPedido=false or pp.InvisivelPedido is null)";
                else if (exportacao || liberacao)
                    sql += " and (pp.InvisivelFluxo=false or pp.InvisivelFluxo is null)";
                else
                    sql += " and ((pp.InvisivelFluxo=false or pp.InvisivelFluxo is null) " +
                        (usarTabelasAdicionais ? "and ppe.idProdPed is not null" : "") + ")";
            }

            if (usarTabelasAdicionais)
            {
                string ignoraRota = !Liberacao.DadosLiberacao.LiberarClienteRota ? "" : @"ped.idCli in (select idCliente from rota_cliente) or ";

                sql += " and (" + ignoraRota + @"if(ppp1.situacao is not null, ppp1.situacao, ppp.situacao)=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                    Or (ppp.situacao is null and (g.idgrupoprod<>" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" Or sg.produtosEstoque=true)))
                    group by ppp.idProdPedProducao, if(pp1.idProdPedAnterior is not null, pp1.idProdPedAnterior, pp.idProdPed), tipoSetorProducao";
            }

            if (soProdutosComprados)
                sql += " and pp.idProdPedEsp in (select idProdPed from produtos_compra where idProdPed is not null and total>0)";

            if (naoBuscarGrupoSubgrupoVolume)
                sql += " AND (IF(p.IdSubgrupoProd > 0, sg.GeraVolume IS NULL OR sg.GeraVolume = 0, g.GeraVolume IS NULL OR g.GeraVolume = 0))";

            if (idProdPedParent > 0)
                sql += " AND pp.idProdPedParent = " + idProdPedParent;

            if (!ignorarFiltroProdComposicao)
                sql += " AND pp.IdProdPedParent IS " + (prodComposicao ? "NOT NULL" : "NULL");

            return sql;
        }

        public IList<ProdutosPedido> GetList(uint idPedido, uint idAmbientePedido, string sortExpression, int startRow, int pageSize)
        {
            return GetList(idPedido, idAmbientePedido, false, 0, sortExpression, startRow, pageSize);
        }

        public IList<ProdutosPedido> GetList(uint idPedido, uint idAmbientePedido, bool prodComposicao, uint idProdPedParent, string sortExpression, int startRow, int pageSize)
        {
            if (CountInPedidoAmbiente(idPedido, idAmbientePedido, prodComposicao, idProdPedParent) == 0)
            {
                var lst = new List<ProdutosPedido>();
                lst.Add(new ProdutosPedido());
                return lst.ToArray();
            }

            string sort = String.IsNullOrEmpty(sortExpression) ? "pp.idProdPed Asc" : sortExpression;

            return LoadDataWithSortExpression(Sql(null, idPedido, idAmbientePedido, 0, 0, true, false, false, false, false, false, false, prodComposicao, idProdPedParent, true), sort, startRow, pageSize, null);
        }

        public int GetCount(uint idPedido, uint idAmbientePedido)
        {
            return GetCount(idPedido, idAmbientePedido, false, 0);
        }

        public int GetCount(uint idPedido, uint idAmbientePedido, bool prodComposicao, uint idProdPedParent)
        {
            return GetCount(null, idPedido, idAmbientePedido, prodComposicao, idProdPedParent);
        }

        public int GetCount(GDASession sessao, uint idPedido, uint idAmbientePedido)
        {
            return GetCount(sessao, idPedido, idAmbientePedido, false, 0);
        }

        public int GetCount(GDASession sessao, uint idPedido, uint idAmbientePedido, bool prodComposicao, uint idProdPedParent)
        {
            int count = objPersistence.ExecuteSqlQueryCount(sessao, Sql(null, idPedido, idAmbientePedido, 0, 0, true, false, false, false, false, false, false, prodComposicao, idProdPedParent, false), null);

            return count == 0 ? 1 : count;
        }

        /// <summary>
        /// Retorna a quantidade de produtos relacionados ao pedido e ao ambiente passado
        /// </summary>
        public int CountInPedidoAmbiente(uint idPedido, uint idAmbientePedido)
        {
            return CountInPedidoAmbiente(idPedido, idAmbientePedido, false, 0);
        }

        /// <summary>
        /// Retorna a quantidade de produtos relacionados ao pedido e ao ambiente passado
        /// </summary>
        public int CountInPedidoAmbiente(uint idPedido, uint idAmbientePedido, bool prodComposicao, uint idProdPedParent)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(null, idPedido, idAmbientePedido, 0, 0, true, false, false, false, false, false, false, prodComposicao, idProdPedParent, false), null);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        public ProdutosPedido GetElement(uint idProdPed)
        {
            return GetElement(null, idProdPed);
        }

        public ProdutosPedido GetElement(GDASession sessao, uint idProdPed)
        {
            return GetElement(sessao, idProdPed, false, false, false);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        public ProdutosPedido GetElement(uint idProdPed, bool fluxo)
        {
            return GetElement(null, idProdPed, fluxo, false, false);
        }

        public ProdutosPedido GetElement(GDASession sessao, uint idProdPed, bool fluxo, bool ignorarFiltroProdComposicao, bool prodComposicao)
        {
            return objPersistence.LoadOneData(sessao, Sql(null, 0, 0, idProdPed, 0, false, fluxo, fluxo, false, false, false, ignorarFiltroProdComposicao, prodComposicao, 0, true));
        }

        public ProdutosPedido GetByProdPedEsp(uint idProdPedEsp)
        {
            return GetByProdPedEsp(null, idProdPedEsp, false);
        }

        public ProdutosPedido GetByProdPedEsp(GDASession sessao, uint idProdPedEsp, bool ignorarFiltroProdComposicao)
        {
            List<ProdutosPedido> item = objPersistence.LoadData(sessao, Sql(null, 0, 0, 0, idProdPedEsp, false, true, false, false, false, false, ignorarFiltroProdComposicao, false, 0, true));
            return item.Count > 0 ? item[0] : null;
        }

        public IList<ProdutosPedido> GetByAmbiente(uint idAmbientePedido)
        {
            return GetByAmbiente(null, idAmbientePedido);
        }

        public IList<ProdutosPedido> GetByAmbiente(GDASession sessao, uint idAmbientePedido)
        {
            return objPersistence.LoadData(sessao, Sql(sessao, null, 0, idAmbientePedido, 0, 0, false, false, false, false, false, false, false, false, 0, true)).ToList();
        }

        public IList<ProdutosPedido> GetByAmbienteInstalacao(uint idAmbientePedido)
        {
            return objPersistence.LoadData(Sql(null, 0, idAmbientePedido, 0, 0, false, false, false, false, false, false, false, false, 0, true)).ToList();
        }

        #endregion

        #region Busca produto do pedido do fluxo

        private string SqlElementFluxoLite(uint idProdPed)
        {
            string campos = @"pp.*, p.Descricao as DescrProduto, p.CodInterno, p.IdGrupoProd, p.idSubgrupoProd";

            string sql = "Select " + campos + @" From produtos_pedido pp
                Left Join produto p On (pp.idProd=p.idProd)
                Where 1";

            if (idProdPed > 0)
                sql += " and pp.idProdPed=" + idProdPed;

            if (!PCPConfig.UsarConferenciaFluxo)
                sql += " and (pp.InvisivelPedido=false or pp.InvisivelPedido is null)";
            else
                sql += " and (pp.InvisivelFluxo=false or pp.InvisivelFluxo is null)";

            return sql;
        }

        public ProdutosPedido GetElementFluxoLite(GDASession sessao, uint idProdPed)
        {
            return objPersistence.LoadOneData(sessao, SqlElementFluxoLite(idProdPed));
        }

        #endregion

        #region Retorna a quantidade de produtos relacionado ao pedido

        // Retorna a quantidade de produtos relacionados ao pedido passado
        public int CountInPedido(uint idPedido)
        {
            return CountInPedido(null, idPedido);
        }

        // Retorna a quantidade de produtos relacionados ao pedido passado
        public int CountInPedido(GDASession session, uint idPedido)
        {
            string sql = "Select Count(*) From produtos_pedido where idPedido=" + idPedido + " and (InvisivelPedido=false or InvisivelPedido is null)";

            return objPersistence.ExecuteSqlQueryCount(session, sql);
        }

        #endregion

        #region Busca todos os produtos relacionados ao pedido

        /// <summary>
        /// Retorna o idProd do primeiro item do pedido.
        /// </summary>
        /// <param name="idPedido">O identificador do pedido.</param>
        /// <returns>O id do produto do primeiro produto do pedido.</returns>
        public int? ObterIdProdPrimeiroProduto(GDASession sessao, int idPedido)
        {
            return ExecuteScalar<int?>($"SELECT idProd FROM produtos_pedido WHERE idPedido={idPedido} ORDER BY idProdPed ASC LIMIT 1");
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca todos os produtos relacionados ao pedido
        /// </summary>
        public IList<ProdutosPedido> GetByPedido(uint idPedido)
        {
            return GetByPedido(null, idPedido);
        }

        /// <summary>
        /// Busca todos os produtos relacionados ao pedido
        /// </summary>
        public IList<ProdutosPedido> GetByPedido(GDASession sessao, uint idPedido)
        {
            return GetByPedido(sessao, idPedido, false);
        }

        /// <summary>
        /// Busca todos os produtos relacionados ao pedido
        /// </summary>
        public IList<ProdutosPedido> GetByPedido(uint idPedido, bool fluxo)
        {
            return GetByPedido(null, idPedido, fluxo);
        }

        /// <summary>
        /// Busca todos os produtos relacionados ao pedido
        /// </summary>
        public IList<ProdutosPedido> GetByPedido(GDASession sessao, uint idPedido, bool fluxo)
        {
            return objPersistence.LoadData(sessao, Sql(null, idPedido, 0, 0, 0, false, fluxo, false, false, false, false, false, false, 0, true) +
                " Order By pp.idProdPed Asc").ToList();
        }

        /// <summary>
        /// Busca todos os produtos relacionados ao pedido
        /// </summary>
        public IList<ProdutosPedido> GetByPedido(GDASession sessao, uint idPedido, bool fluxo, bool ignorarFiltroProdComposicao)
        {
            return objPersistence.LoadData(sessao, Sql(null, idPedido, 0, 0, 0, false, fluxo, false, false, false, false, ignorarFiltroProdComposicao, false, 0, true) +
                " Order By pp.idProdPed Asc").ToList();
        }

        public IList<ProdutosPedido> GetByVidroPedido(uint idPedido, bool fluxo)
        {
            string sql = Sql(null, idPedido, 0, 0, 0, false, fluxo, false, false, false, false, false, false, 0, true);
            sql += " and p.IdGrupoProd = 1";
            sql += @" and pp.idProd in (
                        select idProd from chapa_vidro
                        union all select pbe.idProd from produto_baixa_estoque pbe
                            inner join chapa_vidro c on (pbe.idProdBaixa=c.idProd)
                    )";
            sql += "Order By pp.idProdPed Asc";

            var produtosPedido = objPersistence.LoadData(sql).ToList();

            foreach (var produtoPedido in produtosPedido)
                produtoPedido.DescrProduto =
                    produtoPedido.DescrProduto.Replace("(", "").Replace(")", "").Replace("+", "").Replace("-", "");

            return produtosPedido;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca todos os produtos relacionados ao pedido com o mínimo de joins possível
        /// </summary>
        public IList<ProdutosPedido> GetByPedidoLite(uint idPedido)
        {
            return GetByPedidoLite(idPedido, false).ToList();
        }

        /// <summary>
        /// Busca todos os produtos relacionados ao pedido com o mínimo de joins possível
        /// </summary>
        public IList<ProdutosPedido> GetByPedidoLite(GDASession sessao, uint idPedido)
        {
            return GetByPedidoLite(sessao, idPedido, false);
        }

        /// <summary>
        /// Busca todos os produtos relacionados ao pedido com o mínimo de joins possível
        /// </summary>
        public IList<ProdutosPedido> GetByPedidoLite(GDASession sessao, uint idPedido, bool ignorarProdutoComposicao)
        {
            return GetByPedidoLite(sessao, idPedido, false, ignorarProdutoComposicao).ToList();
        }

        /// <summary>
        /// APAGAR: apagar depois de colocar transação
        /// Busca todos os produtos relacionados ao pedido com o mínimo de joins possível
        /// </summary>
        public IList<ProdutosPedido> GetByPedidoLite(uint idPedido, bool usarEspelho)
        {
            return GetByPedidoLite(null, idPedido, usarEspelho, false);
        }

        /// <summary>
        /// Busca todos os produtos relacionados ao pedido com o mínimo de joins possível
        /// </summary>
        public IList<ProdutosPedido> GetByPedidoLite(GDASession sessao, uint idPedido, bool usarEspelho, bool ignorarProdutoComposicao)
        {
            string campos = @"pp.*, p.Descricao as DescrProduto, p.CodInterno, p.IdGrupoProd, pp.IdProdBaixaEst,
                g.descricao as descrGrupoProd, p.idSubgrupoProd, ap.Ambiente, ap.Descricao as DescrAmbiente,
                ppe.idAmbientePedido as idAmbientePedidoEspelho, COALESCE(ncm.ncm, p.ncm) as NCM";

            string sql = @"
                Select " + campos + @" From produtos_pedido pp
                    Left Join produto p On (pp.idProd=p.idProd)
                    Left Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd)
                    Left Join subgrupo_prod sg On (p.idSubgrupoProd=sg.idSubgrupoProd)
                    Left Join ambiente_pedido ap On (pp.idAmbientePedido=ap.idAmbientePedido)
                    Left Join produtos_pedido_espelho ppe On (pp.idProdPedEsp=ppe.idProdPed)
                    LEFT JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                    LEFT JOIN
                    (
                        SELECT *
                        FROM produto_ncm
                    ) as ncm ON (ped.IdLoja = ncm.IdLoja AND p.IdProd = ncm.IdProd)
                Where pp.idPedido=" + idPedido + @"
                    And (pp.Invisivel{0}=false or pp.Invisivel{0} is null)";

            if (ignorarProdutoComposicao)
                sql += " AND pp.IdProdPedParent IS NULL";

            return objPersistence.LoadData(sessao, String.Format(sql, usarEspelho ? "Fluxo" : "Pedido")).ToList();
        }
        
        /// <summary>
        /// Busca os ids dos produtos de vários pedidos.
        /// </summary>
        public IList<int> ObterIdsProdPedExcetoProducao(GDASession sessao, List<int> idsPedidos)
        {
            if (!idsPedidos?.Any(f => f > 0) ?? false)
            {
                return new List<int>();
            }

            var sql = $@"SELECT pp.IdProdPed 
                FROM produtos_pedido pp
                    LEFT JOIN pedido p ON (pp.IdPedido = p.IdPedido)
                WHERE pp.IdPedido IN ({ string.Join(",", idsPedidos) })
                    AND (InvisivelPedido IS NULL OR InvisivelPedido = 0)
                    AND p.TipoPedido NOT IN ({ (int)Pedido.TipoPedidoEnum.Producao })";

            return ExecuteMultipleScalar<int>(sessao, sql);
        }

        /// <summary>
        /// Busca os ids dos produtos de vários pedidos.
        /// </summary>
        public IList<uint> ObtemIdsProdPedByPedidos(GDASession sessao, string idsPedidos)
        {
            var sql = @"
                SELECT IdProdPed
                FROM produtos_pedido
                WHERE IdPedido in ({0}) AND COALESCE(InvisivelFluxo, 0) = 0";

            sql = string.Format(sql, idsPedidos);

            return ExecuteMultipleScalar<uint>(sessao, sql);
        }

        /// <summary>
        /// Valida se os vidros de um pedido são todos de uma mesma cor e espessura.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <param name="corVidro"></param>
        /// <param name="espessura"></param>
        /// <returns>Verdadeiro, se os vidros forem da mesma cor e espessura desejados; caso contrário, falso.</returns>
        public bool ValidarSeTodosOsVidrosSaoDaMesmaCorEEspessura(GDASession sessao, int idPedido, int? corVidro, double espessura)
        {
            if (idPedido == 0)
            {
                throw new ArgumentException("O pedido deve ser informado.", "idPedido");
            }

            if (!corVidro.HasValue || corVidro == 0)
            {
                throw new ArgumentException("A cor do vidro deve ser informada.", "corVidro");
            }

            if (espessura == 0)
            {
                throw new ArgumentException("A espessura do vidro deve ser informada.", "espessura");
            }

            var sql = $@"
                select count(*)
                from produtos_pedido pp
                    join produto p on (pp.idProd = p.idProd)
                where pp.idPedido={idPedido}
                    and (
                        pp.idProdPedParent is null
                        or pp.idProdPedParent = 0
                    )
                    and p.idGrupoProd={(int)NomeGrupoProd.Vidro}
                    and (
                        p.idCorVidro <> {corVidro.GetValueOrDefault()}
                        or p.espessura <> {espessura}
                    )";

            return this.ExecuteScalar<int>(sessao, sql) == 0;
        }

        #endregion

        #region Busca produtos para instalação, com a qtd já instalada

        private string SqlInst(uint idProdPed, uint idInstalacao, bool selecionar)
        {
            string campos = selecionar ? "pp.*, p.Descricao as DescrProduto, p.CodInterno, " +
                "(Select Sum(Coalesce(pi.qtdeInstalada, 0)) From produtos_instalacao pi Where pi.idProdPed=pp.idProdPed) as qtdeInstalada" : "Count(*)";

            string sql = "Select " + campos + " From produtos_pedido pp " +
                "Inner Join produto p On (pp.idProd=p.idProd) " +
                "Where 1";

            if (idProdPed > 0)
                sql += " and pp.idProdPed=" + idProdPed;

            if (idInstalacao > 0)
                sql += " and pp.idProdPed In (Select idProdPed From produtos_instalacao Where idInstalacao=" + idInstalacao + ")";

            return sql;
        }

        public IList<ProdutosPedido> GetListInst(uint idInstalacao)
        {
            return GetListInst(null, idInstalacao);
        }

        public IList<ProdutosPedido> GetListInst(GDASession session, uint idInstalacao)
        {
            return objPersistence.LoadData(session, SqlInst(0, idInstalacao, true)).ToList();
        }

        public IList<ProdutosPedido> GetListInst(uint idInstalacao, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlInst(0, idInstalacao, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCountInst(uint idInstalacao)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlInst(0, idInstalacao, false), null);
        }

        public ProdutosPedido GetElementInst(uint idProdPed)
        {
            var prod = objPersistence.LoadData(SqlInst(idProdPed, 0, true)).ToList();
            return prod.Count > 0 ? prod[0] : null;
        }

        #endregion

        #region Busca os Produtos Pedido pelo pedido para validar a quantidade do pedido e a quantidade já instalada

        private string SqlProdPedInstByPedido(uint idPedido, bool selecionar)
        {
            string campos = selecionar ? "pp.*, " +
                "(Select Sum(Coalesce(pi.qtdeInstalada, 0)) From produtos_instalacao pi Where pi.idProdPed=pp.idProdPed) as qtdeInstalada" : "Count(*)";

            string sql = "Select " + campos + " From produtos_pedido pp " +
                "Where 1";

            if (idPedido > 0)
                sql += " and pp.idPedido=" + idPedido;

            return sql;
        }

        public IList<ProdutosPedido> ObtemProdPedInstByPedido(uint idPedido)
        {
            return ObtemProdPedInstByPedido(null, idPedido);
        }

        public IList<ProdutosPedido> ObtemProdPedInstByPedido(GDASession sessao, uint idPedido)
        {
            return objPersistence.LoadData(sessao, SqlProdPedInstByPedido(idPedido, true)).ToList();
        }

        #endregion

        #region Busca produtos relacionados à vários pedidos

        /// <summary>
        /// Busca produtos relacionados à vários pedidos
        /// </summary>
        public ProdutosPedido[] GetByVariosPedidos(string idsPedido, string idsLiberarPedido,
            bool agruparProdutos, bool agruparSomentePorProduto)
        {
            return GetByVariosPedidos(idsPedido, idsLiberarPedido, agruparProdutos, agruparSomentePorProduto, false);
        }

        /// <summary>
        /// Busca produtos relacionados à vários pedidos
        /// </summary>
        public ProdutosPedido[] GetByVariosPedidos(string idsPedido, string idsLiberarPedido,
            bool agruparProdutos, bool agruparSomentePorProduto, bool agruparProjetosAoAgruparProdutos)
        {
            var liberacaoParcial = PedidoConfig.LiberarPedido && Liberacao.DadosLiberacao.LiberarPedidoProdutos;
            var sqlLiberacaoParcial = $@"
                IF(ped.Situacao={(int)Pedido.SituacaoPedido.LiberadoParcialmente} OR {(idsLiberarPedido.Length > 0).ToString()}, (
                    SELECT SUM(IFNULL(QtdeCalc, 0))
                    FROM produtos_liberar_pedido plp1
                        LEFT JOIN liberarpedido lp1 ON (plp1.IdLiberarPedido=lp1.IdLiberarPedido)
                    WHERE plp1.IdProdPed=pp.IdProdPed
                        {(idsLiberarPedido.Length > 0 ? "AND plp1.IdLiberarPedido=plp.IdLiberarPedido" : string.Empty)}
                        AND lp1.Situacao={(int)LiberarPedido.SituacaoLiberarPedido.Liberado}
                )-{(FiscalConfig.NotaFiscalConfig.DeduzirQtdTrocaProdutoNF ? "IFNULL(pt.QtdeTrocaDevolucao, 0)" : "0")}, {"{0}"})";

            // Não é necessário multiplicar os cálculos pela qtd dos ambientes, isso porque o total dos produtos_pedido já é o total final
            // este cálculo foi comentado para resolver o chamado 7710

            var qtdMaoDeObra = $@"(IF(ped.TipoPedido={(int)Pedido.TipoPedidoEnum.MaoDeObra},ap.Qtde,1) * pp.Qtde)";

            var sqlProdutoTrocado = string.Format("{0}", FiscalConfig.NotaFiscalConfig.DeduzirQtdTrocaProdutoNF ?
                    $@"SELECT pt.IdProdPed, SUM(pt.Qtde) AS QtdeTrocaDevolucao
                                FROM produto_trocado pt
                                    INNER JOIN troca_devolucao td ON (pt.IdTrocaDevolucao=td.IdTrocaDevolucao)
                                WHERE td.Situacao = { (int)TrocaDevolucao.SituacaoTrocaDev.Finalizada }
                                GROUP BY pt.IdProdPed" :
                    "SELECT NULL AS IdProdPed, 0 AS QtdeTrocaDevolucao");

            var fluxo = PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido";

            var sql = string.Empty;
            var where = string.Format("{0} pp.IdPedido IN ({1}) AND pp.IdProdPedParent IS NULL",
                idsLiberarPedido.Length > 0 ? string.Format("plp.IdLiberarPedido IN ({0}) AND ", idsLiberarPedido) : string.Empty,
                idsPedido);

            if (agruparProdutos)
            {
                var sqlAgruparProjetosAoAgruparProdutos = agruparProjetosAoAgruparProdutos ?

                @",
                  CAST(IFNULL(pm.IdProdParaNf, pp.IdProd) AS UNSIGNED INTEGER) AS IdProdUsar,
                  IF(pm.IdProdParaNf IS NOT NULL, CAST(GROUP_CONCAT(DISTINCT pp.IdItemProjeto) AS CHAR), NULL) AS IdsItemProjeto,
                  SUM(pp.ValorAcrescimo) AS ValorAcrescimoNf,
                  CAST(SUM(pp.ValorDescontoQtde) AS DECIMAL(12,2)) AS ValorDescontoQtdeNf" :
                @",
                  CAST(pp.IdProd AS UNSIGNED INTEGER) AS IdProdUsar";

                var sqlAgruparProjetosAoAgruparProdutos2 = agruparProjetosAoAgruparProdutos ?
                                @"LEFT JOIN item_projeto ip ON (pp.IdItemProjeto=ip.IdItemProjeto)
                                  LEFT JOIN projeto_modelo pm ON (ip.IdProjetoModelo=pm.IdProjetoModelo)" : string.Empty;

                var sqlAgruparProjetosAoAgruparProdutos3 = agruparProjetosAoAgruparProdutos ?
                                "COALESCE(pm.IdProdParaNf, pp.IdProd, 0)" : "pp.IdProd";

                var sqlAgruparProjetosAoAgruparProdutos4 = agruparProjetosAoAgruparProdutos ?
                                "IF(pm.IdProdParaNf IS NULL, pp.IdProd, pm.IdProjetoModelo)" : "pp.IdProd";

                var sqlIdsLiberarpedido = !string.IsNullOrEmpty(idsLiberarPedido) ? string.Format(" AND IdLiberarPedido IN ({0})", idsLiberarPedido) : string.Empty;

                var sqlAgruparSomentePorProduto = !agruparSomentePorProduto ?
                                string.Format(@", {0}
                                SELECT CAST(GROUP_CONCAT(IdBenefConfig) AS CHAR)
                                FROM (SELECT IdProdPed, IdBenefConfig FROM produto_pedido_benef ORDER BY IdBenefConfig) AS ppb
                                WHERE ppb.IdProdPed = pp.IdProdPed) {1}, IFNULL(ped.FastDelivery, 0)",
                                    agruparProjetosAoAgruparProdutos ? "IF(pm.IdProdParaNf IS NULL, (" : "(",
                                    agruparProjetosAoAgruparProdutos ? ", NULL)" : string.Empty) :
                                string.Empty;

                sql = $@"
                    SELECT p.Descricao AS DescrProduto, p.CodInterno, pp.*, SUM((IFNULL({(!FiscalConfig.NotaFiscalConfig.ConsiderarM2CalcNotaFiscal ? "pp.TotM" :
                        "pp.TotM2Calc")},0.00) / (pp.Qtde - IFNULL(pt.QtdeTrocaDevolucao, 0))) * {string.Format(sqlLiberacaoParcial, "(pp.Qtde - IFNULL(pt.QtdeTrocaDevolucao, 0))")}) AS TotM2Nf,
                        CAST(SUM((pp.Total / {qtdMaoDeObra}) * {string.Format(sqlLiberacaoParcial, qtdMaoDeObra)}) AS DECIMAL(12,2)) AS TotalNf,
                        SUM({string.Format(sqlLiberacaoParcial, "(pp.Qtde - IFNULL(pt.QtdeTrocaDevolucao, 0))")}) AS QtdNf,
                        CAST(SUM((pp.ValorBenef / {qtdMaoDeObra}) * {string.Format(sqlLiberacaoParcial, qtdMaoDeObra)}) AS DECIMAL(12,2)) AS ValorBenefNf,
                        p.IdGrupoProd, p.IdSubgrupoProd, SUM(pp.Qtde - IFNULL(pt.QtdeTrocaDevolucao, 0)) AS QtdeOriginal {sqlAgruparProjetosAoAgruparProdutos}
                    FROM produtos_pedido pp
                        LEFT JOIN pedido ped ON (pp.IdPedido=ped.IdPedido)
                        {sqlAgruparProjetosAoAgruparProdutos2}
                        LEFT JOIN produto p ON ({sqlAgruparProjetosAoAgruparProdutos3}=p.idProd)
                        LEFT JOIN (
                            SELECT * FROM produtos_liberar_pedido
                            WHERE QtdeCalc>0 {sqlIdsLiberarpedido}
                            GROUP BY IdProdPed {(!string.IsNullOrEmpty(idsLiberarPedido) ? ", IdLiberarPedido" : string.Empty)}
                        ) plp ON (plp.IdProdPed=pp.IdProdPed)
                        LEFT JOIN liberarpedido lp ON (plp.IdLiberarPedido=lp.IdLiberarPedido)
                        LEFT JOIN ambiente_pedido ap ON (pp.IdAmbientePedido=ap.IdAmbientePedido)
                        LEFT JOIN
                            ({sqlProdutoTrocado}) pt ON (pp.IdProdPed=pt.IdProdPed)
                    WHERE {where}
                        AND (pp.Invisivel{fluxo} IS NULL OR pp.Invisivel{fluxo} = 0)
                        AND IF({PedidoConfig.LiberarPedido.ToString()}, lp.Situacao<>{(int)LiberarPedido.SituacaoLiberarPedido.Cancelado} OR lp.Situacao IS NULL, 1)
                    GROUP BY {sqlAgruparProjetosAoAgruparProdutos4} {sqlAgruparSomentePorProduto}";
            }
            else
            {
                sql = $@"
                    SELECT pp.*, p.Descricao AS DescrProduto, p.CodInterno, (IFNULL({(!FiscalConfig.NotaFiscalConfig.ConsiderarM2CalcNotaFiscal ? "pp.TotM" :
                        "pp.TotM2Calc")},0.00) / (pp.Qtde - IFNULL(pt.QtdeTrocaDevolucao, 0))) * {string.Format(sqlLiberacaoParcial, qtdMaoDeObra)} * (IF(ped.TipoPedido={(int)Pedido.TipoPedidoEnum.MaoDeObra},ap.Qtde,1)) AS TotM2Nf,
                        CAST(((pp.Total / {qtdMaoDeObra}) * {string.Format(sqlLiberacaoParcial, qtdMaoDeObra)}) AS DECIMAL (12,2)) AS TotalNf,
                        {string.Format(sqlLiberacaoParcial, "(pp.Qtde - IFNULL(pt.QtdeTrocaDevolucao, 0))")} AS QtdNf,
                        CAST((pp.ValorBenef / {qtdMaoDeObra}) * {string.Format(sqlLiberacaoParcial, qtdMaoDeObra)} AS DECIMAL (12,2)) AS ValorBenefNf,
                        p.IdGrupoProd, p.IdSubgrupoProd, pt.QtdeTrocaDevolucao,
                        (pp.Qtde - IFNULL(pt.QtdeTrocaDevolucao, 0)) AS QtdeOriginal, CAST(pp.IdProd AS UNSIGNED INTEGER) AS IdProdUsar,
                        pp.ValorAcrescimo AS ValorAcrescimoNf, Cast(pp.ValorDescontoQtde AS DECIMAL(12,2)) AS ValorDescontoQtdeNf, pp.ValorIpi AS ValorIpiNf, plp.IdLiberarPedido
                    FROM produtos_pedido pp
                        LEFT JOIN produto p ON (pp.IdProd=p.IdProd)
                        LEFT JOIN pedido ped on (pp.IdPedido=ped.IdPedido)
                        LEFT JOIN produtos_liberar_pedido plp on (plp.IdProdPed=pp.IdProdPed)
                        LEFT JOIN liberarpedido lp on (plp.IdLiberarPedido=lp.IdLiberarPedido)
                        LEFT JOIN ambiente_pedido ap on (pp.IdAmbientePedido=ap.IdAmbientePedido)
                        LEFT JOIN
                            ({sqlProdutoTrocado}) pt ON (pp.IdProdPed=pt.IdProdPed)
                    WHERE {where}
                        AND (pp.Invisivel{ fluxo } IS NULL OR pp.Invisivel{ fluxo } = 0)
                        AND IF({liberacaoParcial.ToString()}, lp.Situacao<>{(int)LiberarPedido.SituacaoLiberarPedido.Cancelado} OR lp.Situacao IS NULL, 1)";

                sql += $@" GROUP BY pp.IdProdPed{(idsLiberarPedido.Length > 0 ? ", plp.IdLiberarPedido" : string.Empty)}
                                                HAVING IF(idLiberarPedido>0,QtdeOriginal,pp.Qtde)> 0";
            }

            var lstProd = objPersistence.LoadData(sql).ToList();
            var lstRetorno = new List<ProdutosPedido>();

            decimal total = 0;

            foreach (var pp in lstProd)
            {
                var qtdOriginal = pp.Qtde;
                pp.Qtde = (float)pp.QtdNf;

                // Soma a quantidade de fechamentos de projetos
                if (agruparProjetosAoAgruparProdutos && !string.IsNullOrEmpty(pp.IdsItemProjeto))
                {
                    pp.QtdNf = 0;
                    pp.TotM2Nf = 0;
                    pp.Qtde = 0;
                    pp.TotM = 0;

                    foreach (var id in pp.IdsItemProjeto.Split(','))
                        if (!string.IsNullOrEmpty(id))
                        {
                            pp.Qtde += ItemProjetoDAO.Instance.ObtemValorCampo<int>("qtde", "idItemProjeto=" + id);
                            pp.TotM += ItemProjetoDAO.Instance.ObtemValorCampo<float>("m2Vao", "idItemProjeto=" + id);
                        }

                    pp.TotM2Nf = pp.TotM;
                }

                if (pp.Qtde <= 0)
                    continue;

                if (pp.TotM2Nf > 0)
                    pp.TotM = (float)pp.TotM2Nf;

                pp.TotM2Calc = FiscalConfig.NotaFiscalConfig.ConsiderarM2CalcNotaFiscal ? (float)pp.TotM2Nf : pp.TotM;
                pp.Total = (decimal)pp.TotalNf;
                pp.ValorBenef = (decimal)pp.ValorBenefNf;

                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)pp.IdGrupoProd, (int?)pp.IdSubgrupoProd, true);

                if (tipoCalc == (uint)Glass.Data.Model.TipoCalculoGrupoProd.Qtd || tipoCalc == (uint)Glass.Data.Model.TipoCalculoGrupoProd.QtdM2)
                    pp.ValorVendido = pp.Total / (decimal)pp.Qtde;
                else if (pp.TotM > 0)
                    pp.ValorVendido = pp.Total / (decimal)pp.TotM;
                else if (pp.Altura > 0)
                    pp.ValorVendido = (pp.Total * 6) / (decimal)(pp.Altura * pp.Qtde);
                else
                    pp.ValorVendido = pp.Total / (decimal)pp.Qtde;

                if (pp.Qtde > 0)
                    lstRetorno.Add(pp);

                total += pp.Total;
            }

            return lstRetorno.ToArray();
        }

        #endregion

        #region Busca produtos relacionados à vários IDs

        /// <summary>
        /// Busca produtos relacionados à vários IDs
        /// </summary>
        public IList<ProdutosPedido> GetByVariosIDs(string idsProdutosPedido)
        {
            var sql = @"
                SELECT pp.* FROM produtos_pedido pp
                WHERE pp.IdProdPed IN (" + idsProdutosPedido + @")
                    AND (pp.InvisivelPedido=FALSE OR pp.InvisivelPedido IS NULL)";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca produtos para geração de volumes

        private string SqlForGerarVolume(uint idPedido, bool selecionar)
        {
            string campos = selecionar ? @"pp.*, CAST(SUM(vpp.Qtde) as SIGNED) as QtdeVolume, p.Descricao as DescrProduto, p.CodInterno,
                p.idGrupoProd, p.idSubGrupoProd" : "COUNT(DISTINCT pp.idProdPed)";

            string sql = @"
                SELECT " + campos + @"
                FROM produtos_pedido pp
                    INNER JOIN produto p ON (pp.idProd = p.idProd)
                    LEFT JOIN grupo_prod gp ON (p.idGrupoProd = gp.idGrupoProd)
                    LEFT JOIN subgrupo_prod sgp ON (p.idSubGrupoProd = sgp.idSubGrupoProd)
                    LEFT JOIN volume_produtos_pedido vpp ON (pp.idProdPed = vpp.idProdPed)
                WHERE COALESCE(sgp.geraVolume, gp.geraVolume, FALSE) = TRUE
                    AND (pp.Qtde - COALESCE((select SUM(Qtde) from volume_produtos_pedido where idProdPed=pp.idProdPed), 0)) > 0";

            if (idPedido > 0)
                sql += " AND pp.idPedido=" + idPedido;

            if (!PCPConfig.UsarConferenciaFluxo)
                sql += " AND COALESCE(pp.InvisivelPedido, false) = false";
            else
                sql += " AND COALESCE(pp.InvisivelFluxo, false) = false";

            if (selecionar)
                sql += " GROUP BY pp.idProdPed";

            return sql;
        }

        /// <summary>
        /// Recupera os itens para a geração de volume
        /// </summary>
        public IList<ProdutosPedido> GetForGerarVolume(uint idPedido, string sortExpression, int startRow, int pageSize)
        {
            if (GetForGerarVolumeCountReal(idPedido) == 0)
            {
                List<ProdutosPedido> lst = new List<ProdutosPedido>();
                lst.Add(new ProdutosPedido());
                return lst.ToArray();
            }

            var sql = SqlForGerarVolume(idPedido, true);
            sql += " ORDER BY gp.Descricao ";

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize);
        }

        public int GetForGerarVolumeCountReal(uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlForGerarVolume(idPedido, false));
        }

        public int GetForGerarVolumeCount(uint idPedido)
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlForGerarVolume(idPedido, false));

            return count == 0 ? 1 : count;
        }

        #endregion

        #region Busca produtos para os pedidos da OC, Expediçao do Carregamento

        private string SqlForOC(uint idPedido, string idsPedidos, bool produtosEstoque, uint idProd, bool ignorarGerados)
        {
            string campos = @"pp.IdPedido, pp.IdProdPed, pp.IdProd, pp.Altura, pp.Largura, {0}, p.Descricao as DescrProduto, p.CodInterno,
                apl.CodInterno as CodAplicacao, prc.CodInterno as CodProcesso, pp.qtdSaida,
                ppp.numEtiqueta as NumEtiquetaConsulta";

            campos = string.Format(campos, produtosEstoque && ignorarGerados ? "CAST((pp.Qtde - COALESCE(ic.Qtde, 0)) as decimal(12,2)) as Qtde" : "pp.qtde");

            string sql = @"
                SELECT " + campos + @"
                FROM produtos_pedido pp
                    INNER JOIN produto p ON (pp.idProd = p.idProd)
                    LEFT JOIN grupo_prod gp ON (p.idGrupoProd = gp.idGrupoProd)
                    LEFT JOIN subgrupo_prod sgp ON (p.idSubGrupoProd = sgp.idSubGrupoProd)
                    LEFT JOIN etiqueta_processo prc ON (pp.idProcesso=prc.idProcesso)
                    LEFT JOIN etiqueta_aplicacao apl ON (pp.idAplicacao=apl.idAplicacao)
                    LEFT JOIN produto_pedido_producao ppp ON (pp.idProdPedEsp=ppp.idProdPed)
                    {0}
                WHERE COALESCE(sgp.geraVolume, gp.geraVolume, 0) = 0
                    AND COALESCE(pp.invisivelFluxo, 0) = 0
                    AND COALESCE(pp.IdProdPedParent, 0) = 0";

            var joinItemCarregamento = "";

            if (produtosEstoque)
            {
                if (ignorarGerados)
                {
                    joinItemCarregamento = @"
                        LEFT JOIN
                        (
						    SELECT IdProdPed, count(*) as Qtde
                            FROM item_carregamento
                            GROUP BY IdProdPed
                        ) as ic ON (pp.IdProdPed = ic.IdProdPed)";
                }
            }
            else
            {
                sql += " AND ppp.situacao = " + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;

                if (ignorarGerados)
                {
                    sql += " AND ic.IdItemCarregamento is null";
                    joinItemCarregamento = @"LEFT JOIN item_carregamento ic ON (ppp.IdProdPedProducao = ic.IdProdPedProducao)";
                }
            }

            if (idPedido > 0)
                sql += " AND pp.idPedido=" + idPedido;

            if (!string.IsNullOrEmpty(idsPedidos))
                sql += " AND pp.idPedido IN(" + idsPedidos + ")";

            if (idProd > 0)
                sql += " AND pp.IdProd = " + idProd;

            sql += " AND COALESCE(sgp.produtosEstoque, FALSE) =" + produtosEstoque;

            sql = string.Format(sql, joinItemCarregamento);

            return sql;
        }

        private string SqlForOCRpt(uint idOrdemCarga, string idsPedidos)
        {
            var campoQtde = "IF(ic1.IdProdPed IS NULL, pp.qtde - COALESCE(ic.Qtde, 0), ic1.Qtde)";

            var campos = $@"pp.IdPedido, p.CodCliente AS CodPedCliente, CONCAT(prod.CodInterno, ' - ', prod.Descricao) AS DescrProduto, CAST(({campoQtde}) AS DECIMAL(12, 2)) AS Qtde,
                pp.Altura, pp.Largura, CAST(ROUND(((pp.TotM2Calc / pp.Qtde) * {campoQtde}), 2) AS DECIMAL(12, 2)) AS TotM2Calc,
                CAST(ROUND(IF(sgp.TipoSubgrupo IN ({(int)TipoSubgrupoProd.VidroDuplo},{(int)TipoSubgrupoProd.VidroLaminado}),
                    (SELECT SUM(Peso) FROM produtos_pedido WHERE IdProdPedParent = pp.IdProdPed) * pp.Qtde, (pp.Peso / pp.Qtde)) * ({campoQtde}), 2) AS DECIMAL(12, 2)) AS Peso";

            var camposVolume = @"
                v.IdPedido, p.CodCliente as CodPedCliente, CONCAT('Volume: ', v.idVolume, '  Data de Fechamento: ', v.dataFechamento) as DescrProduto,
                null as Qtde, null as Altura, null as Largura, null as TotM2Calc, CAST(ROUND(SUM(pp.peso / if(pp.qtde <> 0, pp.qtde, 1) * vpp.qtde), 2) as decimal(12, 2)) as Peso";

            var sql = $@"
                SELECT { camposVolume }
                FROM volume v
                    LEFT JOIN pedido p ON (v.idPedido = p.idPedido)
                    LEFT JOIN volume_produtos_pedido vpp ON (v.idVolume = vpp.idVolume)
                    LEFT JOIN produtos_pedido pp ON (vpp.idProdPed = pp.idProdPed)
                    LEFT JOIN
                    (
			                SELECT IdProdPed, count(*) as QtdePronto
			                FROM produto_pedido_producao
			                WHERE
                                IdProdPed in (select IdProdPed FROM produtos_pedido_espelho WHERE IdPedido in ({idsPedidos}))
                                AND SituacaoProducao IN ({ (int)SituacaoProdutoProducao.Pronto },{ (int)SituacaoProdutoProducao.Entregue })
			                GROUP BY IdProdPed
                    ) as ppp ON (pp.IdProdPedEsp = ppp.idProdPed)
                WHERE v.idPedido IN ({ idsPedidos }) AND v.IdOrdemCarga = { idOrdemCarga }
                GROUP BY v.IdVolume";

            sql += $@"
                UNION ALL
                SELECT { campos }
                FROM produtos_pedido pp
                    INNER JOIN pedido p ON (pp.idPedido = p.idPedido)
                    INNER JOIN produto prod ON (pp.idProd = prod.idProd)
                    LEFT JOIN produto_pedido_producao ppp ON (pp.idProdPedEsp=ppp.idProdPed)
                    LEFT JOIN grupo_prod gp ON (prod.idGrupoProd = gp.idGrupoProd)
                    LEFT JOIN subgrupo_prod sgp ON (prod.idSubGrupoProd = sgp.idSubGrupoProd)
                    LEFT JOIN
                    (
		                SELECT IdProdPed, count(*) as Qtde
		                FROM item_carregamento
		                WHERE COALESCE(IdProdPed, 0) > 0 AND idPedido IN ({ idsPedidos }) AND IdOrdemCarga <> { idOrdemCarga }
		                GROUP BY IdProdPed
                    ) as ic ON (ic.IdProdPed = pp.IdProdPed)
                    LEFT JOIN
                    (
		                SELECT IdProdPed, count(*) as Qtde
		                FROM item_carregamento
		                WHERE COALESCE(IdProdPed, 0) > 0 AND idPedido IN ({ idsPedidos }) AND IdOrdemCarga = { idOrdemCarga }
		                GROUP BY IdProdPed
                    ) as ic1 ON (ic1.IdProdPed = pp.IdProdPed)
                WHERE COALESCE(sgp.geraVolume, gp.geraVolume, FALSE) = FALSE
                    AND COALESCE(pp.invisivelFluxo, FALSE) = FALSE
                    AND COALESCE(ppp.situacao, { (int)ProdutoPedidoProducao.SituacaoEnum.Producao }) = { (int)ProdutoPedidoProducao.SituacaoEnum.Producao }
                    AND pp.idPedido IN({ idsPedidos })
                    AND pp.IdProdPedParent IS NULL";

            sql += " GROUP BY pp.idProdPed";

            return $"SELECT * FROM ({ sql }) as tmp WHERE IdPedido IS NOT NULL";
        }

        /// <summary>
        /// Recupera os produtos pedido para a OC
        /// </summary>
        public IList<ProdutosPedido> GetByPedidoForOC(uint idPedido, bool produtosEstoque)
        {
            var itens = objPersistence.LoadData(SqlForOC(idPedido, null, produtosEstoque, 0, true)).ToList();

            itens.ForEach(x => x.Peso /= x.Qtde);
            return itens;
        }

        /// <summary>
        /// Recupera os produtos pedido para a expedição do Carregamento
        /// </summary>
        public List<ProdutosPedido> GetByPedidosForExpCarregamento(GDASession sessao, string idsPedido, bool produtosEstoque, bool ignorarGerados)
        {
            var itens = objPersistence.LoadData(sessao, SqlForOC(0, idsPedido, produtosEstoque, 0, ignorarGerados)).ToList();
            itens.ForEach(x => x.Peso /= x.Qtde);
            return itens;
        }

        /// <summary>
        /// Recupera os produtos pedido para a expedição do Carregamento
        /// </summary>
        public List<ProdutosPedido> GetByPedidoProdutoForExpCarregamento(GDASession session, uint idPedido, uint idProd, bool produtosEstoque)
        {
            var itens = objPersistence.LoadData(session, SqlForOC(idPedido, null, produtosEstoque, idProd, false)).ToList();
            itens.ForEach(x => x.Peso /= x.Qtde);
            return itens;
        }

        /// <summary>
        /// Recupera os produtos pedido para o relatorio da OC
        /// </summary>
        public List<ProdutosPedido> GetByPedidosForOcRpt(uint idOrdemCarga, string idsPedido)
        {
            return objPersistence.LoadData(SqlForOCRpt(idOrdemCarga, idsPedido));
        }

        #endregion

        #region Busca produtos para impressão de etiqueta de box

        private string SqlProdEtiq(uint idPedido, uint idCorVidro, float espessura,
            string codProcesso, string codAplicacao, uint idSubgrupoProd, bool selecionar)
        {
            string campos = selecionar ? @"pp.idProdPed, pp.idPedido, pp.idProd, pp.idItemProjeto, pp.idMaterItemProj, pp.idAmbientePedido,
                pp.idAplicacao, pp.idProcesso, pp.qtde, pp.valorVendido, pp.altura, pp.alturaReal, pp.largura, pp.totM, pp.totM2Calc,
                pp.total, pp.aliqIcms, pp.valorIcms, pp.redondo, pp.valorBenef, pp.pedCli, pp.alturaBenef, pp.larguraBenef, pp.espBenef,
                pp.valorAcrescimo, pp.valorDesconto, pp.valorAcrescimoProd, pp.valorDescontoProd, pp.percDescontoQtde,
                pp.valorDescontoQtde, p.descricao As DescrProduto, p.codInterno, p.IdGrupoProd, p.idSubgrupoProd, apl.codInterno As CodAplicacao,
                prc.codInterno As CodProcesso, pp.valorDescontoCliente, pp.valorAcrescimoCliente, pp.invisivelFluxo, pp.invisivelAdmin, pp.aliquotaIpi,
                pp.valorIpi, pp.valorUnitBruto, pp.totalBruto, ped.dataEntrega, ped.dataPedido, cli.nome as nomeCliente, pp.QtdeInvisivel,
                cv.idCorVidro, p.espessura, pp.valorComissao, pp.QtdeBoxImpresso" : "Count(*) As num";

            string where = "";

            string produtoRevenda = "(Coalesce(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + ")=" + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd +
                    " Or Coalesce(s.produtosEstoque, False) = True)";

            if (idPedido > 0)
                where += " And pp.idPedido=" + idPedido;

            if (idCorVidro > 0)
                where += " And cv.idCorVidro=" + idCorVidro;

            if (espessura > 0)
                where += " And p.espessura=" + espessura.ToString().Replace(",", ".");

            if (!String.IsNullOrEmpty(codProcesso))
                where += " And prc.codInterno=?codProc";

            if (!String.IsNullOrEmpty(codAplicacao))
                where += " And apl.codInterno=?codApl";

            if (idSubgrupoProd > 0)
                where += " And p.idSubgrupoProd=" + idSubgrupoProd;

            string filtroApenasVidros = " And p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro +
                " And (ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Revenda + " Or " + produtoRevenda + ")";

            string sql = "Select " + campos + @"
                From produtos_pedido pp
                    Inner Join pedido ped ON (pp.idPedido=ped.idPedido)
                    Inner Join cliente cli ON (ped.idCli=cli.id_cli)
                    Left Join produto p ON (pp.idProd=p.idProd)
                    Left Join grupo_prod g ON (p.idGrupoProd=g.idGrupoProd)
                    Left Join subgrupo_prod s ON (p.idSubgrupoProd=s.idSubgrupoProd)
                    Left Join cor_vidro cv ON (p.idCorVidro=cv.idCorVidro)
                    Left Join etiqueta_aplicacao apl ON (pp.idAplicacao=apl.idAplicacao)
                    Left Join etiqueta_processo prc ON (pp.idProcesso=prc.idProcesso)
                Where pp.Qtde > COALESCE(pp.QtdeBoxImpresso, 0)
                    AND ped.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + filtroApenasVidros + where +
                " Group By pp.idProdPed";

            if (!selecionar)
                sql = "Select Coalesce(Sum(num), 0) From (" + sql + ") As temp";

            return sql;
        }

        /// <summary>
        /// Busca peças de box do pedido de revenda.
        /// </summary>
        public IList<ProdutosPedido> GetProdEtiqBox(uint idPedido, uint idProcesso, uint idAplicacao, uint idCorVidro,
            float espessura, uint idSubgrupoProd)
        {
            string codProc = EtiquetaProcessoDAO.Instance.ObtemCodInterno(idProcesso);
            string codApl = EtiquetaAplicacaoDAO.Instance.ObtemCodInterno(idAplicacao);

            return objPersistence.LoadData(SqlProdEtiq(idPedido, idCorVidro, espessura, codProc, codApl, idSubgrupoProd, true),
                GetParam(codProc, codApl)).ToList();
        }

        private GDAParameter[] GetParam(string codProcesso, string codAplicacao)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codProcesso))
                lstParam.Add(new GDAParameter("?codProc", codProcesso));

            if (!String.IsNullOrEmpty(codAplicacao))
                lstParam.Add(new GDAParameter("?codApl", codAplicacao));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Marca a quantidade de determinado box que foi impresso

        /// <summary>
        /// Marca a quantidade de determinado box que foi impresso
        /// </summary>
        public void MarcarBoxImpressao(GDASession session, int idProdPed, int qtdImpresso)
        {
            string sql = @"
                UPDATE produtos_pedido
                SET qtdeBoxImpresso = coalesce(qtdeBoxImpresso, 0) + " + qtdImpresso + @"
                WHERE IdProdPed = " + idProdPed;

            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region GetForRpt

        /// <summary>
        /// Busca produtos para mostrar no relatório
        /// </summary>
        public ProdutosPedido[] GetForRpt(uint idPedido, bool isRelatorioPcp)
        {
            return GetForRpt(idPedido, isRelatorioPcp, true);
        }

        /// <summary>
        /// Busca produtos para mostrar no relatório
        /// </summary>
        public ProdutosPedido[] GetForRpt(uint idPedido, bool isRelatorioPcp, bool incluirBeneficiamentos)
        {
            string sql = String.Empty;

            // se a empresa trabalha com inserção de ambiente no pedido
            if (PedidoConfig.DadosPedido.AmbientePedido)
            {
                sql = @"
                    Select pp.*, p.Descricao as DescrProduto, p.CodInterno, p.idGrupoProd, p.idSubgrupoProd, p.LocalArmazenagem as LocalArmazenagem, um.codigo as unidade,
                        apl.CodInterno as CodAplicacao, prc.CodInterno as CodProcesso, ap.Ambiente, ap.Descricao as DescrAmbiente, ap.tipoDesconto As tipoDescontoAmbiente,
                        ap.desconto As descontoAmbiente, ip.obs as obsProjeto
                    From produtos_pedido pp
                        Left Join pedido ped on (pp.idPedido=ped.idPedido)
                        Left Join produto p On (pp.idProd=p.idProd)
                        Left Join unidade_medida um On (p.idUnidadeMedida=um.idUnidadeMedida)
                        Left Join ambiente_pedido ap On (pp.idAmbientePedido=ap.idAmbientePedido)
                        Left Join item_projeto ip On (ap.idItemProjeto=ip.idItemProjeto)
                        Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao)
                        Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso)
                    Where pp.idPedido=" + idPedido;
            }
            else
            {
                sql = @"
                    Select pp.*, p.Descricao as DescrProduto, p.CodInterno, p.idGrupoProd, p.LocalArmazenagem as LocalArmazenagem, um.codigo as unidade,
                        p.idSubgrupoProd, apl.CodInterno as CodAplicacao, prc.CodInterno as CodProcesso, COALESCE(ip.Ambiente, ap.Ambiente) AS Ambiente, ip.obs as obsProjeto
                    From produtos_pedido pp
                        Left Join produto p On (pp.idProd=p.idProd)
                        Left Join unidade_medida um On (p.idUnidadeMedida=um.idUnidadeMedida)
                        Left Join ambiente_pedido ap On (pp.idAmbientePedido=ap.idAmbientePedido)
                        Left Join item_projeto ip on (pp.idItemProjeto=ip.idItemProjeto)
                        Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao)
                        Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso)
                    Where pp.idPedido=" + idPedido;
            }

            sql += String.Format(" and (pp.Invisivel{0}=false or pp.Invisivel{0} is null)", !isRelatorioPcp ? "Pedido" : "Fluxo");
            if (isRelatorioPcp)
                sql += " and pp.idProdPedEsp in (select idProdPed from produtos_pedido_espelho where coalesce(invisivelFluxo,false)=false)";

            List<ProdutosPedido> lstProdPed = objPersistence.LoadData(sql + " Order By pp.idProdPed Asc");
            List<ProdutosPedido> lstProdPedRetorno = new List<ProdutosPedido>();

            bool maoDeObra = PedidoDAO.Instance.IsMaoDeObra(null, idPedido);

            // Adiciona os beneficiamentos feitos nos produtos como itens do pedido
            foreach (ProdutosPedido pp in lstProdPed)
            {
                /* Chamado 53945. */
                if (pp.IdProdPedParent > 0)
                {
                    pp.ValorVendido = 0;
                    pp.Total = 0;
                }

                if (/*!PedidoConfig.DadosPedido.AmbientePedido &&*/ maoDeObra && pp.IdAmbientePedido != null)
                {
                    AmbientePedido amb = AmbientePedidoDAO.Instance.GetElement(pp.IdAmbientePedido.Value, idPedido, isRelatorioPcp);
                    if (amb != null)
                    {
                        pp.Ambiente = amb.PecaVidroQtd;
                        pp.Redondo = amb.Redondo;

                        pp.TotM *= (float)amb.Qtde;
                    }
                }
                else if (maoDeObra && isRelatorioPcp)
                {
                    uint? idAmbienteEspelho = ProdutosPedidoEspelhoDAO.Instance.ObtemIdAmbientePedido(pp.IdProdPedEsp.Value);
                    if (idAmbienteEspelho > 0)
                    {
                        pp.IdAmbientePedido = idAmbienteEspelho.Value;

                        AmbientePedido amb = AmbientePedidoDAO.Instance.GetElement(idAmbienteEspelho.Value, idPedido, isRelatorioPcp);
                        if (amb != null)
                        {
                            pp.Ambiente = amb.PecaVidroQtd;
                            pp.Redondo = amb.Redondo;
                        }
                    }
                }
                else if (pp.IdAmbientePedido == null && isRelatorioPcp)
                {
                    uint? idAmbienteEspelho = ProdutosPedidoEspelhoDAO.Instance.ObtemIdAmbientePedido(pp.IdProdPedEsp.Value);
                    if (idAmbienteEspelho > 0)
                    {
                        pp.IdAmbientePedido = idAmbienteEspelho.Value;

                        AmbientePedido amb = AmbientePedidoDAO.Instance.GetElement(idAmbienteEspelho.Value, idPedido, isRelatorioPcp);
                        if (amb != null)
                        {
                            pp.Ambiente = amb.Ambiente;
                            pp.DescrAmbiente = amb.Descricao;
                        }
                    }
                }

                // Verifica se o produto foi marcado como redondo (alterações neste trecho devem ser feitas também em ProdutosLiberarPedidoDAO.GetForRpt(uint))
                if (pp.Redondo)
                {
                    if (!maoDeObra && !pp.DescrProduto.ToLower().Contains("redondo"))
                        pp.DescrProduto += " REDONDO";

                    pp.Largura = 0;
                }

                // Exibe o percentual de desconto por qtd concatenado com a descrição
                if (Geral.ConcatenarDescontoPorQuantidadeNaDescricaoDoProduto && pp.PercDescontoQtde > 0)
                    pp.DescrProduto += "\r\n(Desc. Prod.: " + pp.PercDescontoQtde + "%)";

                lstProdPedRetorno.Add(pp);

                if (!incluirBeneficiamentos)
                    continue;

                // Carrega os beneficiamentos deste produto, se houver
                // Exibe os beneficiamentos do PCP, se for para o relatório do PCP
                pp.UsarBenefPcp = isRelatorioPcp;
                GenericBenefCollection lstBenef = pp.Beneficiamentos;

                if (!PedidoConfig.RelatorioPedido.AgruparBenefRelatorio)
                {
                    // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                    foreach (GenericBenef ppb in lstBenef)
                    {
                        if (ppb.IdBenefConfig == 0)
                            continue;

                        ProdutosPedido prodPed = new ProdutosPedido();
                        prodPed.IdPedido = idPedido;
                        prodPed.IdAmbientePedido = pp.IdAmbientePedido;
                        prodPed.IdItemProjeto = pp.IdItemProjeto;
                        prodPed.Ambiente = pp.Ambiente;
                        prodPed.Qtde = ppb.Qtd > 0 ? ppb.Qtd : 1;

                        /* Chamado 53945. */
                        if (pp.IdProdPedParent > 0)
                        {
                            prodPed.ValorVendido = 0;
                            prodPed.Total = 0;
                        }
                        else
                        {
                            prodPed.ValorVendido = ppb.ValorUnit;
                            prodPed.Total = ppb.Valor;
                        }

                        prodPed.ValorBenef = 0;
                        prodPed.DescrProduto = " " + ppb.DescricaoBeneficiamento +
                            Utils.MontaDescrLapBis(ppb.BisAlt, ppb.BisLarg, ppb.LapAlt, ppb.LapLarg, ppb.EspBisote, null, null, false);

                        lstProdPedRetorno.Add(prodPed);
                    }
                }
                else
                {
                    if (lstBenef.Count > 0)
                    {
                        ProdutosPedido prodPed = new ProdutosPedido();
                        prodPed.IdPedido = idPedido;
                        prodPed.IdAmbientePedido = pp.IdAmbientePedido;
                        prodPed.IdItemProjeto = pp.IdItemProjeto;
                        prodPed.Ambiente = pp.Ambiente;
                        prodPed.Qtde = 0;
                        prodPed.ValorBenef = 0;

                        // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                        foreach (GenericBenef ppb in lstBenef)
                        {
                            /* Chamado 53945. */
                            if (pp.IdProdPedParent > 0)
                            {
                                prodPed.ValorVendido = 0;
                                prodPed.Total = 0;
                            }
                            else
                            {
                                prodPed.ValorVendido += ppb.ValorUnit;
                                prodPed.Total += ppb.Valor;
                            }

                            string textoQuantidade = ppb.Qtd > 0 ? ppb.Qtd.ToString() + " " : "";
                            prodPed.DescrProduto += "; " + textoQuantidade + ppb.DescricaoBeneficiamento +
                                Utils.MontaDescrLapBis(ppb.BisAlt, ppb.BisLarg, ppb.LapAlt, ppb.LapLarg, ppb.EspBisote, null, null, false);
                        }

                        prodPed.DescrProduto = " " + prodPed.DescrProduto.Substring(2);
                        lstProdPedRetorno.Add(prodPed);
                    }
                }
            }

            return lstProdPedRetorno.ToArray();
        }

        #endregion

        #region GetForRptAmbiente

        /// <summary>
        /// Retorna os itens para o relatório de ambientes.
        /// </summary>
        public ProdutosPedido[] GetForRptAmbiente(uint idPedido, bool isRelatorioPcp)
        {
            ProdutosPedido[] itens = GetForRpt(idPedido, isRelatorioPcp);
            Dictionary<uint, List<ProdutosPedido>> dicionario = new Dictionary<uint, List<ProdutosPedido>>();

            foreach (ProdutosPedido item in itens)
            {
                if (item.IdAmbientePedido == null)
                    continue;

                if (!dicionario.ContainsKey(item.IdAmbientePedido.Value))
                    dicionario.Add(item.IdAmbientePedido.Value, new List<ProdutosPedido>());

                dicionario[item.IdAmbientePedido.Value].Add(item);
            }

            List<ProdutosPedido> retorno = new List<ProdutosPedido>();
            foreach (uint key in dicionario.Keys)
            {
                ProdutosPedido novo = new ProdutosPedido();
                novo.Qtde = !isRelatorioPcp ? AmbientePedidoDAO.Instance.GetQtde(key) : AmbientePedidoEspelhoDAO.Instance.GetQtde(key);
                novo.Qtde = novo.Qtde > 0 ? novo.Qtde : 1;
                novo.Ambiente = !isRelatorioPcp ? AmbientePedidoDAO.Instance.ObtemValorCampo<string>("ambiente", "idAmbientePedido=" + key) :
                    AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<string>("ambiente", "idAmbientePedido=" + key);
                novo.DescrProduto = !isRelatorioPcp ? AmbientePedidoDAO.Instance.ObtemValorCampo<string>("descricao", "idAmbientePedido=" + key) :
                    AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<string>("descricao", "idAmbientePedido=" + key);

                foreach (ProdutosPedido item in dicionario[key])
                {
                    novo.Total += item.Total;
                    novo.ValorVendido = novo.Total;
                }

                retorno.Add(novo);
            }

            retorno.Sort(new Comparison<ProdutosPedido>(delegate (ProdutosPedido x, ProdutosPedido y)
            {
                int ambiente = Comparer<string>.Default.Compare(x.Ambiente, y.Ambiente);
                if (ambiente == 0)
                    return Comparer<string>.Default.Compare(x.DescrAmbiente, y.DescrAmbiente);
                else
                    return ambiente;

            }));

            for (int i = 0; i < retorno.Count; i++)
                retorno[i].NumItem = (uint)i + 1;

            return retorno.ToArray();
        }

        #endregion

        #region Retorna Produtos do Pedido para confirmação

        /// <summary>
        /// Retorna os produtos do pedido a ser confirmado
        /// </summary>
        public IList<ProdutosPedido> GetForConfirmation(uint idPedido, string sortExpression, int startRow, int pageSize)
        {
            if (idPedido == 0 || !PedidoDAO.Instance.PedidoExists(idPedido) ||
                PedidoDAO.Instance.ObtemSituacao(null, idPedido) == Pedido.SituacaoPedido.Cancelado)
                return null;

            return LoadDataWithSortExpression(Sql(null, idPedido, 0, 0, 0, false, false, false, false, false, false, false, false, 0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetForConfirmationCount(uint idPedido)
        {
            if (idPedido == 0 || !PedidoDAO.Instance.PedidoExists(idPedido) ||
                PedidoDAO.Instance.ObtemSituacao(null, idPedido) == Pedido.SituacaoPedido.Cancelado)
                return 0;

            return objPersistence.ExecuteSqlQueryCount(Sql(null, idPedido, 0, 0, 0, false, false, false, false, false, false, false, false, 0, false), null);
        }

        #endregion

        #region Retorna Produtos do Pedido para saída de estoque

        /// <summary>
        /// Retorna os produtos do pedido que sairão do estoque
        /// </summary>
        public IList<ProdutosPedido> GetForSaidaEstoque(uint idPedido)
        {
            if (idPedido == 0 || !PedidoDAO.Instance.PedidoExists(idPedido) ||
                PedidoDAO.Instance.ObtemSituacao(null, idPedido) == Pedido.SituacaoPedido.Cancelado)
                return null;

            var sql = Sql((GDASession)null, null, idPedido, 0, 0, 0, false, true, false, false, false, false,
                /* Chamado 27604. */
                PCPConfig.UsarNovoControleExpBalcao, false, 0, true);

            if (PCPConfig.ControlarProducao)
            {
                if (Geral.EmpresaSomenteRevendeBox)
                {
                    sql += " and (p.idGrupoProd<>" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" Or p.idSubgrupoProd in (
                        select idSubgrupoProd from subgrupo_prod where idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" and produtosEstoque=true))";
                }
                else
                {
                    sql += " AND (p.idGrupoProd<>" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @"
                                    OR sg.tipoSubgrupo IN(" + (int)TipoSubgrupoProd.ChapasVidroLaminado + @"))";

                    if (OrdemCargaConfig.UsarControleOrdemCarga)
                        sql += " AND IF(ped.tipoEntrega <> " + (int)Pedido.TipoEntregaPedido.Balcao + ", COALESCE(sg.geraVolume, g.geravolume, false), false)=false";
                }
            }

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna os produtos do pedido que sairão do estoque
        /// </summary>
        public IList<ProdutosPedido> GetForSaidaEstoque(uint idPedido, string sortExpression, int startRow, int pageSize)
        {
            if (idPedido == 0 || !PedidoDAO.Instance.PedidoExists(idPedido) ||
                PedidoDAO.Instance.ObtemSituacao(null, idPedido) == Pedido.SituacaoPedido.Cancelado)
                return null;

            string sql = Sql(null, idPedido, 0, 0, 0, false, true, false, false, false, false, false, false, 0, true) +
                (PCPConfig.ControlarProducao ? " and p.idGrupoProd<>" + (int)Glass.Data.Model.NomeGrupoProd.Vidro : "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, null);
        }

        public int GetForSaidaEstoqueCount(uint idPedido)
        {
            if (idPedido == 0 || !PedidoDAO.Instance.PedidoExists(idPedido) ||
                PedidoDAO.Instance.ObtemSituacao(null, idPedido) == Pedido.SituacaoPedido.Cancelado)
                return 0;

            string sql = Sql(null, idPedido, 0, 0, 0, false, true, false, false, false, false, false, false, 0, false) +
                (PCPConfig.ControlarProducao ? " and p.idGrupoProd<>" + (int)Glass.Data.Model.NomeGrupoProd.Vidro : "");

            return objPersistence.ExecuteSqlQueryCount(sql, null);
        }

        #endregion

        #region Retorna Produtos do Pedido para corte

        /// <summary>
        /// Retorna os produtos do pedido para corte
        /// </summary>
        public IList<ProdutosPedido> GetForCorte(uint idPedido, int situacao, string sortExpression, int startRow, int pageSize)
        {
            // Verifica se o pedido pode ser retornado
            if (VerifyPedidoForCorte(idPedido, situacao) == null)
                return null;

            return LoadDataWithSortExpression(Sql(null, idPedido, 0, 0, 0, false, false, false, false, false, false, false, false, 0, true),
                sortExpression, startRow, pageSize, null);
        }

        public int GetForCorteCount(uint idPedido, int situacao)
        {
            if (VerifyPedidoForCorte(idPedido, situacao) == null)
                return 0;

            return objPersistence.ExecuteSqlQueryCount(Sql(null, idPedido, 0, 0, 0, false, false, false, false,
                false, false, false, false, 0, false), null);
        }

        private string VerifyPedidoForCorte(uint idPedido, int situacao)
        {
            if (idPedido == 0 || !PedidoDAO.Instance.PedidoExists(idPedido))
                return null;

            if (idPedido == 0 || situacao == 0)
                return null;
            else
            {
                bool existsPedidoCorte = PedidoCorteDAO.Instance.ExistsByPedido(idPedido);

                if (situacao == (int)PedidoCorte.SituacaoEnum.Producao)
                {
                    // Se já existir um pedido corte para este pedido, verifica sua situação
                    if (existsPedidoCorte)
                    {
                        PedidoCorte pedidoCorte = PedidoCorteDAO.Instance.GetByIdPedido(idPedido);

                        switch (pedidoCorte.Situacao)
                        {
                            case (int)PedidoCorte.SituacaoEnum.Producao:
                                return null;
                            case (int)PedidoCorte.SituacaoEnum.Pronto:
                                return null;
                            case (int)PedidoCorte.SituacaoEnum.Entregue:
                                return null;
                        }
                    }
                }
                else if (situacao == (int)PedidoCorte.SituacaoEnum.Pronto)
                {
                    // Se não existir um pedido corte para este pedido
                    if (!existsPedidoCorte)
                        return null;

                    PedidoCorte pedidoCorte = PedidoCorteDAO.Instance.GetByIdPedido(idPedido);

                    switch (pedidoCorte.Situacao)
                    {
                        case (int)PedidoCorte.SituacaoEnum.Confirmado:
                            return null;
                        case (int)PedidoCorte.SituacaoEnum.Pronto:
                            return null;
                        case (int)PedidoCorte.SituacaoEnum.Entregue:
                            return null;
                    }
                }
                else if (situacao == (int)PedidoCorte.SituacaoEnum.Entregue)
                {
                    // Se não existir um pedido corte para este pedido
                    if (!existsPedidoCorte)
                        return null;
                    PedidoCorte pedidoCorte = PedidoCorteDAO.Instance.GetByIdPedido(idPedido);

                    switch (pedidoCorte.Situacao)
                    {
                        case (int)PedidoCorte.SituacaoEnum.Confirmado:
                            return null;
                        case (int)PedidoCorte.SituacaoEnum.Producao:
                            return null;
                        case (int)PedidoCorte.SituacaoEnum.Entregue:
                            return null;
                    }
                }
            }

            if (PedidoDAO.Instance.ObtemSituacao(null, idPedido) == Pedido.SituacaoPedido.Cancelado)
                return null;

            return "ok";
        }

        #endregion

        #region Verifica se produto já foi adicionado

        /// <summary>
        /// Verifica se produto com a altura e largura passadas já foi adicionado
        /// </summary>
        public bool ExistsInPedido(uint idPedido, uint idProd, Single altura, int largura, uint? idProcesso, uint? idAplicacao)
        {
            string sql = "Select count(*) From produtos_pedido where idPedido=" + idPedido + " And idProd=" + idProd +
                " And altura=" + altura.ToString().Replace(',', '.') + " And largura=" + largura + " and (InvisivelPedido=false or InvisivelPedido is null)";

            return objPersistence.ExecuteSqlQueryCount(sql, null) > 0;
        }

        /// <summary>
        /// Verifica se produto com a altura e largura passadas já foi adicionado, não sendo o ProdutoPedido passado
        /// </summary>
        public bool ExistsInPedidoUpdate(uint idProdPed, uint idPedido, uint idProd, Single altura, int largura, uint? idProcesso, uint? idAplicacao)
        {
            string sql = "Select count(*) From produtos_pedido where idPedido=" + idPedido + " And idProd=" + idProd +
                " And altura=" + altura.ToString().Replace(',', '.') + " And largura=" + largura + " And idProdPed<>" + idProdPed +
                " and (InvisivelPedido=false or InvisivelPedido is null)";

            return objPersistence.ExecuteSqlQueryCount(sql, null) > 0;
        }

        #endregion

        #region Verifica se há algum produtoPedido que não possui itemProjeto associado

        /// <summary>
        /// Verifica se há algum produtoPedido que não possui itemProjeto associado
        /// </summary>
        public bool ExisteSemItemProjeto(uint idPedido)
        {
            string sql = "Select Count(*) From produtos_pedido Where idItemProjeto is null and idPedido=" + idPedido + " and (InvisivelPedido=false or InvisivelPedido is null)";

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString()) > 0;
        }

        #endregion

        #region Retorna o total dos produtos do pedido

        public string GetTotalByPedido(uint idPedido)
        {
            return GetTotalByPedido(null, idPedido);
        }

        public string GetTotalByPedido(GDASession session, uint idPedido)
        {
            return objPersistence.ExecuteScalar(session, "Select Round(Sum(Total+ValorBenef), 2) From produtos_pedido where idpedido=" + idPedido + " and (InvisivelPedido=false or InvisivelPedido is null)").ToString();
        }

        public decimal GetTotalByPedidoFluxo(uint idPedido)
        {
            return GetTotalByPedidoFluxo(null, idPedido);
        }

        public decimal GetTotalByPedidoFluxo(GDASession session, uint idPedido)
        {
            return ExecuteScalar<decimal>("Select Round(Sum(Total+ValorBenef), 2) From produtos_pedido where idpedido=" + idPedido + " and Coalesce(InvisivelFluxo, false)=false");
        }

        public string GetTotalByVariosPedidos(string idsPedidos)
        {
            return objPersistence.ExecuteScalar("Select Round(Sum(coalesce(Total,0)+coalesce(ValorBenef,0)), 2) From produtos_pedido where idpedido in (" + idsPedidos + ") and (InvisivelPedido=false or InvisivelPedido is null)").ToString();
        }

        #endregion

        #region Retorna IdAmbiente a partir de IdItemProjeto

        /// <summary>
        /// Busca o IdAmbientePedido dos produtos que vieram do IdItemProjeto passado
        /// </summary>
        public uint? GetAmbienteFromItemProj(uint idItemProjeto)
        {
            string sql = "Select Coalesce(idAmbientePedido, 0) From produtos_pedido Where idItemProjeto=" + idItemProjeto + " limit 1";

            object idAmbientePedido = objPersistence.ExecuteScalar(sql);

            if (idAmbientePedido == null || Glass.Conversoes.StrParaUint(idAmbientePedido.ToString()) == 0)
                return null;

            return Glass.Conversoes.StrParaUint(idAmbientePedido.ToString());
        }

        #endregion

        #region Marca saída de produtos

        /// <summary>
        /// Marca saída de produto na tabela produtos_pedido
        /// </summary>
        public void MarcarSaida(GDASession sessao, uint idProdPed, float qtdSaida, uint idSaidaEstoque, string metodo, string numEtiqueta)
        {

            if (idProdPed == 0)
                return;

            var idPedido = (int)ObtemIdPedido(idProdPed);

            if (!string.IsNullOrWhiteSpace(metodo)
                && PedidoDAO.Instance.GetTipoPedido(sessao, (uint)idPedido) == Pedido.TipoPedidoEnum.Revenda)
            {
                var idProd = ObtemValorCampo<uint>(sessao, "idProd", $"idProdPed = { idProdPed }");
                var codInterno = ProdutoDAO.Instance.ObtemValorCampo<string>(sessao, "CodInterno", $"idProd = { idProd }");
                var descricaoProd = ProdutoDAO.Instance.GetDescrProduto(sessao, codInterno);

                var logData = new LogAlteracao();
                logData.Tabela = (int)LogAlteracao.TabelaAlteracao.Pedido;
                logData.IdRegistroAlt = idPedido;
                logData.NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(sessao, LogAlteracao.TabelaAlteracao.Pedido, idPedido);
                logData.Campo = $"MarcarSaida - { metodo }";
                logData.DataAlt = DateTime.Now;
                logData.IdFuncAlt = (uint)FuncionarioDAO.Instance.GetAdministradores(true).Where(f => f.AdminSync == true).Select(f => f.IdFunc).FirstOrDefault();
                logData.ValorAnterior = $"{ codInterno } - { descricaoProd } ({ ObtemValorCampo<string>(sessao, "Qtde", $"IdProdPed = { idProdPed }") }/{ ObtemValorCampo<string>(sessao, "QtdSaida", $"IdProdPed = { idProdPed }") })";
                logData.ValorAtual = $"{ numEtiqueta ?? "Sem Etiqueta" } \nQtd. Estorno ({ qtdSaida.ToString() }) \nFuncionário ({ (UserInfo.GetUserInfo != null ? $"{ UserInfo.GetUserInfo.CodUser } - { UserInfo.GetUserInfo.Nome })" : string.Empty)}";
                logData.Referencia = LogAlteracao.GetReferencia(sessao, (int)LogAlteracao.TabelaAlteracao.Pedido, (uint)idPedido);

                LogAlteracaoDAO.Instance.Insert(sessao, logData);
            }

            string sql = $"Update produtos_pedido set qtdSaida=greatest(Coalesce(qtdSaida, 0)+?qtdSaida, 0) Where idProdPed={ idProdPed }";
            objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?qtdSaida", qtdSaida));

            // Insere um registro na tabela indicando que o produto foi baixado
            if (idSaidaEstoque > 0)
            {
                ProdutoSaidaEstoque novo = new ProdutoSaidaEstoque();
                novo.IdSaidaEstoque = idSaidaEstoque;
                novo.IdProdPed = idProdPed;
                novo.QtdeSaida = qtdSaida;

                ProdutoSaidaEstoqueDAO.Instance.Insert(sessao, novo);
            }
        }

        #endregion

        #region Estorno saída de produtos

        public void EstornoSaida(GDASession sessao, uint idProdPed, float qtdEstorno, string metodo, string numEtiqueta)
        {

            if (idProdPed == 0)
                return;

            var idPedido = (int)ObtemIdPedido(idProdPed);

            if (!string.IsNullOrWhiteSpace(metodo)
                && PedidoDAO.Instance.GetTipoPedido(sessao, (uint)idPedido) == Pedido.TipoPedidoEnum.Revenda)
            {
                var idProd = ObtemValorCampo<uint>(sessao, "idProd", $"idProdPed = { idProdPed }");
                var codInterno = ProdutoDAO.Instance.ObtemValorCampo<string>(sessao, "CodInterno", $"idProd = { idProd }");
                var descricaoProd = ProdutoDAO.Instance.GetDescrProduto(sessao, codInterno);

                var logData = new LogAlteracao();
                logData.Tabela = (int)LogAlteracao.TabelaAlteracao.Pedido;
                logData.IdRegistroAlt = idPedido;
                logData.NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(sessao, LogAlteracao.TabelaAlteracao.Pedido, idPedido);
                logData.Campo = $"EstornoSaida - { metodo }";
                logData.DataAlt = DateTime.Now;
                logData.IdFuncAlt = UserInfo.GetUserInfo != null ? UserInfo.GetUserInfo.CodUser : 0;
                logData.ValorAnterior = $"{ codInterno } - { descricaoProd } ({ ObtemValorCampo<string>(sessao, "Qtde", $"IdProdPed = { idProdPed }")}/{ ObtemValorCampo<string>(sessao, "QtdSaida", $"IdProdPed = { idProdPed }") })";
                logData.ValorAtual = $"{ numEtiqueta ?? "Sem Etiqueta" } \nQtd. Estorno ({ qtdEstorno.ToString() }) \nFuncionário ({ (UserInfo.GetUserInfo != null ? $"{ UserInfo.GetUserInfo.CodUser } - { UserInfo.GetUserInfo.Nome })" : string.Empty)}";
                logData.Referencia = LogAlteracao.GetReferencia(sessao, (int)LogAlteracao.TabelaAlteracao.Pedido, (uint)idPedido);

                LogAlteracaoDAO.Instance.Insert(sessao, logData);
            }

            string sql = $"Update produtos_pedido set qtdSaida=(Coalesce(qtdSaida, 0)-{ qtdEstorno }) Where idProdPed={ idProdPed }";
            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Atualiza total do beneficiamento aplicado neste produto

        /// <summary>
        /// Atualiza os beneficiamentos de um produto.
        /// </summary>
        internal void AtualizaBenef(GDASession sessao, uint idProdPed, GenericBenefCollection beneficiamentos, IContainerCalculo container)
        {
            ProdutoPedidoBenefDAO.Instance.DeleteByProdPed(sessao, idProdPed);
            foreach (ProdutoPedidoBenef ppb in beneficiamentos.ToProdutosPedido(idProdPed))
                if (ppb.IdBenefConfig > 0)
                    ProdutoPedidoBenefDAO.Instance.Insert(sessao, ppb);

            UpdateValorBenef(sessao, idProdPed, container);
        }

        /// <summary>
        /// Calcula o valor total do produto com o beneficiamento aplicado
        /// </summary>
        public void UpdateValorBenef(GDASession sessao, uint idProdPed)
        {
            var idPedido = ObtemValorCampo<uint>("idPedido", "idProdPed=" + idProdPed);
            var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(sessao, idPedido);

            UpdateValorBenef(sessao, idProdPed, pedido);
        }

        public void UpdateValorBenef(GDASession sessao, uint idProdPed, IContainerCalculo container)
        {
            var idProd = ObtemIdProd(sessao, idProdPed);
            if (Glass.Configuracoes.Geral.NaoVendeVidro() || !ProdutoDAO.Instance.CalculaBeneficiamento(sessao, (int)idProd))
                return;

            string sql = @"Update produtos_pedido pp set pp.valorBenef=
                Coalesce(Round((Select Sum(ppb.Valor) from produto_pedido_benef ppb Where ppb.idProdPed=" + idProdPed + @"), 2), 0)
                Where pp.idProdPed=" + idProdPed;

            objPersistence.ExecuteCommand(sessao, sql);

            // Recalcula o total bruto/valor unitário bruto
            ProdutosPedido pp = GetElementByPrimaryKey(sessao, idProdPed);
            UpdateBase(sessao, pp, container);
        }

        #endregion

        #region Apaga referência aos materiais de itemProjeto

        /// <summary>
        /// Apaga referência aos materiais de itemProjeto
        /// </summary>
        public void ApagaRefMaterialItemProj(uint idPedido)
        {
            string sql = "Update produtos_pedido set idMaterItemProj=null where idPedido=" + idPedido;

            objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Exclui produtos do pedido

        /// <summary>
        /// Exclui todos os produtos de um pedido
        /// </summary>
        public void DeleteByPedido(uint idPedido)
        {
            string sql = "Delete From produtos_pedido Where idPedido=" + idPedido;

            objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Fast Delivery

        /// <summary>
        /// Retorna o número de m² de produção para Fast Delivery de um dia.
        /// </summary>
        public float TotalM2FastDelivery(DateTime dia)
        {
            return TotalM2FastDelivery(dia, null);
        }

        /// <summary>
        /// Retorna o número de m² de produção para Fast Delivery de um dia, talvez desconsiderando um pedido.
        /// </summary>
        public float TotalM2FastDelivery(DateTime dia, uint? idPedido)
        {
            return TotalM2FastDelivery(null, dia, idPedido);
        }

        /// <summary>
        /// Retorna o número de m² de produção para Fast Delivery de um dia, talvez desconsiderando um pedido.
        /// </summary>
        public float TotalM2FastDelivery(GDASession session, DateTime dia, uint? idPedido)
        {
            object retorno = objPersistence.ExecuteScalar(session, InfoPedidosDAO.Instance.SqlFastDelivery(session, idPedido, dia.ToShortDateString(), dia.ToShortDateString()));

            return retorno != null && retorno.ToString() != "" ? float.Parse(retorno.ToString()) : 0;
        }

        /// <summary>
        /// Retorna o dia em que o pedido pode ser agendado para Fast Delivery.
        /// </summary>
        public DateTime? GetFastDeliveryDay(uint idPedido, DateTime dataEntrega, float totalM2Pedido)
        {
            return GetFastDeliveryDay(idPedido, dataEntrega, totalM2Pedido, true);
        }

        /// <summary>
        /// Retorna o dia em que o pedido pode ser agendado para Fast Delivery.
        /// </summary>
        internal DateTime? GetFastDeliveryDay(uint idPedido, DateTime dataEntrega, float totalM2Pedido,
            bool verificarDataEntrega)
        {
            return GetFastDeliveryDay(null, idPedido, dataEntrega, totalM2Pedido, verificarDataEntrega);
        }

        /// <summary>
        /// Retorna o dia em que o pedido pode ser agendado para Fast Delivery.
        /// </summary>
        internal DateTime? GetFastDeliveryDay(GDASession session, uint idPedido, DateTime dataEntrega, float totalM2Pedido, bool verificarDataEntrega)
        {
            DateTime retorno = dataEntrega;

            int i = 0;
            while (PedidoConfig.Pedido_FastDelivery.PrazoEntregaFastDelivery > 0 ? i < PedidoConfig.Pedido_FastDelivery.PrazoEntregaFastDelivery : true)
            {
                if (IsFastDeliveryDay(session, retorno, idPedido, totalM2Pedido))
                    break;

                retorno = retorno.AddDays(1);
                while (!retorno.DiaUtil())
                    retorno = retorno.AddDays(1);

                i++;
            }

            if (PedidoConfig.Pedido_FastDelivery.PrazoEntregaFastDelivery == 0 || i < PedidoConfig.Pedido_FastDelivery.PrazoEntregaFastDelivery)
                return verificarDataEntrega ? PedidoDAO.Instance.GetDataEntregaMinimaFinal(session, idPedido, retorno, true).Date : retorno.Date;
            else
                return null;
        }

        /// <summary>
        /// Verifica se a data escolhida pode ser usada para Fast Delivery.
        /// </summary>
        public bool IsFastDeliveryDay(DateTime data, uint idPedido, float totalM2Pedido)
        {
            return IsFastDeliveryDay(null, data, idPedido, totalM2Pedido);
        }

        /// <summary>
        /// Verifica se a data escolhida pode ser usada para Fast Delivery.
        /// </summary>
        public bool IsFastDeliveryDay(GDASession session, DateTime data, uint idPedido, float totalM2Pedido)
        {
            if (totalM2Pedido > PedidoConfig.Pedido_FastDelivery.M2MaximoFastDelivery)
                throw new Exception("Número de metros quadrados do pedido é maior que o máximo permitido diário para Fast Delivery.");

            return (TotalM2FastDelivery(session, data, idPedido) + totalM2Pedido) <= PedidoConfig.Pedido_FastDelivery.M2MaximoFastDelivery;
        }

        #endregion

        #region Retorna o total de m² do pedido

        /// <summary>
        /// Retorna o total de m² de um pedido.
        /// </summary>
        public float GetTotalM2ByPedido(uint idPedido)
        {
            return GetTotalM2ByPedido(null, idPedido);
        }

        /// <summary>
        /// Retorna o total de m² de um pedido.
        /// </summary>
        public float GetTotalM2ByPedido(GDASession session, uint idPedido)
        {
            string sql = "select Coalesce(sum(TotM), 0) from produtos_pedido pp left join produto p on (pp.idProd=p.idProd) " +
                "left join grupo_prod g on (p.idGrupoProd=g.idGrupoProd) left join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd) " +
                "where IdPedido=" + idPedido + InfoPedidosDAO.Instance.SqlWhere(true);

            return float.Parse(objPersistence.ExecuteScalar(session, sql).ToString().Replace(".", ","));
        }

        #endregion

        #region Produtos a serem liberados

        private string SqlLiberacao(string idsPedidos, uint idPedido, uint idAmbientePedido, uint idProdPed, uint idProdPedEsp)
        {
            uint idCliente = idPedido > 0 ? PedidoDAO.Instance.ObtemIdCliente(null, idPedido) :
                !String.IsNullOrEmpty(idsPedidos) ? PedidoDAO.Instance.ObtemIdCliente(null, Glass.Conversoes.StrParaUint(idsPedidos.Split(',')[0])) : 0;

            var idPedidoVerificarLiberacaoPedidosProntos = idPedido > 0 ? idPedido : !String.IsNullOrEmpty(idsPedidos) ? idsPedidos.Split(',').FirstOrDefault().StrParaUint() : 0;

            var idLoja = PedidoDAO.Instance.ObtemIdLoja(null, idPedidoVerificarLiberacaoPedidosProntos);
            var naoIgnorar = idLoja > 0 ? !LojaDAO.Instance.GetIgnorarLiberarProdutosProntos(null, idLoja) : true;
            bool isClienteRota = RotaClienteDAO.Instance.IsClienteAssociado(idCliente);
            bool liberarProdutosProntos = (Liberacao.DadosLiberacao.LiberarProdutosProntos && naoIgnorar) && !(Liberacao.DadosLiberacao.LiberarClienteRota && isClienteRota);

            /* Chamado 34259. */
            var tipoReposicaoPedido = ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Pedido;

            string sqlProdutoTabela = "if(ped.TipoEntrega in (" + (int)Pedido.TipoEntregaPedido.Balcao + ", " +
                (int)Pedido.TipoEntregaPedido.Entrega + "), p.ValorBalcao, p.ValorObra)";

            string campoTipoSetorProducao = "if(ppp.idProdPedProducao is null" + (Liberacao.DadosLiberacao.LiberarClienteRota ? " or ped.idCli in (select rcselect.idCliente from rota_cliente rcselect)" : "") +
                ", " + (int)SituacaoProdutoProducao.Pronto +
                (tipoReposicaoPedido ?
                    string.Format(", if(ppp.Situacao={0}, ppp.situacaoProducao, ppp1.situacaoProducao))", (int)ProdutoPedidoProducao.SituacaoEnum.Producao) :
                    ", ppp.situacaoProducao)");

            string campos = @"pp.*, p.Descricao as DescrProduto, p.CodInterno, p.IdGrupoProd, g.descricao as descrGrupoProd,
                mip.idMaterProjMod, mip.IdPecaItemProj, p.idSubgrupoProd, if(p.AtivarAreaMinima=1,
                Cast(p.AreaMinima as char), '0') as AreaMinima, Round(0, 2) as AliqICMSProd, p.Cst,
                ap.Ambiente, ap.Descricao as DescrAmbiente, " + sqlProdutoTabela + @" as ValorProdutoTabela, " +
                isClienteRota.ToString().ToLower() + @" as isClienteRota, ped.OrdemCargaParcial" + (liberarProdutosProntos ? ", cast(" +
                campoTipoSetorProducao + @" as signed) as TipoSetorProducao, cast(if(ppp.numEtiqueta is not null, 1, null)
                as signed) as qtdeEtiquetas, ppp.numEtiqueta as numEtiquetaConsulta, ppp.idProdPedProducao as idProdPedProducaoConsulta,
                (plp.idProdLiberarPedido is not null or (select count(*) from produtos_liberar_pedido plpselect where plpselect.qtdeCalc>0
                and plpselect.idProdPed=pp.IdProdPed)=0) as temLiberacaoEtiqueta" : "");

            string sql = "Select " + campos + @" From produtos_pedido pp
                Left Join pedido ped On (pp.idPedido=ped.idPedido)
                Left Join pedido_espelho pedEsp On (ped.idPedido=pedEsp.idPedido)
                Left Join produto p On (pp.idProd=p.idProd)
                Left Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd)
                Left Join subgrupo_prod sg On (p.idSubgrupoProd=sg.idSubgrupoProd)
                Left Join ambiente_pedido ap On (pp.idAmbientePedido=ap.idAmbientePedido)
                Left Join material_item_projeto mip On (pp.idMaterItemProj=mip.idMaterItemProj)
                Left Join produtos_pedido_espelho ppe on (pp.idProdPedEsp=ppe.idProdPed)" +
                (liberarProdutosProntos ? @"
                    Left Join ambiente_pedido_espelho ape on (ppe.idAmbientePedido=ape.idAmbientePedido)
                    Left Join produto_pedido_producao ppp on (pp.idProdPedEsp=ppp.idProdPed)
                    Left Join setor s on (ppp.idSetor=s.idSetor)
                    " +
                    (tipoReposicaoPedido ?
                        @"Left Join produtos_pedido pp1 on (ppp.idProdPedProducao=pp1.idProdPedProdRepos)
                        Left Join produto_pedido_producao ppp1 on (ppp1.idProdPed=pp1.idProdPedEsp)
                        Left Join setor s1 on (ppp1.idSetor=s1.idSetor)" : string.Empty) +
                    @"Left Join produtos_liberar_pedido plp on (ppp.idProdPedProducao=plp.idProdPedProducao and plp.qtdeCalc>0) " : "") + @"
                Where pp.IdProdPedParent IS NULL ";

            if (!String.IsNullOrEmpty(idsPedidos))
                sql += " and pp.idPedido in (" + idsPedidos + ")";
            else if (idPedido > 0)
                sql += " and pp.idPedido=" + idPedido;

            if (idAmbientePedido > 0)
                sql += " And pp.idAmbientePedido=" + idAmbientePedido;

            if (idProdPed > 0)
                sql += " and pp.idProdPed=" + idProdPed;
            else if (idProdPedEsp > 0)
                sql += " and pp.idProdPedEsp=" + idProdPedEsp;

            if (!PCPConfig.UsarConferenciaFluxo)
                sql += " and (pp.InvisivelPedido=false or pp.InvisivelPedido is null)";
            else
            {
                // Chamado 15160: Ocorreu um problema ao excluir alguns produtos deste pedido, no entanto, alterando a condição abaixo,
                // garante que caso o pedido possua espelho, só busque produtos que estejam na conferência
                sql += @" and (pp.InvisivelFluxo=false or pp.InvisivelFluxo is null)
                    and (pedEsp.idPedido is null or ppe.idProdPed is not null)
                    /*and (pp.idProdPedEsp is null or ppe.idProdPed is not null)*/";
            }

            if (liberarProdutosProntos)
            {
                string ignoraRota = !Liberacao.DadosLiberacao.LiberarClienteRota ? "" : @"ped.idCli in (select idCliente from rota_cliente) or ";

                sql += " and (" + ignoraRota +
                    (tipoReposicaoPedido ?
                        "if(ppp1.situacao is not null, ppp1.situacao, ppp.situacao)=" :
                        "ppp.situacao=") +
                    (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                    Or (ppp.situacao is null and (g.idgrupoprod<>" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" Or sg.produtosEstoque=true)))
                    group by ppp.idProdPedProducao, " +
                    (tipoReposicaoPedido ?
                        "if(pp1.idProdPedAnterior is not null, pp1.idProdPedAnterior, pp.idProdPed)" :
                        "pp.idProdPed") +
                    ", tipoSetorProducao";
            }

            sql += " order by pp.idProdPed" + (liberarProdutosProntos ? ", ppp.numEtiqueta" : "");
            return sql;
        }

        /// <summary>
        /// Retorna os produtos para liberação de um pedido.
        /// </summary>
        public ProdutosPedido[] GetForLiberacao(uint idPedido)
        {
            return GetForLiberacao(null, idPedido);
        }

        /// <summary>
        /// Retorna os produtos para liberação de um pedido.
        /// </summary>
        public ProdutosPedido[] GetForLiberacao(uint idPedido, string idsOc)
        {
            return GetForLiberacao(null, idPedido, idsOc);
        }

        /// <summary>
        /// Retorna os produtos para liberação de vários pedidos.
        /// </summary>
        public ProdutosPedido[] GetForLiberacao(string idsPedidos)
        {
            return GetForLiberacao(null, idsPedidos, string.Empty);
        }

        /// <summary>
        /// Retorna os produtos para liberação de vários pedidos.
        /// </summary>
        public ProdutosPedido[] GetForLiberacao(string idsPedidos, string idsOc)
        {
            return GetForLiberacao(null, idsPedidos, idsOc);
        }

        /// <summary>
        /// Retorna os produtos para liberação de vários pedidos.
        /// </summary>
        public ProdutosPedido[] GetForLiberacao(GDASession session, string idsPedidos, string idsOc)
        {
            return GetForLiberacao(session, idsPedidos, true, false, idsOc);
        }

        /// <summary>
        /// Retorna os produtos para liberação de um pedido.
        /// </summary>
        public ProdutosPedido[] GetForLiberacao(GDASession session, uint idPedido, string idsOc)
        {
            if (idPedido == 0)
                return new ProdutosPedido[0];

            return GetForLiberacao(session, idPedido.ToString(), idsOc);
        }

        /// <summary>
        /// Retorna os produtos para liberação de um pedido.
        /// </summary>
        public ProdutosPedido[] GetForLiberacao(GDASession session, uint idPedido)
        {
            if (idPedido == 0)
                return new ProdutosPedido[0];

            return GetForLiberacao(session, idPedido.ToString());
        }

        /// <summary>
        /// Retorna os produtos para liberação de vários pedidos.
        /// </summary>
        public ProdutosPedido[] GetForLiberacao(GDASession session, string idsPedidos)
        {
            return GetForLiberacao(session, idsPedidos, true, false);
        }

        /// <summary>
        /// Retorna os produtos para liberação de vários pedidos.
        /// </summary>
        public ProdutosPedido[] GetForLiberacao(GDASession session, string idsPedidos, bool removerProdutosOrdemCargaParcial, bool isRelatorioPedidoParcial)
        {
            return GetForLiberacao(session, idsPedidos, removerProdutosOrdemCargaParcial, isRelatorioPedidoParcial, string.Empty);
        }

        /// <summary>
        /// Retorna os produtos para liberação de vários pedidos.
        /// </summary>
        public ProdutosPedido[] GetForLiberacao(GDASession session, string idsPedidos, bool removerProdutosOrdemCargaParcial, bool isRelatorioPedidoParcial, string idsOc)
        {
            if (string.IsNullOrEmpty(idsPedidos))
                return new ProdutosPedido[0];

            var retorno = objPersistence.LoadData(session, SqlLiberacao(idsPedidos, 0, 0, 0, 0)).ToList();

            for (var i = retorno.Count - 1; i >= 0; i--)
            {
                /* Chamado 64589. */
                if (removerProdutosOrdemCargaParcial && retorno[i].OrdemCargaParcial)
                {
                    #region Verifica se o subgrupo permite produto de revenda em um pedido de venda

                    var subgrupoPermiteItemRevendaNaVenda = false;

                    if (retorno[i].IdProd > 0)
                    {
                        var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, (int)retorno[i].IdProd);

                        if (idSubgrupoProd > 0)
                            subgrupoPermiteItemRevendaNaVenda = SubgrupoProdDAO.Instance.ObtemPermitirItemRevendaNaVenda((int)idSubgrupoProd);
                    }

                    #endregion

                    if (!isRelatorioPedidoParcial)
                    {
                        if (!subgrupoPermiteItemRevendaNaVenda && ItemCarregamentoDAO.Instance.ObterQtdeLiberarParcial(session, retorno[i].IdProdPed, idsOc) == 0)
                        {
                            retorno.RemoveAt(i);
                            continue;
                        }
                    }
                }

                if (retorno[i].QtdeDisponivelLiberacao <= 0)
                    retorno.RemoveAt(i);
            }

            return retorno.ToArray();
        }

        #endregion

        #region Reposição de produto

        private string SqlRepos(uint idPedidoRepos, bool selecionar)
        {
            var campos = selecionar ? @"pp.*, p.Descricao as DescrProduto, p.CodInterno, p.IdGrupoProd, mip.idMaterProjMod,
                p.idSubgrupoProd, if(p.AtivarAreaMinima=1, Cast(p.AreaMinima as char), '0') as AreaMinima, apl.CodInterno as CodAplicacao,
                prc.CodInterno as CodProcesso, p.Cst, a.Ambiente as Ambiente" + (Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos ? @", ppp.numEtiqueta as NumEtiquetaConsulta,
                ppp.idProdPedProducao as idProdPedProducaoConsulta" : "") : "Count(*)";

            var permitirTroca = PedidoConfig.PermitirTrocaPorPedido &&
                PedidoReposicaoDAO.Instance.PedidoParaTroca(idPedidoRepos);

            var sql =
                string.Format(@"
                    Select {0}
                    From produtos_pedido pp
                        Left Join ambiente_pedido a On (pp.IdAmbientePedido=a.IdAmbientePedido)
                        Left Join produto p On (pp.idProd=p.idProd)
                        Left Join material_item_projeto mip On (pp.idMaterItemProj=mip.idMaterItemProj)
                        Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao)
                        Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso)
                        {1}
                    Where pp.idPedido={2}
                        {3}
                        And (pp.idProdPed{4}) not in (
                            select distinct idProdPedAnterior{5}
                            from produtos_pedido
                            where idProdPedAnterior is not null
                                and idPedido={6}
                            )",
                    campos,
                    Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos ?
                        "Left Join produto_pedido_producao ppp On (pp.idProdPedEsp=ppp.idProdPed)" : "",
                    PedidoDAO.Instance.ObtemValorCampo<uint>("idPedidoAnterior", "idPedido=" + idPedidoRepos),
                    Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos ?
                        string.Format(" AND p.IdGrupoProd={0} AND ppp.Situacao IN ({1}{2})", (int)Glass.Data.Model.NomeGrupoProd.Vidro,
                            (int)ProdutoPedidoProducao.SituacaoEnum.Perda,
                                string.Format("{0}",
                                    permitirTroca ? string.Format(",{0}", (int)ProdutoPedidoProducao.SituacaoEnum.Producao) : "")) : "",
                    Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos ? ", ppp.numEtiqueta" : "",
                    Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos ? ", numEtiquetaRepos" : "",
                    idPedidoRepos);

            if (PCPConfig.CriarClone)
                sql += " And (pp.Invisivel{0}=false or pp.Invisivel{0} is null)";

            return String.Format(sql, (PCPConfig.UsarConferenciaFluxo || Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos) && PCPConfig.CriarClone ? "Fluxo" : "Pedido");
        }

        /// <summary>
        /// Retorna os produtos que ainda não foram inclusos na reposição de pedido.
        /// </summary>
        public ProdutosPedido[] GetForRepos(uint idPedidoRepos)
        {
            List<ProdutosPedido> lstProdPed = objPersistence.LoadData(SqlRepos(idPedidoRepos, true) + " order by pp.IdAmbientePedido, pp.IdProdPed");

            if (Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos)
                for (int i = lstProdPed.Count - 1; i >= 0; i--)
                    if (!ProdutoPedidoProducaoDAO.Instance.PodeReporPeca(lstProdPed[i].IdProdPed, lstProdPed[i].NumEtiquetaConsulta))
                        lstProdPed.RemoveAt(i);

            return lstProdPed.ToArray();
        }

        public void UpdateRepos(uint idProdPed, int qtde)
        {
            string sql = "update produtos_pedido set Qtde=" + qtde +
                " where IdProdPed=" + idProdPed;

            objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Espelho de Projeto

        /// <summary>
        /// Altera o ID do produto pedido espelho no produto do pedido.
        /// </summary>
        public void SetIdProdPed(uint idProdPed, uint idProdPedEsp, uint idMaterItemProj)
        {
            objPersistence.ExecuteCommand("update produtos_pedido set idProdPedEsp=" + idProdPedEsp + ", idMaterItemProj=" + idMaterItemProj + " where idProdPed=" + idProdPed);
        }

        /// <summary>
        /// Retorna o ID do produto do pedido pelo ID do material do projeto.
        /// </summary>
        public uint GetIdProdPedByMaterItemProj(uint idPedido, uint idMaterItemProj)
        {
            return GetIdProdPedByMaterItemProj(null, idPedido, idMaterItemProj);
        }

        /// <summary>
        /// Retorna o ID do produto do pedido pelo ID do material do projeto.
        /// </summary>
        public uint GetIdProdPedByMaterItemProj(GDASession sessao, uint idPedido, uint idMaterItemProj)
        {
            object retorno = objPersistence.ExecuteScalar(sessao, "select idProdPed from produtos_pedido where idPedido=" + idPedido + " and idMaterItemProj=" + idMaterItemProj);
            return retorno != null && retorno.ToString() != "" ? Glass.Conversoes.StrParaUint(retorno.ToString()) : 0;
        }

        /// <summary>
        /// Retorna o ID do produto do pedido pelos dados do produto pedido espelho.
        /// </summary>
        public uint GetIdProdPedByProdPedEsp(uint idPedido, ProdutosPedidoEspelho prodPedEsp)
        {
            object retorno = objPersistence.ExecuteScalar("select idProdPed from produtos_pedido where idPedido=" + idPedido + " and idItemProjeto" +
                (prodPedEsp.IdItemProjeto != null ? "=" + prodPedEsp.IdItemProjeto.Value : " is null") + " and idProd=" + prodPedEsp.IdProd +
                " and altura=" + prodPedEsp.Altura.ToString().Replace(',', '.') + " and largura=" + prodPedEsp.Largura + " and qtde=" + prodPedEsp.Qtde +
                " and idProdPedEsp is null");

            return retorno != null && retorno.ToString() != "" ? Glass.Conversoes.StrParaUint(retorno.ToString()) : 0;
        }

        /// <summary>
        /// Retorna o ID do produto do pedido pelos dados do produto pedido espelho.
        /// </summary>
        public uint GetIdProdPedByProdPedEsp(uint idProdPedEsp)
        {
            return GetIdProdPedByProdPedEsp(null, idProdPedEsp);
        }

        /// <summary>
        /// Retorna o ID do produto do pedido pelos dados do produto pedido espelho.
        /// </summary>
        public uint GetIdProdPedByProdPedEsp(GDASession session, uint idProdPedEsp)
        {
            object retorno = objPersistence.ExecuteScalar(session, "select idProdPed from produtos_pedido where idProdPedEsp=" + idProdPedEsp);
            return retorno != null && retorno.ToString() != "" ? Glass.Conversoes.StrParaUint(retorno.ToString()) : 0;
        }

        /// <summary>
        /// Retorna o ID do produto do pedido pelos dados do produto pedido espelho.
        /// </summary>
        public uint GetIdProdPedEspByProdPed(GDASession session, uint idProdPed)
        {
            object retorno = objPersistence.ExecuteScalar(session, "select idProdPedEsp from produtos_pedido where idProdPed=" + idProdPed);
            return retorno != null && retorno.ToString() != "" ? Glass.Conversoes.StrParaUint(retorno.ToString()) : 0;
        }

        #region Altera os flags de invisibilidade dos produtos de um projeto

        /// <summary>
        /// Reinicia os flags dos itens do projeto.
        /// </summary>
        public void ReiniciaItensProjeto(uint idPedido)
        {
            // Garante que o pedido seja de um projeto
            if (objPersistence.ExecuteSqlQueryCount("select count(*) from pedido where idProjeto is not null and idPedido=" + idPedido) == 0)
                return;

            // Reinicia os itens do pedido
            objPersistence.ExecuteCommand("delete from produtos_pedido where invisivelPedido=true and idPedido=" + idPedido + @"
                and idProdPedEsp not in (select idProdPed from produto_impressao pi
                inner join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                where pi.idPedido=" + idPedido + " and !coalesce(pi.cancelado,false) and ie.situacao=" +
                (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa + ")");

            objPersistence.ExecuteCommand(@"update produtos_pedido set invisivelFluxo=false, invisivelPedido=false,
                idProdPedEsp=null where idPedido=" + idPedido + @"
                and idProdPedEsp not in (select idProdPed from produto_impressao pi
                inner join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                where pi.idPedido=" + idPedido + " and !coalesce(pi.cancelado,false) and ie.situacao=" +
                (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa + ")");
        }

        /// <summary>
        /// Altera os flags de invisibilidade dos produtos de um projeto.
        /// </summary>
        public void AtualizaItensProjeto(uint idPedido)
        {
            // Garante que o pedido seja de um projeto
            if (objPersistence.ExecuteSqlQueryCount("select count(*) from pedido where idProjeto is not null and idPedido=" + idPedido) == 0)
                return;

            // Altera os flags de acordo com a conferência
            objPersistence.ExecuteCommand("update produtos_pedido pp set invisivelFluxo=true where invisivelPedido=false and (idMaterItemProj is null " +
                "or idProdPedEsp is null) and idPedido=" + idPedido);
        }

        #endregion

        #endregion

        #region Recupera para troca

        private string SqlTroca(uint idPedido, string codInterno, string descricao, string ambiente, float altura, int largura, bool selecionar, bool novo)
        {
            string sqlSomaTrocados = "select sum(qtde) from {1} pt inner join " +
                "troca_devolucao td on (pt.idTrocaDevolucao=td.idTrocaDevolucao) " +
                "where pt.idProdPed=pp.idProdPed and td.situacao<>" + (int)TrocaDevolucao.SituacaoTrocaDev.Cancelada;

            string campos = selecionar ? @"pp.*, p.descricao as descrProduto, p.codInterno, ap.ambiente,  p.IdGrupoProd, p.IdSubgrupoProd,
                CAST(pp.qtde - coalesce((" + sqlSomaTrocados + "),0) AS DECIMAL (12, 2)) as qtdeTroca" : "count(*)";

            string idsPedidos = !novo ?
                idPedido.ToString() :
                "select idPedido from pedido where idPedidoAnterior=" + idPedido;

            string sql = @"
                select " + campos + @"
                from produtos_pedido pp
                    left join produto p on (pp.idProd=p.idProd)
                    left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                where pp.idPedido in (" + idsPedidos + @")
                    AND (IdProdPedParent IS NULL OR IdProdPedParent=0)
                    and (pp.invisivel{0}=false or pp.invisivel{0} is null)
                    and pp.idProdPed not in (
                        select pt.idProdPed
                        from {1} pt
                            left join troca_devolucao td on (pt.idTrocaDevolucao=td.idTrocaDevolucao)
                        where td.idPedido=pp.idPedido
                            and td.situacao<>" + (int)TrocaDevolucao.SituacaoTrocaDev.Cancelada + @"
                            and (
                                select sum(qtde) from {1} pt2
                                    inner join troca_devolucao td2 On (pt2.idTrocaDevolucao=td2.idTrocaDevolucao)
                                where idProdPed=pt.idProdPed and td2.situacao<>" + (int)TrocaDevolucao.SituacaoTrocaDev.Cancelada + @"
                            )>=pp.qtde
                    )";

            if (!String.IsNullOrEmpty(codInterno))
                sql += " and p.codInterno like ?codInterno";

            if (!String.IsNullOrEmpty(descricao))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descricao);
                sql += " And p.idProd In (" + ids + ")";
            }

            if (!String.IsNullOrEmpty(ambiente))
                sql += " and ap.ambiente like ?ambiente";

            if (altura > 0)
                sql += " AND pp.altura=" + altura;

            if (largura > 0)
                sql += " AND pp.largura=" + largura;

            sql = String.Format(sql, PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido", !novo ? "produto_trocado" : "produto_troca_dev");

            return sql;
        }

        private GDAParameter[] GetParamsTroca(string codInterno, string descricao, string ambiente)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codInterno))
                lst.Add(new GDAParameter("?codInterno", "%" + codInterno + "%"));

            if (!String.IsNullOrEmpty(descricao))
                lst.Add(new GDAParameter("?descricao", "%" + descricao + "%"));

            if (!String.IsNullOrEmpty(ambiente))
                lst.Add(new GDAParameter("?ambiente", "%" + ambiente + "%"));

            return lst.ToArray();
        }

        public IList<ProdutosPedido> GetForTroca(uint idPedido, string codInterno, string descricao, string ambiente, float altura, int largura, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlTroca(idPedido, codInterno, descricao, ambiente, altura, largura, true, false), sortExpression, startRow, pageSize, GetParamsTroca(codInterno, descricao, ambiente));
        }

        public int GetForTrocaCount(uint idPedido, string codInterno, string descricao, string ambiente, float altura, int largura)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlTroca(idPedido, codInterno, descricao, ambiente, altura, largura, false, false), GetParamsTroca(codInterno, descricao, ambiente));
        }

        public IList<ProdutosPedido> GetForTrocaNovo(uint idPedido, string codInterno, string descricao, string ambiente, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlTroca(idPedido, codInterno, descricao, ambiente, 0, 0, true, true), sortExpression, startRow, pageSize, GetParamsTroca(codInterno, descricao, ambiente));
        }

        public int GetForTrocaNovoCount(uint idPedido, string codInterno, string descricao, string ambiente)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlTroca(idPedido, codInterno, descricao, ambiente, 0, 0, false, true), GetParamsTroca(codInterno, descricao, ambiente));
        }

        #endregion

        #region Verifica se o pedido possui produtos do grupo vidro calculado por m²

        /// <summary>
        /// Verifica se o pedido possui produtos do grupo vidro calculado por m²
        /// </summary>
        public bool PossuiVidroCalcM2(uint idPedido)
        {
            return PossuiVidroCalcM2(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido possui produtos do grupo vidro calculado por m²
        /// </summary>
        public bool PossuiVidroCalcM2(GDASession session, uint idPedido)
        {
            var sql = @"
                Select Count(*) From produtos_pedido pp
                    Inner Join produto p On (pp.idProd=p.idProd)
                    Inner Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd)
                    Left Join subgrupo_prod sg On (p.idSubgrupoProd=sg.idSubgrupoProd)
                Where idPedido=" + idPedido + @"
                    And (p.idGrupoProd=" + (int)NomeGrupoProd.MaoDeObra + @"
                    Or (p.idGrupoProd=" + (int)NomeGrupoProd.Vidro + @"
                    And coalesce(sg.tipoCalculo, g.tipoCalculo, " + (int)TipoCalculoGrupoProd.Qtd + ") in (" +
                    (int)TipoCalculoGrupoProd.M2 + "," + (int)TipoCalculoGrupoProd.M2Direto + ")))";

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Insere/Atualiza Produto de Projeto

        /// <summary>
        /// Insere/Atualiza Produto de Projeto
        /// </summary>
        public uint InsereAtualizaProdProj(GDASession sessao, Pedido pedido, uint? idAmbientePedido, ItemProjeto itemProj, bool medidasAlteradas, bool atualizarTotalPedido = true,
            bool atualizaDataEntrega = false, bool forcarAtualizacaoImagemMarcacaoPeca = false)
        {
            try
            {
                /* Chamado 51998. */
                if (atualizarTotalPedido)
                {
                    // Remove acréscimo, desconto e comissão.
                    objPersistence.ExecuteCommand(sessao, "UPDATE pedido SET IdComissionado=NULL WHERE IdPedido=" + pedido.IdPedido);
                    PedidoDAO.Instance.RemoveComissaoDescontoAcrescimo(sessao, pedido);
                }

                AmbientePedido ambiente = new AmbientePedido();
                ambiente.IdPedido = pedido.IdPedido;
                ambiente.IdItemProjeto = itemProj.IdItemProjeto;
                ambiente.Ambiente = !String.IsNullOrEmpty(itemProj.Ambiente) ? itemProj.Ambiente : "Cálculo Projeto";

                string descricao = UtilsProjeto.FormataTextoOrcamento(sessao, itemProj);
                if (!String.IsNullOrEmpty(descricao))
                    ambiente.Descricao = descricao;

                ambiente.Qtde = itemProj.Qtde > 0 ? itemProj.Qtde : 1; // Colocado para centerbox e megatemper ficar com valor correto de fechamentos na impressão

                // Se o ambiente não tiver sido informado, insere ambiente pedido, senão apenas atualiza texto
                if (idAmbientePedido == 0 || idAmbientePedido == null)
                    idAmbientePedido = AmbientePedidoDAO.Instance.Insert(sessao, ambiente);
                else
                {
                    AmbientePedido amb = AmbientePedidoDAO.Instance.GetElementByPrimaryKey(sessao, idAmbientePedido.Value);

                    ambiente.TipoAcrescimo = amb.TipoAcrescimo;
                    ambiente.TipoDesconto = amb.TipoDesconto;
                    ambiente.Desconto = amb.Desconto;
                    ambiente.Acrescimo = amb.Acrescimo;
                    ambiente.IdAmbientePedido = idAmbientePedido.Value;

                    if (String.IsNullOrEmpty(ambiente.Descricao))
                        ambiente.Descricao = amb.Descricao;

                    AmbientePedidoDAO.Instance.Update(sessao, ambiente);
                }

                // Dicionário criado para renomear imagens associadas às peças que estão sendo apagadas para que as mesmas sejam associadas às novas peças
                var dicProdPedMater = new Dictionary<uint, uint>();

                // Salva os materiais de projeto associados ao ambiente, para verificar, mais abaixo, a qual produto a imagem individual deve ser associada.
                foreach (ProdutosPedido pp in objPersistence.LoadData(sessao, "Select * From produtos_pedido Where (InvisivelPedido IS NULL OR InvisivelPedido=0) AND idAmbientePedido=" + idAmbientePedido).ToList())
                    if (pp.IdMaterItemProj != null && !dicProdPedMater.ContainsKey(pp.IdMaterItemProj.Value))
                        dicProdPedMater.Add(pp.IdMaterItemProj.Value, pp.IdProdPed);

                // Recupera os ids dos produtos de pedido que deverão ser exclusos do sistema.
                var idsProdPed = String.Join(",",
                ExecuteMultipleScalar<string>(sessao, "SELECT pp.IdProdPed FROM produtos_pedido pp WHERE (InvisivelPedido IS NULL OR InvisivelPedido=0) AND pp.IdAmbientePedido=" + idAmbientePedido).ToArray());
                // Caso nenhum id de produto de pedido seja retornado então seta o valor "0" na variável para evitar erro de execução do sql.
                if (String.IsNullOrEmpty(idsProdPed))
                    idsProdPed = "0";

                /* Chamado 15363.
                 * O projeto foi confirmado mais de uma vez e os produtos do pedido ficaram duplicados porque o comando abaixo foi
                 * apagado do código. Analisei o log de alterações e recoloquei o comando abaixo para que os produtos não sejam duplicados. */
                // Exclui produto do pedido, caso já tenha sido inserido.
                objPersistence.ExecuteCommand(sessao, @"
                    DELETE FROM produto_pedido_benef WHERE IdProdPed IN  (" + idsProdPed + @");
                    DELETE FROM produtos_pedido WHERE IdProdPed IN (" + idsProdPed + ");");

                // Insere materiais do item projeto no ambiente
                foreach (MaterialItemProjeto mip in MaterialItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProj.IdItemProjeto))
                {
                    ProdutosPedido prodPed = new ProdutosPedido();
                    prodPed.IdPedido = pedido.IdPedido;
                    prodPed.IdAmbientePedido = idAmbientePedido;
                    prodPed.IdItemProjeto = itemProj.IdItemProjeto;
                    prodPed.IdMaterItemProj = mip.IdMaterItemProj;
                    prodPed.IdProd = mip.IdProd;
                    prodPed.IdProcesso = mip.IdProcesso;
                    prodPed.IdAplicacao = mip.IdAplicacao;
                    prodPed.Redondo = mip.Redondo;
                    prodPed.Qtde = mip.Qtde;

                    /* Chamado 37486.
                     * Não alterar estes campos para evitar problemas no cálculo do pedido. */
                    prodPed.AlturaReal = mip.AlturaCalc;
                    prodPed.Altura = mip.Altura;

                    prodPed.Largura = mip.Largura;
                    prodPed.TotM = mip.TotM;
                    prodPed.TotM2Calc = mip.TotM2Calc;
                    prodPed.ValorVendido = mip.Valor;
                    prodPed.Total = mip.Total;
                    prodPed.CustoProd = mip.Custo;
                    prodPed.Espessura = mip.Espessura;
                    prodPed.AliqIcms = mip.AliqIcms;
                    prodPed.ValorIcms = mip.ValorIcms;
                    prodPed.AliqIpi = mip.AliquotaIpi;
                    prodPed.ValorIpi = mip.ValorIpi;
                    prodPed.ValorAcrescimo = mip.ValorAcrescimo;
                    prodPed.ValorDesconto = mip.ValorDesconto;
                    prodPed.PedCli = mip.PedCli;
                    prodPed.Beneficiamentos = mip.Beneficiamentos;

                    ValorBruto.Instance.Calcular(sessao, pedido, prodPed);
                    ValorUnitario.Instance.Calcular(sessao, pedido, prodPed);

                    prodPed.IdProdPed = InsertFromProjeto(sessao, prodPed);

                    // O valor do material deve ser recalculado caso a configuração AlterarValorUnitarioProduto esteja desabilitada ou
                    // caso a configuração esteja habilitada e o valor novo seja maior que o valor antigo.
                    if ((!PedidoConfig.DadosPedido.AlterarValorUnitarioProduto &&
                        Math.Round(prodPed.ValorVendido, 2, MidpointRounding.AwayFromZero) != Math.Round(mip.Valor, 2, MidpointRounding.AwayFromZero)) ||
                        (PedidoConfig.DadosPedido.AlterarValorUnitarioProduto &&
                        Math.Round(prodPed.ValorVendido, 2, MidpointRounding.AwayFromZero) > Math.Round(mip.Valor, 2, MidpointRounding.AwayFromZero)))
                    {
                        MaterialItemProjeto material = mip;

                        var idObra = PedidoConfig.DadosPedido.UsarControleNovoObra ? pedido.IdObra : null;
                        var idCliente = pedido.IdCli;
                        var tipoEntrega = pedido.TipoEntrega;

                        // Verifica qual preço deverá ser utilizado
                        ProdutoObraDAO.DadosProdutoObra dadosObra = idObra > 0
                            ? ProdutoObraDAO.Instance.IsProdutoObra(sessao, idObra.Value, mip.IdProd)
                            : null;
                        material.Valor = dadosObra != null && dadosObra.ProdutoValido
                            ? dadosObra.ValorUnitProduto
                            : ProdutoDAO.Instance.GetValorTabela(sessao, (int)mip.IdProd, tipoEntrega, idCliente, ClienteDAO.Instance.IsRevenda(idCliente), itemProj.Reposicao, 0, (int?)prodPed.IdPedido, null, null, mip.Altura);

                        MaterialItemProjetoDAO.Instance.CalcTotais(sessao, ref material, false);
                        MaterialItemProjetoDAO.Instance.UpdateBase(sessao, material);

                        ItemProjetoDAO.Instance.UpdateTotalItemProjeto(sessao, itemProj.IdItemProjeto);
                    }

                    // Altera as imagens que possam ter sido inseridas anteriormente para ficarem associadas às novas peças inseridas
                    if (prodPed.IdMaterItemProj > 0 && dicProdPedMater.ContainsKey(prodPed.IdMaterItemProj.Value))
                    {
                        AtualizarEdicaoImagemPecaArquivoMarcacao((int)dicProdPedMater[prodPed.IdMaterItemProj.Value], (int)prodPed.IdProdPed, medidasAlteradas, forcarAtualizacaoImagemMarcacaoPeca);
                    }
                }

                if (PedidoEspelhoDAO.Instance.ExisteEspelho(sessao, pedido.IdPedido))
                {
                    // Atualiza os produtos do pedido original, indicando-os como invisíveis para o fluxo
                    objPersistence.ExecuteCommand(sessao, string.Format(@"UPDATE produtos_pedido SET InvisivelFluxo=1 WHERE IdPedido={0} AND IdAmbientePedido={1}
                        AND (InvisivelPedido IS NULL OR InvisivelPedido=0)", pedido.IdPedido, idAmbientePedido));
                }

                // Verifica se o itemProjeto possui referência do idPedido (Ocorreu de não estar associado)
                if (itemProj.IdPedido == null)
                    objPersistence.ExecuteCommand(sessao, "Update item_projeto Set idPedido=" + pedido.IdPedido + " Where idItemProjeto=" + itemProj.IdItemProjeto);

                /* Chamado 51998. */
                if (atualizarTotalPedido)
                {
                    // Aplica acréscimo, desconto e comissão
                    PedidoDAO.Instance.AplicaComissaoDescontoAcrescimo(sessao, pedido, Geral.ManterDescontoAdministrador);

                    // Aplica acréscimo e desconto no ambiente
                    if (OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento
                        && (ambiente.Acrescimo > 0 || ambiente.Desconto > 0))
                    {
                        var produtosPedido = GetByAmbiente(sessao, ambiente.IdAmbientePedido);

                        if (ambiente.Acrescimo > 0)
                        {
                            AmbientePedidoDAO.Instance.AplicarAcrescimo(
                                sessao,
                                pedido,
                                ambiente.IdAmbientePedido,
                                ambiente.TipoAcrescimo,
                                ambiente.Acrescimo,
                                produtosPedido
                            );
                        }

                        if (ambiente.Desconto > 0)
                        {
                            AmbientePedidoDAO.Instance.AplicarDesconto(
                                sessao,
                                pedido,
                                ambiente.IdAmbientePedido,
                                ambiente.TipoDesconto,
                                ambiente.Desconto,
                                produtosPedido
                            );
                        }

                        AmbientePedidoDAO.Instance.FinalizarAplicacaoAcrescimoDesconto(sessao, pedido, produtosPedido, true);
                    }

                    // Atualiza o total do pedido
                    PedidoDAO.Instance.UpdateTotalPedido(sessao, pedido);
                }

                if (atualizaDataEntrega && pedido.DataEntregaSistema != null && !(pedido.DataEntregaSistema.Value.Date != pedido.DataEntrega.Value.Date && Config.PossuiPermissao(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega)))
                {
                    // Atualiza a data de entrega do pedido para considerar o número de dias mínimo de entrega do subgrupo ao informar o produto.
                    bool enviarMensagem;
                    PedidoDAO.Instance.RecalcularEAtualizarDataEntregaPedido(sessao, pedido.IdPedido, null, out enviarMensagem, true);
                }

                return idAmbientePedido.GetValueOrDefault();
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
            var idsProdPed = ExecuteMultipleScalar<int?>(session, string.Format("SELECT IdProdPed FROM produtos_pedido WHERE IdItemProjeto={0}", idItemProjeto));

            if (idsProdPed == null || idsProdPed.Count == 0)
                return;

            foreach (var idProdPed in idsProdPed)
                if (idProdPed > 0)
                    AtualizarEdicaoImagemPecaArquivoMarcacao(idProdPed.Value, null, true, false);
        }

        /// <summary>
        /// Remove ou mantém a edição das imagens e de arquivos de marcação, de acordo com a configuração ManterImagensEditadasAoConfirmarProjeto e o parâmetro medidasAlteradas.
        /// </summary>
        public void AtualizarEdicaoImagemPecaArquivoMarcacao(int idProdPedAtual, int? idProdPedNovo, bool medidasAlteradas, bool forcarAtualizacaoImagemMarcacaoPeca)
        {
            var atualizarImagemMarcacao = idProdPedNovo > 0 && (forcarAtualizacaoImagemMarcacaoPeca || (!medidasAlteradas && ProjetoConfig.ManterImagensEditadasAoConfirmarProjeto));
            var arquivosImagem = Directory.GetFiles(Utils.GetPecaComercialPath, $"{ idProdPedAtual.ToString().PadLeft(10, '0') }_*");
            var caminhoDxf = $"{ PCPConfig.CaminhoSalvarCadProject(false) }{ idProdPedAtual }.dxf";
            var caminhoSvg = $"{ PCPConfig.CaminhoSalvarCadProject(false) }{ idProdPedAtual }.svg";

            foreach (var arquivoImagem in arquivosImagem)
            {
                if (idProdPedNovo > 0 && (forcarAtualizacaoImagemMarcacaoPeca ||
                    (ProjetoConfig.ManterImagensEditadasAoConfirmarProjeto && (!medidasAlteradas || ProjetoConfig.GerarPecasComMedidasIncoerentesDaImagemEditada))))
                {
                    File.Copy(arquivoImagem, arquivoImagem.Replace(idProdPedAtual.ToString().PadLeft(10, '0'), idProdPedNovo.ToString().PadLeft(10, '0')));
                }

                File.Delete(arquivoImagem);
            }

            if (File.Exists(caminhoDxf))
            {
                if (atualizarImagemMarcacao)
                {
                    File.Move(caminhoDxf, string.Format("{0}{1}.dxf", PCPConfig.CaminhoSalvarCadProject(false), idProdPedNovo));
                }
                else
                {
                    File.Delete(caminhoDxf);
                }
            }

            if (File.Exists(caminhoSvg))
            {
                if (atualizarImagemMarcacao)
                {
                    File.Move(caminhoSvg, string.Format("{0}{1}.svg", PCPConfig.CaminhoSalvarCadProject(false), idProdPedNovo));
                }
                else
                {
                    File.Delete(caminhoSvg);
                }
            }
        }

        #endregion

        #region Lista de peças que ainda não deram saída do estoque

        private string SqlPecasSemSaida(uint idCliente, string nomeCliente, uint idPedido, string dataIni, string dataFim,
            uint idLoja, bool selecionar)
        {
            string criterio = "";
            string campos = selecionar ? @"pp.*, p.codInterno, p.descricao as descrProduto, ea.codInterno as codAplicacao,
                ep.codInterno as codProcesso, ped.idCli as idCliente, c.nome as nomeCliente, '$$$' as criterio" : "count(distinct pp.idProdPed)";

            string sql = "select " + campos + @"
                from produtos_pedido pp
                    left join
                    (select p1.idProd, p1.codInterno, p1.descricao, p1.idGrupoProd from produto p1)
                    as p ON (pp.idProd = p.idProd)

                    left join
                    (select ea1.idAplicacao, ea1.codinterno from etiqueta_aplicacao ea1)
                    as ea ON (pp.idAplicacao=ea.idAplicacao)

                    left join
                    (select ep1.idProcesso, ep1.codinterno from etiqueta_processo ep1)
                    as ep ON (pp.idProcesso=ep.idProcesso)

                    left join
                    (select ped1.idpedido, ped1.situacao, ped1.idCli, ped1.dataConf, ped1.TipoPedido from pedido ped1)
                    as ped ON (pp.idpedido=ped.idpedido)

                    left join
                    (select cli1.id_cli, cli1.nome from cliente cli1)
                    as c ON (ped.idCli=c.id_cli) " +

                    // Empresa de confirmação não pode fazer left join com produtos_liberar_pedido nem liberarpedido, chamado 9588.
                    (PedidoConfig.LiberarPedido ?
                    @" left join produtos_liberar_pedido plp on (pp.idProdPed=plp.idProdPed)
                    left join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido)" : "") +

                @"where pp.qtde>pp.qtdSaida
                    and ped.situacao=" + (int)Pedido.SituacaoPedido.Confirmado +
                    /* Chamado 14932.
                     * Pedidos de mão de obra não devem ser exibidos nesta tela, pois, não efetuam a baixa no estoque. */
                    " AND ped.TipoPedido<>" + (int)Pedido.TipoPedidoEnum.MaoDeObra;

            sql += (PCPConfig.ControlarProducao ? " and p.idGrupoProd<>" + (int)Glass.Data.Model.NomeGrupoProd.Vidro : "");

            if (!PedidoConfig.LiberarPedido)
                sql += " and coalesce(pp.invisivelPedido,false)=false";
            else
                sql += " and lp.situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado;

            if (idCliente > 0)
            {
                sql += " and ped.idCli=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            if (idPedido > 0)
            {
                sql += " and ped.idPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
            }

            string criterioConf = PedidoConfig.LiberarPedido ? "Lib" : "Conf";
            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += !PedidoConfig.LiberarPedido ? " and ped.dataConf>=?dataIni" : " and lp.dataLiberacao>=?dataIni";
                criterio += "Data " + criterioConf + ". Início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += !PedidoConfig.LiberarPedido ? " and ped.dataConf<=?dataFim" : " and lp.dataLiberacao<=?dataFim";
                criterio += "Data " + criterioConf + ". Fim: " + dataFim + "    ";
            }

            if (idLoja > 0)
            {
                sql += " AND ped.idLoja=" + idLoja;
                criterio += " Loja: " + LojaDAO.Instance.GetNome(idLoja);
            }

            if (selecionar)
                sql += " group by pp.idProdPed order by pp.idpedido asc";

            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParamsSemSaida(string nomeCliente, string dataIni, string dataFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCliente))
                lst.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lst.ToArray();
        }

        public List<ProdutosPedido> GetListPecasSemSaida(uint idCliente, string nomeCliente, uint idPedido, string dataIni,
            string dataFim, uint idLoja, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = null;

            return objPersistence.LoadDataWithSortExpression(SqlPecasSemSaida(idCliente, nomeCliente, idPedido, dataIni, dataFim, idLoja, true),
                new InfoSortExpression(sortExpression), new InfoPaging(startRow, pageSize), GetParamsSemSaida(nomeCliente, dataIni, dataFim));
        }

        public int GetCountPecasSemSaida(uint idCliente, string nomeCliente, uint idPedido, string dataIni, string dataFim, uint idLoja)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlPecasSemSaida(idCliente, nomeCliente, idPedido, dataIni, dataFim, idLoja, false),
                GetParamsSemSaida(nomeCliente, dataIni, dataFim));
        }

        public IList<ProdutosPedido> GetForRptPecasSemSaida(uint idCliente, string nomeCliente, uint idPedido, string dataIni, string dataFim, uint idLoja)
        {
            return objPersistence.LoadData(SqlPecasSemSaida(idCliente, nomeCliente, idPedido, dataIni, dataFim, idLoja, true),
                GetParamsSemSaida(nomeCliente, dataIni, dataFim)).ToList();
        }

        #endregion

        #region Retorna o total de m² de um produto por obra

        /// <summary>
        /// Retorna o total de m² de um produto por obra.
        /// </summary>
        public float TotalMedidasObra(uint idObra, string codInterno, uint? idPedidoPcp)
        {
            return TotalMedidasObra(null, idObra, codInterno, idPedidoPcp);
        }

        /// <summary>
        /// Retorna o total de m² de um produto por obra.
        /// </summary>
        public float TotalMedidasObra(GDASession sessao, uint idObra, string codInterno, uint? idPedidoPcp)
        {
            string sql = $@"Select Coalesce(Sum(pp.totM2Calc),0) From produtos_pedido pp
                Inner Join produto p On (pp.idProd=p.idProd) Where p.codInterno=?codInterno And
                pp.idPedido In (Select * From (Select idPedido From pedido Where idObra={ idObra } And situacao<>{ (int)Pedido.SituacaoPedido.Cancelado }) As temp) And ";

            if (idPedidoPcp > 0)
                sql += $@"((pp.idPedido<>{ idPedidoPcp } And If((Select Count(*) From pedido_espelho Where idPedido=pp.idPedido)>0,
                    pp.invisivelFluxo Is Null Or pp.invisivelFluxo=False, pp.invisivelPedido Is Null Or pp.invisivelPedido=False))
                    Or (pp.invisivelFluxo Is Null Or pp.invisivelFluxo=false)) And COALESCE(pp.IdProdPedParent,0) = 0";
            else
                sql += "(pp.invisivelPedido Is Null Or pp.invisivelPedido=False) And COALESCE(pp.IdProdPedParent,0) = 0";

            return float.Parse(objPersistence.ExecuteScalar(sessao, sql, new GDAParameter("?codInterno", codInterno)).ToString());
        }

        #endregion

        #region Desconto para administrador

        private string SqlDescontoAdmin(uint idProdPed, uint idPedido, uint idAmbiente, bool visiveis, bool selecionar)
        {
            return SqlDescontoAdmin(null, idProdPed, idPedido, idAmbiente, visiveis, selecionar);
        }

        private string SqlDescontoAdmin(GDASession session, uint idProdPed, uint idPedido, uint idAmbiente, bool visiveis, bool selecionar)
        {
            return SqlDescontoAdmin(session, idProdPed, idPedido, idAmbiente, visiveis, false, false, 0, selecionar);
        }

        private string SqlDescontoAdmin(GDASession session, uint idProdPed, uint idPedido, uint idAmbiente, bool visiveis,
            bool ignorarFiltroProdComp, bool prodComp, uint idProdPedParent, bool selecionar)
        {
            bool isPcp = PedidoEspelhoDAO.Instance.ExisteEspelho(session, idPedido);

            string ambiente = PedidoConfig.LiberarPedido ? "if('{0}'='Fluxo', ape.ambiente, ap.ambiente)" : "ap.ambiente";
            string campos = selecionar ? @"pp.*, p.descricao as descrProduto, " + ambiente + " as ambiente" : "count(*)";

            string sql = "select " + campos + @"
                from produtos_pedido pp
                    left join produto p on (pp.idProd=p.idProd)
                    left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                    left join produtos_pedido_espelho ppe on (pp.idProdPedEsp=ppe.idProdPed)
                    " + (PedidoConfig.LiberarPedido ? @"
                        left join ambiente_pedido_espelho ape on (ppe.idAmbientePedido=ape.idAmbientePedido)" :
                    "") + @"
                where 1";

            if (idProdPed > 0)
                sql += " and pp.idProdPed=" + idProdPed;

            if (idPedido > 0)
                sql += " and pp.idPedido=" + idPedido;

            if (visiveis)
                sql += " and coalesce(pp.invisivel{0}, false)=false and pp.qtde-pp.qtdSaida > 0";
            else
                sql += " and (ppe.qtdeInvisivel > 0 or (pp.qtdeInvisivel > 0 and ppe.idProdPed is null))";

            if (idAmbiente > 0)
                sql += " and pp.idAmbientePedido=" + idAmbiente;

            if (!ignorarFiltroProdComp)
                sql += " AND pp.IdProdPedParent IS " + (prodComp ? "NOT NULL" : "NULL");

            if (idProdPedParent > 0)
                sql += " AND pp.IdProdPedParent = " + idProdPedParent;

            return String.Format(sql, PedidoConfig.LiberarPedido && isPcp ? "Fluxo" : "Pedido");
        }

        /// <summary>
        /// Retorna a lista de produtos (removidos ou não) para a tela de desconto administrador.
        /// </summary>
        public IList<ProdutosPedido> GetDescontoAdminList(uint idPedido, uint idAmbiente, bool visiveis, bool ignorarFiltroProdComp, bool prodComp, uint idProdPedParent,
            string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlDescontoAdmin(null, 0, idPedido, idAmbiente, visiveis, ignorarFiltroProdComp, prodComp, idProdPedParent, true), sortExpression, startRow, pageSize);
        }

        public int GetDescontoAdminCount(uint idPedido, uint idAmbiente, bool visiveis, bool ignorarFiltroProdComp, bool prodComp, uint idProdPedParent)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlDescontoAdmin(null, 0, idPedido, idAmbiente, visiveis, ignorarFiltroProdComp, prodComp, idProdPedParent, false));
        }

        private static readonly object _removerProdutoDescontoAdminLock = new object();

        /// <summary>
        /// Remove um produto do pedido.
        /// </summary>
        public void RemoverProdutoDescontoAdmin(ref uint idProdPed, float qtdeRemover)
        {
            lock (_removerProdutoDescontoAdminLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        // Verifica se o produto já foi instalado
                        if (objPersistence.ExecuteSqlQueryCount(transaction, @"
                            Select count(*) from produtos_instalacao pi
                                Inner Join instalacao i On (pi.idInstalacao=i.idInstalacao)
                            Where pi.idProdPed=" + idProdPed + " And i.Situacao<>" + (int)Instalacao.SituacaoInst.Cancelada) > 0)
                            throw new Exception("Esse produto não pode ser removido do pedido porque tem uma instalação agendada ou já feita. Para prosseguir, cancele a instalação.");

                        var idProdPedEsp = ObterIdProdPedEsp(transaction, idProdPed);
                        if (idProdPedEsp > 0 && ProdutoImpressaoDAO.Instance.EstaImpressa(transaction, idProdPedEsp.Value))
                            throw new Exception("Esse produto possui etiquetas impressas. Cancele as impressões para prosseguir.");

                        //Verifica se o pedido do produto esta vinculad a uma OC
                        if (PedidoOrdemCargaDAO.Instance.PedidoTemOC(transaction, ObtemIdPedido(transaction, idProdPed)))
                            throw new Exception("O pedido esta vinculado a uma OC. Para Proseguir, remova-o da OC.");

                        if (objPersistence.ExecuteSqlQueryCount(transaction, @"
                            SELECT COUNT(*) FROM volume_produtos_pedido
                            WHERE idProdPed=" + idProdPed) > 0)
                            throw new Exception("Esse produto não pode ser removido do pedido porque esta vinculado a um volume. Para Proseguir, remova-o do volume.");

                        var prodPed = objPersistence.LoadOneData(transaction, SqlDescontoAdmin(transaction, idProdPed, ObtemIdPedido(transaction, idProdPed), 0, true, true, true, 0, true));
                        var isPcp = PedidoEspelhoDAO.Instance.ExisteEspelho(transaction, prodPed.IdPedido) && prodPed.IdProdPedEsp > 0;
                        var ped = PedidoDAO.Instance.GetElementByPrimaryKey(transaction, prodPed.IdPedido);

                        /* Chamado 54914. */
                        if (TemFilhoComposicao(transaction, (int)idProdPed) || prodPed.IdProdPedParent > 0)
                            throw new Exception("Não é possível remover produto de composição, por esta tela. Para removê-lo, reabra o pedido no comercial.");

                        #region Valida o caixa diário

                        if (!PedidoConfig.LiberarPedido && Geral.ControleCaixaDiario &&
                            !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(transaction, ped.IdLoja))
                            throw new Exception("O caixa não foi fechado no dia anterior.");

                        #endregion

                        #region Verifica se o produto está liberado

                        if (PedidoConfig.LiberarPedido && (ped.Situacao == Pedido.SituacaoPedido.Confirmado || (isPcp &&
                            LiberarPedidoDAO.Instance.IsProdutoPedidoLiberado(transaction, null, prodPed.IdProdPedEsp.Value, 0))))
                        {
                            throw new Exception("O pedido (ou o produto) já foi liberado. Cancele a liberação antes de continuar.");
                        }

                        #endregion

                        var rem = new GDAParameter("?rem", qtdeRemover);

                        // Esconde a quantidade desejada do produto do pedido
                        objPersistence.ExecuteCommand(transaction, string.Format(@"update produtos_pedido set qtde=greatest(qtde-?rem, 0),
                        qtdeInvisivel=coalesce(qtdeInvisivel,0)+?rem, invisivelAdmin=(qtde=0),
                        invisivel{0}=(COALESCE(invisivel{0},0) or COALESCE(invisivelAdmin,0)) where idProdPed=" + idProdPed,
                            PedidoConfig.LiberarPedido && isPcp ? "Fluxo" : "Pedido"), rem);

                        if (isPcp)
                        {
                            // Esconde a quantidade desejada do produto do PCP
                            objPersistence.ExecuteCommand(transaction, @"update produtos_pedido_espelho set qtde=greatest(qtde-?rem, 0),
                            qtdeInvisivel=coalesce(qtdeInvisivel,0)+?rem, invisivelAdmin=(qtde=0),
                            invisivelFluxo=(COALESCE(invisivelFluxo,0) or COALESCE(invisivelAdmin,0)) where idProdPed=" + prodPed.IdProdPedEsp.Value, rem);

                            if (!ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<bool>(transaction, "invisivelAdmin", "idProdPed=" +
                                prodPed.IdProdPedEsp.Value))
                            {
                                // Recalcula o total do produto com base na nova quantidade
                                ProdutosPedidoEspelhoDAO.Instance.Update(transaction,
                                    ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(transaction, prodPed.IdProdPedEsp.Value));

                                // Recupera o novo ID do clone
                                idProdPed = ObtemValorCampo<uint>(transaction, "idProdPed", "idProdPedEsp=" + prodPed.IdProdPedEsp.Value);
                            }
                            /* Chamado 54060. */
                            else
                                // Recalcula o produto do pedido original.
                                Update(transaction, GetElementByPrimaryKey(transaction, prodPed.IdProdPed));

                            var itemEtiqueta = string.Format("cast(substr({0}, 1, instr({0}, '/') - 1) as signed)",
                                "substr(numEtiqueta, instr(numEtiqueta, '.') + 1)");

                            // Remove as etiquetas da produção
                            objPersistence.ExecuteCommand(transaction, @"
                            update produto_pedido_producao ppp
                                inner join produtos_pedido_espelho ppe on (ppp.idProdPed=ppe.idProdPed)
                            set ppp.canceladoAdmin=true, ppp.numEtiquetaCanc=ppp.numEtiqueta,
                                ppp.numEtiqueta=null, ppp.situacao=" + (!ped.MaoDeObra ?
                                    (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda :
                                    (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra) + @"
                            where ppp.idProdPed=" + prodPed.IdProdPedEsp.Value + @"
                                and " + itemEtiqueta + " > ppe.qtde");

                            // Altera a situação de produção do pedido, se necessário
                            PedidoDAO.Instance.AtualizaSituacaoProducao(transaction, prodPed.IdPedido, null, DateTime.Now);

                            PedidoEspelhoDAO.Instance.UpdateTotalPedido(transaction, prodPed.IdPedido);
                        }
                        else
                        {
                            // Se não for PCP, recalcula o produto do pedido original
                            Instance.Update(transaction,
                                Instance.GetElementByPrimaryKey(transaction, prodPed.IdProdPed));
                        }

                        #region Gera o crédito para o cliente

                        if (!PedidoConfig.LiberarPedido && PedidoDAO.Instance.IsPedidoConfirmado(transaction, prodPed.IdPedido))
                        {
                            var creditoGerado = false;
                            uint idCxDiario = 0, idCxGeral = 0;

                            var idConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado);
                            var valor =
                                (prodPed.Total + prodPed.ValorBenef) / (decimal)(prodPed.Qtde + prodPed.QtdeInvisivel) * (decimal)qtdeRemover;

                            try
                            {
                                if (Geral.ControleCaixaDiario)
                                    idCxDiario = CaixaDiarioDAO.Instance.MovCxPedido(transaction, ped.IdLoja, ped.IdCli, ped.IdPedido, 1,
                                        valor, 0, idConta, null, false);
                                else
                                    idCxGeral = CaixaGeralDAO.Instance.MovCxPedido(transaction, ped.IdPedido, ped.IdCli, idConta, 1, valor, 0,
                                        null, false, null, null);

                                ClienteDAO.Instance.CreditaCredito(transaction, ped.IdCli, valor);
                                creditoGerado = true;
                            }
                            catch (Exception ex)
                            {
                                if (idCxDiario > 0)
                                    CaixaDiarioDAO.Instance.DeleteByPrimaryKey(transaction, idCxDiario);

                                if (idCxGeral > 0)
                                    CaixaGeralDAO.Instance.DeleteByPrimaryKey(transaction, idCxGeral);

                                if (creditoGerado)
                                    ClienteDAO.Instance.DebitaCredito(transaction, ped.IdCli, valor);

                                throw ex;
                            }
                        }

                        #endregion

                        #region Atualiza o estoque

                        // Tira produtos da reserva ou estorna se já tiver dado baixa
                        var m2Saida = prodPed.TotM / prodPed.Qtde * qtdeRemover;
                        var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)prodPed.IdProd);
                        var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 || tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;

                        var qtdSaida = qtdeRemover;

                        if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                            tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6)
                        {
                            qtdSaida *= prodPed.Altura;
                        }

                        //ProdutoLojaDAO.Instance.CreditaEstoque(prodPed.IdProd, ped.IdLoja, qtdSaida, m2 ? m2Saida : 0);
                        if (!PedidoDAO.Instance.IsProducao(transaction, prodPed.IdPedido))
                            ProdutoLojaDAO.Instance.TirarReserva(transaction, (int)ped.IdLoja,
                                new Dictionary<int, float> { { (int)prodPed.IdProd, m2 ? m2Saida : qtdSaida } }, null, null, null, null,
                                null, null, (int)prodPed.IdProdPed, "ProdutosPedidoDAO - RemoverProdutoDescontoAdmin");

                        #endregion

                        // Atualiza os valores dos pedidos (original e PCP)
                        PedidoDAO.Instance.UpdateDesconto(transaction, ped, false);

                        // Salva no log
                        ped.DiferencaProdutoAdmin = qtdeRemover + " x " + prodPed.DescrProduto + (!string.IsNullOrEmpty(prodPed.Ambiente) ?
                            " (ambiente: " + prodPed.Ambiente + ")" : "") + " removido" + (qtdeRemover > 1 ? "s" : "");

                        LogAlteracaoDAO.Instance.LogPedido(transaction, ped, PedidoDAO.Instance.GetElementByPrimaryKey(transaction, ped.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Novo);

                        // Atualiza o desconto na comissão
                        DebitoComissaoDAO.Instance.AtualizaDebitoPedido(transaction, prodPed.IdPedido);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException("RemoverProdutoDescontoAdmin - IdProdPed: " + idProdPed, ex);

                        throw ex;
                    }
                }
            }
        }

        private static readonly object _restaurarProdutoDescontoAdminLock = new object();

        /// <summary>
        /// Restaura um produto removido.
        /// </summary>
        public void RestaurarProdutoDescontoAdmin(ref uint idProdPed, float qtdeRestaurar)
        {
            lock (_restaurarProdutoDescontoAdminLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var prodPed = objPersistence.LoadOneData(transaction, SqlDescontoAdmin(transaction, idProdPed,
                            ObtemIdPedido(transaction, idProdPed), 0, false, true));
                        var isPcp = PedidoEspelhoDAO.Instance.ExisteEspelho(transaction, prodPed.IdPedido) && prodPed.IdProdPedEsp > 0;
                        var ped = PedidoDAO.Instance.GetElementByPrimaryKey(transaction, prodPed.IdPedido);

                        //Verifica se o pedido do produto esta vinculad a uma OC
                        if (PedidoOrdemCargaDAO.Instance.PedidoTemOC(transaction, Instance.ObtemIdPedido(transaction, idProdPed)))
                            throw new Exception("O pedido esta vinculado a uma OC. Para Proseguir, remova-o da OC.");

                        #region Valida o caixa diário

                        if (!PedidoConfig.LiberarPedido && Geral.ControleCaixaDiario &&
                            !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(transaction, ped.IdLoja))
                            throw new Exception("O caixa não foi fechado no dia anterior.");

                        #endregion

                        #region Verifica se o produto está liberado

                        if (PedidoConfig.LiberarPedido && (ped.Situacao == Pedido.SituacaoPedido.Confirmado || (isPcp &&
                            LiberarPedidoDAO.Instance.IsProdutoPedidoLiberado(transaction, null, prodPed.IdProdPedEsp.Value, 0))))
                        {
                            throw new Exception("O pedido (ou o produto) já foi liberado. Cancele a liberação antes de continuar.");
                        }

                        #endregion

                        var rest = new GDAParameter("?rest", qtdeRestaurar);

                        // Restaura a quantidade desejada do produto do pedido
                        objPersistence.ExecuteCommand(transaction, string.Format(@"update produtos_pedido set qtde=qtde+?rest,
                        qtdeInvisivel=coalesce(qtdeInvisivel,0)-?rest, invisivel{0}=false,
                        invisivelAdmin=false where idProdPed=" + idProdPed,
                            PedidoConfig.LiberarPedido && isPcp ? "Fluxo" : "Pedido"), rest);

                        if (isPcp)
                        {
                            // Restaura a quantidade desejada do produto do pedido
                            objPersistence.ExecuteCommand(transaction, @"update produtos_pedido_espelho set qtde=qtde+?rest,
                            qtdeInvisivel=coalesce(qtdeInvisivel,0)-?rest, invisivelFluxo=false,
                            invisivelAdmin=false where idProdPed=" + prodPed.IdProdPedEsp.Value, rest);

                            // Recalcula o total do produto com base na nova quantidade
                            ProdutosPedidoEspelhoDAO.Instance.Update(transaction,
                                ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(transaction, prodPed.IdProdPedEsp.Value));

                            // Recupera o novo ID do clone
                            idProdPed = ObtemValorCampo<uint>(transaction, "idProdPed", "idProdPedEsp=" + prodPed.IdProdPedEsp.Value);

                            var itemEtiqueta = string.Format("cast(substr({0}, 1, instr({0}, '/') - 1) as signed)",
                                "substr(numEtiquetaCanc, instr(numEtiquetaCanc, '.') + 1)");

                            // Restaura as etiquetas na produção
                            objPersistence.ExecuteCommand(transaction, @"
                            update produto_pedido_producao ppp
                                inner join produtos_pedido_espelho ppe on (ppp.idProdPed=ppe.idProdPed)
                            set ppp.canceladoAdmin=false, ppp.numEtiqueta=ppp.numEtiquetaCanc,
                                ppp.numEtiquetaCanc=null, ppp.situacao=if(ppp.dataPerda is null, " +
                                    (int)ProdutoPedidoProducao.SituacaoEnum.Producao + "," + (int)ProdutoPedidoProducao.SituacaoEnum.Perda + @")
                            where ppp.canceladoAdmin and ppp.idProdPed=" + prodPed.IdProdPedEsp.Value + @"
                                and " + itemEtiqueta + " <= ppe.qtde");

                            PedidoEspelhoDAO.Instance.UpdateTotalPedido(transaction, prodPed.IdPedido);
                        }
                        else
                        {
                            // Se não for PCP, recalcula o produto do pedido original
                            Instance.Update(transaction,
                                Instance.GetElementByPrimaryKey(transaction, prodPed.IdProdPed));
                        }

                        #region Debita o crédito do cliente

                        if (!PedidoConfig.LiberarPedido && PedidoDAO.Instance.IsPedidoConfirmado(transaction, prodPed.IdPedido))
                        {
                            var creditoDebitado = false;
                            uint idCxDiario = 0, idCxGeral = 0;

                            var idConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoCreditoVendaGerado);
                            var valor = (prodPed.Total + prodPed.ValorBenef) / (decimal)(prodPed.Qtde + prodPed.QtdeInvisivel) * (decimal)qtdeRestaurar;

                            try
                            {
                                if (Geral.ControleCaixaDiario)
                                    idCxDiario = CaixaDiarioDAO.Instance.MovCxPedido(transaction, ped.IdLoja, ped.IdCli, ped.IdPedido, 2, valor, 0, idConta, null, false);
                                else
                                    idCxGeral = CaixaGeralDAO.Instance.MovCxPedido(transaction, ped.IdPedido, ped.IdCli, idConta, 2, valor, 0, null, false, null, null);

                                ClienteDAO.Instance.DebitaCredito(transaction, ped.IdCli, valor);
                                creditoDebitado = true;
                            }
                            catch (Exception ex)
                            {
                                if (idCxDiario > 0)
                                    CaixaDiarioDAO.Instance.DeleteByPrimaryKey(transaction, idCxDiario);

                                if (idCxGeral > 0)
                                    CaixaGeralDAO.Instance.DeleteByPrimaryKey(transaction, idCxGeral);

                                if (creditoDebitado)
                                    ClienteDAO.Instance.CreditaCredito(transaction, ped.IdCli, valor);

                                throw ex;
                            }
                        }

                        #endregion

                        #region Atualiza o estoque

                        // Tira produtos da reserva ou estorna se já tiver dado baixa
                        var m2Entrada = prodPed.TotM / (prodPed.Qtde > 0 ? prodPed.Qtde : 1) * qtdeRestaurar;
                        var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)prodPed.IdProd);
                        var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 || tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;

                        var qtdEntrada = qtdeRestaurar;

                        if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                            tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6)
                        {
                            qtdEntrada *= prodPed.Altura;
                        }

                        //ProdutoLojaDAO.Instance.BaixaEstoque(prodPed.IdProd, ped.IdLoja, qtdEntrada, m2 ? m2Entrada : 0);
                        if (!ped.Producao)
                            ProdutoLojaDAO.Instance.ColocarReserva(transaction, (int)ped.IdLoja,
                                new Dictionary<int, float> { { (int)prodPed.IdProd, m2 ? m2Entrada : qtdEntrada } }, null, null, null,
                                null, null, null, (int)prodPed.IdProdPed, "PrdutosPedidoDAO - RestaurarProdutoDescontoAdmin");

                        #endregion

                        // Atualiza os valores dos pedidos (original e PCP)
                        PedidoDAO.Instance.UpdateDesconto(transaction, ped, false);

                        // Salva no log
                        ped.DiferencaProdutoAdmin = qtdeRestaurar + " x " + prodPed.DescrProduto + (!string.IsNullOrEmpty(prodPed.Ambiente) ?
                            " (ambiente: " + prodPed.Ambiente + ")" : "") + " restaurado" + (qtdeRestaurar > 1 ? "s" : "");

                        LogAlteracaoDAO.Instance.LogPedido(transaction, ped, PedidoDAO.Instance.GetElementByPrimaryKey(transaction, ped.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Novo);

                        // Atualiza o desconto na comissão
                        DebitoComissaoDAO.Instance.AtualizaDebitoPedido(transaction, prodPed.IdPedido);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException("RemoverProdutoDescontoAdmin - IdProdPed: " + idProdPed, ex);

                        throw ex;
                    }
                }
            }
        }

        #endregion

        #region Estoque de vidros

        /// <summary>
        /// Busca para popup de estoque de vidros.
        /// Produtos em reserva/liberação.
        /// </summary>
        public IList<ProdutosPedido> GetForEstoqueVidrosRL(uint idProd)
        {
            string sql = @"select pp.*, c.nome as nomeCliente, ped.situacao as situacaoPedido, ap.ambiente
                from produtos_pedido pp
                    left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                    left join pedido ped on (pp.idPedido=ped.idPedido)
                    left join cliente c on (ped.idCli=c.id_Cli)
                where pp.idProd=" + idProd + @"
                    and ped.tipoPedido in (" + (int)Pedido.TipoPedidoEnum.Venda + "," + (int)Pedido.TipoPedidoEnum.Revenda + @")
                    and ped.situacao in (" + (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente + "," +
                        (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + @")";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Busca para popup de estoque de vidros.
        /// Produtos em produção.
        /// </summary>
        public IList<ProdutosPedido> GetForEstoqueVidrosP(uint idProd)
        {
            if (idProd == 0)
                return new List<ProdutosPedido>();

            string sql = @"
                Select pp.*, ped.situacao as situacaoPedido, ap.ambiente, ped.dataEntrega as dataEntregaPedido,
                    pped.qtdeProduzindo, pped.totMProduzindo
                from produtos_pedido pp
                    inner join pedido ped on (pp.idPedido=ped.idPedido)
                    left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                    " + ProdutoDAO.Instance.SqlPendenteProducao("pp", null, "ped") + @"
                where pp.idProd=" + idProd + @"
                    and ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @" and ped.situacao in (" +
                        (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente + "," +
                        (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + @")
                    and pped.qtdeProduzindo > 0
                group by ped.idPedido";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Calcula o valor vendido do produto pai dos produtos da composição
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPedParent"></param>
        /// <returns></returns>
        public decimal CalcValorVendidoProdPai(GDASession sessao, uint idProdPedParent)
        {
            var totalComposicao = ExecuteScalar<decimal>(sessao, "SELECT SUM(total) FROM produtos_pedido WHERE IdProdPedParent = " + idProdPedParent);
            totalComposicao += ExecuteScalar<decimal>(sessao, @"
                SELECT SUM(pb.valor)
                FROM produto_pedido_benef pb
                    INNER JOIN produtos_pedido pp ON (pb.IdProdPed = pp.IdProdPed)
                WHERE pp.IdProdPedParent = " + idProdPedParent);

            var totM = ObtemTotM(sessao, idProdPedParent);
            var qtde = ObtemQtde(sessao, idProdPedParent);

            if (totalComposicao == 0 || totM == 0)
                return 0;

            return (totalComposicao * (decimal)(qtde)) / (decimal)totM;
        }

        #endregion

        #region Obtém valores de campos específicos

        /// <summary>
        /// Obtém valores de campos específicos
        /// </summary>
        public uint ObtemIdPedido(uint idProdPed)
        {
            return ObtemIdPedido(null, idProdPed);
        }

        /// <summary>
        /// Obtém valores de campos específicos
        /// </summary>
        public uint ObtemIdPedido(GDASession sessao, uint idProdPed)
        {
            string sql = "Select idPedido From produtos_pedido Where idProdPed=" + idProdPed;

            return ExecuteScalar<uint>(sessao, sql);
        }

        public uint? ObterIdProdPedEsp(uint idProdPed)
        {
            return ObterIdProdPedEsp(null, idProdPed);
        }

        public uint? ObterIdProdPedEsp(GDASession session, uint idProdPed)
        {
            string sql = "Select idProdPedEsp From produtos_pedido Where idProdPed=" + idProdPed;

            object retorno = objPersistence.ExecuteScalar(session, sql);

            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? (uint?)Glass.Conversoes.StrParaUint(retorno.ToString()) : null;
        }

        public int? ObterIdProdPed(GDASession session, int idProdPedEsp)
        {
            var sql = string.Format("SELECT IdProdPed FROM produtos_pedido WHERE IdProdPedEsp={0}", idProdPedEsp);

            object retorno = objPersistence.ExecuteScalar(session, sql);

            return
                retorno != null && retorno != DBNull.Value && !string.IsNullOrEmpty(retorno.ToString()) ?
                    retorno.ToString().StrParaIntNullable() : null;
        }

        /// <summary>
        /// Obtém valores de campos específicos
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public uint ObtemIdProd(GDASession sessao, uint idProdPed)
        {
            string sql = "Select coalesce(idProd, 0) From produtos_pedido Where idProdPed=" + idProdPed;

            return ExecuteScalar<uint>(sessao, sql);
        }

        /// <summary>
        /// Obtém valores de campos específicos
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public float ObtemTotM(uint idProdPed)
        {
            return ObtemTotM(null, idProdPed);
        }

        /// <summary>
        /// Obtém valores de campos específicos
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public float ObtemTotM(GDASession sessao, uint idProdPed)
        {
            string sql = "SELECT TotM FROM produtos_pedido WHERE idProdPed=" + idProdPed;

            var dado = ExecuteScalar<float?>(sessao, sql);

            return dado == null ? 0 : dado.Value;
        }

        /// <summary>
        /// Obtém valores de campos específicos
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public float ObtemQtde(uint idProdPed)
        {
            return ObtemQtde(null, idProdPed);
        }

        /// <summary>
        /// Obtém valores de campos específicos
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public float ObtemQtde(GDASession sessao, uint idProdPed)
        {
            string sql = "Select coalesce(qtde,0) From produtos_pedido Where idProdPed=" + idProdPed;

            return objPersistence.LoadResult(sessao, sql).Select(f => f.GetFloat(0)).FirstOrDefault();
        }

        public float? ObtemQtde(uint idPedido, uint idProd)
        {
            string sql = @"
                SELECT pp.qtde
                FROM produtos_pedido pp
                WHERE pp.idPedido = " + idPedido + " AND pp.idProd = " + idProd;

            return ExecuteScalar<float?>(sql);
        }

        /// <summary>
        /// Obtém valores de campos específicos
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public float ObtemQtdSaida(uint idProdPed)
        {
            string sql = "Select Coalesce(qtdSaida, 0) From produtos_pedido Where idProdPed=" + idProdPed;

            return float.Parse(objPersistence.ExecuteScalar(sql).ToString());
        }

        /// <summary>
        /// Obtém o número da etiqueta de reposição
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public string ObtemNumEtiquetaRepos(uint idProdPed)
        {
            return ObtemValorCampo<string>("numEtiquetaRepos", "idProdPed=" + idProdPed);
        }

        public uint ObtemIdProcesso(GDASession session, uint idProdPed)
        {
            return ObtemValorCampo<uint>(session, "idProcesso", "idProdPed=" + idProdPed);
        }

        public uint ObterAltura(GDASession session, uint idProdPed)
        {
            return ObtemValorCampo<uint>(session, "altura", "idProdPed=" + idProdPed);
        }

        /// <summary>
        /// Obtém a loja do subgrupo do produto do pedido.
        /// </summary>
        public List<int> ObterIdsLojaSubgrupoProdPeloPedido(GDASession session, int idPedido)
        {
            var sql = string.Format(@"SELECT DISTINCT spl.IdLoja FROM produtos_pedido pp
                    INNER JOIN produto p ON (pp.IdProd=p.IdProd)
                    INNER JOIN subgrupo_prod sp ON (p.IdSubgrupoProd=sp.IdSubgrupoProd)
                    INNER JOIN subgrupoprod_loja spl ON (sp.IDSUBGRUPOPROD=spl.IDSUBGRUPOPROD)
                WHERE pp.IdPedido={0} AND pp.IdProdPedParent IS NULL", idPedido);

            var retorno = ExecuteMultipleScalar<int>(session, sql);

            return retorno != null && retorno.Count > 0 ? retorno : new List<int>();
        }

        public decimal ObterCustoProd(GDASession session, int idProdPed)
        {
            return ObtemValorCampo<decimal>(session, "CustoProd", $"IdProdPed={ idProdPed }");
        }

        #endregion

        #region Verifica se uma etiqueta foi reposta

        /// <summary>
        /// Verifica se uma etiqueta foi reposta.
        /// </summary>
        public bool IsEtiquetaReposta(string numEtiqueta)
        {
            if (String.IsNullOrEmpty(numEtiqueta))
                return false;

            string sql = @"select count(*) from pedido p inner join produtos_pedido pp on (pp.idPedido=p.idPedido)
                where p.idPedidoAnterior=?idPedido and numEtiquetaRepos=?numEtiqueta and p.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado;

            string idPedido = numEtiqueta.Split('-')[0];
            return objPersistence.ExecuteSqlQueryCount(sql, new GDAParameter("?idPedido", idPedido),
                new GDAParameter("?numEtiqueta", numEtiqueta)) > 0;
        }

        #endregion

        #region Exportação de pedidos

        /// <summary>
        /// Recupera os produtos do pedido para exportação XML.
        /// </summary>
        public ProdutosPedido[] GetForExportacao(uint idPedido, uint idAmbientePedido, uint[] idsProdutosPedido,
            bool usarEspelho, bool somenteVidros)
        {
            string sql = @"
                select pp.*, p.codInterno, p.descricao as descrProduto, ea.codInterno as codAplicacao,
                    ep.codInterno as codProcesso, COALESCE(ncm.ncm, p.ncm) as ncm, ppe.larguraReal
                from produtos_pedido pp
                    inner join produto p on (pp.idProd=p.idProd)
                    left join etiqueta_aplicacao ea on (pp.idAplicacao=ea.idAplicacao)
                    left join etiqueta_processo ep on (pp.idProcesso=ep.idProcesso)
                    left join produtos_pedido_espelho ppe on (pp.idProdPedEsp=ppe.idProdPed)
                    LEFT JOIN pedido ped ON (pp.idPedido = ped.IdPedido)
                    LEFT JOIN
                    (
                        SELECT *
                        FROM produto_ncm
                    ) as ncm ON (ped.IdLoja = ncm.IdLoja AND p.IdProd = ncm.IdProd)
                where coalesce(pp.invisivel{0}, false)=false and pp.idPedido=" + idPedido;

            if (idsProdutosPedido.Length > 0)
            {
                string idsProd = "";
                foreach (uint idProd in idsProdutosPedido)
                    idsProd += idProd + ",";

                sql += " and pp.idProdPed in (" + idsProd.TrimEnd(',') + ")";
            }

            if (idAmbientePedido > 0)
                sql += " and pp" + (usarEspelho ? "e" : "") + ".idAmbientePedido=" + idAmbientePedido;

            // Não exporta produtos de estoque
            if (somenteVidros)
                sql += " and p.IdGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro +
                    " And (p.idSubgrupoProd is null Or p.idSubgrupoProd not in (Select idSubgrupoProd from subgrupo_prod Where produtosEstoque=true))";

            sql = String.Format(sql, usarEspelho ? "Fluxo" : "Pedido");
            var retorno = objPersistence.LoadData(sql).ToList().ToArray();

            // Coloca as medidas reais do PCP nos pedidos para exportação
            if (usarEspelho)
                foreach (ProdutosPedido pp in retorno)
                {
                    pp.Altura = pp.AlturaReal;
                    if (pp.LarguraReal > 0)
                        pp.Largura = pp.LarguraReal;
                }

            return retorno.ToArray();
        }

        #endregion

        #region Recupera os beneficiamentos de um produto

        public GenericBenefCollection GetBeneficiamentos(GDASession sessao, uint idProdPed)
        {
            string where = "idProdPed=" + idProdPed;

            ProdutosPedido pp = new ProdutosPedido();
            pp.IdProdPed = idProdPed;
            pp.IdProd = ObtemValorCampo<uint>(sessao, "idProd", where);

            return pp.Beneficiamentos;
        }

        #endregion

        #region Recalcula os valores de um produto

        /// <summary>
        /// Recalcula os valores unitários e totais brutos e líquidos.
        /// </summary>
        internal void RecalcularValores(GDASession session, ProdutosPedido prodPed, Pedido pedido,
            bool somarAcrescimoDesconto)
        {
            GenericBenefCollection benef = prodPed.Beneficiamentos;
            decimal valorBenef = prodPed.ValorBenef;

            try
            {
                prodPed.Beneficiamentos = GenericBenefCollection.Empty;
                prodPed.ValorBenef = 0;

                ValorBruto.Instance.Calcular(session, pedido, prodPed);

                prodPed.InicializarParaCalculo(session, pedido);
                prodPed.ValorTabelaPedido = (prodPed as IProdutoCalculo).DadosProduto.ValorTabela();

                var valorUnitario = ValorUnitario.Instance.RecalcularValor(session, pedido, prodPed, !somarAcrescimoDesconto);
                prodPed.ValorVendido = valorUnitario ?? Math.Max(prodPed.ValorTabelaPedido, prodPed.ValorVendido);

                bool isPedidoProducaoCorte = (pedido as IContainerCalculo).IsPedidoProducaoCorte;

                ValorTotal.Instance.Calcular(
                    session,
                    pedido,
                    prodPed,
                    Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarApenasCalculo,
                    prodPed.TipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto && !isPedidoProducaoCorte,
                    prodPed.Beneficiamentos.CountAreaMinimaSession(session)
                );

                // Atualiza o total do produto com os descontos e acréscimos
                if (PedidoConfig.RatearDescontoProdutos)
                    /* Chamado 52325.
                     * Incluí o valor do campo ValorDescontoQtde na somatória de desconto porque este valor é considerado no método
                     * CalculaValorBruto ao se obter o total bruto do produto. Sem a soma deste valor, o total bruto e valor unitário
                     * bruto do produto estavam desconsiderando o valor de desconto por quantidade como um desconto do produto. */
                    prodPed.Total -= prodPed.ValorDesconto + prodPed.ValorDescontoProd + prodPed.ValorDescontoQtde;

                prodPed.Total += prodPed.ValorAcrescimo + prodPed.ValorAcrescimoProd;
                ValorBruto.Instance.Calcular(session, pedido, prodPed);

                if (!PedidoConfig.RatearDescontoProdutos)
                    prodPed.Total -= prodPed.ValorDesconto + prodPed.ValorDescontoProd + prodPed.ValorDescontoQtde;
            }
            finally
            {
                prodPed.Beneficiamentos = benef;
                prodPed.ValorBenef = valorBenef;
            }
        }

        #endregion

        #region Verifica se o desconto pode ser aplicado ao pedido (vendedor)

        /// <summary>
        /// Verifica se o desconto pode ser aplicado ao pedido (vendedor).
        /// </summary>
        public bool PodeAplicarDescontoVendedor(uint idPedido)
        {
            uint idCli = PedidoDAO.Instance.ObtemIdCliente(null, idPedido);
            uint idFunc = PedidoDAO.Instance.ObtemIdFunc(null, idPedido);
            uint? idFuncCliente = ClienteDAO.Instance.ObtemIdFunc(idCli);

            if (!PedidoConfig.DescontoPedidoVendedorUmProduto ||
                UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Vendedor)
                return false;

            // Garante que haja apenas um tipo de produto
            string sql = "select distinct idProd from produtos_pedido where idPedido=" + idPedido;

            IList<uint> idProd = ExecuteMultipleScalar<uint>(sql);
            if (idProd.Count != 1)
                return false;

            // Garante que nenhum produto possua desconto
            sql = @"select count(*) from produtos_pedido where coalesce(valorDescontoQtde, 0)+
                coalesce(valorDescontoProd, 0)>0 and idPedido=" + idPedido;

            if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
                return false;

            // Só permite ao vendedor alterar o desconto se houver um
            // desconto por quantidade cadastrado para o produto
            return DescontoQtdeDAO.Instance.GetCountByProd(idProd[0]) > 0;
        }

        public class DescontoMaximoVendedor
        {
            public DescontoMaximoVendedor(float descontoProd, float descontoCliente)
            {
                DescontoProd = descontoProd;
                DescontoCliente = descontoCliente;
            }

            public float DescontoProd { get; private set; }
            public float DescontoCliente { get; private set; }

            public float DescontoMax
            {
                get { return Math.Max(DescontoProd - DescontoCliente, 0); }
            }
        }

        /// <summary>
        /// Retorna os dados para desconto do pedido (vendedor).
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public DescontoMaximoVendedor GetDescontoMaximoPedidoVendedor(uint idPedido)
        {
            if (!PodeAplicarDescontoVendedor(idPedido))
                return new DescontoMaximoVendedor(0, 0);

            string where = String.Format("!coalesce(invisivel{0}, false) and idPedido={1}",
                PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido", idPedido);

            // Recupera os dados do produto
            string sql = "select idProd from produtos_pedido where " + where;
            uint idProd = ExecuteScalar<uint>(sql);
            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd((int)idProd);
            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)idProd);

            // Quantidade mínima de produtos no pedido (usada para o perc. desconto)
            sql = "select min(qtde) from produtos_pedido where " + where;
            float qtde = ExecuteScalar<float>(sql);

            // Recupera o desconto por produto (pela quantidade mínima)
            float descontoProd = 0;
            foreach (DescontoQtde d in DescontoQtdeDAO.Instance.GetByProd(idProd))
                if (d.Qtde <= qtde)
                    descontoProd = d.PercDescontoMax;
                else
                    break;

            // Recupera o desconto do cliente (calculado)
            sql = @"select 100 * sum(coalesce(valorDescontoCliente,0)) /
                sum((coalesce(valorVendido,0)+coalesce(valorDescontoCliente,0)))
                from produtos_pedido where " + where;

            float descontoCliente = ExecuteScalar<float>(sql);

            return new DescontoMaximoVendedor(descontoProd, descontoCliente);
        }

        #endregion

        #region Verifica se há M2 disponível para o produto

        /// <summary>
        /// Verifica se o pedido de revenda associado ao de produção permite a inserção de um novo item
        /// </summary>
        public bool PedidoReferenciadoPermiteInsercao(GDASession sessao, ProdutosPedido prodPed)
        {
            var idPedRef = PedidoDAO.Instance.ObterIdPedidoRevenda(sessao, (int)prodPed.IdPedido);

            if (idPedRef == 0)
                return true;

            var produtosPedProducao = GetByPedido(sessao, prodPed.IdPedido);
            var produtosPedRevenda = GetByPedido(sessao, (uint)idPedRef);

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
            decimal total = prodPed.Total, custoProd = prodPed.CustoProd;
            Single totM2 = prodPed.TotM, altura = prodPed.Altura, totM2Calc = prodPed.TotM2Calc;
            bool redondo = prodPed.Redondo ||
                           (prodPed.IdAmbientePedido > 0
                               ? AmbientePedidoDAO.Instance.IsRedondo(sessao, prodPed.IdAmbientePedido.Value)
                               : false);

            var isPedidoProducaoCorte = PedidoDAO.Instance.IsPedidoProducaoCorte(sessao, prodPed.IdPedido);

            ProdutoDAO.Instance.CalcTotaisItemProd(null, idCliente, (int)prodPed.IdProd,
                prodPed.Largura, qtde, qtdeAmbiente, prodPed.ValorVendido, prodPed.Espessura,
                redondo, 0, false, prodPed.TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 && !isPedidoProducaoCorte,
                ref custoProd, ref altura, ref totM2, ref totM2Calc, ref total,
                alturaBenef, larguraBenef, false, prodPed.Beneficiamentos.CountAreaMinimaSession(sessao), true);

            var totMRevenda = Math.Round(produtosPedRevenda.Where(f => idsProd.Contains(f.IdProd)).Sum(f => f.TotM2Calc), 2);
            var totMProducao = Math.Round((produtosPedProducao.Where(f => f.IdProdPed != prodPed.IdProdPed && f.IdProd == prodPed.IdProd).Sum(f => f.TotM2Calc) + totM2Calc), 2);

            if (totMProducao > totMRevenda)
                return false;

            return true;
        }

        #endregion

        #region Métodos sobrescritos

        #region Insert chamado ao gerar pedido pelo projeto

        /// <summary>
        /// Utilizado ao gerar pedido através de um projeto
        /// </summary>
        public uint InsertFromProjeto(ProdutosPedido objInsert)
        {
            return InsertFromProjeto(null, objInsert);
        }

        /// <summary>
        /// Utilizado ao gerar pedido através de um projeto
        /// </summary>
        public uint InsertFromProjeto(GDASession sessao, ProdutosPedido objInsert)
        {
            try
            {
                var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(sessao, objInsert.IdPedido);

                ValorTotal.Instance.Calcular(
                    sessao,
                    pedido,
                    objInsert,
                    Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarEAtualizarProduto,
                    true,
                    objInsert.Beneficiamentos.CountAreaMinimaSession(sessao)
                );

                return InsertBase(sessao, objInsert, pedido);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao incluir Produto no Pedido. Erro: " + ex.Message);
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// Insere o produto que veio do orçamento, sem alterar seu valor
        /// </summary>
        public uint InsertBase(GDASession sessao, ProdutosPedido objInsert, IContainerCalculo container)
        {
            CalculaDescontoEValorBrutoProduto(sessao, objInsert, container, true);

            objInsert.IdProdPed = base.Insert(sessao, objInsert);

            AtualizaBenef(sessao, objInsert.IdProdPed, objInsert.Beneficiamentos, container);
            objInsert.RefreshBeneficiamentos();

            return objInsert.IdProdPed;
        }

        public uint InsertEAtualizaDataEntrega(ProdutosPedido objInsert)
        {
            FilaOperacoes.InserirProdutoPedido.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(transaction, (int)objInsert.IdPedido);
                    var atualizarDataEntrega = pedido.DataEntregaSistema != null && !(pedido.DataEntregaSistema.Value.Date != pedido.DataEntrega.Value.Date) && Config.PossuiPermissao(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega);
                    var retorno = Insert(transaction, objInsert, false, atualizarDataEntrega);

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
                finally
                {
                    FilaOperacoes.InserirProdutoPedido.ProximoFila();
                }
            }
        }

        /// <summary>
        /// Atualiza o valor do pedido ao incluir um produto ao mesmo
        /// </summary>
        public override uint Insert(ProdutosPedido objInsert)
        {
            FilaOperacoes.InserirProdutoPedido.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Insert(transaction, objInsert, false, false);

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
                finally
                {
                    FilaOperacoes.InserirProdutoPedido.ProximoFila();
                }
            }
        }

        /// <summary>
        /// Atualiza o valor do pedido ao incluir um produto ao mesmo
        /// </summary>
        public override uint Insert(GDASession session, ProdutosPedido objInsert)
        {
            return Insert(session, objInsert, false, false);
        }

        /// <summary>
        /// Atualiza o valor do pedido ao incluir um produto ao mesmo
        /// </summary>
        public uint Insert(GDASession session, ProdutosPedido objInsert, bool insersaoComposicao, bool atualizaDataEntrega)
        {
            uint returnValue = 0;

            var pedido = PedidoDAO.Instance.GetElement(session, objInsert.IdPedido);

            if (PedidoConfig.DadosPedido.BloquearItensTipoPedido)
            {
                if (objInsert.IdGrupoProd > 0 && pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.Venda && (objInsert.IdGrupoProd != (uint)NomeGrupoProd.Vidro ||
                        (objInsert.IdGrupoProd == (uint)NomeGrupoProd.Vidro && SubgrupoProdDAO.Instance.IsSubgrupoProducao(session, (int)objInsert.IdGrupoProd,
                        (int)objInsert.IdSubgrupoProd))) && (int)objInsert.IdGrupoProd != (uint)NomeGrupoProd.MaoDeObra)
                {
                    throw new Exception("Não é possível incluir produtos de revenda em um pedido de venda.");
                }

                if (objInsert.IdGrupoProd > 0 && pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.Revenda && ((objInsert.IdGrupoProd == (uint)NomeGrupoProd.Vidro &&
                    !SubgrupoProdDAO.Instance.IsSubgrupoProducao(session, (int)objInsert.IdGrupoProd, (int)objInsert.IdSubgrupoProd)) ||
                    objInsert.IdGrupoProd == (uint)NomeGrupoProd.MaoDeObra))
                {
                    throw new Exception("Não é possível incluir produtos de venda em um pedido de revenda.");
                }
            }

            if (pedido.FastDelivery && objInsert.IdAplicacao > 0)
            {
                var aplicacao = EtiquetaAplicacaoDAO.Instance.GetElementByPrimaryKey(session, objInsert.IdAplicacao.Value);

                if (aplicacao != null && aplicacao.NaoPermitirFastDelivery)
                    throw new Exception(string.Format("Erro|O produto {0} tem a aplicacao {1} e esta aplicacao não permite fast delivery", objInsert.DescrProduto, aplicacao.CodInterno));
            }

            /* Chamado 71112*/
            if (objInsert.IdAmbientePedido > 0)
            {
                if (!AmbientePedidoDAO.Instance.Exists(session, objInsert.IdAmbientePedido.GetValueOrDefault()))
                    throw new Exception("Falha ao incluir produto. Ambiente não encontrado. Atualize a pagina e tente novamente.");
            }

            if (PedidoConfig.DadosPedido.BloquearItensTipoPedido)
            {
                if (objInsert.IdGrupoProd > 0 && pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.Venda && (objInsert.IdGrupoProd != (uint)NomeGrupoProd.Vidro ||
                        (objInsert.IdGrupoProd == (uint)NomeGrupoProd.Vidro && SubgrupoProdDAO.Instance.IsSubgrupoProducao(session, (int)objInsert.IdGrupoProd,
                        (int)objInsert.IdSubgrupoProd))) && (int)objInsert.IdGrupoProd != (uint)NomeGrupoProd.MaoDeObra)
                {
                    throw new Exception("Não é possível incluir produtos de revenda em um pedido de venda.");
                }
                if (objInsert.IdGrupoProd > 0 && pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.Revenda && ((objInsert.IdGrupoProd == (uint)NomeGrupoProd.Vidro &&
                    !SubgrupoProdDAO.Instance.IsSubgrupoProducao(session, (int)objInsert.IdGrupoProd, (int)objInsert.IdSubgrupoProd)) ||
                    objInsert.IdGrupoProd == (uint)NomeGrupoProd.MaoDeObra))
                {
                    throw new Exception("Não é possível incluir produtos de venda em um pedido de revenda.");
                }
            }

            //Valida processo
            if (objInsert.IdSubgrupoProd > 0 && objInsert.IdProcesso.GetValueOrDefault(0) > 0 && !ClassificacaoSubgrupoDAO.Instance.VerificarAssociacaoExistente((int)objInsert.IdSubgrupoProd, (int)objInsert.IdProcesso.GetValueOrDefault(0)))
                throw new Exception("Este processo não pode ser selecionado para este produto.");

            if (!PedidoReferenciadoPermiteInsercao(session, objInsert))
            {
                throw new Exception("Não é possível inserir itens diferentes dos inseridos no pedido de revenda associado, ou metragens maiores que as estabelecidas anteriormente.");
            }

            // Chamado 49537 - Só valida a loja do subgrupo do produto pai com a loja do pedido.
            var idsLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdsLojaPeloProduto(session, (int)objInsert.IdProd);
            var lojaPedido = PedidoDAO.Instance.ObtemIdLoja(session, objInsert.IdPedido);

            if (!insersaoComposicao && idsLojaSubgrupoProd.Count() > 0 && !idsLojaSubgrupoProd.Any(f => f == lojaPedido))
                throw new Exception("Esse produto não pode ser utilizado, pois as lojas do seu subgrupo são diferentes da loja do pedido.");

            /* Chamados 52702 e 52911.
             * A verificação deve ser feita pelo filho, pois, ele pode ser inserido de forma avulsa no pai, através do cadastro do pedido. */
            if (objInsert.IdProdPedParent > 0)
            {
                var idProdProdPedParent = (int)ObtemIdProd(session, objInsert.IdProdPedParent.Value);
                var tipoSubgrupoProdProdPedParent = objInsert.IdProdPedParent > 0 ? SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(session, idProdProdPedParent) : 0;

                // O produto filho que está sendo inserido, não pode ser um produto avô, senão, seria criada uma ligação de bisavô após essa inserção.
                if (tipoSubgrupoProdProdPedParent == TipoSubgrupoProd.VidroDuplo && ProdutoDAO.Instance.VerificarProdutoAvo(session, (int)objInsert.IdProd))
                    throw new Exception(string.Format("O produto duplo/laminado pode possuir no máximo 2 produtos em sua hierarquia de composição. " +
                        "Portanto, não é possível inserir o produto {0}, pois, ele possui mais de 2 produtos em sua hierarquia de composição.",
                        ProdutoDAO.Instance.GetCodInterno(session, (int)objInsert.IdProd)));
            }

            DescontoFormaPagamentoDadosProduto descontoFormPagtoProdNovo = null;
            //Bloqueio de produtos com Grupo e Subgrupo diferentes ao utilizar o controle de desconto por forma de pagamento e dados do produto.
            if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
            {
                DescontoFormaPagamentoDadosProduto descontoFormPagtoProd = null;
                var tipoVenda = pedido.TipoVenda;
                var idFormaPagto = pedido.IdFormaPagto;
                var idTipoCartao = pedido.IdTipoCartao;
                var idParcela = pedido.IdParcela;
                descontoFormPagtoProdNovo = DescontoFormaPagamentoDadosProdutoDAO.Instance.ObterDescontoFormaPagamentoDadosProduto(session, (uint)tipoVenda, idFormaPagto, idTipoCartao, idParcela,
                    objInsert.IdGrupoProd, objInsert.IdSubgrupoProd);

                var produtoPedidoInserido = GetByPedido(session, objInsert.IdPedido);
                if (produtoPedidoInserido != null && produtoPedidoInserido.Count > 0)
                {
                    descontoFormPagtoProd = DescontoFormaPagamentoDadosProdutoDAO.Instance.ObterDescontoFormaPagamentoDadosProduto(session, (uint)tipoVenda, idFormaPagto, idTipoCartao, idParcela,
                        produtoPedidoInserido[0].IdGrupoProd, produtoPedidoInserido[0].IdSubgrupoProd);

                    if ((descontoFormPagtoProd?.IdDescontoFormaPagamentoDadosProduto).GetValueOrDefault() != (descontoFormPagtoProdNovo?.IdDescontoFormaPagamentoDadosProduto).GetValueOrDefault())
                        throw new Exception("O desconto por forma de pagamento e dados do produto novo é diferente do desconto de um dos produtos já inserido no pedido.");

                    // Valida o Grupo e Subgrupo dos produtos
                    if (descontoFormPagtoProdNovo != null)
                    {
                        if (descontoFormPagtoProdNovo.IdGrupoProd.GetValueOrDefault() > 0 && produtoPedidoInserido[0].IdGrupoProd != objInsert.IdGrupoProd)
                            throw new Exception("O grupo do produto novo deve ser igual ao grupo do produto já inserido no pedido ao utilizar o controle de desconto por forma de pagamento e dados do produto.");

                        // Se o DescontoFormaPagamentoDadosProdutoNovo tiver subgrupo e o subgrupo do produto atual for diferente de algum já inserido
                        if (descontoFormPagtoProdNovo.IdSubgrupoProd.GetValueOrDefault() > 0 && produtoPedidoInserido[0].IdSubgrupoProd != objInsert.IdSubgrupoProd)
                            throw new Exception("O subgrupo do produto novo deve ser igual ao subgrupo do produto já inserido no pedido ao utilizar o controle de desconto por forma de pagamento e dados do produto.");
                    }
                }
            }

            // Se for atualizar o produto, remove o desconto e acréscimo que pode ter sido inserido
            if (objInsert.IdAmbientePedido > 0)
            {
                var produtosPedido = GetByAmbiente(session, objInsert.IdAmbientePedido.Value);

                bool acrescimoRemovido = AmbientePedidoDAO.Instance.RemoverAcrescimo(
                    session, pedido, objInsert.IdAmbientePedido.Value, produtosPedido);

                bool descontoRemovido = AmbientePedidoDAO.Instance.RemoverDesconto(
                    session, pedido, objInsert.IdAmbientePedido.Value, produtosPedido);

                AmbientePedidoDAO.Instance.FinalizarAplicacaoAcrescimoDesconto(session, pedido, produtosPedido,
                    acrescimoRemovido || descontoRemovido);

                PedidoDAO.Instance.UpdateTotalPedido(session, pedido);
            }

            // Verifica se o produto é do grupo vidro.
            if (ProdutoDAO.Instance.IsVidro(session, (int)objInsert.IdProd))
            {
                // Recupera o id do subgrupo do produto.
                var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, (int)objInsert.IdProd);
                if (idSubgrupoProd > 0)
                    // Se o cálculo for qtde recupera os bneficiamentos inseridos no cadastro do produto.
                    if (SubgrupoProdDAO.Instance.ObtemTipoCalculo(session, idSubgrupoProd.Value, false) ==
                        (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd)
                    {
                        Produto prod = ProdutoDAO.Instance.GetElementByPrimaryKey(session, objInsert.IdProd);
                        if (prod.Beneficiamentos.Count > 0)
                            objInsert.Beneficiamentos = prod.Beneficiamentos;

                        // Busca novamente a altura e largura do produto, caso estejam definidas no cadastro de produto
                        if (prod.Altura > 0 || prod.Largura > 0)
                        {
                            objInsert.Altura = (float)prod.Altura;
                            objInsert.AlturaReal = (float)prod.Altura;
                            objInsert.Largura = prod.Largura.GetValueOrDefault();
                        }
                    }

                var tamanhoMinimoBisote = Configuracoes.PedidoConfig.TamanhoVidro.AlturaELarguraMinimaParaPecasComBisote;
                var tamanhoMinimoLapidacao = Configuracoes.PedidoConfig.TamanhoVidro.AlturaELarguraMinimaParaPecasComLapidacao;
                var tamanhoMinimoTemperado = Configuracoes.PedidoConfig.TamanhoVidro.AlturaELarguraMinimasParaPecasTemperadas;

                var retorno = string.Empty;

                if (objInsert.Beneficiamentos != null)
                {
                    foreach (var prodBenef in objInsert.Beneficiamentos)
                    {
                        if (BenefConfigDAO.Instance.GetElement(prodBenef.IdBenefConfig).TipoControle == Data.Model.TipoControleBenef.Bisote &&
                            (objInsert.Altura < tamanhoMinimoBisote || objInsert.Largura < tamanhoMinimoBisote))
                            retorno += $"O altura ou largura minima para peças com bisotê é de {tamanhoMinimoBisote}.";

                        if (BenefConfigDAO.Instance.GetElement(prodBenef.IdBenefConfig).TipoControle == Data.Model.TipoControleBenef.Lapidacao &&
                            (objInsert.Altura < tamanhoMinimoLapidacao || objInsert.Largura < tamanhoMinimoLapidacao))
                            retorno += $"O altura ou largura minima para peças com lapidação é de {tamanhoMinimoLapidacao}.";
                    }
                }

                if (tamanhoMinimoTemperado > 0 && SubgrupoProdDAO.Instance.IsVidroTemperado(session, objInsert.IdProd) && objInsert.Altura < tamanhoMinimoTemperado && objInsert.Largura < tamanhoMinimoTemperado)
                    retorno += $"O altura ou largura minima para peças com têmpera é de {tamanhoMinimoTemperado}.";

                if (!string.IsNullOrWhiteSpace(retorno))
                    throw new Exception(retorno);
            }

            if (!objInsert.Redondo && objInsert.IdAmbientePedido > 0 && AmbientePedidoDAO.Instance.IsRedondo(session, objInsert.IdAmbientePedido.Value))
                objInsert.Redondo = true;

            bool isPedidoProducaoCorte = (pedido as IContainerCalculo).IsPedidoProducaoCorte;

            ValorTotal.Instance.Calcular(
                session,
                pedido,
                objInsert,
                Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarApenasCalculo,
                objInsert.TipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto && !isPedidoProducaoCorte,
                objInsert.Beneficiamentos.CountAreaMinimaSession(session)
            );

            objInsert.TipoCalculoUsadoPedido =
                Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)objInsert.IdProd);

            if (ProdutoDAO.Instance.ObtemIdGrupoProd(session, (int)objInsert.IdProd) ==
                (int)Glass.Data.Model.NomeGrupoProd.Vidro && objInsert.Espessura == 0)
                objInsert.Espessura = ProdutoDAO.Instance.ObtemEspessura(session, (int)objInsert.IdProd);

            returnValue = InsertBase(session, objInsert, pedido);
            objInsert.IdProdPed = returnValue;

            var tipoSubgrupoProd = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(session, (int)objInsert.IdProd);

            //Caso o produto seja do subgrupo de tipo laminado, insere os filhos
            if (tipoSubgrupoProd == TipoSubgrupoProd.VidroLaminado || tipoSubgrupoProd == TipoSubgrupoProd.VidroDuplo)
            {
                foreach (var p in ProdutoBaixaEstoqueDAO.Instance.GetByProd(session, objInsert.IdProd, false))
                {
                    var alturaFilho = p.Altura > 0 ? p.Altura : objInsert.Altura;
                    var larguraFilho = p.Largura > 0 ? p.Largura : objInsert.Largura;

                    var beneficiamentosInserir = p.Beneficiamentos;

                    if (objInsert.AplicarBenefComposicao)
                        foreach (var item in objInsert.Beneficiamentos)
                            if (!beneficiamentosInserir.Contains(item))
                                beneficiamentosInserir.Add(item);

                    var idProdPed = Insert(session, new ProdutosPedido()
                    {
                        IdProdPedParent = objInsert.IdProdPed,
                        IdProd = (uint)p.IdProdBaixa,
                        IdProcesso = objInsert.IdProcessoFilhas > 0 ? (uint)objInsert.IdProcessoFilhas : (uint)p.IdProcesso,
                        IdAplicacao = objInsert.IdAplicacaoFilhas > 0 ? (uint)objInsert.IdAplicacaoFilhas : (uint)p.IdAplicacao,
                        IdPedido = objInsert.IdPedido,
                        IdAmbientePedido = objInsert.IdAmbientePedido,
                        Qtde = p.Qtde,
                        Altura = alturaFilho,
                        Largura = larguraFilho,
                        IdProdBaixaEst = p.IdProdBaixaEst,
                        ValorVendido = (objInsert as IProdutoCalculo).DadosProduto.ValorTabela(),
                        Beneficiamentos = beneficiamentosInserir
                    }, true, false);

                    var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IProdutoBaixaEstoqueRepositorioImagens>();

                    var stream = new System.IO.MemoryStream();

                    //Verifica se a matéria prima possui imagem
                    var possuiImagem = repositorio.ObtemImagem(p.IdProdBaixaEst, stream);

                    if (possuiImagem)
                        SalvarImagemProdutoPedido(session, idProdPed, stream);
                }
            }

            //Calcula Desconto por Forma de Pagamento e Dados do Produto se a configuração estiver habilitada
            if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto && descontoFormPagtoProdNovo != null)
            {
                pedido.Desconto = descontoFormPagtoProdNovo.Desconto;
                pedido.TipoDesconto = 1;
                PedidoDAO.Instance.Update(session, pedido);
            }

            // Atualiza o total do pedido
            if (objInsert.IdProdPedParent.GetValueOrDefault(0) == 0)
            {
                PedidoDAO.Instance.UpdateTotalPedido(session, pedido, false, true, false, true);
            }
            else
            {
                PedidoDAO.Instance.AtualizaPeso(session, objInsert.IdPedido);
            }

            //Atualiza o produto pedido
            if (!insersaoComposicao && objInsert.IdProdPedParent.GetValueOrDefault(0) > 0)
            {
                var prodPed = GetElement(session, (uint)objInsert.IdProdPedParent, false, true, true);

                if (SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(session, (int)prodPed.IdProd) != TipoSubgrupoProd.VidroLaminado)
                {
                    prodPed.ValorVendido = CalcValorVendidoProdPai(session, objInsert.IdProdPedParent.Value);
                    prodPed.Beneficiamentos = GetBeneficiamentos(session, objInsert.IdProdPedParent.Value);
                    Update(session, prodPed);
                }
            }

            if (atualizaDataEntrega)
            {
                // Atualiza a data de entrega do pedido para considerar o número de dias mínimo de entrega do subgrupo ao informar o produto.
                bool enviarMensagem;
                PedidoDAO.Instance.RecalcularEAtualizarDataEntregaPedido(session, objInsert.IdPedido, null, out enviarMensagem, true);
            }

            AplicarComissaoDescontoAcrescimo(session, PedidoDAO.Instance.GetElement(session, objInsert.IdPedido));

            if (objInsert.IdProdPedParent.GetValueOrDefault() == 0)
            {
                PedidoDAO.Instance.AtualizarParcelasPedido(session, (int)objInsert.IdPedido);
            }

            return returnValue;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Atualiza o valor do pedido ao excluir um produto do mesmo
        /// </summary>
        public override int Delete(ProdutosPedido objDelete)
        {
            using (var transaction = (new GDA.GDATransaction()))
            {
                try
                {
                    transaction.BeginTransaction();

                    var ret = Delete(transaction, objDelete, true, false);

                    transaction.Commit();
                    transaction.Close();

                    return ret;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw ex;
                }
            }
        }

        public int DeleteEAtualizaDataEntrega(ProdutosPedido objDelete)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var prodPed = GetElement(transaction, objDelete.IdProdPed, false, false, false);

                    if (prodPed == null)
                        prodPed = GetElement(transaction, objDelete.IdProdPed, false, false, true);

                    var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(transaction, prodPed.IdPedido);

                    var atualizarDataEntrega = pedido.DataEntregaSistema != null && !(pedido.DataEntregaSistema.Value.Date != pedido.DataEntrega.Value.Date) && Config.PossuiPermissao(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega);

                    var retorno = Delete(transaction, objDelete, true, atualizarDataEntrega);

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
        public int Delete(GDATransaction transaction, ProdutosPedido objDelete, bool fazerCommit, bool atualizaDataEntrega)
        {
            try
            {
                if (
                    objPersistence.ExecuteSqlQueryCount(transaction,
                        "Select Count(*) From produtos_pedido Where IdProdPed=" +
                        objDelete.IdProdPed) == 0)
                    return 0;

                var prodPed = GetElement(transaction, objDelete.IdProdPed, false, false, false);

                if (prodPed == null)
                    prodPed = GetElement(transaction, objDelete.IdProdPed, false, false, true);

                var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(transaction, prodPed.IdPedido);

                var returnValue = 0;

                try
                {
                    //Se for o pai do produto de composição deleta os filhos
                    foreach (var f in ObterFilhosComposicao(transaction, (int)objDelete.IdProdPed))
                        Delete(transaction, f, false, false);

                    // Exclui os beneficiamentos feitos neste produto
                    ProdutoPedidoBenefDAO.Instance.DeleteByProdPed(transaction, objDelete.IdProdPed);
                    ProdutoPedidoRentabilidadeDAO.Instance.ApagarPorProdutoPedido(transaction, objDelete.IdProdPed);

                    returnValue = base.Delete(transaction, objDelete);

                    //Se estiver deletando um filho, recalcula o valor do pai
                    if (prodPed.IdProdPedParent.GetValueOrDefault(0) > 0)
                    {
                        var prodPedParent = GetElement(transaction, (uint)prodPed.IdProdPedParent, false, true, true);
                        prodPedParent.ValorVendido = CalcValorVendidoProdPai(transaction, prodPed.IdProdPedParent.Value);
                        prodPedParent.Beneficiamentos = GetBeneficiamentos(transaction, prodPed.IdProdPedParent.Value);
                        Update(transaction, prodPedParent);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Falha ao excluir Produto do Pedido. Erro: " + ex.Message);
                }

                try
                {
                    PedidoDAO.Instance.UpdateTotalPedido(transaction, pedido, false, true, false, true);
                }
                catch (Exception ex)
                {
                    throw new Exception("Falha ao atualizar Valor do Pedido. Erro: " + ex.Message);
                }

                if (atualizaDataEntrega)
                {
                    // Atualiza a data de entrega do pedido para considerar o número de dias mínimo de entrega do subgrupo ao informar o produto.
                    bool enviarMensagem;
                    PedidoDAO.Instance.RecalcularEAtualizarDataEntregaPedido(transaction, prodPed.IdPedido, null, out enviarMensagem, false);
                }

                AplicarComissaoDescontoAcrescimo(transaction, PedidoDAO.Instance.GetElement(transaction, prodPed.IdPedido));

                if (objDelete.IdProdPedParent.GetValueOrDefault() == 0)
                {
                    PedidoDAO.Instance.AtualizarParcelasPedido(transaction, (int)prodPed.IdPedido);
                }

                if (transaction != null && fazerCommit)
                {
                    transaction.Commit();
                    transaction.Close();
                }

                return returnValue;
            }
            catch
            {
                if (transaction != null && fazerCommit)
                {
                    transaction.Rollback();
                    transaction.Close();
                }

                throw;
            }
        }

        #endregion

        #region Update

        private void AplicarComissaoDescontoAcrescimo(GDASession session, Pedido pedido)
        {
            if (pedido.PercComissao > 0 || pedido.Acrescimo > 0 || pedido.Desconto > 0)
            {
                var produtosPedido = GetByPedidoLite(session, pedido.IdPedido, false, true);

                if (pedido.PercComissao > 0)
                    PedidoDAO.Instance.RemoverComissao(session, pedido, produtosPedido);

                if (pedido.Acrescimo > 0)
                    PedidoDAO.Instance.RemoverAcrescimo(session, pedido, produtosPedido);

                if (pedido.Desconto > 0)
                    PedidoDAO.Instance.RemoverDesconto(session, pedido, produtosPedido);

                if (pedido.PercComissao > 0)
                    PedidoDAO.Instance.AplicarComissao(session, pedido, pedido.PercComissao, produtosPedido);

                if (pedido.Acrescimo > 0)
                    PedidoDAO.Instance.AplicarAcrescimo(session, pedido, pedido.TipoAcrescimo, pedido.Acrescimo, produtosPedido);

                if (pedido.Desconto > 0)
                    PedidoDAO.Instance.AplicarDesconto(session, pedido, pedido.TipoDesconto, pedido.Desconto, produtosPedido);

                PedidoDAO.Instance.FinalizarAplicacaoComissaoAcrescimoDesconto(session, pedido, produtosPedido, true);
                PedidoDAO.Instance.UpdateTotalPedido(session, pedido);
            }
        }

        public int UpdateBase(GDASession sessao, ProdutosPedido objUpdate)
        {
            var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(sessao, objUpdate.IdPedido);
            return UpdateBase(sessao, objUpdate, pedido);
        }

        internal int UpdateBase(GDASession sessao, ProdutosPedido objUpdate, IContainerCalculo container)
        {
            return UpdateBase(sessao, objUpdate, container, true);
        }

        internal int UpdateBase(GDASession sessao, ProdutosPedido objUpdate, IContainerCalculo container, bool atualizarDiferencaCliente)
        {
            CalculaDescontoEValorBrutoProduto(sessao, objUpdate, container, atualizarDiferencaCliente);
            return base.Update(sessao, objUpdate);
        }

        public int UpdateComTransacao(ProdutosPedido objUpdate)
        {
            FilaOperacoes.AtualizarProdutoPedido.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Update(transaction, objUpdate);

                    AplicarComissaoDescontoAcrescimo(transaction, PedidoDAO.Instance.GetElement(transaction, objUpdate.IdPedido));

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
                finally
                {
                    FilaOperacoes.AtualizarProdutoPedido.ProximoFila();
                }
            }
        }

        public void UpdateProcessoAplicacao(ProdutosPedido objUpdate)
        {
            FilaOperacoes.AtualizarProdutoPedido.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    string sqlIdsProdPedProducao = @"SELECT DISTINCT IdProdPedProducao FROM produto_pedido_producao ppp
                        INNER JOIN produtos_pedido_espelho ppe on(ppp.IdProdPed=ppe.IdProdPed)
                        INNER JOIN produtos_pedido pp on(pp.IdProdPedEsp=ppe.IdProdPed)
                        WHERE pp.IdProdPed=" + objUpdate.IdProdPed;

                    var idsProdPedProducao = ExecuteMultipleScalar<int>(transaction, sqlIdsProdPedProducao);

                    var sql = @"
                        UPDATE produtos_pedido SET
                            IdProcesso=" + (objUpdate.IdProcesso != null ? objUpdate.IdProcesso.ToString() : "null") + @",
                            IdAplicacao=" + (objUpdate.IdAplicacao != null ? objUpdate.IdAplicacao.ToString() : "null") + @"
                        WHERE idProdPed =" + objUpdate.IdProdPed + ";" + @"
                        UPDATE produtos_pedido_espelho SET
                            IdProcesso = " + (objUpdate.IdProcesso != null ? objUpdate.IdProcesso.ToString() : "null") + @",
                            IdAplicacao = " + (objUpdate.IdAplicacao != null ? objUpdate.IdAplicacao.ToString() : "null") + @"
                        WHERE idProdPed = (SELECT IdProdPedEsp FROM produtos_pedido WHERE IdProdPed = " + objUpdate.IdProdPed + @" LIMIT 1);
                        UPDATE material_item_projeto SET
                            IdProcesso = " + (objUpdate.IdProcesso != null ? objUpdate.IdProcesso.ToString() : "null") + @",
                            IdAplicacao = " + (objUpdate.IdAplicacao != null ? objUpdate.IdAplicacao.ToString() : "null") + @"
                        WHERE IdMaterItemProj = (SELECT IdMaterItemProj FROM produtos_pedido WHERE IdProdPed = " + objUpdate.IdProdPed + " LIMIT 1); ";

                    objPersistence.ExecuteCommand(transaction, sql);

                    foreach (var idProdutoPedidoProducao in idsProdPedProducao)
                        RoteiroProducaoEtiquetaDAO.Instance.InserirRoteiroEtiqueta(transaction, (uint)idProdutoPedidoProducao);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
                finally
                {
                    FilaOperacoes.AtualizarProdutoPedido.ProximoFila();
                }
            }
        }

        public int UpdateEAtualizaDataEntrega(ProdutosPedido objUpdate)
        {
            FilaOperacoes.AtualizarProdutoPedido.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(transaction, (int)objUpdate.IdPedido);
                    var atualizarDataEntrega = pedido.DataEntregaSistema != null && !(pedido.DataEntregaSistema.Value.Date != pedido.DataEntrega.Value.Date) && Config.PossuiPermissao(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega);

                    var retorno = Update(transaction, objUpdate, pedido, true, true, atualizarDataEntrega);

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
                finally
                {
                    FilaOperacoes.AtualizarProdutoPedido.ProximoFila();
                }
            }
        }

        public int UpdateEAtualizaDataEntrega(GDASession sessao, ProdutosPedido objUpdate)
        {
            FilaOperacoes.AtualizarProdutoPedido.AguardarVez();

            try
            {
                var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(sessao, (int)objUpdate.IdPedido);
                var atualizarDataEntrega = pedido.DataEntregaSistema != null && !(pedido.DataEntregaSistema.Value.Date != pedido.DataEntrega.Value.Date) && Config.PossuiPermissao(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega);
                return Update(sessao, objUpdate, pedido, true, true, atualizarDataEntrega);
            }
            finally
            {
                FilaOperacoes.AtualizarProdutoPedido.ProximoFila();
            }
        }

        public override int Update(ProdutosPedido objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Update(GDASession sessao, ProdutosPedido objUpdate)
        {
            var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(sessao, objUpdate.IdPedido);
            return Update(sessao, objUpdate, pedido);
        }

        internal int Update(GDASession sessao, ProdutosPedido objUpdate, Pedido pedido)
        {
            return Update(sessao, objUpdate, pedido, true, true, false);
        }

        internal int Update(GDASession sessao, ProdutosPedido objUpdate, Pedido pedido, bool atualizarAmbienteBeneficiamento, bool atualizarTotalPedido, bool atualizaDataEntrega)
        {
            try
            {
                if (pedido == null)
                    throw new ArgumentNullException("pedido");

                if (objUpdate.IdAplicacao > 0)
                {
                    var aplicacao = EtiquetaAplicacaoDAO.Instance.GetElementByPrimaryKey(sessao, objUpdate.IdAplicacao.Value);

                    if (aplicacao != null && aplicacao.NaoPermitirFastDelivery)
                        throw new Exception(string.Format("Erro|O produto {0} tem a aplicacao {1} e esta aplicacao não permite fast delivery", objUpdate.DescrProduto, aplicacao.CodInterno));
                }

                objUpdate.BuscarBenefImportacao = false;

                /* Chamado 71112*/
                if (objUpdate.IdAmbientePedido > 0)
                {
                    if (!AmbientePedidoDAO.Instance.Exists(sessao, objUpdate.IdAmbientePedido.GetValueOrDefault()))
                        throw new Exception("Falha ao incluir produto. Ambiente não encontrado. Atualize a pagina e tente novamente.");
                }

                // Carrega os beneficiamentos do produto anterior
                if (objUpdate.IdProdPedAnterior != null)
                    objUpdate.Beneficiamentos = GetBeneficiamentos(sessao, objUpdate.IdProdPed);

                // Se for atualizar o produto, remove o desconto e acréscimo que pode ter sido inserido
                if (atualizarAmbienteBeneficiamento && objUpdate.IdAmbientePedido > 0)
                {
                    var produtosPedido = GetByAmbiente(sessao, objUpdate.IdAmbientePedido.Value);
                    var remover = produtosPedido.FirstOrDefault(p => p.IdProdPed == objUpdate.IdProdPed);
                    produtosPedido.Remove(remover);
                    produtosPedido.Add(objUpdate);

                    bool acrescimoRemovido = AmbientePedidoDAO.Instance.RemoverAcrescimo(sessao, pedido, objUpdate.IdAmbientePedido.Value, produtosPedido);
                    bool descontoRemovido = AmbientePedidoDAO.Instance.RemoverDesconto(sessao, pedido, objUpdate.IdAmbientePedido.Value, produtosPedido);

                    AmbientePedidoDAO.Instance.FinalizarAplicacaoAcrescimoDesconto(sessao, pedido, produtosPedido, acrescimoRemovido || descontoRemovido);
                }

                var tamanhoMinimoBisote = Configuracoes.PedidoConfig.TamanhoVidro.AlturaELarguraMinimaParaPecasComBisote;
                var tamanhoMinimoLapidacao = Configuracoes.PedidoConfig.TamanhoVidro.AlturaELarguraMinimaParaPecasComLapidacao;
                var tamanhoMinimoTemperado = Configuracoes.PedidoConfig.TamanhoVidro.AlturaELarguraMinimasParaPecasTemperadas;

                var retorno = string.Empty;

                if (objUpdate.Beneficiamentos != null)
                {
                    foreach (var prodBenef in objUpdate.Beneficiamentos)
                    {
                        if (BenefConfigDAO.Instance.GetElement(prodBenef.IdBenefConfig).TipoControle == Data.Model.TipoControleBenef.Bisote &&
                            (objUpdate.Altura < tamanhoMinimoBisote || objUpdate.Largura < tamanhoMinimoBisote))
                            retorno += $"O altura ou largura minima para peças com bisotê é de {tamanhoMinimoBisote}.";

                        if (BenefConfigDAO.Instance.GetElement(prodBenef.IdBenefConfig).TipoControle == Data.Model.TipoControleBenef.Lapidacao &&
                            (objUpdate.Altura < tamanhoMinimoLapidacao || objUpdate.Largura < tamanhoMinimoLapidacao))
                            retorno += $"O altura ou largura minima para peças com lapidação é de {tamanhoMinimoLapidacao}.";
                    }
                }

                if (SubgrupoProdDAO.Instance.GetElementByPrimaryKey(sessao, (uint)ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, (int)objUpdate.IdProd)).IsVidroTemperado &&
                        objUpdate.Altura < tamanhoMinimoTemperado && objUpdate.Largura < tamanhoMinimoTemperado)
                    retorno += $"O altura ou largura minima para peças com têmpera é de {tamanhoMinimoTemperado}.";

                if (!string.IsNullOrWhiteSpace(retorno))
                    throw new Exception(retorno);

                if (!PedidoReferenciadoPermiteInsercao(sessao, objUpdate))
                    throw new Exception("Não é possível inserir itens diferentes dos inseridos no pedido de revenda associado, ou metragens maiores que as estabelecidas anteriormente.");

                //
                DescontoFormaPagamentoDadosProduto descontoFormPagtoProd = null;
                //Bloqueio de produtos com Grupo e Subgrupo diferentes ao utilizar o controle de desconto por forma de pagamento e dados do produto.
                if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
                {
                    var tipoVenda = pedido.TipoVenda;
                    var idFormaPagto = pedido.IdFormaPagto;
                    var idTipoCartao = pedido.IdTipoCartao;
                    var idParcela = pedido.IdParcela;
                    var descontoFormPagtoProdNovo = DescontoFormaPagamentoDadosProdutoDAO.Instance.ObterDescontoFormaPagamentoDadosProduto(sessao, (uint)tipoVenda, idFormaPagto, idTipoCartao, idParcela,
                        (uint)ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, (int)objUpdate.IdProd), (uint)ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, (int)objUpdate.IdProd));

                    var produtoPedidoInserido = GetByPedido(sessao, objUpdate.IdPedido);
                    if (produtoPedidoInserido != null && produtoPedidoInserido.Count > 0)
                    {
                        foreach (var p in produtoPedidoInserido)
                        {
                            if (p.IdProdPed == objUpdate.IdProdPed)
                                continue;

                            descontoFormPagtoProd = DescontoFormaPagamentoDadosProdutoDAO.Instance.ObterDescontoFormaPagamentoDadosProduto(sessao, (uint)tipoVenda, idFormaPagto, idTipoCartao, idParcela,
                                p.IdGrupoProd, p.IdSubgrupoProd);

                            if (descontoFormPagtoProd?.IdDescontoFormaPagamentoDadosProduto != descontoFormPagtoProdNovo?.IdDescontoFormaPagamentoDadosProduto)
                                throw new Exception("O desconto por forma de pagamento e dados do produto novo é diferente do desconto de um dos produtos já inserido no pedido.");

                            // Valida o Grupo e Subgrupo dos produtos
                            if (descontoFormPagtoProdNovo != null)
                            {
                                if (descontoFormPagtoProdNovo.IdGrupoProd.GetValueOrDefault() > 0 && p.IdGrupoProd != objUpdate.IdGrupoProd)
                                    throw new Exception("O grupo do produto novo deve ser igual ao grupo do produto já inserido no pedido ao utilizar o controle de desconto por forma de pagamento e dados do produto.");

                                // Se o DescontoFormaPagamentoDadosProdutoNovo tiver subgrupo e o subgrupo do produto atual for diferente de algum já inserido
                                if (descontoFormPagtoProdNovo.IdSubgrupoProd.GetValueOrDefault() > 0 && p.IdSubgrupoProd != objUpdate.IdSubgrupoProd)
                                    throw new Exception("O subgrupo do produto novo deve ser igual ao subgrupo do produto já inserido no pedido ao utilizar o controle de desconto por forma de pagamento e dados do produto.");
                            }
                        }
                    }
                }

                // Chamado 12222. O valor do IPI do produto estava sendo calculado através do método RemoveAcrescimo,
                // porém, como a variável objUpdate estava com o valor do IPI zerado, ao passar no comando "UpdateBase(objUpdate)",
                // no final deste método, o valor do IPI tornava a ficar zerado. Por isso, recuperamos neste ponto o valor do IPI e do ICMS,
                // para evitar que estes valores fiquem incorretos no produto que está sendo atualizado.
                objUpdate.ValorIpi = ObtemValorCampo<decimal>(sessao, "valorIpi", "idProdPed=" + objUpdate.IdProdPed);
                objUpdate.ValorIcms = ObtemValorCampo<decimal>(sessao, "valorIcms", "idProdPed=" + objUpdate.IdProdPed);
                objUpdate.AliqIpi = ObtemValorCampo<float>(sessao, "aliquotaIpi", "idProdPed=" + objUpdate.IdProdPed);
                objUpdate.AliqIcms = ObtemValorCampo<float>(sessao, "aliqIcms", "idProdPed=" + objUpdate.IdProdPed);

                if (!objUpdate.Redondo && objUpdate.IdAmbientePedido > 0 && AmbientePedidoDAO.Instance.IsRedondo(sessao, objUpdate.IdAmbientePedido.Value))
                    objUpdate.Redondo = true;

                var isPedidoProducaoCorte = PedidoDAO.Instance.IsPedidoProducaoCorte(sessao, objUpdate.IdPedido);

                ValorTotal.Instance.Calcular(
                    sessao,
                    pedido,
                    objUpdate,
                    Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarApenasCalculo,
                    objUpdate.TipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto && !isPedidoProducaoCorte,
                    objUpdate.Beneficiamentos.CountAreaMinimaSession(sessao)
                );

                objUpdate.TipoCalculoUsadoPedido = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, (int)objUpdate.IdProd);
                objUpdate.ValorTabelaPedido = (objUpdate as IProdutoCalculo).DadosProduto.ValorTabela();

                if (ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, (int)objUpdate.IdProd) == (int)Glass.Data.Model.NomeGrupoProd.Vidro && objUpdate.Espessura == 0)
                    objUpdate.Espessura = ProdutoDAO.Instance.ObtemEspessura(sessao, (int)objUpdate.IdProd);

                //Chamado 54616
                var prodPedAtual = GetElementByPrimaryKey(sessao, objUpdate.IdProdPed);

                // Chamado 68239: Se o produto tem beneficiamento pré-cadastrado, verifica se os benefs deste produto_pedido foi apagado e caso tenha sido, recupera
                if (objUpdate.Beneficiamentos.Count == 0 && ProdutoBenefDAO.Instance.ProdutoPossuiBenef(sessao, objUpdate.IdProd))
                    objUpdate.Beneficiamentos = ProdutoDAO.Instance.GetElementByPrimaryKey(sessao, objUpdate.IdProd).Beneficiamentos;

                objUpdate.ValorDescontoQtde = 0;
                UpdateBase(sessao, objUpdate, pedido);

                //Chamado 54616
                //Se for produto de composição atualiza o valor do pai
                if (objUpdate.IdProdPedParent > 0 &&
                    (objUpdate.Qtde != prodPedAtual.Qtde || objUpdate.Altura != prodPedAtual.Altura || objUpdate.Largura != prodPedAtual.Largura || objUpdate.ValorVendido != prodPedAtual.ValorVendido) && !PedidoConfig.NaoRecalcularValorProdutoComposicaoAoAlterarAlturaLargura)
                {
                    var prodPedParent = GetElement(sessao, (uint)objUpdate.IdProdPedParent, false, true, false);

                    if (SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, (int)prodPedParent.IdProd) != TipoSubgrupoProd.VidroLaminado)
                    {
                        prodPedParent.ValorVendido = CalcValorVendidoProdPai(sessao, objUpdate.IdProdPedParent.Value);
                        prodPedParent.Beneficiamentos = GetBeneficiamentos(sessao, objUpdate.IdProdPedParent.Value);
                        Update(sessao, prodPedParent);
                    }
                }

                if (SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, (int)objUpdate.IdProd) == TipoSubgrupoProd.VidroLaminado)
                {
                    foreach (var pp in ObterFilhosComposicao(sessao, (int)objUpdate.IdProdPed))
                    {
                        var composicaoAlterada = false;

                        if (pp.Altura != objUpdate.Altura || pp.Largura != objUpdate.Largura)
                        {
                            pp.Altura = objUpdate.Altura;
                            pp.Largura = objUpdate.Largura;
                            composicaoAlterada = true;
                        }

                        if (objUpdate.AplicarBenefComposicao)
                        {
                            pp.Beneficiamentos = objUpdate.Beneficiamentos;
                            composicaoAlterada = true;
                        }

                        if (composicaoAlterada)
                        {
                            Update(sessao, pp);
                        }
                    }
                }

                /* Chamado 25620. */
                if (atualizarAmbienteBeneficiamento)
                {
                    AtualizaBenef(sessao, objUpdate.IdProdPed, objUpdate.Beneficiamentos, pedido);
                    objUpdate.RefreshBeneficiamentos();
                }

                /* Chamado 61922. */
                if (atualizarTotalPedido)
                {
                    PedidoDAO.Instance.UpdateTotalPedido(sessao, pedido, false, true, false, true);
                    PedidoDAO.Instance.AtualizarParcelasPedido(sessao, (int)pedido.IdPedido);
                }

                if (atualizaDataEntrega)
                {
                    // Atualiza a data de entrega do pedido para considerar o número de dias mínimo de entrega do subgrupo ao informar o produto.
                    bool enviarMensagem;
                    PedidoDAO.Instance.RecalcularEAtualizarDataEntregaPedido(sessao, objUpdate.IdPedido, null, out enviarMensagem, true);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao atualizar Produto do Pedido. ", ex));
            }

            return 1;
        }

        #endregion

        private void CalculaDescontoEValorBrutoProduto(GDASession session, ProdutosPedido produto,
            IContainerCalculo container, bool atualizarDiferencaCliente)
        {
            if (OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento)
            {
                DescontoAcrescimo.Instance.RemoverDescontoQtde(session, container, produto);
                DescontoAcrescimo.Instance.AplicarDescontoQtde(session, container, produto);
            }

            if (atualizarDiferencaCliente)
            {
                DiferencaCliente.Instance.Calcular(session, container, produto);
            }

            var calcMult5 = ProdutoDAO.Instance.IsVidro(session ,(int)produto.IdProd) && produto.TipoCalc != (int)TipoCalculoGrupoProd.M2Direto;

            ValorTotal.Instance.Calcular(session,
                container,
                produto,
                Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarApenasCalculo,
                calcMult5,
                produto.Beneficiamentos.CountAreaMinima);

            ValorBruto.Instance.Calcular(session, container, produto);
        }

        #endregion

        #region Composição

        private string SqlImagemPeca(uint idPedido, uint idProdPed, bool selecionar)
        {
            var campos = selecionar ? @"
                pp.*, p.CodInterno, p.Descricao as DescrProduto, ap.ambiente as ambientePedido,
                prc.CodInterno as CodProcesso, apl.CodInterno as CodAplicacao, p.IdGrupoProd, p.idSubgrupoProd" : "Count(*)";

            var sql = @"
                SELECT " + campos + @"
                FROM produtos_pedido pp
                    LEFT JOIN ambiente_pedido ap ON (pp.idAmbientePedido = ap.idAmbientePedido)
                    LEFT JOIN pedido ped ON (pp.idPedido = ped.idPedido)
                    LEFT JOIN produto p ON (pp.idProd = p.idProd)
                    LEFT JOIN etiqueta_aplicacao apl ON (pp.idAplicacao = apl.idAplicacao)
                    LEFT JOIN etiqueta_processo prc ON (pp.idProcesso = prc.idProcesso)
                WHERE COALESCE(pp.invisivelFluxo, 0) = 0
                    AND p.idGrupoProd = {0}
                    AND pp.IdPedido = {1}";

            sql = string.Format(sql, (int)NomeGrupoProd.Vidro, idPedido);

            if (idProdPed > 0)
                sql += " AND pp.IdProdPed = " + idProdPed;

            return sql;
        }

        private string SqlImagemPecaAvulsa(uint idPedido, uint idProdPed, bool selecionar)
        {
            var campos = selecionar ? @"
                pp.*, p.CodInterno, p.Descricao as DescrProduto, ap.ambiente as ambientePedido,
                prc.CodInterno as CodProcesso, apl.CodInterno as CodAplicacao, p.IdGrupoProd, p.idSubgrupoProd" : "Count(*)";

            var sql = @"
                SELECT " + campos + @"
                FROM produtos_pedido pp
                    LEFT JOIN ambiente_pedido ap ON (pp.idAmbientePedido = ap.idAmbientePedido)
                    LEFT JOIN pedido ped ON (pp.idPedido = ped.idPedido)
                    LEFT JOIN produto p ON (pp.idProd = p.idProd)
                    LEFT JOIN etiqueta_aplicacao apl ON (pp.idAplicacao = apl.idAplicacao)
                    LEFT JOIN etiqueta_processo prc ON (pp.idProcesso = prc.idProcesso)
                WHERE COALESCE(pp.invisivelFluxo, 0) = 0
                    AND p.idGrupoProd = {0}
                    AND pp.IdPedido = {1}";

            sql = string.Format(sql, (int)NomeGrupoProd.Vidro, idPedido);

            if (idProdPed > 0)
                sql += " AND pp.IdProdPed = " + idProdPed;

            return sql;
        }

        public IList<ProdutosPedido> ObterParaImagemComposicao(uint idPedido, uint idProdPed, string sortExpression, int startRow, int pageSize)
        {
            var sql = SqlImagemPeca(idPedido, idProdPed, true);
            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize);
        }

        public int ObterParaImagemCount(uint idPedido, uint idProdPed)
        {
            var sql = SqlImagemPeca(idPedido, idProdPed, false);
            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        /// <summary>
        /// Retorna os produtos que podem ou não ter imagem anexadas
        /// </summary>
        public IList<ProdutosPedido> ObterParaImagemPecaAvulsa(uint idPedido, uint idProdPed, string sortExpression, int startRow, int pageSize)
        {
            var sql = SqlImagemPecaAvulsa(idPedido, idProdPed, true);
            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize);
        }
        public int ObterParaImagemPecaAvulsaCount(uint idPedido, uint idProdPed)
        {
            var sql = SqlImagemPecaAvulsa(idPedido, idProdPed, false);
            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        /// <summary>
        /// Retorna os produtos filhos da composição do produto laminado
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public IList<ProdutosPedido> ObterFilhosComposicao(GDASession session, int idProdPed)
        {
            var sql = @"SELECT * FROM produtos_pedido WHERE IdProdPedParent = " + idProdPed;

            return objPersistence.LoadData(session, sql).ToList();
        }

        public int? ObterIdProdPedParent(GDASession session, int idProdPed)
        {
            return ObtemValorCampo<int?>(session, "IdProdPedParent", "IdProdPed = " + idProdPed);
        }

        public uint? ObterIdProdPedEspParent(uint idProdPedParent)
        {
            var sql = @"
                SELECT IdProdPedEsp
                FROM produtos_pedido
                WHERE IdProdPed = " + idProdPedParent;

            return ExecuteScalar<uint?>(sql);
        }

        public uint? ObterIdProdPedParentByEsp(GDASession sessao, uint idProdPedEsp)
        {
            return ObtemValorCampo<uint?>("IdProdPedParent", "IdProdPedEsp=" + idProdPedEsp);
        }

        public bool TemProdutoLamComposicao(uint idPedido)
        {
            return TemProdutoLamComposicao(null, idPedido);
        }

        public bool TemProdutoLamComposicao(GDASession session, uint idPedido)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM produtos_pedido pp
                    INNER JOIN produto p ON (pp.IdProd = p.IdProd)
                    INNER JOIN subgrupo_prod sgp ON (p.IdSubgrupoProd = sgp.IdSubgrupoProd)
                WHERE sgp.TipoSubGrupo IN (" + (int)TipoSubgrupoProd.VidroDuplo + "," + (int)TipoSubgrupoProd.VidroLaminado + @")
                    AND pp.IdPedido = " + idPedido;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        public bool TemProdutoTemperado(uint idPedido)
        {
            return TemProdutoTemperado(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido possui algum produto que seja "Temperado"
        /// </summary>
        public bool TemProdutoTemperado(GDASession session, uint idPedido)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM produtos_pedido pp
                    INNER JOIN produto p ON (pp.IdProd = p.IdProd)
                    INNER JOIN subgrupo_prod sgp ON (p.IdSubgrupoProd = sgp.IdSubgrupoProd)
                WHERE sgp.IsVidroTemperado
                    AND pp.IdPedido = " + idPedido;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Verifica se o pai do produto da composição é do subgrupo do tipo laminado
        /// </summary>
        /// <param name="idProdPedParent"></param>
        /// <returns></returns>
        public bool IsProdLaminado(uint idProdPedParent)
        {
            var idprod = ObtemIdProd(null, idProdPedParent);
            var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)idprod);

            return tipoSubgrupo == TipoSubgrupoProd.VidroLaminado;
        }

        public bool TemFilhoComposicao(int idProdPed)
        {
            return TemFilhoComposicao(null, idProdPed);
        }

        public bool TemFilhoComposicao(GDASession session, int idProdPed)
        {
            return objPersistence.ExecuteSqlQueryCount(session, "SELECT COUNT(*) FROM produtos_pedido WHERE idProdPedParent = " + idProdPed) > 0;
        }

        #endregion

        #region Otimização de Alumínios

        public List<ProdutosPedido> ObterAluminiosParaOtimizacao(int idPedido)
        {
            var sql = @"
                SELECT pp.IdPedido, pp.IdProdPed, pp.AlturaReal, p.CodInterno, pp.Qtde,
                    p.Descricao as DescrProduto, CAST((p.peso * pp.AlturaReal) as DECIMAL(12, 2)) as peso,
                    (po.IdPecaOtimizada IS NOT NULL) as PecaOtimizada, pp.IdProd, mip.GrauCorte, gm.Esquadria as ProjetoEsquadria
                FROM produtos_pedido pp
	                INNER JOIN produto p ON (pp.IdProd = p.IdProd)
                    LEFT JOIN material_item_projeto mip ON (pp.IdMaterItemProj = mip.IdMaterItemProj)
                    LEFT JOIN material_projeto_modelo mpm ON (mip.IdMaterProjMod = mpm.IdMaterProjMod)
                    LEFT JOIN projeto_modelo pm ON (mpm.IdProjetoModelo = pm.IdProjetoModelo)
                    LEFT JOIN grupo_modelo gm ON (pm.IdGrupoModelo = gm.IdGrupoModelo)
	                LEFT JOIN subgrupo_prod sp ON (p.IdSubgrupoProd = sp.IdSubgrupoProd)
                    LEFT JOIN grupo_prod gp ON (p.IdGrupoProd = gp.IdGrupoProd)
                    LEFT JOIN peca_otimizada po ON (pp.IdProdPed = po.IdProdPed)
                WHERE COALESCE(pp.InvisivelFluxo, 0) = 0
	                AND COALESCE(sp.TipoCalculo, gp.TipoCalculo) IN ({0})
                    AND gp.IdGrupoProd={1}
	                AND pp.IdPedido = {2}
                GROUP BY pp.IdProdPed";

            var tipoCalc = (int)TipoCalculoGrupoProd.Perimetro + "," + (int)TipoCalculoGrupoProd.ML + "," + (int)TipoCalculoGrupoProd.MLAL0 + "," + (int)TipoCalculoGrupoProd.MLAL05 + "," +
                (int)TipoCalculoGrupoProd.MLAL1 + "," + (int)TipoCalculoGrupoProd.MLAL6;

            sql = string.Format(sql, tipoCalc, (int)NomeGrupoProd.Alumínio, idPedido);

            return objPersistence.LoadData(sql);
        }

        #endregion

        #region Obtém os produtos de pedido das ordens de carga

        /// <summary>
        /// Obtém a quantidade de pedidos, peso, total de M2 e valor total da ordem de carga informada, com base nos produtos dos pedidos associados e não associados à ela.
        /// Método criado com base no método GetPedidosForOC, da classe PedidoDAO.
        /// </summary>
        public IEnumerable<ProdutosPedido> ObterProdutosPedidoPelasOrdensDeCarga(GDASession session, IEnumerable<int> idsOrdemCarga)
        {
            // Recupera todos os pedidos associados às ordens de carga informadas.
            var idsPedido = ExecuteMultipleScalar<int>(session, string.Format(@"SELECT DISTINCT(p.IdPedido)
                FROM pedido p
                    INNER JOIN pedido_ordem_carga poc ON (p.IdPedido = poc.IdPedido)
                WHERE poc.IdOrdemCarga IN ({0})", string.Join(",", idsOrdemCarga.ToList())));

            /* Chamado 64529. */
            if (idsPedido == null || idsPedido.Count == 0)
                return null;

            var totalProdComDesconto = !PedidoConfig.RatearDescontoProdutos ?
                    "ROUND(pp.Total * ( 1 - IF(ped.TipoDesconto = 2, ped.Desconto/(ped.Total + ped.Desconto),ped.Desconto/100)),2) AS Total" :
                    "pp.Total";

            var idsPedidoString = string.Join(",", idsPedido);

            // Busca os produtos de pedido inclusos na ordem de carga informada, com os dados que são utilizados para recuperar os totais da ordem de carga.
            var sql = $@"
                SELECT pp.IdProdPed, pp.IdProdPedEsp, pp.IdPedido, poc.IdOrdemCarga, IF(ped.TipoPedido={(int)Pedido.TipoPedidoEnum.MaoDeObra}, ap.Qtde * pp.Qtde, pp.Qtde) AS Qtde, 0 AS QtdeVolume, pp.TotM, pp.Peso, {totalProdComDesconto},
                    pp.ValorIpi, pp.ValorIcms
                FROM produtos_pedido pp
                    INNER JOIN pedido ped ON (pp.IdPedido=ped.IdPedido)
                    INNER JOIN pedido_ordem_carga poc ON (pp.IdPedido = poc.IdPedido)
	                INNER JOIN produto prod ON (pp.IdProd = prod.IdProd)
                    LEFT JOIN ambiente_pedido ap ON (pp.IdAmbientePedido=ap.IdAmbientePedido)
	                LEFT JOIN grupo_prod gp ON (prod.IdGrupoProd = gp.IdGrupoProd)
	                LEFT JOIN subgrupo_prod sgp ON (prod.IdSubGrupoProd = sgp.IdSubGrupoProd)
                WHERE pp.IdPedido IN ({idsPedidoString})
	                AND (pp.InvisivelFluxo IS NULL OR pp.InvisivelFluxo = 0)
	                AND gp.IdGrupoProd IN ({(int)NomeGrupoProd.Vidro}, {(int)NomeGrupoProd.MaoDeObra})
	                AND (sgp.ProdutosEstoque IS NULL OR sgp.ProdutosEstoque = 0)
	                AND COALESCE(sgp.GeraVolume, gp.GeraVolume, 0) = 0
	                AND pp.IdProdPedParent IS NULL
	                AND pp.Qtde > 0
                GROUP BY pp.IdProdPed, poc.IdOrdemCarga

                UNION ALL

                SELECT pp.IdProdPed, pp.IdProdPedEsp, pp.IdPedido, poc.IdOrdemCarga, IF(ped.TipoPedido={(int)Pedido.TipoPedidoEnum.MaoDeObra}, ap.Qtde * pp.Qtde, pp.Qtde) AS Qtde, 0 AS QtdeVolume, pp.TotM, pp.Peso, {totalProdComDesconto},
                    pp.ValorIpi, pp.ValorIcms
                FROM produtos_pedido pp
                    INNER JOIN pedido ped ON (pp.IdPedido=ped.IdPedido)
                    INNER JOIN pedido_ordem_carga poc ON (pp.IdPedido = poc.IdPedido)
	                INNER JOIN produto prod ON (pp.IdProd = prod.IdProd)
                    LEFT JOIN ambiente_pedido ap ON (pp.IdAmbientePedido=ap.IdAmbientePedido)
	                LEFT JOIN grupo_prod gp ON (prod.IdGrupoProd = gp.IdGrupoProd)
	                LEFT JOIN subgrupo_prod sgp ON (prod.IdSubGrupoProd = sgp.IdSubGrupoProd)
                WHERE pp.IdPedido IN ({idsPedidoString})
                    AND (pp.InvisivelFluxo IS NULL OR pp.InvisivelFluxo = 0)
                    AND gp.IdGrupoProd IN ({(int)NomeGrupoProd.Vidro}, {(int)NomeGrupoProd.MaoDeObra})
                    AND (sgp.ProdutosEstoque IS NOT NULL AND sgp.ProdutosEstoque = 1)
                    AND COALESCE(sgp.GeraVolume, gp.GeraVolume, 0) = 0
                    AND pp.IdProdPedParent IS NULL
                    AND pp.Qtde > 0
                GROUP BY pp.IdProdPed, poc.IdOrdemCarga

                UNION ALL

                SELECT pp.IdProdPed, pp.IdProdPedEsp, pp.IdPedido, poc.IdOrdemCarga, IF(ped.TipoPedido={(int)Pedido.TipoPedidoEnum.MaoDeObra}, ap.Qtde * pp.Qtde, pp.Qtde) AS Qtde, pp.Qtde AS QtdeVolume, pp.TotM, pp.Peso, {totalProdComDesconto},
                    pp.ValorIpi, pp.ValorIcms
                FROM produtos_pedido pp
                    INNER JOIN pedido ped ON (pp.IdPedido=ped.IdPedido)
                    INNER JOIN pedido_ordem_carga poc ON (pp.IdPedido = poc.IdPedido)
	                INNER JOIN produto prod ON (pp.IdProd = prod.IdProd)
                    LEFT JOIN ambiente_pedido ap ON (pp.IdAmbientePedido=ap.IdAmbientePedido)
	                LEFT JOIN grupo_prod gp ON (prod.IdGrupoProd = gp.IdGrupoProd)
	                LEFT JOIN subgrupo_prod sgp ON (prod.IdSubGrupoProd = sgp.IdSubGrupoProd)
                WHERE pp.IdPedido IN ({idsPedidoString})
	                AND (pp.InvisivelFluxo IS NULL OR pp.InvisivelFluxo = 0)
	                AND COALESCE(sgp.GeraVolume, gp.GeraVolume, 0) = 1
	                AND pp.IdProdPedParent IS NULL
	                AND pp.Qtde > 0
                GROUP BY pp.IdProdPed, poc.IdOrdemCarga;";

            // Recupera os produtos de pedido.
            return objPersistence.LoadResult(session, sql).Select(f =>
                new ProdutosPedido()
                {
                    IdProdPed = f["IdProdPed"],
                    IdProdPedEsp = f["IdProdPedEsp"],
                    IdPedido = f["IdPedido"],
                    IdOrdemCarga = f["IdOrdemCarga"],
                    Qtde = f["Qtde"],
                    TotM = f["TotM"],
                    Peso = f["Peso"],
                    Total = f["Total"],
                    ValorIpi = f["ValorIpi"],
                    ValorIcms = f["ValorIcms"],
                    QtdeVolume = f["QtdeVolume"]
                });
        }

        #endregion

        #region Rentabilidade

        /// <summary>
        /// Recupera os produtos do pedido para o calculo da rentabilidade.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<ProdutosPedido> ObterProdutosParaRentabilidade(GDA.GDASession sessao, uint idPedido)
        {
            return objPersistence.LoadData(sessao,
                "SELECT * FROM produtos_pedido WHERE IdPedido=?id AND InvisivelPedido=0",
                new GDAParameter("?id", idPedido))
                .ToList();
        }

        /// <summary>
        /// Atualiza a rentabilidade do produto do pedido..
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPed"></param>
        /// <param name="percentualRentabilidade">Percentual da rentabilidade.</param>
        /// <param name="rentabilidadeFinanceira">Rentabilidade financeira.</param>
        /// <param name="percComissao">Percentual de comissão do produto.</param>
        public void AtualizarRentabilidade(
            GDA.GDASession sessao, uint idProdPed,
            decimal percentualRentabilidade, decimal rentabilidadeFinanceira,
            decimal percComissao)
        {
            objPersistence.ExecuteCommand(sessao,
                @"UPDATE produtos_pedido SET
                    PercentualRentabilidade=?percentual,
                    RentabilidadeFinanceira=?rentabilidade,
                    PercComissao=?percComissao
                WHERE IdProdPed=?id",
                new GDAParameter("?percentual", percentualRentabilidade),
                new GDAParameter("?rentabilidade", rentabilidadeFinanceira),
                new GDAParameter("?percComissao", percComissao),
                new GDAParameter("?id", idProdPed));
        }

        #endregion

        #region Calculo Impostos

        /// <summary>
        /// Atualiza os valores de impostos associados com a instancia informada.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="produtoPedido"></param>
        public void AtualizarImpostos(GDASession sessao, ProdutosPedido produtoPedido)
        {
            // Relação das propriedades que devem ser atualizadas
            var propriedades = new[]
            {
                nameof(ProdutosPedido.IdNaturezaOperacao),
                nameof(ProdutosPedido.Mva),
                nameof(ProdutosPedido.CodValorFiscal),
                nameof(ProdutosPedido.Csosn),
                nameof(ProdutosPedido.Cst),
                nameof(ProdutosPedido.PercRedBcIcms),
                nameof(ProdutosPedido.AliqIpi),
                nameof(ProdutosPedido.ValorIpi),
                nameof(ProdutosPedido.CstIpi),
                nameof(ProdutosPedido.AliqIcms),
                nameof(ProdutosPedido.BcIcms),
                nameof(ProdutosPedido.ValorIcms),
                nameof(ProdutosPedido.AliqFcp),
                nameof(ProdutosPedido.BcFcp),
                nameof(ProdutosPedido.ValorFcp),
                nameof(ProdutosPedido.AliqIcmsSt),
                nameof(ProdutosPedido.BcIcmsSt),
                nameof(ProdutosPedido.ValorIcmsSt),
                nameof(ProdutosPedido.AliqFcpSt),
                nameof(ProdutosPedido.BcFcpSt),
                nameof(ProdutosPedido.ValorFcpSt),
                nameof(ProdutosPedido.AliqPis),
                nameof(ProdutosPedido.BcPis),
                nameof(ProdutosPedido.ValorPis),
                nameof(ProdutosPedido.CstPis),
                nameof(ProdutosPedido.AliqCofins),
                nameof(ProdutosPedido.BcCofins),
                nameof(ProdutosPedido.ValorCofins),
                nameof(ProdutosPedido.CstCofins)
            };

            objPersistence.Update(sessao, produtoPedido, string.Join(",", propriedades), DirectionPropertiesName.Inclusion);
        }

        #endregion

        public List<ProdutosPedido> ObterProdutosNaoExportados(uint idPedido)
        {
            List<ProdutosPedido> lista = new List<ProdutosPedido>();

            if (idPedido > 0)
            {
                string sqlPadrao = Sql(null, idPedido, 0, 0, 0, false, true, false, true, false, false, false, false, 0, true);
                string sql = @"select distinct temp.* from (" + sqlPadrao + @") as temp ";

                sql += @" where temp.IdGrupoProd = 1 and temp.IdProd
                    not in(select IdProd from produtos_pedido_exportacao where idPedido=" + idPedido + @")
                    And (temp.idSubgrupoProd is null Or temp.idSubgrupoProd not in
                    (Select idSubgrupoProd from subgrupo_prod Where produtosEstoque=true))";

                lista = objPersistence.LoadData(sql);
            }

            return lista;
        }

        public List<ProdutosPedido> ObterProdutosComExportados(string idsPedido)
        {
            string sql = @"select p.CodInterno, p.Descricao as DescrProduto, pp.*, cli.Nome as NomeCliente,
                           case when (select count(*) from produtos_pedido_exportacao ppe
                                where ppe.idProd = pp.idprodped) > 0 then true else false end as Exportado
                           from produtos_pedido pp
                           inner join pedido ped on(ped.IdPedido=pp.IdPedido)
                           inner join produto p on(p.IdProd=pp.IdProd)
                           inner join cliente cli on(ped.IdCli=cli.id_cli)
                           left join pedido_exportacao pedEx on(pedEx.IdPedido=ped.idPedido)
						   left join pedido_espelho pedEs on(pedEs.IdPedido=ped.idPedido)
                           where ped.IdPedido in(" + idsPedido + @")
                                and IF(pedEx.DATASITUACAO < pedEs.DATACONF, pp.invisivelFluxo,
                                pp.invisivelPedido) order by p.Descricao asc, exportado desc";

            return objPersistence.LoadData(sql);
        }

        /// <summary>
        /// Atualiza observação do produto do pedido
        /// </summary>
        public void AtualizaObs(uint idProdPed, string obs)
        {
            AtualizaObs(null, idProdPed, obs);
        }

        /// <summary>
        /// Atualiza observação do produto do pedido
        /// </summary>
        public void AtualizaObs(GDASession sessao, uint idProdPed, string obs)
        {
            string sql = "Update produtos_pedido Set obs=?obs Where idprodped=" + idProdPed;

            this.objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?obs", obs));
        }

        /// <summary>
        /// Recupera se os produtos do pedido estão com os beneficiamentos obrigatórios aplicados
        /// </summary>
        public string VerificarBeneficiamentoObrigatorioAplicado(uint idPedido)
        {
            var beneficiamentos = Data.DAL.BenefConfigDAO.Instance.GetForControl(Data.Model.TipoBenef.Todos);
            var benefObrigatorios = new List<BenefConfig>();

            var mensagem = string.Empty;

            var produtosPedido = GetByPedido(idPedido).Where(f => f.IdGrupoProd == 1 && (f.TipoCalc == 2 || f.TipoCalc == 10));

            foreach (var produtoPedido in produtosPedido)
            {

                var idSubGrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)produtoPedido.IdProd);

                benefObrigatorios = beneficiamentos
                    .Where(f => !string.IsNullOrWhiteSpace(f.IdsSubGrupoPreenchimentoObrigatorio)
                        && f.IdsSubGrupoPreenchimentoObrigatorio.Split(',').Select(x => x.StrParaInt()).ToList().Contains(idSubGrupoProd.GetValueOrDefault(0))
                        && (f.TipoControle == TipoControleBenef.ListaSelecao ||
                            f.TipoControle == TipoControleBenef.Lapidacao ||
                            f.TipoControle == TipoControleBenef.Quantidade ||
                            f.TipoControle == TipoControleBenef.Bisote ||
                            f.TipoControle == TipoControleBenef.ListaSelecaoQtd)).ToList();

                var mensagemProd = string.Empty;

                foreach (var benef in benefObrigatorios)
                {
                    if (!produtoPedido.Beneficiamentos.Any(f => (BenefConfigDAO.Instance.GetElement(f.IdBenefConfig).IdParent == benef.IdBenefConfig || f.IdBenefConfig == benef.IdBenefConfig)))
                    {
                        if (string.IsNullOrEmpty(mensagemProd))
                        {
                            mensagemProd = "Os valores dos beneficiamentos (" + string.Join(", ", benefObrigatorios.Select(f => f.Descricao)) + ") devem ser definidos no produto abaixo.\n";

                            if (produtoPedido.IdAmbientePedido.HasValue)
                                mensagemProd += string.Format("Ambiente ({0}) ", AmbientePedidoDAO.Instance.GetElement(produtoPedido.IdAmbientePedido.Value).Ambiente);

                            mensagemProd += string.Format("Produto ({0})\n", produtoPedido.DescricaoProdutoComBenef);
                        }
                    }
                }

                mensagem += mensagemProd;
            }

            return mensagem;
        }

        /// <summary>
        /// Recupera as peças cortadas do pedido de produção associado aos de revenda (pedido de produção para corte)
        /// </summary>
        /// <param name="idLiberarPedido">Id da liberação dos pedidos de revenda associados</param>
        /// <returns></returns>
        public List<ProdutosPedido> ObterProdutosCortados(uint idLiberarPedido)
        {
            return ObterProdutosCortados(null, idLiberarPedido);
        }

        /// <summary>
        /// Recupera as peças cortadas do pedido de produção associado aos de revenda (pedido de produção para corte)
        /// </summary>
        /// <param name="idLiberarPedido">Id da liberação dos pedidos de revenda associados</param>
        /// <returns></returns>
        public List<ProdutosPedido> ObterProdutosCortados(GDASession session, uint idLiberarPedido)
        {
            var sql = @"SELECT pp.*, prod.CodInterno, prod.Descricao AS DescrProduto FROM produtos_pedido pp
                        INNER JOIN produto prod ON pp.IdProd=prod.IdProd
                        INNER JOIN pedido p on pp.IdPedido=p.IdPedido
                        WHERE IdPedidoRevenda IN (SELECT IdPedido FROM produtos_liberar_pedido WHERE IdLiberarPedido=?idLiberarPedido)";

            var parameter = new GDAParameter("?idLiberarPedido", idLiberarPedido);

            var retorno = objPersistence.LoadData(session, sql, parameter);

            return retorno;
        }

        public string ObterUrlImagemSalvar(uint idProdPed)
        {
            return Utils.GetPecaComercialPath + idProdPed.ToString().PadLeft(10, '0') + "_0.jpg";
        }
        public void SalvarImagemProdutoPedido(GDASession session, uint idProdPedFilho, MemoryStream stream)
        {

            var imagemUrlSalvarItem = ObterUrlImagemSalvar(idProdPedFilho);
            ManipulacaoImagem.SalvarImagem(imagemUrlSalvarItem, stream);
            // Cria Log de alteração da Imagem do Produto Pedido
            //Apenas para controle
            LogAlteracaoDAO.Instance.Insert(session, new LogAlteracao
            {
                Tabela = (int)LogAlteracao.TabelaAlteracao.ImagemProdPed,
                IdRegistroAlt = (int)idProdPedFilho,
                Campo = "Imagem Produto Pedido",
                ValorAtual = "Imagem da matéria prima",
                DataAlt = DateTime.Now,
                IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                Referencia = "Imagem do Produto Pedido " + idProdPedFilho,
                NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(session, LogAlteracao.TabelaAlteracao.ImagemProdPed, (int)idProdPedFilho)
            });
        }
    }
}
