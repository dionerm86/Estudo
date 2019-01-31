// <copyright file="TraducaoOrdenacaoListaLimiteChequeCpfCnpj.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Cheques.LimitePorCpfCnpj
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de limites de cheques por cpf/cnpj.
    /// </summary>
    internal class TraducaoOrdenacaoListaLimiteChequeCpfCnpj : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaLimiteChequeCpfCnpj"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaLimiteChequeCpfCnpj(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "CpfCnpj ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "cpf/cnpj":
                    return "CpfCnpj";

                case "limite":
                    return "Limite";

                case "observacao":
                    return "Observacao";

                case "situacao":
                    return "situacao";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}