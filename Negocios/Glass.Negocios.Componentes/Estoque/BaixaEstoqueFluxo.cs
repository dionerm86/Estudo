using System.Collections.Generic;
using System.Linq;

namespace Glass.Estoque.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de baixa do estoque.
    /// </summary>
    public class BaixaEstoqueFluxo : Entidades.IProdutoBaixaEstoqueRepositorio, Entidades.IProvedorProdutoBaixaEstoque
    {
        #region Membros de IProdutoBaixaEstoqueRepositorio

        /// <summary>
        /// Obtem os detalhes das baixas do estoque do produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        IEnumerable<Entidades.DetalhesBaixaEstoque> 
            Entidades.IProdutoBaixaEstoqueRepositorio.ObtemDetalhesBaixasEstoque(int idProd)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ProdutoBaixaEstoque>("pbe")
                .InnerJoin<Data.Model.Produto>("p.IdProd = pbe.IdProdBaixa", "p")
                .Select(@"pbe.IdProd, pbe.IdProdBaixa, pbe.Qtde, 
                         p.CodInterno AS CodInternoBaixa, p.Descricao AS DescricaoBaixa")
                .Where("pbe.IdProd = ?idProd")
                .Add("?idProd", idProd)
                .Execute<Entidades.DetalhesBaixaEstoque>()
                .ToList();
        }

        #endregion

        #region Membros de IProvedorProdutoBaixaEstoque

        /// <summary>
        /// Recupera a identificação associada com os dados informados.
        /// </summary>
        /// <param name="idProdBaixa"></param>
        /// <param name="qtde"></param>
        /// <returns></returns>
        string Entidades.IProvedorProdutoBaixaEstoque.ObtemIdentificacao(int idProdBaixa, float qtde)
        {
            var codInterno = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Produto>()
                .Where("IdProd=?id")
                .Add("?id", idProdBaixa)
                .Select("CodInterno")
                .Execute()
                .Select(f => f.GetString(0))
                .FirstOrDefault();

            return string.Format("Produto baixa: {0}, Qtde: {1}", codInterno, qtde);
        }

        #endregion
    }
}
