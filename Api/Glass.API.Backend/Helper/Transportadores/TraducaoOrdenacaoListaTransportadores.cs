// <copyright file="TraducaoOrdenacaoListaTransportadores.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Transportadores
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de transportadores.
    /// </summary>
    internal class TraducaoOrdenacaoListaTransportadores : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaTransportadores"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaTransportadores(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdTransportador ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdTransportador";

                case "razaosocial":
                    return "Nome";

                case "nomefantasia":
                    return "NomeFantasia";

                case "cpfcnpj":
                    return "CpfCnpj";

                case "placa":
                    return "Placa";

                case "telefone":
                    return "Telefone";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
