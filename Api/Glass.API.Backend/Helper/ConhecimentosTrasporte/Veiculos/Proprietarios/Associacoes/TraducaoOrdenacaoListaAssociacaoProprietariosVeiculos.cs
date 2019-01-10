// <copyright file="TraducaoOrdenacaoListaAssociacaoProprietariosVeiculos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.ConhecimentosTrasporte.Veiculos.Proprietarios.Associacoes
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de associação de proprietários com veículos.
    /// </summary>
    internal class TraducaoOrdenacaoListaAssociacaoProprietariosVeiculos : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaAssociacaoProprietariosVeiculos"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaAssociacaoProprietariosVeiculos(string ordenacao)
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
                case "nome":
                    return "Nome";

                case "placa":
                    return "Placa";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}