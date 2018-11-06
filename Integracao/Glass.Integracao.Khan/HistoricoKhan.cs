// <copyright file="HistoricoKhan.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Integracao.Historico;

namespace Glass.Integracao.Khan
{
    /// <summary>
    /// Classe responável por fornecer os itens
    /// usados pelo histório de integração da Khan.
    /// </summary>
    internal static class HistoricoKhan
    {
        static HistoricoKhan()
        {
            var identificadoresProduto = new[]
            {
                new IdentificadorItemEsquema(nameof(Global.Negocios.Entidades.Produto.IdProd), typeof(int)),
            };

            Produtos = new ItemEsquema<Global.Negocios.Entidades.Produto>(
                (int)EsquemaHistoricoKhan.Produtos,
                "Produto",
                "Integração dos produtos",
                identificadoresProduto,
                (produto) => new object[] { produto.IdProd });

            var identificadoresNotaFiscal = new[]
            {
                new IdentificadorItemEsquema(nameof(Data.Model.NotaFiscal.IdNf), typeof(int)),
            };

            NotasFiscais = new ItemEsquema<Data.Model.NotaFiscal>(
                (int)EsquemaHistoricoKhan.NotasFiscais,
                "NotaFiscal",
                "Integração das notas fiscais",
                identificadoresNotaFiscal,
                (notaFiscal) => new object[] { notaFiscal.IdNf });
        }

        /// <summary>
        /// Obtém o histório dos produtos.
        /// </summary>
        public static ItemEsquema<Global.Negocios.Entidades.Produto> Produtos { get; }

        /// <summary>
        /// Obtém o histórico das notas fiscais.
        /// </summary>
        public static ItemEsquema<Data.Model.NotaFiscal> NotasFiscais { get; }
    }
}
