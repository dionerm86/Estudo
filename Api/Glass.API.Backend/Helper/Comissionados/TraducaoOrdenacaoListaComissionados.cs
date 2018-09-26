// <copyright file="TraducaoOrdenacaoListaComissionados.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Comissionados
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de comissionados.
    /// </summary>
    internal class TraducaoOrdenacaoListaComissionados : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaComissionados"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaComissionados(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdComissionado ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdComissionado";

                case "nome":
                    return "Nome";

                case "cpfcnpj":
                    return "CpfCnpj";

                case "rginscricaoestadual":
                    return "RgInscEst";

                case "telefoneresidencial":
                    return "telres";

                case "telefonecelular":
                    return "telcel";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
