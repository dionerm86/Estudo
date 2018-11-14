// <copyright file="TraducaoOrdenacaoListaContabilistas.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Contabilistas
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de contabilistas.
    /// </summary>
    internal class TraducaoOrdenacaoListaContabilistas : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaContabilistas"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaContabilistas(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "Nome ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "telefone":
                    return "TelCont";

                case "nome":
                case "cpfcnpj":
                case "crc":
                case "endereco":
                case "fax":
                case "email":
                case "situacao":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
