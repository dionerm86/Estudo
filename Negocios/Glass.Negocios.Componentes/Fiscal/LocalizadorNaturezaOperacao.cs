using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Fiscal.Negocios.Componentes
{
    /// <summary>
    /// Implementação do localizador de natureza de operação.
    /// </summary>
    public class LocalizadorNaturezaOperacao : ILocalizadorNaturezaOperacao
    {
        /// <summary>
        /// Localiza as naturezas de operação para os produtos informados,
        /// com base no cliente e loja.
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="loja"></param>
        /// <param name="produtos"></param>
        /// <returns>Relação da natureza de operação para cada produto.</returns>
        public IEnumerable<Entidades.NaturezaOperacao> Buscar(
            Global.Negocios.Entidades.Cliente cliente, Global.Negocios.Entidades.Loja loja,
            IEnumerable<Global.Negocios.Entidades.Produto> produtos)
        {
            var idsProduto = produtos.Select(f => f.IdProd);
            var produtosNaturezaOperacao = new Dictionary<int, int?>();

            using (var session = new GDA.GDASession())
            {
                // Carrega a relação dos produtos com a natureza de operação associada
                foreach (var i in idsProduto.Distinct()
                    .Select(idProd => new
                    {
                        IdProd = idProd,
                        IdNaturezaOperacao = (int?)Data.DAL.RegraNaturezaOperacaoDAO.Instance
                            .BuscaNaturezaOperacao(session, (uint?)loja?.IdLoja, (uint?)cliente?.IdCli, idProd)
                    }))
                    produtosNaturezaOperacao.Add(i.IdProd, i.IdNaturezaOperacao);
            }

            IEnumerable<Entidades.NaturezaOperacao> naturezasOperacao;

            var idsNaturezaOperacao = produtosNaturezaOperacao
                    .Where(f => f.Value.HasValue)
                    .Select(f => f.Value.Value)
                    .Distinct()
                    .ToList();

            if (idsNaturezaOperacao.Any())
                // Carrega os dados completos das naturezas de operação encontradas
                naturezasOperacao = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.NaturezaOperacao>()
                    .Where($"IdNaturezaOperacao IN ({string.Join(",", idsNaturezaOperacao)})")
                    .ProcessLazyResult<Entidades.NaturezaOperacao>()
                    .ToList();
            
            else
                naturezasOperacao = new Entidades.NaturezaOperacao[0];

            foreach(var idProd in idsProduto)
            {
                var idNaturezaOperacao = produtosNaturezaOperacao[idProd];
                if (idNaturezaOperacao.HasValue)
                    yield return naturezasOperacao.FirstOrDefault(f => f.IdNaturezaOperacao == idNaturezaOperacao.Value);
                else
                    yield return null;
            }
        }
    }
}
