// <copyright file="TraducaoOrdenacaoListaProprietariosDeVeiculos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.ConhecimentosTrasporte.Veiculos.Proprietarios
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de proprietários de veículos.
    /// </summary>
    internal class TraducaoOrdenacaoListaProprietariosDeVeiculos : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaProprietariosDeVeiculos"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaProprietariosDeVeiculos(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IDPROPVEIC ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IDPROPVEIC";

                case "nome":
                    return "Nome";

                case "rntrc":
                    return "RNTRC";

                case "ie":
                    return "IE";

                case "uf":
                    return "UF";

                case "tipo":
                    return "TipoProp";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}