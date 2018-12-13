using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoBaixaEstoqueFiscalDAO : BaseDAO<ProdutoBaixaEstoqueFiscal, ProdutoBaixaEstoqueFiscalDAO>
    {
        //private ProdutoBaixaEstoqueFiscalDAO() { }

        #region Obtém os produtos de baixa fiscal de um determinado produto

        /// <summary>
        /// Busca os produtos que serão usados para baixa de estoque.
        /// Retorna o próprio produto se não houver outro cadastrado.
        /// </summary>
        public ProdutoBaixaEstoqueFiscal[] GetByProd(uint idProd)
        {
            return GetByProd(null, idProd);
        }

        /// <summary>
        /// Busca os produtos que serão usados para baixa de estoque.
        /// Retorna o próprio produto se não houver outro cadastrado.
        /// </summary>
        public ProdutoBaixaEstoqueFiscal[] GetByProd(GDASession session, uint idProd)
        {
            return GetByProd(session, idProd, true);
        }

        #endregion

        #region Verifica se um produto é matéria-prima de outro

        /// <summary>
        /// Verifica se um produto é matéria-prima de outro.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idProdMateriaPrima"></param>
        /// <returns></returns>
        public bool IsMateriaPrima(uint idProd, uint idProdMateriaPrima)
        {
            string sql = "Select Count(*) From produto_baixa_estoque_fiscal Where idProd=" + idProd +
                " And idProdBaixa=" + idProdMateriaPrima;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Verifica se o produto e a matéria-prima possuem matérias-primas iguais

        /// <summary>
        /// Verifica se o produto e a matéria-prima possuem matérias-primas iguais
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool SameMateriaPrima(uint idProd, uint idProdMateriaPrima)
        {
            string sql = @"
                Select Count(*)
                From produto_baixa_estoque_fiscal
                Where idProd=" + idProd + @" And idProdBaixa In
                    (Select idProdBaixa
                    From produto_baixa_estoque_fiscal
                    Where idProd =" + idProdMateriaPrima + ")";

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Verifica se existe algum produto configurado para baixa no estoque

        /// <summary>
        /// Verifica se existe algum produto configurado para baixa no estoque.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool TemProdutoBaixa(uint idProd)
        {
            string sql = "Select Count(*) From produto_baixa_estoque_fiscal Where idProd=" + idProd;
            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Busca os produtos que serão usados para baixa de estoque

        /// <summary>
        /// Busca os produtos que serão usados para baixa de estoque.
        /// </summary>
        public ProdutoBaixaEstoqueFiscal[] GetByProd(uint idProd, bool baixarProdutoSeNaoHouverProdBaixa)
        {
            return GetByProd(null, idProd, baixarProdutoSeNaoHouverProdBaixa);
        }

        /// <summary>
        /// Busca os produtos que serão usados para baixa de estoque.
        /// </summary>
        public ProdutoBaixaEstoqueFiscal[] GetByProd(GDASession session, uint idProd, bool baixarProdutoSeNaoHouverProdBaixa)
        {
            string sql = "Select * From produto_baixa_estoque_fiscal Where idProd=" + idProd;
            List<ProdutoBaixaEstoqueFiscal> retorno = objPersistence.LoadData(session, sql).ToList();

            if (baixarProdutoSeNaoHouverProdBaixa && retorno.Count == 0)
                retorno.Add(new ProdutoBaixaEstoqueFiscal()
                {
                    IdProd = (int)idProd,
                    IdProdBaixa = (int)idProd,
                    Qtde = 1
                });

            return retorno.ToArray();
        }

        #endregion

        #region Busca EFD

        /// <summary>
        /// Obtém os produtos de baixa de estoque fiscal, do produto informado.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProd">idProd.</param>
        /// <returns>Retorna os produtos de baixa de estoque fiscal, do produto informado.</returns>
        public List<ProdutoBaixaEstoqueFiscal> ObterParaItemProduzidoEfd(GDASession session, int idProd)
        {
            if (idProd == 0)
            {
                return new List<ProdutoBaixaEstoqueFiscal>();
            }

            return this.objPersistence.LoadData(session, $"SELECT * FROM produto_baixa_estoque_fiscal WHERE IdProd = {idProd};");
        }

        #endregion

        #region Obtém dados da baixa fiscal

        public uint ObterIdProdBaixa(uint idProd)
        {
            var pbe = GetByProd(idProd, true);
            return pbe.Length > 0 ? (uint)pbe[0].IdProdBaixa : 0;
        }

        #endregion

        #region Métodos Sobescritos

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
            objPersistence.ExecuteCommand(session, "Delete From produto_baixa_estoque_fiscal Where idProd=" + idProd);
        }

        public override uint Insert(ProdutoBaixaEstoqueFiscal objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, ProdutoBaixaEstoqueFiscal objInsert)
        {
            if (objPersistence.ExecuteSqlQueryCount(session, @"Select Count(*) From produto_baixa_estoque_fiscal
                Where idProd=" + objInsert.IdProd + " And idProdBaixa=" + objInsert.IdProdBaixa) > 0)
                return 0;

            return objInsert.IdProd != objInsert.IdProdBaixa && objInsert.Qtde > 0 ? base.Insert(session, objInsert) : 0;
        }

        public override int Update(ProdutoBaixaEstoqueFiscal objUpdate)
        {
            if (objPersistence.ExecuteSqlQueryCount(@"Select Count(*) From produto_baixa_estoque_fiscal
                Where idProd=" + objUpdate.IdProd + " And idProdBaixa=" + objUpdate.IdProdBaixa) > 0)
                return base.Delete(objUpdate);

            return objUpdate.IdProd != objUpdate.IdProdBaixa && objUpdate.Qtde > 0 ? base.Update(objUpdate) : base.Delete(objUpdate);
        }

        #endregion
    }
}
