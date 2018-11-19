// <copyright file="TraducaoOrdenacaoListaTrocasDevolucoes.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Estoques
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de Troca/Devolucao.
    /// </summary>
    internal class TraducaoOrdenacaoListaTrocasDevolucoes : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaTrocasDevolucoes"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaTrocasDevolucoes(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdCli ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdCli";

                case "ultcompra":
                    return "DtUltCompra";

                case "cpfcnpj":
                case "endereco":
                case "telcont":
                case "telcel":
                case "situacao":
                case "email":
                case "totalcomprado":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
