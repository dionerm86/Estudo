// <copyright file="TraducaoOrdenacaoListaMovimentacoesLiberacoes.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Liberacoes.Movimentacoes
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de movimentações de liberações.
    /// </summary>
    internal class TraducaoOrdenacaoListaMovimentacoesLiberacoes : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaMovimentacoesLiberacoes"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaMovimentacoesLiberacoes(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdLiberarPedido DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdLiberarPedido";

                case "cliente":
                    return "Cliente";

                case "situacao":
                    return "Situacao";

                case "total":
                    return "Total";

                case "desconto":
                    return "Desconto";

                case "dinheiro":
                    return "Dinheiro";

                case "cheque":
                    return "Cheque";

                case "prazo":
                    return "Prazo";

                case "boleto":
                    return "Boleto";

                case "deposito":
                    return "Deposito";

                case "cartao":
                    return "Cartao";

                case "outros":
                    return "Outros";

                case "debito":
                    return "Debito";

                case "credito":
                    return "Credito";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
