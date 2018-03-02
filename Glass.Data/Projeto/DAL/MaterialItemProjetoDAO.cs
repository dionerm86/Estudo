using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class MaterialItemProjetoDAO : BaseDAO<MaterialItemProjeto, MaterialItemProjetoDAO>
	{
        //private MaterialItemProjetoDAO() { }

        #region Busca materiais para listagem padrão

        private string Sql(uint idMaterItemProj, uint idItemProjeto, uint idOrcamento, bool selecionar, 
            out string filtroAdicional)
        {
            filtroAdicional = "";

            string campos = selecionar ? @"mip.*, coalesce(proj.idCliente, coalesce(ped.idCli, coalesce(pedEsp.idCli, orca.idCliente))) as idCliente, 
                p.Descricao as DescrProduto, p.CodInterno, p.IdGrupoProd, p.idSubgrupoProd, COALESCE(ncm.ncm, p.ncm) as ncm, p.custoCompra as custoCompraProduto,
                if(p.AtivarAreaMinima=1, Cast(p.AreaMinima as char), '0') as AreaMinima, apl.CodInterno as CodAplicacao, prc.CodInterno as CodProcesso, 
                mpm.qtde as qtdModelo, pp.codInterno as codMaterial, mpm.CalculoQtde, mpm.CalculoAltura, ip.Ambiente as Ambiente, ip.IdOrcamento,
                p.situacao=" + (int)Glass.Situacao.Inativo + " as ProdutoInativo" : "Count(*)";

            string sql = "Select " + campos + @"
                From material_item_projeto mip
                Left Join item_projeto ip on (mip.idItemProjeto=ip.idItemProjeto)
                left join projeto proj on (ip.idProjeto=proj.idProjeto)
                left join pedido ped on (ip.idPedido=ped.idPedido)
                left join pedido pedEsp on (ip.idPedidoEspelho=pedEsp.idPedido)
                left join orcamento orca on (ip.idOrcamento=orca.idOrcamento)
                Left Join produto p On (mip.idProd=p.idProd)
                Left Join material_projeto_modelo mpm On (mip.idMaterProjMod=mpm.idMaterProjMod)
                Left Join produto_projeto pp On (mpm.idProdProj=pp.idProdProj)
                Left Join etiqueta_aplicacao apl On (mip.idAplicacao=apl.idAplicacao)
                Left Join etiqueta_processo prc On (mip.idProcesso=prc.idProcesso)
                LEFT JOIN
                (
                    SELECT *
                    FROM produto_ncm
                ) as ncm ON (ped.IdLoja = ncm.IdLoja AND p.IdProd = ncm.IdProd)
                Where 1 ?filtroAdicional?";

            if (idMaterItemProj > 0)
                filtroAdicional += " and mip.idMaterItemProj=" + idMaterItemProj;
            else if (idItemProjeto > 0)
                filtroAdicional += " and mip.idItemProjeto=" + idItemProjeto;
            else if (idOrcamento > 0)
                filtroAdicional += " and mip.idItemProjeto in (select idItemProjeto from produtos_orcamento where idOrcamento=" + idOrcamento + ")";

            return sql;
        }

        private void AtualizaPodeEditar(ref List<MaterialItemProjeto> mip)
        {
            if (mip.Count == 0)
                return;

            string ids = String.Empty;
            foreach (MaterialItemProjeto m in mip)
                ids += m.IdMaterItemProj + ",";

            // Recupera os idsProdPed dos materiais
            string sql = @"
                select idProdPed
                from produtos_pedido_espelho
                where coalesce(invisivelFluxo,false)=false and idMaterItemProj in ({0})";

            ids = GetValoresCampo(String.Format(sql, ids.TrimEnd(',')), "idProdPed");

            if (!String.IsNullOrEmpty(ids))
            {
                // Recupera os idsProdPed que estão impressos
                sql = @"
                    select pi.idProdPed
                    from produto_impressao pi
                        left join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                    where coalesce(ie.situacao, 0)={0} and !coalesce(pi.cancelado,false)
                        and pi.idProdPed in ({1})";

                ids = GetValoresCampo(String.Format(sql, (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa,
                    ids.TrimEnd(',')), "idProdPed");

                if (!String.IsNullOrEmpty(ids))
                {
                    // Recupera os idsItemProjeto que possuem produtos impressos
                    sql = @"
                        select idMaterItemProj
                        from produtos_pedido_espelho
                        where coalesce(invisivelFluxo,false)=false and idProdPed in ({0})";

                    ids = GetValoresCampo(String.Format(sql, ids.TrimEnd(',')), "idMaterItemProj");
                }
            }

            List<string> i = new List<string>(!String.IsNullOrEmpty(ids) ? ids.Split(',') : new string[] { });
            foreach (MaterialItemProjeto m in mip)
                m.EditDeleteVisible = !i.Contains(m.IdMaterItemProj.ToString());
        }

        public List<MaterialItemProjeto> GetByItemProjeto(uint idItemProjeto)
        {
            return GetByItemProjeto(null, idItemProjeto);
        }

        public List<MaterialItemProjeto> GetByItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            return GetByItemProjeto(sessao, idItemProjeto, true);
        }

        public MaterialItemProjeto[] GetForExportacaoPedido(uint idItemProjeto, uint[] idsProdutosPedido, bool usarEspelho)
        {
            List<MaterialItemProjeto> itens = GetByItemProjeto(idItemProjeto, false);

            uint idAmbientePedido = (!usarEspelho ? AmbientePedidoDAO.Instance.GetIdByItemProjeto(idItemProjeto) : 
                AmbientePedidoEspelhoDAO.Instance.GetIdByItemProjeto(idItemProjeto)).GetValueOrDefault(0);

            string sql = @"select pp.idProdPed from produtos_pedido pp left join produtos_pedido_espelho ppe on (pp.idProdPedEsp=ppe.idProdPed) 
                where pp{0}.idMaterItemProj=?id or (pp{0}.idAmbientePedido=" + idAmbientePedido + @" and pp{0}.idProd=?idProd and pp{0}.altura=?alt 
                and pp{0}.largura=?larg and pp{0}.totM=?totM and pp{0}.totM2Calc=?totM2Calc)";

            sql = String.Format(sql, !usarEspelho ? "" : "e");
            
            List<uint> idsProdPed = new List<uint>(idsProdutosPedido);
            for (int i = itens.Count - 1; i >= 0; i--)
            {
                uint idProdPed = ExecuteScalar<uint>(sql, new GDAParameter("?id", itens[i].IdMaterItemProj),
                    new GDAParameter("?idProd", itens[i].IdProd), new GDAParameter("?alt", itens[i].Altura), 
                    new GDAParameter("?larg", itens[i].Largura), new GDAParameter("?totM", itens[i].TotM),
                    new GDAParameter("?totM2Calc", itens[i].TotM2Calc));

                if (!idsProdPed.Contains(idProdPed))
                    itens.RemoveAt(i);
            }

            return itens.ToArray();
        }

        public List<MaterialItemProjeto> GetByItemProjeto(uint idItemProjeto, bool ordenar)
        {
            return GetByItemProjeto(null, idItemProjeto, ordenar);
        }

        public List<MaterialItemProjeto> GetByItemProjeto(GDASession sessao, uint idItemProjeto, bool ordenar)
        {
            string filtroAdicional;
            string sql = Sql(0, idItemProjeto, 0, true, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sessao, sql + (ordenar ? " Order By p.idGrupoProd, p.descricao" : "")).ToList();
        }

        public List<MaterialItemProjeto> GetByOrcamento(uint idOrcamento)
        {
            string filtroAdicional;
            string sql = Sql(0, 0, idOrcamento, true, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql);
        }

        public IList<MaterialItemProjeto> GetList(uint idItemProjeto, string sortExpression, int startRow, int pageSize)
        {
            if (idItemProjeto == 0 || GetCountReal(idItemProjeto) == 0)
            {
                var lst = new List<MaterialItemProjeto>();
                lst.Add(new MaterialItemProjeto());
                return lst.ToArray();
            }

            // O order "mip.idMaterItemProj" precisa ser usado para que ao editar um material, apareça ele para edição e não outro material, o que ocorre
            // devido ao mysql usar um ordenação aleatória quando os campos ordenados são iguais (neste caso p.idGrupoProd e p.descricao)
            var sort = String.IsNullOrEmpty(sortExpression) ? "p.idGrupoProd, p.descricao, mip.idMaterItemProj" : sortExpression;

            string filtroAdicional;
            var sql = Sql(0, idItemProjeto, 0, true, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            sql += " Order By " + sort;

            var mip = objPersistence.LoadDataWithSortExpression(sql, new InfoSortExpression(String.Empty), new InfoPaging(startRow, pageSize), null).ToList();
            AtualizaPodeEditar(ref mip);

            return mip;
        }

        public int GetCount(uint idItemProjeto)
        {
            int count = idItemProjeto > 0 ? GetCountReal(idItemProjeto) : 0;
            return count == 0 ? 1 : count;
        }

        public int GetCountReal(uint idItemProjeto)
        {
            string filtroAdicional;
            string sql = Sql(0, idItemProjeto, 0, false, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        public MaterialItemProjeto GetElement(uint idMaterItemProj)
        {
            string filtroAdicional;
            string sql = Sql(idMaterItemProj, 0, 0, true, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadOneData(sql);
        }

        #endregion

        #region Busca todos os materiais de um projeto

        /// <summary>
        /// Busca todos os materiais do projeto passado
        /// </summary>
        public List<MaterialItemProjeto> GetByProjeto(uint idProjeto)
        {
            return GetByProjeto(null, idProjeto);
        }

        /// <summary>
        /// Busca todos os materiais do projeto passado
        /// </summary>
        public List<MaterialItemProjeto> GetByProjeto(GDASession session, uint idProjeto)
        {
            string sql = "Select mip.*, ip.ambiente From material_item_projeto mip " +
                "Left Join item_projeto ip On (mip.idItemProjeto=ip.idItemProjeto) " +
                "Left Join produto p On (mip.idProd=p.idProd) Where mip.idItemProjeto In " +
                "(Select idItemProjeto From item_projeto Where idProjeto=" + idProjeto + ") " + 
                "Order By p.idGrupoProd, p.Descricao ";

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca materiais para impressão de ItemProjeto

        /// <summary>
        /// Busca materiais para impressão de ItemProjeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <param name="buscarPecas">Identifica se é para buscar as peças de vidro também</param>
        /// <returns></returns>
        public List<MaterialItemProjeto> GetForRptItemProjeto(uint idItemProjeto, bool buscarPecas)
        {
            string sql = "Select mip.*, p.Descricao as DescrProduto, p.CodInterno, p.IdGrupoProd, " +
                "p.idSubgrupoProd, if(p.AtivarAreaMinima=1, Cast(p.AreaMinima as char), '0') as AreaMinima, apl.CodInterno as CodAplicacao, " +
                "prc.CodInterno as CodProcesso, mpm.qtde as qtdModelo, pp.codInterno as codMaterial From material_item_projeto mip " +
                "Left Join produto p On (mip.idProd=p.idProd) " +
                "Left Join material_projeto_modelo mpm On (mip.idMaterProjMod=mpm.idMaterProjMod) " +
                "Left Join produto_projeto pp On (mpm.idProdProj=pp.idProdProj) " +
                "Left Join etiqueta_aplicacao apl On (mip.idAplicacao=apl.idAplicacao) " +
                "Left Join etiqueta_processo prc On (mip.idProcesso=prc.idProcesso) " +
                "Where mip.idItemProjeto=" + idItemProjeto; 
            
            if (!buscarPecas)
                sql += " And mip.idPecaItemProj is null "; 
            
            sql += " Order By p.idGrupoProd, p.descricao";

            List<MaterialItemProjeto> lstMater = objPersistence.LoadData(sql);
            List<MaterialItemProjeto> lstMaterRetorno = new List<MaterialItemProjeto>();
            List<MaterialProjetoBenef> lstBenef;

            // Adiciona os beneficiamentos feitos nos produtos como itens do pedido
            foreach (MaterialItemProjeto mip in lstMater)
            {
                // Verifica se o material foi marcado como redondo
                if (mip.Redondo)
                {
                    if (!BenefConfigDAO.Instance.CobrarRedondo() && !mip.DescrProduto.ToLower().Contains("redondo"))
                        mip.DescrProduto += " REDONDO";

                    mip.Largura = 0;
                }

                lstMaterRetorno.Add(mip);

                // Carrega os beneficiamentos deste material, se houver
                lstBenef = MaterialProjetoBenefDAO.Instance.GetByMaterial(null, mip.IdMaterItemProj);

                if (!PedidoConfig.RelatorioPedido.AgruparBenefRelatorio)
                {
                    // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                    foreach (MaterialProjetoBenef mpb in lstBenef)
                    {
                        MaterialItemProjeto mater = new MaterialItemProjeto();
                        mater.IdItemProjeto = mip.IdItemProjeto;
                        mater.Ambiente = mip.Ambiente;
                        mater.Qtde = mpb.Qtd > 0 ? mpb.Qtd : 1;
                        mater.Valor = mpb.Valor / (int)mater.Qtde;
                        mater.Total = mpb.Valor;
                        mater.DescrProduto = " " + mpb.DescrBenef +
                            Utils.MontaDescrLapBis(mpb.BisAlt, mpb.BisLarg, mpb.LapAlt, mpb.LapLarg, mpb.EspBisote, null, null, false);

                        lstMaterRetorno.Add(mater);
                    }
                }
                else
                {
                    if (lstBenef.Count > 0)
                    {
                        MaterialItemProjeto mater = new MaterialItemProjeto();

                        // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                        foreach (MaterialProjetoBenef mpb in lstBenef)
                        {
                            mater.IdItemProjeto = mip.IdItemProjeto;
                            mater.Ambiente = mip.Ambiente;
                            mater.Qtde = 0;
                            mater.Valor += mpb.Valor / (mpb.Qtd > 0 ? mpb.Qtd : 1);
                            mater.Total += mpb.Valor;
                            string textoQuantidade = (mpb.TipoCalculoBenef == (int)TipoCalculoBenef.Quantidade) ? mpb.Qtd.ToString() + " " : "";
                            mater.DescrProduto += "; " + textoQuantidade + mpb.DescrBenef +
                                Utils.MontaDescrLapBis(mpb.BisAlt, mpb.BisLarg, mpb.LapAlt, mpb.LapLarg, mpb.EspBisote, null, null, false);
                        }

                        mater.DescrProduto = " " + mater.DescrProduto.Substring(2);
                        lstMaterRetorno.Add(mater);
                    }
                }
            }

            return lstMaterRetorno;
        }

        /// <summary>
        /// Busca materiais para impressão de totais de itens do projeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public List<MaterialItemProjeto> GetForRptTotaisItens(uint idProjeto, uint idOrcamento, uint idPedido, uint idPedidoEspelho)
        {
            string sql = @"
                Select mip.*, Cast(Sum(mip.Qtde) as signed) as SumQtde, cast(Sum(mip.Custo) as decimal(12,2)) as SumCusto, cast(Sum(mip.Total) as decimal(12,2)) as SumTotal,
                    Sum(if(p.IdGrupoProd=3, mip.Altura * mip.Qtde, mip.Altura)) as SumAltura, Sum(mip.TotM) as SumTotM, 
                    p.Descricao as DescrProduto, p.CodInterno, p.IdGrupoProd, gp.Descricao as DescrGrupoProd, p.idSubgrupoProd, 
                    if(p.AtivarAreaMinima=1, Cast(p.AreaMinima as char), '0') as AreaMinima, apl.CodInterno as CodAplicacao, 
                    prc.CodInterno as CodProcesso, mpm.qtde as qtdModelo, pp.codInterno as codMaterial 
                From material_item_projeto mip 
                    Left Join produto p On (mip.idProd=p.idProd) 
                    Left Join grupo_prod gp On (p.IdGrupoProd=gp.IdGrupoProd) 
                    Left Join material_projeto_modelo mpm On (mip.idMaterProjMod=mpm.idMaterProjMod) 
                    Left Join produto_projeto pp On (mpm.idProdProj=pp.idProdProj) 
                    Left Join etiqueta_aplicacao apl On (mip.idAplicacao=apl.idAplicacao) 
                    Left Join etiqueta_processo prc On (mip.idProcesso=prc.idProcesso) 
                Where mip.idItemProjeto in (Select IdItemProjeto From item_projeto Where ";

            if (idProjeto > 0)
                sql += "idProjeto=" + idProjeto;
            else if (idOrcamento > 0)
                sql += "idOrcamento=" + idOrcamento;
            else if (idPedido > 0)
                sql += "idPedido=" + idPedido;
            else if (idPedidoEspelho > 0)
                sql += "idPedidoEspelho=" + idPedidoEspelho;
                                                                                                     
            sql += ") Group By mip.IdProd";

            List<MaterialItemProjeto> lstMater = objPersistence.LoadData(sql);

            foreach (MaterialItemProjeto m in lstMater)
            {
                if (!Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)m.IdGrupoProd) && !Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio((int)m.IdGrupoProd))
                {
                    m.IdGrupoProd = 0;
                    m.DescrGrupoProd = "OUTROS";
                }
            }

            // Ordena a lista pelo nome do grupo e pelo nome dos itens
            lstMater.Sort(new Comparison<MaterialItemProjeto>(delegate(MaterialItemProjeto x, MaterialItemProjeto y)
            {
                int grupoProd = String.Compare(x.DescrGrupoProd, y.DescrGrupoProd);
                return grupoProd != 0 ? grupoProd : String.Compare(x.DescrProduto, y.DescrProduto);

            }));

            return lstMater;
        }

        /// <summary>
        /// Retorna o valor do projeto considerando o arredondamento das barras de alumínio
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <param name="cobrarTaxaPrazo">Identifica se no cálculo do total será incluído a taxa de juros de venda à prazo</param>
        /// <returns></returns>
        public decimal GetTotalProjetoBarra6m(uint idProjeto, bool cobrarTaxaPrazo)
        {
            string campos = "Cast(Sum(mip.Qtde) as signed) as SumQtde, Sum(if(p.IdGrupoProd=3, mip.Altura * mip.Qtde, mip.Altura)) as SumAltura, cast(Sum(mip.Total) as decimal(12,2)) as SumTotal, Sum(mip.TotM) as SumTotM";
            string sql = "Select mip.*, " + campos + ", p.IdGrupoProd " +
                "From material_item_projeto mip " +
                "Left Join produto p On (mip.idProd=p.idProd) " +
                "Where mip.idItemProjeto in (select IdItemProjeto from item_projeto where IdProjeto=" + idProjeto + ") " +
                "Group By mip.IdProd";

            List<MaterialItemProjeto> lstMater = objPersistence.LoadData(sql);
            decimal total = 0;

            foreach (MaterialItemProjeto m in lstMater)
            {
                total += m.Total;
            }

            return total;
        }

        #endregion

        #region Busca material relacionado à uma PecaItemProjeto

        /// <summary>
        /// Busca material relacionado à uma PecaItemProjeto
        /// </summary>
        /// <param name="idPecaItemProjeto"></param>
        /// <returns></returns>
        public MaterialItemProjeto GetMaterialByPeca(uint idPecaItemProj)
        {
            return GetMaterialByPeca(null, idPecaItemProj);
        }

        /// <summary>
        /// Busca material relacionado à uma PecaItemProjeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPecaItemProjeto"></param>
        /// <returns></returns>
        public MaterialItemProjeto GetMaterialByPeca(GDASession sessao, uint idPecaItemProj)
        {
            string sql = "Select * From material_item_projeto Where idPecaItemProj=" + idPecaItemProj;

            List<MaterialItemProjeto> lstMater = objPersistence.LoadData(sessao, sql);

            return lstMater.Count > 0 ? lstMater[0] : null;
        }

        #endregion

        #region Busca material para otimização de vidro

        public IList<MaterialItemProjeto> GetByVidroOrcamento(uint idOrcamento)
        {
            var filtroAdicional = string.Empty;
            var sql = Sql(0, 0, idOrcamento, true, out filtroAdicional);

            sql += " AND p.IdGrupoProd = 1";
            sql = sql.Substring(0, sql.IndexOf("Where", StringComparison.Ordinal));
            sql += " WHERE ip.idOrcamento=" + idOrcamento + " and mip.Altura > 0 and mip.Largura > 0 and mip.IdProd > 0";

            sql += @" and mip.idProd in (
                        select idProd from chapa_vidro
                        union all select pbe.idProd from produto_baixa_estoque pbe
                            inner join chapa_vidro c on (pbe.idProdBaixa=c.idProd)
                    ) ";

            var materialItemProjeto = objPersistence.LoadData(sql).ToList();

            return materialItemProjeto;
        }

        #endregion

        #region Retorna o ID da peça relacionada a um material

        /// <summary>
        /// Retorna o ID da peça relacionada a um material.
        /// </summary>
        /// <param name="idMaterItemProj"></param>
        /// <returns></returns>
        public uint? GetIdPecaByMaterial(uint idMaterItemProj)
        {
            return ExecuteScalar<uint?>("Select idPecaItemProj from material_item_projeto where idMaterItemProj=" + idMaterItemProj);
        }

        #endregion

        #region Cria os materiais do projeto

        /// <summary>
        /// Cria os materiais do item de projeto.
        /// </summary>
        public List<IMaterialItemProjeto> CriarMateriais(GDASession sessao, IItemProjeto itemProjeto, ProjetoModelo projetoModelo, List<PecaProjetoModelo> pecasProjetoModelo,
            IEnumerable<IPecaItemProjeto> pecasItemProjeto, IEnumerable<MedidaProjetoModelo> medidasProjetoModelo, IEnumerable<IMedidaItemProjeto> medidasItemProjeto,
            int? tipoEntrega, uint? idCliente)
        {
            // Verifica se é para cobrar o transpasse
            var isBoxPadrao = ProjetoModeloDAO.Instance.IsBoxPadrao(sessao, projetoModelo.IdProjetoModelo);
            var cobrarTranspasse = ProjetoConfig.CobrarTranspasse || isBoxPadrao;

            var materiaisItemProjeto = new List<IMaterialItemProjeto>();

            // Insere as peças de vidro, tirando a diferença de projeto das peças
            for (var i = 0; i < pecasProjetoModelo.Count; i++)
            {
                var prod = ProdutoDAO.Instance.GetByIdProd(sessao, pecasProjetoModelo[i].IdProd);

                if (prod == null && pecasProjetoModelo[i].IdProd > 0)
                    throw new Exception("A peça de vidro está associada a um produto inativo. Informe outro produto, calcule as medidas e confirme o projeto.");
                
                // Verifica se há fórmula para calcular a qtd de peças
                var qtdPeca = !string.IsNullOrEmpty(pecasProjetoModelo[i].CalculoQtde) && !itemProjeto.MedidaExata ?
                    (int)UtilsProjeto.CalcExpressao(sessao, pecasProjetoModelo[i].CalculoQtde, itemProjeto, pecasItemProjeto, medidasProjetoModelo, medidasItemProjeto, null) :
                    pecasProjetoModelo[i].Qtde;

                var alturaPeca = pecasProjetoModelo[i].Altura;
                var larguraPeca = pecasProjetoModelo[i].Largura;

                if (qtdPeca <= 0 || (alturaPeca == 0 && larguraPeca == 0))
                    continue;

                /* Chamado 63058. */
                if (pecasProjetoModelo[i].IdProd == 0)
                    continue;

                var incrementoAltura = 0;
                var incrementoLargura = 0;

                if (!ProjetoConfig.UsarMedidaExataProjeto && !itemProjeto.MedidaExata)
                {
                    var ppm = PecaProjetoModeloDAO.Instance.GetByCliente(sessao, pecasProjetoModelo[i].IdPecaProjMod, idCliente.GetValueOrDefault());

                    var alturaPecaPadrao =
                        itemProjeto.MedidaExata ?
                            0 : (ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                                (itemProjeto.EspessuraVidro == 3 ? ppm.Altura03MM :
                                itemProjeto.EspessuraVidro == 4 ? ppm.Altura04MM :
                                itemProjeto.EspessuraVidro == 5 ? ppm.Altura05MM :
                                itemProjeto.EspessuraVidro == 6 ? ppm.Altura06MM :
                                itemProjeto.EspessuraVidro == 8 ? ppm.Altura08MM :
                                itemProjeto.EspessuraVidro == 10 ? ppm.Altura10MM :
                                itemProjeto.EspessuraVidro == 12 ? ppm.Altura12MM : 0) : ppm.Altura);

                    var larguraPecaPadrao =
                        itemProjeto.MedidaExata ?
                            0 : (ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                                (itemProjeto.EspessuraVidro == 3 ? ppm.Largura03MM :
                                itemProjeto.EspessuraVidro == 4 ? ppm.Largura04MM :
                                itemProjeto.EspessuraVidro == 5 ? ppm.Largura05MM :
                                itemProjeto.EspessuraVidro == 6 ? ppm.Largura06MM :
                                itemProjeto.EspessuraVidro == 8 ? ppm.Largura08MM :
                                itemProjeto.EspessuraVidro == 10 ? ppm.Largura10MM :
                                itemProjeto.EspessuraVidro == 12 ? ppm.Largura12MM : 0) : ppm.Largura);

                    incrementoLargura += (larguraPecaPadrao * -1);
                    incrementoAltura += (alturaPecaPadrao * -1);

                    // Verifica se é para cobrar o transpasse
                    /* Chamado 15858.
                     * Foi solicitado que a folga de largura do projeto VERSA01-B fosse acrescentada à peça,
                     * O Reinaldo confirmou esta alteração e disse que realmente a folga deve ser acrescentada para este projeto e,
                     * também, para todos os projetos que possuírem folga de caixilho configurada. */
                    if (cobrarTranspasse && larguraPecaPadrao > 10)
                        incrementoLargura += larguraPecaPadrao;
                }

                var material = new MaterialItemProjeto();
                material.IdProd = (uint)prod.IdProd;

                material.IdAplicacao = pecasProjetoModelo[i].IdAplicacao > 0 ? pecasProjetoModelo[i].IdAplicacao :
                    pecasProjetoModelo[i].Tipo == 1 ? InstalacaoConfig.AplicacaoInstalacao :
                    pecasProjetoModelo[i].Tipo == 2 ? ProjetoConfig.Caixilho.AplicacaoCaixilho : null;

                material.IdProcesso = pecasProjetoModelo[i].IdProcesso > 0 ? pecasProjetoModelo[i].IdProcesso :
                    pecasProjetoModelo[i].Tipo == 1 ? InstalacaoConfig.ProcessoInstalacao :
                    pecasProjetoModelo[i].Tipo == 2 ? ProjetoConfig.Caixilho.ProcessoCaixilho : null;

                // Antes estava verificando se o valor inserido é maior que o valor de tabela, se fosse mantinha-o, porém
                // precisou ser mudado porque ao alterar a peça de vidro em "Medidas das Peças", o valor não era alterado
                material.Valor = ProdutoDAO.Instance.GetValorTabela(sessao, prod.IdProd, tipoEntrega, idCliente, false, false, 0, null, null, null);
                
                material.Largura = pecasProjetoModelo[i].Largura;
                material.Altura = pecasProjetoModelo[i].Altura;
                material.Qtde = qtdPeca;
                material.AlturaCalc = alturaPeca;
                material.Espessura = prod.Espessura;
                material.Obs = pecasProjetoModelo[i].Obs;
                material.Redondo = pecasProjetoModelo[i].Redondo;
                material.Item = pecasProjetoModelo[i].Item;

                DescontoAcrescimo.Instance.CalculaValorBruto(sessao, material);

                CalcTotais(sessao, ref material, false);

                materiaisItemProjeto.Add(material);
            }

            return materiaisItemProjeto;
        }

        #endregion

        #region Insere Pecas de Vidro

        /// <summary>
        /// Insere peças de vidro calculadas no item projeto
        /// </summary>
        /// <param name="idObra"></param>
        /// <param name="idCliente"></param>
        /// <param name="tipoEntrega"></param>
        /// <param name="itemProjeto"></param>
        /// <param name="projModelo"></param>
        /// <param name="lstPeca">Peças com os valores calculados e com o codInterno do produto</param>
        public void InserePecasVidro(uint? idObra, uint? idCliente, int? tipoEntrega, ItemProjeto itemProjeto, 
            ProjetoModelo projModelo, List<PecaProjetoModelo> lstPeca)
        {
            InserePecasVidro(null, idObra, idCliente, tipoEntrega, itemProjeto, projModelo, lstPeca);
        }

        /// <summary>
        /// Insere peças de vidro calculadas no item projeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idObra"></param>
        /// <param name="idCliente"></param>
        /// <param name="tipoEntrega"></param>
        /// <param name="itemProjeto"></param>
        /// <param name="projModelo"></param>
        /// <param name="lstPeca">Peças com os valores calculados e com o codInterno do produto</param>
        public void InserePecasVidro(GDASession sessao, uint? idObra, uint? idCliente, int? tipoEntrega, ItemProjeto itemProjeto,
            ProjetoModelo projModelo, List<PecaProjetoModelo> lstPeca)
        {
            InserePecasVidro(sessao, idObra, idCliente, tipoEntrega, itemProjeto, projModelo, lstPeca, false);
        }  

        /// <summary>
        /// Insere peças de vidro calculadas no item projeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idObra"></param>
        /// <param name="idCliente"></param>
        /// <param name="tipoEntrega"></param>
        /// <param name="itemProjeto"></param>
        /// <param name="projModelo"></param>
        /// <param name="lstPeca">Peças com os valores calculados e com o codInterno do produto</param>
        /// <param name="usarMedidaPecaMedidaExata"></param>
        internal void InserePecasVidro(GDASession sessao, uint? idObra, uint? idCliente, int? tipoEntrega, ItemProjeto itemProjeto,
            ProjetoModelo projModelo, List<PecaProjetoModelo> lstPeca, bool usarMedidaPecaMedidaExata)
        {
            bool pedidoMaoObraEspecial = false;
            bool isNfe = itemProjeto.IdPedido.GetValueOrDefault(0) == 0 && itemProjeto.IdPedidoEspelho.GetValueOrDefault(0) == 0 &&
                itemProjeto.IdOrcamento.GetValueOrDefault(0) == 0 && itemProjeto.IdProjeto.GetValueOrDefault(0) == 0;

            uint idPedido = itemProjeto.IdPedido ?? itemProjeto.IdPedidoEspelho ?? 0;
            if (idPedido > 0)
                pedidoMaoObraEspecial = PedidoDAO.Instance.GetTipoPedido(sessao, idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial;

            // Verifica se as peças de vidros deverão ser inseridas, é criada outra variável para manter o valor original,
            // para o caso de uma das peças não ter sido inserida e ter que alterar o valor para true momentaneamente.
            bool inserirMateriaisOrig = objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From material_item_projeto Where idpecaitemproj is not null And idItemProjeto=" + itemProjeto.IdItemProjeto) == 0;
            bool inserirMateriais = inserirMateriaisOrig;

            // Busca as peças do modelo utilizado neste item_projeto
            List<PecaProjetoModelo> lstPecaPadrao = PecaProjetoModeloDAO.Instance.GetByModelo(sessao, itemProjeto.IdProjetoModelo);

            // Busca os materiais deste item_projeto
            List<MaterialItemProjeto> lstMaterItemProj = GetByItemProjeto(sessao, itemProjeto.IdItemProjeto, true);

            // Recupera as peças salvas no item de projeto
            List<PecaItemProjeto> pecas = usarMedidaPecaMedidaExata ?
                PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProjeto.IdItemProjeto, projModelo.IdProjetoModelo) : new List<PecaItemProjeto>();

            // Verifica se é para cobrar o transpasse
            bool isBoxPadrao = ProjetoModeloDAO.Instance.IsBoxPadrao(sessao, projModelo.IdProjetoModelo);
            bool cobrarTranspasse = ProjetoConfig.CobrarTranspasse || isBoxPadrao;

            // Insere as peças de vidro, tirando a diferença de projeto das peças
            for (int i = 0; i < lstPeca.Count; i++)
            {
                // Reestabelece o valor original desta variável, necessário quando umas das peças tiver sido inserida e a outra não
                inserirMateriais = inserirMateriaisOrig;

                var prod = ProdutoDAO.Instance.GetByIdProd(sessao, lstPeca[i].IdProd);

                if (prod == null && lstPeca[i].IdProd > 0)
                    throw new Exception("A peça de vidro está associada a um produto inativo. Informe outro produto, calcule as medidas e confirme o projeto.");

                #region Atualiza a loja do projeto gerado pelo parceiros (a loja do projeto ficará zerada somente no parceiros)

                /* Chamado 48322. */
                if (itemProjeto.IdProjeto > 0 && prod != null)
                {
                    var idLojaProjeto = ProjetoDAO.Instance.ObterIdLoja(sessao, (int)itemProjeto.IdProjeto);

                    var idLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdLoja(sessao, prod.IdSubgrupoProd.Value);

                    if (idLojaProjeto > 0 && idLojaSubgrupoProd > 0 && idLojaProjeto != idLojaSubgrupoProd)
                        throw new Exception(string.Format("O produto {0} não pode ser utilizado, pois, a loja do seu subgrupo é diferente da loja do projeto.", prod.Descricao));
                    /* Chamado 48322. */
                    else if (idLojaProjeto == 0 && idLojaSubgrupoProd > 0)
                        ProjetoDAO.Instance.AtualizarIdLojaProjeto(sessao, (int)itemProjeto.IdProjeto, (int)idLojaSubgrupoProd);
                }

                #endregion
                
                // Verifica se há fórmula para calcular a qtd de peças
                var qtdPeca = !string.IsNullOrEmpty(lstPeca[i].CalculoQtde) && !itemProjeto.MedidaExata ? (int)UtilsProjeto.CalcExpressao(sessao, lstPeca[i].CalculoQtde, itemProjeto) : lstPeca[i].Qtde;

                /* Chamado 22322. */
                if (!PCPConfig.CriarClone &&
                    !itemProjeto.MedidaExata &&
                    !string.IsNullOrEmpty(lstPeca[i].CalculoQtde) &&
                    itemProjeto.IdPedidoEspelho.GetValueOrDefault() > 0)
                    qtdPeca = lstPeca[i].Qtde;
                
                var alturaPeca = lstPeca[i].Altura;
                var larguraPeca = lstPeca[i].Largura;

                MaterialItemProjeto materialAtual = lstMaterItemProj.Find(m => m.IdPecaItemProj == lstPeca[i].IdPecaItemProj);

                if (qtdPeca <= 0 || (alturaPeca == 0 && larguraPeca == 0))
                {
                    /* Chamado 49118. */
                    if (materialAtual != null)
                    {
                        #region Apaga os materiais

                        objPersistence.ExecuteCommand(sessao, "DELETE FROM material_projeto_benef WHERE IdMaterItemProj=?idMaterItemProj",
                            new GDAParameter("?idMaterItemProj", materialAtual.IdMaterItemProj));

                        objPersistence.ExecuteCommand(sessao, "DELETE FROM material_item_projeto WHERE IdMaterItemProj=?idMaterItemProj",
                            new GDAParameter("?idMaterItemProj", materialAtual.IdMaterItemProj));

                        #endregion

                        #region Apaga os produtos de pedido

                        objPersistence.ExecuteCommand(sessao, "DELETE FROM produto_pedido_benef WHERE IdProdPed IN (SELECT IdProdPed FROM produtos_pedido WHERE IdMaterItemProj=?idMaterItemProj)",
                            new GDAParameter("?idMaterItemProj", materialAtual.IdMaterItemProj));

                        objPersistence.ExecuteCommand(sessao, "DELETE FROM produtos_pedido WHERE IdMaterItemProj=?idMaterItemProj",
                            new GDAParameter("?idMaterItemProj", materialAtual.IdMaterItemProj));

                        #endregion

                        #region Apaga os produtos de pedido PCP

                        objPersistence.ExecuteCommand(sessao, "DELETE FROM produto_pedido_espelho_benef WHERE IdProdPed IN (SELECT IdProdPed FROM produtos_pedido_espelho WHERE IdMaterItemProj=?idMaterItemProj)",
                            new GDAParameter("?idMaterItemProj", materialAtual.IdMaterItemProj));

                        objPersistence.ExecuteCommand(sessao, "DELETE FROM produtos_pedido_espelho WHERE IdMaterItemProj=?idMaterItemProj",
                            new GDAParameter("?idMaterItemProj", materialAtual.IdMaterItemProj));

                        #endregion
                    }

                    continue;
                }

                /* Chamado 63058. */
                if (lstPeca[i].IdProd == 0)
                    continue;

                var incrementoAltura = 0;
                var incrementoLargura = 0;

                if (!ProjetoConfig.UsarMedidaExataProjeto && !itemProjeto.MedidaExata)
                {
                    var ppm = PecaProjetoModeloDAO.Instance.GetByCliente(sessao, lstPecaPadrao[i].IdPecaProjMod, itemProjeto.IdCliente.GetValueOrDefault());

                    var alturaPecaPadrao =
                        itemProjeto.MedidaExata ?
                            0 : (ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                                (itemProjeto.EspessuraVidro == 3 ? ppm.Altura03MM :
                                itemProjeto.EspessuraVidro == 4 ? ppm.Altura04MM :
                                itemProjeto.EspessuraVidro == 5 ? ppm.Altura05MM :
                                itemProjeto.EspessuraVidro == 6 ? ppm.Altura06MM :
                                itemProjeto.EspessuraVidro == 8 ? ppm.Altura08MM :
                                itemProjeto.EspessuraVidro == 10 ? ppm.Altura10MM :
                                itemProjeto.EspessuraVidro == 12 ? ppm.Altura12MM : 0) : ppm.Altura);

                    var larguraPecaPadrao =
                        itemProjeto.MedidaExata ?
                            0 : (ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                                (itemProjeto.EspessuraVidro == 3 ? ppm.Largura03MM :
                                itemProjeto.EspessuraVidro == 4 ? ppm.Largura04MM :
                                itemProjeto.EspessuraVidro == 5 ? ppm.Largura05MM :
                                itemProjeto.EspessuraVidro == 6 ? ppm.Largura06MM :
                                itemProjeto.EspessuraVidro == 8 ? ppm.Largura08MM :
                                itemProjeto.EspessuraVidro == 10 ? ppm.Largura10MM :
                                itemProjeto.EspessuraVidro == 12 ? ppm.Largura12MM : 0) : ppm.Largura);

                    incrementoLargura += (larguraPecaPadrao * -1);
                    incrementoAltura += (alturaPecaPadrao * -1);

                    // Verifica se é para cobrar o transpasse
                    /* Chamado 15858.
                     * Foi solicitado que a folga de largura do projeto VERSA01-B fosse acrescentada à peça,
                     * O Reinaldo confirmou esta alteração e disse que realmente a folga deve ser acrescentada para este projeto e,
                     * também, para todos os projetos que possuírem folga de caixilho configurada. */
                    if (cobrarTranspasse && larguraPecaPadrao > 10)
                        incrementoLargura += larguraPecaPadrao;
                }

                MaterialItemProjeto material = new MaterialItemProjeto();
                material.IdItemProjeto = itemProjeto.IdItemProjeto;
                material.IdPecaItemProj = lstPeca[i].IdPecaItemProj > 0 ? (uint?)lstPeca[i].IdPecaItemProj : null;

                // Recupera o id deste material sendo atualizado
                if (!inserirMateriais)
                {
                    object idMater = objPersistence.ExecuteScalar(sessao, @"
                        Select idMaterItemProj From material_item_projeto Where idPecaItemProj=" + material.IdPecaItemProj);

                    if (idMater != null)
                    {
                        material.IdMaterItemProj = Glass.Conversoes.StrParaUint(idMater.ToString());
                        material = MaterialItemProjetoDAO.Instance.GetElementByPrimaryKey(sessao, material.IdMaterItemProj);
                    }
                    else
                        inserirMateriais = true;
                }

                material.IdProd = (uint)prod.IdProd;

                // Recupera a peça da lista se for medida exata e o parâmetro indicar o cálculo ou se for nota fiscal
                PecaItemProjeto peca = (usarMedidaPecaMedidaExata && itemProjeto.MedidaExata) || isNfe ?
                    pecas.Find(x => x.IdPecaProjMod == lstPeca[i].IdPecaProjMod) : null;

                uint? idAplicacao =
                    materialAtual == null || materialAtual.IdAplicacao.GetValueOrDefault() == 0 ?
                        (lstPeca[i].IdAplicacao ??
                        ((peca != null ? peca.Tipo : lstPeca[i].Tipo) == 1 ?
                        InstalacaoConfig.AplicacaoInstalacao :
                        lstPeca[i].Tipo == 2 ? ProjetoConfig.Caixilho.AplicacaoCaixilho : null)) :
                    materialAtual.IdAplicacao;

                uint? idProcesso =
                    materialAtual == null || materialAtual.IdProcesso.GetValueOrDefault() == 0 ?
                        (lstPeca[i].IdProcesso ??
                        ((peca != null ? peca.Tipo : lstPeca[i].Tipo) == 1 ?
                        InstalacaoConfig.ProcessoInstalacao :
                        lstPeca[i].Tipo == 2 ? ProjetoConfig.Caixilho.ProcessoCaixilho : null)) :
                    materialAtual.IdProcesso;

                var aplicacaoAlteradaUsuario =
                    material.IdAplicacao == null && material.IdProcesso == null ? false :
                    ((peca != null && peca.Tipo == 2) || (lstPeca[i].Tipo == 2)) && material.IdAplicacao == ProjetoConfig.Caixilho.AplicacaoCaixilho ? false :
                    ((peca != null && peca.Tipo == 1) || (lstPeca[i].Tipo == 1)) && material.IdAplicacao == InstalacaoConfig.AplicacaoInstalacao ? false :
                    material.IdAplicacao == ProjetoConfig.Caixilho.AplicacaoCaixilho || material.IdAplicacao == InstalacaoConfig.AplicacaoInstalacao ? false : true;

                var processoAlteradoUsuario =
                    material.IdProcesso == null && material.IdAplicacao == null ? false :
                    ((peca != null && peca.Tipo == 2) || (lstPeca[i].Tipo == 2)) && material.IdProcesso == ProjetoConfig.Caixilho.ProcessoCaixilho ? false :
                    ((peca != null && peca.Tipo == 1) || (lstPeca[i].Tipo == 1)) && material.IdProcesso == InstalacaoConfig.ProcessoInstalacao ? false :
                    material.IdProcesso == ProjetoConfig.Caixilho.ProcessoCaixilho || material.IdProcesso == InstalacaoConfig.ProcessoInstalacao ? false : true;

                if (objPersistence.ExecuteSqlQueryCount("SELECT COUNT(*) FROM Classificacao_Roteiro_Producao", null) > 0)
                {
                    if (idProcesso == null || !ClassificacaoSubgrupoDAO.Instance.VerificarAssociacaoExistente(prod.IdSubgrupoProd.GetValueOrDefault(), (int)idProcesso))
                    {
                        idAplicacao = null;
                        idProcesso = null;
                        material.IdAplicacao = null;
                        material.IdProcesso = null;
                    }
                }

                /* Chamado 45441. */
                if (!aplicacaoAlteradaUsuario && !processoAlteradoUsuario)
                {
                    material.IdAplicacao = idAplicacao;
                    material.IdProcesso = idProcesso;
                }

                // Antes estava verificando se o valor inserido é maior que o valor de tabela, se fosse mantinha-o, porém
                // precisou ser mudado porque ao alterar a peça de vidro em "Medidas das Peças", o valor não era alterado
                ProdutoObraDAO.DadosProdutoObra dadosObra = idObra > 0 ? ProdutoObraDAO.Instance.IsProdutoObra(sessao, idObra.Value, prod.CodInterno) : null;
                decimal valorTabela = dadosObra != null && dadosObra.ProdutoValido ? dadosObra.ValorUnitProduto :
                    ProdutoDAO.Instance.GetValorTabela(sessao, prod.IdProd, tipoEntrega, idCliente, false, itemProjeto.Reposicao, 0,
                    (int?)itemProjeto.IdPedido, (int?)itemProjeto.IdProjeto, (int?)itemProjeto.IdOrcamento);

                /* Chamado 53156. */
                /* Chamado 55446. */
                if (PedidoConfig.DadosPedido.AlterarValorUnitarioProduto)
                {
                    if (materialAtual != null && materialAtual.Valor > valorTabela && materialAtual.IdProd == material.IdProd)
                        material.Valor = materialAtual.Valor;
                    else if (materialAtual != null && materialAtual.IdProd != material.IdProd)
                        material.Valor = valorTabela;
                    else if (material.Valor > valorTabela)
                        material.Valor = material.Valor;
                    else
                        material.Valor = valorTabela;
                }
                else
                    material.Valor = valorTabela;

                if (peca == null)
                {
                    material.Largura = larguraPeca + incrementoLargura;
                    material.Altura = alturaPeca + incrementoAltura;
                }
                else
                {
                    material.Largura = peca.Largura;
                    material.Altura = peca.Altura;
                }

                material.Qtde = qtdPeca;
                material.AlturaCalc = alturaPeca;
                material.Espessura = prod.Espessura;
                material.Obs = materialAtual != null && !String.IsNullOrEmpty(materialAtual.Obs) &&
                    material.Obs.Trim().Length > 0 ? materialAtual.Obs : lstPeca[i].Obs;
                material.Redondo = materialAtual != null ? materialAtual.Redondo : lstPecaPadrao.Where(f => f.IdPecaProjMod == lstPeca[i].IdPecaProjMod).FirstOrDefault().Redondo;

                // O beneficiamento não deve ser mantido, caso a quantidade, altura ou largura da peça tenham sido alterados o que pode
                // fazer com que o valor do beneficiamento tenha que ser recalculado, o que não acontece quando o projeto é recalculado
                if (materialAtual != null && materialAtual.Qtde == material.Qtde && materialAtual.Altura == material.Altura && materialAtual.Largura == material.Largura &&
                    material.IdProd == materialAtual.IdProd)
                    material.Beneficiamentos = materialAtual.Beneficiamentos;
                /* Chamado 57058. */
                else if (materialAtual == null || material.IdProd != materialAtual.IdProd)
                {
                    material.Beneficiamentos = lstPeca[i].Beneficiamentos;

                    if (material.Beneficiamentos == null || material.Beneficiamentos.Count == 0)
                        material.Beneficiamentos = lstPecaPadrao[i].Beneficiamentos;
                }

                DescontoAcrescimo.Instance.CalculaValorBruto(sessao, material);

                try
                {
                    if (inserirMateriais)
                        InsertFromNovoItemProjeto(sessao, material, idCliente);
                    else
                        UpdateFromNovoItemProjeto(sessao, material, idCliente);
                }
                catch (Exception ex)
                {
                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao Confirmar Medidas.", ex));
                }
            }
        }

        #endregion

        #region Atualiza qtd dos materiais

        /// <summary>
        /// Atualiza as quantidades dos alumínios e das ferragens
        /// </summary>
        /// <param name="itemProj"></param>
        public void AtualizaQtd(uint? idObra, uint? idCliente, int? tipoEntrega, ItemProjeto itemProj, ProjetoModelo modelo)
        {
            AtualizaQtd(null, idObra, idCliente, tipoEntrega, itemProj, modelo);
        }

        /// <summary>
        /// Atualiza as quantidades dos alumínios e das ferragens
        /// </summary>
        /// <param name="itemProj"></param>
        public void AtualizaQtd(GDASession sessao, uint? idObra, uint? idCliente, int? tipoEntrega, ItemProjeto itemProj, ProjetoModelo modelo)
        {
            // Busca os materiais deste item_projeto
            List<MaterialItemProjeto> lstMaterItemProj = GetByItemProjeto(sessao, itemProj.IdItemProjeto);

            // Pega o id do produto referente ao material "escova"
            uint idProdEscova = ProdutoProjetoDAO.Instance.GetEscovaId(sessao);

            // Pega o id do produto referente ao material "mão de obra"
            uint idProdMaoDeObra = ProdutoProjetoDAO.Instance.GetMaoDeObraId(sessao);

            // Pega as medidas informadas para este item_projeto
            if (!MedidaItemProjetoDAO.Instance.ExistsMedida(sessao, itemProj.IdItemProjeto))
                throw new Exception("Informe todas as medidas da área de instalação.");

            bool pedidoMaoObraEspecial = false;

            uint idPedido = itemProj.IdPedido ?? itemProj.IdPedidoEspelho ?? 0;
            if (idPedido > 0)
                pedidoMaoObraEspecial = PedidoDAO.Instance.GetTipoPedido(sessao,idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial;

            // Faz o cálculo de qtd que os produtos devem ter de acordo com as medidas de instalação informadas
            foreach (MaterialItemProjeto m in lstMaterItemProj)
            {
                decimal valorMaterial = m.Valor;
                MaterialItemProjeto material = m;

                if (m.IdMaterProjMod != null)
                    UtilsProjeto.CalcMaterial(sessao, ref material, itemProj, modelo, idProdEscova, idProdMaoDeObra);
                
                // Verifica qual preço deverá ser utilizado
                ProdutoObraDAO.DadosProdutoObra dadosObra = idObra > 0 ? ProdutoObraDAO.Instance.IsProdutoObra(sessao, idObra.Value, m.IdProd) : null;
                decimal valorTabela = dadosObra != null && dadosObra.ProdutoValido ? dadosObra.ValorUnitProduto :
                    ProdutoDAO.Instance.GetValorTabela(sessao, (int)m.IdProd, tipoEntrega, idCliente, false, itemProj.Reposicao, 0, (int?)itemProj.IdPedido, (int?)itemProj.IdProjeto, (int?)itemProj.IdOrcamento);

                material.Valor = material.Valor > valorTabela ? material.Valor : valorTabela;

                CalcTotais(sessao, ref material, false);

                // Apenas material adiconado automaticamente ou que o valor unitário não seja permitido ter alteração ou
                // que o valor anterior seja menor que o valor atual será recalculado
                if (m.IdMaterProjMod == null && (PedidoConfig.DadosPedido.AlterarValorUnitarioProduto || 
                    valorMaterial > material.Valor))
                    continue;

                UpdateBase(sessao, material);
            }
        }

        #endregion

        #region Verifica se produto já foi inserido

        /// <summary>
        /// Verifica se o produto especificado já foi inserido nos materiais do item_projeto passado
        /// </summary>
        /// <param name="idMaterItemProj"></param>
        /// <param name="idProd"></param>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="idProcesso"></param>
        /// <param name="idAplicacao"></param>
        /// <returns></returns>
        public bool ExistsInItemProjeto(uint idItemProjeto, uint idProd, Single altura, int largura, uint? idProcesso, uint? idAplicacao)
        {
            string sql = "Select count(*) From material_item_projeto where idItemProjeto=" + idItemProjeto + " And idProd=" + idProd +
                " And altura=" + altura.ToString().Replace(',', '.') + " And largura=" + largura;

            return objPersistence.ExecuteSqlQueryCount(sql, null) > 0;
        }

        #endregion

        #region Verifica se existe um produto do grupo kit nos materiais do itemProjeto passado

        /// <summary>
        /// Verifica se existe um produto do grupo kit nos materiais do itemProjeto passado
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public bool ExistsKit(uint idItemProjeto)
        {
            string sql = "Select Count(*) From material_item_projeto m " +
                "Inner Join produto p On (m.idProd=p.idProd) Where p.idGrupoProd=" + (uint)Glass.Data.Model.NomeGrupoProd.KitParaBox +
                " And m.idItemProjeto=" + idItemProjeto;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString()) > 0;
        }

        #endregion

        #region Verifica se existe algum produto "Tubo" nos materiais do itemProjeto passado

        /// <summary>
        /// Verifica se existe algum produto "Tubo" nos materiais do itemProjeto passado
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public bool ExistsTubo(uint idItemProjeto)
        {
            string ids = ProdutoDAO.Instance.ObtemIds(null, "TUBO ");

            string sql = @"
                Select Count(*) 
                From material_item_projeto m 
                    Inner Join produto p On (m.idProd=p.idProd) 
                Where p.idProd In (" + ids + ") And m.idItemProjeto=" + idItemProjeto;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString()) > 0;
        }

        #endregion

        #region Verifica se todos os materiais são da mesma cor e espessura

        /// <summary>
        /// Verifica se todos os materiais são da mesma cor e espessura
        /// </summary>
        public bool VidrosMesmaCorEspessura(uint idOrcamento)
        {
            return VidrosMesmaCorEspessura(null, idOrcamento);
        }

        /// <summary>
        /// Verifica se todos os materiais são da mesma cor e espessura
        /// </summary>
        public bool VidrosMesmaCorEspessura(GDASession session, uint idOrcamento)
        {
            string sql = @"
                Select Count(*) From (
                    Select p.idCorVidro 
                    From material_item_projeto m 
                        Inner Join produto p On (m.idProd=p.idProd) 
                    Where m.idItemProjeto In (Select idItemProjeto From item_projeto Where idOrcamento=" + idOrcamento + @")
                        And p.idGrupoProd=" + (int)NomeGrupoProd.Vidro + @"
                    Group By p.idCorVidro, p.espessura
                ) as tbl";

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(session, sql).ToString()) <= 1;
        }

        #endregion

        #region Exclui todos os materiais do projeto/itemProjeto

        /// <summary>
        /// Exclui todos os materiais do projeto que não existirem no pedido
        /// </summary>
        /// <param name="idItemProjeto"></param>
        public void DeleteNotInPedido(uint idProjeto, uint idPedido)
        {
            string sql = "Delete From material_item_projeto Where idItemProjeto In " +
                "(Select idItemProjeto From item_projeto Where idProjeto=" + idProjeto + ") " +
                "And idItemProjeto Not In (Select idItemProjeto From produtos_pedido Where idPedido=" + idPedido + ")";

            objPersistence.ExecuteCommand(sql);
        }

        /// <summary>
        /// Exclui todos os materiais do projeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        public void DeleteByProjeto(uint idProjeto)
        {
            string sql = "Delete From material_item_projeto Where idItemProjeto In " + 
                "(Select idItemProjeto From item_projeto Where idProjeto=" + idProjeto + ")";

            objPersistence.ExecuteCommand(sql);
        }

        /// <summary>
        /// Exclui todos os materiais do itemProjeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        public void DeleteByItemProjeto(uint idItemProjeto)
        {
            DeleteByItemProjeto(null, idItemProjeto);
        }

        /// <summary>
        /// Exclui todos os materiais do itemProjeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        public void DeleteByItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            string sql = "Delete From material_item_projeto Where idItemProjeto=" + idItemProjeto;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Busca para compra

        private string SqlCompra(uint idPedido, uint idItemProjeto, bool selecionar)
        {
            string sqlQtdeComprada = @"
                select cast(coalesce(sum(qtde), 0) as signed integer) 
                from produtos_compra pc 
                    left join compra c on (pc.idCompra=c.idCompra)
                where c.idPedidoEspelho=ped.idPedido and idMaterItemProj=mip.idMaterItemProj
                    and c.situacao<>" + (int)Compra.SituacaoEnum.Cancelada + @"
                    and (pc.NaoCobrarVidro=false or pc.NaoCobrarVidro is null)";

            string sqlQtdeBenefMaterItemProj = @"
                select coalesce(sum(contagem) * mip.qtde, 0) as contagem
                from (
        	        select mpb.idMaterItemProj, count(*) as contagem
                    from material_projeto_benef mpb
                    group by mpb.idMaterItemProj
                ) as temp
                where temp.idMaterItemProj=mip.idMaterItemProj";

            string sqlQtdeBenefCompra = @"
                select coalesce(sum(contagem), 0) 
                from (" +
                    ProdutosCompraBenefDAO.Instance.SqlProdPedBenef(0, 0, 0, 0, false) + @"
                ) as temp 
                where temp.idMaterItemProj=mip.idMaterItemProj";

            string campos = @"mip.*, p.Descricao as DescrProduto, p.CodInterno, p.IdGrupoProd, p.idSubgrupoProd, 
                if(p.AtivarAreaMinima=1, Cast(p.AreaMinima as char), '0') as AreaMinima, apl.CodInterno as CodAplicacao, 
                prc.CodInterno as CodProcesso, mpm.qtde as qtdModelo, pp.codInterno as codMaterial, mpm.CalculoQtde,
                mpm.CalculoAltura, ip.Ambiente as Ambiente, (" + sqlQtdeComprada + ") as QtdeComprada, (" + sqlQtdeBenefCompra + ") as QtdeBenefCompra, " +
                "(" + sqlQtdeBenefMaterItemProj + ") as QtdeBenefMaterItemProj";

            string sql = @"
                Select " + campos + @"
                From material_item_projeto mip
                    Left Join item_projeto ip on (mip.idItemProjeto=ip.idItemProjeto)
                    Left Join produto p On (mip.idProd=p.idProd)
                    Left Join material_projeto_modelo mpm On (mip.idMaterProjMod=mpm.idMaterProjMod)
                    Left Join produto_projeto pp On (mpm.idProdProj=pp.idProdProj)
                    Left Join etiqueta_aplicacao apl On (mip.idAplicacao=apl.idAplicacao)
                    Left Join etiqueta_processo prc On (mip.idProcesso=prc.idProcesso)
                    Left join pedido ped on (ip.idProjeto=ped.idProjeto)
                Where mip.idItemProjeto=" + idItemProjeto + @"
                    " + (idPedido > 0 ? "and ped.idPedido=" + idPedido : "") + @"
                Having QtdeComprada < mip.qtde
                    or QtdeBenefCompra < QtdeBenefMaterItemProj
	                or (
    	                QtdeBenefMaterItemProj = 0
		                and QtdeComprada < mip.qtde
                    )
                Order By ip.Ambiente, p.CodInterno";

            if (!selecionar)
                sql = "select count(*) from (" + sql + ") as temp";

            return sql;
        }

        public int GetCountForCompra(uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlCompra(idPedido, 0, false));
        }

        public IList<MaterialItemProjeto> GetForCompra(uint idItemProjeto)
        {
            return GetForCompra(idItemProjeto, false);
        }

        public IList<MaterialItemProjeto> GetForCompra(uint idItemProjeto, bool forRpt)
        {
            var materiais = objPersistence.LoadData(SqlCompra(0, idItemProjeto, true)).ToList();
            
            if (forRpt)
            {
                var mip = new List<MaterialItemProjeto>();
                foreach (MaterialItemProjeto m in materiais)
                {
                    if (m.Redondo && !m.DescrProduto.ToLower().Contains("redondo"))
                        m.DescrProduto += " REDONDO";

                    mip.Add(m);

                    foreach (MaterialProjetoBenef b in MaterialProjetoBenefDAO.Instance.GetByMaterial(null, m.IdMaterItemProj))
                    {
                        MaterialItemProjeto benef = new MaterialItemProjeto();
                        benef.DescrProduto = " " + b.DescrBenef;
                        benef.Qtde = b.Qtd > 0 ? b.Qtd : 1;
                        benef.QtdeComprada = ProdutosCompraBenefDAO.Instance.GetCountByMaterItemProjBenef(m.IdMaterItemProj, b.IdBenefConfig);

                        if (benef.QtdeComprar > 0)
                            mip.Add(benef);
                    }
                }

                return mip;
            }
            else
                return materiais;
        }

        #endregion

        #region Soma os valores dos materiais por item de projeto

        public float SumByItemProjeto(uint idItemProjeto)
        {
            return float.Parse(objPersistence.ExecuteScalar("select coalesce(sum(coalesce(Total, 0)), 0) from material_item_projeto where idItemProjeto=" + idItemProjeto).ToString());
        }

        #endregion

        #region Obtém o idProjeto do material passado

        /// <summary>
        /// Obtém o idProjeto do material passado
        /// </summary>
        /// <param name="idMaterItemProj"></param>
        public uint GetIdItemProjeto(uint idMaterItemProj)
        {
            string sql = "Select idItemProjeto From material_item_projeto Where idMaterItemProj=" + idMaterItemProj;

            object obj = objPersistence.ExecuteScalar(sql);

            return obj != null && obj.ToString() != String.Empty ? Glass.Conversoes.StrParaUint(obj.ToString()) : 0;
        }

        #endregion

        #region Obtem dados de um item

        public uint? ObtemIdPecaItemProj(GDASession sessao, uint idMaterItemProj)
        {
            return ObtemValorCampo<uint?>(sessao, "idPecaItemProj", "idMaterItemProj=" + idMaterItemProj);
        }

        public int ObtemLargura(GDASession sessao, uint idMaterItemProj)
        {
            return ObtemValorCampo<int>(sessao, "largura", "idMaterItemProj=" + idMaterItemProj);
        }

        public float ObtemAlturaCalc(GDASession sessao, uint idMaterItemProj)
        {
            return ObtemValorCampo<float>(sessao, "alturaCalc", "idMaterItemProj=" + idMaterItemProj);
        }

        public float ObtemAltura(GDASession sessao, uint idMaterItemProj)
        {
            return ObtemValorCampo<float>(sessao, "altura", "idMaterItemProj=" + idMaterItemProj);
        }

        public float ObtemTotM(GDASession sessao, uint idMaterItemProj)
        {
            return ObtemValorCampo<float>(sessao, "totM", "idMaterItemProj=" + idMaterItemProj);
        }

        public float ObtemTotM2Calc(GDASession sessao, uint idMaterItemProj)
        {
            return ObtemValorCampo<float>(sessao, "totM2Calc", "idMaterItemProj=" + idMaterItemProj);
        }

        public bool ObtemRedondo(GDASession sessao, uint idMaterItemProj)
        {
            return ObtemValorCampo<bool>(sessao, "redondo", "idMaterItemProj=" + idMaterItemProj);
        }

        public bool ObtemIdMaterProjMod(GDASession sessao, uint idMaterItemProj)
        {
            return ObtemValorCampo<bool>(sessao, "idMaterProjMod", "idMaterItemProj=" + idMaterItemProj);
        }

        public uint ObtemIdItemProjeto(uint idMaterItemProj)
        {
            return ObtemValorCampo<uint>("idItemProjeto", "idMaterItemProj=" + idMaterItemProj);
        }

        public decimal ObtemValor(uint idMaterItemProj)
        {
            return ObtemValorCampo<decimal>("valor", "idMaterItemProj=" + idMaterItemProj);
        }

        public uint ObtemidPecaItemProj(uint idMaterItemProj)
        {
            return ObtemidPecaItemProj(null, idMaterItemProj);
        }

        public uint ObtemidPecaItemProj(GDASession sessao, uint idMaterItemProj)
        {
            return ObtemValorCampo<uint>(sessao, "idPecaItemProj", "IdMaterItemProj=" + idMaterItemProj);
        }

        #endregion

        #region Material original (para pedido espelho)

        /// <summary>
        /// Altera o id do material original.
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="idMaterItemProj"></param>
        public void SetIdMaterItemProjOrig(GDASession sessao, uint? valor, uint idMaterItemProj)
        {
            string sql = "update material_item_projeto set idMaterItemProjOrig=?valor where idMaterItemProj=" + idMaterItemProj;
            objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?valor", valor));
        }

        /// <summary>
        /// Obtem o id através do id material original.
        /// </summary>
        /// <param name="idMaterItemProj"></param>
        /// <returns></returns>
        public uint? ObtemIdByIdMaterItemProjOrig(uint idMaterItemProjOrig)
        {
            return ObtemValorCampo<uint?>("idMaterItemProj", "idMaterItemProjOrig=" + idMaterItemProjOrig);
        }

        /// <summary>
        /// Obtem o id do material original.
        /// </summary>
        /// <param name="idMaterItemProj"></param>
        /// <returns></returns>
        public uint? ObtemIdMaterItemProjOrig(uint idMaterItemProj)
        {
            return ObtemValorCampo<uint?>("idMaterItemProjOrig", "idMaterItemProj=" + idMaterItemProj);
        }

        #endregion

        #region Recalcula os valores unitários e totais brutos e líquidos

        /// <summary>
        /// Recalcula os valores unitários e totais brutos e líquidos.
        /// </summary>
        /// <param name="idMaterItemProj"></param>
        public void RecalcularValores(uint idMaterItemProj)
        {
            MaterialItemProjeto prod = GetElementByPrimaryKey(idMaterItemProj);
            RecalcularValores(prod);
            UpdateBase(prod);
        }

        /// <summary>
        /// Recalcula os valores unitários e totais brutos e líquidos.
        /// </summary>
        /// <param name="prod"></param>
        public void RecalcularValores(MaterialItemProjeto prod)
        {
            uint? idCliente;
            int? tipoEntrega;
            bool reposicao;
            ItemProjetoDAO.Instance.GetTipoEntregaCliente(prod.IdItemProjeto, out tipoEntrega, out idCliente, out reposicao);

            RecalcularValores(prod, idCliente, tipoEntrega);
        }

        /// <summary>
        /// Recalcula os valores unitários e totais brutos e líquidos.
        /// </summary>
        public void RecalcularValores(MaterialItemProjeto prod, uint? idCliente, int? tipoEntrega)
        {
            RecalcularValores(null, prod, idCliente, tipoEntrega);
        }

        /// <summary>
        /// Recalcula os valores unitários e totais brutos e líquidos.
        /// </summary>
        public void RecalcularValores(GDASession session, MaterialItemProjeto prod, uint? idCliente, int? tipoEntrega)
        {
            GenericBenefCollection benef = prod.Beneficiamentos;
            decimal valorBenef = prod.ValorBenef;

            try
            {
                var itemProjeto = ItemProjetoDAO.Instance.GetElementByPrimaryKey(prod.IdItemProjeto);

                prod.Beneficiamentos = new GenericBenefCollection();
                prod.ValorBenef = 0;

                DescontoAcrescimo.Instance.CalculaValorBruto(session, prod);

                DescontoAcrescimo.Instance.RecalcularValorUnit(session, prod, idCliente, tipoEntrega, true,
                    benef.CountAreaMinimaSession(session) > 0, (int?)itemProjeto.IdPedido, (int?)itemProjeto.IdProjeto, (int?)itemProjeto.IdOrcamento);
                CalcTotais(session, ref prod, true);
            }
            finally
            {
                prod.Beneficiamentos = benef;
                prod.ValorBenef = valorBenef;
            }
        }

        #endregion

        #region Verificações da peça

        public bool IsRedondo(uint idMaterItemProj)
        {
            return ObtemValorCampo<bool>("redondo", "idMaterItemProj=" + idMaterItemProj);
        }

        /// <summary>
        /// Verifica se a peça do item de projeto possui um material associado.
        /// </summary>
        public bool PecaPossuiMaterial(int idPecaItemProj)
        {
            return objPersistence.ExecuteSqlQueryCount(
                string.Format("SELECT COUNT(*) FROM material_item_projeto WHERE IdPecaItemProj={0}", idPecaItemProj)) > 0;
        }

        #endregion

        #region Atualiza total do beneficiamento aplicado neste produto

        /// <summary>
        /// Atualiza os beneficiamentos de um produto.
        /// </summary>
        /// <param name="idMaterItemProj"></param>
        /// <param name="beneficiamentos"></param>
        public void AtualizaBenef(uint idMaterItemProj, GenericBenefCollection beneficiamentos)
        {
            AtualizaBenef(null, idMaterItemProj, beneficiamentos);
        }

        /// <summary>
        /// Atualiza os beneficiamentos de um produto.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idMaterItemProj"></param>
        /// <param name="beneficiamentos"></param>
        public void AtualizaBenef(GDASession sessao, uint idMaterItemProj, GenericBenefCollection beneficiamentos)
        {
            MaterialProjetoBenefDAO.Instance.DeleteByMaterial(sessao, idMaterItemProj);

            foreach (MaterialProjetoBenef b in beneficiamentos.ToMateriaisProjeto(idMaterItemProj))
                MaterialProjetoBenefDAO.Instance.Insert(sessao, b);

            UpdateValorBenef(sessao, idMaterItemProj);
        }

        #endregion

        #region Métodos sobrescritos

        private void CalcTotais(ref MaterialItemProjeto material, bool calcularAreaMinima)
        {
            CalcTotais(null, ref material, calcularAreaMinima);
        }

        public void CalcTotais(GDASession sessao, ref MaterialItemProjeto material, bool calcularAreaMinima)
        {
            CalcTotais(sessao, ref material, calcularAreaMinima, null);
        }

        public void CalcTotais(GDASession sessao, ref MaterialItemProjeto material, bool calcularAreaMinima, uint? idCliente)
        {
            idCliente = !idCliente.HasValue || idCliente.Value == 0 ? ItemProjetoDAO.Instance.ObtemIdCliente(sessao, material.IdItemProjeto) : idCliente;
            float totM2 = material.TotM, altura = material.Altura, totM2Calc = material.TotM2Calc;
            decimal total = material.Total, custoProd = 0;

            Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(sessao, idCliente.GetValueOrDefault(), (int)material.IdProd, material.Largura, material.Qtde, 1,
                material.Valor, material.Espessura, material.Redondo, 2, false, true, ref custoProd, ref altura, ref totM2, ref totM2Calc,
                ref total, false, material.Beneficiamentos.CountAreaMinimaSession(sessao), calcularAreaMinima);

            material.TotM = totM2;
            material.Total = total;
            material.Custo = custoProd;
            material.TotM2Calc = totM2Calc;
        }

        internal uint InsertOrUpdateMod(MaterialItemProjeto objUpdate)
        {
            if (objUpdate.IdMaterItemProj > 0)
                UpdateBase(objUpdate);
            else
                objUpdate.IdMaterItemProj = InsertBase(objUpdate);

            return objUpdate.IdMaterItemProj;
        }

        /// <summary>
        /// Atualiza o valor dos beneficiamentos.
        /// </summary>
        /// <param name="IdMaterItemProj">O id do material.</param>
        public void UpdateValorBenef(uint idMaterItemProj)
        {
            UpdateValorBenef(null, idMaterItemProj);
        }

        /// <summary>
        /// Atualiza o valor dos beneficiamentos.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="IdMaterItemProj">O id do material.</param>
        public void UpdateValorBenef(GDASession sessao, uint idMaterItemProj)
        {
            int idProd = ObtemValorCampo<int>(sessao, "idProd", "idMaterItemProj=" + idMaterItemProj);
            if (Glass.Configuracoes.Geral.NaoVendeVidro() || !ProdutoDAO.Instance.CalculaBeneficiamento(sessao, idProd))
                return;

            string sql = @"
                Update material_item_projeto set ValorBenef=(
                    select sum(coalesce(Valor, 0)) from material_projeto_benef where idMaterItemProj=?id) 
                        where idMaterItemProj=?id";

            objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?id", idMaterItemProj));

            // Recalcula o total bruto/valor unitário bruto
            MaterialItemProjeto mip = GetElementByPrimaryKey(sessao, idMaterItemProj);
            mip.RemoverDescontoQtde = true;
            UpdateBase(sessao, mip);
        }

        #region Insert

        public uint InsertBase(MaterialItemProjeto objInsert)
        {
            return InsertBase(null, objInsert);
        }

        public uint InsertBase(GDASession sessao, MaterialItemProjeto objInsert)
        {
            var itemProjeto = ItemProjetoDAO.Instance.GetElementByPrimaryKey(objInsert.IdItemProjeto);

            DescontoAcrescimo.Instance.DiferencaCliente(sessao, objInsert, (int?)itemProjeto.IdPedido, (int?)itemProjeto.IdProjeto, (int?)itemProjeto.IdOrcamento);
            DescontoAcrescimo.Instance.CalculaValorBruto(sessao, objInsert);
            
            uint retorno = base.Insert(sessao, objInsert);

            AtualizaBenef(sessao, retorno, objInsert.Beneficiamentos);

            ItemProjetoDAO.Instance.CalculoNaoConferido(sessao, objInsert.IdItemProjeto);
            return retorno;
        }

        /// <summary>
        /// Insert chamado ao incluir material no projeto
        /// </summary>
        /// <param name="objInsert"></param>
        /// <returns></returns>
        public uint InsertFromNovoItemProjeto(MaterialItemProjeto objInsert)
        {
            return InsertFromNovoItemProjeto(null, objInsert);
        }
        /// <summary>
        /// Insert chamado ao incluir material no projeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="objInsert"></param>
        /// <returns></returns>
        public uint InsertFromNovoItemProjeto(GDASession sessao, MaterialItemProjeto objInsert)
        {
            return InsertFromNovoItemProjeto(sessao, objInsert, null);
        }

        /// <summary>
        /// Insert chamado ao incluir material no projeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="objInsert"></param>
        /// <returns></returns>
        public uint InsertFromNovoItemProjeto(GDASession sessao, MaterialItemProjeto objInsert, uint? idCliente)
        {
            try
            {
                CalcTotais(sessao, ref objInsert, false, idCliente);
                var retorno = Insert(sessao, objInsert);

                AtualizaBenef(sessao, retorno, objInsert.Beneficiamentos);

                return retorno;
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao incluir Material. Erro: " + ex.Message);
            }
        }

        /// <summary>
        /// Atualiza o valor do item do projeto ao incluir um produto ao mesmo
        /// </summary>
        public override uint Insert(MaterialItemProjeto objInsert)
        {
            return Insert(null, objInsert);
        }

        /// <summary>
        /// Atualiza o valor do item do projeto ao incluir um produto ao mesmo
        /// </summary>
        public override uint Insert(GDASession session, MaterialItemProjeto objInsert)
        {
            uint returnValue = 0;
            
            try
            {
                var idProjeto = ItemProjetoDAO.Instance.GetIdProjeto(objInsert.IdItemProjeto);

                var projeto = ProjetoDAO.Instance.GetElement(idProjeto);

                if (projeto != null && projeto.FastDelivery)
                {
                    if (objInsert.IdAplicacao > 0)
                    {
                        var aplicacao = EtiquetaAplicacaoDAO.Instance.GetElementByPrimaryKey(objInsert.IdAplicacao.Value);

                        if (aplicacao.NaoPermitirFastDelivery)
                            throw new Exception(string.Format("Erro|O produto {0} tem a aplicacao {1} e esta aplicacao não permite fast delivery", objInsert.DescrProduto, aplicacao.CodInterno));
                    }
                }

                /* Chamado 42796. */
                if (objInsert.IdProd > 0 && objInsert.Espessura == 0 && ProdutoDAO.Instance.IsVidro(session, (int)objInsert.IdProd))
                    objInsert.Espessura = ProdutoDAO.Instance.ObtemEspessura(session, (int)objInsert.IdProd);

                CalcTotais(session, ref objInsert, false);
                returnValue = InsertBase(session, objInsert);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao incluir Material. Erro: " + ex.Message);
            }            

            return returnValue;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Atualiza o valor do item_projeto ao excluir um produto do mesmo
        /// </summary>
        public override int Delete(MaterialItemProjeto objDelete)
        {
            return Delete(null, objDelete);
        }

        /// <summary>
        /// Atualiza o valor do item_projeto ao excluir um produto do mesmo
        /// </summary>
        public override int Delete(GDASession session, MaterialItemProjeto objDelete)
        {
            if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From material_item_projeto Where IdMaterItemProj=" + objDelete.IdMaterItemProj) == 0)
                return 0;

            int returnValue = 0;

            try
            {
                uint idItemProjeto = ObtemValorCampo<uint>(session, "idItemProjeto", "idMaterItemProj=" + objDelete.IdMaterItemProj);

                objPersistence.ExecuteCommand(session, "Delete From material_projeto_benef where idMaterItemProj=" + objDelete.IdMaterItemProj);
                returnValue = base.Delete(session, objDelete);

                ItemProjetoDAO.Instance.CalculoNaoConferido(session, idItemProjeto);

                var idsProdPed = String.Join(",",
                ExecuteMultipleScalar<string>(session, "Select idProdPed From produtos_pedido Where idMaterItemProj=" + objDelete.IdMaterItemProj).ToArray());

                if (String.IsNullOrEmpty(idsProdPed))
                    idsProdPed = "0";

                // Exclui produtos que possam estar associados à este material no pedido
                objPersistence.ExecuteCommand(session, @"Delete From produto_pedido_benef Where idProdPed In (" + idsProdPed + ")");
                objPersistence.ExecuteCommand(session, "Delete From produtos_pedido Where idMaterItemProj=" + objDelete.IdMaterItemProj);

                // Exclui produtos que possam estar associados à este material no pedido espelho
                foreach (ProdutosPedidoEspelho p in ProdutosPedidoEspelhoDAO.Instance.GetByMaterItemProj(session, objDelete.IdMaterItemProj))
                    ProdutosPedidoEspelhoDAO.Instance.Delete(session, p);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao excluir Produto. Erro: " + ex.Message);
            }

            return returnValue;
        }

        #endregion

        #region Update

        public int UpdateFromNovoItemProjeto(MaterialItemProjeto objUpdate)
        {
            return UpdateFromNovoItemProjeto(null, objUpdate);
        }

        public int UpdateFromNovoItemProjeto(GDASession sessao, MaterialItemProjeto objUpdate)
        {
            return UpdateFromNovoItemProjeto(sessao, objUpdate, null);
        }

        public int UpdateFromNovoItemProjeto(GDASession sessao, MaterialItemProjeto objUpdate, uint? idCliente)
        {
            try
            {
                CalcTotais(sessao, ref objUpdate, false, idCliente);
                int retorno = UpdateBase(sessao, objUpdate);

                AtualizaBenef(sessao, objUpdate.IdMaterItemProj, objUpdate.Beneficiamentos);

                return retorno;
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao incluir Material. Erro: " + ex.Message);
            }
        }

        public int UpdateBase(MaterialItemProjeto objUpdate)
        {
            return UpdateBase(null, objUpdate);
        }

        public int UpdateBase(GDASession sessao, MaterialItemProjeto objUpdate)
        {
            var itemProjeto = ItemProjetoDAO.Instance.GetElementByPrimaryKey(objUpdate.IdItemProjeto);

            DescontoAcrescimo.Instance.DiferencaCliente(sessao, objUpdate, (int?)itemProjeto.IdPedido, (int?)itemProjeto.IdProjeto, (int?)itemProjeto.IdOrcamento);
            DescontoAcrescimo.Instance.CalculaValorBruto(sessao, objUpdate);
            
            ItemProjetoDAO.Instance.CalculoNaoConferido(sessao, objUpdate.IdItemProjeto);
            return base.Update(sessao, objUpdate);
        }

        public override int Update(MaterialItemProjeto objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Update(GDASession session, MaterialItemProjeto objUpdate)
        {
            try
            {
                var idProjeto = ItemProjetoDAO.Instance.GetIdProjeto(objUpdate.IdItemProjeto);

                var projeto = ProjetoDAO.Instance.GetElement(idProjeto);

                if (projeto != null && projeto.FastDelivery)
                {
                    if (objUpdate.IdAplicacao > 0)
                    {
                        var aplicacao = EtiquetaAplicacaoDAO.Instance.GetElementByPrimaryKey(objUpdate.IdAplicacao.Value);

                        if (aplicacao.NaoPermitirFastDelivery)
                            throw new Exception(string.Format("Erro|O produto {0} tem a aplicacao {1} e esta aplicacao não permite fast delivery", objUpdate.DescrProduto, aplicacao.CodInterno));
                    }
                }

                CalcTotais(session, ref objUpdate, false);
                UpdateBase(session, objUpdate);

                AtualizaBenef(session, objUpdate.IdMaterItemProj, objUpdate.Beneficiamentos);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao atualizar Produto do Pedido. Erro: " + ex.Message);
            }

            return 1;
        }

        #endregion

        #endregion
    }
}