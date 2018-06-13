using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class ChapaCortePecaDAO : BaseDAO<ChapaCortePeca, ChapaCortePecaDAO>
    {
        //private ChapaCortePecaDAO() { }

        private int GetNumSeq(GDASession sessao, uint idProdImpressaoChapa, uint idProdImpressaoPeca)
        {
            string sql = @"select coalesce(max(numSeq),0)+1 from chapa_corte_peca
                where idProdImpressaoChapa=" + idProdImpressaoChapa + 
                " and idProdImpressaoPeca=" + idProdImpressaoPeca;

            return ExecuteScalar<int>(sessao, sql);
        }

        public override uint Insert(GDASession session, ChapaCortePeca objInsert)
        {
            MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaChapaCortePeca(session, (int)objInsert.IdProdImpressaoChapa, MovEstoque.TipoMovEnum.Saida);

            if (objInsert.IdProdImpressaoPeca > 0)
                MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaCorte(session, (int)objInsert.IdProdImpressaoPeca, MovEstoque.TipoMovEnum.Entrada);

            return base.Insert(session, objInsert);
        }

        /// <summary>
        /// Faz a ligação entre a matéria-prima e a peça.
        /// </summary>
        /// <param name="codChapa"></param>
        /// <param name="codEtiqueta"></param>
        /// <param name="salvarPlanoCorte"></param>
        /// <param name="saidaRevenda"></param>
        public void Inserir(GDASession sessao, string codChapa, string codEtiqueta, bool salvarPlanoCorte, bool saidaRevenda)
        {
            ChapaCortePeca nova = new ChapaCortePeca();

            ProdutoImpressaoDAO.TipoEtiqueta tipoEtiquetaChapa = ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(codChapa);

            uint idProdImpressaoChapa = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, codChapa, tipoEtiquetaChapa);
            bool chapaPossiuLeitura = ChapaPossuiLeitura(sessao, idProdImpressaoChapa);

            nova.IdProdImpressaoChapa = idProdImpressaoChapa;
            nova.IdProdImpressaoPeca = !string.IsNullOrEmpty(codEtiqueta) ? ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, codEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido) : 0;
            nova.NumSeq = GetNumSeq(sessao, nova.IdProdImpressaoChapa, nova.IdProdImpressaoPeca);
            nova.PlanoCorte = !salvarPlanoCorte ? null : ProdutoImpressaoDAO.Instance.ObtemValorCampo<string>(sessao, "planoCorte", "idProdImpressao=" + nova.IdProdImpressaoPeca);
            nova.DataCad = DateTime.Now;
            nova.SaidaRevenda = saidaRevenda;

            var idChapaCortePeca = Insert(sessao, nova);

            if (tipoEtiquetaChapa == ProdutoImpressaoDAO.TipoEtiqueta.Retalho && !saidaRevenda)
            {
                uint idRetalhoProducao = ProdutoImpressaoDAO.Instance.ObtemValorCampo<uint>(sessao, "idRetalhoProducao", "idProdImpressao=" + nova.IdProdImpressaoChapa);

                if (idRetalhoProducao > 0)
                {
                    var situacaoRetalho = RetalhoProducaoDAO.Instance.ObtemSituacao(sessao, idRetalhoProducao);
                    var idProdRetalho = RetalhoProducaoDAO.Instance.ObtemValorCampo<uint>(sessao, "IdProd", "IdRetalhoProducao=" + idRetalhoProducao);

                    if (situacaoRetalho != SituacaoRetalhoProducao.Cancelado)
                    {
                        var idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(sessao, codEtiqueta);

                        RetalhoProducaoDAO.Instance.AlteraSituacao(sessao, idRetalhoProducao, SituacaoRetalhoProducao.EmUso);
                        if (!UsoRetalhoProducaoDAO.Instance.PossuiAssociacao(sessao, idRetalhoProducao, idProdPedProducao.GetValueOrDefault(0)))
                            UsoRetalhoProducaoDAO.Instance.AssociarRetalho(sessao, idRetalhoProducao, idProdPedProducao.GetValueOrDefault(0), false);
                    }

                    if (!chapaPossiuLeitura)
                        MovEstoqueDAO.Instance.BaixaEstoqueRetalho(sessao, idProdRetalho, UserInfo.GetUserInfo.IdLoja, idRetalhoProducao, 1);
                }
            }
            else if (tipoEtiquetaChapa != ProdutoImpressaoDAO.TipoEtiqueta.Retalho && !chapaPossiuLeitura && !saidaRevenda)
            {
                uint? idProdPedProd = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(sessao, codEtiqueta);
                uint? idProd = ProdutoImpressaoDAO.Instance.GetIdProd(sessao, idProdImpressaoChapa);
                uint? idNf = ProdutoImpressaoDAO.Instance.ObtemIdNf(sessao, idProdImpressaoChapa);

                uint? idLojaMovEstoque = (uint?)objPersistence.ExecuteScalar(sessao,
                    string.Format("SELECT idLoja FROM mov_estoque WHERE idNf={0} AND idProd={1} AND tipoMov={2} order by idmovestoque desc limit 1",
                                        idNf.GetValueOrDefault(0), idProd.GetValueOrDefault(), (int)MovEstoque.TipoMovEnum.Entrada));

                var idLojaFuncionario = UserInfo.GetUserInfo.IdLoja;
                var idLojaNf = NotaFiscalDAO.Instance.ObtemIdLoja(sessao, idNf.GetValueOrDefault());

                var idLoja = idLojaMovEstoque ?? (idLojaNf == 0 ? idLojaFuncionario : idLojaNf);

                MovEstoqueDAO.Instance.BaixaEstoqueProducao(sessao, idProd.Value, idLoja, idProdPedProd.Value, 1, 0, false, false, false);
            }
        }

        /// <summary>
        /// Faz a ligação entre a matéria-prima e a peça.
        /// </summary>
        /// <param name="codChapa"></param>
        /// <param name="codEtiqueta"></param>
        public void Inserir(GDASession sessao, string codEtiqueta, List<string> MateriasPrimas)
        {
            ChapaCortePeca nova = new ChapaCortePeca();
            nova.IdProdImpressaoPeca = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, codEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);
            nova.DataCad = DateTime.Now;

            foreach (var mp in MateriasPrimas)
            {
                var idProdImpressao = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, mp, ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(mp));

                nova.IdProdImpressaoChapa = idProdImpressao;
                nova.NumSeq = GetNumSeq(sessao, idProdImpressao, nova.IdProdImpressaoPeca);
                Insert(sessao, nova);
            }
        }

        /// <summary>
        /// Verifica se a chapa ja possui um plano de corte vinculado a ela.
        /// </summary>
        /// <param name="codChapa"></param>
        /// <returns></returns>
        public bool ChapaPossuiPlanoCorteVinculado(GDASession sessao, string codChapa)
        {
            uint idProdImpressaoChapa = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, codChapa, ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(codChapa));

            string sql = @"SELECT COUNT(*) FROM chapa_corte_peca WHERE IdProdImpressaoChapa=" + idProdImpressaoChapa+
                " AND planocorte IS NOT NULL";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se a chapa ja possui um plano de corte vinculado a ela.
        /// </summary>
        /// <param name="codChapa"></param>
        /// <returns></returns>
        public bool ChapaPossuiPlanoCorteVinculado(string codChapa)
        {
            return ChapaPossuiPlanoCorteVinculado(null, codChapa);
        }

        /// <summary>
        /// Verifica se a chapa informada deu saida no sistema atraves de um pedido de revenda.
        /// </summary>
        /// <param name="codChapa"></param>
        /// <returns></returns>
        public bool ChapaDeuSaidaEmPedidoRevenda(string codChapa)
        {
            /* Chamado 63119. */
            if (codChapa == "N0-0.0/0")
                return false;

            uint idProdImpressaoChapa = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(codChapa,
                ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(codChapa));

            string sql = @"SELECT COUNT(*) FROM chapa_corte_peca WHERE IdProdImpressaoChapa=" + idProdImpressaoChapa +
               " AND COALESCE(SaidaRevenda, 0) = 1";

            return objPersistence.ExecuteSqlQueryCount(sql) > 0 && !ChapaTrocadaDevolvidaDAO.Instance.VerificarChapaDisponivel(null, codChapa);
        }
 
         /// <summary>
        /// Obtém o plano de corte associado à chapa
        /// </summary>
        /// <param name="codChapa"></param>
        /// <returns></returns>
        public string ObtemPlanoCorteVinculado(string codChapa)
        {
            uint idProdImpressaoChapa = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(codChapa,
                ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(codChapa));

            return ObtemValorCampo<string>("planocorte", "IdProdImpressaoChapa=" + idProdImpressaoChapa + " AND planocorte IS NOT NULL");
        }

        public bool ValidarChapa(GDASession sessao, Produto produto)
        {
            //Verifica se a etiqueta já foi utilizada
            return ProducaoConfig.BloquearLeituraPVBDuplicada || produto == null ||
                SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(produto.IdProd) != TipoSubgrupoProd.PVB;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se já foi realizada a leitura da chapa
        /// </summary>
        public bool ChapaPossuiLeitura(uint idProdImpressaoChapa)
        {
            return ChapaPossuiLeitura(null, idProdImpressaoChapa);
        }

        /// <summary>
        /// Verifica se já foi efetuada a leitura das chapas.
        /// </summary>
        public bool ChapaPossuiLeitura(GDASession sessao, uint idsProdImpressaoChapa)
        {
            return ChapasPossuemLeitura(sessao, new List<int>() { (int)idsProdImpressaoChapa });
        }

        /// <summary>
        /// Verifica se já foi efetuada a leitura das chapas.
        /// </summary>
        public bool ChapasPossuemLeitura(GDASession sessao, IList<int> idsProdImpressaoChapa)
        {
            /* Chamado 65031. */
            if (idsProdImpressaoChapa == null || idsProdImpressaoChapa.Count == 0 || !idsProdImpressaoChapa.Any(f => f > 0))
                return false;

            var chapasTrocadasDisponiveis = ChapaTrocadaDevolvidaDAO.Instance.VerificarChapaDisponivel(sessao, idsProdImpressaoChapa);

            return ExecuteScalar<bool>(sessao, string.Format(@"SELECT COUNT(*)>0 FROM chapa_corte_peca ccp
                    INNER JOIN produto_impressao pi ON (ccp.IdProdImpressaoChapa=pi.IdProdImpressao)
                    LEFT JOIN produtos_nf pnf ON (pi.IdProdNf=pnf.IdProdNf)
                    LEFT JOIN produtos_pedido_espelho ppe on (ppe.IdProdPed = pi.IdProdPed)
                    LEFT JOIN pedido ped on (ped.IdPedido = ppe.IdPedido) And (ped.TipoPedido = {0})
                    INNER JOIN produto p ON (coalesce(pnf.IdProd,ppe.IdProd)=p.IdProd)
                    INNER JOIN subgrupo_prod sp ON (p.IdSubgrupoProd=sp.IdSubgrupoProd)
                WHERE ccp.IdProdImpressaoChapa IN ({1}) AND sp.TipoSubgrupo IN {2}", (int)Pedido.TipoPedidoEnum.Producao,
                            string.Join(",", idsProdImpressaoChapa.Where(f => f > 0)), string.Format("({0}, {1})", (int)TipoSubgrupoProd.ChapasVidro, (int)TipoSubgrupoProd.ChapasVidroLaminado))) && !chapasTrocadasDisponiveis;
        }

        /// <summary>
        /// Retornar a quantidade de leitura da chapa nos pedidos de produção vinculado ao pedido de revenda passado
        /// </summary>
        /// <param name="idProdImpressaoChapa"></param>
        /// <param name="idPedidoRevenda"></param>
        /// <returns></returns>
        public int QtdeLeituraChapaPedidoRevenda(GDASession sessao, uint idProdImpressaoChapa, uint idPedidoRevenda)
        {
            string sql = string.Format(@"SELECT COUNT(*) FROM chapa_corte_peca WHERE idProdIMpressaoChapa = {0} AND idProdImpressaoPeca IN(SELECT idprodimpressao
                FROM produto_impressao WHERE idpedido IN(SELECT idpedido FROM pedido WHERE idpedidorevenda = {1}))", idProdImpressaoChapa, idPedidoRevenda);

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) ;
        }

        /// <summary>
        /// Verifica se já foi realizado a leitura da chapa
        /// </summary>
        /// <param name="codChapa"></param>
        /// <returns></returns>
        public bool ChapaPossuiLeitura(GDASession sessao, string codChapa)
        {
            uint idProdImpressaoChapa = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, codChapa, ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(codChapa));

            return ChapaPossuiLeitura(sessao, idProdImpressaoChapa);
        }

        /// <summary>
        /// Verifica se já foi realizado a leitura da chapa
        /// </summary>
        /// <param name="codChapa"></param>
        /// <returns></returns>
        public bool ChapaPossuiLeitura(string codChapa)
        {
            return ChapaPossuiLeitura(null, codChapa);
        }

        /// <summary>
        /// Verifica se uma impressão de chapa ja teve leitura
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <returns></returns>
        public bool ImpressaoChapaPossuiLeitura(uint idImpressao, uint numeroNFe)
        {
            string sql = @"
                SELECT count(*)
                FROM chapa_corte_peca ccp
                    INNER JOIN produto_impressao pi ON (ccp.idprodimpressaochapa = pi.idprodimpressao)
                    LEFT JOIN nota_fiscal nf ON (pi.idNf=nf.idNf)
                WHERE pi.idimpressao = " + idImpressao;

            if (numeroNFe > 0)
                sql += " AND nf.numeroNFe=" + numeroNFe;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Remove a leitura da chapa da peça informada
        /// </summary>
        public void DeleteByIdProdImpressaoPeca(GDASession sessao, uint idProdImpressaoPeca, uint idProdPedProducao)
        {
            var idProdImpressaoChapa = ObtemIdProdImpressaoChapa(sessao, (int)idProdImpressaoPeca);
            var numEtiquetaChapa = ProdutoImpressaoDAO.Instance.ObtemNumEtiqueta(sessao, (uint)idProdImpressaoChapa);
            // Obtém a movimentação de estoque associada ao produto de produção.
            var idsMovEstoque = MovEstoqueDAO.Instance.ObtemMovEstoqueChapaCortePeca(sessao, idProdPedProducao, numEtiquetaChapa);

            #region Associa a movimentação de estoque da chapa à outro produto de produção

            /* Chamado 58239. */
            if (idProdImpressaoChapa > 0 && idsMovEstoque.Count > 0)
            {
                // Caso o produto esteja associado à movimentação de estoque e a chapa tenha sido recuperada, troca a referência de produto de produção na movimentação de estoque,
                // para que ela não fique sem referência. Portanto, a movimentação de estoque irá ficar sem referência somente se a última peça for estornada. */
                if (QtdeLeiturasChapa(sessao, idProdImpressaoPeca) > 1)
                {
                    // Produto de impressão do pedido associado ao produto de impressão da nota fiscal.
                    var idProdImpressaoPecaAssociarMovEstoque = ObtemValorCampo<uint?>(sessao, "IdProdImpressaoPeca",
                        string.Format("IdProdImpressaoChapa={0} AND IdProdImpressaoPeca<>{1}", idProdImpressaoChapa, idProdImpressaoPeca));

                    if (idProdImpressaoPecaAssociarMovEstoque > 0)
                    {
                        // Número da etiqueta da peça do pedido associada à etiqueta da chapa (etiqueta da peça da nota fiscal).
                        var numEtiquetaPecaAssociarMovEstoque = ProdutoImpressaoDAO.Instance.ObtemNumEtiqueta(sessao, idProdImpressaoPecaAssociarMovEstoque.Value);

                        if (!string.IsNullOrWhiteSpace(numEtiquetaPecaAssociarMovEstoque))
                        {
                            // ID do produto de produção, da etiqueta que está associada à chapa.
                            var idProdPedProducaoAssiciarMovEstoque = ExecuteScalar<int?>(sessao, @"SELECT IdProdPedProducao FROM produto_pedido_producao WHERE NumEtiqueta=?numEtiqueta",
                                new GDAParameter("?numEtiqueta", numEtiquetaPecaAssociarMovEstoque));

                            if (idProdPedProducaoAssiciarMovEstoque > 0)
                                // Associa a movimentação de estoque ao novo produto de produção.
                                objPersistence.ExecuteCommand(sessao, string.Format("UPDATE mov_estoque SET IdProdPedProducao={0} WHERE IdMovEstoque in ({1})", idProdPedProducaoAssiciarMovEstoque.Value,
                                    string.Join(",", idsMovEstoque)));
                        }
                    }
                }
                // Na movimentação de estoque, salva no campo OBS o número da etiqueta da chapa. Pois, a referência da chapa é recuperada através do produto de produção,
                // caso ele seja apagado ou seja associado à outra chapa, esta movimentação ficará com a referência incorreta.
                else
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE mov_estoque SET IdProdPedProducao=NULL, Obs=?obs WHERE IdMovEstoque in ({0})", string.Join(",", idsMovEstoque)),
                        new GDAParameter("?obs", string.Format("Etiqueta: {0}|{1}.", numEtiquetaChapa, ObtemPlanoCorteVinculado(numEtiquetaChapa) ?? ProdutoImpressaoDAO.Instance.ObtemNumEtiqueta(idProdPedProducao) ?? "").Replace("|.", ".")));
            }

            #endregion
        }

        /// <summary>
        /// Remove a leitura da chapa das peças informadas.
        /// </summary>
        public void DeleteByIdsProdImpressaoPeca(GDASession session, List<int> idsProdImpressaoPeca)
        {
            if (idsProdImpressaoPeca != null && idsProdImpressaoPeca.Count > 0)
            {
                var ids = string.Join(",", idsProdImpressaoPeca);

                var idsProdImpressaoChapa = ObtemIdProdImpressaoChapa(session, ids);

                objPersistence.ExecuteCommand(session, string.Format("DELETE FROM chapa_corte_peca WHERE IdProdImpressaoPeca IN ({0})", ids));

                foreach (var id in idsProdImpressaoChapa)
                    MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaChapaCortePeca(session, id, MovEstoque.TipoMovEnum.Entrada);

                foreach (var id in idsProdImpressaoPeca)
                    MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaChapaCortePeca(session, id, MovEstoque.TipoMovEnum.Saida);
            }
        }

        /// <summary>
        /// Remove as leituras da chapa informada
        /// </summary>
        public void DeleteByIdProdImpressaoChapa(GDASession sessao, uint idProdImpressaoChapa)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM chapa_corte_peca WHERE SaidaRevenda = 1 AND idProdImpressaoChapa=" + idProdImpressaoChapa);

            MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaChapaCortePeca(sessao, (int)idProdImpressaoChapa, MovEstoque.TipoMovEnum.Entrada);
        }

        /// <summary>
        /// Retorna quantas leituras uma chapa possiu passando uma peça lida
        /// </summary>
        /// <param name="idProdImpressaoPeca"></param>
        /// <returns></returns>
        public int QtdeLeiturasChapa(GDASession sessao, uint idProdImpressaoPeca)
        {
            uint idProdImpressaoChapa = ObtemValorCampo<uint>(sessao, "idProdImpressaoChapa", "idProdImpressaoPeca=" + idProdImpressaoPeca);

            return objPersistence.ExecuteSqlQueryCount(sessao, "SELECT COUNT(*) FROM chapa_corte_peca WHERE idProdImpressaoChapa=" + idProdImpressaoChapa);
        }

        /// <summary>
        /// Busca os dias em que uma chapa foi lida
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdImpressaoChapa"></param>
        /// <returns></returns>
        public List<DateTime> ObtemDiasLeitura(GDASession sessao, uint idProdImpressaoChapa)
        {
            string sql = @"
                SELECT date(dataCad)
                FROM chapa_corte_peca
                WHERE idProdImpressaoChapa = " + idProdImpressaoChapa + @"
                GROUP BY date(dataCad)";

            return ExecuteMultipleScalar<DateTime>(sql);
        }

        /// <summary>
        /// Busca os dias em que uma chapa foi lida
        /// </summary>
        /// <param name="codChapa"></param>
        /// <returns></returns>
        public List<DateTime> ObtemDiasLeitura(GDASession sessao, string codChapa)
        {
            uint idProdImpressaoChapa = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, codChapa, ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(codChapa));

            return ObtemDiasLeitura(sessao, idProdImpressaoChapa);
        }

        /// <summary>
        /// Busca os dias em que uma chapa foi lida
        /// </summary>
        /// <param name="codChapa"></param>
        /// <returns></returns>
        public List<DateTime> ObtemDiasLeitura(string codChapa)
        {
            return ObtemDiasLeitura(null, codChapa);
        }

        /// <summary>
        /// Busca a chapa vinculada a peça informada
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdImpressaoPeca"></param>
        /// <returns></returns>
        public int ObtemIdProdImpressaoChapa(GDASession sessao, int idProdImpressaoPeca)
        {
            return ObtemValorCampo<int>(sessao, "IdProdImpressaoChapa", "IdProdImpressaoPeca = " + idProdImpressaoPeca);
        }

        /// <summary>
        /// Busca a chapa vinculada a peça informada
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdImpressaoPeca"></param>
        /// <returns></returns>
        public List<int> ObtemIdProdImpressaoChapa(GDASession sessao, string idsProdImpressaoPeca)
        {
            return ExecuteMultipleScalar<int>(sessao, "SELECT DISTINCT IdProdImpressaoChapa FROM chapa_corte_peca WHERE IdProdImpressaoPeca IN(" + idsProdImpressaoPeca + ")");
        }
    }
}
