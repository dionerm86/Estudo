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
            get { return "IdLiberarPedido desc"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdLiberarPedido";

                case "cliente":
                    return "NomeCliente";

                case "situacao":
                case "total":
                case "desconto":
                case "dinheiro":
                case "cheque":
                case "prazo":
                case "boleto":
                case "deposito":
                case "cartao":
                case "outros":
                case "debito":
                case "credito":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
