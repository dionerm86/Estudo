// <copyright file="TraducaoOrdenacaoListaSeguradoras.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Seguradoras
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de seguradoras.
    /// </summary>
    internal class TraducaoOrdenacaoListaSeguradoras : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaSeguradoras"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaSeguradoras(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "NomeSeguradora ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "nome":
                    return "NomeSeguradora";

                case "cnpj":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
