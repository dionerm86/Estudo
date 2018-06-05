using Glass.Global.Negocios.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios.Componentes
{
    /// <summary>
    /// Implementação do provedor de MVA do produto por UF.
    /// </summary>
    public class ProvedorMvaProdutoUf : Entidades.IProvedorMvaProdutoUf
    {
        /// <summary>
        /// Busca o valor do MVA por produto e UF dos produtos informados.
        /// </summary>
        /// <param name="produtos"></param>
        /// <param name="loja"></param>
        /// <param name="fornecedor"></param>
        /// <param name="cliente"></param>
        /// <param name="saida"></param>
        /// <returns></returns>
        public IEnumerable<float> ObterMvaPorProdutos(
            IEnumerable<Produto> produtos, Loja loja, Fornecedor fornecedor, Cliente cliente, bool saida)
        {
            var resultado = new List<float>();
            using (var sessao = new GDA.GDASession())
            {
                foreach (var produto in produtos)
                    resultado.Add(Data.DAL.MvaProdutoUfDAO.Instance
                        .ObterMvaPorProduto(sessao, produto.IdProd, (uint)loja.IdLoja, fornecedor?.IdFornec, (uint?)cliente?.IdCli, saida)); 
            }

            return resultado;
        }
    }
}
