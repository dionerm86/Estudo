using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;

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

                    if (situacaoRetalho != RetalhoProducao.SituacaoRetalho.Cancelado)
                    {
                        var idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(sessao, codEtiqueta);

                        RetalhoProducaoDAO.Instance.AlteraSituacao(sessao, idRetalhoProducao, RetalhoProducao.SituacaoRetalho.EmUso);
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
                uint idLoja = 0;

                if (idNf > 0 && Geral.ConsiderarLojaClientePedidoFluxoSistema)
                {
                    var idLojaNf = NotaFiscalDAO.Instance.ObtemIdLoja(sessao, idNf.Value);

                    if (idLojaNf > 0)
                        idLoja = idLojaNf;
                }

                if (idLoja == 0)
                    idLoja = UserInfo.GetUserInfo.IdLoja;

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
            uint idProdImpressaoChapa = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(codChapa,
                ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(codChapa));

            string sql = @"SELECT COUNT(*) FROM chapa_corte_peca WHERE IdProdImpressaoChapa=" + idProdImpressaoChapa +
               " AND COALESCE(SaidaRevenda, 0) = 1";

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
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
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se já foi realizada a leitura da chapa
        /// </summary>
        /// <param name="idProdImpressaoChapa"></param>
        /// <returns></returns>
        public bool ChapaPossuiLeitura(uint idProdImpressaoChapa)
        {
            return ChapaPossuiLeitura(null, idProdImpressaoChapa);
        }

        public bool ValidarChapa(GDASession sessao, Produto produto)
        {
            //Verifica se a etiqueta já foi utilizada
            return ProducaoConfig.BloquearLeituraPVBDuplicada || produto == null ||
                SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(produto.IdProd) != TipoSubgrupoProd.PVB;
        }

        /// <summary>
        /// Verifica se já foi realizado a leitura da chapa
        /// </summary>
        /// <param name="idProdImpressaoChapa"></param>
        /// <returns></returns>
        public bool ChapaPossuiLeitura(GDASession sessao, uint idProdImpressaoChapa)
        {
            string sql = @"select count(*) from chapa_corte_peca where idProdImpressaoChapa = " + idProdImpressaoChapa;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
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
        public void DeleteByIdProdImpressaoPeca(GDASession sessao, uint idProdImpressaoPeca)
        {
            var idProdImpressaoChapa = ObtemIdProdImpressaoChapa(sessao, (int)idProdImpressaoPeca);

            DeleteByIdsProdImpressaoPeca(sessao, new List<int> { (int)idProdImpressaoPeca });

            MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaChapaCortePeca(sessao, idProdImpressaoChapa, MovEstoque.TipoMovEnum.Entrada);
            MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaCorte(sessao, (int)idProdImpressaoPeca, MovEstoque.TipoMovEnum.Saida);
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
