using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoBaixaEstoqueDAO : BaseDAO<ProdutoBaixaEstoque, ProdutoBaixaEstoqueDAO>
    {
        //private ProdutoBaixaEstoqueDAO() { }
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se um produto é matéria-prima de outro.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idProdMateriaPrima"></param>
        /// <returns></returns>
        public bool IsMateriaPrima(uint idProd, uint idProdMateriaPrima)
        {
            return IsMateriaPrima(null, idProd, idProdMateriaPrima);
        }

        /// <summary>
        /// Verifica se um produto é matéria-prima de outro.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idProdMateriaPrima"></param>
        /// <returns></returns>
        public bool IsMateriaPrima(GDASession sessao, uint idProd, uint idProdMateriaPrima)
        {
            string sql = "select count(*) from produto_baixa_estoque where idProd=" + idProd +
                " and idProdBaixa=" + idProdMateriaPrima;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se existe algum produto configurado para baixa no estoque.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool TemProdutoBaixa(GDASession sessao, uint idProd)
        {
            string sql = "select count(*) from produto_baixa_estoque where idProd=" + idProd;
            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se existe algum produto configurado para baixa no estoque.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool TemProdutoBaixa(uint idProd)
        {
            return TemProdutoBaixa(null, idProd);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca os produtos que serão usados para baixa de estoque.
        /// Retorna o próprio produto se não houver outro cadastrado.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public ProdutoBaixaEstoque[] GetByProd(uint idProd)
        {
            return GetByProd(null, idProd);
        }

        /// <summary>
        /// Busca os produtos que serão usados para baixa de estoque.
        /// Retorna o próprio produto se não houver outro cadastrado.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public ProdutoBaixaEstoque[] GetByProd(GDASession sessao, uint idProd)
        {
            return GetByProd(sessao, idProd, true, 0);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca os produtos que serão usados para baixa de estoque.
        /// Retorna o próprio produto se não houver outro cadastrado.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public ProdutoBaixaEstoque[] GetByProd(uint idProd, bool baixarProdutoSeNaoHouverProdBaixa)
        {
            return GetByProd(null, idProd, baixarProdutoSeNaoHouverProdBaixa);
        }

        /// <summary>
        /// Busca os produtos que serão usados para baixa de estoque.
        /// Retorna o próprio produto se não houver outro cadastrado.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public ProdutoBaixaEstoque[] GetByProd(GDASession sessao, uint idProd, bool baixarProdutoSeNaoHouverProdBaixa)
        {
            return GetByProd(sessao, idProd, baixarProdutoSeNaoHouverProdBaixa, 0);
        }

        public enum TipoBuscaProduto
        {
            Todos,
            ApenasProducao
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca os produtos que serão usados para baixa de estoque.
        /// Retorna o próprio produto se não houver outro cadastrado.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public ProdutoBaixaEstoque[] GetByProd(uint idProd, TipoBuscaProduto tipoBuscaProduto)
        {
            return GetByProd(null, idProd, tipoBuscaProduto);
        }

        /// <summary>
        /// Busca os produtos que serão usados para baixa de estoque.
        /// Retorna o próprio produto se não houver outro cadastrado.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public ProdutoBaixaEstoque[] GetByProd(GDASession sessao, uint idProd, TipoBuscaProduto tipoBuscaProduto)
        {
            return GetByProd(sessao, idProd, true, tipoBuscaProduto);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca os produtos que serão usados para baixa de estoque.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="baixarProdutoSeNaoHouverProdBaixa">O próprio produto deve ser usado se não houver cadastro?</param>
        /// <param name="apenasVidros">Buscar apenas vidros</param>
        /// <returns></returns>
        public ProdutoBaixaEstoque[] GetByProd(uint idProd, bool baixarProdutoSeNaoHouverProdBaixa, TipoBuscaProduto tipoBuscaProduto)
        {
            return GetByProd(null, idProd, baixarProdutoSeNaoHouverProdBaixa, tipoBuscaProduto);
        }

        /// <summary>
        /// Busca os produtos que serão usados para baixa de estoque.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="baixarProdutoSeNaoHouverProdBaixa">O próprio produto deve ser usado se não houver cadastro?</param>
        /// <param name="apenasVidros">Buscar apenas vidros</param>
        /// <returns></returns>
        public ProdutoBaixaEstoque[] GetByProd(GDASession sessao, uint idProd, bool baixarProdutoSeNaoHouverProdBaixa, TipoBuscaProduto tipoBuscaProduto)
        {
            string sql = @"
                SELECT pbe.* 
                FROM produto_baixa_estoque pbe
                    " + (tipoBuscaProduto == TipoBuscaProduto.ApenasProducao ? @"INNER JOIN produto p ON (pbe.idProdBaixa = p.idProd)
                    LEFT JOIN subgrupo_prod s ON (p.idSubgrupoProd=s.idSubgrupoProd)" : "") + @"
                WHERE pbe.idProd=" + idProd;

            if (tipoBuscaProduto == TipoBuscaProduto.ApenasProducao)
                sql += " AND ((p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + " AND p.tipoMercadoria=" +
                    (int)TipoMercadoria.MateriaPrima + ") OR s.tipoSubgrupo=" + (int)TipoSubgrupoProd.PVB + ")";

            List<ProdutoBaixaEstoque> retorno = objPersistence.LoadData(sessao, sql);

            if (baixarProdutoSeNaoHouverProdBaixa && retorno.Count == 0)
                retorno.Add(new ProdutoBaixaEstoque()
                {
                    IdProd = (int)idProd,
                    IdProdBaixa = (int)idProd,
                    Qtde = 1
                });

            return retorno.ToArray();
        }

        /// <summary>
        /// Apaga todos os produtos para baixa de um produto.
        /// </summary>
        /// <param name="idProd"></param>
        public void DeleteByProd(uint idProd)
        {
            DeleteByProd((GDASession)null, idProd);
        }

        /// <summary>
        /// Apaga todos os produtos para baixa de um produto.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idProd"></param>
        public void DeleteByProd(GDASession session, uint idProd)
        {
            objPersistence.ExecuteCommand(session, "delete from produto_baixa_estoque where idProd=" + idProd);
        }

        public override uint Insert(ProdutoBaixaEstoque objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, ProdutoBaixaEstoque objInsert)
        {
            if (objPersistence.ExecuteSqlQueryCount(session, @"select count(*) from produto_baixa_estoque
                where idProd=" + objInsert.IdProd + " and idProdBaixa=" + objInsert.IdProdBaixa) > 0)
                return 0;

            return objInsert.IdProd != objInsert.IdProdBaixa && objInsert.Qtde > 0 ? base.Insert(session, objInsert) : 0;
        }

        public override int Update(ProdutoBaixaEstoque objUpdate)
        {
            if (objPersistence.ExecuteSqlQueryCount(@"select count(*) from produto_baixa_estoque
                where idProd=" + objUpdate.IdProd + " and idProdBaixa=" + objUpdate.IdProdBaixa) > 0)
                return base.Delete(objUpdate);

            return objUpdate.IdProd != objUpdate.IdProdBaixa && objUpdate.Qtde > 0 ? base.Update(objUpdate) : base.Delete(objUpdate);
        }

        public uint ObterIdProdBaixa(uint idProd)
        {
            var pbe = GetByProd(idProd, true);
            return pbe.Length > 0 ? (uint)pbe[0].IdProdBaixa : 0;
        }
    }
}
