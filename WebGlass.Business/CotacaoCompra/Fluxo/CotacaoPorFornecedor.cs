using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.CotacaoCompra.Fluxo
{
    public sealed class CotacaoPorFornecedor : BaseFluxo<CotacaoPorFornecedor>
    {
        private CotacaoPorFornecedor() { }

        /// <summary>
        /// Busca os dados para o relatório de cotação por fornecedor.
        /// </summary>
        /// <param name="codigoCotacaoCompra"></param>
        /// <param name="apenasFornecedoresCadastrados"></param>
        /// <returns></returns>
        public Entidade.CotacaoCompraCalculada[] ObtemDados(uint codigoCotacaoCompra, bool apenasFornecedoresCadastrados)
        {
            // Recupera a cotação de compra
            var cotacaoCompra = CotacaoCompraDAO.Instance.GetElementByPrimaryKey(codigoCotacaoCompra);
            
            // Recupera os fornecedores da cotação de compra
            var fornecedores = ProdutoFornecedorCotacaoCompraDAO.Instance.ObtemProdutosFornecedorCotacao(null, codigoCotacaoCompra, 
                0, 0, apenasFornecedoresCadastrados);

            // Recupera os produtos da cotação
            var produtos = ProdutoCotacaoCompraDAO.Instance.ObtemProdutos(null, codigoCotacaoCompra);

            // Cria a lista de retorno
            List<Entidade.CotacaoCompraCalculada> retorno = new List<Entidade.CotacaoCompraCalculada>();

            foreach (var f in fornecedores)
                foreach (var p in produtos.Where(x => x.IdProd == f.IdProd))
                    retorno.Add(new Entidade.CotacaoCompraCalculada(cotacaoCompra, p, f));

            // Retorna os itens encontrados
            return retorno.ToArray();
        }
    }
}
