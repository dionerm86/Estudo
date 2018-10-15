// <copyright file="TraducaoOrdenacaoListaVeiculos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Veiculos
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de veículos.
    /// </summary>
    internal class TraducaoOrdenacaoListaVeiculos : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaVeiculos"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaVeiculos(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "Placa ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "placa":
                    return "Placa";

                case "modelo":
                    return "Modelo";

                case "anofabricacao":
                    return "Anofab";

                case "cor":
                    return "Cor";

                case "quilometrageminicial":
                    return "Kminicial";

                case "valoripva":
                    return "Valoripva";

                case "situacao":
                    return "Situacao";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
