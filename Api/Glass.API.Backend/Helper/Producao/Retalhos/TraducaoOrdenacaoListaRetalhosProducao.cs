// <copyright file="TraducaoOrdenacaoListaRetalhosProducao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Producao.Retalhos
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de retalhos de produção.
    /// </summary>
    internal class TraducaoOrdenacaoListaRetalhosProducao : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaRetalhosProducao"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaRetalhosProducao(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get
            {
                return "IdRetalhoProducao DESC";
            }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "codinterno":
                    return "CodInterno";

                case "descricao":
                    return "Descricao";

                case "largura":
                    return "Largura";

                case "altura":
                    return "Altura";

                case "datacad":
                    return "DataCad";

                case "situacao":
                    return "SituacaoString";

                case "datauso":
                    return "DataUso";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}